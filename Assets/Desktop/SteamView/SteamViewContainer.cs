using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class SteamViewContainer : MonoBehaviour
    {
        [SerializeField] UIDocument parentUIDocument;
        Label deviceValueElement;
        Label steamBindingVersionElement;

        private void OnEnable()
        {
            VisualElement root = parentUIDocument.rootVisualElement;

            deviceValueElement = root.Q<Label>("vr-device-value");
            steamBindingVersionElement = root.Q<Label>("steamvr-binding-version");
        }

        
    }
}
