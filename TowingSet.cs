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
        public GPButtonDockMooring[] bollards;
        public Rigidbody towedBy;
        public List<Rigidbody> towedBoats;
        public bool towing;
        public bool towed;
        public Transform GetBoatTransform() { return boatTransform; }

        private void Awake()
        {
            
            mooringSetTransform = gameObject.GetComponentInChildren<MooringSet>().transform.GetChild(0);
            //Debug.Log("base parent is: " + base.transform.name);
            boatTransform = base.transform;
        }

/*        private void Update()
        {
            if (bollards != null)
            {
                foreach (GPButtonDockMooring bollard in bollards)
                {
                    Outline[] outline = bollard.GetComponents<Outline>();
                    foreach (Outline outline2 in outline)
                    {
                        if (outline2 != bollard.GetPrivateField("outline"))
                        {
                            Destroy(outline2);
                        }
                    }

                }

            }
        }*/

        private void FixedUpdate()
        {
            //GPButtonDockMooring[] bollards = gameObject.GetComponentsInChildren<GPButtonDockMooring>();
            if (bollards != null && Plugin.drag.Value == true)
            {
                for (int i = 0; i < bollards.Length; i++)
                {
                    //Vector3 force = bollards[i].transform.GetComponentInChildren<SpringJoint>().currentForce;
                    Vector3 force = bollards[i].transform.position;
                    if (bollards[i].transform.GetComponentInChildren<PickupableBoatMooringRope>())
                    {
                        force = bollards[i].transform.GetComponentInChildren<SpringJoint>().currentForce;
                        if (bollards[i].transform.GetComponentInChildren<SpringJoint>().connectedBody.gameObject.GetComponent<BoatPerformanceSwitcher>().performanceModeIsOn())
                        {
                            force /= 10;
                        }
                        base.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(force / 3f, bollards[i].transform.position, ForceMode.Force);
                    }
                }
            }


            if (GameState.justStarted)
            {
                GetTowedBoat();
                GetTowedBy();
            }
        }

        public List<Rigidbody> GetTowedBoat()
        {
            bool flag = false;
            towedBoats = new List<Rigidbody>();
            //GPButtonDockMooring[] bollards = gameObject.GetComponentsInChildren<GPButtonDockMooring>();
            if (bollards != null)
            {
                for (int i = 0; i < bollards.Length; i++)
                {
                    if (bollards[i].transform.GetComponentInChildren<PickupableBoatMooringRope>() != null)
                    {
                        flag = true;
                        towedBoats.Add(bollards[i].transform.GetComponentInChildren<PickupableBoatMooringRope>().GetBoatRigidbody());
                    }
                }
            }

            towing = flag;
            return towedBoats;
        }
        public Rigidbody GetTowedBy()
        {
            PickupableBoatMooringRope[] ropes = gameObject.GetComponent<BoatMooringRopes>().ropes;
            //GPButtonDockMooring[] bollards = gameObject.GetComponentsInChildren<GPButtonDockMooring>();
            for (int i = 0; i < ropes.Length; i++)
            {
                if (ropes[i].IsMoored())
                {
                    if (ropes[i].GetPrivateField<SpringJoint>("mooredToSpring").gameObject.CompareTag("Boat"))
                    {
                        towed = true;
                        towedBy = ropes[i].GetPrivateField<SpringJoint>("mooredToSpring").transform.GetComponentInParent<TowingSet>().GetBoatTransform().GetComponent<Rigidbody>();
                        return ropes[i].GetPrivateField<SpringJoint>("mooredToSpring").transform.GetComponentInParent<TowingSet>().GetBoatTransform().GetComponent<Rigidbody>();

                    }
                }
            }
            towed = false;
            return null;
        }

        public void AddBollards(GameObject bollard)
        {
            //if (bollard == null) Plugin.logSource.LogError("null bollard");
            Array[] things;
            if (boatTransform.name.Contains("medi medium")) things = brigBollards;
            else if (boatTransform.name.Contains("dhow medium")) things = sanbuqBollards;
            else if (boatTransform.name.Contains("junk medium")) things = junkBollards;
            else return;

            for (int i = 0; i < things.Length; i++)
            {
                object[] thing = things[i] as object[];
                GameObject boatBollard = Instantiate(bollard, (Vector3)thing[1], (Quaternion)thing[2]);
                boatBollard.transform.SetParent(mooringSetTransform, false);
                boatBollard.name = (string)thing[0];

                boatBollard.transform.localScale = new Vector3(boatBollard.transform.localScale.x * 0.6f, boatBollard.transform.localScale.y * 0.6f, boatBollard.transform.localScale.z * 0.6f);
                boatBollard.tag = "Boat";
                //Plugin.logSource.LogInfo($"{i}");
                foreach (Outline outline in boatBollard.GetComponents<Outline>())
                {
                    if (outline != boatBollard.GetPrivateField("outline"))
                    {
                        Destroy(outline);
                    }
                }
            }
            bollards = gameObject.GetComponentsInChildren<GPButtonDockMooring>();
            //Debug.Log("added bollards");


        }



        static readonly object[] brig0 = { "bollard_stern_left", new Vector3(-14.9f, 4.36f, 2.23f), new Quaternion(0.7071068f, 0f, 0f, 0.7071068f) }; // 90, 0, 0
        static readonly object[] brig1 = { "bollard_stern_right", new Vector3(-14.9f, 4.36f, -2.23f), new Quaternion(0.7071068f, 0f, 0f, 0.7071068f) };
        static readonly object[] brig2 = { "bollard_mid_aft_left", new Vector3(-2.3f, 3.53f, 3.27f), new Quaternion(-0.7058f, 0.0252f, -0.0246f, 0.7075f) }; // 270, 0, 0
        static readonly object[] brig3 = { "bollard_mid_aft_right", new Vector3(-2.3f, 3.53f, -3.27f), new Quaternion(-0.7058f, 0.0252f, -0.0246f, 0.7075f) };
        static readonly object[] brig4 = { "bollard_mid_fore_left", new Vector3(6f, 3.62f, 2.92f), new Quaternion(-0.7041f, 0.0292f, 0.0692f, 0.7061f) };
        static readonly object[] brig5 = { "bollard_mid_fore_right", new Vector3(6f, 3.62f, -2.92f), new Quaternion(-0.7036f, -0.0384f, 0.0012f, 0.7095f) };
        static readonly Array[] brigBollards = { brig0, brig1, brig2, brig3, brig4, brig5 };

        static readonly object[] sanbuq0 = { "bollard_stern_left", new Vector3(-11.5f, 3.33f, 2.22f), new Quaternion(0.7071058f, 0f, 0.0599f, -0.7071058f) }; // 270, 0, 0
        static readonly object[] sanbuq1 = { "bollard_stern_right", new Vector3(-11.5f, 3.33f, -2.22f), new Quaternion(0.7071058f, 0f, 0.0599f, -0.7071058f) };
        static readonly object[] sanbuq2 = { "bollard_mid_aft_left", new Vector3(-1.4f, 2.12f, 2.46f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f) };
        static readonly object[] sanbuq3 = { "bollard_mid_aft_right", new Vector3(-1.4f, 2.12f, -2.46f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f) };
        static readonly object[] sanbuq4 = { "bollard_mid_fore_left", new Vector3(4.7f, 2.34f, 2.21f), new Quaternion(-0.7071f, 0.0f, 0.0371f, 0.7061f) }; // 270, 4, 0
        static readonly object[] sanbuq5 = { "bollard_mid_fore_right", new Vector3(4.7f, 2.34f, -2.21f), new Quaternion(-0.7058f, -0.0432f, 0.0062f, 0.7071f) }; // 270, -4, 0
        static readonly Array[] sanbuqBollards = { sanbuq0, sanbuq1, sanbuq2, sanbuq3, sanbuq4, sanbuq5 };

        static readonly object[] junk0 = { "bollard_stern_left", new Vector3(-9.6f, 2.24f, 1.1f), new Quaternion(0.6963642f, 0.1227878f, 0.1227878f, -0.6963642f) }; //270, -20, 0
        static readonly object[] junk1 = { "bollard_stern_right", new Vector3(-9.6f, 2.24f, -1.1f), new Quaternion(0.6963642f, -0.1227878f, -0.1227878f, -0.6963642f) };
        static readonly object[] junk2 = { "bollard_mid_aft_left", new Vector3(1.5f, 1.04f, 2.5f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f) };
        static readonly object[] junk3 = { "bollard_mid_aft_right", new Vector3(1.5f, 1.04f, -2.5f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f) };
        static readonly object[] junk4 = { "bollard_mid_fore_left", new Vector3(8.44f, 1.3f, 1.84f), new Quaternion(0.7010574f, -0.092296f, -0.092296f, -0.7010574f) }; //270, 15, 0
        static readonly object[] junk5 = { "bollard_mid_fore_right", new Vector3(8.44f, 1.3f, -1.84f), new Quaternion(0.7010574f, 0.092296f, 0.092296f, -0.7010574f) }; //270, -15, 0
        static readonly Array[] junkBollards = { junk0, junk1, junk2, junk3, junk4, junk5 };

    }
}
