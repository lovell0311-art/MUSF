using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_LockExchangeItemHandler : AMActorRpcHandler<C2G_LockExchangeItem, G2C_LockExchangeItem>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_LockExchangeItem b_Request, G2C_LockExchangeItem b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_LockExchangeItem b_Request, G2C_LockExchangeItem b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
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

            ExchangeComponent exchangeComponent = mPlayer.GetCustomComponent<ExchangeComponent>();
            if (exchangeComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(816);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("交易组件异常!");
                b_Reply(b_Response);
                return false;
            }

            //检测自己是否正在交易中
            if (exchangeComponent.ExchangeTargetGameUserId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(809);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不在交易中，无法调用接口!");
                b_Reply(b_Response);
                return false;
            }

            //检测对方是否在交易中，若不在交易或交易对象不是自己则终止交易
            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, exchangeComponent.ExchangeTargetGameUserId);
            if (targetPlayer != null && targetPlayer.GetCustomComponent<ExchangeComponent>().ExchangeTargetGameUserId == mPlayer.GameUserId)
            {
                //确认锁定状态
                if (exchangeComponent.IsLock != b_Request.LockState)
                {
                    exchangeComponent.IsLock = b_Request.LockState;
                    var notice = new G2C_LockExchangeItem_notice();
                    notice.GameUserId = mPlayer.GameUserId;
                    notice.LockState = b_Request.LockState;
                    mPlayer.Send(notice);
                    targetPlayer.Send(notice);
                    bool targetLock = targetPlayer.GetCustomComponent<ExchangeComponent>().IsLock;
                    if (targetLock && exchangeComponent.IsLock)
                    {
                        //达成交易 验证是否能交换
                        ExchangeComponent targetExchange = targetPlayer.GetCustomComponent<ExchangeComponent>();
                        int mCheck = exchangeComponent.CheckExchange(out string message1);
                        if (mCheck != ErrorCodeHotfix.ERR_Success)
                        {
                            b_Response.Error = mCheck;
                            b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage(message1);
                            b_Reply(b_Response);
                            return false;
                        }
                        int targetCheck = targetExchange.CheckExchange(out string message2);
                        if (targetCheck != ErrorCodeHotfix.ERR_Success)
                        {
                            b_Response.Error = targetCheck;
                            b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage(message2);
                            b_Reply(b_Response);
                            return false;
                        }

                        //交易物品
                        BackpackComponent mBackpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
                        BackpackComponent targetBackpackComponent = targetPlayer.GetCustomComponent<BackpackComponent>();
                        var dbItemDataCache = mPlayer.GetCustomComponent<DataCacheManageComponent>().Get<DBItemData>();
                        var targetDBItemDataCache = targetPlayer.GetCustomComponent<DataCacheManageComponent>().Get<DBItemData>();
                        List<Item> mItemList = new List<Item>();
                        List<Item> targetItemList = new List<Item>();
                        foreach (var item in exchangeComponent.ItemPosDict)
                        {
                            Item targetItem = mBackpackComponent.GetItemByUID(item.Key);
                            mItemList.Add(mBackpackComponent.RemoveItem(targetItem, $"与玩家交易 target.GameUserId={targetPlayer.GameUserId}"));
                            // TODO 转移物品到其他玩家
                            // 将缓存中的物品数据移除
                            targetItem.SetProp(EItemValue.IsLocking, 0);
                            dbItemDataCache.DataRemove(targetItem.data.Id);
                        }
                        foreach (var item in targetExchange.ItemPosDict)
                        {
                            Item targetItem = targetBackpackComponent.GetItemByUID(item.Key);
                            targetItemList.Add(targetBackpackComponent.RemoveItem(targetItem, $"与玩家交易 player.GameUserId={mPlayer.GameUserId}"));
                            // TODO 转移物品到其他玩家
                            // 将缓存中的物品数据移除
                            targetItem.SetProp(EItemValue.IsLocking, 0);
                            targetDBItemDataCache.DataRemove(targetItem.data.Id);
                        }

                        // 将数据写入数据库
                        // 这里开启新协程写入数据库，无法100%保证数据不会丢失
                        foreach (Item item in mItemList)
                        {
                            item.SaveDBNow().Coroutine();
                        }
                        foreach (Item item in targetItemList)
                        {
                            item.SaveDBNow().Coroutine();
                        }


                        for (int i = 0; i < mItemList.Count; i++)
                        {
                            DBTradeLog dbTradeLog = mPlayer.CreateDBTradeLog(targetPlayer);
                            dbTradeLog.ItemUid = mItemList[i].ItemUID;
                            dbTradeLog.Str = $"#交易系统# 与玩家交易 ({mItemList[i].ToLogString()})";
                            DBLogHelper.Write(dbTradeLog);
                            targetBackpackComponent.AddItem(mItemList[i], $"与玩家交易 player.GameUserId={mPlayer.GameUserId}");
                        }
                        for (int i = 0; i < targetItemList.Count; i++)
                        {
                            DBTradeLog dbTradeLog = targetPlayer.CreateDBTradeLog(mPlayer);
                            dbTradeLog.ItemUid = targetItemList[i].ItemUID;
                            dbTradeLog.Str = $"#交易系统# 与玩家交易 ({targetItemList[i].ToLogString()})";
                            DBLogHelper.Write(dbTradeLog);
                            mBackpackComponent.AddItem(targetItemList[i], $"与玩家交易 target.GameUserId={targetPlayer.GameUserId}");
                        }
                        if (exchangeComponent.ExchangeCoin > 0 || targetExchange.ExchangeCoin > 0)
                        {
                            //交易金钱
                            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
                            var targetGamePlayer = targetPlayer.GetCustomComponent<GamePlayer>();
                            if (exchangeComponent.ExchangeCoin > 0)
                            {
                                DBTradeLog dbTradeLog = mPlayer.CreateDBTradeLog(targetPlayer);
                                dbTradeLog.GoldCoin = exchangeComponent.ExchangeCoin;
                                dbTradeLog.Str = $"#交易系统# 与玩家交易金币";
                                DBLogHelper.Write(dbTradeLog);
                            }
                            if (targetExchange.ExchangeCoin > 0)
                            {
                                DBTradeLog dbTradeLog = targetPlayer.CreateDBTradeLog(mPlayer);
                                dbTradeLog.GoldCoin = targetExchange.ExchangeCoin;
                                dbTradeLog.Str = $"#交易系统# 与玩家交易金币";
                                DBLogHelper.Write(dbTradeLog);
                            }
                          

                            //mGamePlayer.Data.GoldCoin += targetExchange.ExchangeCoin - exchangeComponent.ExchangeCoin;
                            //targetGamePlayer.Data.GoldCoin += exchangeComponent.ExchangeCoin - targetExchange.ExchangeCoin;
                            mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, targetExchange.ExchangeCoin - exchangeComponent.ExchangeCoin, $"交易 target.GameUserId={targetPlayer.GameUserId}");
                            targetGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, exchangeComponent.ExchangeCoin - targetExchange.ExchangeCoin, $"交易 player.GameUserId={mPlayer.GameUserId}");

                            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                            mWriteDataComponent.Save(mGamePlayer.Data, dBProxy2).Coroutine();
                            mWriteDataComponent.Save(targetGamePlayer.Data, dBProxy2).Coroutine();
                            //发送推送
                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                            G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                            mGoldCoinData.Key = (int)E_GameProperty.GoldCoin;
                            mGoldCoinData.Value = mGamePlayer.Data.GoldCoin;
                            mChangeValueMessage.Info.Add(mGoldCoinData);

                            mPlayer.Send(mChangeValueMessage);

                            G2C_ChangeValue_notice mChangeValueMessage2 = new G2C_ChangeValue_notice();
                            G2C_BattleKVData mGoldCoinData2 = new G2C_BattleKVData();
                            mGoldCoinData2.Key = (int)E_GameProperty.GoldCoin;
                            mGoldCoinData2.Value = targetGamePlayer.Data.GoldCoin;
                            mChangeValueMessage2.Info.Add(mGoldCoinData2);

                            targetPlayer.Send(mChangeValueMessage2);
                        }

                        mPlayer.Send(new G2C_ExchangeResult_notice()
                        {
                            State = true,
                            ErrorMessage = "交易完成"
                        });
                        targetPlayer.Send(new G2C_ExchangeResult_notice()
                        {
                            State = true,
                            ErrorMessage = "交易完成"
                        });
                        exchangeComponent.EndExchange();
                        targetPlayer.GetCustomComponent<ExchangeComponent>().EndExchange();

                    }
                }
            }
            else
            {
                mPlayer.Send(new G2C_ExchangeResult_notice()
                {
                    State = false,
                    ErrorMessage = "对方不在交易中，交易终止!"
                });
                exchangeComponent.EndExchange();
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(808);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("对方不在交易中或交易对象不是自己，交易终止!");
            }
            b_Reply(b_Response);
            return true;
        }
    }
}