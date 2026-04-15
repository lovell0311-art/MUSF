using System.IO;
using UnityEditor;

namespace ETEditor
{
    public static class HotfixBuildSync
    {
        private const string ScriptAssembliesDir = "Library/ScriptAssemblies";
        private const string CodeDir = "Assets/Res/Code";
        private const string HotfixDll = "Unity.Hotfix.dll";
        private const string HotfixPdb = "Unity.Hotfix.pdb";

        public static void SyncCompiledHotfixToResCode()
        {
            string dllSource = Path.Combine(ScriptAssembliesDir, HotfixDll);
            string pdbSource = Path.Combine(ScriptAssembliesDir, HotfixPdb);
            string dllTarget = Path.Combine(CodeDir, "Hotfix.dll.bytes");
            string pdbTarget = Path.Combine(CodeDir, "Hotfix.pdb.bytes");

            if (!File.Exists(dllSource))
            {
                throw new FileNotFoundException($"Compiled hotfix dll not found: {dllSource}", dllSource);
            }

            if (!File.Exists(pdbSource))
            {
                throw new FileNotFoundException($"Compiled hotfix pdb not found: {pdbSource}", pdbSource);
            }

            Directory.CreateDirectory(CodeDir);
            File.Copy(dllSource, dllTarget, true);
            File.Copy(pdbSource, pdbTarget, true);

            AssetDatabase.ImportAsset("Assets/Res/Code/Hotfix.dll.bytes", ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset("Assets/Res/Code/Hotfix.pdb.bytes", ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }
    }
}
