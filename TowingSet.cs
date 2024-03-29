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
        private GPButtonDockMooring[] bollards;

        public Transform GetBoatTransform() { return boatTransform; }
        public List<Rigidbody> GetTowedBoats() { return towedBoats; }
        public Rigidbody GetTowedBy() { return towedBy; }
        public GPButtonDockMooring[] GetBollards() { return bollards; }

        private void Awake()
        {

            mooringSetTransform = gameObject.GetComponentInChildren<MooringSet>().transform.GetChild(0);
            //Debug.Log("base parent is: " + base.transform.name);
            boatTransform = base.transform;
        }

        private void FixedUpdate()
        {
            //GPButtonDockMooring[] bollards = gameObject.GetComponentsInChildren<GPButtonDockMooring>();
            if (bollards != null && Plugin.drag.Value == true)
            {
                for (int i = 0; i < bollards.Length; i++)
                {
                    if (bollards[i].transform.GetComponentInChildren<PickupableBoatMooringRope>())
                    {
                        Vector3 force = bollards[i].transform.GetComponentInChildren<SpringJoint>().currentForce;
                        if (bollards[i].transform.GetComponentInChildren<SpringJoint>().connectedBody.gameObject.GetComponent<BoatPerformanceSwitcher>().performanceModeIsOn())
                        {
                            force /= 10;
                        }
                        gameObject.GetComponent<Rigidbody>().AddForceAtPosition(force / 3f, bollards[i].transform.position, ForceMode.Force);
                    }
                }
            }
            if (GameState.justStarted)
            {
                UpdateTowedBoats();
                UpdateTowedBy();
            }
        }

        public void UpdateTowedBoats()
        {
            towedBoats = new List<Rigidbody>();
            bool flag = false;

            if (bollards != null)
            {
                for (int i = 0; i < bollards.Length; i++)
                {
                    if (bollards[i].transform.GetComponentInChildren<PickupableBoatMooringRope>() != null)
                    {
                        towedBoats.Add(bollards[i].transform.GetComponentInChildren<PickupableBoatMooringRope>().GetBoatRigidbody());
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

        public void AddBollards(GameObject bollard)
        {
            Array[] things;
            if (boatTransform.name.Contains("medi medium")) things = brigBollards;
            else if (boatTransform.name.Contains("dhow medium")) things = sanbuqBollards;
            else if (boatTransform.name.Contains("junk medium")) things = junkBollards;
            else if (boatTransform.name.Contains("medi small") && Plugin.smallBoats.Value) things = cogBollards;
            else if (boatTransform.name.Contains("dhow small") && Plugin.smallBoats.Value) things = dhowBollards;
            else if (boatTransform.name.Contains("junk small") && Plugin.smallBoats.Value) things = kakamBollards;
            else return;

            for (int i = 0; i < things.Length; i++)
            {
                object[] thing = things[i] as object[];
                GameObject boatBollard = Instantiate(bollard, (Vector3)thing[1], (Quaternion)thing[2]);
                boatBollard.transform.SetParent(mooringSetTransform, false);
                boatBollard.name = (string)thing[0];

                boatBollard.transform.localScale = (Vector3)thing[3];
                boatBollard.tag = "Boat";

                foreach (Outline outline in boatBollard.GetComponents<Outline>())
                {

                    if (!ReferenceEquals(outline, boatBollard.GetPrivateField("outline")))
                    {
                        Destroy(outline);
                    }
                }
            }
            bollards = gameObject.GetComponentsInChildren<GPButtonDockMooring>();
        }

        static readonly object[] brig0 = { "bollard_stern_left", new Vector3(-14.9f, 4.36f, 2.23f), new Quaternion(0.7071068f, 0f, 0f, 0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) }; // 90, 0, 0
        static readonly object[] brig1 = { "bollard_stern_right", new Vector3(-14.9f, 4.36f, -2.23f), new Quaternion(0.7071068f, 0f, 0f, 0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] brig2 = { "bollard_mid_aft_left", new Vector3(-2.3f, 3.53f, 3.27f), new Quaternion(-0.7058f, 0.0252f, -0.0246f, 0.7075f), new Vector3(0.6f, 0.6f, 0.6f) }; // 270, 0, 0
        static readonly object[] brig3 = { "bollard_mid_aft_right", new Vector3(-2.3f, 3.53f, -3.27f), new Quaternion(-0.7058f, 0.0252f, -0.0246f, 0.7075f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] brig4 = { "bollard_mid_fore_left", new Vector3(4.85f, 3.57f, 3.02f), new Quaternion(-0.7041f, 0.0292f, 0.0692f, 0.7061f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] brig5 = { "bollard_mid_fore_right", new Vector3(4.85f, 3.57f, -3.02f), new Quaternion(-0.7036f, -0.0384f, 0.0012f, 0.7095f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly Array[] brigBollards = { brig0, brig1, brig2, brig3, brig4, brig5 };

        static readonly object[] sanbuq0 = { "bollard_stern_left", new Vector3(-12.08f, 2.12f, 1.48f), new Quaternion(0.4912f, -0.5087f, -0.5087f, -0.4912f), new Vector3(0.6f, 0.6f, 0.6f) }; // 270, 92, 0
        static readonly object[] sanbuq1 = { "bollard_stern_right", new Vector3(-12.08f, 2.12f, -1.48f), new Quaternion(0.5087f, -0.4912f, -0.4912f, -0.5087f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] sanbuq2 = { "bollard_mid_aft_left", new Vector3(-1.4f, 2.12f, 2.46f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] sanbuq3 = { "bollard_mid_aft_right", new Vector3(-1.4f, 2.12f, -2.46f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] sanbuq4 = { "bollard_mid_fore_left", new Vector3(4.2f, 2.32f, 2.25f), new Quaternion(-0.7071f, 0.0f, 0.0371f, 0.7061f), new Vector3(0.6f, 0.6f, 0.6f) }; // 270, 4, 0
        static readonly object[] sanbuq5 = { "bollard_mid_fore_right", new Vector3(4.2f, 2.32f, -2.25f), new Quaternion(-0.7058f, -0.0432f, 0.0062f, 0.7071f), new Vector3(0.6f, 0.6f, 0.6f) }; // 270, -4, 0
        static readonly Array[] sanbuqBollards = { sanbuq0, sanbuq1, sanbuq2, sanbuq3, sanbuq4, sanbuq5 };

        static readonly object[] junk0 = { "bollard_stern_left", new Vector3(-9.7f, 2.27f, 0.8f), new Quaternion(0.4731f, -0.5255f, -0.5255f, -0.4731f), new Vector3(0.6f, 0.6f, 0.6f) }; //270, 96, 0
        static readonly object[] junk1 = { "bollard_stern_right", new Vector3(-9.7f, 2.27f, -0.8f), new Quaternion(0.5255f, -0.4731f, -0.4731f, -0.5255f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] junk2 = { "bollard_mid_aft_left", new Vector3(1.24f, 1.01f, 2.52f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] junk3 = { "bollard_mid_aft_right", new Vector3(1.24f, 1.01f, -2.52f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) };
        static readonly object[] junk4 = { "bollard_mid_fore_left", new Vector3(7.68f, 1.1f, 2.02f), new Quaternion(-0.7071f, 0.0062f, 0.0309f, 0.7064f), new Vector3(0.6f, 0.6f, 0.6f) }; //270, 15, 0
        static readonly object[] junk5 = { "bollard_mid_fore_right", new Vector3(7.68f, 1.1f, -2.02f), new Quaternion(-0.7064f, -0.0309f, -0.0061f, 0.7071f), new Vector3(0.6f, 0.6f, 0.6f) }; //270, -15, 0
        static readonly Array[] junkBollards = { junk0, junk1, junk2, junk3, junk4, junk5 };

        static readonly object[] cog0 = { "bollard_stern_left", new Vector3(6.66f, 2.41f, 1.25f), new Quaternion(0.5f, -0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f) };
        static readonly object[] cog1 = { "bollard_stern_right", new Vector3(6.66f, 2.41f, -1.25f), new Quaternion(0.5f, -0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f) };
        static readonly Array[] cogBollards = { cog0, cog1 };

        static readonly object[] dhow0 = { "bollard_stern_left", new Vector3(-6.4f, 1.815f, 1.53f), new Quaternion(-0.0616f, -0.7044f, -0.7044f, -0.0616f), new Vector3(0.5f, 0.5f, 0.5f) };
        static readonly object[] dhow1 = { "bollard_stern_right", new Vector3(-6.4f, 1.815f, -1.53f), new Quaternion(-0.0616f, -0.7044f, -0.7044f, -0.0616f), new Vector3(0.5f, 0.5f, 0.5f) };
        static readonly Array[] dhowBollards = { dhow0, dhow1 };

        static readonly object[] kakam0 = { "bollard_stern_left", new Vector3(-6.3f, 2.86f, 0.64f), new Quaternion(0.6744f, 0.2126f, 0.2126f, -0.6744f), new Vector3(0.5f, 0.5f, 0.5f) };
        static readonly object[] kakam1 = { "bollard_stern_right", new Vector3(-6.3f, 2.86f, -0.64f), new Quaternion(0.6744f, -0.2126f, -0.2126f, -0.6744f), new Vector3(0.5f, 0.5f, 0.5f) };
        static readonly Array[] kakamBollards = { kakam0, kakam1 };

    }
}
