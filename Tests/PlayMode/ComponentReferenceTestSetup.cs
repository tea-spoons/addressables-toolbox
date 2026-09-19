#if UNITY_EDITOR
namespace TeaSpoons.AddressablesToolbox.Tests
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEngine;
    using UnityEngine.TestTools;

    /// <summary>
    /// Creates two temporary addressable prefabs before play mode starts, because Addressables builds its play mode
    /// catalog when play mode is entered. Removes everything, including Addressables settings it had to create, afterwards.
    /// </summary>
    public class ComponentReferenceTestSetup : IPrebuildSetup, IPostBuildCleanup
    {
        public const string FolderName = "__ComponentReferenceTests";
        public const string FolderPath = "Assets/" + FolderName;
        public const string WithComponentPath = FolderPath + "/WithComponent.prefab";
        public const string WithoutComponentPath = FolderPath + "/WithoutComponent.prefab";

        private const string SettingsFolder = "Assets/AddressableAssetsData";

        // Play mode reloads the domain, so a marker file remembers that the settings were created by the tests.
        private const string CreatedSettingsMarker = FolderPath + "/created-addressable-settings.txt";

        public void Setup()
        {
            var createdSettings = AddressableAssetSettingsDefaultObject.GetSettings(false) == null;
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            AssetDatabase.DeleteAsset(FolderPath);
            AssetDatabase.CreateFolder("Assets", FolderName);
            if (createdSettings)
            {
                File.WriteAllText(CreatedSettingsMarker, "The Addressables settings were created by the tests.");
            }

            CreatePrefab(WithComponentPath, withBehaviour: true);
            CreatePrefab(WithoutComponentPath, withBehaviour: false);

            settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(WithComponentPath), settings.DefaultGroup);
            settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(WithoutComponentPath), settings.DefaultGroup);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void Cleanup()
        {
            var createdSettings = File.Exists(CreatedSettingsMarker);

            var settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
            if (settings != null)
            {
                settings.RemoveAssetEntry(AssetDatabase.AssetPathToGUID(WithComponentPath));
                settings.RemoveAssetEntry(AssetDatabase.AssetPathToGUID(WithoutComponentPath));
            }

            AssetDatabase.DeleteAsset(FolderPath);
            if (createdSettings)
            {
                AssetDatabase.DeleteAsset(SettingsFolder);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreatePrefab(string path, bool withBehaviour)
        {
            var instance = new GameObject(Path.GetFileNameWithoutExtension(path));
            if (withBehaviour)
            {
                instance.AddComponent<TestBehaviour>();
            }

            PrefabUtility.SaveAsPrefabAsset(instance, path);
            Object.DestroyImmediate(instance);
        }
    }
}
#endif
