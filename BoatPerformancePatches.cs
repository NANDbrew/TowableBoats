using HarmonyLib;
using System.Collections.Generic;
using TowableBoats;
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
            public static void BoatKinematicPatch(BoatHorizon __instance, ref Rigidbody ___rigidbody, ref float ___updateCooldown)
            {
                if (__instance.NPCBoat || !GameState.sleeping || ___rigidbody == null) return;
                if (___rigidbody.isKinematic && __instance.transform.parent.GetComponent<TowingSet>()?.Horizon == true)
                {
                    ___rigidbody.isKinematic = false;
                    ___updateCooldown = Random.Range(20f, 30f);
                }
            }
        }
        [HarmonyPatch(typeof(BoatPerformanceSwitcher))]
        private static class BoatPerformanceSwitcherPatch
        {
            [HarmonyPatch("SetPerformanceMode")]
            [HarmonyPrefix]
            public static void BoatPerformaceSwitcherPatch2(BoatPerformanceSwitcher __instance, ref bool newState)
            {
                if (GameState.currentlyLoading || GameState.justStarted) return;
                if (__instance.GetComponent<TowingSet>()?.Physics == true)
                {
                    newState = false;
                }

            }
        }
    }
}
/*[HarmonyPrefix]
public static bool BoatKinematicPatch(BoatHorizon __instance, BoatPhysicsSwitcher ___physicsSwitcher, ref Rigidbody ___rigidbody, ref float ___updateCooldown)
{
    if (__instance.transform.parent.GetComponent<TowingSet>()?.Horizon == true)
    {
        bool shouldFreeze = false;

        if (Debugger.debugForceKinematicBoat || !GameState.playing || GameState.changingStartRegion || GameState.recovering || GameState.currentlyLoading || GameState.justStarted)
        {
            shouldFreeze = true;
        }
        if ((bool)__instance.transform.parent.GetComponent<BoatLocalItems>() && !__instance.transform.parent.GetComponent<BoatLocalItems>().SafeToEnableBoatPhysics())
        {
            shouldFreeze = true;
        }
        if (Time.timeScale == 0f && (bool)___physicsSwitcher && ___physicsSwitcher.paused)
        {
            shouldFreeze = true;
        }

        if (___rigidbody.isKinematic != shouldFreeze)
        {
            ___rigidbody.isKinematic = shouldFreeze;
        }

        if (!shouldFreeze) ___updateCooldown = Random.Range(20f, 30f);
        return false;
    }
    return true;
}*/