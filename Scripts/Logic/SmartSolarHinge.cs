using Sandbox.Common.ObjectBuilders;
using Sisk.Utils.Logging;
using VRage.Game;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace AutoMcD.SmartRotors.Logic {
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

            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
        }

        /// <inheritdoc />
        public override void UpdateAfterSimulation() {
            var sunDirection = Mod.Static.SunTracker.CalculateSunDirection();

            var matrix = Stator.WorldMatrix;
            var start = Stator.GetPosition() + matrix.Forward * 2;
            MyTransparentGeometry.AddLineBillboard(MyStringId.GetOrCompute("Square"), new Color(Color.WhiteSmoke, 0.8f).ToVector4(), start, sunDirection, 2.5f, .05f);
            MyTransparentGeometry.AddPointBillboard(MyStringId.GetOrCompute("WhiteDot"), new Color(Color.CornflowerBlue, 0.8f).ToVector4(), start, .5f, 0);
        }
    }
}