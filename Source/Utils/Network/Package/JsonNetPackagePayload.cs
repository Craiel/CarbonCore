namespace CarbonCore.Utils.Network.Package
{
    using System;
    using System.Collections.Generic;
    
    using CarbonCore.Utils.Contracts.Network;

    public class JsonNetPackagePayload : JsonNetPackage
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override int Id
        {
            get
            {
                return JsonNetUtils.PackagePayload;
            }
        }

        public List<int> PayloadIds { get; set; }
        public List<byte[]> Data { get; set; }

        public void Add<T>(T package) where T : class, IJsonNetPackage
        {
            if (this.PayloadIds == null)
            {
                this.PayloadIds = new List<int>();
            }

            if (this.Data == null)
            {
                this.Data = new List<byte[]>();
            }

            byte[] data = JsonNetUtils.SerializePackage(package);
            if (data == null || data.Length <= 0)
            {
                throw new InvalidOperationException("Package could not serialize correctly!");
            }

            this.PayloadIds.Add(package.Id);
            this.Data.Add(data);
        }
    }
}
