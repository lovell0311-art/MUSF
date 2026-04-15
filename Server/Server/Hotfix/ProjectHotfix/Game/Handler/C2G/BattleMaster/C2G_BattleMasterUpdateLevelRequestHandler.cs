using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BattleMasterUpdateLevelRequestHandler : AMActorRpcHandler<C2G_BattleMasterUpdateLevelRequest, G2C_BattleMasterUpdateLevelResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BattleMasterUpdateLevelRequest b_Request, G2C_BattleMasterUpdateLevelResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_BattleMasterUpdateLevelRequest b_Request, G2C_BattleMasterUpdateLevelResponse b_Response, Action<IMessage> b_Reply)
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

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            DataCacheManageComponent mDataCacheComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();

            var mDataCache_Skill = mDataCacheComponent.Get<DBMasterData>();
            if (mDataCache_Skill == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(611);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return true;
            }
            var mData = mDataCache_Skill.OnlyOne();
            if (mData == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(611);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return true;
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

            bool mHasMaster = mGamePlayer.MasterGroup.TryGetValue(b_Request.BattleMasterId, out var mBattleMaster);
            if (mHasMaster == false)
            {
                var mBattleMasterCreateBuilder = Root.MainFactory.GetCustomComponent<BattleMasterCreateBuilder>();
                mBattleMaster = mBattleMasterCreateBuilder.CreateHeroMaster(b_Request.BattleMasterId, mData);
            }

            if (mBattleMaster == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(604);
                return false;
            }
            // 限制条件满足
            if (mBattleMaster.TryUse(mGamePlayer, null, b_Response) == false)
            {
                if (mHasMaster == false) mBattleMaster.Dispose();

                //b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("前置技能或者上一阶技能等级不足!");
                b_Reply(b_Response);
                return false;
            }

            switch ((E_BattleMaster_Id)b_Request.BattleMasterId)
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
                        if (mData.SkillId.ContainsKey(b_Request.BattleMasterId) == false)
                        {
                            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>();
                            var mJsonDic = mReadConfigComponent.GetJson<BattleMaster_ALLConfigJson>().JsonDic;
                            if (mJsonDic.TryGetValue(b_Request.BattleMasterId, out var mTargetConfig) == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(604);
                                return false;
                            }

                            if (mPlayer.Data.YuanbaoCoin < mTargetConfig.Unlock)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2112);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("大师点数不足!");
                                b_Reply(b_Response);
                                return false;
                            }
                            mGamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -mTargetConfig.Unlock, "超神大师解封技能消耗");

							var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                            mWriteDataComponent.Save(mGamePlayer.Player.Data, dBProxy).Coroutine();

                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, GamePlayer b_GamePlayer, E_GameProperty b_GameProperty)
                            {
                                b_ChangeValue_notice.GameUserId = b_GamePlayer.InstanceId;

                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                mBattleKVData.Key = (int)b_GameProperty;
                                mBattleKVData.Value = b_GamePlayer.GetNumerial(b_GameProperty);
                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                            }
                            G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                            AddPropertyNotice(mChangeValue_notice, mGamePlayer, E_GameProperty.YuanbaoCoin);
                            mPlayer.Send(mChangeValue_notice);

                            mData.SkillId[b_Request.BattleMasterId] = 0;
                            if (mHasMaster == false) mGamePlayer.MasterGroup[b_Request.BattleMasterId] = mBattleMaster;

                            G2C_HotfixKVData mKVData = new G2C_HotfixKVData();
                            mKVData.Key = b_Request.BattleMasterId;
                            mKVData.Value = 0;

                            b_Response.Info.Add(mKVData);
                            b_Response.PropertyPoint = mData.PropertyPoint;

                            mData.Skill = Help_JsonSerializeHelper.Serialize(mData.SkillId);
                            mWriteDataComponent.Save(mData, dBProxy).Coroutine();

                            b_Reply(b_Response);
                            return true;
                        }
                    }
                    break;
                default:
                    break;
            }


            // 自由点数大于消耗点数
            if (mData.PropertyPoint < mBattleMaster.Consume)
            {
                if (mHasMaster == false) mBattleMaster.Dispose();

                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(609);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("大师点数不足!");
                b_Reply(b_Response);
                return false;
            }

            int mMasterLevel = 0;
            mData.SkillId.TryGetValue(b_Request.BattleMasterId, out mMasterLevel);

            // 下一级
            mMasterLevel++;
            if (mBattleMaster.UpLevel.Count == 0)
            {
                if (mMasterLevel > 1)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(610);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("该技能不可加点!");
                    b_Reply(b_Response);
                    return false;
                }
                mMasterLevel = mBattleMaster.Consume;
            }
            else if (mBattleMaster.UpLevel.Contains(mMasterLevel) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(610);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("该技能不可加点!");
                b_Reply(b_Response);
                return false;
            }

            mData.SkillId[b_Request.BattleMasterId] = mMasterLevel;
            mData.PropertyPoint -= mBattleMaster.Consume;

            mBattleMaster.UseSkill(mGamePlayer, null, true);

            if (mHasMaster == false) mGamePlayer.MasterGroup[b_Request.BattleMasterId] = mBattleMaster;

            {
                G2C_HotfixKVData mKVData = new G2C_HotfixKVData();
                mKVData.Key = b_Request.BattleMasterId;
                mKVData.Value = mMasterLevel;

                b_Response.Info.Add(mKVData);
                b_Response.PropertyPoint = mData.PropertyPoint;

                mData.Skill = Help_JsonSerializeHelper.Serialize(mData.SkillId);
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(mData, dBProxy).Coroutine();
            }

            b_Reply(b_Response);
            return true;
        }
    }
}