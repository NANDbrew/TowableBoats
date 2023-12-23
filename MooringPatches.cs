using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using SailwindModdingHelper;
using System.Collections;

namespace TowableBoats
{
    internal class MooringPatches
    {
        private static GameObject bollard;



        [HarmonyPatch(typeof(GPButtonDockMooring))]
        private static class GPButtonDockMooringPatches
        {
            [HarmonyPatch("Awake")]
            [HarmonyPrefix]
            public static void MooringComponentPatch(GPButtonDockMooring __instance)
            {
                if (!bollard) bollard = __instance.gameObject;
                //Plugin.logSource.LogInfo("GPButtonDockMooring is awake");
            }
        }

        [HarmonyPatch(typeof(MooringSet))]
        private static class MooringSetPatches
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void MooringComponentPatch(MooringSet __instance)
            {
                TowingSet towingManager = __instance.transform.parent.gameObject.AddComponent<TowingSet>();
                __instance.StartCoroutine(AddBollards(towingManager));

            }
            private static IEnumerator AddBollards(TowingSet towingManager)
            {
                Debug.Log("waiting for bollard");
                yield return new WaitUntil(() => bollard != null);
                towingManager.AddBollards(bollard);
                Debug.Log("got bollard");

            }
        }

  
        [HarmonyPatch(typeof(BoatHorizon))]
        private static class BoatHorizonPatch
        {
            [HarmonyPatch("UpdateKinematic")]
            [HarmonyPostfix]
            public static void BoatKinematicPatch(BoatHorizon __instance, ref Rigidbody ___rigidbody)
            {
                if (GameState.sleeping && __instance.gameObject.GetComponent<BoatMooringRopes>().AnyRopeMoored())
                {
                    Rigidbody towedBy = __instance.GetComponentInParent<TowingSet>().towedBy;
                    for (int i = 0; i < 5; i++)
                    {
                        if (towedBy.transform == GameState.currentBoat || towedBy.transform == GameState.lastBoat)
                        {
                            //Plugin.logSource.LogInfo("found Boat");

                            ___rigidbody.isKinematic = false;
                            break;
                        }
                        if (towedBy.gameObject.GetComponent<TowingSet>().towed)
                        {
                            towedBy = towedBy.gameObject.GetComponent<TowingSet>().towedBy;
                            continue;
                        }
                        break;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(BoatPerformanceSwitcher))]
        private static class BoatPerformanceSwitcherPatch
        {
            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            public static void BoatPerformaceSwitcherPatch(BoatPerformanceSwitcher __instance, ref Rigidbody ___body)
            {
                // check if we're being towed
                if (__instance.GetComponentInParent<TowingSet>().towed && Plugin.performanceMode.Value > 0)
                {                
                    Rigidbody towedBy = __instance.GetComponentInParent<TowingSet>().towedBy;
                    for (int i = 0; i < Plugin.performanceMode.Value; i++)
                    {
                        if (towedBy.transform == GameState.currentBoat || towedBy.transform == GameState.lastBoat)
                        {
                            //Plugin.logSource.LogInfo("found Boat");

                            __instance.InvokePrivateMethod("SetPerformanceMode", false);
                            break;
                        }
                        if (towedBy.gameObject.GetComponent<TowingSet>().towed)
                        {
                            towedBy = towedBy.gameObject.GetComponent<TowingSet>().towedBy;
                            continue;
                        }
                        break;
                    }
                }
                // check if we're towing something
                if (__instance.GetComponentInParent<TowingSet>().towing)
                {
                    //Plugin.logSource.LogInfo("found bollard");
                    List<Rigidbody> towedBoats = ___body.gameObject.GetComponent<TowingSet>().towedBoats;
                    for (int i = 0; i < Plugin.performanceMode.Value; i++)
                    {
                        foreach (Rigidbody body1 in towedBoats)
                        {
                            if (body1.transform == GameState.currentBoat || body1.transform == GameState.lastBoat)
                            {
                                //Plugin.logSource.LogInfo("found last/current Boat");
                                __instance.InvokePrivateMethod("SetPerformanceMode", false);
                                return;
                            }
                            if (body1.gameObject.GetComponent<TowingSet>().towing)
                            {
                                towedBoats = body1.gameObject.GetComponent<TowingSet>().towedBoats;
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PickupableBoatMooringRope))]
        private static class BoatMooringRopePatches
        {
            [HarmonyPatch("ThrowRopeTo")]
            [HarmonyPrefix]
            public static bool ThrowRopeToPatch(PickupableBoatMooringRope __instance, GPButtonDockMooring mooring, bool ___throwing, Rigidbody ___boatRigidbody)
            {
                if (mooring.gameObject.CompareTag("Boat"))
                {
                    //if (___boatRigidbody.gameObject.GetComponentInChildren<TowingManager>().isTowedBy != null && ___boatRigidbody.gameObject.GetComponentInChildren<TowingManager>().isTowedBy.Any() != mooring.GetComponentInParent<TowingManager>().GetBoatTransform()) return false;

                    if (___boatRigidbody.transform != mooring.GetComponentInParent<TowingSet>().GetBoatTransform())
                    {
                        if (Plugin.multiParent.Value == true || !__instance.GetComponentInParent<TowingSet>().towed || mooring.gameObject.GetComponentInParent<TowingSet>().GetBoatTransform() == __instance.GetComponentInParent<TowingSet>().towedBy.transform)
                        {
                            __instance.MoorTo(mooring);
                            mooring.GetComponentInParent<TowingSet>().towing = true;
                            ___boatRigidbody.gameObject.GetComponent<TowingSet>().towed = true;
                        }
                    }
                    return false;
                }
                return true;
            }
            [HarmonyPatch("Unmoor")]
            [HarmonyPrefix]
            public static void UnmoorPatch(SpringJoint ___mooredToSpring, ref TowingSet __state)
            {
                if (___mooredToSpring != null)
                {
                    //if (___boatRigidbody.gameObject.GetComponentInChildren<TowingManager>().isTowedBy != null && ___boatRigidbody.gameObject.GetComponentInChildren<TowingManager>().isTowedBy.Any() != mooring.GetComponentInParent<TowingManager>().GetBoatTransform()) return false;
                    if (___mooredToSpring.GetComponentInParent<TowingSet>())
                    {
                        __state = ___mooredToSpring.GetComponentInParent<TowingSet>();
                    }
                }
            }
            [HarmonyPatch("Unmoor")]
            [HarmonyPostfix]
            public static void UnmoorPostPatch(TowingSet __state, Rigidbody ___boatRigidbody)
            {
                bool towed = false;

                if (__state)
                {
                    if (___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().AnyRopeMoored())
                    {
                        PickupableBoatMooringRope[] array = ___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().ropes;
                        //Plugin.logSource.LogInfo(array);
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i].GetPrivateField<SpringJoint>("mooredToSpring").gameObject.CompareTag("Boat"))
                            {
                                towed = true;
                                break;
                            }
                        }

                    }
                    __state.GetTowedBoat();
                }
                ___boatRigidbody.gameObject.GetComponent<TowingSet>().towed = towed;

            }
        }
    }
}
