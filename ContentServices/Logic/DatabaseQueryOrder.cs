﻿namespace CarbonCore.ContentServices.Logic
{
    using System;
    
    public class DatabaseQueryOrder
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseQueryOrder(string name)
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(name));

            this.Name = name;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public bool Ascending { get; set; }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Ascending).GetHashCode();
        }
    }
}
