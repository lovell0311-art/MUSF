
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Threading;
using System.Reflection;

namespace CustomFrameWork.Component
{

    public class ConsoleCommandLineAttribute : BaseAttribute
    {
        public string ConsoleCommandLineName { get; }
        public ConsoleCommandLineAttribute(string b_ConsoleCommandLine)
        {
            this.ConsoleCommandLineName = b_ConsoleCommandLine;
        }
    }

    public class ConsoleComponent : TCustomComponent<MainFactory>
    {
        public CancellationTokenSource CancellationTokenSource;
        public Dictionary<string, Type> ConsoleCommandLineDic;

        public override void Awake()
        {
            if (ConsoleCommandLineDic != null)
            {
                if (ConsoleCommandLineDic.Count > 0)
                {
                    ConsoleCommandLineDic.Clear();
                }
                ConsoleCommandLineDic = null;
            }
            ConsoleCommandLineDic = new Dictionary<string, Type>();
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
            }

            if (ConsoleCommandLineDic != null)
            {
                if (ConsoleCommandLineDic.Count > 0)
                {
                    ConsoleCommandLineDic.Clear();
                }
                ConsoleCommandLineDic = null;
            }

            base.Dispose();
        }
    }
}
