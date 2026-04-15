using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using ETModel;

namespace ETTools
{
    internal class OpcodeInfo
    {
        public string Name;
        public int Opcode;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName;
        /// <summary>
        /// 在文件中的行号
        /// </summary>
        public int FileLineCode;
    }

    internal class ProtoInfo
    {
        public string FileName;
        public string ProtoName;
        public string ClassName;
    }

    public enum GenType
    {
        None,
        Server,
        Client,
    }

    public class Option
    {
        public static Option Instance;
        public GenType genType = GenType.Server;
        public Option()
        {
            Instance = this;
        }
        public void Parse(string[] args)
        {
            foreach(var arg in args)
            {
                string[] kv = arg.Split("=");
                if(kv.Length != 2)
                {
                    throw new Exception($"[err] 未知的参数 {arg}");
                }
                switch (kv[0])
                {
                    case "genType":
                        genType = (GenType)Enum.Parse(typeof(GenType), kv[1]);
                        break;
                }
            }
        }
    }


    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Main(string[] args)
        {
            Option option = new Option();

            string protoc = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                protoc = "protoc.exe";
            }
            else
            {
                protoc = "protoc";
            }

            Option.Instance.Parse(args);


            List<ProtoInfo> hotfixMessageList = new List<ProtoInfo>();
            List<ProtoInfo> innerMessageList = new List<ProtoInfo>();   // 暂时不使用
            List<ProtoInfo> clientMessageList = new List<ProtoInfo>();
            var lst = GetFiles("./");
            foreach(var v in lst)
            {
                if(!v.Name.EndsWith("Message.proto"))
                {
                    continue;
                }
                ProtoInfo info = new ProtoInfo();
                info.FileName = v.Name;
                info.ProtoName = v.Name.Substring(0, v.Name.Length - "Message.proto".Length);
                info.ClassName = info.ProtoName + "Opcode";
                switch (info.ProtoName)
                {
                    case "Outer":
                        clientMessageList.Add(info);
                        break;
                    case "Inner":
                        innerMessageList.Add(info);
                        break;
                    default:
                        hotfixMessageList.Add(info);
                        break;
                }

            }

            Console.WriteLine($"[info] 生成文件");
            // 客户端Model层
            foreach (var info in clientMessageList)
            {
                Console.WriteLine($"[info] {info.FileName}");
                ProcessHelper.Run(protoc, "--csharp_out=\"../Unity/Assets/Model/Module/Message/\" --proto_path=\"./\" " + info.FileName, waitExit: true);
                Proto2CS("ETModel", "OuterMessage.proto", clientMessagePath, "OuterOpcode", 100);
            }
            // 客户端Hotfix层
            foreach (var info in hotfixMessageList)
            {
                Console.WriteLine($"[info] {info.FileName}");
                ProcessHelper.Run(protoc, "--csharp_out=\"../Unity/Assets/Hotfix/Module/Message/\" --proto_path=\"./\" " + info.FileName, waitExit: true);
                Proto2CS("ETHotfix", info.FileName, hotfixMessagePath, info.ClassName, 10000);
            }

            // InnerMessage.proto生成cs代码
            if(Option.Instance.genType == GenType.Server)
            {
                // 只有服务端需要生成
                InnerProto2CS.Proto2CS();
            }

            Option.Instance = null;
            Console.WriteLine("proto2cs succeed!");

            Console.Read();
        }

        private static void GetDirectorys(string strPath, ref List<string> lstDirect)
        {
            DirectoryInfo diFliles = new DirectoryInfo(strPath);
            DirectoryInfo[] diArr = diFliles.GetDirectories();
            foreach (DirectoryInfo di in diArr)
            {
                try
                {
                    lstDirect.Add(di.FullName);
                    GetDirectorys(di.FullName, ref lstDirect);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 遍历当前目录及子目录
        /// </summary>
        /// <param name="strPath">文件路径</param>
        /// <returns>所有文件</returns>
        private static List<FileInfo> GetFiles(string strPath)
        {
            List<FileInfo> lstFiles = new List<FileInfo>();
            List<string> lstDirect = new List<string>();
            lstDirect.Add(strPath);
            DirectoryInfo diFliles = null;
            GetDirectorys(strPath, ref lstDirect);
            foreach (string str in lstDirect)
            {
                try
                {
                    diFliles = new DirectoryInfo(str);
                    lstFiles.AddRange(diFliles.GetFiles());
                }
                catch
                {
                    continue;
                }
            }
            return lstFiles;
        }

        private const string protoPath = ".";
        private const string clientMessagePath = "../Unity/Assets/Model/Module/Message/";
        private const string hotfixMessagePath = "../Unity/Assets/Hotfix/Module/Message/";
        private static readonly char[] splitChars = { ' ', '\t' };
        private static readonly List<OpcodeInfo> msgOpcode = new List<OpcodeInfo>();
        private static readonly Dictionary<int,OpcodeInfo> existOpcode = new Dictionary<int, OpcodeInfo>();

        public static void Proto2CS(string ns, string protoName, string outputPath, string opcodeClassName, int startOpcode, bool isClient = true)
        {
            msgOpcode.Clear();
            string proto = Path.Combine(protoPath, protoName);

            string s = File.ReadAllText(proto);

            StringBuilder sb = new StringBuilder();
            sb.Append("using ETModel;\n");
            if(Option.Instance.genType == GenType.Server)
            {
                // 服务端专用代码
                sb.Append("using CustomFrameWork.Component;\n");
            }
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");

            bool isMsgStart = false;
            int lineCode = 0;

            foreach (string line in s.Split('\n'))
            {
                lineCode++;
                string newline = line.Trim();

                if (newline == "")
                {
                    continue;
                }

                if (newline.StartsWith("//"))
                {
                    sb.Append($"{newline}\n");
                    string remark = newline.Substring(2);
                    string[] arg = remark.Split("=");
                    if(arg.Length == 2)
                    {
                        switch (arg[0].Trim())
                        {
                            case "Opcode":
                                startOpcode = int.Parse(arg[1].Trim());
                                break;
                            default:
                                //Console.WriteLine($"[err] {protoName}:{lineCode} 未知的参数 {arg[0].Trim()}");
                                break;
                        }

                    }
                }

                if (newline.StartsWith("message"))
                {
                    string parentClass = "";
                    isMsgStart = true;
                    string msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                    if (ss.Length == 2)
                    {
                        parentClass = ss[1].Trim();
                    }
                    else
                    {
                        parentClass = "";
                    }

                    msgOpcode.Add(new OpcodeInfo() { Name = msgName, Opcode = ++startOpcode });
                    if(existOpcode.TryGetValue(startOpcode,out var info))
                    {
                        // opcode 重复
                        Console.WriteLine($"[err] {protoName}:{lineCode} 消息'{msgName}' 与 {info.FileName}:{info.FileLineCode} 消息'{info.Name}' Opcode 重复");
                    }
                    else
                    {
                        existOpcode.Add(startOpcode, new OpcodeInfo() { Name = msgName, Opcode = startOpcode,FileName = protoName,FileLineCode = lineCode });
                    }

                    sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n");
                    sb.Append($"\tpublic partial class {msgName} ");
                    if (parentClass != "")
                    {
                        sb.Append($": {parentClass} ");
                    }

                    sb.Append("{}\n\n");
                }

                if (isMsgStart && newline == "}")
                {
                    isMsgStart = false;
                }
            }

            sb.Append("}\n");

            GenerateOpcode(ns, opcodeClassName, outputPath, sb);
        }

        private static void GenerateOpcode(string ns, string outputFileName, string outputPath, StringBuilder sb)
        {
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");
            sb.Append($"\tpublic static partial class {outputFileName}\n");
            sb.Append("\t{\n");
            foreach (OpcodeInfo info in msgOpcode)
            {
                sb.Append($"\t\t public const ushort {info.Name} = {info.Opcode};\n");
            }

            sb.Append("\t}\n");
            sb.Append("}\n");

            string csPath = Path.Combine(outputPath, outputFileName + ".cs");
            File.WriteAllText(csPath, sb.ToString());
        }
    }

    public static class InnerProto2CS
    {
        private const string protoPath = ".";
        private const string serverMessagePath = "../Server/Model/Module/Message/";
        private static readonly char[] splitChars = { ' ', '\t' };
        private static readonly List<OpcodeInfo> msgOpcode = new List<OpcodeInfo>();

        public static void Proto2CS()
        {
            msgOpcode.Clear();
            Proto2CS("ETModel", "InnerMessage.proto", serverMessagePath, "InnerOpcode", 1000);
            GenerateOpcode("ETModel", "InnerOpcode", serverMessagePath);
        }

        public static void Proto2CS(string ns, string protoName, string outputPath, string opcodeClassName, int startOpcode)
        {
            msgOpcode.Clear();
            string proto = Path.Combine(protoPath, protoName);
            string csPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(proto) + ".cs");

            string s = File.ReadAllText(proto);

            StringBuilder sb = new StringBuilder();
            sb.Append("using ETModel;\n");
            sb.Append("using System.Collections.Generic;\n");
            if(Option.Instance.genType == GenType.Server)
            {
                // 服务端专用代码
                sb.Append("using CustomFrameWork.Component;\n");
                sb.Append("using MongoDB.Bson;\n");
            }
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");

            bool isMsgStart = false;
            string parentClass = "";
            foreach (string line in s.Split('\n'))
            {
                string newline = line.Trim();

                if (newline == "")
                {
                    continue;
                }

                if (newline.StartsWith("//"))
                {
                    sb.Append($"{newline}\n");
                }

                if (newline.StartsWith("message"))
                {
                    parentClass = "";
                    isMsgStart = true;
                    string msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                    if (ss.Length == 2)
                    {
                        parentClass = ss[1].Trim();
                    }

                    msgOpcode.Add(new OpcodeInfo() { Name = msgName, Opcode = ++startOpcode });

                    sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n");
                    sb.Append($"\tpublic partial class {msgName}");
                    if (parentClass == "IActorMessage" || parentClass == "IActorRequest" || parentClass == "IActorResponse" ||
                        parentClass == "IFrameMessage")
                    {
                        sb.Append($": {parentClass}\n");
                    }
                    else if (parentClass != "")
                    {
                        sb.Append($": {parentClass}\n");
                    }
                    else
                    {
                        sb.Append("\n");
                    }

                    continue;
                }

                if (isMsgStart)
                {
                    if (newline == "{")
                    {
                        sb.Append("\t{\n");
                        continue;
                    }

                    if (newline == "}")
                    {
                        isMsgStart = false;
                        sb.Append("\t}\n\n");
                        continue;
                    }

                    if (newline.Trim().StartsWith("//"))
                    {
                        sb.Append(newline + "\n");
                        continue;
                    }

                    if (newline.Trim() != "" && newline != "}")
                    {
                        if (newline.StartsWith("repeated"))
                        {
                            Repeated(sb, ns, newline);
                        }
                        else
                        {
                            Members(sb, newline, true);
                        }
                    }
                }
            }

            sb.Append("}\n");

            File.WriteAllText(csPath, sb.ToString());
        }

        private static void GenerateOpcode(string ns, string outputFileName, string outputPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");
            sb.Append($"\tpublic static partial class {outputFileName}\n");
            sb.Append("\t{\n");
            foreach (OpcodeInfo info in msgOpcode)
            {
                sb.Append($"\t\t public const ushort {info.Name} = {info.Opcode};\n");
            }

            sb.Append("\t}\n");
            sb.Append("}\n");

            string csPath = Path.Combine(outputPath, outputFileName + ".cs");
            File.WriteAllText(csPath, sb.ToString());
        }

        private static void Repeated(StringBuilder sb, string ns, string newline)
        {
            try
            {
                int index = newline.IndexOf(";");
                newline = newline.Remove(index);
                string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[1];
                type = ConvertType(type);
                string name = ss[2];

                sb.Append($"\t\tpublic List<{type}> {name} = new List<{type}>();\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }

        private static string ConvertType(string type)
        {
            string typeCs = "";
            switch (type)
            {
                case "int16":
                    typeCs = "short";
                    break;
                case "int32":
                    typeCs = "int";
                    break;
                case "bytes":
                    typeCs = "byte[]";
                    break;
                case "uint32":
                    typeCs = "uint";
                    break;
                case "long":
                    typeCs = "long";
                    break;
                case "int64":
                    typeCs = "long";
                    break;
                case "uint64":
                    typeCs = "ulong";
                    break;
                case "uint16":
                    typeCs = "ushort";
                    break;
                default:
                    typeCs = type;
                    break;
            }

            return typeCs;
        }

        private static void Members(StringBuilder sb, string newline, bool isRequired)
        {
            try
            {
                int index = newline.IndexOf(";");
                newline = newline.Remove(index);
                string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[0];
                string name = ss[1];
                string typeCs = ConvertType(type);

                sb.Append($"\t\tpublic {typeCs} {name} {{ get; set; }}\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }
    }
}
