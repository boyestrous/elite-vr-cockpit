using System;
using System.Collections.Generic;
using System.ComponentModel;
using EVRC.Core;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    /// <summary>
    /// Displays the controlButtons (holographic) that are currently configured and provides an interface to add/remove buttons
    /// </summary>
    public class ControlButtonViewController : MonoBehaviour
    {
        [SerializeField] UIDocument parentUIDocument;
        [SerializeField] VisualTreeAsset controlButtonEntryTemplate;
        public SavedGameState savedState;
        [SerializeField] GameEvent overlayStateLoadedEvent;

        [Tooltip("Name of the visual element that will hold the list of controlButtons")]
        public string targetParentName = "ControlButtonsState";
        public ControlButtonAssetCatalog controlButtonCatalog;
        public GameEvent controlButtonRemovedEvent;
        public AddControlButtonForm addControlButtonForm;

        private VisualElement root; // the root of the whole UI
        private Dictionary<(string, string), ControlButtonList> controlButtonLists;

        private Button openAddButtonModalElement;


        // the anchor object that all of the lists will go inside of
        private ScrollView controlListContainer;
        

        public void OnEnable()
        {
            addControlButtonForm = GetComponent<AddControlButtonForm>();

            root = parentUIDocument.rootVisualElement;
            controlButtonLists = new Dictionary<(string, string), ControlButtonList>();
            controlListContainer = root.Q<ScrollView>("control-list-container");
            
            openAddButtonModalElement = root.Q<Button>("open-addButtonModal-button");
            openAddButtonModalElement.clicked += LaunchModal;

            savedState.Load();
            if (savedState.controlButtons != null)
            {
                DisplayControlButtons(savedState.controlButtons);
                overlayStateLoadedEvent.Raise();
            }
        }

        public void DisplayControlButtons(List<SavedControlButton> controlButtons)
        {
            foreach (SavedControlButton item in controlButtons)
            {
                AddControlButton(item);
            }
        }

        public void AddControlButton(SavedControlButton addedControlButton)
        {
            // use the "type" to search for a matching controlButtonAsset
            string type = addedControlButton.type;
            ControlButtonAsset controlButtonAsset = controlButtonCatalog.GetByName(type);

            var cat = (addedControlButton.anchorStatusFlag, addedControlButton.anchorGuiFocus);

            // Check if a ListView exists for the item's category
            if (!controlButtonLists.ContainsKey(cat))
            {
                // Doesn't exist, create a new ControlButtonList
                var newList = new ControlButtonList(cat, controlButtonEntryTemplate, savedState, controlButtonCatalog, controlButtonRemovedEvent);

                // Add it to the list of ControlButtonLists
                controlButtonLists.Add(cat, newList);

                // Add the Visual Element to the UI
                controlListContainer.Add(newList.visualElementContainer);
            }

            // Add to source list
            controlButtonLists[cat].Add(addedControlButton);

            
        }

        void LaunchModal()
        {
            // Get the VisualElement representing the ListView
            VisualElement listViewElement = controlListContainer.hierarchy.parent;

            // Now you can use the RectTransform listViewRectTransform as needed
            // For example, you can get its position, size, etc.
            Vector2 listViewPosition = listViewElement.contentRect.position;
            Vector2 listViewSize = listViewElement.contentRect.size;

            // Calculate the center position of the ListView
            Vector2 listViewCenter = new Vector2(listViewPosition.x + (listViewSize.x / 2f),
                                                 listViewPosition.y + (listViewSize.y / 2f));

            addControlButtonForm.LaunchModal(listViewCenter);
        }
    }
}
