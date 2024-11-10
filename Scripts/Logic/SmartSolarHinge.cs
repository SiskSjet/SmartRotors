using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sisk.SmartRotors.Extensions;
using Sisk.Utils.Logging;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;

namespace Sisk.SmartRotors.Logic {

    // todo: set lower and upper limits. Lower: -22 | Upper: 202
    // todo: set lower and upper limits. Lower: -42 | Upper: 222 Type B
    /// <summary>
    ///     Provides game logic for Smart Solar Hinges.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, Defs.SolarDefs.LB_SMART_SOLAR_HINGE, Defs.SolarDefs.LB_SMART_SOLAR_HINGE_TYPE_B, Defs.SolarDefs.SB_SMART_SOLAR_HINGE_TYPE_B)]
    public sealed class SmartRotorSolarHinge : SmartRotorHinge {

        /// <summary>
        ///     Initializes a new instance of <see cref="SmartRotorSolarHinge" />.
        /// </summary>
        public SmartRotorSolarHinge() : base(nameof(SmartRotorSolarHinge)) {
            Log = Mod.Static.Log.ForScope<SmartRotorSolarHinge>();
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

            Stator.PointRotorAtVector(sunDirection, Stator.Top.WorldMatrix.Left, 3 * MathHelper.RPMToRadiansPerSecond);
        }
    }
}