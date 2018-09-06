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
            var angle = Vector3D.Cross(targetDirection, currentDirection);
            var dot = angle.Dot(stator.WorldMatrix.Up);

            if (dot > maxVelocity) {
                stator.TargetVelocityRad = maxVelocity;
            } else if (dot * -1 > maxVelocity) {
                stator.TargetVelocityRad = maxVelocity * -1;
            } else {
                stator.TargetVelocityRad = (float) dot;
            }
        }
    }
}