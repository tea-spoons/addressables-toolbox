
namespace TeaSpoons.AddressablesToolbox
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using Cysharp.Threading.Tasks;
    using UnityObject = UnityEngine.Object;
    using System;

    /// <summary>
    /// A wrapper for <see cref="AssetReference"/>s that makes sure that its operation handle isn't used multiple times.
    /// </summary>
    /// <typeparam name="TObject">The type of object being referenced.</typeparam>
    /// <typeparam name="TReference">The <see cref="AssetReference"/> type responsible for handling the loading.</typeparam>
    public abstract class SmartReferenceBase<TObject, TReference> : SmartReferenceBase
        where TObject : UnityObject
        where TReference : AssetReference
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            public const string reference = nameof(_reference);
        }
#endif
        [SerializeField]
        private TReference _reference;
        protected TReference reference => _reference;

        /// <summary>
        /// <c>true</c> if a valid asset is referenced in a valid way.
        /// </summary>
        /// <remarks>
        /// Must be <c>true</c> for <see cref="GetAssetAsync"/> to return a non-null value.
        /// </remarks>
        public bool HasValidReference => _reference.RuntimeKeyIsValid();

        private AsyncOperationHandle<TObject> operationHandle;

        protected internal override Type targetType => typeof(TObject);

#if UNITY_EDITOR
        /// <summary>
        /// [EDITOR ONLY] A direct reference to the asset, available regardless of loading state.
        /// </summary>
        /// <remarks>
        /// Useful for editor-time code, including things like OnDrawGizmos.
        /// </remarks>
        public virtual TObject EditorAsset => (TObject)reference.editorAsset;
#endif

        /// <summary>
        /// Returns the progress of the loading operation.
        /// </summary>
        public float LoadProgress => operationHandle.IsValid() ? operationHandle.PercentComplete : 0f;

        /// <summary>
        /// Returns whether the asset is loaded.
        /// </summary>
        public bool IsLoaded => operationHandle.IsValid() && operationHandle.IsDone;

        public DownloadStatus GetDownloadStatus => operationHandle.GetDownloadStatus();

        /// <summary>
        /// Returns the referenced asset.
        /// If it is not yet loaded, it will be loaded first.
        /// </summary>
        public async UniTask<TObject> GetAssetAsync()
        {
            if (!HasValidReference)
            {
                return null;
            }

            if (!operationHandle.IsValid())
            {
                operationHandle = LoadAssetAsync();
            }

            return await operationHandle;
        }

        public void ReleaseAsset()
        {
            if (reference.IsValid())
            {
                reference.ReleaseAsset();
                operationHandle = default;
            }
        }

        /// <summary>
        /// The method for loading the referenced object.
        /// </summary>
        /// <remarks>
        /// Some subtypes of this class are unable to simply override <see cref="AssetReference.LoadAssetAsync{TObject}"/> or may require additional code,
        /// so this method can be overridden to account for that.
        /// </remarks>
        protected virtual AsyncOperationHandle<TObject> LoadAssetAsync()
        {
            return Addressables.LoadAssetAsync<TObject>(reference);
        }
    }

    public abstract class SmartReferenceBase
    {
        /// <summary>
        /// The type of object that can be assigned to the <see cref="reference"/>.
        /// </summary>
        protected internal abstract Type targetType { get; }

        /// <summary>
        /// Whether or not the class has the capability to reference components and handle loading of their GameObjects correctly.
        /// </summary>
        protected internal virtual bool supportsComponents => false;
    }
}
