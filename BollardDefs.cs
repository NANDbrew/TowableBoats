using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TowableBoats
{
    public readonly struct CleatDefs
    {
        public static readonly Array[] brig = 
        {
            new object[] { "cleat_stern_left", new Vector3(-14.9f, 4.36f, 2.23f), new Quaternion(0.7071068f, 0f, 0f, 0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) },
            new object[] { "cleat_stern_right", new Vector3(-14.9f, 4.36f, -2.23f), new Quaternion(0.7071068f, 0f, 0f, 0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) },
            new object[] { "cleat_mid_aft_left", new Vector3(-2.3f, 3.53f, 3.27f), new Quaternion(-0.7058f, 0.0252f, -0.0246f, 0.7075f), new Vector3(0.6f, 0.6f, 0.6f) },
            new object[] { "cleat_mid_aft_right", new Vector3(-2.3f, 3.53f, -3.27f), new Quaternion(-0.7058f, 0.0252f, -0.0246f, 0.7075f), new Vector3(0.6f, 0.6f, 0.6f) },
            new object[] { "cleat_mid_fore_left", new Vector3(4.85f, 3.57f, 3.02f), new Quaternion(-0.7041f, 0.0292f, 0.0692f, 0.7061f), new Vector3(0.6f, 0.6f, 0.6f) },
            new object[] { "cleat_mid_fore_right", new Vector3(4.85f, 3.57f, -3.02f), new Quaternion(-0.7036f, -0.0384f, 0.0012f, 0.7095f), new Vector3(0.6f, 0.6f, 0.6f) }
        };

        public static readonly Array[] sanbuq = 
        {
            new object[] { "cleat_stern_left", new Vector3(-12.08f, 2.12f, 1.48f), new Quaternion(0.4912f, -0.5087f, -0.5087f, -0.4912f), new Vector3(0.6f, 0.6f, 0.6f) },
            new object[] { "cleat_stern_right", new Vector3(-12.08f, 2.12f, -1.48f), new Quaternion(0.5087f, -0.4912f, -0.4912f, -0.5087f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_mid_aft_left", new Vector3(-1.4f, 2.12f, 2.46f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_mid_aft_right", new Vector3(-1.4f, 2.12f, -2.46f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_mid_fore_left", new Vector3(4.2f, 2.32f, 2.25f), new Quaternion(-0.7071f, 0.0f, 0.0371f, 0.7061f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_mid_fore_right", new Vector3(4.2f, 2.32f, -2.25f), new Quaternion(-0.7058f, -0.0432f, 0.0062f, 0.7071f), new Vector3(0.6f, 0.6f, 0.6f) }
        };

        public static readonly Array[] junk = { 
            new object[] { "cleat_stern_left", new Vector3(-9.7f, 2.27f, 0.8f), new Quaternion(0.4731f, -0.5255f, -0.5255f, -0.4731f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_stern_right", new Vector3(-9.7f, 2.27f, -0.8f), new Quaternion(0.5255f, -0.4731f, -0.4731f, -0.5255f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_mid_aft_left", new Vector3(1.24f, 1.01f, 2.52f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_mid_aft_right", new Vector3(1.24f, 1.01f, -2.52f), new Quaternion(0.7071068f, 0f, 0f, -0.7071068f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_mid_fore_left", new Vector3(7.68f, 1.1f, 2.02f), new Quaternion(-0.7071f, 0.0062f, 0.0309f, 0.7064f), new Vector3(0.6f, 0.6f, 0.6f) }, 
            new object[] { "cleat_mid_fore_right", new Vector3(7.68f, 1.1f, -2.02f), new Quaternion(-0.7064f, -0.0309f, -0.0061f, 0.7071f), new Vector3(0.6f, 0.6f, 0.6f) } 
        };

        public static readonly Array[] cog = { 
            new object[] { "cleat_stern_left", new Vector3(6.66f, 2.41f, 1.25f), new Quaternion(0.5f, -0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f) }, 
            new object[] { "cleat_stern_right", new Vector3(6.66f, 2.41f, -1.25f), new Quaternion(0.5f, -0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f) } 
        };

        public static readonly Array[] dhow = { 
            new object[] { "cleat_stern_left", new Vector3(-6.4f, 1.815f, 1.53f), new Quaternion(-0.0616f, -0.7044f, -0.7044f, -0.0616f), new Vector3(0.5f, 0.5f, 0.5f) }, 
            new object[] { "cleat_stern_right", new Vector3(-6.4f, 1.815f, -1.53f), new Quaternion(-0.0616f, -0.7044f, -0.7044f, -0.0616f), new Vector3(0.5f, 0.5f, 0.5f) } 
        };

        public static readonly Array[] kakam = { 
            new object[] { "cleat_stern_left", new Vector3(-6.3f, 2.86f, 0.64f), new Quaternion(0.6744f, 0.2126f, 0.2126f, -0.6744f), new Vector3(0.5f, 0.5f, 0.5f) }, 
            new object[] { "cleat_stern_right", new Vector3(-6.3f, 2.86f, -0.64f), new Quaternion(0.6744f, -0.2126f, -0.2126f, -0.6744f), new Vector3(0.5f, 0.5f, 0.5f) } 
        };
    }
}
