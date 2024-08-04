using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class IssueSubmitter : MonoBehaviour
    {

        [SerializeField] public UIDocument parentUIDocument;

        private VisualElement root; // the root of the whole UI

        private Button githubButton;

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;

            githubButton = root.Q<Button>("submit-github-issue");

            string url = "https://github.com/boyestrous/elite-vr-cockpit/issues/new";
            githubButton.clicked += () => Application.OpenURL(url);


        }
    }
}
