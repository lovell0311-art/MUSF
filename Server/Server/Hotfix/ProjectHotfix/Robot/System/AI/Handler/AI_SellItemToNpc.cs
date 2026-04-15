using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    /// <summary>
    /// 出售物品给npc
    /// </summary>
    [AIHandler]
    public class AI_SellItemToNpc : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit.GetComponent<RobotBackpackComponent>().IsFull) return 0;  // 背包满了，卖点东西
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;
            Log.Console("[{clientScene.Name}] 去出售物品");
            // 前往npc
            Unit targetUnit = await RobotNpcHelper.GoToNpc(localUnit, aiConfig.NodeParams, cancellationToken);
            if (cancellationToken.IsCancel()) return;
            if (targetUnit == null) return;

            RobotBackpackComponent backpack = localUnit.GetComponent<RobotBackpackComponent>();
            using ListComponent<long> sellItemsUid = ListComponent<long>.Create();
            // 保护的物品，每种物品，之保留一个
            Dictionary<int, RobotItem> protectiveItems = new Dictionary<int, RobotItem>();

            foreach (RobotItem item in backpack.GetAll())
            {
                if(CanSellThisItem(localUnit,item))
                {
                    sellItemsUid.Add(item.Id);
                }
                else
                {
                    if (item.Type == EItemType.Consumables ||
                        item.Type == EItemType.Gemstone ||
                        item.Type == EItemType.FGemstone) continue;
                    if (!item.CanSell()) continue;
                    if(protectiveItems.TryGetValue(item.Config.Id,out RobotItem proItem))
                    {
                        if(AI_ReplaceEquipItem.ClacItemPrice(item) > AI_ReplaceEquipItem.ClacItemPrice(proItem))
                        {
                            // 装备价值大于之前的装备
                            // 把之前的装备出售了
                            protectiveItems[item.Config.Id] = item;
                            sellItemsUid.Add(proItem.Id);
                        }
                        else
                        {
                            // 没之前的装备牛逼，就卖自己
                            sellItemsUid.Add(item.Id);
                        }
                    }
                    else
                    {
                        protectiveItems.Add(item.Config.Id, item);
                    }
                    
                }
            }
            if(sellItemsUid.Count == 0)
            {
                // 背包，满了，没有可以出售的物品
                // TODO 先把消耗品卖了
                foreach (RobotItem item in backpack.GetAll())
                {
                    if (item.Type != EItemType.Consumables) continue;
                    if (!item.CanSell()) continue;
                    sellItemsUid.Add(item.Id);
                }
            }

            foreach (long itemUid in sellItemsUid)
            {
                RobotItem item = backpack.GetItemByUid(itemUid);
                if(item != null)
                {
                    bool ret = await RobotNpcHelper.SellItem(localUnit, targetUnit, item, cancellationToken);
                    if (cancellationToken.IsCancel()) return;
                    if (ret == false) return;
                }
            }
            localUnit.GetComponent<RobotBackpackComponent>().IsFull = false;
        }

        /// <summary>
        /// 能不能出售这个物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool CanSellThisItem(Unit localUnit,RobotItem item)
        {
            if (item.Type == EItemType.Consumables ||
                item.Type == EItemType.Gemstone ||
                item.Type == EItemType.FGemstone) return false;   // 消耗品，宝石，荧光宝石，无需出售
            if (!item.CanSell()) return false;
            if((EquipPosition)item.Config.Slot > EquipPosition.None)
            {
                // 是装备
                RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
                RobotItem equip = equipment.GetItem((EquipPosition)item.Config.Slot);
                if(equip != null)
                {
                    if (AI_ReplaceEquipItem.ClacItemPrice(equip) >= AI_ReplaceEquipItem.ClacItemPrice(item)) return true;   // 装备价值比已装备的低
                    if (!equipment.CanEquipInFuture(item)) return true; // 未来不可以装备这个武器
                    return false;
                }
            }
            else if(item.Type == EItemType.SkillBooks)
            {
                // 是技能书
                RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
                if (!AI_StudySkill.CanUseSkillBook(localUnit,item)) return true; // 无法使用这个技能书
                if (!equipment.CanEquipInFuture(item)) return true; // 未来不可以装备这个武器
                return false;
            }

            return true;
        }
    }
}
