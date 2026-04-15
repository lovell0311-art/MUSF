
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

using ETModel;

namespace ETHotfix
{
    [EventMethod(typeof(GameAreaComponent), EventSystemType.INIT)]
    public class GameAreaComponentEventOnInit : ITEventMethodOnInit<GameAreaComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(GameAreaComponent b_Component)
        {
            b_Component.OnInit();
        }
    }
    public static class GameAreaComponentSystem
    {
        public static void OnInit(this GameAreaComponent self)
        {
            self.UpdateAreaServerDataAsync().Coroutine();
        }

        /// <summary>
        /// 更新游戏区服数据
        /// </summary>
        /// <param name="b_Component"></param>
        private static async Task UpdateAreaServerDataAsync(this GameAreaComponent b_Component)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            var mDBProxyComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            if (mTimerComponent == null || mDBProxyComponent == null)
            {
                Log.Error($"mTimerComponent:{mTimerComponent == null},mDBProxyComponent:{mDBProxyComponent == null}");
            }

            while (mTimerComponent != null && mDBProxyComponent != null && b_Component.IsDisposeable != true)
            {
                await mTimerComponent.WaitAsync(10000);
                if (b_Component.IsDisposeable) return;
                try
                {
                    // 正在运行的区服
                    List<ComponentWithId> mRunGameAreaInfos = await mDBProxyComponent.Query<DBGameAreaData>(p => p.State == 1);
                    if (mRunGameAreaInfos == null)
                    {
                        Log.Error("数据库服没有返回区服数据!");
                        continue;
                    }
                    else if (mRunGameAreaInfos.Count == 0)
                    {
                        Log.Error("数据库没有区服数据!");
                        continue;
                    }

                    // 查找60秒前的数据  
                    long mDateTimeTicks = CustomFrameWork.TimeHelper.ClientNowSeconds() - 60;
                    // 查找正在运行的服务器 更新时间在60秒内活跃认为是在运行的
                    List<ComponentWithId> mGameActvityServerInfos = await mDBProxyComponent.Query<DBServiceRegistryInfo>(p => p.UpdateTime >= mDateTimeTicks);
                    if (mGameActvityServerInfos == null || mGameActvityServerInfos.Count == 0)
                    {
                        Log.Error("区服数据异常!");
                        continue;
                    }

                    // 通过读取的数据 得出区服的对应服务器的玩家数量
                    var mOnlinePlayerCountData = b_Component.GetOnlinePlayerCount(mGameActvityServerInfos);

                    b_Component.ClearData();
                    List<int> mGameAreaIdKeys = mOnlinePlayerCountData.Keys.ToList();
                    for (int i = 0, len = mGameAreaIdKeys.Count; i < len; i++)
                    {
                        int mGameAreaId = mGameAreaIdKeys[i];
                        // 该区服是否是有效的
                        DBGameAreaData mAreaInfoData = null;
                        for (int j = 0, jlen = mRunGameAreaInfos.Count; j < jlen; j++)
                        {
                            DBGameAreaData mAreaInfoDataTemp = mRunGameAreaInfos[j] as DBGameAreaData;

                            if (mAreaInfoDataTemp == null) continue;
                            if (mAreaInfoDataTemp.RealLine == 0) continue;
                            int mGameAreaRealId = (int)((uint)mAreaInfoDataTemp.GameAreaId << 16 | (uint)mAreaInfoDataTemp.RealLine);
                            if (mGameAreaRealId == mGameAreaId)
                            {
                                mAreaInfoData = mAreaInfoDataTemp;
                                break;
                            }
                        }
                        if (mAreaInfoData != null)
                        {
                            // 区服总人数
                            int mPlayerCount = 0;
                            // 各个服务器人数
                            Dictionary<int, int> mGameServerPlayerInfo = null;
                            if (mOnlinePlayerCountData.TryGetValue(mGameAreaId, out Dictionary<int, int> mDataTemp))
                            {
                                mGameServerPlayerInfo = mDataTemp;
                                mPlayerCount = mDataTemp.Values.Sum();
                            }

                            Log.Debug(mGameAreaId.ToString());
                            C_GameAreaInfo mInfo = Root.CreateBuilder.GetInstance<C_GameAreaInfo, int>(mGameAreaId);
                            mInfo.NickName = mAreaInfoData.NickName;
                            mInfo.CreateTime = mAreaInfoData.CreateTime;
                            mInfo.GameServerInfo = mGameServerPlayerInfo;
                            mInfo.PlayerCount = mPlayerCount;
                            mInfo.State = mAreaInfoData.State;
                            b_Component.ServerQueryResult.Add(mInfo);
                        }
                    }

                    for (int j = 0, jlen = mRunGameAreaInfos.Count; j < jlen; j++)
                    {
                        DBGameAreaData mAreaInfoDataTemp = mRunGameAreaInfos[j] as DBGameAreaData;

                        if (mAreaInfoDataTemp == null) continue;
                        if (mAreaInfoDataTemp.RealLine != 0) continue;
                        int mGameAreaId = (int)((uint)mAreaInfoDataTemp.GameAreaId << 16 | (uint)mAreaInfoDataTemp.RealLine);
                        C_GameAreaInfo mInfo = Root.CreateBuilder.GetInstance<C_GameAreaInfo, int>(mGameAreaId);
                        mInfo.NickName = mAreaInfoDataTemp.NickName;
                        mInfo.CreateTime = mAreaInfoDataTemp.CreateTime;
                        mInfo.GameServerInfo = new Dictionary<int, int>();
                        mInfo.PlayerCount = 0;
                        mInfo.State = mAreaInfoDataTemp.State;
                        b_Component.ServerQueryResult.Add(mInfo);

                        Log.Debug(mGameAreaId.ToString());
                    }
                    if (b_Component.ServerQueryResult.Count > 1)
                    {
                        b_Component.ServerQueryResult.Sort((left, right) =>
                        {
                            if (left.GameAreaId > right.GameAreaId)
                                return -1;
                            else if (left.GameAreaId == right.GameAreaId)
                                return 0;
                            else
                                return 1;
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        /// <summary>
        /// 获取在线玩家数量
        /// 区服的对应服务器的玩家数量
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        /// <param name="b_GameServerDatas"></param>
        /// <returns></returns>
        private static Dictionary<int, Dictionary<int, int>> GetOnlinePlayerCount(this GameAreaComponent b_CustomComponent,
            List<ComponentWithId> b_GameServerDatas)
        {

            // 区服在线人数  区服id 服务器id 人数
            Dictionary<int, Dictionary<int, int>> mResult = new Dictionary<int, Dictionary<int, int>>();
            for (int i = 0, len = b_GameServerDatas.Count; i < len; i++)
            {
                DBServiceRegistryInfo mData = b_GameServerDatas[i] as DBServiceRegistryInfo;
                if (mData == null) continue;

                // 可能是当前服务器在运行多个区服
                Dictionary<int, List<int>> mGameAreaIds = null;
                try
                {
                    mGameAreaIds = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(mData.GameAreaIds);
                }
                catch (Exception e)
                {
                    Log.Error($"mData.GameAreaIds:{e.Message}");
                }
                // 区服对应的玩家数量
                Dictionary<int, int> mGameAreaPlayerCountDic = null;
                try
                {
                    mGameAreaPlayerCountDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(mData.PlayerCount);
                }
                catch (Exception e)
                {
                    Log.Error($"mData.PlayerCount:{e.Message}");
                }
                // 区服信息
                if (mGameAreaIds != null)
                {
                    List<int> mRunKeylist = mGameAreaIds.Keys.ToList();
                    for (int j = 0, jlen = mRunKeylist.Count; j < jlen; j++)
                    {
                        int mGameAreaId = mRunKeylist[j];
                        var mGameAreaIdlist = mGameAreaIds[mGameAreaId];

                        // 人数信息
                        if (mGameAreaPlayerCountDic != null)
                        {
                            if (mGameAreaPlayerCountDic.TryGetValue(mGameAreaId, out int mPlayerCount))
                            {
                                for (int x = 0, xlen = mGameAreaIdlist.Count; x < xlen; x++)
                                {
                                    var mGameAreaIdTemp = mGameAreaIdlist[x];

                                    if (mResult.TryGetValue(mGameAreaIdTemp, out Dictionary<int, int> mTempDic) == false)
                                    {
                                        mTempDic = mResult[mGameAreaIdTemp] = new Dictionary<int, int>();
                                    }

                                    mTempDic[mData.GameServerId] = mPlayerCount;
                                }
                            }
                        }
                    }
                }
            }
            return mResult;
        }

    }
}