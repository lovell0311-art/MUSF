using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Diagnostics;
using CustomFrameWork;

namespace ETModel
{
    public class CodeLoader
    {
        public static CodeLoader Instance = new CodeLoader();

        private AssemblyLoadContext assemblyLoadContext;

        public void Start()
        {
            Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
            LoadHotfix();
        }

        public void LoadHotfix()
        {
            assemblyLoadContext?.Unload();
            GC.Collect();

            Guid guid = Guid.NewGuid();

            assemblyLoadContext = new AssemblyLoadContext("Hotfix", true);

            string srcDllFile = Environment.CurrentDirectory + "/Hotfix.dll";
            string srcPdbFile = Environment.CurrentDirectory + "/Hotfix.pdb";
            string tempDir = Environment.CurrentDirectory + $"/temp/hotfix";
            string destDir = tempDir + $"/{guid}";
            string destDllFile = destDir + "/Hotfix.dll";
            string destPdbFile = destDir + "/Hotfix.pdb";

//             if (Directory.Exists(tempDir))
//                 CleanDirectory(tempDir
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // 复制 Hotfix.dll 文件
            File.Copy(srcDllFile, destDllFile, true);
            File.Copy(srcPdbFile, destPdbFile, true);

            // Repl 可以调用Hotfix程序集
            Assembly hotfixAssembly = assemblyLoadContext.LoadFromAssemblyPath(destDllFile);


            //byte[] dllBytes = File.ReadAllBytes("./Hotfix.dll");
            //byte[] pdbBytes = File.ReadAllBytes("./Hotfix.pdb");
            //Assembly hotfixAssembly = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

            // ET框架热重载
            Game.EventSystem.Add(DLLType.Hotfix, hotfixAssembly);

            // 自定义框架热重载
            Root.SetHotfixAssembly(hotfixAssembly);
            Root.EventSystem.LoadDllCode();
        }

        public void StartDebug(Assembly hotfixAssembly)
        {
            Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
            Game.EventSystem.Add(DLLType.Hotfix, hotfixAssembly);

            Root.SetHotfixAssembly(hotfixAssembly);
            Root.EventSystem.LoadDllCode();
        }

        public void CleanDirectory(string folderPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);

            if (!dirInfo.Exists)
            {
                return;
            }

            foreach (DirectoryInfo info in dirInfo.GetDirectories())
            {
                if (IsDirectoryLocked(info))
                {
                    continue;
                }

                info.Delete(true);
            }
        }

        public bool IsDirectoryLocked(DirectoryInfo dirInfo)
        {
            // 遍历目录下的所有文件
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                try
                {
                    using (FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        
                        fileStream.Close();
                    }

                    continue;
                }
                catch (IOException)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
