using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class PatchMiniMapHotfix
{
    private const string UnityManagedDir = @"C:\Program Files\Unity 2020.3.49f1c1\Editor\Data\Managed";
    private const string UnityManagedEngineDir = @"C:\Program Files\Unity 2020.3.49f1c1\Editor\Data\Managed\UnityEngine";
    private const string ProjectAssembliesDir = @"F:\MUSF\Client\Unity\Library\ScriptAssemblies";

    public static int Run(string inputPath, string outputPath)
    {
        if (!File.Exists(inputPath))
        {
            Console.Error.WriteLine("Input dll not found: " + inputPath);
            return 2;
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

            DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(inputPath) ?? ".");
            resolver.AddSearchDirectory(UnityManagedDir);
            resolver.AddSearchDirectory(UnityManagedEngineDir);
            resolver.AddSearchDirectory(ProjectAssembliesDir);

            ReaderParameters readerParameters = new ReaderParameters
            {
                AssemblyResolver = resolver,
                ReadSymbols = false
            };

            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(inputPath, readerParameters);
            ModuleDefinition module = assembly.MainModule;
            TypeDefinition miniMapType = FindType(module, "ETHotfix.MiniMap");
            if (miniMapType == null)
            {
                Console.Error.WriteLine("Type ETHotfix.MiniMap not found.");
                return 1;
            }

            MethodDefinition method = miniMapType.Methods.FirstOrDefault(m => m.Name == "ApplyMapTransform" && !m.HasParameters);
            if (method == null)
            {
                Console.Error.WriteLine("Method ApplyMapTransform not found.");
                return 1;
            }

            RewriteApplyMapTransform(module, resolver, miniMapType, method);
            assembly.Write(outputPath, new WriterParameters { WriteSymbols = false });

            Console.WriteLine("Patched " + miniMapType.FullName + "." + method.Name + " -> " + outputPath);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Patch failed: " + ex.GetType().FullName);
            Console.Error.WriteLine("Message: " + ex.Message);
            if (ex.InnerException != null)
            {
                Console.Error.WriteLine("Inner: " + ex.InnerException.GetType().FullName);
                Console.Error.WriteLine("InnerMessage: " + ex.InnerException.Message);
            }

            return 1;
        }
    }

    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: PatchMiniMapHotfix <input-dll> <output-dll>");
            return 2;
        }

        return Run(args[0], args[1]);
    }

    private static void RewriteApplyMapTransform(ModuleDefinition module, DefaultAssemblyResolver resolver, TypeDefinition miniMapType, MethodDefinition method)
    {
        AssemblyNameReference unityCoreRef = module.AssemblyReferences.First(r => r.Name == "UnityEngine.CoreModule");
        AssemblyDefinition unityCore = resolver.Resolve(unityCoreRef);

        TypeDefinition vector2Type = FindType(unityCore.MainModule, "UnityEngine.Vector2");
        TypeDefinition vector3Type = FindType(unityCore.MainModule, "UnityEngine.Vector3");
        TypeDefinition quaternionType = FindType(unityCore.MainModule, "UnityEngine.Quaternion");
        TypeDefinition rectTransformType = FindType(unityCore.MainModule, "UnityEngine.RectTransform");
        TypeDefinition transformType = FindType(unityCore.MainModule, "UnityEngine.Transform");
        TypeDefinition matrix4x4Type = FindType(unityCore.MainModule, "UnityEngine.Matrix4x4");

        FieldDefinition mapRectTransformField = GetField(miniMapType, "mapRectTransform");
        FieldDefinition mapZoomScaleField = GetField(miniMapType, "mapZoomScale");
        FieldDefinition mapCalibrationScaleField = GetField(miniMapType, "mapCalibrationScale");
        FieldDefinition mapCalibrationOffsetField = GetField(miniMapType, "mapCalibrationOffset");
        FieldDefinition matrixField = GetField(miniMapType, "matrix4X4");
        FieldDefinition matrixChangedField = GetField(miniMapType, "matrixChanged");

        MethodReference vector2ZeroGetter = module.ImportReference(GetPropertyGetter(vector2Type, "zero"));
        MethodReference vector3OneGetter = module.ImportReference(GetPropertyGetter(vector3Type, "one"));
        MethodReference vector3Ctor = module.ImportReference(GetConstructor(vector3Type, "System.Single", "System.Single", "System.Single"));
        MethodReference vector3Multiply = module.ImportReference(GetMethod(vector3Type, "op_Multiply", "UnityEngine.Vector3", "System.Single"));
        MethodReference quaternionEuler = module.ImportReference(GetMethod(quaternionType, "Euler", "System.Single", "System.Single", "System.Single"));
        MethodReference setAnchoredPosition = module.ImportReference(GetPropertySetter(rectTransformType, "anchoredPosition"));
        MethodReference setLocalRotation = module.ImportReference(GetPropertySetter(transformType, "localRotation"));
        MethodReference getLocalRotation = module.ImportReference(GetPropertyGetter(transformType, "localRotation"));
        MethodReference setLocalScale = module.ImportReference(GetPropertySetter(transformType, "localScale"));
        MethodReference matrixSetTrs = module.ImportReference(GetMethod(matrix4x4Type, "SetTRS", "UnityEngine.Vector3", "UnityEngine.Quaternion", "UnityEngine.Vector3"));
        FieldReference vector2XField = module.ImportReference(GetField(vector2Type, "x"));
        FieldReference vector2YField = module.ImportReference(GetField(vector2Type, "y"));

        method.Body.InitLocals = true;
        method.Body.Instructions.Clear();
        method.Body.ExceptionHandlers.Clear();
        method.Body.Variables.Clear();
        method.Body.MaxStackSize = 8;

        VariableDefinition finalScaleVar = new VariableDefinition(module.TypeSystem.Single);
        VariableDefinition translationVar = new VariableDefinition(module.ImportReference(vector3Type));
        VariableDefinition trsScaleVar = new VariableDefinition(module.ImportReference(vector3Type));
        method.Body.Variables.Add(finalScaleVar);
        method.Body.Variables.Add(translationVar);
        method.Body.Variables.Add(trsScaleVar);

        ILProcessor il = method.Body.GetILProcessor();
        Instruction notNull = Instruction.Create(OpCodes.Nop);

        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldfld, mapRectTransformField));
        il.Append(Instruction.Create(OpCodes.Brtrue_S, notNull));
        il.Append(Instruction.Create(OpCodes.Ret));
        il.Append(notNull);

        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldfld, mapRectTransformField));
        il.Append(Instruction.Create(OpCodes.Call, vector2ZeroGetter));
        il.Append(Instruction.Create(OpCodes.Callvirt, setAnchoredPosition));

        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldfld, mapRectTransformField));
        il.Append(Instruction.Create(OpCodes.Ldc_R4, 0f));
        il.Append(Instruction.Create(OpCodes.Ldc_R4, 0f));
        il.Append(Instruction.Create(OpCodes.Ldc_R4, 315f));
        il.Append(Instruction.Create(OpCodes.Call, quaternionEuler));
        il.Append(Instruction.Create(OpCodes.Callvirt, setLocalRotation));

        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldfld, mapZoomScaleField));
        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldfld, mapCalibrationScaleField));
        il.Append(Instruction.Create(OpCodes.Mul));
        il.Append(Instruction.Create(OpCodes.Stloc, finalScaleVar));

        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldfld, mapRectTransformField));
        il.Append(Instruction.Create(OpCodes.Call, vector3OneGetter));
        il.Append(Instruction.Create(OpCodes.Ldloc, finalScaleVar));
        il.Append(Instruction.Create(OpCodes.Call, vector3Multiply));
        il.Append(Instruction.Create(OpCodes.Callvirt, setLocalScale));

        il.Append(Instruction.Create(OpCodes.Ldloca_S, translationVar));
        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldflda, mapCalibrationOffsetField));
        il.Append(Instruction.Create(OpCodes.Ldfld, vector2XField));
        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldflda, mapCalibrationOffsetField));
        il.Append(Instruction.Create(OpCodes.Ldfld, vector2YField));
        il.Append(Instruction.Create(OpCodes.Ldc_R4, 0f));
        il.Append(Instruction.Create(OpCodes.Call, vector3Ctor));

        il.Append(Instruction.Create(OpCodes.Ldloca_S, trsScaleVar));
        il.Append(Instruction.Create(OpCodes.Ldloc, finalScaleVar));
        il.Append(Instruction.Create(OpCodes.Ldloc, finalScaleVar));
        il.Append(Instruction.Create(OpCodes.Ldloc, finalScaleVar));
        il.Append(Instruction.Create(OpCodes.Call, vector3Ctor));

        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldflda, matrixField));
        il.Append(Instruction.Create(OpCodes.Ldloc, translationVar));
        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldfld, mapRectTransformField));
        il.Append(Instruction.Create(OpCodes.Callvirt, getLocalRotation));
        il.Append(Instruction.Create(OpCodes.Ldloc, trsScaleVar));
        il.Append(Instruction.Create(OpCodes.Call, matrixSetTrs));

        il.Append(Instruction.Create(OpCodes.Ldarg_0));
        il.Append(Instruction.Create(OpCodes.Ldc_I4_1));
        il.Append(Instruction.Create(OpCodes.Stfld, matrixChangedField));
        il.Append(Instruction.Create(OpCodes.Ret));
    }

    private static TypeDefinition FindType(ModuleDefinition module, string fullName)
    {
        TypeDefinition direct = module.Types.FirstOrDefault(t => t.FullName == fullName);
        if (direct != null)
        {
            return direct;
        }

        foreach (TypeDefinition type in module.Types)
        {
            TypeDefinition nested = FindNestedType(type, fullName);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private static TypeDefinition FindNestedType(TypeDefinition type, string fullName)
    {
        foreach (TypeDefinition nested in type.NestedTypes)
        {
            if (nested.FullName == fullName)
            {
                return nested;
            }

            TypeDefinition child = FindNestedType(nested, fullName);
            if (child != null)
            {
                return child;
            }
        }

        return null;
    }

    private static FieldDefinition GetField(TypeDefinition type, string name)
    {
        FieldDefinition field = type.Fields.FirstOrDefault(f => f.Name == name);
        if (field == null)
        {
            throw new InvalidOperationException("Field not found: " + type.FullName + "." + name);
        }

        return field;
    }

    private static MethodDefinition GetPropertyGetter(TypeDefinition type, string propertyName)
    {
        PropertyDefinition property = type.Properties.FirstOrDefault(p => p.Name == propertyName);
        if (property == null || property.GetMethod == null)
        {
            throw new InvalidOperationException("Getter not found: " + type.FullName + "." + propertyName);
        }

        return property.GetMethod;
    }

    private static MethodDefinition GetPropertySetter(TypeDefinition type, string propertyName)
    {
        PropertyDefinition property = type.Properties.FirstOrDefault(p => p.Name == propertyName);
        if (property == null || property.SetMethod == null)
        {
            throw new InvalidOperationException("Setter not found: " + type.FullName + "." + propertyName);
        }

        return property.SetMethod;
    }

    private static MethodDefinition GetConstructor(TypeDefinition type, params string[] parameterTypeNames)
    {
        return GetMethod(type, ".ctor", parameterTypeNames);
    }

    private static MethodDefinition GetMethod(TypeDefinition type, string name, params string[] parameterTypeNames)
    {
        MethodDefinition method = type.Methods.FirstOrDefault(m =>
            m.Name == name &&
            m.Parameters.Count == parameterTypeNames.Length &&
            ParametersMatch(m, parameterTypeNames));
        if (method == null)
        {
            throw new InvalidOperationException("Method not found: " + type.FullName + "." + name);
        }

        return method;
    }

    private static bool ParametersMatch(MethodDefinition method, string[] parameterTypeNames)
    {
        for (int i = 0; i < parameterTypeNames.Length; i++)
        {
            if (method.Parameters[i].ParameterType.FullName != parameterTypeNames[i])
            {
                return false;
            }
        }

        return true;
    }
}
