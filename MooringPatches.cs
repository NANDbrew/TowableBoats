using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Collections;
using BepInEx.Bootstrap;

namespace TowableBoats
{
    internal class MooringPatches
    {
        [HarmonyPatch(typeof(MooringSet))]
        private static class MooringSetPatches
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void MooringComponentPatch(MooringSet __instance)
            {
                __instance.transform.parent.gameObject.AddComponent<TowingSet>();

            }
        }
        [HarmonyPatch(typeof(PickupableBoatMooringRope))]
        private static class BoatMooringRopePatches
        {
            [HarmonyPatch("MoorTo")]
            [HarmonyPostfix]
            public static void MoorToPatch(GPButtonDockMooring mooring, Rigidbody ___boatRigidbody)
            {
                if (mooring is TowingCleat cleat)
                {
                    cleat.RegisterTowed(___boatRigidbody.GetComponent<TowingSet>());
                }
            }
            [HarmonyPatch("Unmoor")]
            [HarmonyPrefix]
            public static void UnmoorPatch(SpringJoint ___mooredToSpring, ref TowingCleat __state)
            {
                if (___mooredToSpring != null && ___mooredToSpring.GetComponent<TowingCleat>() is TowingCleat cleat)
                {
                    __state = cleat;

                }
            }
            [HarmonyPatch("Unmoor")]
            [HarmonyPostfix]
            public static void UnmoorPostPatch(TowingCleat __state, Rigidbody ___boatRigidbody)
            {
                if (__state)
                {
                    __state.Unhook(___boatRigidbody.GetComponent<TowingSet>());
                }
            }
        }

        [HarmonyPatch(typeof(BoatMooringRopes))]
        private static class BoatMooringRopesPatches
        {
            [HarmonyPatch("UnmoorAllRopes")]
            [HarmonyPostfix]
            public static void UnmoorAllRopesPatch(BoatMooringRopes __instance)
            {
                if (__instance.GetComponent<TowingSet>() is TowingSet towingSet)
                {
                    towingSet.UnmoorAllRopes();
                }
            }
        }
    }
}
