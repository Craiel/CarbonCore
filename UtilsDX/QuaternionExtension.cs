namespace CarbonCore.UtilsDX
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat;

    using SharpDX;

    public static class QuaternionExtension
    {
        /*public static Quaternion RotateTo(Vector3 source, Vector3 target)
        {
            Vector3 axis = Vector3.Cross(source, target);
            axis.Normalize();
            var angle = (float)Math.Acos(Vector3.Dot(source, target) / source.Length() / target.Length());
            return Quaternion.RotationAxis(axis, angle);
        }*/

        public static Quaternion RotateTo(Vector3 source, Vector3 dest, Vector3 up)
        {
            float dot = Vector3.Dot(source, dest);

            if (Math.Abs(dot - (-1.0f)) < float.Epsilon)
            {
                return new Quaternion(up, MathExtension.DegreesToRadians(180.0f));
            }

            if (Math.Abs(dot - 1.0f) < float.Epsilon)
            {
                return Quaternion.Identity;
            }

            var angle = (float)Math.Acos(dot);
            Vector3 axis = Vector3.Cross(source, dest);
            axis = Vector3.Normalize(axis);
            return Quaternion.RotationAxis(axis, angle);
        }

        public static Quaternion QuaterionFromList(IList<float> floats)
        {
            return new Quaternion(floats[0], floats[1], floats[2], floats[3]);
        }

        public static Quaternion RotationYawPitchRoll(Vector3 vector)
        {
            return Quaternion.RotationYawPitchRoll(vector.X, vector.Y, vector.Z);
        }

        public static IList<float> ToList(this Quaternion quaternion)
        {
            return new List<float> { quaternion.X, quaternion.Y, quaternion.Z, quaternion.W };
        }
    }
}
