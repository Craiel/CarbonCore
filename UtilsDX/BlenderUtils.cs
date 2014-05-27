namespace CarbonCore.UtilsDX
{
    using SharpDX;

    public static class BlenderUtils
    {
        public static readonly Matrix BlenderCoordinateCompensation = Matrix.Scaling(1, 1, -1);

        // Flip the z by -1 to compensate for blender coordinate system
        public static Vector3 AdjustCoordinateSystem(Vector3 source)
        {
            Vector4 adjusted = Vector3.Transform(source, BlenderCoordinateCompensation);
            return new Vector3(adjusted.X, adjusted.Y, adjusted.Z);
        }
    }
}
