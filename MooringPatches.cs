using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using System.Collections;

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
            public static void MoorToPatch(PickupableBoatMooringRope __instance, GPButtonDockMooring mooring, Rigidbody ___boatRigidbody)
            {
                if (mooring.gameObject.CompareTag("Boat"))
                {
                    TowingSet towingSet = ___boatRigidbody.GetComponent<TowingSet>();
                    if (mooring.GetComponentInParent<TowingSet>() is TowingSet parent && (!towingSet.towed || parent == towingSet.GetTowedBy()))
                    {
                        towingSet.UpdateTowedBy();
                        parent.UpdateTowedBoats();
                    }
                }
            }
            [HarmonyPatch("ThrowRopeTo")]
            [HarmonyPrefix]
            public static bool ThrowRopeToPatch(PickupableBoatMooringRope __instance, GPButtonDockMooring mooring, Rigidbody ___boatRigidbody)
            {
                if (mooring.gameObject.CompareTag("Boat"))
                {
                    if (___boatRigidbody.transform != mooring.GetComponentInParent<TowingSet>().transform)
                    {
                        __instance.MoorTo(mooring);
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
                    if (___mooredToSpring.GetComponentInParent<TowingSet>() is TowingSet parent)
                    {
                        __state = parent;
                        //Debug.Log("found " + __state);
                    }
                }
            }
            [HarmonyPatch("Unmoor")]
            [HarmonyPostfix]
            public static void UnmoorPostPatch(TowingSet __state, Rigidbody ___boatRigidbody)
            {

                if (__state)
                {
                    __state.UpdateTowedBoats();
                }
                ___boatRigidbody.GetComponent<TowingSet>().UpdateTowedBy();
            }
        }

        [HarmonyPatch(typeof(BoatMooringRopes))]
        private static class BoatMooringRopesPatches
        {
            [HarmonyPatch("UnmoorAllRopes")]
            [HarmonyPostfix]
            public static void UnmoorAllRopesPatch(BoatMooringRopes __instance)
            {
                if (__instance.GetComponentInParent<TowingSet>() is TowingSet parentSet && parentSet.GetCleats() != null)
                {
                    foreach (GPButtonDockMooring cleat in parentSet.GetCleats())
                    {
                        if (cleat.GetComponentInChildren<PickupableBoatMooringRope>() is PickupableBoatMooringRope rope)
                        {
                            //Debug.Log("found a thing");
                            rope.Unmoor();
                            rope.ResetRopePos();
                        }
                    }
                }
            }
        }
    }
}
