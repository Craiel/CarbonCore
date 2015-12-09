namespace CarbonCore.Utils.IoC
{
    using System;
    using System.Collections.Generic;

    public partial class CarbonQuickContainer
    {
        internal class ResolvePersistentContext
        {
            private long resetLock;

            // -------------------------------------------------------------------
            // Constructor
            // -------------------------------------------------------------------
            public ResolvePersistentContext()
            {
                this.RecursiveCheckList = new List<Type>();
            }

            // -------------------------------------------------------------------
            // Public
            // -------------------------------------------------------------------
            public long ResolveCount { get; set; }

            public IList<Type> RecursiveCheckList { get; private set; }

            public void Lock()
            {
                this.resetLock++;
            }

            public void Unlock()
            {
                this.resetLock--;
            }

            public void Reset()
            {
                if (this.resetLock > 0)
                {
                    return;
                }

                this.RecursiveCheckList.Clear();
            }

            public void Assert(Type target)
            {
                this.ResolveCount++;
                if (this.ResolveCount >= ResolveHardLimit)
                {
                    string message = string.Format("Resolve Exceeded max of {0}, aborting! ({1})", this.ResolveCount, this.RecursiveCheckList.Count);
                    throw new InvalidOperationException(message);
                }

                // Todo: Find a better way to detect recursive issues
                /*if (this.RecursiveCheckList.Contains(target))
                {
                    throw new InvalidOperationException("Recursive Resolve Detected of " + target);
                }*/

                this.RecursiveCheckList.Add(target);
            }
        }

        internal class ResolveContext
        {
            // -------------------------------------------------------------------
            // Constructor
            // -------------------------------------------------------------------
            public ResolveContext(Type targetType)
            {
                this.TargetType = targetType;
                this.CustomParameters = new Dictionary<string, object>();
            }

            // -------------------------------------------------------------------
            // Public
            // -------------------------------------------------------------------
            public Type TargetType { get; private set; }

            public IDictionary<string, object> CustomParameters { get; private set; }

            public void SetCustomParameters(IDictionary<string, object> parameters)
            {
                if (parameters == null)
                {
                    return;
                }

                foreach (string key in parameters.Keys)
                {
                    this.CustomParameters.Add(key, parameters[key]);
                }
            }
        }
    }
}
