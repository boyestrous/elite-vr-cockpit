using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class BindingsParentController : MonoBehaviour
    {
        public MissingBindingsListController missingBindingsListController;
        public BindingItemsListController BindingItemsListController;

        VisualElement missingBindingsContainer;
        VisualElement vJoyBindingsContainer;

        [SerializeField] UIDocument parentUIDocument;

        public void OnEnable()
        {
            VisualElement root = parentUIDocument.rootVisualElement;
            
            //parent containers for the whole section
            missingBindingsContainer = root.Q<VisualElement>("MissingBindingsList"); 
            vJoyBindingsContainer = root.Q<VisualElement>("BindingItemsList"); 

            if (missingBindingsContainer == null || vJoyBindingsContainer == null) 
            {
                Debug.LogError("Cannot get reference to bindings containers in Desktop UI");
            }
        }


        public void Refresh()
        {
            BindingItemsListController.Refresh();

            bool missingBindings = missingBindingsListController.ValidateBindings();
            // If bindings are missing, display the missingBindings List
            missingBindingsContainer.style.display = missingBindings ? DisplayStyle.Flex : DisplayStyle.None;
            missingBindingsContainer.style.width = missingBindings ? Length.Percent(50) : 0;
            vJoyBindingsContainer.style.width = missingBindings ? Length.Percent(50) : Length.Percent(100);


        }
    }
}
