namespace CarbonCore.Utils.Unity.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    using CarbonCore.Utils.Unity.Contracts;

    // Static helper method to extend a RefCountedObject
    public static class RefCountedWeakReferenceExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static RefCountedWeakReference<T> GetReference<T>(this IRefCountedObject target)
            where T : IRefCountedObject
        {
            return new RefCountedWeakReference<T>((T)target);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class RefCountedWeakReference<T> : WeakReference<T>, IDisposable
        where T : IRefCountedObject
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RefCountedWeakReference(T target)
            : base(target)
        {
            // Count up the reference as we come alive
            this.Target.RefCountIncrease();
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
            if (!isDisposing)
            {
                return;
            }

            if (!this.IsAlive)
            {
                throw new InvalidOperationException("Target reference lost before de-referencing");
            }

            this.Target.RefCountDecrease();
        }
    }
}
