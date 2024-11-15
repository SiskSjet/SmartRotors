using System;
using System.Collections.Generic;
using System.Linq;
using ParallelTasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using Sisk.SmartRotors.Data;
using Sisk.SmartRotors.Extensions;
using Sisk.Utils.Logging;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;
using static Sisk.SmartRotors.Defs;

namespace Sisk.SmartRotors.Logic {

    /// <summary>
    ///     Provide game logic for Smart Solar Rotors bases.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, Defs.SolarDefs.LB_SMART_SOLAR_BASE, Defs.SolarDefs.LB_SMART_SOLAR_BASE_TYPE_B, Defs.SolarDefs.SB_SMART_SOLAR_BASE_TYPE_B)]
    public sealed class SmartSolarRotorBase : SmartRotorBase {
        private const string ADD_HEAD_ACTION_ID = "AddRotorTopPart";
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

        public override void Init(MyObjectBuilder_EntityBase objectBuilder) {
            using (Log.BeginMethod(nameof(Init))) {
                Log.Debug($"START {nameof(Init)}");
                base.Init(objectBuilder);

                if (!Mod.Static.Controls.AreTerminalControlsInitialized) {
                    Mod.Static.Controls.InitializeControls();
                }

                if (Mod.Static.Network == null || Mod.Static.Network.IsServer) {
                    NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
                }

                Log.Debug($"END {nameof(Init)}");
            }
        }

        /// <inheritdoc />
        public override void UpdateBeforeSimulation100() {
            if (Stator == null || !Stator.IsWorking || Stator.Top == null || Stator.Top.Closed) {
                return;
            }

            Vector3D sunDirection;
            if (Mod.Static.RealStarsApi.IsReady) {
                var info = Mod.Static.RealStarsApi.GetSunInfoNearbyPlanets(Stator.GetPosition());
                sunDirection = Mod.Static.RealStarsApi.GetSunInfoWithPlanetAtPosition(Stator.GetPosition(), info.Item1, info.Item2, false).Item1;
            } else {
                sunDirection = (Vector3D)MyVisualScriptLogicProvider.GetSunDirection();
            }

            Stator.PointRotorAtVector(sunDirection, Stator.Top.WorldMatrix.Forward, 3 * MathHelper.RPMToRadiansPerSecond);
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
                    case Defs.SolarDefs.LB_SMART_SOLAR_BASE:
                    case Defs.SolarDefs.LB_SMART_SOLAR_BASE_TYPE_B:
                    default:
                        origin = headPosition + up * gridSize;
                        break;

                    case Defs.SolarDefs.SB_SMART_SOLAR_BASE_TYPE_B:
                        origin = headPosition + up + left * gridSize + forward * gridSize;
                        break;
                }

                var hingePosition = cubeGrid.WorldToGridInteger(origin);
                string hingeSubtype;
                if (!Mod.Static.Defs.Solar.BaseToHinge.TryGetValue(baseSubtype, out hingeSubtype)) {
                    Log.Error($"No matching hinge found for '{baseSubtype}'");
                    data.FlagAsFailed();
                    return;
                }

                if (cubeGrid.CubeExists(hingePosition)) {
                    var slimBlock = cubeGrid.GetCubeBlock(hingePosition);
                    var hinge = slimBlock?.FatBlock as IMyMotorAdvancedStator;
                    if (hinge != null) {
                        if (hinge.BlockDefinition.SubtypeId == hingeSubtype) {
                            data.FlagAsSucceeded();
                            return;
                        }
                    }

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
                    var colorMask = Stator.SlimBlock.ColorMaskHSV;
                    var skin = Stator.SlimBlock.SkinSubtypeId;
                    var buildPercent = head.SlimBlock.IsFullIntegrity ? 1 : 0.00001525902f;
                    var maxAngle = hingeSubtype == SolarDefs.LB_SMART_SOLAR_HINGE ? MathHelper.ToRadians(195) : float.MaxValue;
                    var minAngle = hingeSubtype == SolarDefs.LB_SMART_SOLAR_HINGE ? MathHelper.ToRadians(-15) : float.MinValue;
                    var hingeBuilder = new MyObjectBuilder_MotorAdvancedStator {
                        SubtypeName = hingeSubtype,
                        Owner = Stator.OwnerId,
                        BuiltBy = Stator.OwnerId,
                        BuildPercent = buildPercent,
                        IntegrityPercent = buildPercent,
                        LimitsActive = true,
                        MaxAngle = maxAngle,
                        MinAngle = minAngle,

                        Min = hingePosition,
                        BlockOrientation = new SerializableBlockOrientation(head.Orientation.Up, head.Orientation.Left),
                        ColorMaskHSV = colorMask
                    };

                    MyAPIGateway.Utilities.InvokeOnGameThread(() => {
                        cubeGrid.AddBlock(hingeBuilder, false);
                        var slimBlock = cubeGrid.GetCubeBlock(hingePosition);
                        var hinge = slimBlock?.FatBlock as IMyMotorAdvancedStator;
                        if (hinge != null) {
                            if (hinge.BlockDefinition.SubtypeId == hingeSubtype) {
                                List<IMyTerminalAction> defaultActions;
                                MyAPIGateway.TerminalControls.GetActions<IMyMotorAdvancedStator>(out defaultActions);

                                var attach = defaultActions.FirstOrDefault(x => x.Id == ADD_HEAD_ACTION_ID)?.Action;
                                if (attach == null) {
                                    return;
                                }

                                attach(hinge);

                                if (hinge.Top != null) {
                                    var hingePartCubeGrid = hinge.TopGrid as MyCubeGrid;
                                    if (hingePartCubeGrid != null) {
                                        hingePartCubeGrid.ChangeColorAndSkin(hingePartCubeGrid.GetCubeBlock(hinge.Top.Position), colorMask, skin);
                                    }

                                    if (head.SlimBlock.IsFullIntegrity) {
                                        var topSlimBlock = hinge.Top.SlimBlock;
                                        var welderMountAmount = topSlimBlock.MaxIntegrity - topSlimBlock.Integrity;
                                        topSlimBlock.IncreaseMountLevel(welderMountAmount, hinge.OwnerId);
                                    }

                                    data.FlagAsSucceeded();
                                }
                            }
                        }
                    });
                } catch (Exception exception) {
                    Log.Error(exception);
                    Log.Error(exception.StackTrace);
                    data.FlagAsFailed();
                }
            }
        }
    }
}