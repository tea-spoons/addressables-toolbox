
namespace TeaSpoons.AddressablesToolbox
{
    using System;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// An <see cref="AssetReference"/> to a prefab that has to contain a component of type <typeparamref name="TComponent"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>In the editor only prefabs that have the component can be assigned, see <see cref="ValidateAsset(UnityObject)"/>.</description></item>
    /// <item><description>At runtime <see cref="LoadAssetAsync()"/> loads the prefab and returns its component instead of the GameObject.</description></item>
    /// </list>
    /// Like any <see cref="AssetReference"/>, it can only be loaded again after <see cref="ReleaseAsset"/> was called.
    /// To create instances, load the component and instantiate <c>component.gameObject</c> yourself.
    /// </remarks>
    /// <typeparam name="TComponent">The component that the referenced prefab must have.</typeparam>
    [Serializable]
    public class ComponentReference<TComponent> : AssetReference
        where TComponent : Component
    {
        // Loading the prefab is a separate operation; this one wraps it and yields the component.
        private AsyncOperationHandle<TComponent> componentHandle;

        /// <summary>
        /// Creates a reference to the prefab with the given asset GUID.
        /// </summary>
        public ComponentReference(string guid) : base(guid)
        {
        }

#if UNITY_EDITOR
        /// <summary>
        /// Tells the Addressables editor that this reference points at a GameObject (the prefab root).
        /// </summary>
        protected override Type DerivedClassType => typeof(GameObject);

        /// <summary>
        /// [EDITOR ONLY] The prefab that is currently assigned, or <c>null</c> if there is none.
        /// </summary>
        public new GameObject editorAsset => base.editorAsset as GameObject;
#endif

        /// <summary>
        /// Loads the referenced prefab and returns its <typeparamref name="TComponent"/>.
        /// </summary>
        /// <remarks>
        /// The operation fails if the prefab cannot be loaded or does not have the component.
        /// Call <see cref="ReleaseAsset"/> when the component is no longer needed.
        /// </remarks>
        public AsyncOperationHandle<TComponent> LoadAssetAsync()
        {
            var prefabHandle = LoadAssetAsync<GameObject>();
            componentHandle = Addressables.ResourceManager.CreateChainOperation(prefabHandle, ToComponent);
            return componentHandle;
        }

        /// <summary>
        /// Releases the loaded component together with the prefab it came from.
        /// </summary>
        public override void ReleaseAsset()
        {
            if (componentHandle.IsValid())
            {
                Addressables.Release(componentHandle);
                componentHandle = default;
            }

            base.ReleaseAsset();
        }

        /// <summary>
        /// Whether <paramref name="obj"/> is a GameObject that has a <typeparamref name="TComponent"/>.
        /// </summary>
        public override bool ValidateAsset(UnityObject obj)
        {
            return obj is GameObject gameObject && gameObject.TryGetComponent<TComponent>(out _);
        }

        /// <summary>
        /// Whether the asset at <paramref name="path"/> is a prefab that has a <typeparamref name="TComponent"/>.
        /// </summary>
        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return gameObject != null && gameObject.TryGetComponent<TComponent>(out _);
#else
            return false;
#endif
        }

        private static AsyncOperationHandle<TComponent> ToComponent(AsyncOperationHandle<GameObject> prefabHandle)
        {
            var prefab = prefabHandle.Result;
            if (prefab != null && prefab.TryGetComponent(out TComponent component))
            {
                return Addressables.ResourceManager.CreateCompletedOperation(component, null);
            }

            var prefabName = prefab != null ? prefab.name : "(not loaded)";
            return Addressables.ResourceManager.CreateCompletedOperation<TComponent>(
                null, $"Prefab '{prefabName}' has no {typeof(TComponent).Name} component.");
        }
    }
}
