using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class BindingItemDisplay
    {
        Label m_bindingLabel;
        Label m_keyValue;
        Label m_deviceValue;
        Label m_deviceIndex;
        Label m_errorMessage;
        VisualElement m_rowContainer;
        public Button m_autoFixButton;
        public DropdownField m_chooseKeyDropdown;

        public void SetVisualElement(VisualElement visualElement)
        {
            m_bindingLabel = visualElement.Q<Label>("binding-label");
            m_keyValue = visualElement.Q<Label>("key-value");
            m_deviceValue = visualElement.Q<Label>("device-value");
            m_deviceIndex = visualElement.Q<Label>("index-value");
            m_errorMessage = visualElement.Q<Label>("error-message");
            m_autoFixButton = visualElement.Q<Button>("auto-fix-button");
            m_chooseKeyDropdown = visualElement.Q<DropdownField>("choose-key-dropdown");
            m_rowContainer = visualElement;
        }

        public void SetBindingData(BindingItem bindingItem)
        {
            m_bindingLabel.text = bindingItem.name; 
            m_keyValue.text = bindingItem.keyValue;
            m_deviceValue.text = bindingItem.deviceValue;
            m_deviceIndex.text = bindingItem.deviceIndexValue;
            AddStatusStyles(bindingItem);
        }

        public void AddStatusStyles(BindingItem bindingItem)
        {
            // Reset the styles, so the ListView doesn't retain the styles on the recycled pool items
            m_rowContainer.RemoveFromClassList("errorRow");
            m_autoFixButton.style.display = DisplayStyle.None;
            m_errorMessage.style.display = DisplayStyle.None;
            m_errorMessage.text = "Placeholder Error Text";

            if (bindingItem.state == BindingItemState.MissingHolographic)
            {
                m_rowContainer.AddToClassList("errorRow");
                m_deviceValue.text = "N/A";
                m_keyValue.text = "MISSING";
                m_autoFixButton.style.display = DisplayStyle.Flex;
                m_errorMessage.style.display = DisplayStyle.Flex; 
                m_errorMessage.text = "There is a holographic button configured for this control. You can set this binding to ANY valid key";
            }

            if (bindingItem.state == BindingItemState.MissingRequired)
            {
                m_rowContainer.AddToClassList("errorRow");
                m_deviceValue.text = "N/A";
                m_keyValue.text = "MISSING";
                m_autoFixButton.style.display = DisplayStyle.Flex;
                m_errorMessage.style.display = DisplayStyle.Flex;
                m_errorMessage.text = "This binding is required for EVRC functions. Try to set it to something memorable...";
            }
        }
    }
}
