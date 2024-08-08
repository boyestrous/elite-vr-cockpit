using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public static class LegacyOverlayStates
    {

        /// <summary>
        /// Sample JSON from a version 4 sample file with 1 item in each of the major categories
        /// </summary>
        public static string version4json = @"{
    ""version"": 4,
    ""staticLocations"": {
        ""metaPanel"": {
            ""pos"": {
                ""x"": 0.6430835723876953,
                ""y"": -0.7182695865631104,
                ""z"": 0.19057822227478028
            },
            ""rot"": {
                ""x"": 42.27288055419922,
                ""y"": 63.87510299682617,
                ""z"": 6.786276817321777
            }
        },
        ""mapPlaneController"": {
            ""pos"": {
                ""x"": 0.6430835723876953,
                ""y"": -0.7182695865631104,
                ""z"": 0.19057822227478028
            },
            ""rot"": {
                ""x"": 42.27288055419922,
                ""y"": 63.87510299682617,
                ""z"": 6.786276817321777
            }
        },
        ""shipThrottle"": {
            ""pos"": {
                ""x"": -0.2424752116203308,
                ""y"": -0.4337214231491089,
                ""z"": 0.4214426577091217
            },
            ""rot"": {
                ""x"": 356.91796875,
                ""y"": 358.66851806640627,
                ""z"": 3.049741506576538
            }
        },
        ""srvThrottle"": {
            ""pos"": {
                ""x"": -0.23925966024398805,
                ""y"": -0.3998030424118042,
                ""z"": 0.38963639736175539
            },
            ""rot"": {
                ""x"": 0.0,
                ""y"": 0.0,
                ""z"": 0.0
            }
        },
        ""shipJoystick"": {
            ""pos"": {
                ""x"": 0.286592572927475,
                ""y"": -0.5523958802223206,
                ""z"": 0.46413975954055788
            },
            ""rot"": {
                ""x"": 0.0,
                ""y"": 0.0,
                ""z"": 0.0
            }
        },
        ""srvJoystick"": {
            ""pos"": {
                ""x"": 0.30446121096611025,
                ""y"": -0.5019428730010986,
                ""z"": 0.4471163749694824
            },
            ""rot"": {
                ""x"": 0.0,
                ""y"": 0.0,
                ""z"": 0.0
            }
        },
        ""shipPowerDeliveryPanel"": {
            ""pos"": {
                ""x"": 0.5052585005760193,
                ""y"": -0.4378976821899414,
                ""z"": 0.5127792954444885
            },
            ""rot"": {
                ""x"": 14.50324821472168,
                ""y"": 36.54065704345703,
                ""z"": 359.7928466796875
            }
        },
        ""srvPowerDeliveryPanel"": {
            ""pos"": {
                ""x"": 0.3499999940395355,
                ""y"": -0.3199999928474426,
                ""z"": 0.550000011920929
            },
            ""rot"": {
                ""x"": 10.00000286102295,
                ""y"": 34.99998474121094,
                ""z"": -4.334721950272069e-7
            }
        },
        ""sixDofController"": {
            ""pos"": {
                ""x"": -0.01766963303089142,
                ""y"": -0.4408041834831238,
                ""z"": 0.55136638879776
            },
            ""rot"": {
                ""x"": 0.0,
                ""y"": 0.0,
                ""z"": 0.0
            }
        }
    },
    ""controlButtons"": [
        {
            ""type"": ""SetSpeedZero"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.3058055341243744,
                    ""y"": -0.37422531843185427,
                    ""z"": 0.5306997299194336
                },
                ""rot"": {
                    ""x"": 32.86201477050781,
                    ""y"": 338.933837890625,
                    ""z"": 0.7185190320014954
                }
            }
        },
        {
            ""type"": ""DisableRotationCorrectToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.5700806379318237,
                    ""y"": -0.37424367666244509,
                    ""z"": 0.48769205808639529
                },
                ""rot"": {
                    ""x"": 24.980066299438478,
                    ""y"": 35.90341567993164,
                    ""z"": 359.565185546875
                }
            }
        },
        {
            ""type"": ""UseAlternateFlightValuesToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.4312703311443329,
                    ""y"": -0.2522873878479004,
                    ""z"": 0.4161375164985657
                },
                ""rot"": {
                    ""x"": 23.73296356201172,
                    ""y"": 325.5726013183594,
                    ""z"": 359.4088134765625
                }
            }
        },
        {
            ""type"": ""LandingGearToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.7434319257736206,
                    ""y"": 0.10918164253234863,
                    ""z"": 0.1789981871843338
                },
                ""rot"": {
                    ""x"": 2.6257517337799074,
                    ""y"": 71.50733184814453,
                    ""z"": 359.3211669921875
                }
            }
        },
        {
            ""type"": ""NightVisionToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.007317543029785156,
                    ""y"": -0.5104745030403137,
                    ""z"": 0.41632723808288576
                },
                ""rot"": {
                    ""x"": 44.495357513427737,
                    ""y"": 357.0286865234375,
                    ""z"": 359.0043029785156
                }
            }
        },
        {
            ""type"": ""ToggleCargoScoop"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.06592021137475968,
                    ""y"": -0.5114732980728149,
                    ""z"": 0.4143006503582001
                },
                ""rot"": {
                    ""x"": 48.20628356933594,
                    ""y"": 356.9822998046875,
                    ""z"": 359.15557861328127
                }
            }
        },
        {
            ""type"": ""ShipSpotLightToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.05333852767944336,
                    ""y"": -0.5091609358787537,
                    ""z"": 0.4178834557533264
                },
                ""rot"": {
                    ""x"": 44.14359664916992,
                    ""y"": 0.6835651397705078,
                    ""z"": 0.8602958917617798
                }
            }
        },
        {
            ""type"": ""TargetNextRouteSystem"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.15204918384552003,
                    ""y"": -0.36655205488204958,
                    ""z"": 0.5025507807731628
                },
                ""rot"": {
                    ""x"": 32.633270263671878,
                    ""y"": 17.998310089111329,
                    ""z"": 0.09995715320110321
                }
            }
        },
        {
            ""type"": ""DeployHardpointToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.2197645604610443,
                    ""y"": -0.3154599666595459,
                    ""z"": 0.5195396542549133
                },
                ""rot"": {
                    ""x"": 32.03402328491211,
                    ""y"": 18.281299591064454,
                    ""z"": 359.98822021484377
                }
            }
        },
        {
            ""type"": ""ToggleAdvanceMode"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.4767804443836212,
                    ""y"": -0.3762210011482239,
                    ""z"": 0.5661931037902832
                },
                ""rot"": {
                    ""x"": 28.946428298950197,
                    ""y"": 37.54703140258789,
                    ""z"": 359.0314025878906
                }
            }
        },
        {
            ""type"": ""GalaxyMapOpen"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.5346707701683044,
                    ""y"": -0.2795320749282837,
                    ""z"": 0.2708769142627716
                },
                ""rot"": {
                    ""x"": 25.449138641357423,
                    ""y"": 300.9561767578125,
                    ""z"": 1.1529165506362916
                }
            }
        },
        {
            ""type"": ""DeployHeatSink"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.18014338612556458,
                    ""y"": -0.3954036831855774,
                    ""z"": 0.5514640212059021
                },
                ""rot"": {
                    ""x"": 27.025285720825197,
                    ""y"": 355.25390625,
                    ""z"": 359.91290283203127
                }
            }
        },
        {
            ""type"": ""FireChaffLauncher"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.2411087453365326,
                    ""y"": -0.3646922707557678,
                    ""z"": 0.5697704553604126
                },
                ""rot"": {
                    ""x"": 26.27860450744629,
                    ""y"": 347.5660095214844,
                    ""z"": 358.0083923339844
                }
            }
        },
        {
            ""type"": ""NightVisionToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.23012615740299226,
                    ""y"": -0.25919365882873537,
                    ""z"": 0.5504413843154907
                },
                ""rot"": {
                    ""x"": 26.9667911529541,
                    ""y"": 17.8153133392334,
                    ""z"": 359.0630798339844
                }
            }
        },
        {
            ""type"": ""OrbitLinesToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.17056331038475038,
                    ""y"": -0.25754880905151369,
                    ""z"": 0.5677188038825989
                },
                ""rot"": {
                    ""x"": 25.460723876953126,
                    ""y"": 17.124279022216798,
                    ""z"": 358.9051818847656
                }
            }
        },
        {
            ""type"": ""PlayerHUDModeToggle"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.5246438980102539,
                    ""y"": -0.37504732608795168,
                    ""z"": 0.5273261666297913
                },
                ""rot"": {
                    ""x"": 24.31182098388672,
                    ""y"": 37.0345573425293,
                    ""z"": 359.6915283203125
                }
            }
        },
        {
            ""type"": ""Hyperspace"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.3799280524253845,
                    ""y"": -0.30641746520996096,
                    ""z"": 0.43306058645248415
                },
                ""rot"": {
                    ""x"": 22.38573455810547,
                    ""y"": 322.63726806640627,
                    ""z"": 357.72589111328127
                }
            }
        },
        {
            ""type"": ""HyperSuperCombination"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.4215417504310608,
                    ""y"": -0.3028174638748169,
                    ""z"": 0.3973831236362457
                },
                ""rot"": {
                    ""x"": 20.195205688476564,
                    ""y"": 320.22625732421877,
                    ""z"": 357.19671630859377
                }
            }
        },
        {
            ""type"": ""ExplorationFSSEnter"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.170242041349411,
                    ""y"": -0.34242749214172366,
                    ""z"": 0.685775637626648
                },
                ""rot"": {
                    ""x"": 28.95980453491211,
                    ""y"": 356.9845275878906,
                    ""z"": 359.8040466308594
                }
            }
        },
        {
            ""type"": ""ExplorationFSSQuit"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.170242041349411,
                    ""y"": -0.34242749214172366,
                    ""z"": 0.685775637626648
                },
                ""rot"": {
                    ""x"": 28.95980453491211,
                    ""y"": 356.9845275878906,
                    ""z"": 359.8040466308594
                }
            }
        },
        {
            ""type"": ""RecallDismissShip"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.047402381896972659,
                    ""y"": 0.259108304977417,
                    ""z"": 0.5878440141677856
                },
                ""rot"": {
                    ""x"": 347.4894104003906,
                    ""y"": 3.611732244491577,
                    ""z"": 358.88360595703127
                }
            }
        },
        {
            ""type"": ""ToggleCargoScoop_Buggy"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.1207604929804802,
                    ""y"": 0.25867605209350588,
                    ""z"": 0.566266655921936
                },
                ""rot"": {
                    ""x"": 342.8597412109375,
                    ""y"": 13.969743728637696,
                    ""z"": 358.09002685546877
                }
            }
        },
        {
            ""type"": ""ToggleDriveAssist"",
            ""loc"": {
                ""pos"": {
                    ""x"": -0.17632296681404115,
                    ""y"": -0.43846189975738528,
                    ""z"": 0.5864892601966858
                },
                ""rot"": {
                    ""x"": 41.47140884399414,
                    ""y"": 357.3030090332031,
                    ""z"": 359.52435302734377
                }
            }
        },
        {
            ""type"": ""ToggleBuggyTurretButton"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.20629365742206574,
                    ""y"": -0.3947014808654785,
                    ""z"": 0.49978166818618777
                },
                ""rot"": {
                    ""x"": 20.391891479492189,
                    ""y"": 14.896941184997559,
                    ""z"": 2.683077096939087
                }
            }
        },
        {
            ""type"": ""HeadlightsBuggyButton"",
            ""loc"": {
                ""pos"": {
                    ""x"": 0.06548576802015305,
                    ""y"": 0.26130104064941409,
                    ""z"": 0.5779727101325989
                },
                ""rot"": {
                    ""x"": 345.32208251953127,
                    ""y"": 10.028312683105469,
                    ""z"": 359.3002014160156
                }
            }
        }
    ],
    ""booleanSettings"": [
        {
            ""name"": ""joystick.enabled"",
            ""value"": true
        },
        {
            ""name"": ""throttle.enabled"",
            ""value"": true
        },
        {
            ""name"": ""sixDofController.enabled"",
            ""value"": true
        },
        {
            ""name"": ""powerDistributionPanel.enabled"",
            ""value"": true
        }
    ]
}";

        [Serializable]
        public struct Version4Struct
        {
            [Serializable]
            public struct SavedTransform
            {
                public Vector3 pos;
                public Vector3 rot;
            }

            [Serializable]
            public struct StaticLocations
            {
                public SavedTransform metaPanel;
                public SavedTransform shipThrottle;
                public SavedTransform srvThrottle;
                public SavedTransform shipJoystick;
                public SavedTransform srvJoystick;
                public SavedTransform shipPowerDeliveryPanel;
                public SavedTransform srvPowerDeliveryPanel;
                public SavedTransform sixDofController;
                public SavedTransform mapPlaneController;
            }

            [Serializable]
            public struct SavedControlButton
            {
                public string type;
                public SavedTransform loc;
            }

            [Serializable]
            public struct V4SavedBooleanSetting
            {
                public string name;
                public bool value;

                public static implicit operator SavedBooleanSetting(V4SavedBooleanSetting source)
                {
                    return new SavedBooleanSetting
                    {
                        name = source.name,
                        value = source.value
                    };
                }
            }

            public int version;
            public StaticLocations staticLocations;
            public SavedControlButton[] controlButtons;
            public SavedBooleanSetting[] booleanSettings;
        }
    }
}
