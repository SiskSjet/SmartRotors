using System;
using System.Linq;
using Sandbox.ModAPI;
using VRage.Game.ObjectBuilders;
using VRageMath;

namespace Sisk.SmartRotors {
    /// <summary>
    ///     Track the sun direction.
    /// </summary>
    public class SunTracker {
        private readonly Vector3 _baseSunDirection;
        private readonly bool _enabled;
        private readonly DateTime _offset = new DateTime(2081, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly Vector3 _sunRotationAxis;

        /// <summary>
        ///     Creates a new instance of <see cref="SunTracker" />.
        /// </summary>
        public SunTracker() {
            var environment = MyAPIGateway.Session.GetSector().Environment;
            var checkpoint = MyAPIGateway.Session.GetCheckpoint("null");

            Speed = 60f * MyAPIGateway.Session.SessionSettings.SunRotationIntervalMinutes;
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
        ///     The speed of sun rotation.
        /// </summary>
        public float Speed { get; }

        /// <summary>
        ///     Calculates the current sun direction.
        /// </summary>
        /// <returns>Returns the sun direction vector.</returns>
        public Vector3D CalculateSunDirection() {
            if (_enabled) {
                const float predict = 16f * 100 / 1000;
                var vector3 = Vector3D.Transform(_baseSunDirection, MatrixD.CreateFromAxisAngle(_sunRotationAxis, 6.283186f * ((ElapsedGameTime.TotalSeconds + predict) / Speed)));
                vector3.Normalize();

                return vector3;
            }

            return _baseSunDirection;
        }
    }
}