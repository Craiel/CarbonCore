namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System.IO;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Unity.Logic.Enums;

    using UnityEngine;

    public class BundleLoadRequest
    {
        private const int BytesReadPerFrame = 256 * 256 * 32; // 512k

        private byte[] data;
        private FileStream stream;
        private bool fileLoaded;
        private int dataOffset;
        
        private AssetBundleCreateRequest request;

        private AssetBundle bundle;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BundleLoadRequest(BundleLoadInfo info, CarbonFile file)
        {
            this.Info = info;
            this.File = file;
            this.Metric = Diagnostic.BeginTimeMeasure();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BundleLoadInfo Info { get; private set; }

        public CarbonFile File { get; private set; }

        public MetricTime Metric { get; private set; }

        public void LoadImmediate()
        {
            if ((this.Info.Flags & BundleLoadFlags.Uncompressed) != 0)
            {
                this.LoadFromFileImmediate();
                return;
            }

            this.LoadFromMemoryImmediate();
        }

        public bool ContinueLoading()
        {
            if ((this.Info.Flags & BundleLoadFlags.Uncompressed) != 0)
            {
                // File is not async for now, just load sync
                this.LoadFromFileImmediate();
                return false;
            }

            if (this.ContinueReadingFile())
            {
                return true;
            }

            if (this.request == null)
            {
                this.request = AssetBundle.CreateFromMemory(this.data);
                return true;
            }

            if (!this.request.isDone)
            {
                return true;
            }

            this.bundle = this.request.assetBundle;
            this.request = null;
            
            // Clear out the data to have it collected by the GC
            if (this.stream != null)
            {
                this.stream.Dispose();
                this.stream = null;
            }

            this.data = null;
            return false;
        }

        public AssetBundle GetBundle()
        {
            return this.bundle;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool ContinueReadingFile()
        {
            if (this.fileLoaded)
            {
                return false;
            }

            if (this.stream == null)
            {
                this.stream = this.File.OpenRead();
                this.data = new byte[this.stream.Length];
                return true;
            }

            if (this.stream.CanRead && this.dataOffset < this.data.Length)
            {
                int bytesRead = this.stream.Read(
                    this.data,
                    this.dataOffset,
                    Mathf.Min(BytesReadPerFrame, this.data.Length - this.dataOffset));
                this.dataOffset += bytesRead;

                return true;
            }

            this.fileLoaded = true;
            return false;
        }

        private void LoadFromFileImmediate()
        {
            this.bundle = AssetBundle.CreateFromFile(this.File.GetPath());
        }

        private void LoadFromMemoryImmediate()
        {
            using (this.stream = this.File.OpenRead())
            {
                this.data = new byte[this.stream.Length];
                this.stream.Read(this.data, 0, this.data.Length);
            }

            this.bundle = AssetBundle.CreateFromMemoryImmediate(this.data);

            this.stream = null;
            this.data = null;
        }
    }
}
