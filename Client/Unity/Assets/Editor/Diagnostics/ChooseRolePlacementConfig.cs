using System;
using UnityEngine;

namespace ETEditor
{
    [Serializable]
    public struct ChooseRolePedestalSlot
    {
        public string Label;
        public Vector3 Position;
        public float Yaw;
    }

    [Serializable]
    public struct ChooseRolePreviewRole
    {
        public string Label;
        public string PrefabPath;
        public int PedestalIndex;
        public Vector3 ModelOffset;
        public float ModelYaw;
        public float Scale;
    }

    public sealed class ChooseRolePlacementConfig : ScriptableObject
    {
        public const int PedestalCount = 5;
        public const int PreviewRoleCount = 4;

        public ChooseRolePedestalSlot[] Pedestals = new ChooseRolePedestalSlot[PedestalCount];
        public ChooseRolePedestalSlot[] TargetPedestals = new ChooseRolePedestalSlot[PedestalCount];
        public ChooseRolePreviewRole[] PreviewRoles = new ChooseRolePreviewRole[PreviewRoleCount];

        public void ResetToDefaults()
        {
            Pedestals = CreateDefaultRuntimePedestals();
            TargetPedestals = CreateDefaultTargetPedestals();
            PreviewRoles = CreateDefaultPreviewRoles();
        }

        public void CopyPedestalsToTarget()
        {
            TargetPedestals = ClonePedestals(Pedestals);
        }

        public void CopyTargetToPedestals()
        {
            Pedestals = ClonePedestals(TargetPedestals);
        }

        public void EnsureInitialized()
        {
            if (Pedestals == null || Pedestals.Length != PedestalCount)
            {
                Pedestals = CreateDefaultRuntimePedestals();
            }

            if (TargetPedestals == null || TargetPedestals.Length != PedestalCount)
            {
                TargetPedestals = CreateDefaultTargetPedestals();
            }

            if (PreviewRoles == null || PreviewRoles.Length != PreviewRoleCount)
            {
                PreviewRoles = CreateDefaultPreviewRoles();
            }

            for (int i = 0; i < Pedestals.Length; ++i)
            {
                if (string.IsNullOrWhiteSpace(Pedestals[i].Label))
                {
                    Pedestals[i].Label = $"P{i}";
                }
            }

            for (int i = 0; i < TargetPedestals.Length; ++i)
            {
                if (string.IsNullOrWhiteSpace(TargetPedestals[i].Label))
                {
                    TargetPedestals[i].Label = $"T{i}";
                }
            }

            ChooseRolePreviewRole[] defaultPreviewRoles = CreateDefaultPreviewRoles();
            for (int i = 0; i < PreviewRoles.Length; ++i)
            {
                if (string.IsNullOrWhiteSpace(PreviewRoles[i].Label))
                {
                    PreviewRoles[i].Label = defaultPreviewRoles[i].Label;
                }

                if (string.IsNullOrWhiteSpace(PreviewRoles[i].PrefabPath))
                {
                    PreviewRoles[i].PrefabPath = defaultPreviewRoles[i].PrefabPath;
                }

                if (PreviewRoles[i].Scale <= 0f)
                {
                    PreviewRoles[i].Scale = defaultPreviewRoles[i].Scale;
                }

                if (PreviewRoles[i].PedestalIndex < 0 || PreviewRoles[i].PedestalIndex >= PedestalCount)
                {
                    PreviewRoles[i].PedestalIndex = defaultPreviewRoles[i].PedestalIndex;
                }
            }
        }

        private static ChooseRolePedestalSlot[] CreateDefaultRuntimePedestals()
        {
            return new[]
            {
                new ChooseRolePedestalSlot { Label = "P0", Position = new Vector3(7.10f, 1.15f, 5.04f), Yaw = -10f },
                new ChooseRolePedestalSlot { Label = "P1", Position = new Vector3(4.61f, 1.15f, 1.96f), Yaw = -5f },
                new ChooseRolePedestalSlot { Label = "P2", Position = new Vector3(-0.84f, 1.15f, 3.76f), Yaw = 0f },
                new ChooseRolePedestalSlot { Label = "P3", Position = new Vector3(-4.95f, 1.15f, 4.44f), Yaw = 5f },
                new ChooseRolePedestalSlot { Label = "P4", Position = new Vector3(-10.01f, 1.15f, 4.05f), Yaw = 10f },
            };
        }

        private static ChooseRolePedestalSlot[] CreateDefaultTargetPedestals()
        {
            return new[]
            {
                new ChooseRolePedestalSlot { Label = "T0", Position = new Vector3(7.10f, 1.15f, 5.04f), Yaw = -10f },
                new ChooseRolePedestalSlot { Label = "T1", Position = new Vector3(4.61f, 1.15f, 1.96f), Yaw = -5f },
                new ChooseRolePedestalSlot { Label = "T2", Position = new Vector3(-0.84f, 1.15f, 3.76f), Yaw = 0f },
                new ChooseRolePedestalSlot { Label = "T3", Position = new Vector3(-4.95f, 1.15f, 4.44f), Yaw = 5f },
                new ChooseRolePedestalSlot { Label = "T4", Position = new Vector3(-10.01f, 1.15f, 4.05f), Yaw = 10f },
            };
        }

        private static ChooseRolePreviewRole[] CreateDefaultPreviewRoles()
        {
            return new[]
            {
                new ChooseRolePreviewRole
                {
                    Label = "GrowLancer",
                    PrefabPath = "Assets/Bundles/Roles/Role_GrowLancer.prefab",
                    PedestalIndex = 3,
                    ModelOffset = Vector3.zero,
                    ModelYaw = 0f,
                    Scale = 0.88f,
                },
                new ChooseRolePreviewRole
                {
                    Label = "Gladiator",
                    PrefabPath = "Assets/Bundles/Roles/Role_Gladiator.prefab",
                    PedestalIndex = 2,
                    ModelOffset = Vector3.zero,
                    ModelYaw = 0f,
                    Scale = 0.88f,
                },
                new ChooseRolePreviewRole
                {
                    Label = "Holymentor",
                    PrefabPath = "Assets/Bundles/Roles/Role_Holymentor.prefab",
                    PedestalIndex = 1,
                    ModelOffset = Vector3.zero,
                    ModelYaw = 0f,
                    Scale = 0.88f,
                },
                new ChooseRolePreviewRole
                {
                    Label = "Archer",
                    PrefabPath = "Assets/Bundles/Roles/Role_Archer.prefab",
                    PedestalIndex = 0,
                    ModelOffset = Vector3.zero,
                    ModelYaw = 0f,
                    Scale = 0.88f,
                },
            };
        }

        private static ChooseRolePedestalSlot[] ClonePedestals(ChooseRolePedestalSlot[] source)
        {
            ChooseRolePedestalSlot[] clone = new ChooseRolePedestalSlot[PedestalCount];
            for (int i = 0; i < clone.Length; ++i)
            {
                clone[i] = source[i];
            }

            return clone;
        }
    }
}
