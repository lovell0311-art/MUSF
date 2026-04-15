using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CodeBundleProbe
{
    public static void RunFromEnv()
    {
        string raw = Environment.GetEnvironmentVariable("CODE_BUNDLE_PATHS") ?? string.Empty;
        string exportDir = Environment.GetEnvironmentVariable("CODE_BUNDLE_EXPORT_DIR") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
        {
            Debug.LogError("CODE_BUNDLE_PATHS is empty.");
            EditorApplication.Exit(2);
            return;
        }

        string[] paths = raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        bool anyFailure = false;

        foreach (string rawPath in paths)
        {
            string path = rawPath.Trim();
            if (!File.Exists(path))
            {
                Debug.LogError($"PROBE missing: {path}");
                anyFailure = true;
                continue;
            }

            AssetBundle bundle = null;
            try
            {
                bundle = AssetBundle.LoadFromFile(path);
                if (bundle == null)
                {
                    Debug.LogError($"PROBE failed: {path}");
                    anyFailure = true;
                    continue;
                }

                string[] names = bundle.GetAllAssetNames();
                Debug.Log($"PROBE ok: {path} assets={names.Length}");
                foreach (string name in names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
                {
                    Debug.Log($"PROBE asset-name: {name}");
                }

                foreach (UnityEngine.Object asset in bundle.LoadAllAssets())
                {
                    if (asset == null)
                    {
                        continue;
                    }

                    Debug.Log($"PROBE asset: type={asset.GetType().FullName} name={asset.name}");
                    if (asset is TextAsset textAsset && string.IsNullOrWhiteSpace(exportDir) == false)
                    {
                        ExportTextAsset(textAsset, exportDir, textAsset.name);
                    }

                    if (asset is GameObject gameObject)
                    {
                        ReferenceCollector rc = gameObject.GetComponent<ReferenceCollector>();
                        if (rc == null)
                        {
                            continue;
                        }

                        Debug.Log($"PROBE rc: entries={rc.data.Count}");
                        foreach (ReferenceCollectorData data in rc.data.OrderBy(d => d.key, StringComparer.OrdinalIgnoreCase))
                        {
                            string objectName = data.gameObject != null ? data.gameObject.name : "<null>";
                            string objectType = data.gameObject != null ? data.gameObject.GetType().FullName : "<null>";
                            Debug.Log($"PROBE rc-entry: key={data.key} type={data.type} objectType={objectType} objectName={objectName}");

                            if (data.gameObject is TextAsset rcTextAsset && string.IsNullOrWhiteSpace(exportDir) == false)
                            {
                                ExportTextAsset(rcTextAsset, exportDir, data.key);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"PROBE exception: {path}\n{ex}");
                anyFailure = true;
            }
            finally
            {
                if (bundle != null)
                {
                    bundle.Unload(true);
                }
            }
        }

        EditorApplication.Exit(anyFailure ? 1 : 0);
    }

    private static void ExportTextAsset(TextAsset textAsset, string exportDir, string logicalName)
    {
        Directory.CreateDirectory(exportDir);
        string safeName = MakeSafeFileName(string.IsNullOrWhiteSpace(logicalName) ? textAsset.name : logicalName);
        string bytesPath = Path.Combine(exportDir, $"{safeName}.bytes");
        File.WriteAllBytes(bytesPath, textAsset.bytes);
        Debug.Log($"PROBE exported: {bytesPath} length={textAsset.bytes?.Length ?? 0}");
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
