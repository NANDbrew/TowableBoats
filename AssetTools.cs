using System.IO;
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
            string firstTry = Path.Combine(Plugin.dataPath, assetDir, assetFile);
            string secondTry = Path.Combine(Plugin.dataPath, assetFile);
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
