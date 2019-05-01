using System.Collections.Generic;

namespace Sisk.SmartRotors {
    public class Defs {
        public Defs() {
            Solar = new SolarDefs();
        }

        public SolarDefs Solar { get; }

        public class SolarDefs {
            public const string LB_SMART_SOLAR_HINGE = "MA_SmartRotor_Solar_Hinge";
            public const string LB_SMART_SOLAR_HINGE_B = "MA_SmartRotor_Solar_Hinge_TypeB";
            public const string LB_SMART_SOLAR_ROTOR = "MA_SmartRotor_Solar_Base";
            public const string LB_SMART_SOLAR_ROTOR_B = "MA_SmartRotor_Solar_Base_TypeB";
            public const string SB_SMART_SOLAR_HINGE_B = "MA_SmartRotor_Solar_Hinge_TypeB_sm";
            public const string SB_SMART_SOLAR_ROTOR_B = "MA_SmartRotor_Solar_Base_TypeB_sm";

            private readonly Dictionary<string, string> _baseToHinge = new Dictionary<string, string> {
                { LB_SMART_SOLAR_ROTOR, LB_SMART_SOLAR_HINGE },
                { LB_SMART_SOLAR_ROTOR_B, LB_SMART_SOLAR_HINGE_B },
                { SB_SMART_SOLAR_ROTOR_B, SB_SMART_SOLAR_HINGE_B }
            };

            public readonly IReadOnlyCollection<string> BaseIds = new HashSet<string> { LB_SMART_SOLAR_ROTOR, LB_SMART_SOLAR_ROTOR_B, SB_SMART_SOLAR_ROTOR_B };
            public readonly IReadOnlyCollection<string> HingeIds = new HashSet<string> { LB_SMART_SOLAR_HINGE, LB_SMART_SOLAR_HINGE_B, SB_SMART_SOLAR_HINGE_B };

            public IReadOnlyDictionary<string, string> BaseToHinge => _baseToHinge;
        }
    }
}