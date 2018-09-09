using System;
using Sandbox.ModAPI;
using VRageMath;

namespace AutoMcD.SmartRotors.Extensions {
    public static class StatorExtensions {
        /// <summary>
        ///     Rotate rotor to given direction.
        /// </summary>
        /// <param name="stator">The stator that should rotate to direction.</param>
        /// <param name="targetDirection">The direction to which the rotor should rotate.</param>
        /// <param name="currentDirection">Current direction of the rotor.</param>
        /// <param name="maxVelocity">Max velocity with which the rotor rotates.</param>
        public static void PointRotorAtVector(this IMyMotorStator stator, Vector3D targetDirection, Vector3D currentDirection, float maxVelocity = 5f * MathHelper.RPMToRadiansPerSecond) {
            var cross = Vector3D.Cross(targetDirection, currentDirection);
            var dot = cross.Dot(stator.WorldMatrix.Up);

            stator.TargetVelocityRad = (float) (dot > 0 ? Math.Min(maxVelocity, dot) : Math.Max(maxVelocity * -1, dot));
        }
    }
}