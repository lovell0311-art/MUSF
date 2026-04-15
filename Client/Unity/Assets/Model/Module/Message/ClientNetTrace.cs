using System;
using System.IO;
using UnityEngine;

namespace ETModel
{
    public static class ClientNetTrace
    {
        private static readonly object SyncRoot = new object();

        public static string TracePath => Path.Combine(PathHelper.AppHotfixResPath, "model-net-trace.txt");

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
            catch
            {
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
            catch
            {
            }
        }
    }
}
