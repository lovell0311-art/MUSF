
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CustomFrameWork.Baseic;

namespace CustomFrameWork.Component
{
    public partial class ServerManageComponent : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 已经启动的服务
        /// </summary>
        private readonly Dictionary<int, C_StartUpInfoJson> mAllStartUpInfoDics = new Dictionary<int, C_StartUpInfoJson>();
        /// <summary>
        /// 新添加的服务
        /// </summary>
        private List<C_StartUpInfoJson> mAddAllStartUpInfos;
        /// <summary>
        /// 已经启动的服务的端口信息
        /// </summary>
        private readonly Dictionary<int, Dictionary<int, C_StartUpInfo>> mAllStartUpInfoDic = new Dictionary<int, Dictionary<int, C_StartUpInfo>>();
        /// <summary>
        /// 要移除的ID
        /// </summary>
        public List<int> mRemoveServerIDs;


        public Dictionary<int, C_StartUpInfoJson> JsonDic = new Dictionary<int, C_StartUpInfoJson>();

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Awake()
        {
            ConfigInfoComponent c_component = Parent.GetCustomComponent<ConfigInfoComponent>();
            string mStartUpPath = @$"{OptionComponent.Options.ConfigPath}\StartUpConfig\StartUp_ServerConfig.json";
            string Info = File.ReadAllText(mStartUpPath.Replace("\\", "/"));

            JsonDic.Clear();
            mAddAllStartUpInfos = Help_JsonSerializeHelper.DeSerialize<List<C_StartUpInfoJson>>(Info);
            for (int i = 0; i < mAddAllStartUpInfos.Count; i++)
            {
                var mCard_TypeConfig = mAddAllStartUpInfos[i];
                JsonDic[mCard_TypeConfig.AppId] = mCard_TypeConfig;
            }

            // 要移除的ID
            mRemoveServerIDs = mAllStartUpInfoDics.Keys.ToList();
            for (int i = 0, len = mAddAllStartUpInfos.Count; i < len; i++)
            {
                int keyServerID = mAddAllStartUpInfos[i].AppId;
                if (mRemoveServerIDs.Contains(keyServerID))
                {
                    mRemoveServerIDs.Remove(keyServerID);
                }
            }

            mAddAllStartUpInfos = mAddAllStartUpInfos.Where(s => !mAllStartUpInfoDics.ContainsKey(s.AppId)).ToList();

            for (int i = 0, len = mAddAllStartUpInfos.Count; i < len; i++)
            {
                C_StartUpInfoJson mInside_StartUpInfo = mAddAllStartUpInfos[i];
                if (!mAllStartUpInfoDic.TryGetValue((int)mInside_StartUpInfo.AppType, out Dictionary<int, C_StartUpInfo> temp))
                    mAllStartUpInfoDic[(int)mInside_StartUpInfo.AppType] = temp = new Dictionary<int, C_StartUpInfo>();

                C_StartUpInfo mC_StartUpInfo = new C_StartUpInfo()
                {
                    AppId = mInside_StartUpInfo.AppId,
                    AppType = mInside_StartUpInfo.AppType,
                    ServerInnerIP = Help_NetworkHelper.ToIPEndPoint(mInside_StartUpInfo.InnerConfig["Address"]),
                    ServerOutIP = (mInside_StartUpInfo.OuterConfig.ContainsKey("Address") == true) ? Help_NetworkHelper.ToIPEndPoint(mInside_StartUpInfo.OuterConfig["Address"]) : null,
                    OutIP = (mInside_StartUpInfo.OuterConfig.ContainsKey("Address2") == true) ? Help_NetworkHelper.ToIPEndPoint(mInside_StartUpInfo.OuterConfig["Address2"]) : null,
                    PointLog = mInside_StartUpInfo.PointLog,
                    RunParameter = mInside_StartUpInfo.RunParameter,
                    IsPVP = mInside_StartUpInfo.IsPVP,
                    IsVIP = mInside_StartUpInfo.IsVIP,
                    ZoneId = mInside_StartUpInfo.ZoneId,
                };
                if (mC_StartUpInfo.InnerIP == null && mC_StartUpInfo.ServerInnerIP != null) mC_StartUpInfo.InnerIP = mC_StartUpInfo.ServerInnerIP;
                if (mC_StartUpInfo.OutIP == null && mC_StartUpInfo.ServerOutIP != null) mC_StartUpInfo.OutIP = mC_StartUpInfo.ServerOutIP;

                temp[mInside_StartUpInfo.AppId] = mC_StartUpInfo;
            }
            OnEnable();
            for (int i = 0, len = mAddAllStartUpInfos.Count; i < len; i++)
            {
                mAllStartUpInfoDics[mAddAllStartUpInfos[i].AppId] = mAddAllStartUpInfos[i];
            }

            {
                C_StartUpInfo mC_StartUpInfo = GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
                OptionComponent.Options.ServerInnerIP = mC_StartUpInfo.ServerInnerIP != null ? mC_StartUpInfo.ServerInnerIP.ToString() : "";
                OptionComponent.Options.ServerOutIP = mC_StartUpInfo.ServerOutIP != null ? mC_StartUpInfo.ServerOutIP.ToString() : "";
                OptionComponent.Options.RunParameter = mC_StartUpInfo.RunParameter;
                OptionComponent.Options.PointLog = mC_StartUpInfo.PointLog;
                OptionComponent.Options.serverTypeNameNew = OptionComponent.Options.AppType.ToString();
                OptionComponent.Options.ZoneId = mC_StartUpInfo.ZoneId;
            }
        }
        /// <summary>
        /// 清理
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;

            mRemoveServerIDs.Clear();
            mAllStartUpInfoDics.Clear();
            mAddAllStartUpInfos = null;
            mAllStartUpInfoDic.Clear();
            base.Dispose();
        }

        public C_StartUpInfo GetStartUpInfo(int b_ServerID)
        {
            if (mAllStartUpInfoDics.TryGetValue(b_ServerID, out C_StartUpInfoJson temp))
                if (mAllStartUpInfoDic.TryGetValue((int)temp.AppType, out Dictionary<int, C_StartUpInfo> tempDic))
                    if (tempDic.TryGetValue(b_ServerID, out C_StartUpInfo mResult))
                        return mResult;
            return null;
        }
        public Dictionary<int, Dictionary<int, C_StartUpInfo>> GetAll()
        {
            return mAllStartUpInfoDic;
        }
        public C_StartUpInfo GetStartUpInfo(AppType b_ServerType, int b_ServerID)
        {
            if (mAllStartUpInfoDic.TryGetValue((int)b_ServerType, out Dictionary<int, C_StartUpInfo> temp))
                if (temp.TryGetValue(b_ServerID, out C_StartUpInfo mResult))
                    return mResult;
            return null;
        }
        public C_StartUpInfo[] GetStartUpInfos(AppType b_ServerType)
        {
            if (mAllStartUpInfoDic.TryGetValue((int)b_ServerType, out Dictionary<int, C_StartUpInfo> mResult))
                return mResult.Values.ToArray();
            return null;
        }

        public class C_StartUpInfo
        {
            public int AppId { get; set; }
            public AppType AppType { get; set; }
            public IPEndPoint ServerOutIP { get; set; }
            public IPEndPoint ServerInnerIP { get; set; }
            public IPEndPoint InnerIP { get; set; }
            public IPEndPoint OutIP { get; set; }
            public int PointLog { get; set; } = 1;
            public string RunParameter { get; set; } = "[]";
            public int IsPVP { get; set; } = 0;
            public int IsVIP { get; set; } = 0;
            public int ZoneId { get; set; } = 0;
        }

        public class C_StartUpInfoJson
        {
            public int AppId { get; set; }
            public AppType AppType { get; set; }
            public Dictionary<string, string> InnerConfig { get; set; }
            public Dictionary<string, string> OuterConfig { get; set; }
            public int PointLog { get; set; } = 1;
            public string RunParameter { get; set; } = "[]";
            public int IsPVP { get; set; } = 0;
            public int IsVIP { get; set; } = 0;
            public int ZoneId { get; set; } = 0;
        }
    }
}
