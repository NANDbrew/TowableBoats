using HarmonyLib;
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
                if (! __instance.GetComponentInParent<TowingSet>() || !__instance.GetComponentInParent<TowingSet>().GetTowedBy()) return;
                if (GameState.sleeping)
                {
                    Rigidbody towedBy = __instance.GetComponentInParent<TowingSet>().GetTowedBy();
                    for (int i = 0; i < 5; i++)
                    {
                        if (towedBy.transform == GameState.currentBoat || towedBy.transform == GameState.lastBoat)
                        {
                            //Plugin.logSource.LogInfo("found Boat");

                            ___rigidbody.isKinematic = false;
                            break;
                        }
                        if (towedBy.gameObject.GetComponent<TowingSet>().GetTowedBy())
                        {
                            towedBy = towedBy.gameObject.GetComponent<TowingSet>().GetTowedBy();
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
            public static void BoatPerformaceSwitcherPatch(BoatPerformanceSwitcher __instance, ref Rigidbody ___body, ref bool ___performanceModeOn)
            {
                if (__instance.GetComponentInParent<TowingSet>() == null) return;
                // check if we're being towed
                if (__instance.GetComponentInParent<TowingSet>().GetTowedBy() && Plugin.performanceMode.Value > 0)
                {
                    Rigidbody towedBy = __instance.GetComponentInParent<TowingSet>().GetTowedBy();
                    for (int i = 0; i < Plugin.performanceMode.Value; i++)
                    {
                        if (towedBy.transform == GameState.currentBoat || towedBy.transform == GameState.lastBoat)
                        {
                            //Plugin.logSource.LogInfo("found Boat");

                            ___performanceModeOn = false;
                            break;
                        }
                        if (towedBy.gameObject.GetComponent<TowingSet>().GetTowedBy())
                        {
                            towedBy = towedBy.gameObject.GetComponent<TowingSet>().GetTowedBy();
                            continue;
                        }
                        break;
                    }
                }
                // check if we're towing something
                if (__instance.GetComponentInParent<TowingSet>().GetTowedBoats() != null && __instance.GetComponentInParent<TowingSet>().GetTowedBoats().Count > 0)
                {
                    List<Rigidbody> towedBoats = ___body.gameObject.GetComponent<TowingSet>().GetTowedBoats();
                    for (int i = 0; i < Plugin.performanceMode.Value; i++)
                    {
                        foreach (Rigidbody body1 in towedBoats)
                        {
                            if (body1.transform == GameState.currentBoat || body1.transform == GameState.lastBoat)
                            {
                                ___performanceModeOn = false;
                                return;
                            }
                            if (body1.gameObject.GetComponent<TowingSet>().GetTowedBoats() != null && body1.gameObject.GetComponent<TowingSet>().GetTowedBoats().Count > 0)
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
