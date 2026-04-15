using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ETEditor
{
    public static class ChooseRoleSceneDiagnostics
    {
        private const string ScenePath = "Assets/Scenes/GameMap/ChooseRole.unity";

        [MenuItem("Tools/Diagnostics/Dump ChooseRole Scene Hierarchy")]
        public static void DumpChooseRoleSceneHierarchyMenu()
        {
            DumpChooseRoleSceneHierarchy();
        }

        public static void DumpChooseRoleSceneHierarchy()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string reportDir = Path.Combine(projectRoot, "reports");
            Directory.CreateDirectory(reportDir);

            string reportPath = Path.Combine(reportDir, "choose-role-scene-hierarchy.txt");
            StringBuilder builder = new StringBuilder(64 * 1024);

            builder.AppendLine("ChooseRole Scene Hierarchy");
            builder.AppendLine($"scenePath={ScenePath}");
            builder.AppendLine($"sceneName={scene.name}");
            builder.AppendLine();

            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; ++i)
            {
                DumpTransformRecursive(roots[i].transform, 0, builder);
            }

            File.WriteAllText(reportPath, builder.ToString(), Encoding.UTF8);
            Debug.Log($"ChooseRoleSceneDiagnostics wrote: {reportPath}");
            AssetDatabase.Refresh();
        }

        private static void DumpTransformRecursive(Transform current, int depth, StringBuilder builder)
        {
            if (current == null)
            {
                return;
            }

            string indent = new string(' ', depth * 2);
            builder.Append(indent);
            builder.Append("- path=");
            builder.Append(GetTransformPath(current));
            builder.Append(" localPos=");
            builder.Append(FormatVector3(current.localPosition));
            builder.Append(" worldPos=");
            builder.Append(FormatVector3(current.position));
            builder.Append(" localScale=");
            builder.Append(FormatVector3(current.localScale));

            Renderer renderer = current.GetComponent<Renderer>();
            if (renderer != null)
            {
                builder.Append(" rendererBoundsCenter=");
                builder.Append(FormatVector3(renderer.bounds.center));
                builder.Append(" rendererBoundsSize=");
                builder.Append(FormatVector3(renderer.bounds.size));
            }

            MeshFilter meshFilter = current.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                builder.Append(" mesh=");
                builder.Append(meshFilter.sharedMesh.name);
                builder.Append(" vertexCount=");
                builder.Append(meshFilter.sharedMesh.vertexCount);
            }

            builder.AppendLine();

            for (int i = 0; i < current.childCount; ++i)
            {
                DumpTransformRecursive(current.GetChild(i), depth + 1, builder);
            }
        }

        private static string GetTransformPath(Transform current)
        {
            List<string> names = new List<string>();
            Transform cursor = current;
            while (cursor != null)
            {
                names.Add(cursor.name);
                cursor = cursor.parent;
            }

            names.Reverse();
            return string.Join("/", names);
        }

        private static string FormatVector3(Vector3 value)
        {
            return $"({value.x:0.###},{value.y:0.###},{value.z:0.###})";
        }
    }
}
