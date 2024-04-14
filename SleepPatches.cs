using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SailwindModdingHelper;



namespace TowableBoats
{
    internal class SleepPatches
    {
        [HarmonyPatch(typeof(Sleep), "CurrentBoatIsMoored")]
        private static class IsMooredPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result)
            {
                if (!GameState.currentBoat) return;
                if (__result && GameState.currentBoat.GetComponentInParent<TowingSet>().towed)
                {
                    __result = false;
                }
            }
        }

    }
}
