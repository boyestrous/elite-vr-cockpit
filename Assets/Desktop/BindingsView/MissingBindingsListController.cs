using EVRC.Core;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class MissingBindingsListController : MonoBehaviour
    {
        [Header("Game State references")]
        public SavedGameState savedGameState;
        public ControlBindingsState bindingsState;
        public ControlButtonAssetCatalog assetCatalog;

        [Header("Templates and Scene Refs")]
        // UXML template for list entries
        [SerializeField] VisualTreeAsset m_BindingEntryTemplate;
        public UIDocument parentUIDocument;


        // Other ListView controllers always include a custom class for the list items, but this one only has a single property...
        // Including this class for uniformity, maybe not necessary
        public class MissingBindingItem
        {
            public string buttonName;
        }

        [SerializeField] List<MissingBindingItem> m_missingBindings;
        // UI element references
        ListView missingBindingsListView;

        public List<EDControlButton> missingBindings = new List<EDControlButton>();
        private List<SavedControlButton> controlButtons;

        public void OnEnable()
        {
            controlButtons = savedGameState.controlButtons;
            m_missingBindings = new List<MissingBindingItem>();

            VisualElement root = parentUIDocument.rootVisualElement;

            // Store a reference to the log list element
            missingBindingsListView = root.Q<ListView>("missing-bindings-list");
            SetListBindingMethods();
        }

        /// <summary>
        /// Check that each ControlButtonAsset in the catalog has a valid binding in the bindings file
        /// </summary>
        /// <returns>True if bindings are valid, False if bindings are missing.</returns>
        public bool ValidateBindings()
        {

            //Get a list of controlButtons that are configured by the player and have no active bindings
            List<EDControlButton> missing = controlButtons
                .Select(button => assetCatalog.GetByName(button.type))
                .Where(button => bindingsState.buttonBindings[button.GetControl()].HasKeyboardKeybinding == false && bindingsState.buttonBindings[button.GetControl()].HasVJoyKeybinding == false)
                .Select(asset => asset.GetControl())
                .ToList();

            missingBindings = missing;
            return UpdateListView();
        }

        private bool UpdateListView()
        {
            m_missingBindings.Clear();

            foreach (var binding in missingBindings)
            {
                var tempBindingItem = new MissingBindingItem()
                {
                    buttonName = binding.ToString(),
                };

                m_missingBindings.Add(tempBindingItem);
            }

            missingBindingsListView.Rebuild();
            return missingBindings.Count != 0;
        }

        private void OnMissingItemSelected(IEnumerable<object> enumerable)
        {
            // Log the selected items
            foreach (object item in enumerable)
            {
                // Assuming each item is of type MissingBindingItem
                MissingBindingItem selectedItem = (MissingBindingItem)item;
                Debug.Log("Selected Item Name: " + selectedItem.buttonName);
            }
        }

        void SetListBindingMethods()
        {
            //Set up a make item function for a list entry
            missingBindingsListView.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = m_BindingEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var newListEntryLogic = new MissingItemDisplay();

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Initialize the controller script
                newListEntryLogic.SetVisualElement(newListEntry);

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            missingBindingsListView.onSelectionChange += OnMissingItemSelected;

            // Set up bind function for a specific list entry
            missingBindingsListView.bindItem = (item, index) =>
            {
                (item.userData as MissingItemDisplay).SetBindingData(m_missingBindings[index]);
            };

            // Set a fixed item height
            //m_LogList.fixedItemHeight = 45;

            // Set the actual item's source list/array
            missingBindingsListView.itemsSource = m_missingBindings;
        }

       
    }
    public class MissingItemDisplay
    {
        Label m_bindingLabel;

        public void SetVisualElement(VisualElement visualElement)
        {
            m_bindingLabel = visualElement.Q<Label>("binding-label");
        }

        public void SetBindingData(MissingBindingsListController.MissingBindingItem bindingItem)
        {
            m_bindingLabel.text = bindingItem.buttonName;
        }
    }
}
