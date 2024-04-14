using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using cakeslice;

namespace TowableBoats
{
    internal class TowingSet : MonoBehaviour
    {
        private Transform mooringSetTransform;
        private Transform boatTransform;
        private Rigidbody towedBy;
        private List<Rigidbody> towedBoats;
        public bool towing;
        public bool towed;
        private GPButtonDockMooring[] cleats;
        public bool smallBoat;

        //public BoatPart boatPart;
        //public BoatPartOption partOption;

        public Transform GetBoatTransform() { return boatTransform; }
        public List<Rigidbody> GetTowedBoats() { return towedBoats; }
        public Rigidbody GetTowedBy() { return towedBy; }
        public GPButtonDockMooring[] GetCleats() { return cleats; }

        private void Awake()
        {
            
            mooringSetTransform = gameObject.GetComponentInChildren<MooringSet>().transform.GetChild(0);
            //Debug.Log("base parent is: " + base.transform.name);
            boatTransform = base.transform;
        }

        private void FixedUpdate()
        {
            //GPButtonDockMooring[] cleats = gameObject.GetComponentsInChildren<GPButtonDockMooring>();
            if (cleats != null && Plugin.drag.Value == true)
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
            if (GameState.justStarted)
            {
                UpdateTowedBoats();
                UpdateTowedBy();
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
                    if (ropes[i].GetPrivateField<SpringJoint>("mooredToSpring").gameObject.CompareTag("Boat"))
                    {
                        towedBy = ropes[i].GetPrivateField<SpringJoint>("mooredToSpring").transform.GetComponentInParent<TowingSet>().GetBoatTransform().GetComponent<Rigidbody>();

                        flag = true;
                    }
                }
            }
            towed = flag;

        }

        public void AddCleats(GameObject refCleat)
        {
            Array[] cleatDefs;
            if (boatTransform.name.Contains("medi medium")) cleatDefs = CleatDefs.brig;
            else if (boatTransform.name.Contains("dhow medium")) cleatDefs = CleatDefs.sanbuq;
            else if (boatTransform.name.Contains("junk medium")) cleatDefs = CleatDefs.junk;
            else if (boatTransform.name.Contains("medi small")) { cleatDefs = CleatDefs.cog; smallBoat = true; }
            else if (boatTransform.name.Contains("dhow small")) { cleatDefs = CleatDefs.dhow; smallBoat = true; }
            else if (boatTransform.name.Contains("junk small")) { cleatDefs = CleatDefs.kakam; smallBoat = true; }
            else return;

            GameObject towingSet = Instantiate(new GameObject(), mooringSetTransform.position, mooringSetTransform.rotation, mooringSetTransform.parent);
            towingSet.name = "towing set";

            /*SpringJoint boatSpring = base.gameObject.AddComponent<SpringJoint>();
            boatSpring.autoConfigureConnectedAnchor = false;
            boatSpring.connectedAnchor = new TowingSet().transform.localPosition;
            boatSpring.maxDistance = 10;
            boatSpring.minDistance = 0;
            boatSpring.damper = 1;
            boatSpring.spring = 100;*/

            #region boatpart
            //GameObject towing_cleats = Instantiate(new GameObject(), towingSet.transform);
            //towing_cleats.name = "towing cleats";

            /*
                        GameObject towing_none = Instantiate(new GameObject(), towingSet.transform);

                        partOption = towing_cleats.AddComponent<BoatPartOption>();
                        Debug.Log("added boatPartOption component");
                        partOption.optionName = "towing cleats";
                        partOption.basePrice = cleatDefs.Length * 200;
                        partOption.installCost = 100;
                        partOption.mass = 10;
                        partOption.childOptions = new GameObject[0];
                        partOption.requires = new List<BoatPartOption>();
                        partOption.requiresDisabled = new List<BoatPartOption>();
                        partOption.walkColObject = towing_cleats;

                        Debug.Log("set fields for part Option");

                        BoatPartOption cleatsNone = towing_none.AddComponent<BoatPartOption>();
                        cleatsNone.optionName = "(no cleats)";
                        cleatsNone.name = "no cleats";
                        cleatsNone.childOptions = new GameObject[0];
                        cleatsNone.requires = new List<BoatPartOption>();
                        cleatsNone.requiresDisabled = new List<BoatPartOption>();
                        cleatsNone.walkColObject = towing_none;

                        boatPart = new BoatPart();

                        boatPart.partOptions = new List<BoatPartOption> { cleatsNone, partOption };

                        boatPart.category = 1;
                        boatPart.activeOption = 0;

                        Debug.Log("added part options to boatPart");
                        var customParts = boatTransform.gameObject.GetComponent<BoatCustomParts>();
                        if (customParts) Debug.Log("found customparts component");
                        customParts.availableParts.Add(boatPart);
                        Debug.Log("Added boatPart to availableParts");*/
            #endregion

            for (int i = 0; i < cleatDefs.Length; i++)
            {
                object[] cleatDef = cleatDefs[i] as object[];
                GameObject towingCleat = Instantiate(refCleat, (Vector3)cleatDef[1], (Quaternion)cleatDef[2]);
                towingCleat.transform.SetParent(towingSet.transform, false);
                towingCleat.name = (string)cleatDef[0];

                towingCleat.transform.localScale = (Vector3)cleatDef[3];
                towingCleat.tag = "Boat";

                //towingCleat.GetComponent<GPButtonDockMooring>().spring = boatSpring;

                foreach (Outline outline in towingCleat.GetComponents<Outline>())
                {

                    if (!ReferenceEquals(outline, towingCleat.GetPrivateField("outline")))
                    {
                        Destroy(outline);
                    }
                }
            }
            cleats = towingSet.GetComponentsInChildren<GPButtonDockMooring>();
        }


    }
}
