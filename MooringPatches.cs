using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using SailwindModdingHelper;
using System.Collections;

namespace TowableBoats
{
    internal class MooringPatches
    {
        private static GameObject bollard;



        [HarmonyPatch(typeof(GPButtonDockMooring))]
        private static class GPButtonDockMooringPatches
        {
            [HarmonyPatch("Awake")]
            [HarmonyPrefix]
            public static void MooringComponentPatch(GPButtonDockMooring __instance)
            {
                if (!bollard) bollard = __instance.gameObject;
                //Plugin.logSource.LogInfo("GPButtonDockMooring is awake");
            }
        }

        [HarmonyPatch(typeof(MooringSet))]
        private static class MooringSetPatches
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void MooringComponentPatch(MooringSet __instance)
            {
                TowingSet towingManager = __instance.transform.parent.gameObject.AddComponent<TowingSet>();
                __instance.StartCoroutine(AddBollards(towingManager));

            }
            private static IEnumerator AddBollards(TowingSet towingManager)
            {
                Debug.Log("waiting for bollard");
                yield return new WaitUntil(() => bollard != null);
                towingManager.AddBollards(bollard);
                Debug.Log("got bollard");

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
                    if (___boatRigidbody.transform != mooring.GetComponentInParent<TowingSet>().GetBoatTransform())
                    {
                        if (Plugin.multiParent.Value == true || !__instance.GetComponentInParent<TowingSet>().towed || mooring.gameObject.GetComponentInParent<TowingSet>().GetBoatTransform() == ___boatRigidbody.gameObject.GetComponentInParent<TowingSet>().GetTowedBy().transform)

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
                if (!__instance.GetComponentInParent<TowingSet>() || __instance.GetComponentInParent<TowingSet>().bollards == null) return;

                foreach (GPButtonDockMooring bollard in __instance.GetComponentInParent<TowingSet>().bollards)

                {
                    if (bollard.GetComponentInChildren<PickupableBoatMooringRope>())
                    {
                        PickupableBoatMooringRope rope = bollard.GetComponentInChildren<PickupableBoatMooringRope>();
                        Debug.Log("found a thing");
                        rope.Unmoor();
                        rope.ResetRopePos();

                    }
                }
            }
        }
    }
}
