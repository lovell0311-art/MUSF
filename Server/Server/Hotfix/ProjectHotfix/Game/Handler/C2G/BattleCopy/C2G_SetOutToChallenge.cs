using ETModel;
using System;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Net;
using ETModel.Robot;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SetOutToChallengeHandler : AMActorRpcHandler<C2G_SetOutToChallenge, G2C_SetOutToChallenge>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SetOutToChallenge b_Request, G2C_SetOutToChallenge b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_SetOutToChallenge b_Request, G2C_SetOutToChallenge b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            GamePlayer gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gamePlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mConfigDic = mReadConfigComponent.GetJson<TrialTower_RewardsConfigJson>().JsonDic;
            var ItemInfo = mReadConfigComponent.GetJson<Activity_RewardPropsConfigJson>().JsonDic;
            if (mConfigDic.TryGetValue(gamePlayer.CopyCount, out var Info))
            {
                Dictionary<int, int> Item = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(Info.Reward);
                foreach (var item in Item)
                {
                    if (ItemInfo.TryGetValue(item.Key, out var Item1))
                    {
                        for (int i = 0; i < item.Value; ++i)
                        {
                            ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                            itemCreateAttr.Level = Item1.Level;
                            itemCreateAttr.Quantity = Item1.Quantity;
                            itemCreateAttr.OptLevel = Item1.OptLevel;
                            itemCreateAttr.HaveSkill = Item1.HasSkill == 1;
                            itemCreateAttr.SetId = Item1.SetId;
                            itemCreateAttr.OptionExcellent = Item1.OptionExcellent;
                            if(!string.IsNullOrEmpty(Item1.CustomAttrMathod))
                                itemCreateAttr.CustomAttrMethod.AddRange(Item1.CustomAttrMathod.Split(','));

                            var RewardItem = ItemFactory.TryCreate(Item1.ItemId, mPlayer.GameAreaId, itemCreateAttr);
                            if (backpackComponent.AddItem(RewardItem, "试炼塔领取") == false)
                            {
                                MailInfo mailinfo = new MailInfo();
                                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                                mailinfo.MailName = "试炼塔奖励";
                                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                                mailinfo.MailContent = "试炼塔奖励由于背包空间不足，通过邮件发放";
                                mailinfo.MailState = 0;
                                mailinfo.ReceiveOrNot = 0;
                                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                                MailItem mailItem = new MailItem();
                                mailItem.ItemConfigID = Item1.ItemId;
                                mailItem.ItemID = 0;
                                mailItem.CreateAttr = itemCreateAttr;
                                mailinfo.MailEnclosure.Add(mailItem);
                                MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mAreaId).Coroutine();
                            }
                        }
                    }
                }
            }

            //检查和记录通关的层次
            async Task SetTrialGroundCnt(int Cnt)
            {
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
                var mBattleCopyData = mDataCacheManageComponent.Get<DBBattleCopyData>();
                if (mBattleCopyData == null)
                {
                    mBattleCopyData = await mDataCacheManageComponent.Add<DBBattleCopyData>(dBProxy, p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
                }
                var mDatalist2 = mBattleCopyData.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
                if (mDatalist2.Count <= 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1500);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                    b_Reply(b_Response);
                    return;
                }
                DBBattleCopyData battleCopyData = mDatalist2[0];

                battleCopyData.TrialGroundCnt = Cnt;
                mWriteDataComponent.Save(battleCopyData, dBProxy).Coroutine();
                
            }
            if (b_Request.Type == 0)
            {
                SetTrialGroundCnt(gamePlayer.CopyCount).Coroutine();
                gamePlayer.CopyCount = 0;
                gamePlayer.ISNo = false;
                gamePlayer.RemoveCustomComponent<TrialTowerCheckComponent>();
               
                var mTransferPointId1 = 400;
                var mJsonMapDic1 = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
                if (mJsonMapDic1.TryGetValue(mTransferPointId1, out var _TransferPointConfig1) != false)
                {
                    if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(_TransferPointConfig1.MapId, out var mTargetMapComponent) != false)
                    {
                        if (mTargetMapComponent.TransferPointFindTheWayDic.TryGetValue(mTransferPointId1, out var mTransferPointlist1) != false)
                        {
                            if (mTransferPointlist1 != null || mTransferPointlist1.Count > 0)
                            {
                                var mPlayerUnitData = gamePlayer.UnitData;
                                if (mPlayerUnitData != null)
                                {
                                    var mRandomIndex1 = Help_RandomHelper.Range(0, mTransferPointlist1.Count);
                                    var mTransferPoint1 = mTransferPointlist1[mRandomIndex1];

                                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                    DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, gamePlayer.Player.GameAreaId);
                                    // 切换地图
                                    mTargetMapComponent.Switch(gamePlayer, mTransferPoint1.X, mTransferPoint1.Y);
                                    dBProxy.Save(mPlayerUnitData).Coroutine();
                                }
                            }
                        }
                    }
                }
                BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
                if (batteCopyManagerCpt.TrialTowerList.TryGetValue(mPlayer.GameUserId, out var mapComponent))
                {
                    mapComponent.Dispose();
                    batteCopyManagerCpt.TrialTowerList.Remove(mPlayer.GameUserId);
                }

            }
            else
            {
                BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
                if (batteCopyManagerCpt.TrialTowerList.TryGetValue(mPlayer.GameUserId, out var mapComponent))
                {
                    var Enemy = gamePlayer.CurrentMap.GetCustomComponent<EnemyComponent>();
                    if (Enemy != null && Enemy.AllEnemyDic != null)
                    {
                        if (Enemy.AllEnemyDic.Count > 0)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(509);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    
                    SetTrialGroundCnt(gamePlayer.CopyCount).Coroutine();
                    gamePlayer.CopyCount++;
                    gamePlayer.ISNo = false;
                    var mEnemy = mReadConfigComponent.GetJson<TrialTower_MonsterConfigJson>().JsonDic;
                    if (mEnemy.TryGetValue(gamePlayer.CopyCount, out var EnemyInfo))
                    {
                        mapComponent.GetCustomComponent<EnemyComponent>().InitMapEnemy(EnemyInfo.MobId, 1, EnemyInfo.Number, true);
                    }
                    var mFindTheWaySource = gamePlayer.CurrentMap.GetFindTheWay2D(gamePlayer.UnitData.X, gamePlayer.UnitData.Y);
                    if (mFindTheWaySource == null)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(509);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                        b_Reply(b_Response);
                        return false;
                    }
                    int mTransferPointId =10500;
                    var mJsonMapDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
                    if (mJsonMapDic.TryGetValue(mTransferPointId, out var _TransferPointConfig) == false)
                    {
                        // 传送点不存在
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(513);
                        b_Reply(b_Response);
                        return false;
                    }

                    if (mapComponent.TransferPointFindTheWayDic.TryGetValue(mTransferPointId, out var mTransferPointlist) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(517);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("传送点初始化异常!");
                        b_Reply(b_Response);
                        return false;
                    }
                    
                    if (mTransferPointlist == null || mTransferPointlist.Count == 0)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(517);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("传送点初始化异常!");
                        b_Reply(b_Response);
                        return false;
                    }

                    var mRandomIndex = Help_RandomHelper.Range(0, mTransferPointlist.Count);
                    var mTransferPoint = mTransferPointlist[mRandomIndex];
                    gamePlayer.Move(mTransferPoint.X, mTransferPoint.Y, false);
                    // 公告移动信息
                    //mapComponent.MoveSendNotice(mFindTheWaySource, mTransferPoint, gamePlayer);
                    //mapComponent.MoveSendNotice(null, mTransferPoint, gamePlayer);

                }
            }
            b_Response.Cnt = gamePlayer.CopyCount;
            b_Reply(b_Response);
            return true;
        }
    }
}
