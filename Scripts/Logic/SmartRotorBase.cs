using AutoMcD.SmartRotors.Extensions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sisk.Utils.Logging;
using Sisk.Utils.Profiler;
using VRage.Game.Components;
using VRage.ObjectBuilders;

// ReSharper disable UsePatternMatching

namespace AutoMcD.SmartRotors.Logic {
    public abstract class SmartRotorBase : MyGameLogicComponent {
        private readonly string _debugName;
        private bool _lastAttachedState;

        protected SmartRotorBase(string debugName) {
            _debugName = debugName;
            Log = Mod.Static.Log.ForScope<SmartRotorBase>();
        }

        public override string ComponentTypeDebugString => $"{_debugName} - Game Logic";

        public bool IsJustPlaced { get; private set; }
        private ILogger Log { get; }
        public IMyMotorAdvancedStator Stator { get; private set; }

        public override void Close() {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartRotorBase), nameof(Close)) : null) {
                using (Log.BeginMethod(nameof(Close))) {
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

        public override void Init(MyObjectBuilder_EntityBase objectBuilder) {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartRotorBase), nameof(Init)) : null) {
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
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartRotorBase), nameof(OnAddedToScene)) : null) {
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
        }

        /// <summary>
        ///     Place the matching smart rotor on top of this rotor.
        /// </summary>
        protected abstract void PlaceSmartRotorHinge();

        private void OnAttachedEntityChanged(IMyMotorBase @base) {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartRotorBase), nameof(OnAttachedEntityChanged)) : null) {
                using (Log.BeginMethod(nameof(OnAttachedEntityChanged))) {
                    // hack: until IMyMotorStator.AttachedEntityChanged event is fixed.
                    _lastAttachedState = @base.Top != null;

                    if (@base.Top != null) {
                        PlaceSmartRotorHinge();
                    }
                }
            }
        }

        // hack: until IMyMotorStator.AttachedEntityChanged event is fixed.
        private void OnHierarchyUpdated(MyCubeGrid obj) {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SmartRotorBase), nameof(OnHierarchyUpdated)) : null) {
                using (Log.BeginMethod(nameof(OnHierarchyUpdated))) {
                    if (_lastAttachedState && Stator.TopGrid == null) {
                        OnAttachedEntityChanged(Stator);
                    } else if (!_lastAttachedState && Stator.TopGrid != null) {
                        OnAttachedEntityChanged(Stator);
                    }
                }
            }
        }
    }
}