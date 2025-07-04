using BepInEx.Bootstrap;
using UnityEngine;

namespace TowableBoats
{
    public class TowingCleat : GPButtonDockMooring
    {
        public TowingSet towingSet;
        public TowingSet towed;

        public override bool OnItemClick(PickupableItem heldItem)
        {
            if (heldItem.GetComponent<PickupableBoatMooringRope>() is PickupableBoatMooringRope rope && rope.GetBoatRigidbody().gameObject != towingSet.gameObject)
            {
                rope.MoorTo(this);
                return true;
            }
            return false;
        }

        public void RegisterTowed(TowingSet toTow)
        {
            toTow.towedBy = towingSet;
            towed = toTow;
            towingSet.UpdateTowedBoats();

            if (!Chainloader.PluginInfos.ContainsKey("com.nandbrew.nandfixes") && !Plugin.ignoreWarning.Value)
            {
                NotificationUi.instance.ShowNotification("NANDFixes not present\nnot responsible for lost items\nor sunk ships");
                //Hints.instance.ShowExternalHint("NANDFixes not present. Not responsible for lost items or sunk ships");
            }
        }

        public void Unhook(TowingSet towedBoat)
        {
            towedBoat.UpdateTowedBy();
            towingSet.UpdateTowedBoats();
        }
    }
}
