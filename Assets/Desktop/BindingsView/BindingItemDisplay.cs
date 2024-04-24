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

        public void SetVisualElement(VisualElement visualElement)
        {
            m_bindingLabel = visualElement.Q<Label>("binding-label");
            m_keyValue = visualElement.Q<Label>("key-value");
            m_deviceValue = visualElement.Q<Label>("device-value");
            m_deviceIndex = visualElement.Q<Label>("index-value");
            m_errorMessage = visualElement.Q<Label>("error-message");
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
            if (bindingItem.state == BindingItemState.MissingHolographic)
            {
                m_rowContainer.AddToClassList("errorRow");
                m_deviceValue.text = "N/A";
                m_keyValue.text = "MISSING";
                m_errorMessage.style.display = DisplayStyle.Flex; 
                m_errorMessage.text = "There is a holographic button configured for this control. A binding is required in order to operate a holographic button";
            }

            if (bindingItem.state == BindingItemState.MissingRequired)
            {
                m_rowContainer.AddToClassList("errorRow");
                m_deviceValue.text = "N/A";
                m_keyValue.text = "MISSING";
                m_errorMessage.style.display = DisplayStyle.Flex;
                m_errorMessage.text = "This binding is required for EVRC functions.";
            }
        }
    }
}
