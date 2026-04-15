using System;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using CustomFrameWork;

namespace ETHotfix
{
    [ObjectSystem]
    public class ReplComponentAwakeSystem : AwakeSystem<ReplComponent>
    {
        public override void Awake(ReplComponent self)
        {
            self.Load();
        }
    }

    [ObjectSystem]
    public class ReplComponentLoadSystem : LoadSystem<ReplComponent>
    {
        public override void Load(ReplComponent self)
        {
            self.CancellationTokenSource?.Cancel();
            self.CancellationTokenSource = null;
            self.Load();
        }
    }

    [ObjectSystem]
    public class ReplComponentDestroySystem : DestroySystem<ReplComponent>
    {
        public override void Destroy(ReplComponent self)
        {
            self.CancellationTokenSource?.Cancel();
            self.CancellationTokenSource = null;
            self.ScriptState = null;
            self.ScriptOptions = null;
        }
    }

    public static class ReplComponentSystem
    {
        public static void Load(this ReplComponent self)
        {
            self.ScriptState = null;
            using ListComponent<Assembly> assemblyList = ListComponent<Assembly>.Create();
            assemblyList.Add(typeof(ReplComponent).Assembly);
            assemblyList.Add(typeof(Root).Assembly);
            assemblyList.Add(typeof(HotfixHelper).Assembly);

            using ListComponent<string> importsList = ListComponent<string>.Create();
            importsList.Add("System");
            importsList.Add("System.Linq");
            importsList.Add("System.Collections.Generic");
            importsList.Add("ETModel");
            importsList.Add("ETHotfix");
            importsList.Add("CustomFrameWork");

            self.ScriptOptions = ScriptOptions.Default
              .WithMetadataResolver(ScriptMetadataResolver.Default.WithBaseDirectory(Environment.CurrentDirectory))
              .AddReferences(assemblyList)
              .AddImports(importsList);
        }


        public static async Task RunCode(this ReplComponent self,string code)
        {
            self.CancellationTokenSource?.Cancel();
            self.CancellationTokenSource = new CancellationTokenSource();
            self.PrintContext.Clear();
            try
            {
                if (self.ScriptState == null)
                {
                    self.ScriptState = await CSharpScript.RunAsync("Action<string> Print = (string msg) => { PrintContext.Append(msg);PrintContext.Append(\"\\n\"); };", self.ScriptOptions, globals: self,cancellationToken: self.CancellationTokenSource.Token);
                }

                self.ScriptState = await self.ScriptState.ContinueWithAsync(code, cancellationToken: self.CancellationTokenSource.Token);
                self.PrintContext.Append("返回结果:\n");
                self.PrintContext.Append((self.ScriptState.ReturnValue == null)?"null":Help_JsonSerializeHelper.Serialize(self.ScriptState.ReturnValue));
            }
            catch (Exception e)
            {
                self.PrintContext.Append(e.ToString());
            }
        }
    }
}
