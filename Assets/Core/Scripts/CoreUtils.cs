using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EVRC.Core
{
    public static class CoreUtils
    {
        //Slight delay is required to make this load correctly. Otherwise, the desktop UI tries to populate stuff to fast
        public static async Task DelayAndExecute(Action methodToExecute, int delayInSeconds)
        {
            await Task.Delay(delayInSeconds * 1000);
            methodToExecute();
        }
    }
}
