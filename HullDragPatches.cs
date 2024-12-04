using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Crest;
using UnityEngine;

namespace TowableBoats
{
    internal class HullDragPatches
    {
        [HarmonyPatch(typeof(HullDrag))]
        private static class HullDragPatch
        {
            [HarmonyPatch("FixedUpdate")]
            [HarmonyPostfix]
            public static void Postfix(HullDrag __instance, BoatProbes ___boat)
            {
                if (!Plugin.drag.Value || !GameState.playing) return;
                TowingSet towingSet = __instance.gameObject.GetComponent<TowingSet>();
                if (!towingSet.towing) return;
                float dragToAdd = 0f;
                //GPButtonDockMooring[] cleats = towingSet.cleats;
                
                foreach (GPButtonDockMooring cleat in towingSet.GetCleats())
                {
                    if (cleat.spring.currentForce.magnitude > 10)
                    {
                        dragToAdd += cleat.GetComponentInChildren<GPButtonDockMooring>().GetComponent<BoatProbes>().addedHullDrag;
                    }
                }

                /*foreach (Rigidbody body in towingSet.GetTowedBoats())
                {
                    dragToAdd += body.GetComponent<BoatProbes>().addedHullDrag;
                }*/
                ___boat.addedHullDrag += dragToAdd;
            }
        }
    }
}
