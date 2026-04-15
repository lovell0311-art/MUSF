using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenBattleMasterRequestHandler : AMActorRpcHandler<C2G_OpenBattleMasterRequest, G2C_OpenBattleMasterResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenBattleMasterRequest b_Request, G2C_OpenBattleMasterResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_OpenBattleMasterRequest b_Request, G2C_OpenBattleMasterResponse b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mServerArea.GameAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (!ConstServer.PlayerMaster)
            {
                if (mGamePlayer.Data.Level < 400)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(601);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色等级不足400级!");
                    b_Reply(b_Response);
                    return false;
                }
                if (mGamePlayer.Data.OccupationLevel < 3)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(602);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色转职次数少于3次!");
                    b_Reply(b_Response);
                    return false;
                }
            }
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            DataCacheManageComponent mDataCacheComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();

            var mDataCache = mDataCacheComponent.Get<DBMasterData>();
            if (mDataCache == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mDataCache = await HelpDb_DBMasterData.Init(mPlayer, mDataCacheComponent, dBProxy2);
            }
            var mData = mDataCache.OnlyOne();
            if (mData == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(611);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return true;
            }
            if (mData.SkillId == null) mData.DeSerialize();

            var mMasterGrouplist = mGamePlayer.MasterGroup.Values.ToArray();

            if (mMasterGrouplist.Length > 0)
            {
                for (int i = 0, len = mMasterGrouplist.Length; i < len; i++)
                {
                    var mMasterGroupItem = mMasterGrouplist[i];

                    G2C_HotfixKVData mKVData = new G2C_HotfixKVData();
                    mKVData.Key = mMasterGroupItem.Id;
                    mKVData.Value = mMasterGroupItem.Data.SkillId[mMasterGroupItem.Id];

                    b_Response.Info.Add(mKVData);
                }
            }

            if (mData.SkillId.Count > 0)
            {
                foreach (var item in mData.SkillId)
                {
                    switch ((E_BattleMaster_Id)item.Key)
                    {
                        case E_BattleMaster_Id.Common2001:
                        case E_BattleMaster_Id.Common2002:
                        case E_BattleMaster_Id.Common2003:
                        case E_BattleMaster_Id.Common2004:
                        case E_BattleMaster_Id.Common2005:
                        case E_BattleMaster_Id.Common2006:
                        case E_BattleMaster_Id.Common2007:
                        case E_BattleMaster_Id.Common2008:
                        case E_BattleMaster_Id.Common2009:
                        case E_BattleMaster_Id.Common2010:
                        case E_BattleMaster_Id.Common2011:
                        case E_BattleMaster_Id.Common2012:
                        case E_BattleMaster_Id.Common2013:
                            {
                                if (item.Value == 0)
                                {
                                    G2C_HotfixKVData mKVData = new G2C_HotfixKVData();
                                    mKVData.Key = item.Key;
                                    mKVData.Value = item.Value;

                                    b_Response.Info.Add(mKVData);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            b_Response.PropertyPoint = mData.PropertyPoint;

            b_Reply(b_Response);
            return true;
        }
    }
}