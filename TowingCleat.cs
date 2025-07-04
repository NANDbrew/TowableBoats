using BepInEx.Bootstrap;
using UnityEngine;

namespace TowableBoats
{
    internal class TowingCleat : GPButtonDockMooring
    {
        public TowingSet towingSet;
        public TowingSet towed;

        public void RegisterTowed(TowingSet toTow)
        {
            toTow.towedBy = towingSet;
            towed = toTow;
            towingSet.UpdateTowedBoats();

            if (!Chainloader.PluginInfos.ContainsKey("com.nandbrew.nandfixes") && !Plugin.ignoreWarning.Value)
            {
                NotificationUi.instance.ShowNotification("NANDFixes not present. Not responsible for lost items or sunk ships");
                //Hints.instance.ShowExternalHint("NANDFixes not present. Not responsible for lost items or sunk ships");
            }
        }

        public void Unhook()
        {
            towed.UpdateTowedBy();
            towingSet.UpdateTowedBoats();
        }
    }
}
