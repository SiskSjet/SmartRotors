using AutoMcD.SmartRotors.Data;
using AutoMcD.SmartRotors.Extensions;
using ParallelTasks;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sisk.Utils.Logging;
using VRage.Game.Components;
using VRage.ObjectBuilders;

// ReSharper disable UsePatternMatching

namespace AutoMcD.SmartRotors.Logic {
    /// <summary>
    ///     Shared game logic for all SmartRotor bases.
    /// </summary>
    public abstract class SmartRotorBase : MyGameLogicComponent {
        private readonly string _debugName;
        private bool _lastAttachedState;

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
        public override void Close() {
            using (Log.BeginMethod(nameof(Close))) {
                // todo: check if it is enough if it is executed on the server.
                if (Mod.Static.Network == null || Mod.Static.Network.IsServer) {
                    // bug: IMyMotorStator.AttachedEntityChanged throws "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type.".
                    //Stator.AttachedEntityChanged -= OnAttachedEntityChanged;

                    // hack: until IMyMotorStator.AttachedEntityChanged event is fixed.
                    var cubeGrid = Stator.CubeGrid as MyCubeGrid;
                    if (cubeGrid != null) {
                        cubeGrid.OnHierarchyUpdated -= OnHierarchyUpdated;
                    }
                }
            }
        }

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
                if (Stator.IsProjected()) {
                    return;
                }

                _lastAttachedState = Stator.Top != null;

                // bug: IMyMotorStator.AttachedEntityChanged throws "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type.".
                //Stator.AttachedEntityChanged += OnAttachedEntityChanged;

                // hack: until IMyMotorStator.AttachedEntityChanged event is fixed.
                var cubeGrid = Stator.CubeGrid as MyCubeGrid;
                if (cubeGrid != null) {
                    cubeGrid.OnHierarchyUpdated += OnHierarchyUpdated;
                }
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
                // hack: until IMyMotorStator.AttachedEntityChanged event is fixed.
                _lastAttachedState = @base.Top != null;

                MyAPIGateway.Parallel.Start(PlaceSmartHinge, PlaceSmartHingeCompleted, new PlaceSmartHingeData(Stator.Top));
            }
        }

        // hack: until IMyMotorStator.AttachedEntityChanged event is fixed.
        /// <summary>
        ///     Used to check if <see cref="IMyMotorStator.Top" /> is changed, because of a the
        ///     <see cref="IMyMotorStator.AttachedEntityChanged" /> event bug.
        /// </summary>
        /// <param name="cubeGrid">The cube grid on which the hierarchy updated.</param>
        private void OnHierarchyUpdated(MyCubeGrid cubeGrid) {
            using (Log.BeginMethod(nameof(OnHierarchyUpdated))) {
                if (_lastAttachedState && Stator.TopGrid == null) {
                    OnAttachedEntityChanged(Stator);
                } else if (!_lastAttachedState && Stator.TopGrid != null) {
                    OnAttachedEntityChanged(Stator);
                }
            }
        }

        /// <summary>
        ///     Get called after <see cref="PlaceSmartHinge" /> task is completed.
        /// </summary>
        /// <param name="workData">The work data used in this method.</param>
        private void PlaceSmartHingeCompleted(WorkData workData) {
            using (Log.BeginMethod(nameof(PlaceSmartHingeCompleted))) {
                Log.Debug("Hinge placed");
            }
        }
    }
}