namespace CarbonCore.Utils.Unity.Logic
{
    using CarbonCore.Utils.Unity.Contracts;

    public class DelayedLoadedObject : IDelayedLoadedObject
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsLoaded { get; private set; }

        public virtual bool ContinueLoad()
        {
            this.IsLoaded = true;
            return false;
        }
    }
}
