namespace CarbonCore.JSharpBridge.GL
{
    using System;

    using CarbonCore.JSharpBridge.IO;

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
            throw new NotImplementedException();
        }

        public static void GlShadeModel(int glSmooth)
        {
            throw new NotImplementedException();
        }

        public static void GlClearDepth(double depth)
        {
            throw new NotImplementedException();
        }

        public static void GlDepthFunc(int glLequal)
        {
            throw new NotImplementedException();
        }

        public static void GlAlphaFunc(int glGreater, float f)
        {
            throw new NotImplementedException();
        }

        public static void GlCullFace(int glBack)
        {
            throw new NotImplementedException();
        }

        public static void GlMatrixMode(int glProjection)
        {
            throw new NotImplementedException();
        }

        public static void GlLoadIdentity()
        {
            throw new NotImplementedException();
        }

        public static void GlViewport(int x, int y, int displayWidth, int displayHeight)
        {
            throw new NotImplementedException();
        }

        public static void GlClear(int flags)
        {
            throw new NotImplementedException();
        }

        public static void GlOrtho(double d, double getScaledWidthDouble, double getScaledHeightDouble, double d1, double d2, double d3)
        {
            throw new NotImplementedException();
        }

        public static void GlTranslatef(float f, float f1, float f2)
        {
            throw new NotImplementedException();
        }

        public static void GlClearColor(float f, float f1, float f2, float f3)
        {
            throw new NotImplementedException();
        }

        public static void GlDisable(object glLighting)
        {
            throw new NotImplementedException();
        }

        public static void GlColor4f(float f, float f1, float f2, float f3)
        {
            throw new NotImplementedException();
        }

        public static int GlGetError()
        {
            throw new NotImplementedException();
        }

        public static void GlFlush()
        {
            throw new NotImplementedException();
        }

        public static void GlLineWidth(float f)
        {
            throw new NotImplementedException();
        }

        public static object GlGetString(int glVersion)
        {
            throw new NotImplementedException();
        }

        public static int GlGetInteger(int glMaxVertexUniformComponents, IntBuffer bla = null)
        {
            throw new NotImplementedException();
        }

        public static void GlTexImage2D(int glProxyTexture_2D, int i, int glRgba, int var0, int var1, int i1, int o, int glUnsignedByte, ByteBuffer byteBuffer)
        {
            throw new NotImplementedException();
        }

        public static int GlGetTexLevelParameteri(int glProxyTexture_2D, int i, int glTextureWidth)
        {
            throw new NotImplementedException();
        }

        public static int GlGenLists(int par0)
        {
            throw new NotImplementedException();
        }

        public static void GlDeleteLists(int par0, int i)
        {
            throw new NotImplementedException();
        }

        public static void GlDeleteTextures(int i)
        {
            throw new NotImplementedException();
        }

        public static void GlRotatef(float p0, float p1, float p2, float p3)
        {
            throw new NotImplementedException();
        }

        public static void GlScaled(double cameraZoom, double d, double d1)
        {
            throw new NotImplementedException();
        }

        public static void GlScalef(float p0, float var4, float p2)
        {
            throw new NotImplementedException();
        }

        public static void GlPushMatrix()
        {
            throw new NotImplementedException();
        }

        public static void GlPopMatrix()
        {
            throw new NotImplementedException();
        }

        public static void GlTexParameteri(int glTexture_2D, int glTextureMinFilter, int glLinear)
        {
            throw new NotImplementedException();
        }

        public static void GlColorMask(bool p0, bool p1, bool p2, bool p3)
        {
            throw new NotImplementedException();
        }

        public static void GlBlendFunc(int glSrcAlpha, int glOneMinusSrcAlpha)
        {
            throw new NotImplementedException();
        }

        public static void GlDepthMask(bool p0)
        {
            throw new NotImplementedException();
        }

        public static void GlNormal3f(float f, float f1, float f2)
        {
            throw new NotImplementedException();
        }

        public static void GlFog(int glFogColor, FloatBuffer setFogColorBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlFogi(int glFogMode, int glLinear)
        {
            throw new NotImplementedException();
        }

        public static void GlFogf(int glFogStart, float p1)
        {
            throw new NotImplementedException();
        }

        public static void GlColorMaterial(int glFront, int glAmbient)
        {
            throw new NotImplementedException();
        }

        public static void GlBindTexture(int glTexture_2D, int par0)
        {
            throw new NotImplementedException();
        }

        public static int GlGenTextures()
        {
            throw new NotImplementedException();
        }

        public static void GlTexSubImage2D(int glTexture_2D, int i, int par3, int i1, int par1, int var10, int glBgra, int glUnsignedInt8888Rev, IntBuffer dataBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlPixelStorei(int glPackAlignment, int p1)
        {
            throw new NotImplementedException();
        }

        public static void GlReadPixels(int i, int i1, int par2, int par3, int glBgra, int glUnsignedInt8888Rev, IntBuffer field74293B)
        {
            throw new NotImplementedException();
        }

        public static void GlTexCoordPointer(int p0, int glFloat, int p2, long p3)
        {
            throw new NotImplementedException();
        }

        public static void GlEnableClientState(int glTextureCoordArray)
        {
            throw new NotImplementedException();
        }

        public static void GlColorPointer(int p0, int glUnsignedByte, int p2, long p3)
        {
            throw new NotImplementedException();
        }

        public static void GlNormalPointer(int glUnsignedByte, int p1, long l)
        {
            throw new NotImplementedException();
        }

        public static void GlVertexPointer(int p0, int glFloat, int p2, long l)
        {
            throw new NotImplementedException();
        }

        public static void GlDrawArrays(int glTriangles, int p1, int vertexCount)
        {
            throw new NotImplementedException();
        }

        public static void GlDisableClientState(int glVertexArray)
        {
            throw new NotImplementedException();
        }

        public static void GlLight(int glLight0, int glPosition, FloatBuffer setColorBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlLightModel(int glLightModelAmbient, FloatBuffer setColorBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlTexGeni(int glS, int glTextureGenMode, int glObjectLinear)
        {
            throw new NotImplementedException();
        }

        public static void GlTexGen(int glS, int glObjectPlane, FloatBuffer func76907A)
        {
            throw new NotImplementedException();
        }

        public static void GlColor3f(float var3, float var4, float var5)
        {
            throw new NotImplementedException();
        }

        public static void GlCallList(int glSkyList)
        {
            throw new NotImplementedException();
        }

        public static void GlNewList(int starGlCallList, int glCompile)
        {
            throw new NotImplementedException();
        }

        public static void GlEndList()
        {
            throw new NotImplementedException();
        }

        public static void GlPolygonOffset(float p0, float p1)
        {
            throw new NotImplementedException();
        }

        public static void GlLogicOp(int glOrReverse)
        {
            throw new NotImplementedException();
        }

        public static void GlGetFloat(int glProjectionMatrix, FloatBuffer projectionMatrixBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlBegin(int glTriangleStrip)
        {
            throw new NotImplementedException();
        }

        public static void GlTexCoord2f(float p0, float p1)
        {
            throw new NotImplementedException();
        }

        public static void GlVertex3f(float p0, float posY, float p2)
        {
            throw new NotImplementedException();
        }

        public static void GlEnd()
        {
            throw new NotImplementedException();
        }

        public static void GlTexParameterf(int glTexture_2D, int glTextureWrapS, float p2)
        {
            throw new NotImplementedException();
        }

        public static void GlCopyTexSubImage2D(int glTexture_2D, int i, int i1, int i2, int i3, int i4, int i5, int i6)
        {
            throw new NotImplementedException();
        }

        public static void GlCallLists(IntBuffer field78424G)
        {
            throw new NotImplementedException();
        }

        public static void GlTexCoordPointer(int p0, int glFloat, ShortBuffer shortBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlTexCoordPointer(int p0, int glFloat, FloatBuffer floatBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlColorPointer(int p0, bool glUnsignedByte, int p2, ByteBuffer byteBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlNormalPointer(int glUnsignedByte, ByteBuffer byteBuffer)
        {
            throw new NotImplementedException();
        }
        
        public static void GlVertexPointer(int p0, int glFloat, FloatBuffer floatBuffer)
        {
            throw new NotImplementedException();
        }

        public static void GlRotatef(double p0, float p1, float p2, float p3)
        {
            throw new NotImplementedException();
        }

        public static void GlTranslatef(float f, double f1, float f2)
        {
            throw new NotImplementedException();
        }
    }
}
