namespace CarbonCore.Utils.Edge.WPF.Collections
{
    using System;

    public class ExtendedObservableCollectionSuspendRegion<T> : IDisposable
    {
        private readonly ExtendedObservableCollection<T> host;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ExtendedObservableCollectionSuspendRegion(ExtendedObservableCollection<T> host)
        {
            this.host = host;
            this.host.SuspendNotification();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.host.ResumeNotification();
            }
        }
    }
}
