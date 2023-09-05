//using NUnit.Framework;
//using UnityEngine;
//using EVRC.Core;
//using EVRC.Core.Tests;

//public class EDStateManagerTests
//{
//    private EDStateManager edStateManager;
//    private EliteDangerousState edState;
//    private GameObject gameObject;

//    // Required GameEvents
//    private EDStateEvent edStateEvent;
//    private BoolEvent startStopEvent;
//    private EDStatusFlagsEvent edStatusFlagsEvent;
//    private EDGuiFocusEvent edGuiFocusEvent;

//    [SetUp]
//    public void Setup()
//    {
//        // Create State Objects for testing
//        edState = ScriptableObject.CreateInstance<EliteDangerousState>();

//        //Create GameEvents objects for testing
//        edStateEvent = ScriptableObject.CreateInstance <EDStateEvent>();
//        startStopEvent = ScriptableObject.CreateInstance<BoolEvent>();
//        edStatusFlagsEvent = ScriptableObject.CreateInstance<EDStatusFlagsEvent>();
//        edGuiFocusEvent = ScriptableObject.CreateInstance<EDGuiFocusEvent>();

//        // Create GameObjects and attach components/scriptableObjects
//        gameObject = new GameObject("GameObject");
//        edStateManager = gameObject.AddComponent<EDStateManager>();
//        edStateManager.eliteDangerousState = edState;

//        // Attach events
//        edStateManager.statusChanged = edStateEvent;
//        edStateManager.eliteDangerousStartStop = startStopEvent;
//        edStateManager.eDStatusFlagsEvent = edStatusFlagsEvent;
//        edStateManager.eDGuiFocusEvent = edGuiFocusEvent;


//    }

//    public struct StatusFileTestCase
//    {
//        public string rawFileText;
//        public string testCaseDescription;
//        public EDStatusFlags? expectedStatusFlags;
//        public EDStatusFlags2? expectedStatusFlags2;
//        public EDGuiFocus? expectedGuiFocus;

//        // Customize the display of the test case in the TestRunner Window2
//        public override string ToString()
//        {

//            return $"{testCaseDescription}";
//        }
//    }

//    private static StatusFileTestCase[] StatusFileTestCases =
//    {
//        // Status File sample from sitting in a station hangar
//        new StatusFileTestCase()
//        {
//            testCaseDescription = "Ship In Hangar",
//            rawFileText = @"{
//                    ""timestamp"": ""2023-08-12T14:50:28Z"",
//                    ""event"": ""Status"",
//                    ""Flags"": 151060493,
//                    ""Flags2"": 0,
//                    ""Pips"": [
//                    4,
//                    8,
//                    0
//                    ],
//                    ""FireGroup"": 1,
//                    ""GuiFocus"": 0,
//                    ""Fuel"": {
//                    ""FuelMain"": 8.0,
//                    ""FuelReservoir"": 0.391909
//                    },
//                    ""Cargo"": 0.0,
//                    ""LegalState"": ""Clean"",
//                    ""Balance"": 133031956
//                }",
//            expectedStatusFlags = EDStatusFlags.Docked | EDStatusFlags.LandingGearDown | EDStatusFlags.ShieldsUp | EDStatusFlags.FsdMassLocked | EDStatusFlags.InMainShip | EDStatusFlags.HudInAnalysisMode,
//            expectedGuiFocus = 0,
//            expectedStatusFlags2 = 0

//        },

//        // Status File sample from using the galaxy map while sitting in a station hangar
//        new StatusFileTestCase()
//        {
//            testCaseDescription = "Galaxy Map while Ship in Hanger",
//            rawFileText = @"{
//                    ""timestamp"": ""2023-08-12T14:50:28Z"",
//                    ""event"": ""Status"",
//                    ""Flags"": 151060493,
//                    ""Flags2"": 0,
//                    ""Pips"": [
//                    4,
//                    8,
//                    0
//                    ],
//                    ""FireGroup"": 1,
//                    ""GuiFocus"": 6,
//                    ""Fuel"": {
//                    ""FuelMain"": 8.0,
//                    ""FuelReservoir"": 0.391909
//                    },
//                    ""Cargo"": 0.0,
//                    ""LegalState"": ""Clean"",
//                    ""Balance"": 133031956
//                }",
//            expectedStatusFlags = EDStatusFlags.Docked | EDStatusFlags.LandingGearDown | EDStatusFlags.ShieldsUp | EDStatusFlags.FsdMassLocked | EDStatusFlags.InMainShip | EDStatusFlags.HudInAnalysisMode,
//            expectedGuiFocus = EDGuiFocus.GalaxyMap,
//            expectedStatusFlags2 = 0

//        },
        
//        //// On Foot in a station, just disembarked from a ship
//        new StatusFileTestCase()
//        {
//            testCaseDescription = "On Foot In Hangar",
//            rawFileText = @"{
//                ""timestamp"": ""2023-08-12T14:50:23Z"",
//                ""event"": ""Status"",
//                ""Flags"": 0,
//                ""Flags2"": 90121,
//                ""Oxygen"": 1.0,
//                ""Health"": 1.0,
//                ""Temperature"": 293.0,
//                ""SelectedWeapon"": """",
//                ""LegalState"": ""Clean"",
//                ""BodyName"": ""Shajn Market"",
//                ""Balance"": 133031956
//            }",
//            expectedStatusFlags = 0,
//            expectedGuiFocus = 0,
//            expectedStatusFlags2 = EDStatusFlags2.OnFoot | EDStatusFlags2.OnFootInStation | EDStatusFlags2.OnFootInHangar | EDStatusFlags2.OnFootSocialSpace | EDStatusFlags2.BreathableAtmosphere


//        },
//        // Game not running
//        new StatusFileTestCase()
//        {
//            testCaseDescription = "Game Not Running",
//            rawFileText = @"{
//                ""timestamp"": ""2023-08-12T15:07:28Z"",
//                ""event"": ""Status"",
//                ""Flags"": 0
//            }",
//            expectedStatusFlags = 0,
//            expectedGuiFocus = 0,
//            expectedStatusFlags2 = 0
//        }        

//    };

//    /// <summary>
//    /// Tests to confirm that the StatusFlags are accurately read from various forms of the Status.json file
//    /// </summary>
//    [Test, TestCaseSource(nameof(StatusFileTestCases))]
//    public void EDStateManager_Reads_Enums_Accurately(StatusFileTestCase testCase)
//    {
//        #region -------------- Arrange ---------------------
//        // Read the raw text into Json
//        EDStatus eliteStatus = JsonUtility.FromJson<EDStatus>(testCase.rawFileText);

//        // Catch the EDStatusFlags & EDStatusFlags2 that should be raised in an event
//        EDStatusFlags flagsFromEvent = 0;
//        EDStatusFlags2 flags2FromEvent = 0;
//        edStatusFlagsEvent.Event += (raisedFlags, raisedFlags2) =>
//            {
//                flagsFromEvent = raisedFlags;
//                flags2FromEvent = raisedFlags2;
//            };

//        // Catch the GuiFocus that should be raised in an event
//        EDGuiFocus guiFocusFromEvent = 0;
//        edGuiFocusEvent.Event += (raisedGuiFocus) => guiFocusFromEvent = raisedGuiFocus;

//        #endregion


//        // -------------- Act ---------------------
//        edStateManager.UpdateFlags(eliteStatus);
//        edStateManager.UpdateGuiFocus(eliteStatus);


//        // -------------- Assert ---------------------
//        Assert.AreEqual(flagsFromEvent, testCase.expectedStatusFlags);
//        Assert.AreEqual(guiFocusFromEvent, testCase.expectedGuiFocus);
//        Assert.AreEqual(flags2FromEvent, testCase.expectedStatusFlags2);

//        // -------------- Cleanup ---------------------
//        edStatusFlagsEvent.Event -= (raisedFlags, raisedFlags2) =>
//        {
//            flagsFromEvent = raisedFlags;
//            flags2FromEvent = raisedFlags2;
//        };
//        edGuiFocusEvent.Event -= (raisedGuiFocus) => guiFocusFromEvent = raisedGuiFocus;

//    }

//    // ---------------- CockpitModeAnchor tests ---------------------------
//    // Does the MainShip anchor deactivate when the flags value is raised in the OnFoot situation?
//    // Do we really need a 3rd anchor setting for Flags2?
//}
