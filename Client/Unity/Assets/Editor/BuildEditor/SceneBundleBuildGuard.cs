using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class SceneBundleBuildGuard
    {
        private const string ScenesRoot = "Assets/Scenes";
        private const string GameMapRoot = "Assets/Scenes/GameMap";

        [MenuItem("Tools/构建/修复并校验地图Scene Bundle标签")]
        public static void RepairAndAuditGameplaySceneBundlesMenu()
        {
            RepairAndAuditGameplaySceneBundles();
        }

        public static void RepairAndAuditGameplaySceneBundles()
        {
            int repairedCount = SynchronizeGameplaySceneBundleTags();

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            AuditGameplaySceneBundles();
            Debug.Log($"SceneBundleBuildGuard: repaired {repairedCount} gameplay scene bundle tag entries.");
        }

        private static int SynchronizeGameplaySceneBundleTags()
        {
            int repairedCount = 0;

            foreach (string gameMapScenePath in EnumerateGameMapScenePaths())
            {
                string sceneName = Path.GetFileNameWithoutExtension(gameMapScenePath);
                string expectedBundleName = $"{sceneName.ToLowerInvariant()}.unity3d";

                repairedCount += SetBundleName(gameMapScenePath, expectedBundleName);

                string legacyScenePath = $"{ScenesRoot}/{sceneName}.unity";
                repairedCount += ClearBundleName(legacyScenePath);
            }

            return repairedCount;
        }

        private static void AuditGameplaySceneBundles()
        {
            List<string> issues = new List<string>();

            foreach (string gameMapScenePath in EnumerateGameMapScenePaths())
            {
                string sceneName = Path.GetFileNameWithoutExtension(gameMapScenePath);
                string expectedBundleName = $"{sceneName.ToLowerInvariant()}.unity3d";

                string[] owners = AssetDatabase
                        .GetAssetPathsFromAssetBundle(expectedBundleName)
                        .Where(path => path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                if (owners.Length != 1 || !string.Equals(owners[0], gameMapScenePath, StringComparison.OrdinalIgnoreCase))
                {
                    string actualOwners = owners.Length == 0 ? "<none>" : string.Join(", ", owners);
                    issues.Add($"{expectedBundleName} => {actualOwners}");
                }
            }

            if (issues.Count > 0)
            {
                throw new Exception("Gameplay scene bundle ownership audit failed:\n" + string.Join("\n", issues));
            }
        }

        private static IEnumerable<string> EnumerateGameMapScenePaths()
        {
            return AssetDatabase
                    .FindAssets("t:Scene", new[] { GameMapRoot })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(path => path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);
        }

        private static int SetBundleName(string assetPath, string bundleName)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
            {
                return 0;
            }

            if (string.Equals(importer.assetBundleName, bundleName, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            importer.assetBundleName = bundleName;
            EditorUtility.SetDirty(importer);
            Debug.Log($"SceneBundleBuildGuard: set {assetPath} -> {bundleName}");
            return 1;
        }

        private static int ClearBundleName(string assetPath)
        {
            SceneAsset legacyScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
            if (legacyScene == null)
            {
                return 0;
            }

            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null || string.IsNullOrEmpty(importer.assetBundleName))
            {
                return 0;
            }

            Debug.Log($"SceneBundleBuildGuard: clear legacy scene bundle {assetPath} (was {importer.assetBundleName})");
            importer.assetBundleName = string.Empty;
            EditorUtility.SetDirty(importer);
            return 1;
        }
    }
}
