using HarmonyLib;

using SailwindModdingHelper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TowableBoats
{
    internal class BoatPerformancePatches
    {

        [HarmonyPatch(typeof(BoatHorizon))]
        private static class BoatHorizonPatch
        {
            [HarmonyPatch("UpdateKinematic")]
            [HarmonyPostfix]
            public static void BoatKinematicPatch(BoatHorizon __instance, ref Rigidbody ___rigidbody)
            {

                if (GameState.sleeping)
                {
                    if (__instance.GetComponentInParent<TowingSet>().towed)
                    {
                        Rigidbody towedBy = __instance.GetComponentInParent<TowingSet>().GetTowedBy();
                        for (int i = 0; i < 5; i++)
                        {
                            if (towedBy.transform == GameState.currentBoat || towedBy.transform == GameState.lastBoat)
                            {
                                ___rigidbody.isKinematic = false;
                                break;
                            }
                            if (towedBy.gameObject.GetComponent<TowingSet>().towed)

                            {
                                towedBy = towedBy.gameObject.GetComponent<TowingSet>().GetTowedBy();
                                continue;
                            }
                            break;
                        }
                    }

                    if (__instance.GetComponentInParent<TowingSet>().towing)
                    {
                        List<Rigidbody> towedBoats = __instance.GetComponentInParent<TowingSet>().GetTowedBoats();
                        for (int i = 0; i < 5; i++)
                        {
                            foreach (Rigidbody body1 in towedBoats)
                            {
                                if (body1.transform == GameState.currentBoat || body1.transform == GameState.lastBoat)
                                {
                                    ___rigidbody.isKinematic = false;
                                    return;
                                }
                                if (body1.gameObject.GetComponent<TowingSet>().towing)
                                {
                                    towedBoats = body1.gameObject.GetComponent<TowingSet>().GetTowedBoats();
                                }
                            }
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(BoatPerformanceSwitcher))]
        private static class BoatPerformanceSwitcherPatch
        {
            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            public static void BoatPerformaceSwitcherPatch(BoatPerformanceSwitcher __instance, ref Rigidbody ___body, ref bool ___performanceModeOn)
            {
                if (__instance.GetComponentInParent<TowingSet>() == null) return;
                // check if we're being towed
                if (__instance.GetComponentInParent<TowingSet>().towed && Plugin.performanceMode.Value > 0)
                {
                    Rigidbody towedBy = __instance.GetComponentInParent<TowingSet>().GetTowedBy();
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
                            towedBy = towedBy.gameObject.GetComponent<TowingSet>().GetTowedBy();
                            continue;
                        }
                        break;
                    }
                }
                // check if we're towing something

                if (__instance.GetComponentInParent<TowingSet>().towing)
                {
                    //Plugin.logSource.LogInfo("found bollard");
                    List<Rigidbody> towedBoats = ___body.gameObject.GetComponent<TowingSet>().GetTowedBoats();
                    for (int i = 0; i < 5; i++)

                    {
                        foreach (Rigidbody body1 in towedBoats)
                        {
                            if (body1.transform == GameState.currentBoat || body1.transform == GameState.lastBoat)
                            {
                                __instance.InvokePrivateMethod("SetPerformanceMode", false);
                                return;
                            }
                            if (body1.gameObject.GetComponent<TowingSet>().towing)
                            {
                                towedBoats = body1.gameObject.GetComponent<TowingSet>().GetTowedBoats();
                            }
                        }
                    }
                }
            }
        }

    }
}
