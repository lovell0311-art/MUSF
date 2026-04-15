using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_RepairEquipItemRequestHandler : AMActorRpcHandler<C2G_RepairEquipItemRequest, G2C_RepairEquipItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_RepairEquipItemRequest b_Request, G2C_RepairEquipItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_RepairEquipItemRequest b_Request, G2C_RepairEquipItemResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
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

            GamePlayer gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

            //PlayerShopMallComponent playerShopMall = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            //if (playerShopMall.GetPlayerShopState(DeviationType.MinMonthlyCard) == false &&
            //    playerShopMall.GetPlayerShopState(DeviationType.MaxMonthlyCard) == false)
            //{
            //    // TODO 没有月卡，检查玩家附近有没有维修NPC
            //    MapManageComponent mapManage = mServerArea.GetCustomComponent<MapManageComponent>();
            //    MapComponent map = mapManage.GetMapByMapIndex(gamePlayer.UnitData.Index);
            //    MapCellAreaComponent mapCellArea = map.GetMapCellFieldByPos(b_Request.NpcPosX, b_Request.NpcPosY);
            //    if (mapCellArea == null)
            //    {
            //        // 参数错误，没有找到Npc
            //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(721);
            //        b_Reply(b_Response);
            //        return false;
            //    }
            //    if (!mapCellArea.FieldNpcDic.TryGetValue(b_Request.NpcUID, out GameNpc npc))
            //    {
            //        // 参数错误，没有找到Npc
            //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(721);
            //        b_Reply(b_Response);
            //        return false;
            //    }
            //    if (npc.Config.NpcType != (int)NPCTypeEnum.Shop_EquipRepair)
            //    {
            //        // 参数错误，Npc不能维修装备
            //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(722);
            //        b_Reply(b_Response);
            //        return false;
            //    }
            //    int diffX = (npc.X - gamePlayer.UnitData.X);
            //    int diffY = (npc.Y - gamePlayer.UnitData.Y);
            //    if ((diffX * diffX + diffY * diffY) > 10 * 10)
            //    {
            //        // Npc 距离玩家太远
            //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(723);
            //        b_Reply(b_Response);
            //        return false;
            //    }
            //}


            EquipmentComponent equipComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
            if (equipComponent != null)
            {
                using HashSetComponent<Item> itemHashSet = HashSetComponent<Item>.Create();
                int needGold = 0;
                foreach (int positionId in b_Request.EquipPosition)
                {
                    Item item = equipComponent.GetEquipItemByPosition((EquipPosition)positionId);
                    if (item != null)
                    {
                        if (item.GetProp(EItemValue.RepairMoney) <= 0)
                        {
                            // 参数错误，装备卡槽中的物品不能维修
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(718);
                            b_Reply(b_Response);
                            return false;
                        }
                        if (itemHashSet.Add(item))
                        {
                            needGold += item.GetProp(EItemValue.RepairMoney);
                        }
                        else
                        {
                            // 参数错误，请求的卡槽重复
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(719);
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    else
                    {
                        // 参数错误，装备卡槽中没有物品
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(717);
                        b_Reply(b_Response);
                        return false;
                    }
                }

                if (gamePlayer.Data.GoldCoin < needGold)
                {
                    // 维修金币不足
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(720);
                    b_Reply(b_Response);
                    return false;
                }

                // TODO 检查通过，开始维修装备
                //gamePlayer.Data.GoldCoin -= needGold;
                gamePlayer.UpdateCoin(E_GameProperty.GoldCoin, -needGold, $"开始维修装备");

                foreach (Item item in itemHashSet)
                {
                    item.data.DurabilitySmall = 0;
                    item.SetProp(EItemValue.Durability, item.GetProp(EItemValue.DurabilityMax));
                    item.UpdateProp();
                    item.OnlySaveDB();
                    item.SendAllPropertyData(mPlayer);
                }

                equipComponent.ApplyEquipProp();

                //保存数据库
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(gamePlayer.Data, dBProxy2).Coroutine();

                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                mGoldCoinData.Key = (int)E_GameProperty.GoldCoin;
                mGoldCoinData.Value = gamePlayer.Data.GoldCoin;
                mChangeValueMessage.Info.Add(mGoldCoinData);
                mPlayer.Send(mChangeValueMessage);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}