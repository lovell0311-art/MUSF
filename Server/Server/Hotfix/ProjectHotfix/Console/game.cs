
using ETModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [ConsoleCommandLineAttribute(ConsoleCommandLinePath.game)]
    public class ConsoleCommandLine_Game : C_ConsoleCommandLine
    {
        public override async Task Run(string b_Contex)
        {
            switch (b_Contex)
            {
                case ConsoleCommandLinePath.area:
                    {
                        Log.Console($"进入了{b_Contex} 子命令!!!");
                    }
                    break;
            }
        }
        public override async Task Run(ModeContexCommandlineComponent b_Component, string b_Contex)
        {
            switch (b_Contex)
            {
                case "shutdown":
                    Help_GameServerHelper.ShutdownWait(1000 * 30).Coroutine();
                    break;
                case "shutdown_now":
                    Help_GameServerHelper.Shutdown().Coroutine();
                    break;
                default:
                    Log.Console($"未知指令:{b_Contex}");
                    break;

            
            }

        }
    }
}
