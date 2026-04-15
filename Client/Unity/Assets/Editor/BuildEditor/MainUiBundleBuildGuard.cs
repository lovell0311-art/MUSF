using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class MainUiBundleBuildGuard
    {
        private const string ConfigRelativePath = @"Tools\LockedBundles\MainUiLayout\lock-config.json";
        public const string UIMainCanvasBundleName = "uimaincanvas.unity3d";
        public const string UIHudBundleName = "ui_hud.unity3d";

        [Serializable]
        private sealed class LockedMainUiConfig
        {
            public string UIMainCanvasSource = string.Empty;
            public string UIHudSource = string.Empty;
        }

        [MenuItem("Tools/构建/修复并校验锁定主界面 Bundle 标签")]
        public static void RepairAndAuditLockedMainUiBundlesMenu()
        {
            RepairAndAuditLockedMainUiBundles();
        }

        public static LockedMainUiBundleTargets RepairAndAuditLockedMainUiBundles()
        {
            LockedMainUiBundleTargets targets = GetLockedMainUiBundleTargets();

            RepairBundleOwnership(targets.UIMainCanvasSource, UIMainCanvasBundleName);
            RepairBundleOwnership(targets.UIHudSource, UIHudBundleName);

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            AuditBundleOwnership(targets.UIMainCanvasSource, UIMainCanvasBundleName);
            AuditBundleOwnership(targets.UIHudSource, UIHudBundleName);

            Debug.Log(
                $"MainUiBundleBuildGuard: locked {UIMainCanvasBundleName} -> {targets.UIMainCanvasSource}, " +
                $"{UIHudBundleName} -> {targets.UIHudSource}");

            return targets;
        }

        public static LockedMainUiBundleTargets GetLockedMainUiBundleTargets()
        {
            string configPath = GetConfigAbsolutePath();
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("Locked main UI config not found.", configPath);
            }

            string json = File.ReadAllText(configPath);
            LockedMainUiConfig config = JsonUtility.FromJson<LockedMainUiConfig>(json);
            if (config == null)
            {
                throw new Exception($"Failed to parse locked main UI config: {configPath}");
            }

            ValidateSourcePath(config.UIMainCanvasSource, nameof(config.UIMainCanvasSource));
            ValidateSourcePath(config.UIHudSource, nameof(config.UIHudSource));

            return new LockedMainUiBundleTargets(config.UIMainCanvasSource, config.UIHudSource);
        }

        public static string GetSnapshotDirectoryAbsolutePath()
        {
            string repositoryRoot = GetRepositoryRootAbsolutePath();
            return Path.Combine(repositoryRoot, "Tools", "LockedBundles", "MainUiLayout", "snapshot");
        }

        private static string GetConfigAbsolutePath()
        {
            return Path.Combine(GetRepositoryRootAbsolutePath(), ConfigRelativePath);
        }

        private static string GetRepositoryRootAbsolutePath()
        {
            DirectoryInfo current = new DirectoryInfo(Application.dataPath);
            while (current != null)
            {
                string candidate = Path.Combine(current.FullName, "Tools");
                if (Directory.Exists(candidate))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new Exception("Unable to resolve repository root for locked main UI build guard.");
        }

        private static void ValidateSourcePath(string assetPath, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                throw new Exception($"Locked main UI config field is empty: {fieldName}");
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                throw new Exception($"Locked main UI source prefab not found: {assetPath}");
            }
        }

        private static void RepairBundleOwnership(string canonicalAssetPath, string bundleName)
        {
            foreach (string ownerPath in AssetDatabase.GetAssetPathsFromAssetBundle(bundleName))
            {
                if (string.Equals(ownerPath, canonicalAssetPath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                ClearBundleName(ownerPath, bundleName);
            }

            SetBundleName(canonicalAssetPath, bundleName);
        }

        private static void AuditBundleOwnership(string canonicalAssetPath, string bundleName)
        {
            string[] owners = AssetDatabase
                    .GetAssetPathsFromAssetBundle(bundleName)
                    .Where(path => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

            if (owners.Length != 1 || !string.Equals(owners[0], canonicalAssetPath, StringComparison.OrdinalIgnoreCase))
            {
                string actualOwners = owners.Length == 0 ? "<none>" : string.Join(", ", owners);
                throw new Exception($"Locked main UI bundle ownership audit failed: {bundleName} => {actualOwners}");
            }
        }

        private static void SetBundleName(string assetPath, string bundleName)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
            {
                throw new Exception($"Unable to get importer for locked main UI asset: {assetPath}");
            }

            if (string.Equals(importer.assetBundleName, bundleName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            importer.assetBundleName = bundleName;
            EditorUtility.SetDirty(importer);
            Debug.Log($"MainUiBundleBuildGuard: set {assetPath} -> {bundleName}");
        }

        private static void ClearBundleName(string assetPath, string expectedBundleName)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null || string.IsNullOrEmpty(importer.assetBundleName))
            {
                return;
            }

            if (!string.Equals(importer.assetBundleName, expectedBundleName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            importer.assetBundleName = string.Empty;
            EditorUtility.SetDirty(importer);
            Debug.Log($"MainUiBundleBuildGuard: cleared {assetPath} from {expectedBundleName}");
        }
    }

    public readonly struct LockedMainUiBundleTargets
    {
        public readonly string UIMainCanvasSource;
        public readonly string UIHudSource;

        public LockedMainUiBundleTargets(string uiMainCanvasSource, string uiHudSource)
        {
            UIMainCanvasSource = uiMainCanvasSource;
            UIHudSource = uiHudSource;
        }
    }
}
