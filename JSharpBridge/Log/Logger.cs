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
            throw new System.NotImplementedException();
        }

        public void SetUseParentHandlers(bool b)
        {
            throw new System.NotImplementedException();
        }

        public Handler[] GetHandlers()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveHandler(Handler var4)
        {
            throw new System.NotImplementedException();
        }

        public void AddHandler(Handler var7)
        {
            throw new System.NotImplementedException();
        }

        public void Log(Level warning, string s, Exception var5)
        {
            throw new NotImplementedException();
        }

        public void Log(Level warning, string par1Str)
        {
            throw new NotImplementedException();
        }

        public void Log(Level warning, string par1Str, object[] par2ArrayOfObj)
        {
            throw new NotImplementedException();
        }

        public static Logger GetAnonymousLogger()
        {
            throw new NotImplementedException();
        }
    }
}
