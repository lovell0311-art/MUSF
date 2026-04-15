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
    public class C2G_SweepTheTrialGroundsHandler : AMActorRpcHandler<C2G_SweepTheTrialGrounds, G2C_SweepTheTrialGrounds>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SweepTheTrialGrounds b_Request, G2C_SweepTheTrialGrounds b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_SweepTheTrialGrounds b_Request, G2C_SweepTheTrialGrounds b_Response, Action<IMessage> b_Reply)
        {
            b_Reply(b_Response);
            return true;
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
            var Title = mPlayer.GetCustomComponent<PlayerTitle>();
            if (Title == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
            DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            if (batteCopyManagerCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2600);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到副本管理组件");
                b_Reply(b_Response);
                return false;
            }
            var mBattleCopyData = mDataCacheManageComponent.Get<DBBattleCopyData>();
            if (mBattleCopyData == null)
            {
                mBattleCopyData = await mDataCacheManageComponent.Add<DBBattleCopyData>(dBProxy, p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            }
            var mDatalist2 = mBattleCopyData.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            if (mDatalist2.Count == 0)
            {
                DBBattleCopyData bBattleCopyData = new DBBattleCopyData()
                {
                    Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                    GameUserId = mPlayer.GameUserId,
                    GameAreaId = mPlayer.GameAreaId,
                    demonSquaeNum = batteCopyManagerCpt.demonSquaeNum,
                    redCastleNum = batteCopyManagerCpt.redCastleNum,
                    TrialTowerNum = 0,
                    updateTime = Help_TimeHelper.GetCurrenTimeStamp(),
                };
                bool mSaveResult = await dBProxy.Save(bBattleCopyData);
                if (mSaveResult == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("保存数据失败!");
                    b_Reply(b_Response);
                    return false;
                }
                mBattleCopyData.DataAdd(bBattleCopyData);
            }

            mDatalist2 = mBattleCopyData.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            if (mDatalist2.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1500);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return false;
            }
            DBBattleCopyData battleCopyData = mDatalist2[0];
            DateTime UpdateDate = Help_TimeHelper.ConvertStringToDateTime(battleCopyData.updateTime);
            DateTime CurrentDate = DateTime.Now;
            if (UpdateDate.Day != CurrentDate.Day)
            {
                battleCopyData.demonSquaeNum = batteCopyManagerCpt.demonSquaeNum;
                battleCopyData.redCastleNum = batteCopyManagerCpt.redCastleNum;
                battleCopyData.TrialTowerNum = 0;
                battleCopyData.updateTime = Help_TimeHelper.GetCurrenTimeStamp();
            }
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mConfigDic = mReadConfigComponent.GetJson<TrialTower_ExpendConfigJson>().JsonDic;

            if (mConfigDic.TryGetValue(battleCopyData.TrialTowerNum + 1, out var value))
            {

                int Ceng = battleCopyData.TrialTowerNum + 1;
                int Expend = value.Expend;
                if (!Title.CheckTitle(60012))
                {
                    if (Ceng >= 4)
                    {
                        Ceng -= 2;
                        if (mConfigDic.TryGetValue(Ceng, out var value1))
                            Expend = value1.Expend;
                    }
                    else
                        Expend = 0;

                }
                Expend += 20;
                if (Expend != 0 && Expend <= mPlayer.Data.YuanbaoCoin)
                {
                    gamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -Expend, "试炼塔消耗");
                    mWriteDataComponent.Save(gamePlayer.Data, dBProxy).Coroutine();
                    G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                    mBattleKVData.Key = (int)E_GameProperty.YuanbaoCoin;
                    mBattleKVData.Value = gamePlayer.GetNumerial(E_GameProperty.YuanbaoCoin);
                    mChangeValue_notice.Info.Add(mBattleKVData);
                    mPlayer.Send(mChangeValue_notice);
                }
                else if (Expend != 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3312);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("魔晶不足!");
                    b_Reply(b_Response);
                    return false;
                }
                battleCopyData.TrialTowerNum++;
                mWriteDataComponent.Save(battleCopyData, dBProxy).Coroutine();

            }

            var mRewardsDic = mReadConfigComponent.GetJson<TrialTower_RewardsConfigJson>().JsonDic;
            var ItemInfo = mReadConfigComponent.GetJson<Activity_RewardPropsConfigJson>().JsonDic;
            if (battleCopyData.TrialGroundCnt != 0)
            {
                for (int i = 1; i <= battleCopyData.TrialGroundCnt; ++i)
                {
                    if (mRewardsDic.TryGetValue(i, out var Info))
                    {
                        Dictionary<int, int> Item = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(Info.Reward);
                        foreach (var item in Item)
                        {
                            if (ItemInfo.TryGetValue(item.Key, out var Item1))
                            {
                                for (int j = 0; j < item.Value; ++j)
                                {
                                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                    itemCreateAttr.Level = Item1.Level;
                                    itemCreateAttr.Quantity = Item1.Quantity;
                                    itemCreateAttr.OptLevel = Item1.OptLevel;
                                    itemCreateAttr.HaveSkill = Item1.HasSkill == 1;
                                    itemCreateAttr.SetId = Item1.SetId;

                                    itemCreateAttr.OptionExcellent = Item1.OptionExcellent;
                                    if (!string.IsNullOrEmpty(Item1.CustomAttrMathod))
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
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}
