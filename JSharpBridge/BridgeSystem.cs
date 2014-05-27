namespace CarbonCore.JSharpBridge
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using CarbonCore.JSharpBridge.Core;
    using CarbonCore.JSharpBridge.IO;

    public class ThreadLocal : ThreadLocal<object>
    {
        public object Get()
        {
            throw new NotImplementedException();
        }
    }

    public static class BridgeSystem
    {
        public static int UTF_8;

        public static object LocaleUS;

        public static InputStream @in;

        public static StackTraceElement[] GetStackTrace(this Exception exception)
        {
            throw new InvalidOperationException();
        }

        public static void PrintStackTrace(this Exception exception, TextWriter customWriter = null)
        {
            System.Diagnostics.Trace.TraceWarning("PrintStackTrace is not tested!");

            Console.WriteLine(exception.StackTrace);
        }

        public static string GetMessage(this Exception exception)
        {
            System.Diagnostics.Trace.TraceWarning("GetMessage is not tested!");

            return exception.Message;
        }

        public static long JavaNanoTime()
        {
            throw new NotImplementedException();
        }

        public static long GetSystemTime()
        {
            return DateTime.Now.Ticks;
        }

        public static long GetTimerResolution()
        {
            throw new NotImplementedException();
        }

        public static string GetVersion()
        {
            throw new NotImplementedException();
        }

        public static string JavaStringFormat(string format, params object[] entries)
        {
            throw new NotImplementedException();
        }

        public static string ToBinaryString(int value)
        {
            throw new NotImplementedException();
        }

        public static string GetCanonicalName(this Type type)
        {
            throw new NotImplementedException();
        }

        public static string GetSigners(this Type type)
        {
            throw new NotImplementedException();
        }

        public static Package GetPackage(this Type type)
        {
            throw new NotImplementedException();
        }

        public static string GetSimpleName(this Type type)
        {
            throw new NotImplementedException();
        }

        public static Field GetDeclaredField(this Type type, string name)
        {
            throw new NotImplementedException();
        }

        public static ClassLoader GetClassLoader(this Type type)
        {
            throw new NotImplementedException();
        }

        public static string GetProperty(string key)
        {
            throw new NotImplementedException();
        }

        public static RuntimeMXBean GetRuntimeMXBean()
        {
            throw new NotImplementedException();
        }

        public static Runtime GetRuntime()
        {
            throw new NotImplementedException();
        }

        // Todo: check arguments
        public static void Arraycopy<T>(T[] source, int index, T[] target, int targetIndex, int count)
        {
            throw new NotImplementedException();
        }
        
        public static bool RegionMatches(this string source, bool unknown1, int unknown2, string unknown3, int unknown4, int unknown5)
        {
            throw new NotImplementedException();
        }

        public static bool EqualsIgnoreCase(this string source, object other)
        {
            throw new NotImplementedException();
        }

        public static string[] Split(this string source, string splitString, int unknown = 0)
        {
            throw new NotImplementedException();
        }

        public static bool IsEmpty(this string source)
        {
            throw new NotImplementedException();
        }

        public static bool IsBlank(string value)
        {
            throw new NotImplementedException();
        }

        public static bool IsNotEmpty(string value)
        {
            throw new NotImplementedException();
        }

        public static long GetMostSignificantBits(this Guid guid)
        {
            throw new NotImplementedException();
        }

        public static long GetLeastSignificantBits(this Guid guid)
        {
            throw new NotImplementedException();
        }

        public static Guid CreateGUID(string value)
        {
            throw new NotImplementedException();
        }

        public static Guid CreateGUID(long mostSignificant, long leastSignificant)
        {
            throw new NotImplementedException();
        }

        public static Guid RandomUUID()
        {
            throw new NotImplementedException();
        }

        public static byte ByteValue(this object source)
        {
            throw new NotImplementedException();
        }

        public static short ShortValue(this object source)
        {
            throw new NotImplementedException();
        }

        public static float FloatValue(this object source)
        {
            throw new NotImplementedException();
        }

        public static byte[] GetBytes(this string source, string encoding = null)
        {
            throw new NotImplementedException();
        }

        public static void PrintLine(string line)
        {
            throw new NotImplementedException();
        }

        public static object NewInstance(this Type type)
        {
            throw new NotImplementedException();
        }

        public static int IndexOf(this string value, int target, int unknown)
        {
            throw new NotImplementedException();
        }

        public static void Println(string getMessage)
        {
            throw new NotImplementedException();
        }

        public static void Println(object something)
        {
            throw new NotImplementedException();
        }

        public static Type TypeForName(string name)
        {
            throw new NotImplementedException();
        }

        public static void SetProperty(string javaNetPreferipv4stack, string @true)
        {
            throw new NotImplementedException();
        }

        public static long CurrentTimeMillis()
        {
            throw new NotImplementedException();
        }

        public static long NanoTime()
        {
            throw new NotImplementedException();
        }

        public static void DeleteCharAt(this StringBuilder builder, int position)
        {
            throw new NotImplementedException();
        }

        public static InputStream GetResourceAsStream(this Type type, string assetsMinecraftLangEnUsLang)
        {
            throw new NotImplementedException();
        }

        public static int IndexOf(this string value, int index)
        {
            throw new NotImplementedException();
        }

        public static byte[] GetBytes(this string source, object encoding)
        {
            throw new NotImplementedException();
        }

        public static void Exit(int exitCode)
        {
            throw new NotImplementedException();
        }

        public static void Gc()
        {
            throw new NotImplementedException();
        }

        public static long GetTime()
        {
            throw new NotImplementedException();
        }

        public static Guid GuidFromString(string e199ad21Ba8aC53D13D5c69d3a)
        {
            throw new NotImplementedException();
        }

        public static string NormalizeSpace(string var2)
        {
            throw new NotImplementedException();
        }

        public static Toolkit GetDefaultToolkit()
        {
            throw new NotImplementedException();
        }

        public static void OpenURL(string s)
        {
            throw new NotImplementedException();
        }

        public static string Substring(string text, int i, int i1)
        {
            throw new NotImplementedException();
        }

        public static string ReplaceAll(this string value, string what, string with)
        {
            throw new NotImplementedException();
        }

        public static string GetName(this Type type)
        {
            throw new NotImplementedException();
        }

        public static Type GetSuperclass(this Type type)
        {
            throw new NotImplementedException();
        }

        public static Field[] JavaGetFields(this Type type)
        {
            throw new NotImplementedException();
        }

        public static object GetDefaultTimeZone()
        {
            throw new NotImplementedException();
        }

        public static object NewInstance(this ConstructorInfo info, object[] unknown)
        {
            throw new NotImplementedException();
        }

        public static string CreateString(byte[] par0ArrayOfByte = null, int par1 = 0, int i = 0, string utf = null)
        {
            throw new NotImplementedException();
        }

        public static string CreateString(char[] par0ArrayOfByte)
        {
            throw new NotImplementedException();
        }
    }
}
