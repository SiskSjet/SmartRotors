using Sandbox.Common.ObjectBuilders;
using Sisk.Utils.Logging;
using VRage.Game.Components;

namespace AutoMcD.SmartRotors.Logic {
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorAdvancedStator), false, LB_SMART_SOLAR_ROTOR)]
    public class SmartSolarRotorBase : SmartRotorBase {
        private const string LB_SMART_SOLAR_ROTOR = "MA_SmartRotor_Solar_Base";

        public SmartSolarRotorBase() : base(nameof(SmartSolarRotorBase)) {
            Log = Mod.Static.Log.ForScope<SmartSolarRotorBase>();
        }

        private ILogger Log { get; }

        protected override void PlaceSmartRotorHinge() { }
    }
}