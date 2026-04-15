using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_LoginSystemCreateGamePlayerRequestHandler : AMActorRpcHandler<C2G_LoginSystemCreateGamePlayerRequest, G2C_LoginSystemCreateGamePlayerResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_LoginSystemCreateGamePlayerRequest b_Request, G2C_LoginSystemCreateGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
            // 这条消息的 ActorId 是 UserId
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGame, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_LoginSystemCreateGamePlayerRequest b_Request, G2C_LoginSystemCreateGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
            GameUser mGameUser = Root.MainFactory.GetCustomComponent<GameUserComponent>().GetPlayer(b_Request.ActorId);
            if (mGameUser == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(120);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return true;
            }
            // 区服判断
            if (mGameUser.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(121);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return true;
            }

            string mRenameStr = b_Request.NickName.Trim();
            if (mRenameStr.Equals(""))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("更改的名字不能为空!!");
                b_Reply(b_Response);
                return true;
            }
            if (RegexName(mRenameStr) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(314);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("更改的名字不能为空!!");
                b_Reply(b_Response);
                return true;
            }

            bool RegexName(string str)
            {
                return Regex.IsMatch(str, @"^[\u4e00-\u9fa5_a-zA-Z0-9]+$");
            }
            if (mRenameStr.Length > 6)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(315);
                b_Reply(b_Response);
                return true;
            }
            var mNameComponent = Root.MainFactory.GetCustomComponent<NameComponent>();
            if (mNameComponent.Namelist.Contains(mRenameStr))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("更改的名字不能为空!!");
                b_Reply(b_Response);
                return true;
            }

            Session SessionMGMT = null;
            var allConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.MGMT);
            foreach (var config in allConfig)
            {
                List<int> gameAreaIds = JsonHelper.FromJson<List<int>>(config.RunParameter);
                if ((gameAreaIds[0] >> 16) != mGameUser.GameAreaId) continue;

                SessionMGMT = Game.Scene.GetComponent<NetInnerComponent>().Get(config.ServerInnerIP);
            }
            G2M_NameLockRecordRequest g2M_NameLockRecordRequest = new G2M_NameLockRecordRequest();
            g2M_NameLockRecordRequest.Name = mRenameStr;
            var NameLockRecordRequest = await SessionMGMT?.Call(g2M_NameLockRecordRequest);
            if (NameLockRecordRequest == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(303);
                b_Reply(b_Response);
                return true;
            }
            else if (NameLockRecordRequest.Error != 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(304);
                b_Reply(b_Response);
                return true;
            }

            // 角色判断
            Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<CreateRole_InfoConfigJson>().JsonDic.TryGetValue(b_Request.PlayerType, out var mConfig);
            if (mConfig == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(305);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据异常!");
                b_Reply(b_Response);
                return true;
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var mDBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mGameUser.GameAreaId);

            var mQueryNamelist = await mDBProxy.Query<DBGamePlayerData>(p => p.NickName == mRenameStr);
            if (mQueryNamelist != null && mQueryNamelist.Count > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(304);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("昵称重复!");
                b_Reply(b_Response);
                return true;
            }

            DataCacheManageComponent mDataCacheManageComponent = mGameUser.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGamePlayerData>();
            if (mDataCache == null)
            {
                mDataCache = await mDataCacheManageComponent.Add<DBGamePlayerData>(mDBProxy, p => p.UserId == mGameUser.UserId
                                                                                               && p.GameAreaId == mGameUser.GameAreaId
                                                                                               && p.IsDisposePlayer == 0, b_TagId: mGameUser.GameAreaId);
            }


            switch ((E_GameOccupation)b_Request.PlayerType)
            {
                case E_GameOccupation.Spell:
                    break;
                case E_GameOccupation.Swordsman:
                    break;
                case E_GameOccupation.Archer:
                    break;
                case E_GameOccupation.Spellsword:
                    {
                        //var mDatalistTemp = mDataCache.DataQuery(p => p.UserId == mGameUser.UserId
                        //                                           && p.GameAreaId == mGameUser.GameAreaId
                        //                                           && p.Level >= 220
                        //                                           && p.IsDisposePlayer == 0, b_TagId: mGameUser.GameAreaId);
                        //if (mDatalistTemp == null || mDatalistTemp.Count == 0)
                        //{
                        //    var mQueryTypelist = await mDBProxy.Query<DBAccountZoneData>(p => p.Id == mGameUser.UserId);
                        //    if (mQueryTypelist == null || mQueryTypelist.Count == 0)
                        //    {
                        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(309);
                        //        b_Reply(b_Response);
                        //        return true;
                        //    }

                        //    DBAccountZoneData mDBAccountZoneData = mQueryTypelist[0] as DBAccountZoneData;
                        //    mDBAccountZoneData.DeSerialize();

                        //    if (mDBAccountZoneData.RoleDic.Count == 0 || mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.Spellsword) == false)
                        //    {
                        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(309);
                        //        b_Reply(b_Response);
                        //        return true;
                        //    }
                        //}
                    }
                    break;
                case E_GameOccupation.Holyteacher:
                    {
                        //var mDatalistTemp = mDataCache.DataQuery(p => p.UserId == mGameUser.UserId
                        //                                         && p.GameAreaId == mGameUser.GameAreaId
                        //                                         && p.Level >= 320
                        //                                         && p.IsDisposePlayer == 0, b_TagId: mGameUser.GameAreaId);
                        //if (mDatalistTemp == null || mDatalistTemp.Count == 0)
                        //{
                        //    var mQueryTypelist = await mDBProxy.Query<DBAccountZoneData>(p => p.Id == mGameUser.UserId);
                        //    if (mQueryTypelist == null || mQueryTypelist.Count == 0)
                        //    {
                        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(310);
                        //        b_Reply(b_Response);
                        //        return true;
                        //    }

                        //    DBAccountZoneData mDBAccountZoneData = mQueryTypelist[0] as DBAccountZoneData;
                        //    mDBAccountZoneData.DeSerialize();

                        //    if (mDBAccountZoneData.RoleDic.Count == 0 || mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.Holyteacher) == false)
                        //    {
                        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(310);
                        //        b_Reply(b_Response);
                        //        return true;
                        //    }
                        //}
                    }
                    break;
                case E_GameOccupation.SummonWarlock:
                    {
                        //var mDatalistTemp = mDataCache.DataQuery(p => p.UserId == mGameUser.UserId
                        //                                         && p.GameAreaId == mGameUser.GameAreaId
                        //                                         && p.Level >= 400
                        //                                         && p.IsDisposePlayer == 0, b_TagId: mGameUser.GameAreaId);
                        //if (mDatalistTemp == null || mDatalistTemp.Count == 0)
                        //{
                        //    var mQueryTypelist = await mDBProxy.Query<DBAccountZoneData>(p => p.Id == mGameUser.UserId);
                        //    if (mQueryTypelist == null || mQueryTypelist.Count == 0)
                        //    {
                        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(311);
                        //        b_Reply(b_Response);
                        //        return true;
                        //    }

                        //    DBAccountZoneData mDBAccountZoneData = mQueryTypelist[0] as DBAccountZoneData;
                        //    mDBAccountZoneData.DeSerialize();

                        //    if (mDBAccountZoneData.RoleDic.Count == 0 || mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.SummonWarlock) == false)
                        //    {
                        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(311);
                        //        b_Reply(b_Response);
                        //        return true;
                        //    }
                        //}
                    }
                    break;
                case E_GameOccupation.Combat:
                    {
                        //var mQueryTypelist = await mDBProxy.Query<DBAccountZoneData>(p => p.Id == mGameUser.UserId);
                        //if (mQueryTypelist == null || mQueryTypelist.Count == 0)
                        //{
                        //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(312);
                        //    b_Reply(b_Response);
                        //    return true;
                        //}

                        //DBAccountZoneData mDBAccountZoneData = mQueryTypelist[0] as DBAccountZoneData;
                        //mDBAccountZoneData.DeSerialize();

                        //if (mDBAccountZoneData.RoleDic.Count == 0 || mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.Combat) == false)
                        //{
                        //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(312);
                        //    b_Reply(b_Response);
                        //    return true;
                        //}
                    }
                    break;
                case E_GameOccupation.GrowLancer:
                    {
                        //var mQueryTypelist = await mDBProxy.Query<DBAccountZoneData>(p => p.Id == mGameUser.UserId);
                        //if (mQueryTypelist == null || mQueryTypelist.Count == 0)
                        //{
                        //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(313);
                        //    b_Reply(b_Response);
                        //    return true;
                        //}

                        //DBAccountZoneData mDBAccountZoneData = mQueryTypelist[0] as DBAccountZoneData;
                        //mDBAccountZoneData.DeSerialize();

                        //if (mDBAccountZoneData.RoleDic.Count == 0 || mDBAccountZoneData.RoleDic.ContainsKey((int)E_GameOccupation.GrowLancer) == false)
                        //{
                        //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(313);
                        //    b_Reply(b_Response);
                        //    return true;
                        //}
                    }
                    break;
                default:
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(305);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据异常!");
                    b_Reply(b_Response);
                    return true;
            }

            // 保存数据
            DBGamePlayerData mNewData = new DBGamePlayerData()
            {
                Id = IdGeneraterNew.Instance.GenerateUnitId((int)mGameUser.GameAreaId),
                UserId = mGameUser.UserId,
                GameAreaId = (int)mGameUser.GameAreaId,
                PlayerTypeId = mConfig.Id,
                NickName = mRenameStr,
                CreateTime = DateTime.UtcNow,
                CreateTimeTick = TimeHelper.ClientNowSeconds()
            };
            bool mSaveResult = await mDBProxy.Save(mNewData);
            if (mSaveResult == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(306);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色创建失败!");
                b_Reply(b_Response);
                return true;
            }

            if (mDataCache.ContainsKey(mGameUser.GameAreaId) == false)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mGameUser.GameAreaId);
                var mInitResult = await mDataCache.DataQueryInit(dBProxy2, p => p.UserId == mGameUser.UserId
                                                                             && p.GameAreaId == mGameUser.GameAreaId
                                                                             && p.IsDisposePlayer == 0, mGameUser.GameAreaId);
                if (mInitResult == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色不存在!");
                    b_Reply(b_Response);
                    return false;
                }
            }
            else
            {
                mDataCache.DataAdd(mNewData, b_TagId: mGameUser.GameAreaId);
            }

            // 查找角色
            var mDatalist = mDataCache.DataQuery(p => p.UserId == mGameUser.UserId
                                                   && p.GameAreaId == mGameUser.GameAreaId
                                                   && p.IsDisposePlayer == 0, b_TagId: mGameUser.GameAreaId);

            if (mDatalist == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色不存在!");
                b_Reply(b_Response);
                return false;
            }

            for (int i = 0, len = mDatalist.Count; i < len; i++)
            {
                DBGamePlayerData mGamePlayerData = mDatalist[i];

                b_Response.GameIds.Add(mGamePlayerData.Id);
            }
            M2G_NameLockRecord m2G_NameLockRecord1 = new M2G_NameLockRecord();
            m2G_NameLockRecord1.Name = mRenameStr;
            SessionMGMT?.Send(m2G_NameLockRecord1);
            b_Reply(b_Response);
            return true;
        }
    }
}