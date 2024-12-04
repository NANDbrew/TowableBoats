using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TowableBoats
{
    public static class Util
    {
        public static object InvokePrivate(object obj, string name, object arg)
        {
            return AccessTools.Method(obj.GetType(), name).Invoke(obj, new object[1] { arg });
        }
    }
}
