﻿using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using Sisk.SmartRotors.Extensions;
using Sisk.Utils.Logging;
using VRage.Game.Components;
using VRage.ModAPI;

namespace Sisk.SmartRotors.Logic {
    /// <summary>
    ///     Shared game logic for all SmartRotor hinges.
    /// </summary>
    public abstract class SmartRotorHinge : MyGameLogicComponent {
        public const string AUTO_PLACED_TAG = "AUTO PLACED: YOU SHOULD NEVER SEE THIS";
        private const string ADD_HEAD_ACTION_ID = "Add Top Part";
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
        public override void OnAddedToScene() {
            using (Log.BeginMethod(nameof(OnAddedToScene))) {
                Stator = Entity as IMyMotorAdvancedStator;
                if (Stator == null) {
                    return;
                }

                if (Stator.IsProjected()) {
                    return;
                }

                NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            }
        }

        public override void UpdateOnceBeforeFrame() {
            NeedsUpdate &= ~MyEntityUpdateEnum.NONE;

            if (Stator.Top == null && Stator.CustomName == AUTO_PLACED_TAG) {
                Stator.CustomName = Stator.DefinitionDisplayNameText;

                List<IMyTerminalAction> defaultActions;
                MyAPIGateway.TerminalControls.GetActions<IMyMotorAdvancedStator>(out defaultActions);

                var attach = defaultActions.FirstOrDefault(x => x.Id == ADD_HEAD_ACTION_ID)?.Action;
                if (attach == null) {
                    return;
                }

                attach(Stator);

                if (Mod.Static.Network == null || Mod.Static.Network.IsServer) {
                    // todo: fix instant build in multiplayer.
                    if (MyAPIGateway.Session.CreativeMode || MyAPIGateway.Session.HasCreativeRights && MyAPIGateway.Session.EnableCopyPaste) {
                        InstantBuild();
                    }
                }
            }
        }

        /// <summary>
        ///     Fake creative mode.
        /// </summary>
        private void InstantBuild() {
            if (Stator.Top != null) {
                var slimBlock = Stator.Top.SlimBlock;
                var welderMountAmount = slimBlock.MaxIntegrity - slimBlock.Integrity;
                slimBlock.IncreaseMountLevel(welderMountAmount, Stator.OwnerId);
            }
        }
    }
}