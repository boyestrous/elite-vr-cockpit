using EVRC.Core;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using EVRC.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{

    public class AddControlButtonForm : MonoBehaviour
    {

        public ControlButtonAddedEvent controlButtonAddedEvent;
        [SerializeField] UIDocument parentUIDocument;
        public ControlButtonAssetCatalog catalog;
        public SavedGameState savedGameState;
        private VisualElement root; // the root of the whole UI
        private Toggle advancedModeToggle;
        private DropdownField statusFlagDropdown;
        private DropdownField guiFocusDropdown;
        private DropdownField controlButtonDropdown;
        private Label messageText;
        private Button addButton;
        private ControlButtonSpawnManager spawnManager;

        private EDStatusFlags? selectedStatusFlag;
        private EDStatusFlags2? selectedStatusFlag2;
        private EDGuiFocus? selectedGuiFocus;
        private ControlButtonAsset selectedControlButton;

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;
            advancedModeToggle = root.Q<Toggle>("advanceModeToggle");
            statusFlagDropdown = root.Q<DropdownField>("statusFlag-dropdown");
            guiFocusDropdown = root.Q<DropdownField>("guiFocus-dropdown");
            controlButtonDropdown = root.Q<DropdownField>("control-button-dropdown");
            messageText = root.Q<Label>("messageText");
            addButton = root.Q<Button>("add-button");

            //Populate the status Flag dropdown
            List<string> statusFlagList = new List<string>();
            statusFlagList.Add("--Any Flag--");
            statusFlagList.AddRange(Enum.GetNames(typeof(EDStatusFlags)).ToList());
            statusFlagList.AddRange(Enum.GetNames(typeof(EDStatusFlags2)).ToList());
            statusFlagDropdown.choices = statusFlagList;
            statusFlagDropdown.index = 0;

            //Populate the GuiFocus dropdown
            List<string> guiFocusList = new List<string>();            
            guiFocusList.AddRange(Enum.GetNames(typeof(EDGuiFocus)).ToList());

            // Move "PanelOrNoFocus" to the first position
            int panelOrNoFocusIndex = guiFocusList.IndexOf(EDGuiFocus.PanelOrNoFocus.ToString());
            guiFocusList.RemoveAt(panelOrNoFocusIndex);
            guiFocusList.Insert(0, EDGuiFocus.PanelOrNoFocus.ToString());

            // Set choices for the GuiFocus dropdown VisualElement
            guiFocusDropdown.choices = guiFocusList;
            guiFocusDropdown.index = 0;

            controlButtonDropdown.choices = new List<string>(); //blank until a category is chosen
            SetAvailableControlButtons();

            //Register the callbacks
            statusFlagDropdown.RegisterValueChangedCallback(OnStatusFlagChanged);
            guiFocusDropdown.RegisterValueChangedCallback(OnGuiFocusChanged);
            controlButtonDropdown.RegisterValueChangedCallback(OnButtonSelectionChanged);
            addButton.RegisterCallback<ClickEvent>(Submit);

            // Initialize a Spawn manager instance
            spawnManager = new ControlButtonSpawnManager(savedGameState);
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
            if (BothNullCheck()) return;

            List<string> availableControlButtons = new List<string>();
            // Filter the catalog
            availableControlButtons.AddRange(
                catalog.controlButtons
                .Where(x => selectedStatusFlag.HasValue ? 
                            x.statusFlagFilters.HasFlag(selectedStatusFlag): 
                            x.statusFlagFilters != 0
                            )
                .Where(x => selectedStatusFlag2.HasValue ?
                            x.statusFlag2Filters.HasFlag(selectedStatusFlag2) :
                            x.statusFlag2Filters != 0
                            )
                .Where(y => selectedGuiFocus.HasValue ? y.guiFocusRequired.Contains(selectedGuiFocus.Value) : y.guiFocusRequired.Count() == 0)
                .Select(x => x.name).ToList()
                );

            controlButtonDropdown.choices = availableControlButtons;
        }

        public void OnStatusFlagChanged(ChangeEvent<string> evt)
        {
            
            if (evt.newValue == "--Any Flag--")
            {
                selectedStatusFlag = null;
                selectedStatusFlag2 = null;
            } else {
                var parsedSelection = EnumUtils.ParseEnumsOrDefault<EDStatusFlags, EDStatusFlags2>(evt.newValue);
                selectedStatusFlag = parsedSelection.Item2;
                selectedStatusFlag2 = parsedSelection.Item3;
            }                     

            controlButtonDropdown.value = "";
            selectedControlButton = null;

            SetAvailableControlButtons();
        }

        public void OnGuiFocusChanged(ChangeEvent<string> evt)
        {
            selectedGuiFocus = Enum.Parse<EDGuiFocus>(evt.newValue);

            controlButtonDropdown.value = "";
            selectedControlButton = null;

            SetAvailableControlButtons();
        }

        public void OnButtonSelectionChanged(ChangeEvent<string> evt)
        {
            selectedControlButton = catalog.controlButtons.FirstOrDefault(x => x.name == evt.newValue);
           
        }
        
        public void Submit(ClickEvent evt)
        {
            if (selectedGuiFocus == null) selectedGuiFocus = EDGuiFocus.PanelOrNoFocus;

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
