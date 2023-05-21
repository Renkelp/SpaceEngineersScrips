using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // Enter all AirVent names that should be included in the display
        private List<string> airVentNameList = new List<string> { "Air Vent Storage Room - Base" };

        // Enter the name of the LCD display
        private string lcdDisplayName = "LCD Panel Storage Room - Base";

        private List<IMyAirVent> airVentList = new List<IMyAirVent>();
        private IMyTextSurface lcdDisplay;

        public Program()
        {
            lcdDisplay = GridTerminalSystem.GetBlockWithName(lcdDisplayName) as IMyTextSurface;
            lcdDisplay.ContentType = ContentType.TEXT_AND_IMAGE;
            lcdDisplay.Alignment = TextAlignment.LEFT;
            var allAirVents = new List<IMyAirVent>();
            GridTerminalSystem.GetBlocksOfType<IMyAirVent>(allAirVents);

            airVentNameList.ForEach(blockName => airVentList.AddList(allAirVents.FindAll((airVent) => airVent.DisplayNameText.Equals(blockName))));

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            String newStatus = String.Empty;
            airVentList.ForEach(airVent =>
            {
                var name = airVent.DisplayNameText;
                var isEnabled = airVent.Enabled ? "On" : "Off";
                var isWorking = airVent.IsWorking;
                var roomOxygenLevel = airVent.GetOxygenLevel();
                
                var pressurizationState = airVent.Depressurize ? "Depressurizing" : "Pressurizing";
                newStatus += $"Name: {name}\nOxygenLevel: {roomOxygenLevel}\nStatus: {isEnabled}\nAble to work: {isWorking}\n{pressurizationState}\n---\n";
            });

            lcdDisplay.WriteText(newStatus, false);
        }
    }
}
