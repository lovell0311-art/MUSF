using UnityEngine;
using ETModel;
using System;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;

namespace ETHotfix
{
	[ObjectSystem]
	public class LogCollectionComponentAwakeSystem : AwakeSystem<LogCollectionComponent>
	{
        public override void Awake(LogCollectionComponent self)
        {
			LogCollectionComponent.Instance = self;
		}
    }

	[ObjectSystem]
	public class LogCollectionComponentDestroySystem : DestroySystem<LogCollectionComponent>
	{
		public override void Destroy(LogCollectionComponent self)
		{
            LogCollectionComponent.Instance = null;
		}
	}

	public enum ELogType
	{
		trace,
		debug,
		info,
		warn,
		error,
		fatal,
		mark,
	}

	public class LogCollectionComponent : Entity
	{
		public static LogCollectionComponent Instance;

		private const bool DisableRemoteLogUpload = true;

		public long UserId { set { userIdStr = value.ToString(); } }

		public long GameUserId { set { gameUserIdStr = value.ToString(); } }

		public string IP = "";

		public string ApiUrl = "http://10.0.2.2:3080/logger";

		private string userIdStr = "";
		private string gameUserIdStr = "";

		private StringBuilder stringBuilder = new StringBuilder();

		private bool RemoteLogUploadEnabled => false;

        public void Trace(string message,string stackTrace = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
		{
			Log(ELogType.trace, message,stackTrace, callerLineNumber, callerMemberName, callerFilePath).Coroutine();
		}

		public void Debug(string message, string stackTrace = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
		{
			Log(ELogType.debug, message, stackTrace, callerLineNumber, callerMemberName, callerFilePath).Coroutine();
		}

		public void Info(string message, string stackTrace = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
		{
			Log(ELogType.info, message, stackTrace, callerLineNumber, callerMemberName, callerFilePath).Coroutine();
		}

		public void Warn(string message, string stackTrace = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
		{
			Log(ELogType.warn, message, stackTrace, callerLineNumber, callerMemberName, callerFilePath).Coroutine();
		}

		public void Error(string message, string stackTrace = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
		{
			Log(ELogType.error, message, stackTrace, callerLineNumber, callerMemberName, callerFilePath).Coroutine();
		}

		public void Fatal(string message, string stackTrace = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
		{
			Log(ELogType.fatal, message, stackTrace, callerLineNumber, callerMemberName, callerFilePath).Coroutine();
		}

		public void Mark(string message, string stackTrace = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
		{
			Log(ELogType.mark, message, stackTrace, callerLineNumber, callerMemberName, callerFilePath).Coroutine();
		}

		#region Core
		private string ELogType2String(ELogType logType)
		{
			switch(logType)
			{
				case ELogType.trace: return "trace";
				case ELogType.debug: return "debug";
				case ELogType.info: return "info";
				case ELogType.warn: return "warn";
				case ELogType.error: return "error";
				case ELogType.fatal: return "fatal";
				case ELogType.mark: return "mark";
			}
			return "none";
		}

		public async ETTask Log(ELogType logType, string message,string stackTrace,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
		{
			return;
        }

        private HashSet<string> handleErrorSet = new HashSet<string>();
        private Queue<string> handleErrorQueue = new Queue<string>();
        private int handleErrorCount = 10;

        private bool TryAddHandleError(string message)
        {
            if (handleErrorSet.Add(message))
            {
                handleErrorQueue.Enqueue(message);
                if (handleErrorQueue.Count > handleErrorCount)
                {
                    string oldKey = handleErrorQueue.Dequeue();
                    handleErrorSet.Remove(oldKey);
                }
                return true;
            }
            return false;
        }

        public void HandleLog(string logString, string stackTrace, UnityEngine.LogType type)
		{
			return;
		}
		#endregion
	}


	public static class AsyncOptExtensionMethods
	{
		public static ETTask.Awaiter GetAwaiter(this AsyncOperation asyncOp)
		{
			ETTaskCompletionSource tcs = new ETTaskCompletionSource();
			asyncOp.completed += obj =>
			{
				tcs.SetResult();
			};
			return (tcs.Task).GetAwaiter();
		}
	}


}
