using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using System.Reflection;

namespace TowableBoats
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    //[BepInDependency("com.nandbrew.nandfixes", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_ID = "com.nandbrew.towableboats";
        public const string PLUGIN_NAME = "Towable Boats";
        public const string PLUGIN_VERSION = "0.2.3";

        //--settings--
        internal static ConfigEntry<int> performanceMode;
        //internal static ConfigEntry<bool> multiParent;
        internal static ConfigEntry<bool> drag;
        internal static ConfigEntry<bool> smallBoats;

        internal static Plugin instance;
        internal static string dataPath;

        private void Awake()
        {
            instance = this;
            dataPath = Directory.GetParent(Info.Location).FullName;
            /*string firstTry = Path.Combine(dataPath, PLUGIN_NAME.Replace(" ", ""));
            dataPath += Directory.Exists(firstTry) ? firstTry : "";*/

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            performanceMode = Config.Bind("Settings", "Performance mode", 1, new ConfigDescription("How many boats in the chain behind the current boat get full physics", new AcceptableValueRange<int>(0, 5), new ConfigurationManagerAttributes { Order = 2 }));
            //multiParent = Config.Bind("Settings", "Team towing", false, new ConfigDescription("Allow a boat to BE TOWED BY multiple boats at once (buggy)", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            drag = Config.Bind("Settings", "Drag", false, new ConfigDescription("Allow boats to pull on the boat towing them (buggy)", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            smallBoats = Config.Bind("Settings", "Small boats can tow", true, new ConfigDescription("Add anchor points to small boats", null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1}));
            AssetTools.LoadAssetBundles();


        }
    }
}
