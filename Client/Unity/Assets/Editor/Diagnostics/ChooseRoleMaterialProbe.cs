using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ETEditor
{
    public static class ChooseRoleMaterialProbe
    {
        private const string ScenePath = "Assets/Scenes/GameMap/ChooseRole.unity";
        private const string ProbeMaterialPath = "Assets/Editor/Diagnostics/ChooseRoleProbeMagenta.mat";
        private const string ReportPath = "reports/choose-role-material-probe.txt";
        private const string BackdropTexturePath = "Assets/SceneRes/Juese_xuanze/texture/ChooseRoleTargetBackdrop.png";
        private const string BackdropMaterialPath = "Assets/SceneRes/Juese_xuanze/texture/ChooseRoleTargetBackdrop.mat";

        [MenuItem("Tools/Diagnostics/ChooseRole/Report Materials")]
        public static void ReportMaterials()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var target = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .FirstOrDefault(t => t.name == "cj_mod" && t.parent != null && t.parent.name == "map");

            if (target == null)
            {
                Debug.LogError("ChooseRole material probe: map/cj_mod not found.");
                return;
            }

            var renderer = target.GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("ChooseRole material probe: Renderer missing on map/cj_mod.");
                return;
            }

            var meshFilter = target.GetComponent<MeshFilter>();
            var mesh = meshFilter != null ? meshFilter.sharedMesh : null;
            var lines = renderer.sharedMaterials
                .Select((material, index) =>
                {
                    var materialName = material != null ? material.name : "<null>";
                    var shaderName = material != null && material.shader != null ? material.shader.name : "<null>";
                    return $"slot={index} material={materialName} shader={shaderName}";
                })
                .ToList();

            lines.Insert(0, $"subMeshCount={(mesh != null ? mesh.subMeshCount : -1)}");
            lines.Insert(1, $"renderer={target.name}");

            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath) ?? "reports");
            File.WriteAllLines(ReportPath, lines);
            AssetDatabase.Refresh();
            Debug.Log($"ChooseRole material probe report written to {ReportPath}");
        }

        [MenuItem("Tools/Diagnostics/ChooseRole/Report All Renderers")]
        public static void ReportAllRenderers()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var lines = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Renderer>(true))
                .Select(renderer =>
                {
                    var path = BuildPath(renderer.transform);
                    var materials = string.Join(", ", renderer.sharedMaterials.Select((material, index) =>
                    {
                        var materialName = material != null ? material.name : "<null>";
                        var shaderName = material != null && material.shader != null ? material.shader.name : "<null>";
                        return $"[{index}] {materialName} <{shaderName}>";
                    }));

                    return $"{path} => {materials}";
                })
                .OrderBy(line => line)
                .ToList();

            const string allRendererReportPath = "reports/choose-role-renderers.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(allRendererReportPath) ?? "reports");
            File.WriteAllLines(allRendererReportPath, lines);
            AssetDatabase.Refresh();
            Debug.Log($"ChooseRole all-renderer report written to {allRendererReportPath}");
        }

        [MenuItem("Tools/Diagnostics/ChooseRole/Probe Slot 0")]
        public static void ProbeSlot0() => ProbeSlot(0);

        [MenuItem("Tools/Diagnostics/ChooseRole/Probe Slot 1")]
        public static void ProbeSlot1() => ProbeSlot(1);

        [MenuItem("Tools/Diagnostics/ChooseRole/Probe Slot 2")]
        public static void ProbeSlot2() => ProbeSlot(2);

        [MenuItem("Tools/Diagnostics/ChooseRole/Probe Slot 3")]
        public static void ProbeSlot3() => ProbeSlot(3);

        [MenuItem("Tools/Diagnostics/ChooseRole/Probe Slot 4")]
        public static void ProbeSlot4() => ProbeSlot(4);

        [MenuItem("Tools/Diagnostics/ChooseRole/Restore Scene")]
        public static void RestoreScene()
        {
            AssetDatabase.Refresh();
            var originalScene = @"F:\MUSF\backups\choose-role-icebaseline-20260414-182106\ChooseRole.unity";
            if (!File.Exists(originalScene))
            {
                Debug.LogError($"ChooseRole restore failed: missing structural reference scene {originalScene}");
                return;
            }

            File.Copy(originalScene, Path.GetFullPath(ScenePath), true);
            AssetDatabase.Refresh();
            Debug.Log("ChooseRole scene restored from icebaseline structural reference.");
        }

        [MenuItem("Tools/Diagnostics/ChooseRole/Convert Back Mats To Additive")]
        public static void ConvertBackMaterialsToAdditive()
        {
            ConvertMaterialShader("Assets/SceneRes/Juese_xuanze/fbx/Materials/back1.mat", "Legacy Shaders/Particles/Additive");
            ConvertMaterialShader("Assets/SceneRes/Juese_xuanze/fbx/Materials/back2.mat", "Legacy Shaders/Particles/Additive");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("ChooseRole back materials converted to additive shader.");
        }

        [MenuItem("Tools/Diagnostics/ChooseRole/Disable texiao01")]
        public static void DisableTexiao01() => SetObjectActive("texiao/texiao01", false);

        [MenuItem("Tools/Diagnostics/ChooseRole/Disable object71")]
        public static void DisableObject71()
        {
            SetObjectActive("texiao/object71", false);
            SetObjectActive("texiao/object71 (1)", false);
        }

        [MenuItem("Tools/Diagnostics/ChooseRole/Disable Particle System 1")]
        public static void DisableParticleSystem1() => SetObjectActive("texiao/Particle System (1)", false);

        [MenuItem("Tools/Diagnostics/ChooseRole/Install Grass Overlay")]
        public static void InstallGrassOverlay()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var existing = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .FirstOrDefault(t => t.name == "ChooseRoleGrassOverlay");
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing.gameObject);
            }

            var shader = Shader.Find("Unlit/Transparent");
            if (shader == null)
            {
                Debug.LogError("ChooseRole grass overlay failed: missing Unlit/Transparent shader.");
                return;
            }

            const string materialPath = "Assets/SceneRes/Juese_xuanze/Terrain/ChooseRoleGrassOverlay.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                material = new Material(shader) { name = "ChooseRoleGrassOverlay" };
                AssetDatabase.CreateAsset(material, materialPath);
            }

            var grassTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SceneRes/Juese_xuanze/texture/ChooseRoleGrassOverlay.png");
            if (grassTexture == null)
            {
                Debug.LogError("ChooseRole grass overlay failed: missing ChooseRoleGrassOverlay.png");
                return;
            }

            material.shader = shader;
            material.mainTexture = grassTexture;
            material.color = Color.white;
            material.mainTextureScale = Vector2.one;
            EditorUtility.SetDirty(material);

            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "ChooseRoleGrassOverlay";
            plane.transform.position = new Vector3(0.2f, 0.9f, -6.2f);
            plane.transform.localScale = new Vector3(5.3f, 1f, 3.4f);
            var renderer = plane.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            plane.GetComponent<MeshCollider>().enabled = false;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("ChooseRole grass overlay installed.");
        }

        [MenuItem("Tools/Diagnostics/ChooseRole/Install Target Backdrop")]
        public static void InstallTargetBackdrop()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            SetRootObjectActive(scene, "map", false);
            SetRootObjectActive(scene, "Terrain", false);
            SetRootObjectActive(scene, "texiao", false);

            var existingBackdrops = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .Where(t => t.name == "ChooseRoleTargetBackdrop" || t.name == "ChooseRoleTargetBackdrop_Back")
                .Select(t => t.gameObject)
                .ToArray();
            foreach (var existing in existingBackdrops)
            {
                UnityEngine.Object.DestroyImmediate(existing);
            }

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(BackdropTexturePath);
            if (texture == null)
            {
                Debug.LogError($"ChooseRole target backdrop failed: missing {BackdropTexturePath}");
                return;
            }

            var shader = Shader.Find("Unlit/Texture");
            if (shader == null)
            {
                Debug.LogError("ChooseRole target backdrop failed: missing Unlit/Texture shader.");
                return;
            }

            var material = AssetDatabase.LoadAssetAtPath<Material>(BackdropMaterialPath);
            if (material == null)
            {
                material = new Material(shader) { name = "ChooseRoleTargetBackdrop" };
                AssetDatabase.CreateAsset(material, BackdropMaterialPath);
            }

            material.shader = shader;
            material.mainTexture = texture;
            EditorUtility.SetDirty(material);

            const float fieldOfView = 45f;
            const float cameraZ = 25f;
            const float backdropZ = -12f;
            float distance = cameraZ - backdropZ;
            float aspect = texture.width / (float)texture.height;
            float height = 2f * distance * Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);
            float width = height * aspect;

            CreateBackdropQuad("ChooseRoleTargetBackdrop", material, new Vector3(-5f, 7f, backdropZ), Quaternion.identity, new Vector3(width, height, 1f));
            CreateBackdropQuad("ChooseRoleTargetBackdrop_Back", material, new Vector3(-5f, 7f, backdropZ + 0.01f), Quaternion.Euler(0f, 180f, 0f), new Vector3(width, height, 1f));

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("ChooseRole target backdrop installed.");
        }

        private static void ProbeSlot(int slotIndex)
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var target = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .FirstOrDefault(t => t.name == "cj_mod" && t.parent != null && t.parent.name == "map");

            if (target == null)
            {
                Debug.LogError("ChooseRole material probe: map/cj_mod not found.");
                return;
            }

            var renderer = target.GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("ChooseRole material probe: Renderer missing on map/cj_mod.");
                return;
            }

            var materials = renderer.sharedMaterials.ToArray();
            if (slotIndex < 0 || slotIndex >= materials.Length)
            {
                Debug.LogError($"ChooseRole material probe: slot {slotIndex} out of range, count={materials.Length}");
                return;
            }

            var probeMaterial = AssetDatabase.LoadAssetAtPath<Material>(ProbeMaterialPath);
            if (probeMaterial == null)
            {
                probeMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                {
                    color = Color.magenta,
                    name = "ChooseRoleProbeMagenta"
                };
                AssetDatabase.CreateAsset(probeMaterial, ProbeMaterialPath);
                AssetDatabase.SaveAssets();
            }

            materials[slotIndex] = probeMaterial;
            renderer.sharedMaterials = materials;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"ChooseRole material probe: slot {slotIndex} replaced with probe material.");
        }

        private static void ConvertMaterialShader(string materialPath, string shaderName)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                Debug.LogError($"ChooseRole material conversion failed: missing {materialPath}");
                return;
            }

            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"ChooseRole material conversion failed: missing shader {shaderName}");
                return;
            }

            material.shader = shader;
            EditorUtility.SetDirty(material);
        }

        private static void SetObjectActive(string objectPath, bool active)
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var target = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .FirstOrDefault(t => string.Equals(BuildPath(t), objectPath, StringComparison.Ordinal));

            if (target == null)
            {
                Debug.LogError($"ChooseRole object toggle failed: missing {objectPath}");
                return;
            }

            target.gameObject.SetActive(active);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"ChooseRole object toggle: {objectPath} active={active}");
        }

        private static void SetRootObjectActive(Scene scene, string objectName, bool active)
        {
            var target = scene.GetRootGameObjects().FirstOrDefault(go => string.Equals(go.name, objectName, StringComparison.Ordinal));
            if (target == null)
            {
                return;
            }

            target.SetActive(active);
        }

        private static void CreateBackdropQuad(string objectName, Material material, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = objectName;
            quad.transform.position = position;
            quad.transform.rotation = rotation;
            quad.transform.localScale = scale;
            quad.GetComponent<MeshRenderer>().sharedMaterial = material;
            quad.GetComponent<Collider>().enabled = false;
        }

        private static string BuildPath(Transform transform)
        {
            var segments = new System.Collections.Generic.Stack<string>();
            var current = transform;
            while (current != null)
            {
                segments.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", segments);
        }
    }
}
