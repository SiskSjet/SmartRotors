using System.ComponentModel;
using System.Xml.Serialization;
using ProtoBuf;

namespace Sisk.SmartRotors.Settings {
    [ProtoContract]
    public class SmartRotorSettings {
        public const int VERSION = 1;
        public const string GUID = "FDC80FBD-29C6-4219-8B59-5B624E8DDBB1";
        private const bool IS_HINGE_ATTACHED = false;

        [ProtoMember(1)]
        [XmlElement(Order = 1)]
        public int Version { get; set; } = VERSION;

        [ProtoMember(2)]
        [DefaultValue(IS_HINGE_ATTACHED)]
        public bool IsHingeAttached { get; set; } = IS_HINGE_ATTACHED;
    }
}