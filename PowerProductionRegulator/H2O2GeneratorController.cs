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
    partial class Program
    {
        public class H2O2GeneratorController
        {
            private IMyGridTerminalSystem _gridTerminalSystem;
            private List<IMyGasGenerator> _h2O2Generators = new List<IMyGasGenerator>();
            private List<IMyGasTank> _hydrogenTanks = new List<IMyGasTank>();
            private List<IMyGasTank> _oxygenTanks = new List<IMyGasTank>();
            private List<IMyCargoContainer> _cargoContainers = new List<IMyCargoContainer>();
            private MyItemType iceType = MyItemType.MakeOre("ice");

            public H2O2GeneratorController(IMyGridTerminalSystem gridTerminalSystem)
            {
                _gridTerminalSystem = gridTerminalSystem;
                _gridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(_h2O2Generators);
                _gridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(_cargoContainers);

                var tempGasTankList = new List<IMyGasTank>();
                _gridTerminalSystem.GetBlocksOfType<IMyGasTank>(tempGasTankList);
                tempGasTankList.ForEach(tempGasTank =>
                {
                    if (tempGasTank.BlockDefinition.ToString().Contains("Hydrogen"))
                    {
                        _hydrogenTanks.Add(tempGasTank);
                    }
                    else
                    {
                        _oxygenTanks.Add(tempGasTank);
                    }
                });
            }

            private float _TanksFillRatio(List<IMyGasTank> tanksToBeRefilled)
            {
                var tanksCapacity = 0f;
                var tanksFillState = 0f;

                tanksToBeRefilled.ForEach(tank =>
                {
                    tanksCapacity += tank.Capacity;
                    tanksFillState += (float)tank.FilledRatio * tank.Capacity;
                });

                return tanksFillState / tanksCapacity;
            }

            private void _SetGenerators(Boolean newState)
            {
                _h2O2Generators.ForEach(h2o2Generator =>
                {
                    h2o2Generator.SetValueBool("OnOff", newState);
                });
            }

            public void CheckGeneratorState()
            {
                var icePresent = false;
                _cargoContainers.ForEach(cargoContainer => { 
                    if (cargoContainer.GetInventory().ContainItems(1, iceType)) 
                    { 
                        icePresent = true;
                        return; 
                    } 
                });

                if (!icePresent) { 
                    _SetGenerators(false);
                    return; 
                }

                var hydrogenTanksFillRatio = _TanksFillRatio(_hydrogenTanks);
                var oxygenTanksFillRatio = _TanksFillRatio(_oxygenTanks);

                if (hydrogenTanksFillRatio < 0.7 || oxygenTanksFillRatio < 0.1) _SetGenerators(true);
                else if (hydrogenTanksFillRatio > 0.95 && oxygenTanksFillRatio > 0.3) _SetGenerators(false);
            }
        }
    }
}
