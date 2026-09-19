#if UNITY_EDITOR
namespace TeaSpoons.AddressablesToolbox.Tests
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.TestTools;

    /// <summary>
    /// Runs against real Addressables: two temporary prefabs (one with <see cref="TestBehaviour"/>, one without) are
    /// marked addressable by <see cref="ComponentReferenceTestSetup"/> and loaded through <see cref="ComponentReference{TComponent}"/>.
    /// </summary>
    [PrebuildSetup(typeof(ComponentReferenceTestSetup))]
    [PostBuildCleanup(typeof(ComponentReferenceTestSetup))]
    public class ComponentReferenceTests
    {
        private string withGuid;
        private string withoutGuid;
        private GameObject withPrefab;
        private GameObject withoutPrefab;

        [SetUp]
        public void SetUp()
        {
            LogAssert.ignoreFailingMessages = false;

            withPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ComponentReferenceTestSetup.WithComponentPath);
            withoutPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ComponentReferenceTestSetup.WithoutComponentPath);
            Assert.IsNotNull(withPrefab, "The setup should have created the prefab that has the component.");
            Assert.IsNotNull(withoutPrefab, "The setup should have created the prefab without the component.");
            Assert.IsNotNull(withPrefab.GetComponent<TestBehaviour>(), "The test prefab should have kept its component.");

            withGuid = AssetDatabase.AssetPathToGUID(ComponentReferenceTestSetup.WithComponentPath);
            withoutGuid = AssetDatabase.AssetPathToGUID(ComponentReferenceTestSetup.WithoutComponentPath);
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
        }

        [UnityTest]
        public IEnumerator LoadAssetAsyncReturnsTheComponentOfThePrefab()
        {
            var reference = new ComponentReference<TestBehaviour>(withGuid);

            var handle = reference.LoadAssetAsync();
            yield return handle;

            Assert.AreEqual(AsyncOperationStatus.Succeeded, handle.Status);
            Assert.IsNotNull(handle.Result);
            Assert.AreEqual("WithComponent", handle.Result.gameObject.name);

            reference.ReleaseAsset();
        }

        [UnityTest]
        public IEnumerator LoadAssetAsyncFailsWhenThePrefabLacksTheComponent()
        {
            // A failed Addressables operation logs its exception; that is expected here.
            LogAssert.ignoreFailingMessages = true;
            var reference = new ComponentReference<TestBehaviour>(withoutGuid);

            var handle = reference.LoadAssetAsync();
            yield return handle;

            Assert.AreEqual(AsyncOperationStatus.Failed, handle.Status);
            Assert.IsNull(handle.Result);
            // Make sure it failed because of the missing component, not because the prefab could not be found.
            StringAssert.Contains("has no TestBehaviour component", handle.OperationException?.ToString());

            reference.ReleaseAsset();
        }

        [UnityTest]
        public IEnumerator ReferenceCanBeLoadedAgainAfterItWasReleased()
        {
            var reference = new ComponentReference<TestBehaviour>(withGuid);

            var first = reference.LoadAssetAsync();
            yield return first;
            Assert.AreEqual(AsyncOperationStatus.Succeeded, first.Status);
            reference.ReleaseAsset();

            var second = reference.LoadAssetAsync();
            yield return second;

            Assert.AreEqual(AsyncOperationStatus.Succeeded, second.Status);
            Assert.IsNotNull(second.Result);

            reference.ReleaseAsset();
        }

        [Test]
        public void ValidateAssetAcceptsOnlyPrefabsWithTheComponent()
        {
            var reference = new ComponentReference<TestBehaviour>(withGuid);

            Assert.IsTrue(reference.ValidateAsset(withPrefab));
            Assert.IsFalse(reference.ValidateAsset(withoutPrefab));
            Assert.IsFalse(reference.ValidateAsset(Texture2D.whiteTexture));
        }

        [Test]
        public void ValidateAssetChecksThePathToo()
        {
            var reference = new ComponentReference<TestBehaviour>(withGuid);

            Assert.IsTrue(reference.ValidateAsset(ComponentReferenceTestSetup.WithComponentPath));
            Assert.IsFalse(reference.ValidateAsset(ComponentReferenceTestSetup.WithoutComponentPath));
        }

        [Test]
        public void EditorAssetIsTheAssignedPrefab()
        {
            var reference = new ComponentReference<TestBehaviour>(withGuid);

            Assert.AreEqual(withPrefab, reference.editorAsset);
            Assert.IsTrue(reference.RuntimeKeyIsValid());
        }
    }
}
#endif
