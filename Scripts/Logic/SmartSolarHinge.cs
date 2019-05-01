using Sandbox.Common.ObjectBuilders;
using Sisk.SmartRotors.Extensions;
using Sisk.Utils.Logging;
using VRage.Game.Components;
using VRage.ModAPI;

namespace Sisk.SmartRotors.Logic {
    // todo: set lower and upper limits. Lower: -22 | Upper: 202
    /// <summary>
    ///     Provides game logic for Smart Solar Hinges.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, Defs.SolarDefs.LB_SMART_SOLAR_HINGE, Defs.SolarDefs.LB_SMART_SOLAR_HINGE_B, Defs.SolarDefs.SB_SMART_SOLAR_HINGE_B)]
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
            Stator.PointRotorAtVector(sunDirection, Stator.Top.WorldMatrix.Left);
        }
    }
}