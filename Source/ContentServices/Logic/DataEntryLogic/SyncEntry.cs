﻿namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.IO;

    using CarbonCore.ContentServices.Contracts;

    public abstract class SyncEntry : ISyncEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract bool IsChanged { get; }

        public abstract void Save(Stream target, bool ignoreChangeState = false);

        public abstract void Load(Stream source);

        public abstract void ResetChangeState(bool state = false);
    }
}
