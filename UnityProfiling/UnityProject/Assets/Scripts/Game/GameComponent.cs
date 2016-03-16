namespace Assets.Scripts.Game
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Data;

    using CarbonCore.Utils.Unity.Logic;

    using UnityEngine;

    public abstract class GameComponent : DelayedLoadedObject
    {
        private readonly IList<IntervalTrigger> intervals;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected GameComponent()
        {
            this.intervals = new List<IntervalTrigger>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsInitialized { get; private set; }

        public virtual void Initialize()
        {
            if (this.IsInitialized)
            {
                throw new InvalidOperationException(string.Format("Component {0} was already initialized", this.GetType().Name));
            }

            this.IsInitialized = true;
        }

        public virtual void Destroy()
        {
            this.IsInitialized = false;
        }

        public virtual void Update()
        {
            if (!this.IsInitialized)
            {
                throw new InvalidOperationException(string.Format("Update called on {0} before Initialize!", this.GetType().Name));
            }
            
            foreach (IntervalTrigger trigger in this.intervals)
            {
                trigger.Update(Time.time);
            }
        }

        public virtual void Save(SaveData target)
        {
            // Nothing to do by default
        }

        public virtual void Load(SaveData source)
        {
            // Nothing to do by default
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void RegisterInterval(float interval, IntervalTriggerDelegate callback)
        {
            System.Diagnostics.Trace.Assert(interval > 0f);

            var trigger = new IntervalTrigger(interval);
            trigger.OnTrigger += callback;
            this.RegisterInterval(trigger);
        }

        protected void RegisterInterval(IntervalTrigger trigger)
        {
            this.intervals.Add(trigger);
        }

        protected void UnregisterInterval(IntervalTrigger trigger)
        {
            this.intervals.Remove(trigger);
        }
    }
}
