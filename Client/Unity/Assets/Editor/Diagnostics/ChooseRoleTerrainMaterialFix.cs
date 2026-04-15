using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ETEditor.Diagnostics
{
    public static class ChooseRoleTerrainMaterialFix
    {
        private const string ScenePath = "Assets/Scenes/GameMap/ChooseRole.unity";
        private const string MaterialPath = "Assets/SceneRes/Juese_xuanze/Terrain/ChooseRoleTerrainLit.mat";
        private const string ShaderName = "Universal Render Pipeline/Terrain/Lit";

        public static void Apply()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Terrain terrain = Object.FindObjectOfType<Terrain>();
            if (terrain == null)
            {
                throw new System.InvalidOperationException("ChooseRole terrain not found.");
            }

            Shader shader = Shader.Find(ShaderName);
            if (shader == null)
            {
                throw new System.InvalidOperationException($"Shader not found: {ShaderName}");
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material == null)
            {
                material = new Material(shader)
                {
                    name = "ChooseRoleTerrainLit"
                };
                AssetDatabase.CreateAsset(material, MaterialPath);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
                EditorUtility.SetDirty(material);
            }

            terrain.materialTemplate = material;
            EditorUtility.SetDirty(terrain);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"ChooseRoleTerrainMaterialFix applied: {MaterialPath}");
        }
    }
}
