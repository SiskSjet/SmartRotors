using System.Collections.Generic;
using System.Linq;
using AutoMcD.SmartRotors.Extensions;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using Sisk.Utils.Logging;
using Sisk.Utils.Profiler;
using VRage.Game.Components;
using VRage.ObjectBuilders;

// ReSharper disable InlineOutVariableDeclaration

namespace AutoMcD.SmartRotors.Logic {
    public abstract class SmartRotorHinge : MyGameLogicComponent {
        private const string ADD_HEAD_ACTION_ID = "Add Top Part";
        private readonly string _debugName;
        private bool _isInitialized;

        protected SmartRotorHinge(string debugName) {
            _debugName = debugName;
            Log = Mod.Static.Log.ForScope<SmartRotorHinge>();
        }

        public override string ComponentTypeDebugString => $"{_debugName} - Game Logic";

        public bool IsJustPlaced { get; private set; }
        private ILogger Log { get; }
        public IMyMotorAdvancedStator Stator { get; private set; }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder) {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartRotorHinge), nameof(Init)) : null) {
                using (Log.BeginMethod(nameof(Init))) {
                    base.Init(objectBuilder);

                    Stator = Entity as IMyMotorAdvancedStator;
                    if (Stator == null) {
                        return;
                    }

                    IsJustPlaced = Stator.CubeGrid?.Physics != null;
                }
            }
        }

        public override void OnAddedToScene() {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartRotorHinge), nameof(OnAddedToScene)) : null) {
                using (Log.BeginMethod(nameof(OnAddedToScene))) {
                    if (_isInitialized) {
                        return;
                    }

                    if (Stator.IsProjected()) {
                        return;
                    }

                    if (Stator.Top == null) {
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
}