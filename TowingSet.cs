using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;


namespace TowableBoats
{
    public class TowingSet : MonoBehaviour
    {
        public TowingSet towedBy;
        private List<TowingSet> towedBoats;

        private TowingCleat[] cleats;
        private BoatMooringRopes mooringRopes;

        public bool Horizon { get; private set; }
        public bool Physics { get; private set; }

        public List<TowingSet> GetTowedBoats() { return towedBoats; }

        public void Awake()
        {
            mooringRopes = GetComponent<BoatMooringRopes>();
            if (transform.Find("towing set") is Transform set)
            {
                if (set.GetComponent<BoatPartOption>() == null) set.gameObject.SetActive(true);
                RegisterCleats(set.GetComponentsInChildren<GPButtonDockMooring>());
            }
            else
            {
                AddCleats();
            }
        }

        public void Update()
        {
            if (transform != GameState.lastBoat)
            {
                UpdatePhysicsMode();
            }
        }

        public void UpdateTowedBoats()
        {
            if (cleats != null)
            {
                towedBoats = new List<TowingSet>();
                for (int i = 0; i < cleats.Length; i++)
                {
                    if (cleats[i].towed is TowingSet towed && !towedBoats.Contains(towed))
                    {
                        towedBoats.Add(towed);
                    }
                }

            }

        }

        public void UpdateTowedBy()
        {
            towedBy = null;

            foreach (PickupableBoatMooringRope rope in mooringRopes.ropes)
            {
                if (rope.IsMoored())
                {
                    //Debug.Log("rope is moored");
                    if (rope.transform.parent.GetComponent<TowingCleat>() is TowingCleat cleat)
                    {
                        towedBy = cleat.towingSet;
                        return;
                    }

                }
            }
        }
        public void UpdatePhysicsMode()
        {
            int depth = 10;
            Physics = false;
            Horizon = false;
            //check if we're being towed
            if (towedBy)
            {
                TowingSet towedByLocal = towedBy;
                for (int i = 0; i < depth; i++) // limit. maybe we don't want everything getting physics
                {
                    //check if what's towing us is the active boat
                    if (towedByLocal.transform == GameState.lastBoat)
                    {
                        Physics = i < Plugin.performanceMode.Value;
                        Horizon = true;
                        return;
                    }
                    if (towedByLocal.towedBy)
                    {
                        towedByLocal = towedByLocal.towedBy;
                    }
                    else break;
                }
            }
            // check if we're towing something
            if (towedBoats != null && towedBoats.Count > 0)
            {
                List<TowingSet> towedBoatsLocal = towedBoats;
                for (int i = 0; i < depth; i++) // sanity limit. we want to know if the player is on the boat behind us
                {
                    bool towedLocal = false;
                    foreach (TowingSet towedBoat in towedBoatsLocal)
                    {
                        //check if what we're towing is the active boat
                        if (towedBoat.transform == GameState.lastBoat)
                        {
                            Physics = true;
                            Horizon = true;
                            return;
                        }
                        if (towedBoat.towedBoats.Count > 0)
                        {
                            towedBoatsLocal = towedBoat.GetTowedBoats();
                            towedLocal = true;
                        }
                    }
                    if (!towedLocal) break;
                }
            }
        }

        private void AddCleats()
        {
            if (AssetTools.bundle == null) AssetTools.LoadAssetBundles();
            GameObject prefab;
            int index = gameObject.GetComponent<SaveableObject>().sceneIndex;
            try
            {
                if (index == 50) prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set brig.prefab");
                else if (index == 20) prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set sanbuq.prefab");
                else if (index == 80) prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set junk.prefab");
                else if (index == 70) prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set jong.prefab");
                else if (index == 40 && Plugin.smallBoats.Value) prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set cog.prefab");
                else if (index == 10 && Plugin.smallBoats.Value) prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set dhow.prefab");
                else if (index == 90 && Plugin.smallBoats.Value) prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set kakam.prefab");
                else return;
                var towingSet = Instantiate(prefab, transform, false);
                towingSet.name = "towing set";
                RegisterCleats(towingSet.GetComponentsInChildren<GPButtonDockMooring>());

            }
            catch 
            { 
                Debug.LogError("TowableBoats: Couldn't load towing set for " + name + "!!");
            }

        }

        private void RegisterCleats(GPButtonDockMooring[] cleatArray)
        {
            cleats = new TowingCleat[cleatArray.Length];
            for (int i = 0; i < cleats.Length; i++)
            {
                cleats[i] = cleatArray[i].gameObject.AddComponent<TowingCleat>();
                cleats[i].towingSet = this;
                Component.Destroy(cleatArray[i]);
            }
            towedBoats = new List<TowingSet>();
        }

        public void UnmoorAllRopes()
        {
            if (cleats != null)
            {
                foreach (TowingCleat cleat in cleats)
                {
                    if (cleat.transform.childCount > 0 && cleat.transform.GetChild(0).GetComponent<PickupableBoatMooringRope>() is PickupableBoatMooringRope rope)
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
