using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TowableBoats
{
    internal class BoatPerformancePatches
    {
        public static Dictionary<GameObject, TowingSet> towingSets = new Dictionary<GameObject, TowingSet>();
        [HarmonyPatch(typeof(BoatHorizon))]
        private static class BoatHorizonPatch
        {
            [HarmonyPatch("UpdateKinematic")]
            [HarmonyPostfix]
            public static void BoatKinematicPatch(BoatHorizon __instance, ref Rigidbody ___rigidbody)
            {
                if (__instance.NPCBoat || !___rigidbody.isKinematic || !GameState.sleeping) return;
                if (__instance.transform.parent.GetComponent<TowingSet>()?.Horizon == true)
                {
                   ___rigidbody.isKinematic = false;
                }
            }
        }
        [HarmonyPatch(typeof(BoatPerformanceSwitcher))]
        private static class BoatPerformanceSwitcherPatch
        {
            [HarmonyPatch("SetPerformanceMode")]
            [HarmonyPrefix]
            public static void BoatPerformaceSwitcherPatch(ref bool newState, BoatPerformanceSwitcher __instance, Rigidbody ___body)
            {
                if (newState && !___body.isKinematic)
                {
                    if (__instance.GetComponent<TowingSet>()?.Physics == true)
                    {
                        newState = false;
                    }
                }
            }
            

        }

    }
}
