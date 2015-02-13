namespace CarbonCore.JSharpBridge.Log
{
    using System;

    public enum Level
    {
        WARNING,
        INFO,
        SEVERE,
        FINE
    }

    public class Logger
    {
        public static Logger GetLogger(string par1Str)
        {
            return Utils.Diagnostics.Internal.NotImplemented<Logger>();
        }

        public void SetUseParentHandlers(bool b)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public Handler[] GetHandlers()
        {
            return Utils.Diagnostics.Internal.NotImplemented<Handler[]>();
        }

        public void RemoveHandler(Handler var4)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void AddHandler(Handler var7)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void Log(Level warning, string s, Exception var5)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void Log(Level warning, string par1Str)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void Log(Level warning, string par1Str, object[] par2ArrayOfObj)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static Logger GetAnonymousLogger()
        {
            return Utils.Diagnostics.Internal.NotImplemented<Logger>();
        }
    }
}
