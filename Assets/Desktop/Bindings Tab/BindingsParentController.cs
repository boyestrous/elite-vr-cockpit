using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class BindingsParentController : MonoBehaviour
    {
        public BindingItemsListController bindingItemsListController;

        VisualElement allBindingsContainer;

        [SerializeField] UIDocument parentUIDocument;

        public void OnEnable()
        {

            VisualElement root = parentUIDocument.rootVisualElement;
            
            //parent containers for the whole section
            allBindingsContainer = root.Q<VisualElement>("BindingsItemsList");

            if (allBindingsContainer == null) 
            {
                Debug.LogError("Cannot get reference to bindings containers in Desktop UI");
            }
        }

        public void Refresh()
        {
            //bindingItemsListController.RefreshBindingsList();
            bindingItemsListController.FindMissingBindings();
        }
    }
}
