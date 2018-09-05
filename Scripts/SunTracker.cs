using System;
using System.Linq;
using Sandbox.ModAPI;
using Sisk.Utils.Profiler;
using VRage.Game.ObjectBuilders;
using VRageMath;

// ReSharper disable UsePatternMatching
// ReSharper disable InlineOutVariableDeclaration

namespace AutoMcD.SmartRotors {
    /// <summary>
    ///     Track the sun direction.
    /// </summary>
    public class SunTracker {
        private readonly Vector3 _baseSunDirection;
        private readonly bool _enabled;
        private readonly DateTime _offset = new DateTime(2081, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly float _speed;
        private readonly Vector3 _sunRotationAxis;

        /// <summary>
        ///     Creates a new instance of <see cref="SunTracker" />.
        /// </summary>
        public SunTracker() {
            var environment = MyAPIGateway.Session.GetSector().Environment;
            var checkpoint = MyAPIGateway.Session.GetCheckpoint("null");

            _speed = 60f * MyAPIGateway.Session.SessionSettings.SunRotationIntervalMinutes;
            _enabled = MyAPIGateway.Session.SessionSettings.EnableSunRotation;

            Vector3 sunDirectionNormalized;
            Vector3.CreateFromAzimuthAndElevation(environment.SunAzimuth, environment.SunElevation, out sunDirectionNormalized);

            var weatherComponent = checkpoint.SessionComponents.OfType<MyObjectBuilder_SectorWeatherComponent>().FirstOrDefault();
            if (weatherComponent != null && !weatherComponent.BaseSunDirection.IsZero) {
                _baseSunDirection = weatherComponent.BaseSunDirection;
            }

            var cross = Vector3.Cross(Math.Abs(Vector3.Dot(sunDirectionNormalized, Vector3.Up)) > 0.95f ? Vector3.Cross(sunDirectionNormalized, Vector3.Left) : Vector3.Cross(sunDirectionNormalized, Vector3.Up), sunDirectionNormalized);
            cross.Normalize();
            _sunRotationAxis = cross;
        }

        /// <summary>
        ///     The elapsed game time.
        /// </summary>
        private TimeSpan ElapsedGameTime => MyAPIGateway.Session.GameDateTime - _offset;

        /// <summary>
        ///     Calculates the current sun direction.
        /// </summary>
        /// <returns>Returns the sun direction vector.</returns>
        public Vector3 CalculateSunDirection() {
            using (Mod.PROFILE ? Profiler.Measure(nameof(SunTracker), nameof(CalculateSunDirection)) : null) {
                if (_enabled) {
                    var vector3 = Vector3.Transform(_baseSunDirection, Matrix.CreateFromAxisAngle(_sunRotationAxis, 6.283186f * ((float) ElapsedGameTime.TotalSeconds / _speed)));
                    vector3.Normalize();

                    return vector3;
                }

                return _baseSunDirection;
            }
        }
    }
}