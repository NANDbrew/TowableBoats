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
                if (!__instance.GetComponentInParent<TowingSet>()) return;
                if (!___rigidbody.isKinematic || !GameState.sleeping) return;

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
        [HarmonyPatch(typeof(BoatPerformanceSwitcher))]
        private static class BoatPerformanceSwitcherPatch
        {
            [HarmonyPatch("Update")]
            [HarmonyPrefix]
            public static bool BoatPerformaceSwitcherPatch(BoatPerformanceSwitcher __instance, ref Rigidbody ___body, BoatDamage ___damage)
            {
                bool flag = true;
                if (GameState.lastBoat == __instance.transform || ___damage.sunk)
                {
                    return true;
                }
                if (__instance.GetComponentInParent<TowingSet>() == null) return true;
                // check if we're being towed
                if (__instance.GetComponentInParent<TowingSet>().towed && Plugin.performanceMode.Value > 0)
                {
                    Rigidbody towedBy = __instance.GetComponentInParent<TowingSet>().GetTowedBy();
                    for (int i = 0; i < Plugin.performanceMode.Value; i++)
                    {
                        if (towedBy.transform == GameState.currentBoat || towedBy.transform == GameState.lastBoat)
                        {
                            //Plugin.logSource.LogInfo("found Boat");

                                //__instance.InvokePrivateMethod("SetPerformanceMode", false);
                                flag = false;
                            
                            break;
                        }
                        if (towedBy.gameObject.GetComponent<TowingSet>().towed)
                        {
                            towedBy = towedBy.gameObject.GetComponent<TowingSet>().GetTowedBy();
                            continue;
                        }
                        //flag = true;
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

                                    //__instance.InvokePrivateMethod("SetPerformanceMode", false);
                                    flag = false;
                                
                                i = 6;
                                break;
                            }
                            if (body1.gameObject.GetComponent<TowingSet>().towing)
                            {
                                towedBoats = body1.gameObject.GetComponent<TowingSet>().GetTowedBoats();
                            }
                        }
                    }
                }
                if (__instance.performanceModeIsOn() != flag)
                {
                    __instance.InvokePrivateMethod("SetPerformanceMode", flag);

                }
                return false;
            }
        }
    }
}
