using EVRC.Core;
using System.Collections.Generic;
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

        // UI element references
        ListView requiredBindingListView;
        

        [SerializeField] List<BindingItem> m_requiredBindings;

        public void OnEnable()
        {
            m_requiredBindings = new List<BindingItem>();

            VisualElement root = parentUIDocument.rootVisualElement;

            // Store a reference to the log list element
            requiredBindingListView = root.Q<ListView>("required-bindings-list");

            SetListBindingMethods();
            m_requiredBindings.Add(new BindingItem() 
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
                if (binding.Value.HasVJoyKeybinding)
                {
                    BindingItem tempBindingItem = new BindingItem()
                    {
                        name = binding.Key.ToString(),
                        keyValue = binding.Value.VJoyKeybinding.Value.Key,
                        deviceValue = binding.Value.VJoyKeybinding.Value.Device
                    };
                    tempBindingItem.deviceIndexValue = binding.Value.VJoyKeybinding.Value.DeviceIndex == null ? null : binding.Value.VJoyKeybinding.Value.DeviceIndex;

                    m_requiredBindings.Add(tempBindingItem);
                } 
                else if (binding.Value.HasKeyboardKeybinding)
                {
                    BindingItem tempBindingItem = new BindingItem()
                    {
                        name = binding.Key.ToString(),
                        keyValue = binding.Value.KeyboardKeybinding.Value.Key,
                        deviceValue = binding.Value.KeyboardKeybinding.Value.Device
                    };
                    tempBindingItem.deviceIndexValue = binding.Value.KeyboardKeybinding.Value.DeviceIndex == null ? null : binding.Value.KeyboardKeybinding.Value.DeviceIndex;

                    m_requiredBindings.Add(tempBindingItem);
                } 

                else { continue; }
                
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
                var newListEntryLogic = new BindingItemDisplay();

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
                (item.userData as BindingItemDisplay).SetBindingData(m_requiredBindings[index]);
            };

            // Set a fixed item height
            //m_LogList.fixedItemHeight = 45;

            // Set the actual item's source list/array
            requiredBindingListView.itemsSource = m_requiredBindings;
        }
    }
}
