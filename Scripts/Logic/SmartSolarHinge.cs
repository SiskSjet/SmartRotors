using Sandbox.Common.ObjectBuilders;
using Sisk.Utils.Logging;
using VRage.Game.Components;

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
    }
}