using EVRC.Core;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using EVRC.DesktopUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Unity.Plastic.Antlr3.Runtime;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class BindingItemsListController : PreCheck
    {
        [Header("Templates and Scene Refs")]
        // UXML template for list entries
        [SerializeField] UIDocument parentUIDocument;
        [SerializeField] VisualTreeAsset m_BindingEntryTemplate;
        [SerializeField] ControlBindingsState bindings;

        [SerializeField] VisualTreeAsset m_FixBindingModalTemplate;
        [SerializeField] RequiredBindingsState requiredBindings;
        public SavedGameState savedGameState;
        public ControlButtonAssetCatalog assetCatalog;
        public GameEvent requestBindingsReloadEvent;
        
        // Filters and Toggles for the ListView
        [SerializeField] bool vJoyOnly; // filter for showing vJoy bindings only
        [SerializeField] bool validOnly; // filter for showing vJoy bindings only
        [SerializeField] bool errorsOnly; // filter for showing vJoy bindings only
        Label bindingsFilenameElement;
        Toggle vJoyToggleElement;
        Toggle validToggleElement;
        Toggle errorsToggleElement;
        Button reloadButtonElement;

        VisualElement modalUI; //fix bindings modal

        //Lists of problematic bindings
        Dictionary<EDControlButton, ControlButtonBinding> filteredButtonBindings;
        private List<EDControlButton> missingRequiredBindings; //no binding, and default required
        private List<EDControlButton> missingHolographicBindings; //no binding, but has a holo button
        private List<EDControlButton> allErrorBindings; // combination of binds with all types of errors
        //public bool bindingsHaveErrors;

        private List<string> availableSingleKeyBindings;
        ListView bindingsListView;
        BindingItem selectedItem;

        private List<SavedControlButton> controlButtons;
       

        public override void OnEnable()
        {
            base.OnEnable();

            // filter starts as off
            vJoyOnly = false;
            validOnly = false;
            errorsOnly = false;

            allErrorBindings = new List<EDControlButton>();

            controlButtons = savedGameState.controlButtons;
            filteredButtonBindings = bindings.buttonBindings;

            VisualElement root = parentUIDocument.rootVisualElement;

            // Store a reference to the log list element
            bindingsListView = root.Q<ListView>("required-bindings-list");
            bindingsFilenameElement = root.Q<Label>("binding-filename-value");
            vJoyToggleElement = root.Q<Toggle>("vjoy-toggle");
            validToggleElement = root.Q<Toggle>("valid-toggle");
            errorsToggleElement = root.Q<Toggle>("errors-toggle");
            reloadButtonElement = root.Q<Button>("reload-button");


            vJoyToggleElement.RegisterValueChangedCallback(OnVJoyToggleChange);
            validToggleElement.RegisterValueChangedCallback(OnValidToggleChange);
            errorsToggleElement.RegisterValueChangedCallback(OnErrorsToggleChange);
            reloadButtonElement.RegisterCallback<ClickEvent>(OnReloadButtonPressed);

            
            List<BindingItem> notLoadedList = new List<BindingItem>();
            notLoadedList.Add(new BindingItem() 
            { 
                name = "BindingsNotReadYet", 
                keyValue = "Joy_ZYXAxis",
                deviceIndexValue = "-5",
                deviceValue = "Boyestrous"            
            });

            PopulateListView(notLoadedList);
        }


        void OnClickButtonInRow(ClickEvent evt)
        {
            // Get the clicked element
            VisualElement clickedElement = evt.target as VisualElement;

            // Check if the clicked element is a button
            if (clickedElement.name == "auto-fix-button")
            {
                // Get the parent of the button, which should be the ListView row
                VisualElement listViewRow = clickedElement.parent;

                string clickedBindingName = listViewRow.Q<Label>("binding-label").text;

                // Get the index of the ListView row
                int index = bindingsListView.IndexOf(listViewRow);

                // Update the selected item of the ListView
                bindingsListView.selectedIndex = index;

                // Manually invoke the onSelectionChange method
                LaunchFixBindingModal(clickedBindingName);
            }
        }
        private void RemoveFromErrorLists(EDControlButton eDControlButton)
        {
            missingHolographicBindings.Remove(eDControlButton);
            missingRequiredBindings.Remove(eDControlButton);
            UpdateErrorBindings();
        }

        public void UpdateErrorBindings()
        {
            allErrorBindings.Clear();
            allErrorBindings.AddRange(missingHolographicBindings);
            allErrorBindings.AddRange(missingRequiredBindings);

            FilterList();
        }    

        public void FindMissingBindings()
        {

            //Get a list of controlButtons that are configured by the player and have no active bindings
            List<EDControlButton> holoMissing = controlButtons
                .Select(button => assetCatalog.GetByName(button.type))
                .Where(button => bindings.buttonBindings[button.GetControl()].HasKeyboardKeybinding == false && bindings.buttonBindings[button.GetControl()].HasVJoyKeybinding == false)
                .Select(asset => asset.GetControl())
                .ToList();

            missingHolographicBindings = holoMissing;

            List<EDControlButton> reqMissing = bindings.buttonBindings
                .Where(kv => bindings.buttonBindings[kv.Key].HasKeyboardKeybinding == false && bindings.buttonBindings[kv.Key].HasVJoyKeybinding == false)
                .Select(kv => kv.Key)
                .Where(button => requiredBindings.requiredBindings.Contains(button))
                .ToList(); // Convert to List<EDControlButton>

            missingRequiredBindings = reqMissing;

            UpdateErrorBindings();
        }

        private async Task ClearAllAndReload()
        {
            //Debug.Log("Reload Button Pressed");
            bindingsListView.Clear();
            allErrorBindings.Clear();
            filteredButtonBindings.Clear();

            await Task.Delay(1500);
            FilterList();
        }

        private void OnReloadButtonPressed(ClickEvent evt)
        {
            _ = ClearAllAndReload();            

            requestBindingsReloadEvent.Raise();
        }

        private void OnVJoyToggleChange(ChangeEvent<bool> evt)  
        {
            //Debug.Log($"Bindings list toggle value changed to: {evt.newValue} || from: {evt.previousValue}");
            vJoyOnly = evt.newValue;
            FilterList();

        }

        private void OnValidToggleChange(ChangeEvent<bool> evt)
        {
            //Debug.Log($"Bindings list toggle value changed to: {evt.newValue} || from: {evt.previousValue}");
            validOnly = evt.newValue;
            FilterList();
        }

        private void OnErrorsToggleChange(ChangeEvent<bool> evt)
        {
            //Debug.Log($"Bindings list toggle value changed to: {evt.newValue} || from: {evt.previousValue}");
            errorsOnly = evt.newValue;
            FilterList();

        }

        private BindingItemState GetBindingItemState(EDControlButton controlButton)
        {
            if (missingHolographicBindings.Contains(controlButton))
            {
                return BindingItemState.MissingHolographic;
            }

            if (missingRequiredBindings.Contains(controlButton))
            {
                return BindingItemState.MissingRequired;
            }

            return BindingItemState.Good;
        }

        private void FindAvailableKeyCombos()
        {
            // Get all available key strings that could be used 
            List<string> allKeyStrings = KeyboardInterface.GetAllKeycodeStrings();

            // These key strings are not great for consistent implementation. Avoid auto-assigning them.
            List<string> ignoreKeyStrings = new List<string> {
                "Key_Escape",
                "Key_LeftShift",
                "Key_LeftControl",
                "Key_LeftAlt",
                "Key_RightShift",
                "Key_RightControl",
                "Key_RightAlt",
                "Key_Backspace",
                "Key_Tab",
                "Key_Enter",
                "Key_CapsLock",
                "Key_Space",
                "Key_PageUp",
                "Key_PageDown",
                "Key_End",
                "Key_Home",
                "Key_LeftArrow",
                "Key_UpArrow",
                "Key_RightArrow",
                "Key_DownArrow",
                "Key_Insert",
                "Key_Delete",
            };

            // Filter out the ignored strings
            allKeyStrings = allKeyStrings.Where(item => !ignoreKeyStrings.Contains(item)).ToList();

            // Initialize a list to store strings without matching key bindings
            List<string> availableKeyCombos = new List<string>();

            // Check each key string for matching key bindings
            foreach (string keyString in allKeyStrings)
            {
                // Check if any key binding (Primary or Secondary) matches the key string
                bool hasMatchingKey = bindings.buttonBindings.Values
                    .Any(binding => binding.Primary.Key == keyString || binding.Secondary.Key == keyString);

                // If there are no matching key bindings for the current key string, add it to the list
                if (!hasMatchingKey)
                {
                    availableKeyCombos.Add(keyString);
                }
            }

            if (availableKeyCombos.Count > 0)
            {
                // Found strings without matching key bindings
                Debug.Log($"There are {availableKeyCombos.Count} available keys that could be used for the missing bindings.");
                //foreach (string keyCombo in availableKeyCombos)
                //{
                //    Debug.Log($" >>>> {keyCombo}");
                //}
            }
            else
            {
                // No key strings found without matching key bindings
                Debug.LogWarning("BindingItemsListController: No key_combos were found to be available without an existing binding. You will be unable to fix the missing bindings.");

            }
            availableSingleKeyBindings = availableKeyCombos;
        }

        public void FilterList()
        { 
            List<BindingItem> filteredBindings = new List<BindingItem>();

            if (bindings.buttonBindings == null) return;

            // Filter buttonBindings based to include only errors
            if (errorsOnly)
            {
                filteredButtonBindings = bindings.buttonBindings
                    .Where(kv => allErrorBindings.Contains(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            else
            {
                filteredButtonBindings = bindings.buttonBindings;
            }

            // Loop through and select items for the list
            foreach (var binding in filteredButtonBindings)
            {
                bool isValid = (!validOnly) ||
                               (binding.Value.HasVJoyKeybinding || binding.Value.HasKeyboardKeybinding);

                if (!isValid) continue;

                if (vJoyOnly && !binding.Value.HasVJoyKeybinding) continue;

                var tempBindingItem = new BindingItem()
                {
                    name = binding.Key.ToString()
                };

                if (binding.Value.HasVJoyKeybinding)
                {
                    tempBindingItem.keyValue = binding.Value.VJoyKeybinding.Value.Key;
                    tempBindingItem.deviceValue = binding.Value.VJoyKeybinding.Value.Device;
                    tempBindingItem.deviceIndexValue = binding.Value.VJoyKeybinding.Value.DeviceIndex;
                    tempBindingItem.state = GetBindingItemState(binding.Key);
                }
                else if (binding.Value.HasKeyboardKeybinding)
                {
                    tempBindingItem.keyValue = binding.Value.KeyboardKeybinding.Value.Key;
                    tempBindingItem.deviceValue = binding.Value.KeyboardKeybinding.Value.Device;
                    tempBindingItem.deviceIndexValue = binding.Value.KeyboardKeybinding.Value.DeviceIndex;
                    tempBindingItem.state = GetBindingItemState(binding.Key);
                }
                else
                {
                    var primary = binding.Value.Primary;
                    var secondary = binding.Value.Secondary;

                    tempBindingItem.deviceValue = !string.IsNullOrEmpty(primary.Device) && primary.Device != "{NoDevice}" ? primary.Device : secondary.Device;
                    tempBindingItem.deviceIndexValue = primary.DeviceIndex ?? secondary.DeviceIndex;
                    tempBindingItem.state = GetBindingItemState(binding.Key);

                }

                filteredBindings.Add(tempBindingItem);
            }

            if (allErrorBindings.Count > 0)
            {
                FindAvailableKeyCombos();
            }

            RebuildListView(filteredBindings);

            bool readyToLaunch = allErrorBindings.Count > 0 ? true : false;
            preLaunchController.SetReady(readyToLaunch);

        }

        private void RebuildListView(List<BindingItem> items)
        {
            // Clear the ListView by setting an empty list as the itemsSource
            bindingsListView.itemsSource = new List<BindingItem>();

            // Refresh the ListView to clear it
            bindingsListView.Rebuild();

            // Repopulate ListView with new items
            PopulateListView(items);
        }

        void LaunchFixBindingModal(string bindingName)
        {
            modalUI = m_FixBindingModalTemplate.Instantiate();
            //availableSingleKeyBindings
            Button okButton = modalUI.Q<Button>("ok-button");
            Button cancelButton = modalUI.Q<Button>("cancel-button");
            Button xButton = modalUI.Q<Button>("x-button");
            Label bindingLabel = modalUI.Q<Label>("control-binding-name");
            DropdownField availableKeys = modalUI.Q<DropdownField>("available-keys-dropdown");

            okButton.clicked += SubmitFixBinding;
            cancelButton.clicked += CloseFixBindingModal;
            xButton.clicked += CloseFixBindingModal;
            bindingLabel.text = bindingName;
            availableKeys.choices = availableSingleKeyBindings;
            availableKeys.RegisterCallback<ChangeEvent<string>>(OnBindingFixDropdownSelectionChanged);

            // Get the VisualElement representing the ListView
            VisualElement listViewElement = bindingsListView.hierarchy.parent;

            // Now you can use the RectTransform listViewRectTransform as needed
            // For example, you can get its position, size, etc.
            Vector2 listViewPosition = listViewElement.contentRect.position;
            Vector2 listViewSize = listViewElement.contentRect.size;

            // Calculate the center position of the ListView
            Vector2 listViewCenter = new Vector2(listViewPosition.x + (listViewSize.x / 2f),
                                                 listViewPosition.y + (listViewSize.y / 2f));

            // Set the position of modalUI
            modalUI.style.position = Position.Absolute;
            modalUI.style.left = listViewCenter.x;
            modalUI.style.top = listViewCenter.y;

            parentUIDocument.rootVisualElement.Add(modalUI);
        }

        void SubmitFixBinding()
        {
            var controlName = modalUI.Q<Label>("control-binding-name");            
            var dropdownField = modalUI.Q<DropdownField>("available-keys-dropdown");
            var newValue = dropdownField.value;

            Debug.Log($"Updating binding for {controlName} to {newValue}");

            EDControlBindingsUtils.UpdateBindingXml(bindings.bindingsFilePath, controlName.text, newValue);
            RemoveFromErrorLists((EDControlButton)Enum.Parse(typeof(EDControlButton), controlName.text));

            CloseFixBindingModal();
        }

        void OnBindingFixDropdownSelectionChanged(ChangeEvent<string> evt)
        {
            Button okButton = modalUI.Q<Button>("ok-button");

            if (evt.newValue != null)
            {
                okButton.SetEnabled(true);
                okButton.RemoveFromClassList("disabledBtn");
            }
            else
            {
                okButton.SetEnabled(false);
                okButton.AddToClassList("disabledBtn");
            }

            //Debug.Log($"dropdown changed: {evt.newValue}");
        }

        void CloseFixBindingModal()
        {
            modalUI.RemoveFromHierarchy();
        }

        public void SetBindingsFileName()
        {
            if (bindings != null)
            {
                bindingsFilenameElement.text = bindings.bindingsFileName != null ? bindings.bindingsFileName : "-- Not Yet Set --";
            }
        }

        void PopulateListView(List<BindingItem> items)
        {
            SetBindingsFileName();

            //Set up a make item function for a list entry
            bindingsListView.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                TemplateContainer newListEntry = m_BindingEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                BindingItemDisplay newListEntryLogic = new BindingItemDisplay();

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Initialize the controller script
                newListEntryLogic.SetVisualElement(newListEntry);

                // Find the button in the listItem prefab
                Button button = newListEntry.Q<Button>("auto-fix-button");

                // Add a click listener to the button
                //button.clicked += LaunchFixBindingModal; 
                button.RegisterCallback<ClickEvent>(OnClickButtonInRow);

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            bindingsListView.onSelectionChange += (IEnumerable<object> selections) =>
            {             
                selectedItem = selections.FirstOrDefault() as BindingItem;
                Debug.Log($"selected Item: {selectedItem.name}");
            };

            // Set up bind function for a specific list entry
            bindingsListView.bindItem = (item, index) =>
            {
                (item.userData as BindingItemDisplay).SetBindingData(items[index]);
            };


            bindingsListView.itemsSource = items;

            bindingsListView.Rebuild();
        }

    }
}
