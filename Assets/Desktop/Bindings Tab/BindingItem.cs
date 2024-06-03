using System;

namespace EVRC.Desktop
{
    public enum BindingItemState
    {
        Good,
        MissingRecommended,
        MissingHolographic,
        MissingRequired
    }

    [Serializable]
    public class BindingItem
    {
        public string name;
        public string keyValue;
        public string deviceValue;
        public string deviceIndexValue;
        public BindingItemState state;
    }
}
