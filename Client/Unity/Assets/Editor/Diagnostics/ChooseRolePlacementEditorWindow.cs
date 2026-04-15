using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ETEditor
{
    public sealed class ChooseRolePlacementEditorWindow : EditorWindow
    {
        private const string ScenePath = "Assets/Scenes/GameMap/ChooseRole.unity";
        private const string AssetPath = "Assets/Editor/Diagnostics/ChooseRolePlacementConfig.asset";
        private const string HotfixSourcePath = "Assets/Hotfix/Miracle_MU/UI/UIChooseRole/UIChooseRoleComponent.cs";

        private static readonly Color[] PedestalColors =
        {
            new Color(1f, 0.78f, 0f, 1f),
            new Color(0.15f, 1f, 1f, 1f),
            new Color(0.35f, 1f, 0.35f, 1f),
            new Color(1f, 0.45f, 0.45f, 1f),
            new Color(0.8f, 0.6f, 1f, 1f),
        };

        private ChooseRolePlacementConfig config;
        private Vector2 scroll;
        private bool handlesEnabled = true;
        private static bool openWorkspacePending;
        private static bool openScenePending;

        [MenuItem("Tools/Diagnostics/ChooseRole Placement Editor")]
        public static void OpenWindow()
        {
            ChooseRolePlacementEditorWindow window = GetWindow<ChooseRolePlacementEditorWindow>("ChooseRole Placement");
            window.minSize = new Vector2(460f, 420f);
            window.Show();
        }

        [MenuItem("Tools/Diagnostics/Open ChooseRole Placement Workspace")]
        public static void OpenWorkspace()
        {
            openWorkspacePending = true;
            RequestOpenSceneWorkflow();
        }

        private void OnEnable()
        {
            config = LoadOrCreateConfig();
            SceneView.duringSceneGui += OnSceneGui;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGui;
        }

        private void OnGUI()
        {
            config = LoadOrCreateConfig();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("ChooseRole Pedestal Placement", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Open the ChooseRole scene, drag the five pedestal anchors in Scene view, then save the config. After you finish adjusting, we can write the values back into the hotfix source.",
                MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(EditorApplication.isCompiling))
                {
                    if (GUILayout.Button("Open ChooseRole Scene", GUILayout.Height(28f)))
                    {
                        openScenePending = true;
                        RequestOpenSceneWorkflow();
                    }
                }

                if (GUILayout.Button("Reset Defaults", GUILayout.Height(28f)))
                {
                    Undo.RecordObject(config, "Reset ChooseRole Placement");
                    config.ResetToDefaults();
                    SaveConfig();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Load From Hotfix Source", GUILayout.Height(28f)))
                {
                    LoadFromHotfixSource();
                }

                if (GUILayout.Button("Save Config", GUILayout.Height(28f)))
                {
                    SaveConfig();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Runtime -> Target", GUILayout.Height(28f)))
                {
                    Undo.RecordObject(config, "Copy Runtime Pedestals To Target");
                    config.CopyPedestalsToTarget();
                    SaveConfig();
                    Repaint();
                    SceneView.RepaintAll();
                }

                if (GUILayout.Button("Target -> Runtime", GUILayout.Height(28f)))
                {
                    Undo.RecordObject(config, "Copy Target Pedestals To Runtime");
                    config.CopyTargetToPedestals();
                    SaveConfig();
                    Repaint();
                    SceneView.RepaintAll();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Apply To Hotfix Source", GUILayout.Height(28f)))
                {
                    ApplyToHotfixSource();
                }

                handlesEnabled = GUILayout.Toggle(handlesEnabled, "Scene Handles Enabled", "Button", GUILayout.Height(28f));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh Preview Roles", GUILayout.Height(28f)))
                {
                    ChooseRolePreviewUtility.RefreshPreview(config);
                }

                if (GUILayout.Button("Clear Preview Roles", GUILayout.Height(28f)))
                {
                    ChooseRolePreviewUtility.ClearPreview();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use Pillar Backdrop", GUILayout.Height(28f)))
                {
                    ChooseRolePreviewUtility.RefreshCalibrationBackdrop(config);
                }

                if (GUILayout.Button("Restore Forest Background", GUILayout.Height(28f)))
                {
                    ChooseRolePreviewUtility.RestoreSceneBackground();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Render Preview Screenshot", GUILayout.Height(28f)))
                {
                    string output = ChooseRolePreviewUtility.RenderPreviewScreenshot(config);
                    Debug.Log($"ChooseRole preview rendered: {output}");
                }
            }

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Config Asset", AssetPath);
            EditorGUILayout.LabelField("Hotfix Source", HotfixSourcePath);

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Editor is currently in Play Mode. Stop Play Mode first, or use the open-scene button and the tool will reopen the scene after Play Mode exits.",
                    MessageType.Warning);
            }

            EditorGUILayout.HelpBox(
                "Target Pedestals are the correct pillar locations. Runtime Pedestals are the current in-game spawn points. Drag the target pillars first, then use Target -> Runtime when you want the preview roles to snap over.",
                MessageType.None);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("Target Pedestals", EditorStyles.boldLabel);
            for (int i = 0; i < ChooseRolePlacementConfig.PedestalCount; ++i)
            {
                DrawPedestalEditor(i, true);
            }

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Runtime Pedestals", EditorStyles.boldLabel);
            for (int i = 0; i < ChooseRolePlacementConfig.PedestalCount; ++i)
            {
                DrawPedestalEditor(i, false);
            }

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Preview Roles", EditorStyles.boldLabel);
            for (int i = 0; i < config.PreviewRoles.Length; ++i)
            {
                DrawPreviewRoleEditor(i);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawPedestalEditor(int index, bool targetMode)
        {
            ChooseRolePedestalSlot[] pedestalArray = targetMode ? config.TargetPedestals : config.Pedestals;
            ChooseRolePedestalSlot slot = pedestalArray[index];
            GUI.color = PedestalColors[index % PedestalColors.Length];
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;

            EditorGUILayout.LabelField($"{(targetMode ? "Target" : "Runtime")} {slot.Label}", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            Vector3 position = EditorGUILayout.Vector3Field("Position", slot.Position);
            float yaw = EditorGUILayout.FloatField("Yaw", slot.Yaw);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(config, $"Change {(targetMode ? "Target" : "Runtime")} {slot.Label}");
                slot.Position = position;
                slot.Yaw = yaw;
                pedestalArray[index] = slot;
                if (targetMode)
                {
                    config.TargetPedestals = pedestalArray;
                }
                else
                {
                    config.Pedestals = pedestalArray;
                }
                EditorUtility.SetDirty(config);
                Repaint();
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }

        private void DrawPreviewRoleEditor(int index)
        {
            ChooseRolePreviewRole role = config.PreviewRoles[index];
            GUI.color = PedestalColors[index % PedestalColors.Length];
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.white;

            EditorGUILayout.LabelField(role.Label, EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            string label = EditorGUILayout.TextField("Label", role.Label);
            string prefabPath = EditorGUILayout.TextField("Prefab Path", role.PrefabPath);
            int pedestalIndex = EditorGUILayout.IntSlider("Pedestal Index", role.PedestalIndex, 0, ChooseRolePlacementConfig.PedestalCount - 1);
            Vector3 modelOffset = EditorGUILayout.Vector3Field("Model Offset", role.ModelOffset);
            float modelYaw = EditorGUILayout.FloatField("Model Yaw", role.ModelYaw);
            float scale = EditorGUILayout.FloatField("Scale", role.Scale);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(config, $"Change Preview Role {role.Label}");
                role.Label = label;
                role.PrefabPath = prefabPath;
                role.PedestalIndex = pedestalIndex;
                role.ModelOffset = modelOffset;
                role.ModelYaw = modelYaw;
                role.Scale = scale;
                config.PreviewRoles[index] = role;
                EditorUtility.SetDirty(config);
                Repaint();
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }

        private void OnSceneGui(SceneView sceneView)
        {
            if (!handlesEnabled || config == null)
            {
                return;
            }

            if (sceneView == null || sceneView.camera == null)
            {
                return;
            }

            for (int i = 0; i < config.TargetPedestals.Length; ++i)
            {
                Handles.color = PedestalColors[i % PedestalColors.Length];
                ChooseRolePedestalSlot targetSlot = config.TargetPedestals[i];
                ChooseRolePedestalSlot runtimeSlot = config.Pedestals[i];

                Handles.color = new Color(1f, 1f, 1f, 0.35f);
                Handles.DrawDottedLine(runtimeSlot.Position, targetSlot.Position, 6f);
                Handles.SphereHandleCap(0, runtimeSlot.Position, Quaternion.identity, 0.18f, EventType.Repaint);

                EditorGUI.BeginChangeCheck();
                Handles.color = PedestalColors[i % PedestalColors.Length];
                Vector3 newPosition = Handles.PositionHandle(targetSlot.Position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(config, $"Move {targetSlot.Label}");
                    targetSlot.Position = newPosition;
                    config.TargetPedestals[i] = targetSlot;
                    EditorUtility.SetDirty(config);
                    Repaint();
                }

                Vector3 labelPosition = targetSlot.Position + Vector3.up * 0.35f;
                Handles.Label(labelPosition, $"{targetSlot.Label} target\n{targetSlot.Position.x:0.00}, {targetSlot.Position.y:0.00}, {targetSlot.Position.z:0.00}\nYaw {targetSlot.Yaw:0.0}");
                Handles.ArrowHandleCap(0, targetSlot.Position, Quaternion.Euler(0f, targetSlot.Yaw, 0f), 0.9f, EventType.Repaint);
            }
        }

        private static ChooseRolePlacementConfig LoadOrCreateConfig()
        {
            ChooseRolePlacementConfig asset = AssetDatabase.LoadAssetAtPath<ChooseRolePlacementConfig>(AssetPath);
            if (asset != null)
            {
                asset.EnsureInitialized();
                return asset;
            }

            asset = CreateInstance<ChooseRolePlacementConfig>();
            asset.ResetToDefaults();
            AssetDatabase.CreateAsset(asset, AssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        private static void RequestOpenSceneWorkflow()
        {
            EditorApplication.delayCall -= ContinueOpenSceneWorkflow;
            EditorApplication.delayCall += ContinueOpenSceneWorkflow;
        }

        private static void ContinueOpenSceneWorkflow()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                RequestOpenSceneWorkflow();
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }

                RequestOpenSceneWorkflow();
                return;
            }

            if (!openScenePending && !openWorkspacePending)
            {
                return;
            }

            openScenePending = false;
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            SceneView sceneView = SceneView.lastActiveSceneView ?? GetWindow<SceneView>();
            if (sceneView != null)
            {
                sceneView.Show();
                sceneView.Focus();
            }

            if (openWorkspacePending)
            {
                openWorkspacePending = false;
                OpenWindow();
                ChooseRolePlacementConfig config = LoadOrCreateConfig();
                ChooseRolePreviewUtility.RefreshPreview(config);
            }
        }

        private void SaveConfig()
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void LoadFromHotfixSource()
        {
            string absolutePath = Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, HotfixSourcePath);
            if (!File.Exists(absolutePath))
            {
                Debug.LogError($"ChooseRolePlacementEditor: hotfix source not found: {absolutePath}");
                return;
            }

            string content = File.ReadAllText(absolutePath);
            Match positionsBlock = Regex.Match(
                content,
                @"private static readonly Vector3\[\] ChooseRolePedestalPositions =\s*\{(?<body>[\s\S]*?)\};",
                RegexOptions.Multiline);
            Match yawBlock = Regex.Match(
                content,
                @"private static readonly float\[\] ChooseRolePedestalYaw =\s*\{(?<body>[\s\S]*?)\};",
                RegexOptions.Multiline);

            if (!positionsBlock.Success || !yawBlock.Success)
            {
                Debug.LogError("ChooseRolePlacementEditor: failed to locate pedestal blocks in hotfix source.");
                return;
            }

            MatchCollection matches = Regex.Matches(
                positionsBlock.Groups["body"].Value,
                @"new Vector3\(([-0-9.]+)f,\s*([-0-9.]+)f,\s*([-0-9.]+)f\)");
            MatchCollection yawMatches = Regex.Matches(yawBlock.Groups["body"].Value, @"([-0-9.]+)f");
            if (matches.Count < ChooseRolePlacementConfig.PedestalCount || yawMatches.Count < ChooseRolePlacementConfig.PedestalCount)
            {
                Debug.LogError("ChooseRolePlacementEditor: failed to parse pedestal constants from hotfix source.");
                return;
            }

            Undo.RecordObject(config, "Load ChooseRole Placement From Source");
            for (int i = 0; i < ChooseRolePlacementConfig.PedestalCount; ++i)
            {
                config.Pedestals[i].Label = $"P{i}";
                config.Pedestals[i].Position = new Vector3(
                    ParseFloat(matches[i].Groups[1].Value),
                    ParseFloat(matches[i].Groups[2].Value),
                    ParseFloat(matches[i].Groups[3].Value));
                config.Pedestals[i].Yaw = ParseFloat(yawMatches[i].Groups[1].Value);
            }

            if (config.TargetPedestals == null || config.TargetPedestals.Length != ChooseRolePlacementConfig.PedestalCount)
            {
                config.CopyPedestalsToTarget();
            }

            SaveConfig();
            Repaint();
            SceneView.RepaintAll();
        }

        private void ApplyToHotfixSource()
        {
            string absolutePath = Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, HotfixSourcePath);
            if (!File.Exists(absolutePath))
            {
                Debug.LogError($"ChooseRolePlacementEditor: hotfix source not found: {absolutePath}");
                return;
            }

            string content = File.ReadAllText(absolutePath);
            string positionsBlock = BuildPositionsBlock();
            string yawBlock = BuildYawBlock();

            content = Regex.Replace(
                content,
                @"private static readonly Vector3\[\] ChooseRolePedestalPositions =\s*\{[\s\S]*?\};",
                positionsBlock,
                RegexOptions.Multiline);
            content = Regex.Replace(
                content,
                @"private static readonly float\[\] ChooseRolePedestalYaw =\s*\{[\s\S]*?\};",
                yawBlock,
                RegexOptions.Multiline);

            File.WriteAllText(absolutePath, content);
            AssetDatabase.Refresh();
            Debug.Log("ChooseRolePlacementEditor: applied config to hotfix source.");
        }

        private string BuildPositionsBlock()
        {
            return
                "        private static readonly Vector3[] ChooseRolePedestalPositions =\n" +
                "        {\n" +
                $"            new Vector3({config.Pedestals[0].Position.x:0.00}f, {config.Pedestals[0].Position.y:0.00}f, {config.Pedestals[0].Position.z:0.00}f),\n" +
                $"            new Vector3({config.Pedestals[1].Position.x:0.00}f, {config.Pedestals[1].Position.y:0.00}f, {config.Pedestals[1].Position.z:0.00}f),\n" +
                $"            new Vector3({config.Pedestals[2].Position.x:0.00}f, {config.Pedestals[2].Position.y:0.00}f, {config.Pedestals[2].Position.z:0.00}f),\n" +
                $"            new Vector3({config.Pedestals[3].Position.x:0.00}f, {config.Pedestals[3].Position.y:0.00}f, {config.Pedestals[3].Position.z:0.00}f),\n" +
                $"            new Vector3({config.Pedestals[4].Position.x:0.00}f, {config.Pedestals[4].Position.y:0.00}f, {config.Pedestals[4].Position.z:0.00}f),\n" +
                "        };";
        }

        private string BuildYawBlock()
        {
            return
                "        private static readonly float[] ChooseRolePedestalYaw =\n" +
                "        {\n" +
                $"            {config.Pedestals[0].Yaw:0.0}f,\n" +
                $"            {config.Pedestals[1].Yaw:0.0}f,\n" +
                $"            {config.Pedestals[2].Yaw:0.0}f,\n" +
                $"            {config.Pedestals[3].Yaw:0.0}f,\n" +
                $"            {config.Pedestals[4].Yaw:0.0}f,\n" +
                "        };";
        }

        private static float ParseFloat(string value)
        {
            if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsed))
            {
                return parsed;
            }

            return 0f;
        }
    }
}
