using AutoMcD.SmartRotors.Extensions;
using Sandbox.Common.ObjectBuilders;
using Sisk.Utils.Logging;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;

namespace AutoMcD.SmartRotors.Logic {
    // todo: set lower and upper limits. Lower: -22 | Upper: 202
    /// <summary>
    ///     Provides game logic for Smart Solar Hinges.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, LB_SMART_SOLAR_HINGE)]
    public sealed class SmartRotorSolarHinge : SmartRotorHinge {
        private const string LB_SMART_SOLAR_HINGE = "MA_SmartRotor_Solar_Hinge";

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

        /// <inheritdoc />
        public override void Init(MyObjectBuilder_EntityBase objectBuilder) {
            base.Init(objectBuilder);

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