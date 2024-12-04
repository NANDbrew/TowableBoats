using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using cakeslice;
using BepInEx;
using System.Reflection;

namespace TowableBoats
{
    internal class TowingSet : MonoBehaviour
    {
        //private Transform mooringSetTransform;
        //private Transform boatTransform;
        private Rigidbody towedBy;
        private List<Rigidbody> towedBoats;
        public bool towing;
        public bool towed;
        private GPButtonDockMooring[] cleats;
        public bool smallBoat;

        //public BoatPart boatPart;
        //public BoatPartOption partOption;

        //public Transform GetBoatTransform() { return boatTransform; }
        public List<Rigidbody> GetTowedBoats() { return towedBoats; }
        public Rigidbody GetTowedBy() { return towedBy; }
        public GPButtonDockMooring[] GetCleats() { return cleats; }

        private void Awake()
        {
            //mooringSetTransform = gameObject.GetComponentInChildren<MooringSet>().transform.GetChild(0);
            //Debug.Log("base parent is: " + base.transform.name);
            //boatTransform = base.transform;
            if (transform.Find("towing set") is Transform set)
            {
                cleats = set.GetComponentsInChildren<GPButtonDockMooring>();
            }
            else
            {
                AddCleats();
            }
        }

        private void FixedUpdate()
        {

            if (GameState.justStarted)
            {
                UpdateTowedBoats();
                UpdateTowedBy();
            }
            if (cleats == null) return;
            if (Plugin.drag.Value == true)
            {
                for (int i = 0; i < cleats.Length; i++)
                {
                    if (cleats[i].transform.GetComponentInChildren<PickupableBoatMooringRope>())
                    {
                        Vector3 force = cleats[i].transform.GetComponentInChildren<SpringJoint>().currentForce;
                        if (cleats[i].transform.GetComponentInChildren<SpringJoint>().connectedBody.gameObject.GetComponent<BoatPerformanceSwitcher>().performanceModeIsOn())
                        {
                            force /= 10;
                        }
                        gameObject.GetComponent<Rigidbody>().AddForceAtPosition(force / 3f, cleats[i].transform.position, ForceMode.Force);
                    }
                }
            }
            if (smallBoat)
            {
                foreach (var cleat in cleats)
                {
                    if (!Plugin.smallBoats.Value && cleat.GetComponentInChildren<PickupableBoatMooringRope>() is PickupableBoatMooringRope rope)
                    {
                        rope.Unmoor();
                        rope.ResetRopePos();
                    }
                    cleat.gameObject.SetActive(Plugin.smallBoats.Value);
                }
            }
        }

        public void UpdateTowedBoats()
        {
            towedBoats = new List<Rigidbody>();
            bool flag = false;

            if (cleats != null)
            {
                for (int i = 0; i < cleats.Length; i++)
                {
                    if (cleats[i].transform.GetComponentInChildren<PickupableBoatMooringRope>() != null)
                    {
                        towedBoats.Add(cleats[i].transform.GetComponentInChildren<PickupableBoatMooringRope>().GetBoatRigidbody());
                        flag = true;
                    }
                }

            }
            towing = flag;

        }

        public void UpdateTowedBy()
        {
            bool flag = false;

            towedBy = null;
            PickupableBoatMooringRope[] ropes = gameObject.GetComponent<BoatMooringRopes>().ropes;

            for (int i = 0; i < ropes.Length; i++)
            {
                if (ropes[i].IsMoored())
                {
                    if (Traverse.Create(ropes[i].GetType()).Field("mooredToSpring").GetValue<SpringJoint>() is SpringJoint spr && spr.gameObject.CompareTag("Boat"))
                    {
                        towedBy = spr.transform.GetComponentInParent<TowingSet>().gameObject.GetComponent<Rigidbody>();

                        flag = true;
                    }
                }
            }
            towed = flag;

        }

        public void AddCleats()
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
                else if (index == 40) { prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set cog.prefab"); smallBoat = true; }
                else if (index == 10) { prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set dhow.prefab"); smallBoat = true; }
                else if (index == 90) { prefab = AssetTools.bundle.LoadAsset<GameObject>("Assets/TowableBoats/towing set kakam.prefab"); smallBoat = true; }
                else return;
                var towingSet = UnityEngine.Object.Instantiate(prefab, transform, false);
                towingSet.name = "towing set";
                cleats = towingSet.GetComponentsInChildren<GPButtonDockMooring>();
            }
            catch 
            { 
                Debug.LogError("TowableBoats: Couldn't load towing set for " + name + "!!");
            }

        }


    }
}
