using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;

namespace Sisk.SmartRotors {
    /// <summary>
    ///     Class to modify terminal controls and actions for blocks from this mod.
    /// </summary>
    public class Controls {
        private readonly HashSet<string> _hiddenActions = new HashSet<string> { "Add Small Top Part", "Reverse", "IncreaseVelocity", "DecreaseVelocity", "ResetVelocity" };
        private readonly HashSet<string> _hiddenControls = new HashSet<string> { "Add Small Top Part", "Velocity" };

        /// <summary>
        ///     Indicates if terminal controls are initialized.
        /// </summary>
        public bool AreTerminalControlsInitialized { get; private set; }

        /// <summary>
        ///     Check if the given block is one of the solar stator blocks.
        /// </summary>
        /// <param name="block">The block that will be check if it's one of the solar stator blocks.</param>
        /// <returns>Return true if given block is one of the solar block stators.</returns>
        private static bool IsSolarStator(IMyTerminalBlock block) {
            return block != null && (Mod.Static.Defs.Solar.BaseIds.Contains(block.BlockDefinition.SubtypeId) || Mod.Static.Defs.Solar.HingeIds.Contains(block.BlockDefinition.SubtypeId));
        }

        /// <summary>
        ///     Initialize the controls.
        /// </summary>
        public void InitializeControls() {
            ModifyVanillaControls();
            ModifyVanillaActions();
            AreTerminalControlsInitialized = true;
        }

        /// <summary>
        ///     Modify vanilla terminal actions to hide some actions for solar stator blocks.
        /// </summary>
        private void ModifyVanillaActions() {
            List<IMyTerminalAction> actions;
            MyAPIGateway.TerminalControls.GetActions<IMyMotorAdvancedStator>(out actions);

            foreach (var action in actions) {
                if (_hiddenActions.Contains(action.Id)) {
                    var original = action.Enabled;
                    action.Enabled = block => !IsSolarStator(block) && original.Invoke(block);
                }
            }
        }

        /// <summary>
        ///     Modify the vanilla controls to hide some controls for solar stator blocks.
        /// </summary>
        private void ModifyVanillaControls() {
            List<IMyTerminalControl> controls;
            MyAPIGateway.TerminalControls.GetControls<IMyMotorAdvancedStator>(out controls);

            foreach (var control in controls) {
                if (_hiddenControls.Contains(control.Id)) {
                    var visible = control.Visible;
                    var enabled = control.Enabled;
                    control.Visible = block => !IsSolarStator(block) && visible.Invoke(block);
                    control.Enabled = block => !IsSolarStator(block) && enabled.Invoke(block);
                }
            }
        }
    }
}