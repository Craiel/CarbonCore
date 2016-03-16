namespace CarbonCore.Unity.Utils.Data
{
    using System;

    using CarbonCore.Utils;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public struct ResourceKey
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceKey(BundleKey bundle, string path, Type type)
            : this(path, type)
        {
            this.Bundle = bundle;
        }

        public ResourceKey(string path, Type type)
            : this()
        {
            this.Path = path;
            this.Type = type ?? typeof(UnityEngine.Object);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [JsonProperty]
        public BundleKey? Bundle { get; set; }

        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public Type Type { get; set; }

        public static ResourceKey Create<T>(BundleKey bundle, string path)
        {
            return new ResourceKey(bundle, path, typeof(T));
        }

        public static ResourceKey Create<T>(string path)
        {
            return new ResourceKey(path, typeof(T));
        }

        public static bool operator ==(ResourceKey rhs, ResourceKey lhs)
        {
            return rhs.Bundle == lhs.Bundle 
                && rhs.Path == lhs.Path
                && rhs.Type == lhs.Type;
        }

        public static bool operator !=(ResourceKey rhs, ResourceKey lhs)
        {
            return !(rhs == lhs);
        }

        public static ResourceKey GetFromString(string key)
        {
            return JsonExtensions.LoadFromData<ResourceKey>(key);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.Bundle, this.Path);
        }

        public override bool Equals(object other)
        {
            if (other.GetType() != typeof(ResourceKey))
            {
                return false;
            }

            ResourceKey typed = (ResourceKey)other;
            return this == typed;
        }

        public override string ToString()
        {
            return string.Format("{0}:>{1} ({2})", this.Bundle == null ? "default" : this.Bundle.Value.ToString(), this.Path, this.Type);
        }

        public string GetAsString()
        {
            return JsonExtensions.SaveToData(this);
        }
    }
}
