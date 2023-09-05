using EVRC.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class VJoyBindingsListController : MonoBehaviour
    {
        [Header("Templates and Scene Refs")]
        // UXML template for list entries
        [SerializeField] VisualTreeAsset m_BindingEntryTemplate;
        [SerializeField] UIDocument parentUIDocument;
        [SerializeField] ControlBindingsState bindings;

        // UI element references
        ListView requiredBindingListView;
        

        [SerializeField] List<VJoyBindingItem> m_requiredBindings;

        public void OnEnable()
        {
            m_requiredBindings = new List<VJoyBindingItem>();

            VisualElement root = parentUIDocument.rootVisualElement;

            // Store a reference to the log list element
            requiredBindingListView = root.Q<ListView>("required-bindings-list");

            SetListBindingMethods();
            m_requiredBindings.Add(new VJoyBindingItem() 
            { 
                name = "BindingsNotReadYet", 
                keyValue = "Joy_ZYXAxis",
                deviceIndexValue = "-5",
                deviceValue = "Boyestrous"            
            });

            Refresh();
        }

        public void Refresh()
        {
            m_requiredBindings.Clear();

            if (bindings.buttonBindings == null) return;

            foreach(var binding in bindings.buttonBindings)
            {
                if (!binding.Value.HasVJoyKeybinding) continue;

                var tempBindingItem = new VJoyBindingItem()
                {
                    name = binding.Key.ToString(),
                    keyValue = binding.Value.VJoyKeybinding.Value.Key,
                    deviceValue = binding.Value.VJoyKeybinding.Value.Device
                };

                tempBindingItem.deviceIndexValue = binding.Value.VJoyKeybinding.Value.DeviceIndex == null ? null : binding.Value.VJoyKeybinding.Value.DeviceIndex;

                m_requiredBindings.Add(tempBindingItem);
            }

            requiredBindingListView.Rebuild();
        }

        void SetListBindingMethods()
        {
            //Set up a make item function for a list entry
            requiredBindingListView.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = m_BindingEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var newListEntryLogic = new VJoyBindingItemDisplay();

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Initialize the controller script
                newListEntryLogic.SetVisualElement(newListEntry);

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            // Set up bind function for a specific list entry
            requiredBindingListView.bindItem = (item, index) =>
            {
                (item.userData as VJoyBindingItemDisplay).SetBindingData(m_requiredBindings[index]);
            };

            // Set a fixed item height
            //m_LogList.fixedItemHeight = 45;

            // Set the actual item's source list/array
            requiredBindingListView.itemsSource = m_requiredBindings;
        }
    }
}
