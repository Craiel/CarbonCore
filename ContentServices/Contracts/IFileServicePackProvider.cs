﻿namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.Utils.Compat.IO;

    public interface IFileServicePackProvider : IFileServiceProvider
    {
        string PackPrefix { get; set; }

        CarbonDirectory Root { get; set; }
    }
}
