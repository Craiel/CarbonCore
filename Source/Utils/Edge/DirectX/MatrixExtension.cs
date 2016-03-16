namespace CarbonCore.Utils.Edge.DirectX
{
    using SharpDX;

    public static class MatrixExtension
    {
        public static Matrix GetLocalMatrix(Vector3 scale, Quaternion rotation, Vector3 position)
        {
            Matrix scaled = Matrix.Scaling(scale);
            Matrix rotated = Matrix.RotationQuaternion(rotation);
            Matrix translated = Matrix.Translation(new Vector3(position.X, position.Y, position.Z));
            
            return scaled * rotated * translated;
        }
    }
}
