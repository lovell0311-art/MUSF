
using CustomFrameWork.Baseic;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace CustomFrameWork.Component
{
    public class OptionComponent : TCustomComponent<MainFactory, string[]>
    {
        /// <summary>
        /// 命令行参数
        /// </summary>
        public static Options Options { get; private set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="b_Args">参数</param>
        public override void Awake(string[] b_Args)
        {
            Options = Parse<Options>(b_Args);
            Help_UniqueValueHelper.Init();
        }
        public T Parse<T>(string[] b_Args) where T : new()
        {
            T mResult = new T();

            Dictionary<string, PropertyInfo> mPropertyInfoDic = new Dictionary<string, PropertyInfo>();
            Dictionary<string, OptionAttribute> mOptionAttributeDic = new Dictionary<string, OptionAttribute>();

            PropertyInfo[] mPropertyInfos = typeof(T).GetProperties();
            for (int i = 0, len = mPropertyInfos.Length; i < len; i++)
            {
                PropertyInfo mPropertyInfo = mPropertyInfos[i];
                OptionAttribute mOptionAttribute = mPropertyInfo.GetCustomAttribute<OptionAttribute>();
                if (mOptionAttribute != null && mOptionAttribute.OptionIgnore == false)
                {
                    mOptionAttributeDic[mOptionAttribute.Name] = mOptionAttribute;
                    mPropertyInfoDic[mOptionAttribute.Name] = mPropertyInfo;
                }
            }
            for (int i = 0, len = b_Args.Length; i < len; i++)
            {
                string args = b_Args[i];
                string[] mCommands = args.Split("=");
                if (mCommands.Length != 2)
                {
                    throw new Exception($"=>命令行格式错误!\n\"{args}\"");
                }
                string mCommandName = mCommands[0].TrimStart('-');
                if (mPropertyInfoDic.ContainsKey(mCommandName))
                {
                    PropertyInfo mPropertyInfo = mPropertyInfoDic[mCommandName];
                    mPropertyInfo.SetValue(mResult, Help_TypeSerializeHelper.DeSerialize(mCommands[1], mPropertyInfo.PropertyType));
                    if (mOptionAttributeDic.ContainsKey(mCommandName))
                    {
                        mOptionAttributeDic.Remove(mCommandName);
                    }
                }
            }
            // 没有输入的命令 使用默认值
            List<string> mNoCommandPropertyInfos = mOptionAttributeDic.Keys.ToList();
            for (int i = 0, len = mNoCommandPropertyInfos.Count; i < len; i++)
            {
                string mCommandName = mNoCommandPropertyInfos[i];
                if (mPropertyInfoDic.ContainsKey(mCommandName))
                {
                    PropertyInfo mPropertyInfo = mPropertyInfoDic[mCommandName];
                    mPropertyInfo.SetValue(mResult, mOptionAttributeDic[mCommandName].Default);
                }
            }
            return mResult;
        }

        /// <summary>
        /// 清理
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;
            Options = null;
            base.Dispose();
        }
    }

    /// <summary>
    /// 启动参数类
    /// </summary>
    public class Options
    {
        private string serverTypeName = AppDomain.CurrentDomain.FriendlyName;
        public string serverTypeNameNew { get; set; } = null;

        /// <summary>
        /// 服务器名字 倾向使用项目名
        /// </summary>
        [Option("ServerTypeName", OptionIgnore = true)]
        public string ServerTypeName
        {
            get
            {
                if (serverTypeNameNew != null)
                {
                    return serverTypeNameNew;
                }
                return serverTypeName;
            }
        }
        /// <summary>
        /// AppId
        /// </summary>
        [Option("AppId", Default = 1)]
        public int AppId { get; set; }
        /// <summary>
        /// 服务器类型
        /// </summary>
        [Option("AppType", Default = AppType.Game)]
        public AppType AppType { get; set; }
        /// <summary>
        /// 外网IP地址
        /// </summary>
        [Option("ServerOutIP", Default = "192.168.1.100:54321")]
        public string ServerOutIP { get; set; }
        /// <summary>
        /// 内网IP地址
        /// </summary>
        [Option("ServerInnerIP", Default = "127.0.0.1:54320")]
        public string ServerInnerIP { get; set; }

        /// <summary>
        /// 打印日志
        /// </summary>
        [Option("PointLog", Default = 1)]
        public int PointLog { get; set; }
        /// <summary>
        /// 游戏区服名字
        /// </summary>
        [Option("RunParameter", Default = "[]")]
        public string RunParameter { get; set; }

        /// <summary>
        /// 配置路径
        /// </summary>

        [Option("ConfigPath", Default = "../Config/StartConfig/192.json")]
        public string ConfigPath { get; set; }

        [Option("LogLevel", Default = "Trace")]
        public string LogLevel { get; set; }

        [Option("AppendValue", Default = "")]
        public string AppendValue { get; set; }

        [Option("ZoneId", Default =0)]
        public int ZoneId { get; set; }
    }
}
