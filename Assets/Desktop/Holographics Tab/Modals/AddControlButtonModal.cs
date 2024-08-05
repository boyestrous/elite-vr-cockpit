using EVRC.Core;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using EVRC.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{

    public class AddControlButtonModal : Modal
    {
        [SerializeField] private ControlButtonAddedEvent controlButtonAddedEvent;
        [SerializeField] private ControlButtonAssetCatalog catalog;
        [SerializeField] private SavedGameState savedGameState;
        [SerializeField] private bool advancedModeActive;
       
        private ControlButtonSpawnManager spawnManager;

        //Basic Mode Form Elements - basic mode sets values to the invisible Advanced Mode
        private VisualElement basicFormContainer;
        private DropdownField basicDropdown;
        private Toggle advancedModeToggle;
        private Label messageText;

        // Advanced Mode Form Elements
        private VisualElement advancedFormContainer;
        private DropdownField statusFlagDropdown;
        private DropdownField guiFocusDropdown;
        private DropdownField controlButtonDropdown;
        private EDStatusFlags selectedStatusFlag;
        private EDStatusFlags2 selectedStatusFlag2;
        private EDGuiFocus selectedGuiFocus;
        private bool requiredGuiFocus; // whether or not to search for a required GuiFocus
        private ControlButtonAsset selectedControlButton;

        // Lists that source the dropdown fields
        List<string> guiFocusList;
        List<string> statusFlagList;
        Dictionary<string,(EDStatusFlags, EDStatusFlags2, EDGuiFocus?)> basicOptionsList;

        public override void OnEnable()
        {
            base.OnEnable();

            basicOptionsList = new Dictionary<string, (EDStatusFlags, EDStatusFlags2, EDGuiFocus?)>
            {
                { "Main Ship", (EDStatusFlags.InMainShip, EDStatusFlags2.None, null)},
                { "SRV (Buggy)",(EDStatusFlags.InSRV, EDStatusFlags2.None, null)},
                { "Fighter",(EDStatusFlags.InFighter, EDStatusFlags2.None, null)},
                { "FSS Mode",(EDStatusFlags.InMainShip, EDStatusFlags2.None, EDGuiFocus.FSSMode)},
                { "Galaxy Map",(EDStatusFlags.InMainShip, EDStatusFlags2.None, EDGuiFocus.GalaxyMap)},
                { "Station Services",(EDStatusFlags.InMainShip, EDStatusFlags2.None, EDGuiFocus.StationServices)},
            };
        }

        public override void LaunchModal()
        {
            base.LaunchModal();

            PopulateBasicOptions();
            PopulateAdvancedOptions();

            selectedStatusFlag = EDStatusFlags.InMainShip;
            selectedGuiFocus = EDGuiFocus.PanelOrNoFocus;
            selectedStatusFlag2 = EDStatusFlags2.None;
            requiredGuiFocus = false;

            advancedModeToggle = modalUI.Q<Toggle>("advanced-mode-toggle");

            advancedFormContainer = modalUI.Q<VisualElement>("advanced-options");
            basicFormContainer = modalUI.Q<VisualElement>("basic-options");
            
            controlButtonDropdown = modalUI.Q<DropdownField>("control-button-dropdown");
            messageText = modalUI.Q<Label>("messageText");

            //Register the callbacks
            statusFlagDropdown.RegisterValueChangedCallback(OnStatusFlagChanged);
            guiFocusDropdown.RegisterValueChangedCallback(OnGuiFocusChanged);
            controlButtonDropdown.RegisterValueChangedCallback(OnButtonSelectionChanged);
            advancedModeToggle.RegisterValueChangedCallback(OnAdvancedToggleChanged);
            basicDropdown.RegisterValueChangedCallback(OnBasicDropdownChanged);

            // Initialize a Spawn manager instance
            spawnManager = new ControlButtonSpawnManager(savedGameState);

            parentUIDocument.rootVisualElement.Add(modalUI);
        }

        private void PopulateBasicOptions()
        {
            basicDropdown = modalUI.Q<DropdownField>("basic-dropdown");

            basicDropdown.choices = basicOptionsList.Keys.ToList();
            basicDropdown.index = 0;

        }

        private void PopulateAdvancedOptions()
        {
            statusFlagDropdown = modalUI.Q<DropdownField>("statusFlag-dropdown");
            guiFocusDropdown = modalUI.Q<DropdownField>("guiFocus-dropdown");

            //Create list for the Status Flag options
            statusFlagList = new List<string>();
            statusFlagList.AddRange(Enum.GetNames(typeof(EDStatusFlags)).ToList());
            statusFlagList.AddRange(Enum.GetNames(typeof(EDStatusFlags2)).ToList());

            //Create list for the GuiFocus options
            guiFocusList = new List<string>();
            guiFocusList.Add("--Any Focus--");
            guiFocusList.AddRange(Enum.GetNames(typeof(EDGuiFocus)).ToList());

            // Move "PanelOrNoFocus" to the first position - this is the most used option
            int panelOrNoFocusIndex = guiFocusList.IndexOf(EDGuiFocus.PanelOrNoFocus.ToString());
            guiFocusList.RemoveAt(panelOrNoFocusIndex);
            guiFocusList.Insert(1, EDGuiFocus.PanelOrNoFocus.ToString());

            // Set the choices for the actual Dropdown VisualElements
            statusFlagDropdown.choices = statusFlagList;
            statusFlagDropdown.index = statusFlagList.IndexOf(EDStatusFlags.InMainShip.ToString());

            guiFocusDropdown.choices = guiFocusList;
            guiFocusDropdown.index = 0;

            
            if (advancedModeActive)
            {
                //blank until a category is chosen when using Advanced Mode
                controlButtonDropdown.choices = new List<string>(); 
            }
        }

        private bool BothNullCheck()
        {
            if (selectedGuiFocus == null && selectedStatusFlag == null && selectedStatusFlag2 == null)
            {
                controlButtonDropdown.SetEnabled(false);
                messageText.AddToClassList("warningMessage");
                messageText.text = "You must choose either a StatusFlag or a GuiFocus";
                messageText.style.display = DisplayStyle.Flex;
                return true;
            }
            controlButtonDropdown.SetEnabled(true);
            messageText.RemoveFromClassList("warningMessage");
            messageText.style.display = DisplayStyle.None;
            return false;
        }

        private void SetAvailableControlButtons()
        {
            List<string> availableControlButtons = new List<string>();

            // Filter the catalog for matching Assets
            availableControlButtons.AddRange(
                catalog.controlButtons
                .Where(x => x.statusFlagFilters.HasFlag(selectedStatusFlag))
                .Where(x => x.statusFlag2Filters.HasFlag(selectedStatusFlag2))
                .Where(y => requiredGuiFocus ? y.guiFocusRequired.Contains(selectedGuiFocus) : y.guiFocusRequired.Count() == 0)
                .Select(x => x.name).ToList()
                );
            controlButtonDropdown.choices = availableControlButtons;
        }

        private void OnBasicDropdownChanged(ChangeEvent<string> evt)
        {
            basicOptionsList.TryGetValue(evt.newValue, out var flagSettings);

            selectedStatusFlag = flagSettings.Item1;
            selectedStatusFlag2 = flagSettings.Item2;

            if (flagSettings.Item3 == null)
            {
                requiredGuiFocus = false;
            } else
            {
                requiredGuiFocus = true;
                selectedGuiFocus = (EDGuiFocus)flagSettings.Item3;
            }
            
            controlButtonDropdown.value = "";
            selectedControlButton = null;

            SetAvailableControlButtons();
        }

        private void OnAdvancedToggleChanged(ChangeEvent<bool> evt) 
        {
            Debug.Log($"Advanced mode toggled. New Value: {evt.newValue}");
            advancedModeActive = evt.newValue;
            
            // If active
            if (evt.newValue)
            {
                advancedFormContainer.style.display = DisplayStyle.Flex;
                basicFormContainer.style.display = DisplayStyle.None;
            }
            else
            {
                advancedFormContainer.style.display = DisplayStyle.None;
                basicFormContainer.style.display = DisplayStyle.Flex;
            }
            selectedStatusFlag = EDStatusFlags.InMainShip;
            selectedGuiFocus = EDGuiFocus.PanelOrNoFocus;
            requiredGuiFocus = false;
            selectedStatusFlag2 = EDStatusFlags2.None;

        }

        public void OnStatusFlagChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == "--Any Flag--")
            {
                selectedStatusFlag = EDStatusFlags.InMainShip;
                selectedStatusFlag2 = EDStatusFlags2.None;
            } else {
                var parsedSelection = EnumUtils.ParseEnumsOrDefault<EDStatusFlags, EDStatusFlags2>(evt.newValue);
                selectedStatusFlag = parsedSelection.Item2;
                selectedStatusFlag2 = parsedSelection.Item3;
            }                     

            controlButtonDropdown.value = "";
            selectedControlButton = null;

            if (BothNullCheck()) return;
            SetAvailableControlButtons();
        }

        public void OnGuiFocusChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == "--Any Focus--")
            {
                requiredGuiFocus = false;
            }
            else
            {
                requiredGuiFocus = true;
                selectedGuiFocus = Enum.Parse<EDGuiFocus>(evt.newValue);
            }

            controlButtonDropdown.value = "";
            selectedControlButton = null;

            if (BothNullCheck()) return;
            SetAvailableControlButtons();
        }

        public void OnButtonSelectionChanged(ChangeEvent<string> evt)
        {
            selectedControlButton = catalog.controlButtons.FirstOrDefault(x => x.name == evt.newValue);
           
        }
        
        public override void Submit()
        {
            //if (selectedGuiFocus == null) selectedGuiFocus = EDGuiFocus.PanelOrNoFocus;


            if (selectedControlButton != null)
            {
                // Get the location
                var placePosition = spawnManager.GetSpawnLocation();
                var addedControlButton = new SavedControlButton()
                {
                    type = selectedControlButton.name,
                    anchorGuiFocus = selectedGuiFocus.ToString(),
                    anchorStatusFlag = selectedStatusFlag != default(EDStatusFlags) ? selectedStatusFlag.ToString() : selectedStatusFlag2.ToString(),
                    overlayTransform = new OverlayTransform()
                    {
                        pos = placePosition,
                        rot = Vector3.zero,
                    }
                };

                // Write the new button to the SavedGameState
                savedGameState.controlButtons.Add(addedControlButton);
                savedGameState.Save();

                controlButtonAddedEvent.Raise(addedControlButton);
            }
        }
    }
}
