using Silk.NET.Maths;

namespace Szeminarium
{
    internal class CameraDescriptor
    {
        public double DistanceToOrigin { get; private set; } = 0;
        public double AngleToZYPlane { get; private set; } = 0;  // Yaw (Horizontal Rotation)
        public double AngleToZXPlane { get; private set; } = 0;  // Pitch (Vertical Rotation)

        private const double DistanceScaleFactor = 1.1;
        private const double AngleChangeStepSize = Math.PI / 180 * 5;
        private const float MovementSpeed = 0.5f; // Speed of movement

        private Vector3D<float> position = new(0, 0, -5); // Camera starts 5 units away from origin

        /// Gets the position of the camera.
        public Vector3D<float> Position => position;

        /// Gets the forward direction of the camera.
        public Vector3D<float> Forward => Vector3D.Normalize(GetPointFromAngles(1, AngleToZYPlane, AngleToZXPlane));

        /// Gets the forward direction of the camera. horizontal movement
        public Vector3D<float> ForwardMovement => Vector3D.Normalize(GetPointFromAngles(1, AngleToZYPlane, 0));

        /// Gets the right direction of the camera.
        public Vector3D<float> Right => Vector3D.Normalize(Vector3D.Cross(Forward, UpVector));

        /// Gets the up vector of the camera.
        public Vector3D<float> UpVector => Vector3D<float>.UnitY;

        /// Gets the target point of the camera view.
        public Vector3D<float> Target => position + Forward;

        /// Moves the camera forward (W key) - stays in XZ plane.
        public void MoveForward()
        {
            position += ForwardMovement * MovementSpeed;
        }

        /// Moves the camera backward (S key) - stays in XZ plane.
        public void MoveBackward()
        {
            position -= ForwardMovement * MovementSpeed;
        }

        /// Moves the camera left (A key).
        public void MoveLeft()
        {
            position -= Right * MovementSpeed;
        }

        /// Moves the camera right (D key).
        public void MoveRight()
        {
            position += Right * MovementSpeed;
        }

        /// Moves the camera up (R key).
        public void MoveUp()
        {
            position += Vector3D<float>.UnitY * MovementSpeed;
        }

        /// Moves the camera down (F key).
        public void MoveDown()
        {
            position -= Vector3D<float>.UnitY * MovementSpeed;
        }

        /// Rotates camera left (left arrow key).
        public void RotateLeft()
        {
            AngleToZYPlane += AngleChangeStepSize;
        }

        /// Rotates camera right (right arrow key).
        public void RotateRight()
        {
            AngleToZYPlane -= AngleChangeStepSize;
        }

        /// Rotates camera up (up arrow key).
        public void RotateUp()
        {
            AngleToZXPlane += AngleChangeStepSize;
        }

        /// Rotates camera down (down arrow key).
        public void RotateDown()
        {
            AngleToZXPlane -= AngleChangeStepSize;
        }

        /// Zoom in (scroll up).
        public void IncreaseDistance()
        {
            DistanceToOrigin *= DistanceScaleFactor;
        }

        /// Zoom out (scroll down).
        public void DecreaseDistance()
        {
            DistanceToOrigin /= DistanceScaleFactor;
        }

        /// Calculates a point from given angles.
        private static Vector3D<float> GetPointFromAngles(double distance, double yaw, double pitch)
        {
            var x = distance * Math.Cos(pitch) * Math.Sin(yaw);
            var z = distance * Math.Cos(pitch) * Math.Cos(yaw);
            var y = distance * Math.Sin(pitch);

            return new Vector3D<float>((float)x, (float)y, (float)z);
        }
    }
}