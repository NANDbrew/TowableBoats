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
        public const string PLUGIN_VERSION = "0.2.4";

        //--settings--
        internal static ConfigEntry<int> performanceMode;
        internal static ConfigEntry<bool> ignoreWarning;
        internal static ConfigEntry<bool> smallBoats;

        internal static Plugin instance;
        internal static string dataPath;

        private void Awake()
        {
            instance = this;
            dataPath = Directory.GetParent(Info.Location).FullName;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            performanceMode = Config.Bind("Settings", "Performance mode", 1, new ConfigDescription("How many boats in the chain behind the current boat get full physics", new AcceptableValueRange<int>(0, 10), new ConfigurationManagerAttributes { Order = 2 }));
            smallBoats = Config.Bind("Settings", "Small boats can tow", true, new ConfigDescription("Add anchor points to small boats", null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1}));
            ignoreWarning = Config.Bind("Settings", "Ignore NANDfixes warning", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            AssetTools.LoadAssetBundles();


        }
    }
}
