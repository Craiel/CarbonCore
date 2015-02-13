namespace CarbonCore.JSharpBridge.GL
{
    using System;

    using CarbonCore.JSharpBridge.IO;

    [Serializable]
    public class BridgeGLException : Exception
    {
    }

    public static class GL11
    {
        public static int GL_TEXTURE_2D;
        public static int GL_SMOOTH;
        public static int GL_DEPTH_TEST;

        public static int GL_LEQUAL;

        public static int GL_ALPHA_TEST;

        public static int GL_GREATER;

        public static int GL_BACK;

        public static int GL_PROJECTION;

        public static int GL_MODELVIEW;

        public static int GL_COLOR_BUFFER_BIT;

        public static int GL_DEPTH_BUFFER_BIT;

        public static int GL_LIGHTING;

        public static int GL_FOG;

        public static int GL_COLOR_MATERIAL;

        public static int GL_BLEND;

        public static int GL_VERSION;

        public static int GL_VENDOR;

        public static int GL_PROXY_TEXTURE_2D;

        public static int GL_RGBA;

        public static int GL_UNSIGNED_BYTE;

        public static int GL_TEXTURE_WIDTH;

        public static int GL_RENDERER;

        public static int GL_TEXTURE;

        public static int GL_TEXTURE_MIN_FILTER;

        public static int GL_LINEAR;

        public static int GL_TEXTURE_MAG_FILTER;

        public static int GL_TEXTURE_WRAP_S;

        public static int GL_TEXTURE_WRAP_T;

        public static int GL_CLAMP;

        public static int GL_CULL_FACE;

        public static int GL_SRC_ALPHA;

        public static int GL_ONE_MINUS_SRC_ALPHA;

        public static int GL_FLAT;

        public static int GL_ONE;

        public static int GL_FOG_COLOR;

        public static int GL_FOG_MODE;

        public static int GL_FOG_START;

        public static int GL_FOG_END;

        public static int GL_EXP;

        public static int GL_FOG_DENSITY;

        public static int GL_FRONT;

        public static int GL_AMBIENT;

        public static int GL_REPEAT;

        public static int GL_NEAREST;

        public static int GL_PACK_ALIGNMENT;

        public static int GL_UNPACK_ALIGNMENT;

        public static int GL_FLOAT;

        public static int GL_TEXTURE_COORD_ARRAY;

        public static int GL_SHORT;

        public static int GL_VERTEX_ARRAY;

        public static int GL_TRIANGLES;

        public static int GL_NORMAL_ARRAY;

        public static int GL_COLOR_ARRAY;

        public static int GL_ONE_MINUS_DST_COLOR;

        public static int GL_ONE_MINUS_SRC_COLOR;

        public static int GL_ZERO;

        public static int GL_AMBIENT_AND_DIFFUSE;

        public static int GL_LIGHT0;

        public static int GL_LIGHT1;

        public static int GL_FRONT_AND_BACK;

        public static int GL_POSITION;

        public static int GL_DIFFUSE;

        public static int GL_SPECULAR;

        public static int GL_LIGHT_MODEL_AMBIENT;

        public static int GL_S;

        public static int GL_TEXTURE_GEN_MODE;

        public static int GL_OBJECT_LINEAR;

        public static int GL_T;

        public static int GL_R;

        public static int GL_Q;

        public static int GL_EYE_LINEAR;

        public static int GL_OBJECT_PLANE;

        public static int GL_EYE_PLANE;

        public static int GL_TEXTURE_GEN_S;

        public static int GL_TEXTURE_GEN_T;

        public static int GL_TEXTURE_GEN_R;

        public static int GL_TEXTURE_GEN_Q;

        public static int GL_COMPILE;

        public static int GL_DST_COLOR;

        public static int GL_SRC_COLOR;

        public static int GL_POLYGON_OFFSET_FILL;

        public static int GL_COLOR_LOGIC_OP;

        public static int GL_OR_REVERSE;

        public static int GL_EQUAL;

        public static int GL_PROJECTION_MATRIX;

        public static int GL_MODELVIEW_MATRIX;

        public static int GL_NORMALIZE;

        public static int GL_TRIANGLE_STRIP;

        public static int GL_DST_ALPHA;

        public static int GL_GEQUAL;

        public static int GL_VIEWPORT;

        public static void GlEnable(int glTexture_2D)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlShadeModel(int glSmooth)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlClearDepth(double depth)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlDepthFunc(int glLequal)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlAlphaFunc(int glGreater, float f)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlCullFace(int glBack)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlMatrixMode(int glProjection)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlLoadIdentity()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlViewport(int x, int y, int displayWidth, int displayHeight)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlClear(int flags)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlOrtho(double d, double getScaledWidthDouble, double getScaledHeightDouble, double d1, double d2, double d3)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTranslatef(float f, float f1, float f2)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlClearColor(float f, float f1, float f2, float f3)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlDisable(object glLighting)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlColor4f(float f, float f1, float f2, float f3)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static int GlGetError()
        {
            return Utils.Diagnostics.Internal.NotImplemented<int>();
        }

        public static void GlFlush()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlLineWidth(float f)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static object GlGetString(int glVersion)
        {
            return Utils.Diagnostics.Internal.NotImplemented<object>();
        }

        public static int GlGetInteger(int glMaxVertexUniformComponents, IntBuffer bla = null)
        {
            return Utils.Diagnostics.Internal.NotImplemented<int>();
        }

        public static void GlTexImage2D(int glProxyTexture_2D, int i, int glRgba, int var0, int var1, int i1, int o, int glUnsignedByte, ByteBuffer byteBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static int GlGetTexLevelParameteri(int glProxyTexture_2D, int i, int glTextureWidth)
        {
            return Utils.Diagnostics.Internal.NotImplemented<int>();
        }

        public static int GlGenLists(int par0)
        {
            return Utils.Diagnostics.Internal.NotImplemented<int>();
        }

        public static void GlDeleteLists(int par0, int i)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlDeleteTextures(int i)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlRotatef(float p0, float p1, float p2, float p3)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlScaled(double cameraZoom, double d, double d1)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlScalef(float p0, float var4, float p2)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlPushMatrix()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlPopMatrix()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTexParameteri(int glTexture_2D, int glTextureMinFilter, int glLinear)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlColorMask(bool p0, bool p1, bool p2, bool p3)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlBlendFunc(int glSrcAlpha, int glOneMinusSrcAlpha)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlDepthMask(bool p0)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlNormal3f(float f, float f1, float f2)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlFog(int glFogColor, FloatBuffer setFogColorBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlFogi(int glFogMode, int glLinear)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlFogf(int glFogStart, float p1)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlColorMaterial(int glFront, int glAmbient)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlBindTexture(int glTexture_2D, int par0)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static int GlGenTextures()
        {
            return Utils.Diagnostics.Internal.NotImplemented<int>();
        }

        public static void GlTexSubImage2D(int glTexture_2D, int i, int par3, int i1, int par1, int var10, int glBgra, int glUnsignedInt8888Rev, IntBuffer dataBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlPixelStorei(int glPackAlignment, int p1)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlReadPixels(int i, int i1, int par2, int par3, int glBgra, int glUnsignedInt8888Rev, IntBuffer field74293B)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTexCoordPointer(int p0, int glFloat, int p2, long p3)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlEnableClientState(int glTextureCoordArray)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlColorPointer(int p0, int glUnsignedByte, int p2, long p3)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlNormalPointer(int glUnsignedByte, int p1, long l)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlVertexPointer(int p0, int glFloat, int p2, long l)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlDrawArrays(int glTriangles, int p1, int vertexCount)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlDisableClientState(int glVertexArray)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlLight(int glLight0, int glPosition, FloatBuffer setColorBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlLightModel(int glLightModelAmbient, FloatBuffer setColorBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTexGeni(int glS, int glTextureGenMode, int glObjectLinear)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTexGen(int glS, int glObjectPlane, FloatBuffer func76907A)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlColor3f(float var3, float var4, float var5)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlCallList(int glSkyList)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlNewList(int starGlCallList, int glCompile)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlEndList()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlPolygonOffset(float p0, float p1)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlLogicOp(int glOrReverse)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlGetFloat(int glProjectionMatrix, FloatBuffer projectionMatrixBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlBegin(int glTriangleStrip)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTexCoord2f(float p0, float p1)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlVertex3f(float p0, float posY, float p2)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlEnd()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTexParameterf(int glTexture_2D, int glTextureWrapS, float p2)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlCopyTexSubImage2D(int glTexture_2D, int i, int i1, int i2, int i3, int i4, int i5, int i6)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlCallLists(IntBuffer field78424G)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTexCoordPointer(int p0, int glFloat, ShortBuffer shortBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTexCoordPointer(int p0, int glFloat, FloatBuffer floatBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlColorPointer(int p0, bool glUnsignedByte, int p2, ByteBuffer byteBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlNormalPointer(int glUnsignedByte, ByteBuffer byteBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }
        
        public static void GlVertexPointer(int p0, int glFloat, FloatBuffer floatBuffer)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlRotatef(double p0, float p1, float p2, float p3)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void GlTranslatef(float f, double f1, float f2)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }
    }
}
