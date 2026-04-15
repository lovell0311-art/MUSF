using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class AndroidShaderKeepAliveBuilder
    {
        private const string OutputFolder = "Assets/Resources/ShaderKeepAlive";

        private static readonly string[] ShaderNames =
        {
            "Universal Render Pipeline/Lit",
            "Universal Render Pipeline/Unlit",
            "Nature/Soft Occlusion Bark",
            "Nature/Soft Occlusion Leaves",
            "Nature/Soft Occlusion Bark Rendertex",
            "Nature/Soft Occlusion Leaves Rendertex",
            "Nature/Tree Creator Bark",
            "Nature/Tree Creator Leaves",
            "Nature/Tree Creator Leaves Fast",
            "Hidden/TerrainEngine/BillboardTree",
        };

        public static void EnsureAssets()
        {
            EnsureFolderExists("Assets/Resources");
            EnsureFolderExists(OutputFolder);

            int createdOrUpdatedCount = 0;
            List<string> foundShaders = new List<string>();
            List<string> missingShaders = new List<string>();

            foreach (string shaderName in ShaderNames)
            {
                Shader shader = Shader.Find(shaderName);
                if (shader == null)
                {
                    missingShaders.Add(shaderName);
                    continue;
                }

                string assetName = SanitizeAssetName(shaderName) + ".mat";
                string assetPath = Path.Combine(OutputFolder, assetName).Replace('\\', '/');
                Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (material == null)
                {
                    material = new Material(shader);
                    AssetDatabase.CreateAsset(material, assetPath);
                    createdOrUpdatedCount++;
                }
                else if (material.shader != shader)
                {
                    material.shader = shader;
                    EditorUtility.SetDirty(material);
                    createdOrUpdatedCount++;
                }

                foundShaders.Add(shaderName);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"AndroidShaderKeepAliveBuilder.EnsureAssets found={foundShaders.Count} updated={createdOrUpdatedCount} missing={missingShaders.Count}");
            if (foundShaders.Count > 0)
            {
                Debug.Log("AndroidShaderKeepAliveBuilder found shaders: " + string.Join(", ", foundShaders));
            }

            if (missingShaders.Count > 0)
            {
                Debug.LogWarning("AndroidShaderKeepAliveBuilder missing shaders: " + string.Join(", ", missingShaders));
            }
        }

        private static string SanitizeAssetName(string shaderName)
        {
            return shaderName
                .Replace('/', '_')
                .Replace('\\', '_')
                .Replace(':', '_')
                .Replace(' ', '_');
        }

        private static void EnsureFolderExists(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; ++i)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
    }
}
