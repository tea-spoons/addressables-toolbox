
namespace TeaSpoons.AddressablesToolbox
{
    using System;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    /// <inheritdoc/>
    [System.Serializable]
    public class SmartComponentReference<T> : SmartReferenceBase<T, ComponentReference<T>>
        where T : Component
    {
#if UNITY_EDITOR
        // Not using the null conditional operator here just be sure with Unity's null check.
        public override T EditorAsset => (reference.editorAsset != null) ? reference.editorAsset.GetComponent<T>() : null;
#endif

        protected internal override Type targetType => typeof(T);
        protected internal override bool supportsComponents => true;

        protected override AsyncOperationHandle<T> LoadAssetAsync()
        {
            return reference.LoadAssetAsync();
        }
    }
}
