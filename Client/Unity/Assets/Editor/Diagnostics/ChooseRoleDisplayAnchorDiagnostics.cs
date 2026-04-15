using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class ChooseRoleDisplayAnchorDiagnostics
    {
        private const string OutputRelativePath = "reports/choose-role-preview/choose-role-display-anchor-offsets.json";

        [Serializable]
        private struct RoleProbeDefinition
        {
            public string Label;
            public string ResName;
            public string PrefabPath;
        }

        [Serializable]
        private sealed class AnchorReport
        {
            public string generatedAt;
            public AnchorEntry[] entries;
        }

        [Serializable]
        private sealed class AnchorEntry
        {
            public string label;
            public string resName;
            public string prefabPath;
            public string pelvisName;
            public string leftFootName;
            public string rightFootName;
            public SerializableVector3 pelvisOffset;
            public SerializableVector3 footOffset;
            public SerializableVector3 displayAnchorOffset;
        }

        [Serializable]
        private struct SerializableVector3
        {
            public float x;
            public float y;
            public float z;

            public SerializableVector3(Vector3 value)
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        private static readonly RoleProbeDefinition[] ProbeDefinitions =
        {
            new RoleProbeDefinition { Label = "Archer", ResName = "Role_Archer", PrefabPath = "Assets/Bundles/Roles/Role_Archer.prefab" },
            new RoleProbeDefinition { Label = "Holymentor", ResName = "Role_Holymentor", PrefabPath = "Assets/Bundles/Roles/Role_Holymentor.prefab" },
            new RoleProbeDefinition { Label = "Gladiator", ResName = "Role_Gladiator", PrefabPath = "Assets/Bundles/Roles/Role_Gladiator.prefab" },
            new RoleProbeDefinition { Label = "GrowLancer", ResName = "Role_GrowLancer", PrefabPath = "Assets/Bundles/Roles/Role_GrowLancer.prefab" },
            new RoleProbeDefinition { Label = "Summoner", ResName = "Role_Summoner", PrefabPath = "Assets/Bundles/Roles/Role_Summoner.prefab" },
        };

        [MenuItem("Tools/Diagnostics/Dump ChooseRole Display Anchors")]
        public static void DumpDisplayAnchorsMenu()
        {
            string outputPath = DumpDisplayAnchors();
            Debug.Log($"ChooseRoleDisplayAnchorDiagnostics wrote: {outputPath}");
        }

        public static string DumpDisplayAnchors()
        {
            List<AnchorEntry> entries = new List<AnchorEntry>(ProbeDefinitions.Length);
            for (int i = 0; i < ProbeDefinitions.Length; ++i)
            {
                entries.Add(ProbeRole(ProbeDefinitions[i]));
            }

            AnchorReport report = new AnchorReport
            {
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                entries = entries.ToArray(),
            };

            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? "F:/MUSF/Client/Unity";
            string outputPath = Path.Combine(projectRoot, OutputRelativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? projectRoot);
            File.WriteAllText(outputPath, JsonUtility.ToJson(report, true));
            AssetDatabase.Refresh();
            return outputPath;
        }

        private static AnchorEntry ProbeRole(RoleProbeDefinition definition)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(definition.PrefabPath);
            if (prefab == null)
            {
                throw new InvalidOperationException($"ChooseRoleDisplayAnchorDiagnostics: prefab not found at {definition.PrefabPath}");
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
            {
                throw new InvalidOperationException($"ChooseRoleDisplayAnchorDiagnostics: failed to instantiate {definition.PrefabPath}");
            }

            try
            {
                instance.name = $"DisplayAnchorProbe_{definition.Label}";
                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;

                Transform pelvis = FindBone(instance.transform, "pelvis");
                Transform leftFoot = FindBone(instance.transform, "l foot", "leftfoot", "left_foot");
                Transform rightFoot = FindBone(instance.transform, "r foot", "rightfoot", "right_foot");

                if (pelvis == null)
                {
                    pelvis = FindBone(instance.transform, "hip", "hips");
                }

                if (pelvis == null)
                {
                    throw new InvalidOperationException($"ChooseRoleDisplayAnchorDiagnostics: pelvis bone not found for {definition.ResName}");
                }

                Vector3 footWorld;
                if (leftFoot != null && rightFoot != null)
                {
                    Vector3 left = leftFoot.position;
                    Vector3 right = rightFoot.position;
                    footWorld = new Vector3((left.x + right.x) * 0.5f, Mathf.Min(left.y, right.y), (left.z + right.z) * 0.5f);
                }
                else
                {
                    Transform fallbackFoot = leftFoot ?? rightFoot ?? FindBone(instance.transform, "foot");
                    if (fallbackFoot == null)
                    {
                        throw new InvalidOperationException($"ChooseRoleDisplayAnchorDiagnostics: foot bone not found for {definition.ResName}");
                    }

                    footWorld = fallbackFoot.position;
                }

                Vector3 pelvisOffset = pelvis.position - instance.transform.position;
                Vector3 footOffset = footWorld - instance.transform.position;
                Vector3 displayAnchor = new Vector3(pelvis.position.x, footWorld.y, pelvis.position.z) - instance.transform.position;

                return new AnchorEntry
                {
                    label = definition.Label,
                    resName = definition.ResName,
                    prefabPath = definition.PrefabPath,
                    pelvisName = pelvis.name,
                    leftFootName = leftFoot?.name ?? string.Empty,
                    rightFootName = rightFoot?.name ?? string.Empty,
                    pelvisOffset = new SerializableVector3(pelvisOffset),
                    footOffset = new SerializableVector3(footOffset),
                    displayAnchorOffset = new SerializableVector3(displayAnchor),
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

        private static Transform FindBone(Transform root, params string[] fragments)
        {
            if (root == null || fragments == null || fragments.Length == 0)
            {
                return null;
            }

            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; ++i)
            {
                Transform current = transforms[i];
                if (current == null || string.IsNullOrEmpty(current.name))
                {
                    continue;
                }

                string lowered = current.name.ToLowerInvariant();
                bool matched = true;
                for (int j = 0; j < fragments.Length; ++j)
                {
                    if (!lowered.Contains(fragments[j]))
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    return current;
                }
            }

            for (int i = 0; i < transforms.Length; ++i)
            {
                Transform current = transforms[i];
                if (current == null || string.IsNullOrEmpty(current.name))
                {
                    continue;
                }

                string lowered = current.name.ToLowerInvariant();
                for (int j = 0; j < fragments.Length; ++j)
                {
                    if (lowered.Contains(fragments[j]))
                    {
                        return current;
                    }
                }
            }

            return null;
        }
    }
}
