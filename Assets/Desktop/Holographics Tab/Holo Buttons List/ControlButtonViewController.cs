using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
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
        public ControlButtonAssetCatalog controlButtonCatalog;
        public GameEvent controlButtonRemovedEvent;
        private AddControlButtonModal addControlButtonModal;

        private VisualElement root; // the root of the whole UI
        private Dictionary<(string, string), ControlButtonList> controlButtonLists;

        private Button openAddButtonModalElement;

        // the Scrollview that will hold all of the mini ListViews of controlButtons
        private ScrollView controlListScrollview;

        public void OnEnable()
        {
            addControlButtonModal = GetComponent<AddControlButtonModal>();

            root = parentUIDocument.rootVisualElement;
            controlButtonLists = new Dictionary<(string, string), ControlButtonList>();
            controlListScrollview = root.Q<ScrollView>("control-list-container");

            openAddButtonModalElement = root.Q<Button>("open-addButtonModal-button");
            openAddButtonModalElement.clicked += addControlButtonModal.LaunchModal;

            PopulateHolographicButtonsListView();
        }

        // Invoked from GameEvent
        public void PopulateHolographicButtonsListView()
        {
            UnityEngine.Debug.LogWarning("PopulateHolographicButtonsListView");

            //Reset the List of Lists and the containing scrollview
            controlButtonLists = new Dictionary<(string, string), ControlButtonList>();

            if (controlListScrollview == null)
            {
                controlListScrollview = root.Q<ScrollView>("control-list-container");
            }
            //CreateScrollview();
            controlListScrollview.Clear();
            controlListScrollview.scrollOffset = Vector2.zero; // Reset scroll position to the top-left corner

            List<SavedControlButton> controlButtons = savedState.controlButtons;

            foreach (SavedControlButton item in controlButtons)
            {
                AddControlButton(item);
            }
        }

        public void AddControlButton(SavedControlButton addedControlButton)
        {
            UnityEngine.Debug.Log($"Adding ControlButton: {addedControlButton.type}");
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
                controlListScrollview.Add(newList.visualElementContainer);
            }

            // Add to source list
            controlButtonLists[cat].Add(addedControlButton);
        }
    }
}
