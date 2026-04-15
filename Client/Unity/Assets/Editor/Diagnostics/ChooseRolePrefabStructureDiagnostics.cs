using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class ChooseRolePrefabStructureDiagnostics
    {
        private const string OutputRelativePath = "reports/choose-role-preview/choose-role-prefab-structure.json";

        [Serializable]
        private struct RoleProbeDefinition
        {
            public string Label;
            public string ResName;
            public string PrefabPath;
        }

        [Serializable]
        private sealed class StructureReport
        {
            public string generatedAt;
            public RoleStructureEntry[] roles;
        }

        [Serializable]
        private sealed class RoleStructureEntry
        {
            public string label;
            public string resName;
            public string prefabPath;
            public string prefabRootName;
            public SerializableVector3 prefabRootLocalPosition;
            public SerializableVector3 prefabRootLocalEuler;
            public SerializableVector3 prefabRootLocalScale;
            public int prefabRootChildCount;
            public string suggestedModelRoot;
            public string suggestedReason;
            public ChildEntry[] rootChildren;
        }

        [Serializable]
        private sealed class ChildEntry
        {
            public int index;
            public string name;
            public string path;
            public SerializableVector3 localPosition;
            public SerializableVector3 localEuler;
            public SerializableVector3 localScale;
            public int directChildCount;
            public int transformCount;
            public int rendererCount;
            public int skinnedRendererCount;
            public int meshRendererCount;
            public int particleRendererCount;
            public int animatorCount;
            public string leftFootName;
            public string rightFootName;
            public string anyFootName;
            public bool hasFootBone;
            public bool hasAnimator;
            public float score;
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
            new RoleProbeDefinition { Label = "Archer", ResName = "Role_Archer", PrefabPath = "Assets/Bundles/Roles/Role_Archer.prefab" },
            new RoleProbeDefinition { Label = "Holymentor", ResName = "Role_Holymentor", PrefabPath = "Assets/Bundles/Roles/Role_Holymentor.prefab" },
            new RoleProbeDefinition { Label = "Gladiator", ResName = "Role_Gladiator", PrefabPath = "Assets/Bundles/Roles/Role_Gladiator.prefab" },
            new RoleProbeDefinition { Label = "GrowLancer", ResName = "Role_GrowLancer", PrefabPath = "Assets/Bundles/Roles/Role_GrowLancer.prefab" },
            new RoleProbeDefinition { Label = "Summoner", ResName = "Role_Summoner", PrefabPath = "Assets/Bundles/Roles/Role_Summoner.prefab" },
        };

        [MenuItem("Tools/Diagnostics/Dump ChooseRole Prefab Structure")]
        public static void DumpPrefabStructureMenu()
        {
            string outputPath = DumpPrefabStructure();
            Debug.Log($"ChooseRolePrefabStructureDiagnostics wrote: {outputPath}");
        }

        public static string DumpPrefabStructure()
        {
            List<RoleStructureEntry> roles = new List<RoleStructureEntry>(ProbeDefinitions.Length);
            for (int i = 0; i < ProbeDefinitions.Length; ++i)
            {
                roles.Add(ProbeRole(ProbeDefinitions[i]));
            }

            StructureReport report = new StructureReport
            {
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                roles = roles.ToArray(),
            };

            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? "F:/MUSF/Client/Unity";
            string outputPath = Path.Combine(projectRoot, OutputRelativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? projectRoot);
            File.WriteAllText(outputPath, JsonUtility.ToJson(report, true));
            AssetDatabase.Refresh();
            return outputPath;
        }

        private static RoleStructureEntry ProbeRole(RoleProbeDefinition definition)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(definition.PrefabPath);
            if (prefab == null)
            {
                throw new InvalidOperationException($"ChooseRolePrefabStructureDiagnostics: prefab not found at {definition.PrefabPath}");
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
            {
                throw new InvalidOperationException($"ChooseRolePrefabStructureDiagnostics: failed to instantiate {definition.PrefabPath}");
            }

            try
            {
                instance.name = $"StructureProbe_{definition.Label}";
                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;

                List<ChildEntry> rootChildren = new List<ChildEntry>(instance.transform.childCount);
                ChildEntry bestCandidate = default;
                bool hasBestCandidate = false;
                string suggestedReason = "no-children";

                for (int i = 0; i < instance.transform.childCount; ++i)
                {
                    Transform child = instance.transform.GetChild(i);
                    ChildEntry entry = BuildChildEntry(child, i);
                    rootChildren.Add(entry);
                    if (!hasBestCandidate || entry.score > bestCandidate.score)
                    {
                        bestCandidate = entry;
                        hasBestCandidate = true;
                        suggestedReason = BuildSuggestedReason(entry);
                    }
                }

                return new RoleStructureEntry
                {
                    label = definition.Label,
                    resName = definition.ResName,
                    prefabPath = definition.PrefabPath,
                    prefabRootName = instance.name,
                    prefabRootLocalPosition = new SerializableVector3(instance.transform.localPosition),
                    prefabRootLocalEuler = new SerializableVector3(instance.transform.localEulerAngles),
                    prefabRootLocalScale = new SerializableVector3(instance.transform.localScale),
                    prefabRootChildCount = instance.transform.childCount,
                    suggestedModelRoot = hasBestCandidate ? bestCandidate.path : string.Empty,
                    suggestedReason = suggestedReason,
                    rootChildren = rootChildren.ToArray(),
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

        private static ChildEntry BuildChildEntry(Transform child, int index)
        {
            Renderer[] renderers = child.GetComponentsInChildren<Renderer>(true);
            SkinnedMeshRenderer[] skinnedRenderers = child.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            MeshRenderer[] meshRenderers = child.GetComponentsInChildren<MeshRenderer>(true);
            ParticleSystemRenderer[] particleRenderers = child.GetComponentsInChildren<ParticleSystemRenderer>(true);
            Animator[] animators = child.GetComponentsInChildren<Animator>(true);
            Transform[] transforms = child.GetComponentsInChildren<Transform>(true);

            string leftFootName;
            string rightFootName;
            string anyFootName;
            bool hasFootBone = TryFindFootBones(transforms, out leftFootName, out rightFootName, out anyFootName);
            bool hasAnimator = animators != null && animators.Length > 0;

            float score = 0f;
            score += skinnedRenderers.Length * 10f;
            score += meshRenderers.Length * 2f;
            score += hasFootBone ? 120f : 0f;
            score += hasAnimator ? 20f : 0f;
            score += transforms.Length * 0.02f;
            score -= particleRenderers.Length * 5f;

            Bounds bounds = default;
            if (!TryGetCombinedRendererBounds(renderers, out bounds))
            {
                bounds = new Bounds(child.position, Vector3.zero);
            }

            return new ChildEntry
            {
                index = index,
                name = child.name,
                path = BuildTransformPath(child),
                localPosition = new SerializableVector3(child.localPosition),
                localEuler = new SerializableVector3(child.localEulerAngles),
                localScale = new SerializableVector3(child.localScale),
                directChildCount = child.childCount,
                transformCount = transforms?.Length ?? 0,
                rendererCount = renderers?.Length ?? 0,
                skinnedRendererCount = skinnedRenderers?.Length ?? 0,
                meshRendererCount = meshRenderers?.Length ?? 0,
                particleRendererCount = particleRenderers?.Length ?? 0,
                animatorCount = animators?.Length ?? 0,
                leftFootName = leftFootName ?? string.Empty,
                rightFootName = rightFootName ?? string.Empty,
                anyFootName = anyFootName ?? string.Empty,
                hasFootBone = hasFootBone,
                hasAnimator = hasAnimator,
                score = score,
                boundsCenter = new SerializableVector3(bounds.center),
                boundsSize = new SerializableVector3(bounds.size),
            };
        }

        private static bool TryFindFootBones(Transform[] transforms, out string leftFootName, out string rightFootName, out string anyFootName)
        {
            leftFootName = string.Empty;
            rightFootName = string.Empty;
            anyFootName = string.Empty;

            if (transforms == null || transforms.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < transforms.Length; ++i)
            {
                Transform current = transforms[i];
                if (current == null || string.IsNullOrEmpty(current.name))
                {
                    continue;
                }

                string lowered = current.name.ToLowerInvariant();
                if (string.IsNullOrEmpty(leftFootName) && (lowered.Contains("l foot") || lowered.Contains("leftfoot") || lowered.Contains("left_foot")))
                {
                    leftFootName = current.name;
                    continue;
                }

                if (string.IsNullOrEmpty(rightFootName) && (lowered.Contains("r foot") || lowered.Contains("rightfoot") || lowered.Contains("right_foot")))
                {
                    rightFootName = current.name;
                    continue;
                }

                if (string.IsNullOrEmpty(anyFootName) && lowered.Contains("foot"))
                {
                    anyFootName = current.name;
                }
            }

            return !string.IsNullOrEmpty(leftFootName) || !string.IsNullOrEmpty(rightFootName) || !string.IsNullOrEmpty(anyFootName);
        }

        private static string BuildTransformPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            string path = transform.name;
            Transform current = transform.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        private static string BuildSuggestedReason(ChildEntry entry)
        {
            List<string> reasons = new List<string>(4);
            if (entry.hasFootBone)
            {
                reasons.Add("foot-bone");
            }

            if (entry.hasAnimator)
            {
                reasons.Add("animator");
            }

            if (entry.skinnedRendererCount > 0)
            {
                reasons.Add($"skinned={entry.skinnedRendererCount}");
            }

            if (entry.meshRendererCount > 0)
            {
                reasons.Add($"mesh={entry.meshRendererCount}");
            }

            if (reasons.Count == 0)
            {
                reasons.Add("highest-score");
            }

            return string.Join(", ", reasons.ToArray());
        }

        private static bool TryGetCombinedRendererBounds(Renderer[] renderers, out Bounds bounds)
        {
            bounds = default;
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
                if (renderer == null)
                {
                    continue;
                }

                Bounds rendererBounds;
                try
                {
                    rendererBounds = renderer.bounds;
                }
                catch
                {
                    continue;
                }

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
