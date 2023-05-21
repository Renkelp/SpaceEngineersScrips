using Sandbox.Definitions;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.Entities.Blocks;
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
       private List<IMyBatteryBlock> batteryList = new List<IMyBatteryBlock>();
       private List<IMyPowerProducer> engineReactorList= new List<IMyPowerProducer>();
       private H2O2GeneratorController h202Controller;

        public Program()
        {
            h202Controller = new H2O2GeneratorController(GridTerminalSystem);
            
            var tempPowerProducers = new List<IMyPowerProducer>();

            GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(tempPowerProducers);
            tempPowerProducers.ForEach(powerProducer =>
            {
                if (powerProducer.BlockDefinition.ToString().Equals("MyObjectBuilder_HydrogenEngine/LargeHydrogenEngine"))
                {
                    engineReactorList.Add(powerProducer);
                }
            });
            GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteryList);

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main()
        {
            h202Controller.CheckGeneratorState();

            float maxStoredPower = 0;
            float currentStoredPower = 0;

            batteryList.ForEach(battery =>
            {
                maxStoredPower += battery.MaxStoredPower;
                currentStoredPower += battery.CurrentStoredPower;
            });
            float percentagePowerStorage = currentStoredPower / maxStoredPower;

            if (percentagePowerStorage < 0.3) SetEnginesState(true);
            else if (percentagePowerStorage > 0.4) SetEnginesState(false);
        }

        private void SetEnginesState(bool state)
        {
            engineReactorList.ForEach(powerProducer => powerProducer.SetValue("OnOff", state));
        }
    }
}
