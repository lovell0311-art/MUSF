using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

class MethodDump
{
    static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("usage: MethodDump <assembly> <out>");
            return 2;
        }

        string asmPath = args[0];
        string outPath = args[1];
        AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(asmPath, new ReaderParameters { ReadSymbols = false });
        TypeDefinition type = asm.MainModule.Types.FirstOrDefault(t => t.FullName == "ETHotfix.MiniMap");
        if (type == null)
        {
            Console.Error.WriteLine("type not found");
            return 3;
        }

        string[] methods = new[]
        {
            "ApplyMapTransform",
            "Map2UIPosition",
            "ScreenToCalibratedMapPosition",
            "SetMapScale",
            "ConfigureMapCalibration"
        };

        using (StreamWriter writer = new StreamWriter(outPath, false))
        {
            writer.WriteLine("Assembly: " + asmPath);
            foreach (string methodName in methods)
            {
                MethodDefinition[] matches = type.Methods.Where(m => m.Name == methodName).ToArray();
                foreach (MethodDefinition method in matches)
                {
                    writer.WriteLine("=== " + method.FullName + " ===");
                    if (!method.HasBody)
                    {
                        writer.WriteLine("<no body>");
                        continue;
                    }

                    foreach (Instruction ins in method.Body.Instructions)
                    {
                        string operand = FormatOperand(ins.Operand);
                        writer.WriteLine(ins.Offset.ToString("X4") + ": " + ins.OpCode.ToString().PadRight(12) + " " + operand);
                    }
                    writer.WriteLine();
                }
            }
        }
        return 0;
    }

    static string FormatOperand(object operand)
    {
        if (operand == null) return string.Empty;
        MethodReference mr = operand as MethodReference;
        if (mr != null) return mr.FullName;
        FieldReference fr = operand as FieldReference;
        if (fr != null) return fr.FullName;
        ParameterDefinition pd = operand as ParameterDefinition;
        if (pd != null) return pd.Name;
        VariableDefinition vd = operand as VariableDefinition;
        if (vd != null) return "V_" + vd.Index + ":" + vd.VariableType.FullName;
        Instruction target = operand as Instruction;
        if (target != null) return "IL_" + target.Offset.ToString("X4");
        Instruction[] targets = operand as Instruction[];
        if (targets != null) return string.Join(", ", targets.Select(t => "IL_" + t.Offset.ToString("X4")).ToArray());
        TypeReference tr = operand as TypeReference;
        if (tr != null) return tr.FullName;
        return operand.ToString();
    }
}
