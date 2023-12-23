using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;

namespace TowableBoats
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_ID = "com.nandbrew.towableboats";
        public const string PLUGIN_NAME = "Towable Boats";
        public const string PLUGIN_VERSION = "0.0.1";

        //--settings--
        internal static ConfigEntry<int> performanceMode;

        internal static ManualLogSource logSource;

        private void Awake()
        {
            logSource = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            performanceMode = Config.Bind("Settings", "Performance Mode", 2, new ConfigDescription("", new AcceptableValueRange<int>(0, 2)));
        }
    }
}
