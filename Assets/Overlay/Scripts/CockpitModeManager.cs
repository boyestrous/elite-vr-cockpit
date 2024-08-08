using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC
{
    public class CockpitModeManager : MonoBehaviour
    {
        [SerializeField] private List<CockpitModeAnchor> modeAnchors;

        internal void OnEnable()
        {
            modeAnchors = FindObjectsOfType<CockpitModeAnchor>(true).ToList();
        }

        public void Clear()
        {
            // Anchors are dynamically added, so we need to re-acquire the list of anchors to be able to remove all the targets
            modeAnchors = FindObjectsOfType<CockpitModeAnchor>(true).ToList();

            foreach (CockpitModeAnchor anchor in modeAnchors)
            {
                anchor.TargetList.Clear();
            }
        }


    }
}
