using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;



namespace ETHotfix
{
    [EventMethod("GamePlayerDeath")]
    public class GamePlayerDeath_Penalty : ITEventMethodOnRun<ETModel.EventType.GamePlayerDeath>
    {
        public void OnRun(ETModel.EventType.GamePlayerDeath args)
        {
            Player player = args.gamePlayer.Player;

            int mPkNumber = args.gamePlayer.GetNumerial(E_GameProperty.PkNumber);
            // TODO 死亡惩罚 - 金币损失
            if (mPkNumber > 0)
            {
                // 背包金币
                int dropGoldCoin = (int)(args.gamePlayer.Data.GoldCoin * 0.04f);
                if (dropGoldCoin > 0)
                {
                    args.gamePlayer.UpdateCoin(E_GameProperty.GoldCoin, -dropGoldCoin, "死亡惩罚");

                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, player.GameAreaId);
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(player.GameAreaId);
                    mWriteDataComponent.Save(args.gamePlayer.Data, dBProxy).Coroutine();

                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                    G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                    mGoldCoinData.Key = (int)E_GameProperty.GoldCoinChange;
                    mGoldCoinData.Value = -dropGoldCoin;
                    mChangeValueMessage.Info.Add(mGoldCoinData);

                    mGoldCoinData = new G2C_BattleKVData();
                    mGoldCoinData.Key = (int)E_GameProperty.GoldCoin;
                    mGoldCoinData.Value = args.gamePlayer.Data.GoldCoin;
                    mChangeValueMessage.Info.Add(mGoldCoinData);

                    player.Send(mChangeValueMessage);
                }

                //// 仓库金币
                //WarehouseComponent warehouse = player.GetCustomComponent<WarehouseComponent>();
                //dropGoldCoin = (int)(warehouse.Coin * 0.04f);
                //if (dropGoldCoin > 0)
                //{
                //    warehouse.DeductCoin(dropGoldCoin, "死亡惩罚");
                //}
            }


            // TODO 死亡惩罚 - 装备道具掉落
            {
                ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
                readConfig.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(args.map.MapId, out Map_InfoConfig mapConfig);
                if (mapConfig != null)
                {
                    int dropRate = 0;
                    if (mapConfig.ItemDropByMonsterKill == 1)
                    {
                        if (mPkNumber > 43200)
                        {// 极恶 20
                            dropRate = 90;
                        }
                        else if (mPkNumber > 21600)
                        {// 红 10
                            dropRate = 50;
                        }
                        else if (mPkNumber > 0)
                        {// 5
                            dropRate = 25;
                        }
                    }
                    else if (mapConfig.ItemDropByRoleKill == 1)
                    {
                        if (mPkNumber > 43200)
                        {// 极恶 20
                            dropRate = 90;
                        }
                        else if (mPkNumber > 21600)
                        {// 红 10
                            dropRate = 50;
                        }
                        else if (mPkNumber > 0)
                        {// 5
                            dropRate = 25;
                        }
                    }
                    if (dropRate > 0 && Help_RandomHelper.Range(0, 100) < dropRate)
                    {
                        BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
                        using ListComponent<Item> itemList = ListComponent<Item>.Create();
                        foreach (Item item in backpack.Where(item => CanDrop(item)))
                        {
                            itemList.Add(item);
                        }
                        EquipmentComponent equipment = player.GetCustomComponent<EquipmentComponent>();
                        foreach (Item item in equipment.Where(item => CanDrop(item)))
                        {
                            itemList.Add(item);
                        }

                        if (itemList.Count > 0)
                        {
                            Item dropItem = itemList[Help_RandomHelper.Range(0, itemList.Count)];
                            if (dropItem.data.InComponent == EItemInComponent.Backpack)
                            {
                                backpack.DiscardItemToGround(dropItem.ItemUID, args.map, args.gamePlayer.UnitData.X, args.gamePlayer.UnitData.Y, EMapItemCreateType.Other, $"死亡惩罚 PkNumber={mPkNumber}");
                            }
                            else if (dropItem.data.InComponent == EItemInComponent.Equipment)
                            {
                                Item item = equipment.UnloadEquipItem((EquipPosition)dropItem.data.posId, $"死亡惩罚 PkNumber={mPkNumber}");
                                // 设置物品在地面
                                item.data.InComponent = EItemInComponent.Map;
                                item.data.posX = args.gamePlayer.UnitData.X;
                                item.data.posY = args.gamePlayer.UnitData.Y;
                                // 物品已经移出player.DataCacheManageComponent，需要将物品状态立即更新到数据库。
                                // 防止重新上线，存档错误
                                item.SaveDBNow().Coroutine();
                                // 这个物品已经不属于这个玩家，将物品数据从缓存中删除
                                player.GetCustomComponent<DataCacheManageComponent>().Get<DBItemData>().DataRemove(item.data.Id);

                                MapItem mDropItem = MapItemFactory.Create(item, EMapItemCreateType.Other);
                                // 添加拾取保护
                                mDropItem.KillerId.Add(args.gamePlayer.InstanceId);

                                args.map.MapEntityEnter(mDropItem, item.data.posX, item.data.posY);
                            }
                            else
                            {
                                Log.Error($"dropItem.data.InComponent != EItemInComponent.Backpack || dropItem.data.InComponent != EItemInComponent.Equipment");
                            }
                        }
                    }
                }
            }
        }

        public static bool CanDrop(Item item)
        {
            // 3.宝石类道具不会掉落。
            // 4.翅膀类道具不会掉落。
            switch (item.Type)
            {
                case EItemType.Wing:
                case EItemType.Gemstone:
                    return false;
                default: break;
            }
            // 1.被怪物/角色攻击死亡时，付费类道具无法掉落。
            // 5.付费类道具不会掉落。(任务物品)
            if (item.GetProp(EItemValue.IsBind) != 0 ||
                item.GetProp(EItemValue.IsTask) == 1 ||
                item.GetProp(EItemValue.IsLocking) == 1 ||
                item.GetProp(EItemValue.IsUsing) == 1)
            {
                return false;
            }
            // 2.使用再生宝石强化过的装备不会掉落。
            if (item.GetProp(EItemValue.OrecycledID) > 0)
            {
                return false;
            }
            return true;
        }

    }

}
