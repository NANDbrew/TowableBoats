using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;

namespace TowableBoats
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.app24.sailwindmoddinghelper", "2.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_ID = "com.nandbrew.towableboats";
        public const string PLUGIN_NAME = "Towable Boats";
        public const string PLUGIN_VERSION = "0.1.4";

        //--settings--
        internal static ConfigEntry<int> performanceMode;
        internal static ConfigEntry<bool> multiParent;
        internal static ConfigEntry<bool> drag;

        internal static ManualLogSource logSource;

        private void Awake()
        {
            logSource = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            performanceMode = Config.Bind("Settings", "Performance mode", 1, new ConfigDescription("How many boats in the chain behind the current boat get full physics", new AcceptableValueRange<int>(0, 5), new ConfigurationManagerAttributes { Order = 1 }));
            multiParent = Config.Bind("Settings", "Team towing", false, new ConfigDescription("Allow a boat to BE TOWED BY multiple boats at once (buggy)", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            drag = Config.Bind("Settings", "Drag", false, new ConfigDescription("Allow boats to pull on the boat towing them (buggy)", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
        }
    }
}
