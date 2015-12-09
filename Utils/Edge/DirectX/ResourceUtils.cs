namespace CarbonCore.Utils.Edge.DirectX
{
    using System.IO;

    using SharpDX.Direct3D11;

    public static class ResourceUtils
    {
        public static void ResourceToStream(DeviceContext context, Texture2D texture2D, ImageFileFormat format, Stream target)
        {
            Diagnostic.Internal.NotImplemented("This changed from 11 to 11.1 need to use something else");

            // http://stackoverflow.com/questions/9602102/loading-textures-with-sharpdx-in-metro
            /*Good news! Alexandre Mutel, the author of SharpDX, let me know that Microsoft removed the "helper methods" from Direct3D11.
             * It wasn't an omission by SharpDX. And he also pointed me in the right direction; to use WIC to load my textures. I found sample code here:
            http://crazylights.googlecode.com/svn/CLReach/win8/SDX_CLGC/clgc.cs
            I just needed two of the methods: LoadBitmap() and CreateTex2DFromBitmap(). 
             * I had to change the "R8G8B8A8_UNorm_SRgb" to "R8G8B8A8_UNorm", but ultimately I got it working.
             * And now my game now looks fabulous in Metro with all it's textures in place! :)*/
        }

        public static T ResourceFromMemory<T>(Device graphics, byte[] data, ImageLoadInformation? loadInformation = null)
        {
            return Diagnostic.Internal.NotImplemented<T>("same thing as ResourceToStream");
        }
    }
}
