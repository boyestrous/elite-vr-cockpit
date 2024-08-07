using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// Singleton manager of all overlay elements in the scene. 
    /// </summary>
    public class OverlayManager : MonoBehaviour
    {
        public GameEvent overlayStateLoaded;

        private StaticLocationsManager staticLocationsManager;
        private ControlButtonManager controlButtonManager;
        private CockpitModeManager cockpitModeManager;

        //private SavedStateFile loadedState;
        [SerializeField] private SavedGameState savedGameState;

        void Awake()
        {
            staticLocationsManager = GetComponentInChildren<StaticLocationsManager>(true);
            controlButtonManager = GetComponentInChildren<ControlButtonManager>(true);
            cockpitModeManager = GetComponentInChildren<CockpitModeManager>(true);
        }

        void OnEnable()
        {
            LoadAndPlace();
        }

        private IEnumerator PlaceLoadedObjects(System.Action callback)
        {
            yield return StartCoroutine(controlButtonManager.PlaceWhenReady(savedGameState.controlButtons));
            yield return StartCoroutine(staticLocationsManager.PlaceWhenReady(savedGameState.staticLocations));

            callback?.Invoke();
        }


        public void OnEditLockChanged(bool editLocked)
        {
            // When unlocking, no need to do anything
            if (!editLocked) { return; }

            // Get Current State and save to file
            SavedStateFile currentState = new SavedStateFile();
            currentState.version = Paths.currentOverlayFileVersion;
            currentState.staticLocations = staticLocationsManager.GetCurrentStates();
            currentState.controlButtons = controlButtonManager.GetCurrentStates();
            OverlayFileUtils.WriteToFile(currentState);
        }

        public void LoadAndPlace()
        {
            if (savedGameState.GetStatusText() != "Loaded")
            {
                savedGameState.Load();
            }

            //Start all of the placement Coroutines, raise the loaded GameEvent when done.
            StartCoroutine(PlaceLoadedObjects(() => overlayStateLoaded.Raise()));
        }

        public void Rebuild()
        {
            controlButtonManager.Clear();
            cockpitModeManager.Clear();
            LoadAndPlace();
        }

    }
}
