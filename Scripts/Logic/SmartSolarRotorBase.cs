using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sisk.Utils.Logging;
using Sisk.Utils.Profiler;
using VRage;
using VRage.Game;
using VRage.Game.Components;

namespace AutoMcD.SmartRotors.Logic {
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, LB_SMART_SOLAR_ROTOR)]
    public class SmartSolarRotorBase : SmartRotorBase {
        private const string LB_SMART_SOLAR_ROTOR = "MA_SmartRotor_Solar_Base";

        public SmartSolarRotorBase() : base(nameof(SmartSolarRotorBase)) {
            Log = Mod.Static.Log.ForScope<SmartSolarRotorBase>();
        }

        private ILogger Log { get; }

        protected override void PlaceSmartRotorHinge() {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartSolarRotorBase), nameof(PlaceSmartRotorHinge)) : null) {
                using (Log.BeginMethod(nameof(PlaceSmartRotorHinge))) {
                    Log.Debug("Try to place smart solar hinge.");
                    var head = Stator.Top;
                    if (head == null) {
                        return;
                    }

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
                        (cubeGrid as MyCubeGrid)?.PasteBlocksToGrid(gridsToMerge, 0, false, false);

                        //MyTransparentGeometry.AddLineBillboard(MyStringId.GetOrCompute("Square"), new Color(Color.WhiteSmoke, 0.8f).ToVector4(), headPosition + up * 1.5, up, 2.5f, .05f);
                        //MyTransparentGeometry.AddPointBillboard(MyStringId.GetOrCompute("WhiteDot"), new Color(Color.CornflowerBlue, 0.8f).ToVector4(), headPosition + up * 1.5, .5f, 0);
                        //MyTransparentGeometry.AddBillboardOriented(MyStringId.GetOrCompute("Square"), new Color(Color.CornflowerBlue, 0.8f).ToVector4(), origin, left, up, 1f);

                        Log.Debug("Hinge placed");
                    } catch (Exception exception) {
                        Log.Error(exception);
                    }
                }
            }
        }
    }
}