using EVRC.Core;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using EVRC.DesktopUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class BindingItemsListController : MonoBehaviour
    {
        [Header("Templates and Scene Refs")]
        // UXML template for list entries
        [SerializeField] VisualTreeAsset m_BindingEntryTemplate;
        [SerializeField] UIDocument parentUIDocument;
        [SerializeField] ControlBindingsState bindings;
        [SerializeField] RequiredBindingsState requiredBindings;
        public SavedGameState savedGameState;
        public ControlButtonAssetCatalog assetCatalog;
        
        // Filters and Toggles for the ListView
        [SerializeField] bool vJoyOnly; // filter for showing vJoy bindings only
        [SerializeField] bool validOnly; // filter for showing vJoy bindings only
        [SerializeField] bool errorsOnly; // filter for showing vJoy bindings only
        Toggle vJoyToggleElement;
        Toggle validToggleElement;
        Toggle errorsToggleElement;

        //Lists of problematic bindings
        Dictionary<EDControlButton, ControlButtonBinding> filteredButtonBindings;
        public List<EDControlButton> missingRequiredBindings; //no binding, and default required
        public List<EDControlButton> missingHolographicBindings; //no binding, but has a holo button
        public List<EDControlButton> allErrorBindings; // combination of binds with all types of errors
        ListView bindingsListView;

        private List<SavedControlButton> controlButtons;
        

        [SerializeField] List<BindingItem> bindingItems;

        public void OnEnable()
        {            
            // filter starts as off
            vJoyOnly = false;
            validOnly = false;
            errorsOnly = false;

            controlButtons = savedGameState.controlButtons;
            bindingItems = new List<BindingItem>();
            filteredButtonBindings = bindings.buttonBindings;

            VisualElement root = parentUIDocument.rootVisualElement;

            // Store a reference to the log list element
            bindingsListView = root.Q<ListView>("required-bindings-list");
            vJoyToggleElement = root.Q<Toggle>("vjoy-toggle");
            validToggleElement = root.Q<Toggle>("valid-toggle");
            errorsToggleElement = root.Q<Toggle>("errors-toggle");

            vJoyToggleElement.RegisterValueChangedCallback(OnVJoyToggleChange);
            validToggleElement.RegisterValueChangedCallback(OnValidToggleChange);
            errorsToggleElement.RegisterValueChangedCallback(OnErrorsToggleChange);

            SetListBindingMethods();
            bindingItems.Add(new BindingItem() 
            { 
                name = "BindingsNotReadYet", 
                keyValue = "Joy_ZYXAxis",
                deviceIndexValue = "-5",
                deviceValue = "Boyestrous"            
            });

            RefreshBindingsList();
        }

        public void UpdateErrorBindings()
        {
            allErrorBindings.Clear();
            allErrorBindings.AddRange(missingHolographicBindings);
            allErrorBindings.AddRange(missingRequiredBindings);
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

        private void OnVJoyToggleChange(ChangeEvent<bool> evt)
        {
            //Debug.Log($"Bindings list toggle value changed to: {evt.newValue} || from: {evt.previousValue}");
            vJoyOnly = evt.newValue;
            RefreshBindingsList();
        }

        private void OnValidToggleChange(ChangeEvent<bool> evt)
        {
            //Debug.Log($"Bindings list toggle value changed to: {evt.newValue} || from: {evt.previousValue}");
            validOnly = evt.newValue;
            RefreshBindingsList();
        }

        private void OnErrorsToggleChange(ChangeEvent<bool> evt)
        {
            //Debug.Log($"Bindings list toggle value changed to: {evt.newValue} || from: {evt.previousValue}");
            errorsOnly = evt.newValue;

            RefreshBindingsList();
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

        public void RefreshBindingsList()
        {
            bindingItems.Clear();
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

                bindingItems.Add(tempBindingItem);
            }

            bindingsListView.Rebuild();
        }



        void SetListBindingMethods()
        {
            //Set up a make item function for a list entry
            bindingsListView.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = m_BindingEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var newListEntryLogic = new BindingItemDisplay();

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Initialize the controller script
                newListEntryLogic.SetVisualElement(newListEntry);

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            // Set up bind function for a specific list entry
            bindingsListView.bindItem = (item, index) =>
            {
                (item.userData as BindingItemDisplay).SetBindingData(bindingItems[index]);
            };

            // Set a fixed item height
            //m_LogList.fixedItemHeight = 45;

            // Set the actual item's source list/array
            bindingsListView.itemsSource = bindingItems;
        }

    }
}
