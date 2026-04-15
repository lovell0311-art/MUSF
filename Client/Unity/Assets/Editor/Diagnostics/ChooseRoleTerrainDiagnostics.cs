using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ETEditor.Diagnostics
{
    public static class ChooseRoleTerrainDiagnostics
    {
        private const string ScenePath = "Assets/Scenes/GameMap/ChooseRole.unity";

        public static void Dump()
        {
            EditorSceneManager.OpenScene(ScenePath);

            Terrain terrain = Object.FindObjectOfType<Terrain>();
            if (terrain == null)
            {
                Debug.LogError("ChooseRoleTerrainDiagnostics: terrain not found.");
                return;
            }

            TerrainData data = terrain.terrainData;
            if (data == null)
            {
                Debug.LogError("ChooseRoleTerrainDiagnostics: terrain data missing.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== ChooseRole Terrain Diagnostics ===");
            sb.AppendLine($"Scene: {ScenePath}");
            sb.AppendLine($"Terrain: {terrain.name}");
            sb.AppendLine($"TerrainData: {AssetDatabase.GetAssetPath(data)}");
            sb.AppendLine($"DrawHeightmap={terrain.drawHeightmap} DrawTreesAndFoliage={terrain.drawTreesAndFoliage}");
            sb.AppendLine($"MaterialTemplate={(terrain.materialTemplate != null ? AssetDatabase.GetAssetPath(terrain.materialTemplate) : "<builtin>")}");

            TerrainLayer[] terrainLayers = data.terrainLayers;
            sb.AppendLine($"TerrainLayers={terrainLayers?.Length ?? 0}");
            if (terrainLayers != null)
            {
                for (int i = 0; i < terrainLayers.Length; ++i)
                {
                    TerrainLayer layer = terrainLayers[i];
                    if (layer == null)
                    {
                        sb.AppendLine($"  [{i}] <null>");
                        continue;
                    }

                    sb.AppendLine($"  [{i}] {layer.name} path={AssetDatabase.GetAssetPath(layer)}");
                    sb.AppendLine($"      diffuse={AssetDatabase.GetAssetPath(layer.diffuseTexture)} normal={AssetDatabase.GetAssetPath(layer.normalMapTexture)} mask={AssetDatabase.GetAssetPath(layer.maskMapTexture)}");
                }
            }

            DetailPrototype[] details = data.detailPrototypes;
            sb.AppendLine($"DetailPrototypes={details?.Length ?? 0}");
            if (details != null)
            {
                for (int i = 0; i < details.Length; ++i)
                {
                    DetailPrototype detail = details[i];
                    sb.AppendLine($"  [{i}] renderMode={detail.renderMode} useMesh={detail.usePrototypeMesh}");
                    sb.AppendLine($"      prototype={AssetDatabase.GetAssetPath(detail.prototype)} prototypeTexture={AssetDatabase.GetAssetPath(detail.prototypeTexture)}");
                }
            }

            GameObject[] roots = Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in roots)
            {
                if (!go.name.Contains("cao_sigel"))
                {
                    continue;
                }

                sb.AppendLine($"Object={go.name}");
                Renderer renderer = go.GetComponent<Renderer>();
                if (renderer == null)
                {
                    sb.AppendLine("  renderer=<null>");
                    continue;
                }

                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; ++i)
                {
                    Material material = materials[i];
                    if (material == null)
                    {
                        sb.AppendLine($"  material[{i}]=<null>");
                        continue;
                    }

                    string shaderName = material.shader != null ? material.shader.name : "<null>";
                    sb.AppendLine($"  material[{i}]={material.name} path={AssetDatabase.GetAssetPath(material)} shader={shaderName}");
                }
            }

            Debug.Log(sb.ToString());
        }
    }
}
