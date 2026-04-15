using ETModel;
using System.Runtime.CompilerServices;
using CustomFrameWork;

namespace ETHotfix
{
    public static partial class LogHelper
    {
        public static void PLog(this Player self,string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            Log.PLog($"a:{self.UserId} r:{self.GameUserId} {message}", callerLineNumber, callerMemberName, callerFilePath);
        }

        public static void PLog(this Player self, string tag, string message,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            Log.PLog($"a:{self.UserId} r:{self.GameUserId} #{tag}# {message}", callerLineNumber, callerMemberName, callerFilePath);
        }
    }
}
