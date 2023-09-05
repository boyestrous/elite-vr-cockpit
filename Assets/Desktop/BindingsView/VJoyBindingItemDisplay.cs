using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class VJoyBindingItemDisplay
    {
        Label m_bindingLabel;
        Label m_keyValue;
        Label m_deviceValue;
        Label m_deviceIndex;      

        public void SetVisualElement(VisualElement visualElement)
        {
            m_bindingLabel = visualElement.Q<Label>("binding-label");
            m_keyValue = visualElement.Q<Label>("key-value");
            m_deviceValue = visualElement.Q<Label>("device-value");
            m_deviceIndex = visualElement.Q<Label>("index-value");
        }

        public void SetBindingData(VJoyBindingItem bindingItem)
        {
            m_bindingLabel.text = bindingItem.name; 
            m_keyValue.text = bindingItem.keyValue;
            m_deviceValue.text = bindingItem.deviceValue;
            m_deviceIndex.text = bindingItem.deviceIndexValue;
        }
    }
}
