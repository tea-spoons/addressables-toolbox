
namespace TeaSpoons.AddressablesToolbox
{
    using UnityEngine.AddressableAssets;
    using UnityObject = UnityEngine.Object;

    /// <inheritdoc/>
    [System.Serializable]
    public class SmartAssetReference<T> : SmartReferenceBase<T, AssetReferenceT<T>>
        where T : UnityObject
    {

    }
}
