using System.Collections.Generic;
using System.Linq;
using AutoMcD.SmartRotors.Extensions;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using Sisk.Utils.Logging;
using VRage.Game.Components;
using VRage.ObjectBuilders;

// ReSharper disable InlineOutVariableDeclaration

namespace AutoMcD.SmartRotors.Logic {
    /// <summary>
    ///     Shared game logic for all SmartRotor hinges.
    /// </summary>
    public abstract class SmartRotorHinge : MyGameLogicComponent {
        private const string ADD_HEAD_ACTION_ID = "Add Top Part";
        private readonly string _debugName;
        private bool _isInitialized;

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
        ///     Indicates if the block which holds this game logic is just placed.
        /// </summary>
        public bool IsJustPlaced { get; private set; }

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
            using (Log.BeginMethod(nameof(Init))) {
                base.Init(objectBuilder);

                Stator = Entity as IMyMotorAdvancedStator;
                if (Stator == null) {
                    return;
                }

                IsJustPlaced = Stator.CubeGrid?.Physics != null;
            }
        }

        /// <inheritdoc />
        public override void OnAddedToScene() {
            using (Log.BeginMethod(nameof(OnAddedToScene))) {
                if (_isInitialized) {
                    return;
                }

                if (Stator.IsProjected()) {
                    return;
                }

                if (Stator.Top == null && Stator.CustomName == "Auto Placed") {
                    Stator.CustomName = Stator.DefinitionDisplayNameText;

                    var instantBuild = MyAPIGateway.Session.CreativeMode || MyAPIGateway.Session.HasCreativeRights && MyAPIGateway.Session.EnableCopyPaste;

                    List<IMyTerminalAction> defaultActions;
                    MyAPIGateway.TerminalControls.GetActions<IMyMotorAdvancedStator>(out defaultActions);

                    var attach = defaultActions.FirstOrDefault(x => x.Id == ADD_HEAD_ACTION_ID)?.Action;
                    if (attach == null) {
                        return;
                    }

                    attach(Stator);

                    if (instantBuild && Stator.Top != null) {
                        var slimBlock = Stator.Top.SlimBlock;
                        var welderMountAmount = slimBlock.MaxIntegrity - slimBlock.Integrity;
                        slimBlock.IncreaseMountLevel(welderMountAmount, Stator.OwnerId);

                        Log.Debug("Hinge Head placed.");
                    }
                }

                _isInitialized = true;
            }
        }
    }
}