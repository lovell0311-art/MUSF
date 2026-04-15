using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BaiTanBuyItemRequestHandler : AMActorRpcHandler<C2G_BaiTanBuyItemRequest, G2C_BaiTanBuyItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BaiTanBuyItemRequest b_Request, G2C_BaiTanBuyItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_BaiTanBuyItemRequest b_Request, G2C_BaiTanBuyItemResponse b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get((int)(b_Request.AppendData >> 16), b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
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

            if (mPlayer.GameUserId == b_Request.BaiTanInstanceId)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2116);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return true;
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer.UnitData.Index != 1)
            {
                // 该地图不能摆摊
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2102);
                b_Reply(b_Response);
                return false;
            }
            if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(1, out var mapComponent) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2103);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常");
                b_Reply(b_Response);
                return false;
            }

            if (mapComponent.TryGetPosX(mGamePlayer.UnitData.X) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2104);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常x,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            var mFindTheWaySource = mapComponent.GetFindTheWay2D(mGamePlayer);
            if (mFindTheWaySource == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2105);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            if (mFindTheWaySource.IsSafeArea == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2120);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("摆摊需要在安全区!");
                b_Reply(b_Response);
                return false;
            }
            var mMapCellField = mapComponent.GetMapCellField(mFindTheWaySource);
            if (mMapCellField == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2106);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置区域数据异常x,不能攻击!");
                b_Reply(b_Response);
                return false;
            }

            if (mMapCellField.MapStallDic.TryGetValue(b_Request.BaiTanInstanceId, out var mStallComponent) == false)
            {
                bool mFoundResult = false;
                for (int i = 0, len = mMapCellField.AroundFieldArray.Length; i < len; i++)
                {
                    var mAroundField = mMapCellField.AroundFieldArray[i];
                    if (mAroundField.AreaIndex == mMapCellField.AreaIndex) continue;

                    if (mAroundField.MapStallDic.TryGetValue(b_Request.BaiTanInstanceId, out mStallComponent))
                    {
                        mFoundResult = true;
                        break;
                    }
                }
                if (mFoundResult == false)
                {
                    // 查找不到你的摊位
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2109);
                    b_Reply(b_Response);
                    return false;
                }
            }

            var mPlayerManageComponent = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();
            Player mTargetPlayer = null;
            for (int i = 0, len = mServerArea.VirtualIdlist.Count; i < len; i++)
            {
                int mTargetAreaId = mServerArea.VirtualIdlist[i] >> 16;
                mTargetPlayer = mPlayerManageComponent.Get(mTargetAreaId, b_Request.BaiTanInstanceId);
                if (mTargetPlayer != null)
                {
                    break;
                }
            }
            if (mTargetPlayer == null)
            {
                // 查找不到你的摊主
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2115);
                b_Reply(b_Response);
                return false;
            }

            DataCacheManageComponent mTargetDataCacheComponent = mTargetPlayer.AddCustomComponent<DataCacheManageComponent>();

            var mDataCache_TargetStall = mTargetDataCacheComponent.Get<DBStallItem>();
            if (mDataCache_TargetStall == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mTargetPlayer.GameAreaId);
                mDataCache_TargetStall = await HelpDb_DBStallItem.Init(mTargetPlayer, mTargetDataCacheComponent, dBProxy2);
            }
            var mTargetData = mDataCache_TargetStall.OnlyOne();
            if (mTargetData == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2100);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return true;
            }
            if (mTargetData.StallItemlist == null) mTargetData.DeSerialize();

            if (mTargetData.StallItemlist.Count < 0)
            {
                // 摊位查找不到物品
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2111);
                b_Reply(b_Response);
                return false;
            }
            if (mTargetData.StallItemlist.TryGetValue(b_Request.ItemUUID, out var mPrice) == false)
            {
                // 摊位查找不到物品
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2108);
                b_Reply(b_Response);
                return false;
            }
            if (mPrice.Item1 > 0)
            {
                if (mGamePlayer.Data.GoldCoin < mPrice.Item1)
                {
                    // 钱不够
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2112);
                    b_Reply(b_Response);
                    return false;
                }
            }
            if (mPrice.Item2 > 0)
            {
                if (mGamePlayer.Player.Data.YuanbaoCoin < mPrice.Item2)
                {
                    // 钱不够
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2112);
                    b_Reply(b_Response);
                    return false;
                }
            }

            if (mStallComponent.keyValuePairs.TryGetValue(b_Request.ItemUUID, out var mItem) == false)
            {
                mItem = ItemFactory.CreateFormDB(b_Request.ItemUUID, mTargetPlayer);
                if (mItem == null)
                {
                    // 查找不到你的摊位
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2110);
                    b_Reply(b_Response);
                    return false;
                }

                mStallComponent.keyValuePairs[b_Request.ItemUUID] = mItem;
            }

            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            int posX = mItem.data.posX, posY = mItem.data.posY;

            if (!backpackComponent.CanAddItem(mItem, b_Request.PosInBackpackX, b_Request.PosInBackpackY))
            {
                //背包装不下
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2113);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("指定位置装不下物品");
                b_Reply(b_Response);
                return false;
            }



            mStallComponent.mItemBox.RemoveItem(mItem.ConfigData.X, mItem.ConfigData.Y, posX, posY);
            mStallComponent.keyValuePairs.Remove(b_Request.ItemUUID);

            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mTargetPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mTargetPlayer.GameAreaId);

            mTargetData.StallItemlist.Remove(b_Request.ItemUUID);
            mTargetData.Serialize();
            mWriteDataComponent.Save(mTargetData, dBProxy).Coroutine();

            // TODO 物品转移到其他玩家
            // 将自己身上的物品缓存删掉
            mItem.data.posX = 0;
            mItem.data.posY = 0;
            mItem.data.GameUserId = 0;
            mItem.data.UserId = 0;
            mItem.data.InComponent = EItemInComponent.None;
            mItem.SaveDBNow().Coroutine();
            mTargetPlayer.GetCustomComponent<DataCacheManageComponent>().Get<DBItemData>().DataRemove(mItem.data.Id);

            Log.PLog("Stall", $"a:{mTargetPlayer.UserId} r:{mTargetPlayer.GameUserId} 物品离开摊位(玩家购买 GameUserId={mPlayer.GameUserId}) {mItem.ToLogString()}");
            // TODO 玩家行为 交易
            DBTradeLog dBTradeLog = mTargetPlayer.CreateDBTradeLog(mPlayer);
            dBTradeLog.ItemUid = mItem.ItemUID;
            dBTradeLog.Str = $"#摆摊系统# 玩家交易物品 ({mItem.ToLogString()})";
            DBLogHelper.Write(dBTradeLog);


            if (!backpackComponent.AddItem(mItem, b_Request.PosInBackpackX, b_Request.PosInBackpackY, $"玩家摊位购买 GameUserId={mTargetPlayer.GameUserId}"))
            {
                // 代码出问题了,不应该可能走到这里
                Log.Error($"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 物品无法添加进背包({mItem.ToLogString()})");
            }


            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, GamePlayer b_GamePlayer, E_GameProperty b_GameProperty)
            {
                b_ChangeValue_notice.GameUserId = b_GamePlayer.InstanceId;

                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)b_GameProperty;
                mBattleKVData.Value = b_GamePlayer.GetNumerial(b_GameProperty);
                b_ChangeValue_notice.Info.Add(mBattleKVData);
            }

            G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
            G2C_ChangeValue_notice mChangeValue_notice2 = new G2C_ChangeValue_notice();
            if (mPrice.Item1 > 0)
            {
                dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, (int)-mPrice.Item1, "摊位买入");
                mWriteDataComponent.Save(mGamePlayer.Data, dBProxy).Coroutine();

                dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mTargetPlayer.GameAreaId);
                mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mTargetPlayer.GameAreaId);
                var mTargetGamePlayer = mTargetPlayer.GetCustomComponent<GamePlayer>();
                var mYuanbaoCoinAdd = (int)(mPrice.Item1 * 0.95f);//mPrice.Item2;扣除百分之5
                mTargetGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, mYuanbaoCoinAdd, "摊位卖出");
                mWriteDataComponent.Save(mTargetGamePlayer.Data, dBProxy).Coroutine();

                AddPropertyNotice(mChangeValue_notice, mGamePlayer, E_GameProperty.GoldCoin);
                AddPropertyNotice(mChangeValue_notice2, mTargetGamePlayer, E_GameProperty.GoldCoin);

                // TODO 玩家行为 交易
                dBTradeLog = mPlayer.CreateDBTradeLog(mTargetPlayer);
                dBTradeLog.GoldCoin = mYuanbaoCoinAdd;
                dBTradeLog.Str = "#摆摊系统# 玩家交易金币";
                DBLogHelper.Write(dBTradeLog);
            }
            if (mPrice.Item2 > 0)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mGamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, (int)-mPrice.Item2, "摊位买入");
                mWriteDataComponent.Save(mGamePlayer.Player.Data, dBProxy2).Coroutine();

                dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mTargetPlayer.GameAreaId);
                mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mTargetPlayer.GameAreaId);
                var mTargetGamePlayer = mTargetPlayer.GetCustomComponent<GamePlayer>();

                var mYuanbaoCoinAdd = (int)(mPrice.Item2 * 0.95f);//mPrice.Item2;扣除百分之5
                mTargetGamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, mYuanbaoCoinAdd, "摊位卖出");
                mWriteDataComponent.Save(mTargetGamePlayer.Player.Data, dBProxy2).Coroutine();

                AddPropertyNotice(mChangeValue_notice, mGamePlayer, E_GameProperty.YuanbaoCoin);
                AddPropertyNotice(mChangeValue_notice2, mTargetGamePlayer, E_GameProperty.YuanbaoCoin);

                // TODO 玩家行为 交易
                dBTradeLog = mPlayer.CreateDBTradeLog(mTargetPlayer);
                dBTradeLog.Yuanbao = mYuanbaoCoinAdd;
                dBTradeLog.Str = "#摆摊系统# 玩家交易魔晶";
                DBLogHelper.Write(dBTradeLog);
            }
            mPlayer.Send(mChangeValue_notice);
            mTargetPlayer.Send(mChangeValue_notice2);

            b_Reply(b_Response);
            return true;
        }
    }
}