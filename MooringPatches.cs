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
            [HarmonyPatch("ThrowRopeTo")]
            [HarmonyPrefix]
            public static bool ThrowRopeToPatch(PickupableBoatMooringRope __instance, GPButtonDockMooring mooring, Rigidbody ___boatRigidbody)
            {
                if (mooring.gameObject.CompareTag("Boat"))
                {
                    if (___boatRigidbody.transform != mooring.GetComponentInParent<TowingSet>().transform)
                    {
                        if (!__instance.GetComponentInParent<TowingSet>().towed || mooring.gameObject.GetComponentInParent<TowingSet>().transform == ___boatRigidbody.gameObject.GetComponentInParent<TowingSet>().GetTowedBy().transform)
                        {
                            __instance.MoorTo(mooring);
                            ___boatRigidbody.GetComponentInParent<TowingSet>().UpdateTowedBy();
                            mooring.GetComponentInParent<TowingSet>().UpdateTowedBoats();
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
                if (!__instance.GetComponentInParent<TowingSet>() || __instance.GetComponentInParent<TowingSet>().GetCleats() == null) return;

                foreach (GPButtonDockMooring cleat in __instance.GetComponentInParent<TowingSet>().GetCleats())

                {
                    if (cleat.GetComponentInChildren<PickupableBoatMooringRope>())
                    {
                        PickupableBoatMooringRope rope = cleat.GetComponentInChildren<PickupableBoatMooringRope>();
                        Debug.Log("found a thing");
                        rope.Unmoor();
                        rope.ResetRopePos();

                    }
                }
            }
        }
    }
}
