
using Silk.NET.Input;
using Silk.NET.Maths;

namespace Szeminarium
{
    internal class CameraDescriptor
    {
        public double DistanceToOrigin { get; private set; } = 1;

        public double AngleToZYPlane { get; private set; } = 0;

        public double AngleToZXPlane { get; private set; } = 0;

        const double DistanceScaleFactor = 1.1;

        const double AngleChangeStepSize = Math.PI / 180 * 5;

        const float MovingSpeed = 0.5f;

        private Vector3D<float> CameraPosition = new(0, 0, -5);

        /// <summary>
        /// Gets the position of the camera.
        /// </summary>
        public Vector3D<float> Position
        {
            get
            {
                return CameraPosition;
                    //GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane);
            }
        }

        /// <summary>
        /// Gets the up vector of the camera.
        /// </summary>
        public Vector3D<float> UpVector
        {
            get
            {
                return Vector3D.Normalize(GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane + Math.PI / 2));
            }
        }

        public Vector3D<float> Forward
        {
            get
            {
                return Vector3D.Normalize(GetPointFromAngles(1, AngleToZYPlane, AngleToZXPlane));
            }
        }

        public Vector3D<float> ForwardMovement
        {
            get
            {
                return Vector3D.Normalize(GetPointFromAngles(1, AngleToZYPlane, 0));
            }
        }

        public Vector3D<float> Right
        {
            get
            {
                return Vector3D.Normalize(Vector3D.Cross(Forward, UpVector));
            }
        }

        /// <summary>
        /// Gets the target point of the camera view.
        /// </summary>
        public Vector3D<float> Target
        {
            get
            {
                // For the moment the camera is always pointed at the origin.
                return Vector3D<float>.Zero;
            }
        }


        public void MoveForward()
        {
            CameraPosition += ForwardMovement * MovingSpeed;
        }

        public void MoveBackward()
        {
            CameraPosition -= ForwardMovement * MovingSpeed;
        }

        public void MoveLeft()
        {
            CameraPosition -= Right * MovingSpeed;
        }

        public void MoveRight()
        {
            CameraPosition += Right * MovingSpeed;
        }

        public void MoveUp()
        {
            CameraPosition += Vector3D<float>.UnitY * MovingSpeed;
        }

        public void MoveDown()
        {
            CameraPosition -= Vector3D<float>.UnitY * MovingSpeed;
        }

        public void RotateLeft()
        {
            AngleToZYPlane += AngleChangeStepSize;
        }

        public void RotateRight()
        {
            AngleToZYPlane -= AngleChangeStepSize;
        }

        public void RotateUp()
        {
            AngleToZXPlane += AngleChangeStepSize;
        }

        public void RotateDown()
        {
            AngleToZXPlane -= AngleChangeStepSize;
        }



        /*public void IncreaseZXAngle()
        {
            AngleToZXPlane += AngleChangeStepSize;
        }

        public void DecreaseZXAngle()
        {
            AngleToZXPlane -= AngleChangeStepSize;
        }

        public void IncreaseZYAngle()
        {
            AngleToZYPlane += AngleChangeStepSize;

        }

        public void DecreaseZYAngle()
        {
            AngleToZYPlane -= AngleChangeStepSize;
        }*/

        public void IncreaseDistance()
        {
            DistanceToOrigin = DistanceToOrigin * DistanceScaleFactor;
        }

        public void DecreaseDistance()
        {
            DistanceToOrigin = DistanceToOrigin / DistanceScaleFactor;
        }

        private static Vector3D<float> GetPointFromAngles(double distance, double yaw, double pitch)
        {
            var x = distance * Math.Cos(pitch) * Math.Sin(yaw);
            var z = distance * Math.Cos(pitch) * Math.Cos(yaw);
            var y = distance * Math.Sin(pitch);

            return new Vector3D<float>((float)x, (float)y, (float)z);
        }
    }
}
