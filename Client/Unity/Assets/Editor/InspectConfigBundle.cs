using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class InspectConfigBundle
    {
        public static void RunFromEnv()
        {
            string rawPaths = Environment.GetEnvironmentVariable("INSPECT_CONFIG_BUNDLE_PATHS") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rawPaths))
            {
                Debug.LogError("[InspectConfigBundle] INSPECT_CONFIG_BUNDLE_PATHS is empty.");
                EditorApplication.Exit(2);
                return;
            }

            string rawKeys = Environment.GetEnvironmentVariable("INSPECT_CONFIG_KEYS") ?? "Map_InfoConfig";
            string outputDir = Environment.GetEnvironmentVariable("INSPECT_CONFIG_OUTPUT_DIR") ?? string.Empty;
            string[] keys = rawKeys
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (keys.Length == 0)
            {
                keys = new[] { "Map_InfoConfig" };
            }

            bool anyFailure = false;
            foreach (string bundlePath in rawPaths.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!InspectOne(bundlePath.Trim(), keys, outputDir))
                {
                    anyFailure = true;
                }
            }

            EditorApplication.Exit(anyFailure ? 1 : 0);
        }

        public static void Inspect()
        {
            string[] bundlePaths =
            {
                @"F:/MUSF/Client/Unity/Assets/StreamingAssets/config.unity3d",
                @"C:/Users/ZM/Documents/New project/phone-base-config.unity3d",
            };

            foreach (string bundlePath in bundlePaths)
            {
                InspectOne(bundlePath, new[]
                {
                    "AttributeNode_InfoConfig",
                    "Auto_AreaConfig",
                    "BattleCareer_SpellConfig",
                    "BattleMaster_CareerConfig",
                    "BloodAwakening_InfoConfig",
                    "CreateRoleConfig_InfoConfig",
                    "ItemAdvanEntry_BaseConfig",
                    "LimitedPurchase_RewardPropsConfig",
                    "Map_InfoConfig",
                    "Reincarnate_InfoConfig",
                    "Tips_LogicErrorConfig",
                    "ValueGift_ItemInfoConfig",
                    "ValueGift_TypeConfig",
                });
            }
        }

        private static bool InspectOne(string bundlePath, string[] keysToCheck, string outputDir = "")
        {
            Debug.Log($"[InspectConfigBundle] bundle={bundlePath}");

            if (!File.Exists(bundlePath))
            {
                Debug.LogError($"[InspectConfigBundle] missing bundle: {bundlePath}");
                return false;
            }

            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
            if (assetBundle == null)
            {
                Debug.LogError($"[InspectConfigBundle] failed to load bundle: {bundlePath}");
                return false;
            }

            bool success = true;
            try
            {
                GameObject configObject = assetBundle.LoadAsset<GameObject>("Config");
                if (configObject == null)
                {
                    Debug.LogError($"[InspectConfigBundle] missing Config prefab in: {bundlePath}");
                    return false;
                }

                ReferenceCollector rc = configObject.GetComponent<ReferenceCollector>();
                if (rc == null)
                {
                    Debug.LogError($"[InspectConfigBundle] Config prefab missing ReferenceCollector: {bundlePath}");
                    return false;
                }

                Dictionary<string, ReferenceCollectorData> textAssets = rc.data
                    .Where(d => d.type == MonoReferenceType.TextAsset)
                    .GroupBy(d => d.key)
                    .ToDictionary(g => g.Key, g => g.First());

                Debug.Log($"[InspectConfigBundle] text asset count={textAssets.Count}");

                bool exportAll = keysToCheck.Any(k => k == "*");
                if (exportAll)
                {
                    foreach (KeyValuePair<string, ReferenceCollectorData> kv in textAssets.OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        TextAsset textAsset = kv.Value.gameObject as TextAsset;
                        int length = textAsset != null ? textAsset.text.Length : -1;
                        Debug.Log($"[InspectConfigBundle] key={kv.Key} asset={(textAsset != null)} length={length}");
                        if (textAsset != null)
                        {
                            ExportTextAsset(bundlePath, kv.Key, textAsset, outputDir);
                        }
                    }
                }

                foreach (string key in keysToCheck)
                {
                    if (key == "*")
                    {
                        continue;
                    }

                    if (!textAssets.TryGetValue(key, out ReferenceCollectorData data))
                    {
                        Debug.LogError($"[InspectConfigBundle] missing key={key}");
                        success = false;
                        continue;
                    }

                    TextAsset textAsset = data.gameObject as TextAsset;
                    int length = textAsset != null ? textAsset.text.Length : -1;
                    Debug.Log($"[InspectConfigBundle] key={key} asset={(textAsset != null)} length={length}");
                    if (textAsset != null)
                    {
                        ExportTextAsset(bundlePath, key, textAsset, outputDir);
                        string preview = textAsset.text.Length > 600 ? textAsset.text.Substring(0, 600) : textAsset.text;
                        Debug.Log($"[InspectConfigBundle] preview key={key}\n{preview}");
                    }
                }

                foreach (string key in keysToCheck)
                {
                    TextAsset directAsset = assetBundle.LoadAsset<TextAsset>(key);
                    Debug.Log($"[InspectConfigBundle] direct key={key} asset={(directAsset != null)} length={(directAsset != null ? directAsset.text.Length : -1)}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[InspectConfigBundle] exception bundle={bundlePath}\n{ex}");
                success = false;
            }
            finally
            {
                assetBundle.Unload(true);
            }

            return success;
        }

        private static void ExportTextAsset(string bundlePath, string key, TextAsset textAsset, string outputDir)
        {
            if (textAsset == null || string.IsNullOrWhiteSpace(outputDir))
            {
                return;
            }

            string bundleName = Path.GetFileNameWithoutExtension(bundlePath);
            string exportDir = Path.Combine(outputDir, bundleName);
            Directory.CreateDirectory(exportDir);

            string safeKey = MakeSafeFileName(key);
            string exportPath = Path.Combine(exportDir, $"{safeKey}.txt");
            File.WriteAllText(exportPath, textAsset.text);
            Debug.Log($"[InspectConfigBundle] exported key={key} path={exportPath}");
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidChar, '_');
            }

            return name;
        }
    }
}
