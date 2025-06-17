using HarmonyLib;

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
