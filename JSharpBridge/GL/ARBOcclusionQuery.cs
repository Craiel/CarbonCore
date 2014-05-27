using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarbonCore.JSharpBridge.GL
{
    using CarbonCore.JSharpBridge.IO;

    public static class ARBOcclusionQuery
    {
        public static int GL_QUERY_RESULT_AVAILABLE_ARB;

        public static int GL_QUERY_RESULT_ARB;

        public static int GL_SAMPLES_PASSED_ARB;

        public static void GlGenQueriesARB(IntBuffer glOcclusionQueryBase)
        {
            throw new NotImplementedException();
        }

        public static void GlGetQueryObjectuARB(int glOcclusionQuery, int glQueryResultAvailableArb, IntBuffer occlusionResult)
        {
            throw new NotImplementedException();
        }

        public static void GlBeginQueryARB(int glSamplesPassedArb, int glOcclusionQuery)
        {
            throw new NotImplementedException();
        }

        public static void GlEndQueryARB(int glSamplesPassedArb)
        {
            throw new NotImplementedException();
        }
    }
}
