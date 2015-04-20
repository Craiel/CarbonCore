namespace CarbonCore.Applications.CrystalBuild.Logic
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class ProcessingCache
    {
        private readonly IDictionary<string, string> images;
        private readonly IDictionary<string, long> imageUseCount;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ProcessingCache()
        {
            this.images = new Dictionary<string, string>();
            this.imageUseCount = new Dictionary<string, long>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReadOnlyDictionary<string, string> Images
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(this.images);
            }
        }

        public IReadOnlyDictionary<string, long> ImageUseCount
        {
            get
            {
                return new ReadOnlyDictionary<string, long>(this.imageUseCount);
            }
        }

        public void RegisterImage(string key, string value)
        {
            this.images.Add(key, value);
            this.imageUseCount.Add(key, 0);
        }

        public void RegisterImageUse(string key)
        {
            this.imageUseCount[key]++;
        }
    }
}
