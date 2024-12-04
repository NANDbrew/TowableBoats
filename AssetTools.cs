using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TowableBoats
{
    internal class AssetTools
    {
        public static AssetBundle bundle;
        const string assetDir = "TowableBoats";
        const string assetFile = "towable_boats.assets";
        public static void LoadAssetBundles()    //Load the bundle
        {
            string firstTry = Path.Combine(Paths.PluginPath, assetDir, assetFile);
            string secondTry = Path.Combine(Paths.PluginPath, assetFile);
            //else { Debug.LogError("TowableBoats: can't find asset file"); return; }

            bundle = AssetBundle.LoadFromFile(File.Exists(firstTry) ? firstTry : secondTry);
            if (bundle == null)
            {
                Debug.LogError("Bundle not loaded! Did you place it in the correct folder?");
            }
            else { Debug.Log("TowableBoats: loaded bundle " + bundle.ToString()); }
        }

    }
}
