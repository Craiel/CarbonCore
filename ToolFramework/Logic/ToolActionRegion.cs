namespace CarbonCore.ToolFramework.Logic
{
    using System;

    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.Utils.Contracts.IoC;

    public class ToolActionRegion : IDisposable
    {
        private readonly IToolAction toolAction;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ToolActionRegion(IFactory factory, IToolAction toolAction)
        {
            this.toolAction = toolAction;
            this.Result = factory.Resolve<IToolActionResult>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IToolActionResult Result { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Assign the result when we leave the region
                this.toolAction.Result = this.Result;
            }
        }
    }
}
