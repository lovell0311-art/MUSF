
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2Game_EnterGameAreaMessageHandler : AMRpcHandler<G2Game_EnterGameAreaMessage, Game2G_EnterGameAreaMessage>
    {
        protected override async Task<bool> BeforeCodeAsync(Session session, G2Game_EnterGameAreaMessage b_Request, Game2G_EnterGameAreaMessage b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGame, b_Request.UserId))
            {
                return await base.BeforeCodeAsync(session, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> CodeAsync(Session session, G2Game_EnterGameAreaMessage b_Request, Game2G_EnterGameAreaMessage b_Response, Action<IMessage> b_Reply)
        {
            if (Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().ServerStopping)
            {
                // 服务器正在关闭，禁止进入
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(114);
                b_Reply(b_Response);
                return false;
            }
            int newGameAreaId = (int)((uint)b_Request.AreaId << 16 | (uint)b_Request.AreaLineId);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(newGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return false;
            }

            //var mIpConnectDic = Root.MainFactory.GetCustomComponent<IpCacheComponent>().IpConnectDic;
            //if (mIpConnectDic.TryGetValue(b_Request.ConnectIp, out var connectCount) && connectCount > ConstData.LoginLimit)
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(118);
            //    b_Reply(b_Response);
            //    return false;
            //}
            //connectCount++;
            //mIpConnectDic[b_Request.ConnectIp] = connectCount;
            //Console.WriteLine($"链接 {b_Request.ConnectIp}:{connectCount}");

            // 加入玩家
            GameUserComponent mGameUserComponent = Root.MainFactory.GetCustomComponent<GameUserComponent>();
            GameUser mGameUser = mGameUserComponent.AddUserByUserId(b_Request.UserId);
            mGameUser.GateServerId = b_Request.GateServerID;
            mGameUser.GameAreaId = b_Request.AreaId;
            mGameUser.GameAreaLineId = b_Request.AreaLineId;
            mGameUser.ConnectIp = b_Request.ConnectIp;

            DataCacheManageComponent mDataCacheComponent = mGameUser.AddCustomComponent<DataCacheManageComponent>();

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            var mDataCache2 = mDataCacheComponent.Get<DBGamePlayerData>();
            if (mDataCache2 == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_Request.AreaId);
                mDataCache2 = await mDataCacheComponent.Add<DBGamePlayerData>(dBProxy2, p => p.UserId == b_Request.UserId
                                                                                          && p.GameAreaId == b_Request.AreaId
                                                                                          && p.IsDisposePlayer == 0, b_Request.AreaId);
            }
            if (mDataCache2.ContainsKey(b_Request.AreaId) == false)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_Request.AreaId);
                var mInitResult = await mDataCache2.DataQueryInit(dBProxy2, p => p.UserId == b_Request.UserId
                                                                               && p.GameAreaId == b_Request.AreaId
                                                                               && p.IsDisposePlayer == 0, b_Request.AreaId);
                if (mInitResult == false)
                {
                    mGameUserComponent.Remove(b_Request.UserId);
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色不存在!");
                    b_Reply(b_Response);
                    return false;
                }
            }

            // 查找角色
            var mDatalist2 = mDataCache2.DataQuery(p => p.UserId == b_Request.UserId
                                                     && p.GameAreaId == b_Request.AreaId
                                                     && p.IsDisposePlayer == 0, b_Request.AreaId);

            if (mDatalist2 == null)
            {
                mGameUserComponent.Remove(b_Request.UserId);
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色不存在!");
                b_Reply(b_Response);
                return false;
            }

            int mLevelMax = 0;
            for (int i = 0, len = mDatalist2.Count; i < len; i++)
            {
                DBGamePlayerData mGamePlayerData = mDatalist2[i];

                if (mLevelMax < mGamePlayerData.Level)
                {
                    mLevelMax = mGamePlayerData.Level;
                }
                b_Response.GameUserIds.Add(mGamePlayerData.Id);
            }

            var mDBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mGameUser.GameAreaId);

            //E_GameOccupation.Spellsword
            {
                var mQueryTypelist = await mDBProxy.Query<DBAccountZoneData>(p => p.Id == mGameUser.UserId);
                if (mQueryTypelist == null || mQueryTypelist.Count == 0)
                {

                }
                else
                {
                    DBAccountZoneData mDBAccountZoneData = mQueryTypelist[0] as DBAccountZoneData;
                    mDBAccountZoneData.DeSerialize();

                    if (mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.Spellsword))
                    {
                        b_Response.GameOccupation.Add((int)E_GameOccupation.Spellsword);
                    }
                    if (mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.Holyteacher))
                    {
                        b_Response.GameOccupation.Add((int)E_GameOccupation.Holyteacher);
                    }
                    if (mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.SummonWarlock))
                    {
                        b_Response.GameOccupation.Add((int)E_GameOccupation.SummonWarlock);
                    }
                    if (mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.Combat))
                    {
                        b_Response.GameOccupation.Add((int)E_GameOccupation.Combat);
                    }
                    if (mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.GrowLancer))
                    {
                        b_Response.GameOccupation.Add((int)E_GameOccupation.GrowLancer);
                    }
                }

                //if (mLevelMax >= 220)
                {
                    int mGameOccupation = (int)E_GameOccupation.Spellsword;
                    if (b_Response.GameOccupation.Contains(mGameOccupation) == false) b_Response.GameOccupation.Add(mGameOccupation);
                }
                //if (mLevelMax >= 320)
                {
                    int mGameOccupation = (int)E_GameOccupation.Holyteacher;
                    if (b_Response.GameOccupation.Contains(mGameOccupation) == false) b_Response.GameOccupation.Add(mGameOccupation);
                }
                //if (mLevelMax >= 400)
                {
                    int mGameOccupation = (int)E_GameOccupation.SummonWarlock;
                    if (b_Response.GameOccupation.Contains(mGameOccupation) == false) b_Response.GameOccupation.Add(mGameOccupation);
                }
                //if (mLevelMax >= 400)
                {
                    int mGameOccupation = (int)E_GameOccupation.Combat;
                    if (b_Response.GameOccupation.Contains(mGameOccupation) == false) b_Response.GameOccupation.Add(mGameOccupation);
                }
                //if (mLevelMax >= 400)
                {
                    int mGameOccupation = (int)E_GameOccupation.GrowLancer;
                    if (b_Response.GameOccupation.Contains(mGameOccupation) == false) b_Response.GameOccupation.Add(mGameOccupation);
                }

                /*
                if (mLevelMax >= 250)
                {
                    int mGameOccupation = (int)E_GameOccupation.Holyteacher;
                    if (b_Response.GameOccupation.Contains(mGameOccupation) == false) b_Response.GameOccupation.Add(mGameOccupation);
                }
                */
            }

            Log.PLog("Login", $"[a:{mGameUser.UserId}] 连接 GameServer");

            b_Reply(b_Response);
            return true;
        }
    }
}