using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class PatchedCodeBundleBuilder
    {
        private const string PatchDllEnv = "CODE_BUNDLE_PATCH_DLL";
        private const string PatchPdbEnv = "CODE_BUNDLE_PATCH_PDB";
        private const string ExportDirEnv = "CODE_BUNDLE_BUILD_EXPORT_DIR";

        public static void RunFromEnv()
        {
            bool success = false;
            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? string.Empty;
            string codeDir = Path.Combine(Application.dataPath, "Res", "Code");
            string dllTarget = Path.Combine(codeDir, "Hotfix.dll.bytes");
            string pdbTarget = Path.Combine(codeDir, "Hotfix.pdb.bytes");
            string backupDir = Path.Combine(projectRoot, "Temp", "PatchedCodeBundleBuilderBackup", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
            string patchDllPath = Environment.GetEnvironmentVariable(PatchDllEnv) ?? string.Empty;
            string patchPdbPath = Environment.GetEnvironmentVariable(PatchPdbEnv) ?? string.Empty;
            string exportDir = Environment.GetEnvironmentVariable(ExportDirEnv) ?? string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(patchDllPath) || !File.Exists(patchDllPath))
                {
                    throw new FileNotFoundException($"Patched dll not found. env={PatchDllEnv}", patchDllPath);
                }

                Directory.CreateDirectory(backupDir);
                File.Copy(dllTarget, Path.Combine(backupDir, "Hotfix.dll.bytes"), true);
                File.Copy(pdbTarget, Path.Combine(backupDir, "Hotfix.pdb.bytes"), true);

                File.Copy(patchDllPath, dllTarget, true);
                if (!string.IsNullOrWhiteSpace(patchPdbPath) && File.Exists(patchPdbPath))
                {
                    File.Copy(patchPdbPath, pdbTarget, true);
                }

                AssetDatabase.ImportAsset("Assets/Res/Code/Hotfix.dll.bytes", ImportAssetOptions.ForceUpdate);
                AssetDatabase.ImportAsset("Assets/Res/Code/Hotfix.pdb.bytes", ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();

                BatchBuildCodeBundle.PerformBuild();

                if (!string.IsNullOrWhiteSpace(exportDir))
                {
                    Directory.CreateDirectory(exportDir);
                    string streamingAssetsDir = Path.Combine(Application.dataPath, "StreamingAssets");
                    File.Copy(Path.Combine(streamingAssetsDir, "code.unity3d"), Path.Combine(exportDir, "code.unity3d"), true);
                    File.Copy(Path.Combine(streamingAssetsDir, "code.unity3d.manifest"), Path.Combine(exportDir, "code.unity3d.manifest"), true);
                }

                success = true;
                Debug.Log($"PatchedCodeBundleBuilder succeeded. patch={patchDllPath} export={exportDir}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                success = false;
            }
            finally
            {
                try
                {
                    string backupDll = Path.Combine(backupDir, "Hotfix.dll.bytes");
                    string backupPdb = Path.Combine(backupDir, "Hotfix.pdb.bytes");
                    if (File.Exists(backupDll))
                    {
                        File.Copy(backupDll, dllTarget, true);
                    }

                    if (File.Exists(backupPdb))
                    {
                        File.Copy(backupPdb, pdbTarget, true);
                    }

                    AssetDatabase.ImportAsset("Assets/Res/Code/Hotfix.dll.bytes", ImportAssetOptions.ForceUpdate);
                    AssetDatabase.ImportAsset("Assets/Res/Code/Hotfix.pdb.bytes", ImportAssetOptions.ForceUpdate);
                    AssetDatabase.Refresh();
                }
                catch (Exception restoreEx)
                {
                    Debug.LogException(restoreEx);
                    success = false;
                }

                EditorApplication.Exit(success ? 0 : 1);
            }
        }
    }
}
