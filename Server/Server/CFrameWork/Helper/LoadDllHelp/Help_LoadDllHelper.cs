
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace CustomFrameWork
{
    public class Help_LoadDllHelper
    {
        public static Assembly Load(string b_DllPath)
        {
            byte[] dllBuffer = File.ReadAllBytes($"{b_DllPath}.dll");
            byte[] pdbBuffer = File.ReadAllBytes($"{b_DllPath}.pdb");
            return Assembly.Load(dllBuffer, pdbBuffer);
        }
        public async static Task<Assembly> LoadAsync(string b_DllPath)
        {
            byte[] dllBuffer = await File.ReadAllBytesAsync($"{b_DllPath}.dll");
            byte[] pdbBuffer = await File.ReadAllBytesAsync($"{b_DllPath}.pdb");
            return Assembly.Load(dllBuffer, pdbBuffer);
        }
        public static Type LoadType(string b_TypeName, string b_DllPath)
        {
            Assembly assembly = Load(b_DllPath);
            return assembly.GetType(b_TypeName);
        }
        public async static Task<Type> LoadTypeAsync(string b_TypeName, string b_DllPath)
        {
            Assembly assembly = await LoadAsync(b_DllPath);
            return assembly.GetType(b_TypeName);
        }
    }
}
