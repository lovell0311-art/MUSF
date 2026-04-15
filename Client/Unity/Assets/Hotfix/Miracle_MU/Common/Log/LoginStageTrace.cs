using System;
using System.IO;
using ETModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ETHotfix
{
    public static class LoginStageTrace
    {
        private static readonly object SyncRoot = new object();

        public static string TracePath => Path.Combine(PathHelper.AppHotfixResPath, "login-trace.txt");

        public static void Clear()
        {
            try
            {
                string dir = Path.GetDirectoryName(TracePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                lock (SyncRoot)
                {
                    using (FileStream fs = new FileStream(TracePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(fs, new System.Text.UTF8Encoding(false)))
                    {
                        writer.WriteLine("TRACE_RESET");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"#LoginTrace# clear failed: {e.Message}");
            }
        }

        public static void Append(string message)
        {
            try
            {
                string dir = Path.GetDirectoryName(TracePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                lock (SyncRoot)
                {
                    using (FileStream fs = new FileStream(TracePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(fs, new System.Text.UTF8Encoding(false)))
                    {
                        writer.WriteLine(message);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"#LoginTrace# append failed: {e.Message}");
            }
        }

        public static void AppendWorldSnapshot(string stage)
        {
            try
            {
                CameraFollowComponent followComponent = CameraFollowComponent.Instance;
                RoleEntity localRole = UnitEntityComponent.Instance?.LocalRole;
                RoleMoveControlComponent moveControl = localRole?.GetComponent<RoleMoveControlComponent>();
                UnitEntityPathComponent pathComponent = localRole?.GetComponent<UnitEntityPathComponent>();
                Transform cameraTransform = followComponent?.cameraTransform ?? CameraComponent.Instance?.MainCamera?.transform;
                string sceneName = SceneManager.GetActiveScene().name;
                string roleNode = FormatNode(localRole?.CurrentNodePos);
                string moveState = moveControl == null
                    ? "moveCan=null moveNav=null"
                    : $"moveCan={moveControl.IsCanMove} moveNav={moveControl.IsNavigation}";
                string pathState = pathComponent == null
                    ? "pathQueued=null pathPoints=null"
                    : $"pathQueued={pathComponent.moveNodes.Count} pathPoints={pathComponent.Path.Count}";

                string followAngles = followComponent == null
                    ? "h=null v=null d=null chg=null hOff=null"
                    : $"h={followComponent.curAngleH:F2} v={followComponent.curAngleV:F2} d={followComponent.distance:F2} chg={followComponent.ChangeScene} hOff={followComponent.h:F2}";

                Append(
                    $"WorldSnapshot stage={stage} scene={sceneName} " +
                    $"roleId={(localRole != null ? localRole.Id.ToString() : "null")} " +
                    $"rolePos={FormatVector3(localRole?.Position)} " +
                    $"roleRootPos={FormatVector3(localRole?.roleTrs)} " +
                    $"roleModelPos={FormatVector3(localRole?.Game_Object?.transform)} " +
                    $"roleNode={roleNode} " +
                    $"followTargetPos={FormatVector3(followComponent?.followTarget)} " +
                    $"cameraPos={FormatVector3(cameraTransform)} " +
                    $"cameraEuler={FormatEuler(cameraTransform)} " +
                    $"{followAngles} {moveState} {pathState}");
            }
            catch (Exception e)
            {
                Append($"WorldSnapshot stage={stage} exception={e.GetType().Name}:{e.Message}");
            }
        }

        private static string FormatVector3(Vector3? vector)
        {
            if (!vector.HasValue)
            {
                return "null";
            }

            Vector3 value = vector.Value;
            return $"{value.x:F2},{value.y:F2},{value.z:F2}";
        }

        private static string FormatVector3(Transform transform)
        {
            if (transform == null)
            {
                return "null";
            }

            return FormatVector3(transform.position);
        }

        private static string FormatEuler(Transform transform)
        {
            if (transform == null)
            {
                return "null";
            }

            Vector3 value = transform.eulerAngles;
            return $"{value.x:F2},{value.y:F2},{value.z:F2}";
        }

        private static string FormatNode(AstarNode node)
        {
            if (node == null)
            {
                return "null";
            }

            return $"{node.x},{node.z},walkable={node.isWalkable}";
        }
    }
}
