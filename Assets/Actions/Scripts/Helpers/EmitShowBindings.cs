﻿using UnityEngine;

namespace EVRC.Core.Actions
{
    /**
     * Helper that shows bindings
     */
    public class EmitShowBindings : MonoBehaviour
    {
        public BindingsHintCategory hintCategory = BindingsHintCategory.Default;

        public void Emit()
        {
            var bindingsController = ActionsControllerBindingsLoader.CurrentBindingsController;
            if (bindingsController != null)
            {
                bindingsController.ShowBindings(hintCategory);
            }
        }
    }
}
