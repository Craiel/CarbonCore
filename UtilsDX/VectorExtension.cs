namespace CarbonCore.UtilsDX
{
    using System.Collections.Generic;

    using SharpDX;

    public static class VectorExtension
    {
        static VectorExtension()
        {
            Vector3Identity = new Vector3(1.0f);
            Vector4Identity = new Vector4(1.0f);
        }

        public static Vector3 Vector3Identity { get; private set; }
        public static Vector4 Vector4Identity { get; private set; }

        public static Vector3 Invert(this Vector3 vector)
        {
            return new Vector3(-vector.X, -vector.Y, -vector.Z);
        }

        public static Vector3 XYZ(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        // These are critical so we don't check for count before, inconsistency will cause crash
        public static Vector2 Vector2FromList(IList<float> floats)
        {
            return new Vector2(floats[0], floats[1]);
        }

        public static IList<float> ToList(this Vector2 vector)
        {
            return new List<float> { vector.X, vector.Y };
        }

        public static Vector3 Vector3FromList(IList<float> floats)
        {
            return new Vector3(floats[0], floats[1], floats[2]);
        }

        public static IList<float> ToList(this Vector3 vector)
        {
            return new List<float> { vector.X, vector.Y, vector.Z };
        }

        public static Vector4 Vector4FromList(IList<float> floats)
        {
            return new Vector4(floats[0], floats[1], floats[2], floats[3]);
        }

        public static IList<float> ToList(this Vector4 vector)
        {
            return new List<float> { vector.X, vector.Y, vector.Z, vector.W };
        }
    }
}
