using System.Collections.Generic;

namespace Sisk.SmartRotors {
    public class Defs {
        public Defs() {
            Solar = new SolarDefs();
        }

        public SolarDefs Solar { get; }

        public class SolarDefs {
            public const string LB_SMART_ROTOR_PART_1 = "MA_SmartRotor_Part1";
            public const string LB_SMART_SOLAR_BASE = "MA_SmartRotor_Solar_Base";
            public const string LB_SMART_SOLAR_BASE_TYPE_B = "MA_SmartRotor_Solar_Base_TypeB";
            public const string LB_SMART_SOLAR_HINGE = "MA_SmartRotor_Solar_Hinge";
            public const string LB_SMART_SOLAR_HINGE_TYPE_B = "MA_SmartRotor_Solar_Hinge_TypeB";
            public const string LB_SMART_SOLAR_PART_2 = "MA_SmartRotor_Solar_Part2";
            public const string LB_SMART_SOLAR_ROTOR_PART_2_TYPE_B = "MA_SmartRotor_Solar_Part2_TypeB";
            public const string SB_SMART_SOLAR_BASE_TYPE_B = "MA_SmartRotor_Solar_Base_TypeB_sm";
            public const string SB_SMART_SOLAR_HINGE_TYPE_B = "MA_SmartRotor_Solar_Hinge_TypeB_sm";
            public const string SB_SMART_SOLAR_PART_1 = "MA_SmartRotor_Part1_sm";
            public const string SB_SMART_SOLAR_PART_2_TYPE_B = "MA_SmartRotor_Solar_Part2_TypeB_sm";

            private readonly Dictionary<string, string> _baseToHinge = new Dictionary<string, string> {
                { LB_SMART_SOLAR_BASE, LB_SMART_SOLAR_HINGE },
                { LB_SMART_SOLAR_BASE_TYPE_B, LB_SMART_SOLAR_HINGE_TYPE_B },
                { SB_SMART_SOLAR_BASE_TYPE_B, SB_SMART_SOLAR_HINGE_TYPE_B }
            };

            private readonly Dictionary<string, string> _rotorToPart = new Dictionary<string, string> {
                { LB_SMART_SOLAR_BASE, LB_SMART_ROTOR_PART_1 },
                { LB_SMART_SOLAR_HINGE, LB_SMART_SOLAR_PART_2 },
                { LB_SMART_SOLAR_BASE_TYPE_B, LB_SMART_ROTOR_PART_1 },
                { LB_SMART_SOLAR_HINGE_TYPE_B, LB_SMART_SOLAR_ROTOR_PART_2_TYPE_B },
                { SB_SMART_SOLAR_BASE_TYPE_B, SB_SMART_SOLAR_PART_1 },
                { SB_SMART_SOLAR_HINGE_TYPE_B, SB_SMART_SOLAR_PART_2_TYPE_B }
            };

            public readonly IReadOnlyCollection<string> BaseIds = new HashSet<string> { LB_SMART_SOLAR_BASE, LB_SMART_SOLAR_BASE_TYPE_B, SB_SMART_SOLAR_BASE_TYPE_B };
            public readonly IReadOnlyCollection<string> HingeIds = new HashSet<string> { LB_SMART_SOLAR_HINGE, LB_SMART_SOLAR_HINGE_TYPE_B, SB_SMART_SOLAR_HINGE_TYPE_B };

            public IReadOnlyDictionary<string, string> BaseToHinge => _baseToHinge;
            public IReadOnlyDictionary<string, string> RotorToPart => _rotorToPart;
        }
    }
}