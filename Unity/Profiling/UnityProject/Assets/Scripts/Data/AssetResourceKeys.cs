namespace Assets.Scripts.Data
{
    using CarbonCore.Utils.Unity.Data;

    using UnityEngine;

    public static class AssetResourceKeys
    {
        public static readonly ResourceKey PrefabGameAudioSourceAssetKey = ResourceKey.Create<GameObject>("Sound/GameAudioSource");
        
        public static readonly ResourceKey SfxAcceptAssetKey = ResourceKey.Create<AudioClip>("Sound/Effects/Accept");
    }
}
