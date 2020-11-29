using System;
using Sandbox.ModAPI;
using VRage.Game;
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
            Speed = 60f * MyAPIGateway.Session.SessionSettings.SunRotationIntervalMinutes;
            _enabled = MyAPIGateway.Session.SessionSettings.EnableSunRotation;

            _baseSunDirection = MySunProperties.Default.BaseSunDirectionNormalized;
            _sunRotationAxis = MySunProperties.Default.SunRotationAxis;
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
                var vector3 = Vector3D.Transform(_baseSunDirection, MatrixD.CreateFromAxisAngle(_sunRotationAxis, 6.2831859588623 * ((ElapsedGameTime.TotalSeconds + predict) / Speed)));
                vector3.Normalize();

                return vector3;
            }

            return _baseSunDirection;
        }
    }
}