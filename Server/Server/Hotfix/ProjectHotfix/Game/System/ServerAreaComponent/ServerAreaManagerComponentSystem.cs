
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ETHotfix
{
    [EventMethod(typeof(ServerAreaManagerComponent), EventSystemType.INIT)]
    public class ServerAreaManagerComponentEventOnInit : ITEventMethodOnInit<ServerAreaManagerComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(ServerAreaManagerComponent b_Component)
        {
            b_Component.OnInit();
        }
    }
    public static class ServerAreaManagerComponentSystem
    {
        public static void OnInit(this ServerAreaManagerComponent self)
        {
            self.TimingQueryTaskDataAsync().Coroutine();
        }

        /// <summary>
        /// 定时拉取数据
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        private static async Task TimingQueryTaskDataAsync(this ServerAreaManagerComponent b_CustomComponent)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            if (mTimerComponent == null)
            {
                Log.Error("TimerComponent == null");
                return;
            }
            while (true)
            {
                await mTimerComponent.WaitAsync(10000);
                try
                {
                    await b_CustomComponent.QueryGameAreaDataAsync();
                }
                catch(Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        /// <summary>
        /// 查当前物理服所运行的游戏区服
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <returns></returns>
        private static async Task QueryGameAreaDataAsync(this ServerAreaManagerComponent b_CustomComponent)
        {
            long mNowTimeTick = DateTime.Now.Ticks;
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            List<ComponentWithId> mQueryInfoResults = await mDBProxy.Query<DBServiceRegistryInfo>(p => p.GameServerId == OptionComponent.Options.AppId);

            if (mQueryInfoResults == null) return;

            if (mQueryInfoResults.Count == 0)
            {
                Dictionary<int, List<int>> keyValuePairs = new Dictionary<int, List<int>>()
                {
                    { 65537, new List<int>(){ 65537 } },
                    { 65538, new List<int>(){ 65538 } },
                };
                string str = Help_JsonSerializeHelper.Serialize(keyValuePairs);
                str = OptionComponent.Options.RunParameter;

                DBServiceRegistryInfo dBServiceRegistryInfo = new DBServiceRegistryInfo()
                {
                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                    PlayerCount = Newtonsoft.Json.JsonConvert.SerializeObject(b_CustomComponent.GetAllGameAreaAllPlayerCount()),
                    GameServerId = OptionComponent.Options.AppId,
                    GameAreaIds = str,
                    UpdateTime = mNowTimeTick,
                    UpdateTime2 = DateTime.UtcNow
                };

                bool mSaveResult = await mDBProxy.Save(dBServiceRegistryInfo);

                if (mSaveResult == false)
                {
                    Log.Error($"数据插入失败:{OptionComponent.Options.AppId}");
                    return;
                }
                else
                {
                    mQueryInfoResults.Add(dBServiceRegistryInfo);
                }
            }
            if (mQueryInfoResults != null && mQueryInfoResults.Count > 0)
            {
                DBServiceRegistryInfo mDBGameServerInfoData = mQueryInfoResults[0] as DBServiceRegistryInfo;

                if (mDBGameServerInfoData == null) return;

                var mGameAreaIds = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(mDBGameServerInfoData.GameAreaIds);
                // 运行其他区服
                b_CustomComponent.RunGameAreaDataAsync(mGameAreaIds);

                mDBGameServerInfoData.UpdateTime = mNowTimeTick;
                mDBGameServerInfoData.UpdateTime2 = DateTime.UtcNow;
                mDBGameServerInfoData.PlayerCount = Newtonsoft.Json.JsonConvert.SerializeObject(b_CustomComponent.GetAllGameAreaAllPlayerCount());

                bool mDataUpdateResult = await mDBProxy.Save(mDBGameServerInfoData);

                if (mDataUpdateResult == false)
                {
                    Log.Error($"数据更新失败:{OptionComponent.Options.AppId}");
                }
                else
                {
                    Log.Info($"数据更新成功:{OptionComponent.Options.AppId}");
                }
            }
        }
        /// <summary>
        /// 运行某个区服
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        private static void RunGameAreaDataAsync(this ServerAreaManagerComponent b_Component, Dictionary<int, List<int>> b_GameAreaIds)
        {
            List<int> mAddGameAreas = new List<int>(b_GameAreaIds.Keys.ToArray());
            List<int> mRunKeyslist = b_Component.GameAreaOnServerDic.Keys.ToList();
            for (int i = 0, len = mRunKeyslist.Count; i < len; i++)
            {
                int mGameAreaId = mRunKeyslist[i];

                if (mAddGameAreas.Contains(mGameAreaId))
                {
                    mAddGameAreas.Remove(mGameAreaId);
                    continue;
                }

                b_Component.RemoveGameArea(mGameAreaId);
            }
            for (int i = 0, len = mAddGameAreas.Count; i < len; i++)
            {
                int mGameAreaId = mAddGameAreas[i];

                if (b_Component.GameAreaOnServerDic.ContainsKey(mGameAreaId) == false)
                {
                    if (b_GameAreaIds.TryGetValue(mGameAreaId, out var mServerMappinglist) == false)
                    {
                        Log.Error(33333.ToString());
                    }

                    var mGameAreaComponent = Root.CreateBuilder.GetInstance<C_ServerArea>(false);
                    mGameAreaComponent.Awake(b_Component, mGameAreaId, mServerMappinglist);
                    b_Component.GameAreaOnServerDic[mGameAreaId] = mGameAreaComponent;
                }
            }

            b_Component.IdMappingDic.Clear();
            mRunKeyslist = b_Component.GameAreaOnServerDic.Keys.ToList();
            for (int i = 0, len = mRunKeyslist.Count; i < len; i++)
            {
                int mGameAreaId = mRunKeyslist[i];

                if (b_GameAreaIds.TryGetValue(mGameAreaId, out var mServerMappinglist))
                {
                    for (int j = 0, jlen = mServerMappinglist.Count; j < jlen; j++)
                    {
                        var mMappingId = mServerMappinglist[j];

                        b_Component.IdMappingDic[mMappingId] = mGameAreaId;
                    }
                }
            }
        }

        private static Dictionary<int, int> GetAllGameAreaAllPlayerCount(this ServerAreaManagerComponent b_Component)
        {
            Dictionary<int, int> mResult = new Dictionary<int, int>();
            if (b_Component.GameAreaOnServerDic.Count > 0)
            {
                List<C_ServerArea> mGameArealist = b_Component.GameAreaOnServerDic.Values.ToList();
                for (int i = 0, len = mGameArealist.Count; i < len; i++)
                {
                    C_ServerArea mGameAreaComponent = mGameArealist[i];
                    var mGameAreaId = mGameAreaComponent.GameAreaId;

                    var mCount = 0;
                    var mPlayerManageComponent = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();
                    if (mPlayerManageComponent != null)
                    {
                        var mAllPlayer = mPlayerManageComponent.GetAllByZone(mGameAreaId);
                        if (mAllPlayer != null) mCount = mAllPlayer.Count;
                    }

                    mResult[mGameAreaComponent.SourceId] = mCount;
                }
            }
            return mResult;
        }

    }
}