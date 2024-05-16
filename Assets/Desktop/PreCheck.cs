using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Desktop
{
    [RequireComponent(typeof(PreLaunchController))]
    public class PreCheck : MonoBehaviour
    {
        public PreLaunchController preLaunchController;

        public virtual void OnEnable()
        {
            preLaunchController = gameObject.GetComponentInParent<PreLaunchController>();
        }
    }
}
