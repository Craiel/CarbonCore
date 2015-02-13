namespace CarbonCore.JSharpBridge.Core
{
    using System;
    
    public class BridgeThread : IDisposable
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BridgeThread(string name = null)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public BridgeThread(Runnable source)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public BridgeThread(object source, string fileIoThread)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void Dispose()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void SetDaemon(bool unknown)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void Start()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void Stop()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void Sleep(long time)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void Interrupt()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public bool IsInterrupted()
        {
            return Utils.Diagnostics.Internal.NotImplemented<bool>();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        public static void Yield()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public static void DumpStack()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public bool IsAlive()
        {
            return Utils.Diagnostics.Internal.NotImplemented<bool>();
        }

        public static BridgeThread CurrentThread()
        {
            return Utils.Diagnostics.Internal.NotImplemented<BridgeThread>();
        }

        public StackTraceElement[] GetStackTrace()
        {
            return Utils.Diagnostics.Internal.NotImplemented<StackTraceElement[]>();
        }

        public void SetPriority(int i)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public void SetName(string minecraftMainThread)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }
    }
}
