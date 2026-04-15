using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class ChooseRoleRoleFootOffsetDiagnostics
    {
        private const string OutputRelativePath = "reports/choose-role-preview/choose-role-role-foot-offsets.json";

        [Serializable]
        private struct RoleProbeDefinition
        {
            public string Label;
            public string ResName;
            public string PrefabPath;
        }

        [Serializable]
        private sealed class RoleFootOffsetReport
        {
            public string generatedAt;
            public RoleFootOffsetEntry[] entries;
            public string csharpSnippet;
        }

        [Serializable]
        private sealed class RoleFootOffsetEntry
        {
            public string label;
            public string resName;
            public string prefabPath;
            public string footSource;
            public string leftFootName;
            public string rightFootName;
            public SerializableVector3 authoredRootOffset;
            public SerializableVector3 authoredRootEuler;
            public SerializableVector3 unitFootOffset;
            public SerializableVector3 fiveRoleFootOffset;
            public SerializableVector3 boundsCenter;
            public SerializableVector3 boundsSize;
        }

        [Serializable]
        private struct SerializableVector3
        {
            public float x;
            public float y;
            public float z;

            public SerializableVector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public SerializableVector3(Vector3 value)
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        private static readonly RoleProbeDefinition[] ProbeDefinitions =
        {
            new RoleProbeDefinition
            {
                Label = "Archer",
                ResName = "Role_Archer",
                PrefabPath = "Assets/Bundles/Roles/Role_Archer.prefab",
            },
            new RoleProbeDefinition
            {
                Label = "Holymentor",
                ResName = "Role_Holymentor",
                PrefabPath = "Assets/Bundles/Roles/Role_Holymentor.prefab",
            },
            new RoleProbeDefinition
            {
                Label = "Gladiator",
                ResName = "Role_Gladiator",
                PrefabPath = "Assets/Bundles/Roles/Role_Gladiator.prefab",
            },
            new RoleProbeDefinition
            {
                Label = "GrowLancer",
                ResName = "Role_GrowLancer",
                PrefabPath = "Assets/Bundles/Roles/Role_GrowLancer.prefab",
            },
            new RoleProbeDefinition
            {
                Label = "Summoner",
                ResName = "Role_Summoner",
                PrefabPath = "Assets/Bundles/Roles/Role_Summoner.prefab",
            },
        };

        [MenuItem("Tools/Diagnostics/Dump ChooseRole Role Foot Offsets")]
        public static void DumpRoleFootOffsetsMenu()
        {
            string outputPath = DumpRoleFootOffsets();
            Debug.Log($"ChooseRoleRoleFootOffsetDiagnostics wrote: {outputPath}");
        }

        public static string DumpRoleFootOffsets()
        {
            List<RoleFootOffsetEntry> entries = new List<RoleFootOffsetEntry>(ProbeDefinitions.Length);

            for (int i = 0; i < ProbeDefinitions.Length; ++i)
            {
                entries.Add(ProbeRole(ProbeDefinitions[i]));
            }

            RoleFootOffsetReport report = new RoleFootOffsetReport
            {
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                entries = entries.ToArray(),
                csharpSnippet = BuildCSharpSnippet(entries),
            };

            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? "F:/MUSF/Client/Unity";
            string outputPath = Path.Combine(projectRoot, OutputRelativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? projectRoot);
            File.WriteAllText(outputPath, JsonUtility.ToJson(report, true));
            AssetDatabase.Refresh();
            return outputPath;
        }

        private static RoleFootOffsetEntry ProbeRole(RoleProbeDefinition definition)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(definition.PrefabPath);
            if (prefab == null)
            {
                throw new InvalidOperationException($"ChooseRoleRoleFootOffsetDiagnostics: prefab not found at {definition.PrefabPath}");
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
            {
                throw new InvalidOperationException($"ChooseRoleRoleFootOffsetDiagnostics: failed to instantiate {definition.PrefabPath}");
            }

            try
            {
                instance.name = $"Probe_{definition.Label}";
                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;

                Vector3 authoredRootOffset = instance.transform.localPosition;
                Vector3 authoredRootEuler = instance.transform.localEulerAngles;
                Quaternion authoredRootRotation = instance.transform.localRotation;

                // Match the runtime choose-role flow: the root gets placement rotation,
                // then the authored root rotation is restored, while child[0] is zeroed.
                instance.transform.rotation = Quaternion.identity * authoredRootRotation;
                if (instance.transform.childCount > 0)
                {
                    Transform modelRoot = instance.transform.GetChild(0);
                    modelRoot.localPosition = Vector3.zero;
                    modelRoot.localRotation = Quaternion.identity;
                }

                Bounds bounds = GetCombinedRendererBounds(instance);
                Vector3 footOffset = TryGetFootFromBones(instance, out Vector3 boneFoot, out string leftFootName, out string rightFootName)
                    ? boneFoot - instance.transform.position
                    : new Vector3(bounds.center.x, bounds.min.y, bounds.center.z) - instance.transform.position;
                string footSource = string.IsNullOrEmpty(leftFootName) && string.IsNullOrEmpty(rightFootName) ? "bounds" : "bones";

                return new RoleFootOffsetEntry
                {
                    label = definition.Label,
                    resName = definition.ResName,
                    prefabPath = definition.PrefabPath,
                    footSource = footSource,
                    leftFootName = leftFootName ?? string.Empty,
                    rightFootName = rightFootName ?? string.Empty,
                    authoredRootOffset = new SerializableVector3(authoredRootOffset),
                    authoredRootEuler = new SerializableVector3(authoredRootEuler),
                    unitFootOffset = new SerializableVector3(footOffset),
                    fiveRoleFootOffset = new SerializableVector3(footOffset * 0.88f),
                    boundsCenter = new SerializableVector3(bounds.center),
                    boundsSize = new SerializableVector3(bounds.size),
                };
            }
            finally
            {
                if (instance != null)
                {
                    UnityEngine.Object.DestroyImmediate(instance);
                }
            }
        }

        private static bool TryGetFootFromBones(GameObject instance, out Vector3 footWorld, out string leftFootName, out string rightFootName)
        {
            footWorld = Vector3.zero;
            leftFootName = string.Empty;
            rightFootName = string.Empty;

            Transform[] transforms = instance.GetComponentsInChildren<Transform>(true);
            if (transforms == null || transforms.Length == 0)
            {
                return false;
            }

            Transform leftFoot = null;
            Transform rightFoot = null;
            Transform anyFoot = null;
            for (int i = 0; i < transforms.Length; ++i)
            {
                Transform current = transforms[i];
                if (current == null)
                {
                    continue;
                }

                string lowered = current.name.ToLowerInvariant();
                if (lowered.Contains("l foot") || lowered.Contains("leftfoot") || lowered.Contains("left_foot"))
                {
                    leftFoot = current;
                    continue;
                }

                if (lowered.Contains("r foot") || lowered.Contains("rightfoot") || lowered.Contains("right_foot"))
                {
                    rightFoot = current;
                    continue;
                }

                if (anyFoot == null && lowered.Contains("foot"))
                {
                    anyFoot = current;
                }
            }

            if (leftFoot != null && rightFoot != null)
            {
                Vector3 left = leftFoot.position;
                Vector3 right = rightFoot.position;
                footWorld = new Vector3((left.x + right.x) * 0.5f, Mathf.Min(left.y, right.y), (left.z + right.z) * 0.5f);
                leftFootName = leftFoot.name;
                rightFootName = rightFoot.name;
                return true;
            }

            Transform selected = leftFoot ?? rightFoot ?? anyFoot;
            if (selected == null)
            {
                return false;
            }

            footWorld = selected.position;
            leftFootName = selected.name;
            rightFootName = string.Empty;
            return true;
        }

        private static Bounds GetCombinedRendererBounds(GameObject instance)
        {
            Transform root = instance.transform.childCount > 0 ? instance.transform.GetChild(0) : instance.transform;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                return new Bounds(instance.transform.position, Vector3.zero);
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

            return hasBounds ? new Bounds((min + max) * 0.5f, max - min) : new Bounds(instance.transform.position, Vector3.zero);
        }

        private static string BuildCSharpSnippet(List<RoleFootOffsetEntry> entries)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("private static readonly Dictionary<string, Vector3> ChooseRoleUnitFootOffsets = new Dictionary<string, Vector3>");
            builder.AppendLine("{");
            for (int i = 0; i < entries.Count; ++i)
            {
                RoleFootOffsetEntry entry = entries[i];
                builder.AppendLine(
                    $"    [\"{entry.resName}\"] = new Vector3({entry.unitFootOffset.x:0.0000}f, {entry.unitFootOffset.y:0.0000}f, {entry.unitFootOffset.z:0.0000}f),");
            }

            builder.AppendLine("};");
            return builder.ToString();
        }
    }
}
