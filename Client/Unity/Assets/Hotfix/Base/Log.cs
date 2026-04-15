using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
	public static class Log
	{

		public static bool isLog = false;

		static Log() 
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
                isLog =true;
            }
			else 
			{
				isLog = true;
			}

        }
        public static void Trace(string msg)
		{
		  if(isLog)
			ETModel.Log.Trace(msg);
		}
		
		public static void Warning(string msg)
		{
            if (isLog)
                ETModel.Log.Warning(msg);
		}

		public static void Info(string msg)
		{
            if (isLog)
                ETModel.Log.Info(msg);
		}

		public static void Error(Exception e)
		{
			if (isLog)
                ETModel.Log.Error(e.ToStr());
		}

		public static void Error(string msg)
		{
			if (isLog)
                ETModel.Log.Error(msg);
		}

		public static void Debug(string msg)
		{
			
				if (isLog)
                UnityEngine.Debug.Log(msg);
		}
		
		
		
		public static void Msg(object msg)
		{
            if (isLog)
                Debug(Dumper.DumpAsString(msg));
		}

		public static void DebugPurple(string msg)
		{
            if (isLog)
                UnityEngine.Debug.Log(string.Concat("-><color=#9900FF>", msg, "</color>"));
        }
		public static void DebugBrown(string msg)
		{
			if (isLog)
                UnityEngine.Debug.Log(string.Concat("-><color=#FF8100>", msg, "</color>"));

		}
		public static void DebugRed(string msg)
		{

			if (isLog)
                UnityEngine.Debug.Log(string.Concat("-><color=#FF0000>", msg, "</color>"));

		}
		public static void DebugGreen(string msg)
		{
            if (isLog)
                UnityEngine.Debug.Log(string.Concat("-><color=#00FF00>", msg, "</color>"));

		}
		public static void DebugWhtie(string msg)
		{
            if (isLog)
                UnityEngine.Debug.Log(string.Concat("-><color=#FFFFFF>", msg, "</color>"));

		}
		public static void DebugYellow(string msg)
		{
            if (isLog)
                UnityEngine.Debug.Log(string.Concat("<color=#FFED00>", msg, "</color>"));

		}
	}
}