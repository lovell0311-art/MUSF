using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ETEditor
{
    public static class ChooseRolePreviewUtility
    {
        private const string ScenePath = "Assets/Scenes/GameMap/ChooseRole.unity";
        private const string PreviewRootName = "__ChooseRolePlacementPreview__";
        private const string BackdropRootName = "__ChooseRolePlacementBackdrop__";
        private const string ConfigAssetPath = "Assets/Editor/Diagnostics/ChooseRolePlacementConfig.asset";
        private static readonly string[] HiddenSceneRootNames = { "map", "Terrain", "texiao" };

        [MenuItem("Tools/Diagnostics/Refresh ChooseRole Preview Roles")]
        public static void RefreshPreviewMenu()
        {
            ChooseRolePlacementConfig config = AssetDatabase.LoadAssetAtPath<ChooseRolePlacementConfig>(ConfigAssetPath);
            if (config == null)
            {
                Debug.LogError($"ChooseRolePreviewUtility: config asset not found at {ConfigAssetPath}");
                return;
            }

            config.EnsureInitialized();
            RefreshPreview(config);
        }

        [MenuItem("Tools/Diagnostics/Render ChooseRole Preview Screenshot")]
        public static void RenderPreviewScreenshotMenu()
        {
            ChooseRolePlacementConfig config = AssetDatabase.LoadAssetAtPath<ChooseRolePlacementConfig>(ConfigAssetPath);
            if (config == null)
            {
                Debug.LogError($"ChooseRolePreviewUtility: config asset not found at {ConfigAssetPath}");
                return;
            }

            config.EnsureInitialized();
            string output = RenderPreviewScreenshot(config);
            Debug.Log($"ChooseRole preview rendered: {output}");
        }

        [MenuItem("Tools/Diagnostics/Restore ChooseRole Scene Background")]
        public static void RestoreSceneBackgroundMenu()
        {
            RestoreSceneBackground();
        }

        public static GameObject RefreshPreview(ChooseRolePlacementConfig config)
        {
            if (config == null)
            {
                return null;
            }

            EnsureChooseRoleSceneOpen();
            RefreshCalibrationBackdrop(config);
            ClearPreview();

            GameObject root = new GameObject(PreviewRootName);
            root.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
            Undo.RegisterCreatedObjectUndo(root, "Create ChooseRole Preview Root");

            for (int i = 0; i < config.PreviewRoles.Length; ++i)
            {
                CreatePreviewRole(root.transform, config, config.PreviewRoles[i], i);
            }

            Selection.activeGameObject = root;
            FocusSceneOnCalibration(config);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            return root;
        }

        public static void ClearPreview()
        {
            GameObject existing = GameObject.Find(PreviewRootName);
            if (existing == null)
            {
                return;
            }

            Object.DestroyImmediate(existing);
        }

        public static void RefreshCalibrationBackdrop(ChooseRolePlacementConfig config)
        {
            EnsureChooseRoleSceneOpen();
            SetOriginalSceneRootsHidden(true);
            CreateCalibrationBackdrop(config);
        }

        public static void RestoreSceneBackground()
        {
            SetOriginalSceneRootsHidden(false);

            GameObject existing = GameObject.Find(BackdropRootName);
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }
        }

        public static string RenderPreviewScreenshot(ChooseRolePlacementConfig config)
        {
            RefreshPreview(config);

            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? "F:/MUSF/Client/Unity";
            string reportDir = Path.Combine(projectRoot, "reports", "choose-role-preview");
            Directory.CreateDirectory(reportDir);
            string outputPath = Path.Combine(reportDir, $"choose-role-preview-{System.DateTime.Now:yyyyMMdd-HHmmss}.png");

            GameObject cameraObject = new GameObject("__ChooseRolePreviewCamera__");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.transform.position = new Vector3(-5f, 7f, 25f);
            camera.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            camera.fieldOfView = 45f;
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.allowHDR = false;
            camera.allowMSAA = false;

            RenderTexture renderTexture = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
            Texture2D texture = new Texture2D(1920, 1080, TextureFormat.RGBA32, false);

            try
            {
                camera.targetTexture = renderTexture;
                RenderTexture current = RenderTexture.active;
                RenderTexture.active = renderTexture;
                camera.Render();
                texture.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
                texture.Apply(false, false);
                File.WriteAllBytes(outputPath, texture.EncodeToPNG());
                RenderTexture.active = current;
            }
            finally
            {
                camera.targetTexture = null;
                Object.DestroyImmediate(renderTexture);
                Object.DestroyImmediate(texture);
                Object.DestroyImmediate(cameraObject);
            }

            AssetDatabase.Refresh();
            return outputPath;
        }

        public static void EnsureChooseRoleSceneOpen()
        {
            if (EditorSceneManager.GetActiveScene().path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }
        }

        private static void FocusSceneOnCalibration(ChooseRolePlacementConfig config)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null || config == null || config.TargetPedestals == null || config.TargetPedestals.Length == 0)
            {
                return;
            }

            float minX = config.TargetPedestals[0].Position.x;
            float maxX = config.TargetPedestals[0].Position.x;
            float minZ = config.TargetPedestals[0].Position.z;
            float maxZ = config.TargetPedestals[0].Position.z;
            for (int i = 1; i < config.TargetPedestals.Length; ++i)
            {
                Vector3 position = config.TargetPedestals[i].Position;
                minX = Mathf.Min(minX, position.x);
                maxX = Mathf.Max(maxX, position.x);
                minZ = Mathf.Min(minZ, position.z);
                maxZ = Mathf.Max(maxZ, position.z);
            }

            if (config.Pedestals != null)
            {
                for (int i = 0; i < config.Pedestals.Length; ++i)
                {
                    Vector3 position = config.Pedestals[i].Position;
                    minX = Mathf.Min(minX, position.x);
                    maxX = Mathf.Max(maxX, position.x);
                    minZ = Mathf.Min(minZ, position.z);
                    maxZ = Mathf.Max(maxZ, position.z);
                }
            }

            Vector3 pivot = new Vector3((minX + maxX) * 0.5f, config.TargetPedestals[0].Position.y + 1.6f, (minZ + maxZ) * 0.5f - 0.2f);
            float size = Mathf.Max(10f, (maxX - minX) * 1.18f);
            sceneView.orthographic = false;
            sceneView.LookAt(pivot, Quaternion.Euler(16f, 180f, 0f), size, false, false);
            sceneView.Repaint();
        }

        private static void SetOriginalSceneRootsHidden(bool hidden)
        {
            SceneVisibilityManager visibilityManager = SceneVisibilityManager.instance;
            if (visibilityManager == null)
            {
                return;
            }

            for (int i = 0; i < HiddenSceneRootNames.Length; ++i)
            {
                GameObject root = GameObject.Find(HiddenSceneRootNames[i]);
                if (root == null)
                {
                    continue;
                }

                if (hidden)
                {
                    visibilityManager.Hide(root, true);
                }
                else
                {
                    visibilityManager.Show(root, true);
                }
            }
        }

        private static void CreateCalibrationBackdrop(ChooseRolePlacementConfig config)
        {
            GameObject existing = GameObject.Find(BackdropRootName);
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject root = new GameObject(BackdropRootName);
            root.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

            Material iceMaterial = CreatePreviewMaterial("ChooseRoleBackdropIce", new Color(0.74f, 0.89f, 1f, 1f), 0.05f, 0.95f, new Color(0.15f, 0.45f, 0.8f, 0.45f));
            Material glowMaterial = CreatePreviewMaterial("ChooseRoleBackdropGlow", new Color(0.35f, 0.85f, 1f, 0.92f), 0f, 0.7f, new Color(0.2f, 0.9f, 1f, 1.35f));
            Material snowMaterial = CreatePreviewMaterial("ChooseRoleBackdropSnow", new Color(0.95f, 0.97f, 1f, 1f), 0f, 0.6f, new Color(0.1f, 0.15f, 0.25f, 0f));

            Vector3 center = ComputePedestalCenter(config.TargetPedestals);
            float minX = config.TargetPedestals[0].Position.x;
            float maxX = config.TargetPedestals[0].Position.x;
            float minZ = config.TargetPedestals[0].Position.z;
            float maxZ = config.TargetPedestals[0].Position.z;
            for (int i = 1; i < config.TargetPedestals.Length; ++i)
            {
                Vector3 position = config.TargetPedestals[i].Position;
                minX = Mathf.Min(minX, position.x);
                maxX = Mathf.Max(maxX, position.x);
                minZ = Mathf.Min(minZ, position.z);
                maxZ = Mathf.Max(maxZ, position.z);
            }

            float spanX = Mathf.Max(8f, (maxX - minX) + 6f);
            float spanZ = Mathf.Max(6f, (maxZ - minZ) + 3f);
            float platformTopY = center.y - 0.18f;

            CreateCube(root.transform, "Backdrop_BasePlate", new Vector3(center.x, platformTopY - 1.35f, center.z + 0.15f), new Vector3(spanX, 0.55f, spanZ), iceMaterial);
            CreateCube(root.transform, "Backdrop_RearCliff", new Vector3(center.x + 0.2f, platformTopY + 2.0f, center.z - 2.7f), new Vector3(spanX * 0.48f, 3.0f, 1.7f), iceMaterial);
            CreateCube(root.transform, "Backdrop_LeftShelf", new Vector3(center.x - 5.1f, platformTopY + 0.55f, center.z - 1.35f), new Vector3(2.6f, 1.2f, 1.6f), snowMaterial);
            CreateCube(root.transform, "Backdrop_RightShelf", new Vector3(center.x + 5.4f, platformTopY + 0.4f, center.z - 1.15f), new Vector3(2.8f, 1.0f, 1.7f), snowMaterial);
            CreateCube(root.transform, "Backdrop_RightBridge", new Vector3(center.x + 7.4f, platformTopY + 0.15f, center.z + 1.05f), new Vector3(2.5f, 0.45f, 2.8f), iceMaterial);
            CreateCube(root.transform, "Backdrop_LeftBridge", new Vector3(center.x - 7.1f, platformTopY + 0.15f, center.z + 0.8f), new Vector3(2.3f, 0.45f, 2.6f), iceMaterial);
            CreateCube(root.transform, "Backdrop_Arch_L", new Vector3(center.x - 1.8f, platformTopY + 3.5f, center.z - 2.9f), new Vector3(0.45f, 3.1f, 0.55f), glowMaterial);
            CreateCube(root.transform, "Backdrop_Arch_R", new Vector3(center.x + 1.8f, platformTopY + 3.5f, center.z - 2.9f), new Vector3(0.45f, 3.1f, 0.55f), glowMaterial);
            CreateCube(root.transform, "Backdrop_Arch_Top", new Vector3(center.x, platformTopY + 5.25f, center.z - 2.9f), new Vector3(4.1f, 0.45f, 0.65f), glowMaterial);

            for (int i = 0; i < config.TargetPedestals.Length; ++i)
            {
                CreatePedestal(root.transform, config.TargetPedestals[i], i, iceMaterial, glowMaterial);
            }
        }

        private static Vector3 ComputePedestalCenter(ChooseRolePedestalSlot[] pedestals)
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < pedestals.Length; ++i)
            {
                sum += pedestals[i].Position;
            }

            return sum / Mathf.Max(1, pedestals.Length);
        }

        private static void CreatePedestal(Transform root, ChooseRolePedestalSlot pedestal, int index, Material iceMaterial, Material glowMaterial)
        {
            float columnHeight = 1.35f;
            float columnRadius = 0.52f;

            Vector3 columnCenter = pedestal.Position + Vector3.down * (columnHeight * 0.5f);
            GameObject column = CreateCylinder(root, $"Pedestal_{pedestal.Label}_Column", columnCenter, new Vector3(columnRadius, columnHeight * 0.5f, columnRadius), iceMaterial);
            column.transform.rotation = Quaternion.Euler(0f, pedestal.Yaw, 0f);

            Vector3 capCenter = pedestal.Position + Vector3.down * 0.06f;
            GameObject cap = CreateCylinder(root, $"Pedestal_{pedestal.Label}_Cap", capCenter, new Vector3(columnRadius * 1.32f, 0.07f, columnRadius * 1.32f), glowMaterial);
            cap.transform.rotation = Quaternion.Euler(0f, pedestal.Yaw, 0f);

            Vector3 haloCenter = pedestal.Position + Vector3.up * 0.015f;
            GameObject halo = CreateCylinder(root, $"Pedestal_{pedestal.Label}_Halo", haloCenter, new Vector3(columnRadius * 1.32f, 0.015f, columnRadius * 1.32f), glowMaterial);
            halo.transform.rotation = Quaternion.Euler(90f, pedestal.Yaw, 0f);
            Collider haloCollider = halo.GetComponent<Collider>();
            if (haloCollider != null)
            {
                Object.DestroyImmediate(haloCollider);
            }

            CreateCube(root, $"Pedestal_{pedestal.Label}_Label", pedestal.Position + new Vector3(0f, 0.16f, 0f), new Vector3(0.1f, 0.02f, 0.1f), glowMaterial);
        }

        private static GameObject CreateCube(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
            cube.transform.SetParent(parent, false);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            ApplyMaterial(cube, material);
            return cube;
        }

        private static GameObject CreateCylinder(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            cylinder.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
            cylinder.transform.SetParent(parent, false);
            cylinder.transform.position = position;
            cylinder.transform.localScale = scale;
            ApplyMaterial(cylinder, material);
            return cylinder;
        }

        private static void ApplyMaterial(GameObject gameObject, Material material)
        {
            if (gameObject == null || material == null)
            {
                return;
            }

            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private static Material CreatePreviewMaterial(string name, Color color, float metallic, float glossiness, Color emission)
        {
            Shader shader = Shader.Find("Unlit/Color");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Unlit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            Material material = new Material(shader)
            {
                name = name,
                hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild
            };

            if (material.HasProperty("_Color"))
            {
                material.color = color;
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", metallic);
            }

            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", glossiness);
            }

            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", glossiness);
            }

            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", emission);
            }

            return material;
        }

        private static void CreatePreviewRole(Transform root, ChooseRolePlacementConfig config, ChooseRolePreviewRole previewRole, int index)
        {
            if (previewRole.PedestalIndex < 0 || previewRole.PedestalIndex >= config.Pedestals.Length)
            {
                Debug.LogWarning($"ChooseRolePreviewUtility skipped {previewRole.Label}: invalid pedestal index {previewRole.PedestalIndex}");
                return;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(previewRole.PrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"ChooseRolePreviewUtility skipped {previewRole.Label}: prefab not found at {previewRole.PrefabPath}");
                return;
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
            {
                Debug.LogWarning($"ChooseRolePreviewUtility failed to instantiate {previewRole.PrefabPath}");
                return;
            }

            instance.name = $"Preview_{index}_{previewRole.Label}";
            instance.transform.SetParent(root, false);
            Vector3 authoredRootOffset = instance.transform.localPosition;
            Quaternion authoredRootRotation = instance.transform.localRotation;

            ChooseRolePedestalSlot pedestal = config.Pedestals[previewRole.PedestalIndex];
            instance.transform.position = pedestal.Position;
            instance.transform.rotation = Quaternion.Euler(0f, pedestal.Yaw, 0f);
            instance.transform.localScale = Vector3.one * previewRole.Scale;
            instance.transform.position += instance.transform.rotation * authoredRootOffset;
            instance.transform.rotation = instance.transform.rotation * authoredRootRotation;

            if (instance.transform.childCount > 0)
            {
                Transform modelRoot = instance.transform.GetChild(0);
                modelRoot.localPosition = previewRole.ModelOffset;
                modelRoot.localRotation = Quaternion.Euler(0f, previewRole.ModelYaw, 0f);
            }

            SnapPreviewRoleToPedestal(instance, pedestal.Position);
        }

        private static void SnapPreviewRoleToPedestal(GameObject instance, Vector3 pedestalPosition)
        {
            if (instance == null)
            {
                return;
            }

            if (!TryGetPreviewBounds(instance, out Bounds bounds))
            {
                return;
            }

            Vector3 footPoint = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            Vector3 delta = pedestalPosition - footPoint;
            if (delta.sqrMagnitude <= 0.000001f)
            {
                return;
            }

            instance.transform.position += delta;
        }

        private static bool TryGetPreviewBounds(GameObject instance, out Bounds bounds)
        {
            bounds = default;
            if (instance == null)
            {
                return false;
            }

            Transform root = instance.transform.childCount > 0 ? instance.transform.GetChild(0) : instance.transform;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                return false;
            }

            bool hasBounds = false;
            Vector3 min = Vector3.zero;
            Vector3 max = Vector3.zero;
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || !renderer.enabled)
                {
                    continue;
                }

                Bounds rendererBounds = renderer.bounds;
                if (!hasBounds)
                {
                    min = rendererBounds.min;
                    max = rendererBounds.max;
                    hasBounds = true;
                    continue;
                }

                min = Vector3.Min(min, rendererBounds.min);
                max = Vector3.Max(max, rendererBounds.max);
            }

            if (!hasBounds)
            {
                return false;
            }

            bounds = new Bounds((min + max) * 0.5f, max - min);
            return true;
        }
    }
}
