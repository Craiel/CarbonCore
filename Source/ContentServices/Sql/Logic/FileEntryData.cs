﻿namespace CarbonCore.ContentServices.Sql.Logic
{
    public class FileEntryData
    {
        public FileEntryData(byte[] data)
        {
            this.ByteData = data;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public byte[] ByteData { get; private set; }
    }
}
