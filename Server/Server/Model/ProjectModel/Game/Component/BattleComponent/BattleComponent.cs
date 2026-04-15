using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;


using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETModel
{

    public partial class BattleComponent : TCustomComponent<MapComponent>
    {
        public long UpdateTimeCompensate { get; set; }
        public long UpdateTick { get; set; }

        public long CurrentTimeTick { get; set; }

        public long TimerId;

        public override void Dispose()
        {
            if (IsDisposeable) return;

            UpdateTimeCompensate = 0;
            UpdateTick = 0;
            CurrentTimeTick = 0;

            SyncTimerDispose();
            base.Dispose();
        }


        public static void Log(object b_Message, bool b_IsStackTrace = true,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            CustomFrameWork.Log.PLog("war battle", b_Message.ToString(), callerLineNumber, callerMemberName, callerFilePath);
        }

        public static void Error(object b_Message, bool b_IsStackTrace = true)
        {
            CustomFrameWork.Log.Error(b_Message.ToString());

        }
    }
}