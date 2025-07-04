using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

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
            [HarmonyPatch("Update")]
            [HarmonyPrefix]
            public static bool BoatPerformaceSwitcherPatch(BoatPerformanceSwitcher __instance, ref Rigidbody ___body, BoatDamage ___damage)
            {
                if (GameState.lastBoat == __instance.transform || ___damage.sunk || ___body.isKinematic)
                {
                    return true;
                }
                if (__instance.GetComponent<TowingSet>()?.Physics == true)
                {
                    Util.InvokePrivate(__instance, "SetPerformanceMode", false);
                    return false;
                }
                return true;
            }

        }
    }
}
