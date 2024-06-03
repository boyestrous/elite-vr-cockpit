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

    public class AddControlButtonModal : MonoBehaviour
    {

        public ControlButtonAddedEvent controlButtonAddedEvent;
        [SerializeField] UIDocument parentUIDocument;
        public ControlButtonAssetCatalog catalog;
        public SavedGameState savedGameState;
        private Toggle advancedModeToggle;
        private DropdownField statusFlagDropdown;
        private DropdownField guiFocusDropdown;
        private DropdownField controlButtonDropdown;
        private Label messageText;
        private Button addButton;
        private Button cancelButton;
        private ControlButtonSpawnManager spawnManager;

        [SerializeField] VisualTreeAsset addButtonModalTemplate;

        private EDStatusFlags? selectedStatusFlag;
        private EDStatusFlags2? selectedStatusFlag2;
        private EDGuiFocus? selectedGuiFocus;
        private ControlButtonAsset selectedControlButton;

        List<string> statusFlagList;
        List<string> guiFocusList;

        VisualElement modalUI;

        //public void Awake()
        //{
        //    //Populate the status Flag dropdown
        //    statusFlagList = new List<string>();
        //    statusFlagList.Add("--Any Flag--");
        //    statusFlagList.AddRange(Enum.GetNames(typeof(EDStatusFlags)).ToList());
        //    statusFlagList.AddRange(Enum.GetNames(typeof(EDStatusFlags2)).ToList());

        //    //Populate the GuiFocus dropdown
        //    guiFocusList = new List<string>();
        //    guiFocusList.AddRange(Enum.GetNames(typeof(EDGuiFocus)).ToList());

        //    // Move "PanelOrNoFocus" to the first position
        //    int panelOrNoFocusIndex = guiFocusList.IndexOf(EDGuiFocus.PanelOrNoFocus.ToString());
        //    guiFocusList.RemoveAt(panelOrNoFocusIndex);
        //    guiFocusList.Insert(0, EDGuiFocus.PanelOrNoFocus.ToString());
        //}

        //// Function to find the parent element with the specific name
        //public VisualElement FindParentByName(VisualElement element, string name)
        //{
        //    VisualElement currentElement = element;

        //    while (currentElement != null)
        //    {
        //        if (currentElement.name == name)
        //        {
        //            return currentElement;
        //        }
        //        currentElement = currentElement.parent;
        //    }

        //    return null;
        //}

        public void LaunchModal(Vector2 position)
        {
            //Populate the status Flag dropdown
            statusFlagList = new List<string>();
            statusFlagList.Add("--Any Flag--");
            statusFlagList.AddRange(Enum.GetNames(typeof(EDStatusFlags)).ToList());
            statusFlagList.AddRange(Enum.GetNames(typeof(EDStatusFlags2)).ToList());

            //Populate the GuiFocus dropdown
            guiFocusList = new List<string>();
            guiFocusList.AddRange(Enum.GetNames(typeof(EDGuiFocus)).ToList());

            // Move "PanelOrNoFocus" to the first position
            int panelOrNoFocusIndex = guiFocusList.IndexOf(EDGuiFocus.PanelOrNoFocus.ToString());
            guiFocusList.RemoveAt(panelOrNoFocusIndex);
            guiFocusList.Insert(0, EDGuiFocus.PanelOrNoFocus.ToString());


            modalUI = addButtonModalTemplate.Instantiate();

            advancedModeToggle = modalUI.Q<Toggle>("advanceModeToggle");
            statusFlagDropdown = modalUI.Q<DropdownField>("statusFlag-dropdown");
            guiFocusDropdown = modalUI.Q<DropdownField>("guiFocus-dropdown");
            controlButtonDropdown = modalUI.Q<DropdownField>("control-button-dropdown");
            messageText = modalUI.Q<Label>("messageText");


            addButton = modalUI.Q<Button>("add-button");
            cancelButton = modalUI.Q<Button>("cancel-button");

            addButton.clicked += Submit;
            cancelButton.clicked += CloseModal;

            
            statusFlagDropdown.choices = statusFlagList;
            statusFlagDropdown.index = 0;


            // Set choices for the GuiFocus dropdown VisualElement
            guiFocusDropdown.choices = guiFocusList;
            guiFocusDropdown.index = 0;

            controlButtonDropdown.choices = new List<string>(); //blank until a category is chosen
            SetAvailableControlButtons();

            //Register the callbacks
            statusFlagDropdown.RegisterValueChangedCallback(OnStatusFlagChanged);
            guiFocusDropdown.RegisterValueChangedCallback(OnGuiFocusChanged);
            controlButtonDropdown.RegisterValueChangedCallback(OnButtonSelectionChanged);

            // Initialize a Spawn manager instance
            spawnManager = new ControlButtonSpawnManager(savedGameState);

            // Set the position of modalUI
            modalUI.style.position = Position.Absolute;
            modalUI.style.left = position.x;
            modalUI.style.top = position.y;

            parentUIDocument.rootVisualElement.Add(modalUI);
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
        
        public void Submit()
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

        private void CloseModal()
        {
            modalUI.RemoveFromHierarchy();
        }
    }
}
