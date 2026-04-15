using System;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork;

namespace ETModel
{

    public class ReplComponent : Component
    {
        public ScriptOptions ScriptOptions;
        public ScriptState ScriptState;
        public CancellationTokenSource CancellationTokenSource;
        public StringBuilder PrintContext = new StringBuilder();


    }
}