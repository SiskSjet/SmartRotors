using ParallelTasks;
using Sandbox.ModAPI;
using Sisk.SmartRotors.Data;
using Sisk.SmartRotors.Extensions;
using Sisk.Utils.Logging;
using VRage.Game.Components;

namespace Sisk.SmartRotors.Logic {
    /// <summary>
    ///     Shared game logic for all SmartRotor bases.
    /// </summary>
    public abstract class SmartRotorBase : MyGameLogicComponent {
        private readonly string _debugName;

        /// <summary>
        ///     Initializes a new instance of the abstract game logic component for SmartRotor bases.
        /// </summary>
        /// <param name="debugName">A debug string used to generate <see cref="ComponentTypeDebugString" />.</param>
        protected SmartRotorBase(string debugName) {
            _debugName = debugName;
            Log = Mod.Static.Log.ForScope<SmartRotorBase>();
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
        public override void Close() {
            using (Log.BeginMethod(nameof(Close))) {
                // todo: check if it is enough if it is executed on the server.
                if (Mod.Static.Network == null || Mod.Static.Network.IsServer) {
                    Stator.AttachedEntityChanged -= OnAttachedEntityChanged;
                }
            }
        }

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

                // todo: check if it is enough if it is executed on the server.
                Stator.AttachedEntityChanged += OnAttachedEntityChanged;
            }
        }

        /// <summary>
        ///     Place the matching smart rotor on top of this rotor. This is called in an separate thread.
        /// </summary>
        /// <param name="workData">The work data used in this method.</param>
        protected abstract void PlaceSmartHinge(WorkData workData);

        /// <summary>
        ///     Called if <see cref="IMyMotorStator.Top" /> changed.
        /// </summary>
        /// <param name="base">The base on which the top is changed.</param>
        private void OnAttachedEntityChanged(IMyMechanicalConnectionBlock @base) {
            using (Log.BeginMethod(nameof(OnAttachedEntityChanged))) {
                if (@base.Top != null) {
                    MyAPIGateway.Parallel.Start(PlaceSmartHinge, PlaceSmartHingeCompleted, new PlaceSmartHingeData(Stator.Top));
                }
            }
        }

        /// <summary>
        ///     Get called after <see cref="PlaceSmartHinge" /> task is completed.
        /// </summary>
        /// <param name="workData">The work data used in this method.</param>
        private void PlaceSmartHingeCompleted(WorkData workData) {
            using (Log.BeginMethod(nameof(PlaceSmartHingeCompleted))) {
                var data = workData as PlaceSmartHingeData;

                if (data?.Head == null) {
                    return;
                }

                switch (data.Result) {
                    case PlaceSmartHingeData.DataResult.Running:
                        break;
                    case PlaceSmartHingeData.DataResult.Success:
                        Log.Debug("Hinge placed");
                        break;
                    case PlaceSmartHingeData.DataResult.Failed:
                        Log.Error("Something went wrong when trying to place hinge.");
                        break;
                }
            }
        }
    }
}