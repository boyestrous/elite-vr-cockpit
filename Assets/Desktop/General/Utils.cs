using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public static class Utils
    {
        public static Vector2 FindCenterForModal(UIDocument parentUIDocument)
        {
            Vector2 center = Vector2.zero;

            // Get the VisualElement representing the ListView
            VisualElement contentContainer = parentUIDocument.rootVisualElement.Q<VisualElement>("content");

            // Now you can use the RectTransform contentContainerRectTransform as needed
            // For example, you can get its position, size, etc.
            Vector2 contentContainerPosition = contentContainer.contentRect.position;
            Vector2 contentContainerSize = contentContainer.contentRect.size;

            // Calculate the center position of the ListView
            center = new Vector2(contentContainerPosition.x + (contentContainerSize.x / 2f),
                                                 contentContainerPosition.y + (contentContainerSize.y / 3f));

            return center;
        }

        public static T FindUIElement<T>(VisualElement root, string itemName) where T : VisualElement
        {
            T tempItem;
            tempItem = root.Q<T>(itemName);

            if (tempItem == null)
            {
                Debug.LogError($"Cannot get reference to {typeof(T).Name} with name {itemName} in Desktop UI");
            }

            return tempItem;
        }
    }
}