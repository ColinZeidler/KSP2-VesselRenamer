using BepInEx;
using BepInEx.Logging;
using KSP;
using KSP.Game;
using KSP.Sim.impl;
using KSP.UI.Binding;
using MoonSharp.VsCodeDebugger.SDK;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;

namespace VesselRenamer
{
    [BepInPlugin("com.github.ColinZeidler.VesselRenamer", "VesselRenamer", "0.1.0")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class VesselRenamerMod : BaseSpaceWarpPlugin
    {
        private bool showGui = false;
        private ManualLogSource logger = null;
        private Rect windowRect;
        private string newName = "";

        private int width = 300;
        private int height = 100;

        public override void OnInitialized()
        {
            base.OnInitialized();

            logger = BepInEx.Logging.Logger.CreateLogSource("com.github.ColinZeidler.VesselRenamer");

            Appbar.RegisterAppButton(
                "Rename Vessel",
                "BTN-ReV",
                AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                ToggleButton);
            logger.LogInfo("Initialized Plugin");
        }

        void ToggleButton(bool toggle)
        {
            showGui = toggle;
            GameObject.Find("BTN-ReV")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);
        }

        void Awake()
        {
            // Set initial position for window
            windowRect = new Rect((Screen.width * 0.7f) - (width / 2), (Screen.height / 2) - (height / 2), 0, 0);
        }

        void OnGUI()
        {
            if (showGui)
            {
                GUI.skin = Skins.ConsoleSkin;
                windowRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    windowRect,
                    PopulateGui,
                    "<color=#696DFF>// Vessel Renamer</color>",
                    GUILayout.Height(height),
                    GUILayout.Width(width));

            }

        }

        private void PopulateGui(int WindowID)
        {
            GameInstance game = GameManager.Instance.Game;
            VesselComponent vessel = game.ViewController.GetActiveVehicle(true)?.GetSimVessel(true);

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Active Vehicle:");
            GUILayout.FlexibleSpace();
            GUILayout.Label(vessel?.Name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("New name:");
            newName = GUILayout.TextField(newName);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Rename") && newName != "")
            {
                vessel.SimulationObject.Name = newName;
            }

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, height, width));
        }

    }
}