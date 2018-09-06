using System;
using System.Collections.Generic;
using AutoMcD.SmartRotors.Data;
using AutoMcD.SmartRotors.Extensions;
using ParallelTasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sisk.Utils.Logging;
using Sisk.Utils.Profiler;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;

namespace AutoMcD.SmartRotors.Logic {
    /// <summary>
    ///     Provide game logic for Smart Solar Rotors bases.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, LB_SMART_SOLAR_ROTOR)]
    public sealed class SmartSolarRotorBase : SmartRotorBase {
        private const string LB_SMART_SOLAR_ROTOR = "MA_SmartRotor_Solar_Base";

        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartSolarRotorBase" /> game logic component.
        /// </summary>
        public SmartSolarRotorBase() : base(nameof(SmartSolarRotorBase)) {
            Log = Mod.Static.Log.ForScope<SmartSolarRotorBase>();
        }

        /// <summary>
        ///     Logger used for logging.
        /// </summary>
        private ILogger Log { get; }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder) {
            base.Init(objectBuilder);

            if (Stator.IsProjected()) {
                return;
            }

            NeedsUpdate = MyEntityUpdateEnum.EACH_100TH_FRAME;
        }

        /// <inheritdoc />
        public override void UpdateBeforeSimulation100() {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartRotorSolarHinge), nameof(UpdateBeforeSimulation100)) : null) {
                if (Stator == null || !Stator.IsWorking || Stator.Top == null || Stator.Top.Closed) {
                    return;
                }

                var sunDirection = Mod.Static.SunTracker.CalculateSunDirection();
                Stator.PointRotorAtVector(sunDirection, Stator.Top.WorldMatrix.Forward);
            }
        }

        /// <inheritdoc />
        protected override void PlaceSmartHinge(WorkData workData) {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartSolarRotorBase), nameof(PlaceSmartHinge)) : null) {
                using (Log.BeginMethod(nameof(PlaceSmartHinge))) {
                    var data = workData as PlaceSmartHingeData;

                    if (data?.Head == null) {
                        return;
                    }

                    var head = data.Head;

                    var cubeGrid = head.CubeGrid;
                    var gridSize = cubeGrid.GridSize;
                    var forward = head.WorldMatrix.Forward;
                    var up = head.WorldMatrix.Up;

                    var headPosition = head.GetPosition();
                    var origin = headPosition + up * gridSize;
                    var hingePosition = cubeGrid.WorldToGridInteger(origin);
                    if (cubeGrid.CubeExists(hingePosition)) {
                        Log.Debug($"There is already a block on this position: {hingePosition}.");
                        return;
                    }

                    var canPlaceCube = cubeGrid.CanAddCube(hingePosition);
                    if (!canPlaceCube) {
                        Log.Debug($"Unable to place block on this position: {hingePosition}.");
                    }

                    try {
                        var instantBuild = MyAPIGateway.Session.CreativeMode || MyAPIGateway.Session.HasCreativeRights && MyAPIGateway.Session.EnableCopyPaste;
                        var buildPercent = instantBuild ? 1 : 0.00001525902f;
                        var hingeBuilder = new MyObjectBuilder_MotorAdvancedStator {
                            SubtypeName = "MA_SmartRotor_Solar_Hinge",
                            Owner = Stator.OwnerId,
                            BuiltBy = Stator.OwnerId,
                            BuildPercent = buildPercent,
                            IntegrityPercent = buildPercent
                        };

                        var cubeGridBuilder = new MyObjectBuilder_CubeGrid {
                            CreatePhysics = true,
                            GridSizeEnum = head.CubeGrid.GridSizeEnum,
                            PositionAndOrientation = new MyPositionAndOrientation(origin, up, forward)
                        };

                        cubeGridBuilder.CubeBlocks.Add(hingeBuilder);

                        var gridsToMerge = new List<MyObjectBuilder_CubeGrid> { cubeGridBuilder };

                        MyAPIGateway.Utilities.InvokeOnGameThread(() => (cubeGrid as MyCubeGrid)?.PasteBlocksToGrid(gridsToMerge, 0, false, false));
                    } catch (Exception exception) {
                        Log.Error(exception);
                    }
                }
            }
        }
    }
}