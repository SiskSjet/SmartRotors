using Sandbox.ModAPI;
using Sisk.Utils.Logging;
using VRage.Game.Components;
using VRage.ObjectBuilders;

namespace Sisk.SmartRotors.Logic {
    /// <summary>
    ///     Shared game logic for all SmartRotor hinges.
    /// </summary>
    public abstract class SmartRotorHinge : MyGameLogicComponent {
        private readonly string _debugName;

        /// <summary>
        ///     Initializes a new instance of the abstract game logic component for SmartRotor hinges.
        /// </summary>
        /// <param name="debugName">A debug string used to generate <see cref="ComponentTypeDebugString" />.</param>
        protected SmartRotorHinge(string debugName) {
            _debugName = debugName;
            Log = Mod.Static.Log.ForScope<SmartRotorHinge>();
        }

        /// <inheritdoc />
        public override string ComponentTypeDebugString => $"{_debugName} - Game Logic";

        /// <summary>
        ///     Logger used for logging.
        /// </summary>
        private ILogger Log { get; }

        /// <summary>
        ///     The entity which holds this game logic component.
        /// </summary>
        public IMyMotorAdvancedStator Stator { get; private set; }

        /// <inheritdoc />
        public override void Init(MyObjectBuilder_EntityBase objectBuilder) {
            base.Init(objectBuilder);

            Stator = Entity as IMyMotorAdvancedStator;
        }
    }
}