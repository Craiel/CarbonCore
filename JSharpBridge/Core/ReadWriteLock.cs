namespace CarbonCore.JSharpBridge.Core
{
    using System;

    public class ReadWriteLock
    {
        public JavaLock WriteLock()
        {
            throw new InvalidOperationException();
        }

        public JavaLock ReadLock()
        {
            throw new System.NotImplementedException();
        }
    }
}
