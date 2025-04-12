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
                if (!___rigidbody.isKinematic || !GameState.sleeping) return;
                if (__instance.GetComponentInParent<TowingSet>() is TowingSet towingSet)
                {
                   ___rigidbody.isKinematic = !towingSet.PhysicsMode(10);
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
                if (GameState.lastBoat == __instance.transform || ___damage.sunk)
                {
                    return true;
                }
                if (__instance.GetComponentInParent<TowingSet>() is TowingSet towingSet)
                {
                    Util.InvokePrivate(__instance, "SetPerformanceMode", !towingSet.PhysicsMode(Plugin.performanceMode.Value));
                    return false;
                }
                return true;
            }
            

        }

    }
}
