using System;
using System.Collections.Generic;
using ParallelTasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sisk.SmartRotors.Data;
using Sisk.SmartRotors.Extensions;
using Sisk.Utils.Logging;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRageMath;

namespace Sisk.SmartRotors.Logic {
    /// <summary>
    ///     Provide game logic for Smart Solar Rotors bases.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, Defs.SolarDefs.LB_SMART_SOLAR_ROTOR, Defs.SolarDefs.LB_SMART_SOLAR_ROTOR_B, Defs.SolarDefs.SB_SMART_SOLAR_ROTOR_B)]
    public sealed class SmartSolarRotorBase : SmartRotorBase {
        private const string ERROR_BUILD_SPOT_OCCUPIED = "Solar hinge cannot be placed. Build spot occupied.";
        private const string ERROR_UNABLE_TO_PLACE = "Solar hinge cannot be placed.";

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

        /// <summary>
        ///     Called if entity is added to scene.
        /// </summary>
        public override void OnAddedToScene() {
            base.OnAddedToScene();

            if (!Mod.Static.Controls.AreTerminalControlsInitialized) {
                Mod.Static.Controls.InitializeControls();
            }

            NeedsUpdate = MyEntityUpdateEnum.EACH_100TH_FRAME;
        }

        /// <inheritdoc />
        public override void UpdateBeforeSimulation100() {
            if (Stator == null || !Stator.IsWorking || Stator.Top == null || Stator.Top.Closed) {
                return;
            }

            var sunDirection = Mod.Static.SunTracker.CalculateSunDirection();
            Stator.PointRotorAtVector(sunDirection, Stator.Top.WorldMatrix.Forward);
        }

        /// <inheritdoc />
        protected override void PlaceSmartHinge(WorkData workData) {
            using (Log.BeginMethod(nameof(PlaceSmartHinge))) {
                var data = workData as PlaceSmartHingeData;

                if (data?.Head == null) {
                    return;
                }

                var head = data.Head;

                var cubeGrid = head.CubeGrid;
                var gridSize = cubeGrid.GridSize;
                var matrix = head.WorldMatrix;
                var up = matrix.Up;
                var left = matrix.Left;
                var forward = matrix.Forward;

                var headPosition = head.GetPosition();
                var baseSubtype = Stator.BlockDefinition.SubtypeId;
                Vector3D origin;
                switch (baseSubtype) {
                    case Defs.SolarDefs.LB_SMART_SOLAR_ROTOR:
                    case Defs.SolarDefs.LB_SMART_SOLAR_ROTOR_B:
                    default:
                        origin = headPosition + up * gridSize;
                        break;
                    case Defs.SolarDefs.SB_SMART_SOLAR_ROTOR_B:
                        origin = headPosition + up * 4 * gridSize - left * gridSize + forward * gridSize;
                        break;
                }

                var hingePosition = cubeGrid.WorldToGridInteger(origin);

                if (cubeGrid.CubeExists(hingePosition)) {
                    Log.Error(ERROR_BUILD_SPOT_OCCUPIED);
                    MyAPIGateway.Utilities.ShowNotification(ERROR_BUILD_SPOT_OCCUPIED);
                    data.FlagAsFailed();
                    return;
                }

                var canPlaceCube = cubeGrid.CanAddCube(hingePosition);
                if (!canPlaceCube) {
                    Log.Error(ERROR_UNABLE_TO_PLACE);
                    MyAPIGateway.Utilities.ShowNotification(ERROR_UNABLE_TO_PLACE);
                    data.FlagAsFailed();
                }

                try {
                    var instantBuild = MyAPIGateway.Session.CreativeMode || MyAPIGateway.Session.HasCreativeRights && MyAPIGateway.Session.EnableCopyPaste;
                    var buildPercent = instantBuild ? 1 : 0.00001525902f;
                    string hingeSubtype;
                    if (Mod.Static.Defs.Solar.BaseToHinge.TryGetValue(baseSubtype, out hingeSubtype)) {
                        var hingeBuilder = new MyObjectBuilder_MotorAdvancedStator {
                            SubtypeName = hingeSubtype,
                            Owner = Stator.OwnerId,
                            BuiltBy = Stator.OwnerId,
                            BuildPercent = buildPercent,
                            IntegrityPercent = buildPercent,
                            LimitsActive = true,
                            MaxAngle = MathHelper.ToRadians(195),
                            MinAngle = MathHelper.ToRadians(-15),
                            CustomName = SmartRotorHinge.AUTO_PLACED_TAG
                        };

                        var cubeGridBuilder = new MyObjectBuilder_CubeGrid {
                            CreatePhysics = true,
                            GridSizeEnum = head.CubeGrid.GridSizeEnum,
                            PositionAndOrientation = new MyPositionAndOrientation(origin, up, left)
                        };

                        cubeGridBuilder.CubeBlocks.Add(hingeBuilder);
                        var gridsToMerge = new List<MyObjectBuilder_CubeGrid> { cubeGridBuilder };

                        MyAPIGateway.Utilities.InvokeOnGameThread(() => (cubeGrid as MyCubeGrid)?.PasteBlocksToGrid(gridsToMerge, 0, false, false));
                        data.FlagAsSucceeded();
                    }
                } catch (Exception exception) {
                    Log.Error(exception);
                    data.FlagAsFailed();
                }
            }
        }
    }
}