using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class HomeView : MonoBehaviour
    {
        public UIDocument uiDocument;
        public VisualElement root;
        public Button gettingStartedLink;
        public Button gettingStartedYoutubeLink;
        public Button reportBugLink;
        public Button oculusWorkaroundLink;

        // Start is called before the first frame update
        void Start()
        {
            root = uiDocument.rootVisualElement;

            // Find the button and assign the click event
            gettingStartedLink = root.Q<Button>("getting-started-link");
            gettingStartedLink.clicked += () =>
            {
                Application.OpenURL("https://github.com/boyestrous/elite-vr-cockpit/blob/main/Assets/Documentation/GETTING-STARTED.md");
            };

            gettingStartedYoutubeLink = root.Q<Button>("getting-started-youtube-link");
            gettingStartedYoutubeLink.clicked += () => {
                Application.OpenURL("https://www.youtube.com/channel/UCZcxz-04m5DO8kJgm1qorFA");
            };

            reportBugLink = root.Q<Button>("report-bug-link");
            reportBugLink.clicked += () =>
            {
                Application.OpenURL("https://github.com/boyestrous/elite-vr-cockpit/issues");
            };

            oculusWorkaroundLink = root.Q<Button>("oculus-workaround-link");
            oculusWorkaroundLink.clicked += () =>
            {
                Application.OpenURL("https://github.com/boyestrous/elite-vr-cockpit/blob/main/Assets/Documentation/OCULUS-WORKAROUND.md");
            };

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
