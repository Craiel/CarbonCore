namespace CarbonCore.JSharpBridge.IO
{
    public class ByteOrder
    {
        public static ByteOrder LITTLE_ENDIAN;

        public static ByteOrder NativeOrder()
        {
            return Utils.Diagnostics.Internal.NotImplemented<ByteOrder>();
        }
    }
}
