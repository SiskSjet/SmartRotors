using Sandbox.Common.ObjectBuilders;
using Sisk.Utils.Logging;
using VRage.Game.Components;

namespace AutoMcD.SmartRotors.Logic {
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, LB_SMART_SOLAR_HINGE)]
    public class SmartRotorSolarHinge : SmartRotorHinge {
        private const string LB_SMART_SOLAR_HINGE = "MA_SmartRotor_Solar_Hinge";

        public SmartRotorSolarHinge() : base(nameof(SmartRotorSolarHinge)) {
            Log = Mod.Static.Log.ForScope<SmartRotorSolarHinge>();
        }

        private ILogger Log { get; }
    }
}