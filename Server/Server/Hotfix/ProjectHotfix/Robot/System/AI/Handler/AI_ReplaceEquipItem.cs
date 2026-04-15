using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using System.Threading.Tasks;

using static ETModel.Robot.AI_ReplaceEquipItem;

namespace ETHotfix.Robot
{
    /// <summary>
    /// 更新装备
    /// </summary>
    [AIHandler]
    public class AI_ReplaceEquipItem : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            RobotBackpackComponent backpack = localUnit.GetComponent<RobotBackpackComponent>();
            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            if (backpack.WhereFromItemType(EquipItemTypeList, p => CanReplaceEquip(localUnit,equipment, p)).Count > 0) return 0;  // 有可以更换的装备
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            // 开始换装备
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;
            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            RobotBackpackComponent backpack = localUnit.GetComponent<RobotBackpackComponent>();
            // 可以更换的装备列表
            List<RobotItem> itemList = backpack.WhereFromItemType(EquipItemTypeList, p => CanReplaceEquip(localUnit, equipment, p));

            MultiMap<EquipPosition, RobotItem> equipMap = new MultiMap<EquipPosition, RobotItem>();
            
            foreach(RobotItem item in itemList)
            {
                equipMap.Add((EquipPosition)item.Config.Slot, item);
            }

            if(equipMap.GetDictionary().TryGetValue(EquipPosition.Weapon,out var weaponList))
            {
                // 需要更换装备
                RobotItem weapon = weaponList.OrderBy(item => CanReplaceEquip(localUnit, equipment, item)).Last();
                equipMap.Remove(EquipPosition.Weapon);

                if(weapon.Type == EItemType.Bows || weapon.Type == EItemType.Crossbows)
                {
                    RobotItem shield = equipment.GetItem(EquipPosition.Shield);
                    if (equipMap.GetDictionary().TryGetValue(EquipPosition.Shield, out var shieldList))
                    {
                        if (weapon.Type == EItemType.Bows)
                        {
                            // 更换的装备是弓
                            // 主动更换弓箭，箭袋
                            shield = shieldList.Where(item => item.Type == EItemType.Arrow || item.Config.Id == 40019)
                                .OrderBy(item => CanReplaceEquip(localUnit, equipment, item)).Last();
                        }
                        else if (weapon.Type == EItemType.Crossbows)
                        {
                            // 更换的装备是弩 
                            // 主动更换弩箭，箭袋
                            shield = shieldList.Where(item => item.Type == EItemType.Arrow || item.Config.Id == 50012)
                                .OrderBy(item => CanReplaceEquip(localUnit, equipment, item)).Last();
                        }
                    }
                    
                    equipMap.Remove(EquipPosition.Shield);
                    bool ret = await RobotEquipmentHelper.ReplaceEquipItem(localUnit, shield, EquipPosition.Shield, cancellationToken);
                    if (cancellationToken.IsCancel()) return;
                    if (ret == false) return;
                }

                bool ret2 = await RobotEquipmentHelper.ReplaceEquipItem(localUnit, weapon, EquipPosition.Weapon, cancellationToken);
                if (cancellationToken.IsCancel()) return;
                if (ret2 == false) return;

            }

            // 开始更换其他部分装备
            foreach(var kv in equipMap.GetDictionary())
            {
                RobotItem item = kv.Value.OrderBy(item => CanReplaceEquip(localUnit, equipment, item)).Last();
                bool ret = await RobotEquipmentHelper.ReplaceEquipItem(localUnit, item, kv.Key, cancellationToken);
                if (cancellationToken.IsCancel()) return;
                if (ret == false) return;
            }
        }




        #region 是否可以更换装备相关判断
        /// <summary>
        /// 计算物品价值
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int ClacItemPrice(RobotItem item)
        {
            int itemPrice = item.GetComponent<NumericComponent>().GetAsInt((int)EItemValue.BuyMoney);
            if(item.Config.Id == 40019 ||
                item.Config.Id == 50012 ||
                item.Type == EItemType.Arrow)
            {
                // 弓箭、弩箭、价值都比较高
                return itemPrice + 10000000;
            }
            return itemPrice;
        }

        public static bool CanReplaceEquip(Unit localUnit, RobotEquipmentComponent equipment,RobotItem item)
        {
            bool ret = false;
            RobotItem equip = equipment.GetItem((EquipPosition)item.Config.Slot);
            if(equip == null)
            {
                ret = equipment.CanEquip(item);
            }
            else if (equipment.CanEquip(item))
            {
                int equipPrice = ClacItemPrice(equip);
                int itemPrice = ClacItemPrice(item);


                // 背包中的物品价值比装备的高
                ret = (equipPrice < itemPrice);
            }

            if(ret)
            {
                // 弓弩，箭筒特殊判断
                // 检查玩家背包或装备栏有个有一套的装备
                if(item.Config.Id == 40019)
                {
                    // 弓箭
                    return GetBows(localUnit).Count != 0;
                }
                else if(item.Config.Id == 50012)
                {
                    // 弩箭
                    return GetCrossbows(localUnit).Count != 0;
                }
                else if(item.Type == EItemType.Arrow)
                {
                    // 箭筒
                    return GetBowsAndCrossbows(localUnit).Count != 0;
                }
                else if(item.Type == EItemType.Bows)
                {
                    // 弓
                    return GetBowsArrow(localUnit).Count != 0;
                }
                else if (item.Type == EItemType.Crossbows)
                {
                    // 弩
                    return GetCrossbowsArrow(localUnit).Count != 0;
                }

                if(item.Type == EItemType.Shields)
                {
                    // 要换的装备是盾牌
                    // 检查武器栏是不是弓或弩
                    RobotItem weapon = equipment.GetItem(EquipPosition.Weapon);
                    if(weapon != null)
                    {
                        if (weapon.Type == EItemType.Bows || weapon.Type == EItemType.Crossbows)
                        {
                            // 弓箭手优先使用弓、弩
                            return false;
                        }
                    }

                }
            }
            return ret;
        }

        private static List<RobotItem> GetBowsArrow(Unit unit)
        {
            List<RobotItem> itemList = new List<RobotItem>();
            RobotBackpackComponent backpack = unit.GetComponent<RobotBackpackComponent>();
            RobotEquipmentComponent equipment = unit.GetComponent<RobotEquipmentComponent>();

            RobotItem arrow = equipment.GetItem(EquipPosition.Shield);
            if(arrow != null)
            {
                if (arrow.Type == EItemType.Arrow || arrow.Config.Id == 40019) itemList.Add(arrow);
            }

            itemList.AddRange(backpack.WhereFromItemType(EItemType.Arrow, item=> equipment.CanEquip(item)));
            itemList.AddRange(backpack.GetItemsByConfigId(40019).Where(item => equipment.CanEquip(item)));

            return itemList;
        }

        private static List<RobotItem> GetBows(Unit unit)
        {
            List<RobotItem> itemList = new List<RobotItem>();
            RobotBackpackComponent backpack = unit.GetComponent<RobotBackpackComponent>();
            RobotEquipmentComponent equipment = unit.GetComponent<RobotEquipmentComponent>();

            RobotItem weapon = equipment.GetItem(EquipPosition.Weapon);
            if (weapon != null)
            {
                if (weapon.Type == EItemType.Bows) itemList.Add(weapon);
            }

            itemList.AddRange(backpack.WhereFromItemType(EItemType.Bows, item => equipment.CanEquip(item)));

            return itemList;
        }

        private static List<RobotItem> GetCrossbowsArrow(Unit unit)
        {
            List<RobotItem> itemList = new List<RobotItem>();
            RobotBackpackComponent backpack = unit.GetComponent<RobotBackpackComponent>();
            RobotEquipmentComponent equipment = unit.GetComponent<RobotEquipmentComponent>();

            RobotItem arrow = equipment.GetItem(EquipPosition.Shield);
            if (arrow != null)
            {
                if (arrow.Type == EItemType.Arrow || arrow.Config.Id == 50012) itemList.Add(arrow);
            }

            itemList.AddRange(backpack.WhereFromItemType(EItemType.Arrow, item => equipment.CanEquip(item)));
            itemList.AddRange(backpack.GetItemsByConfigId(50012).Where(item => equipment.CanEquip(item)));

            return itemList;
        }

        private static List<RobotItem> GetCrossbows(Unit unit)
        {
            List<RobotItem> itemList = new List<RobotItem>();
            RobotBackpackComponent backpack = unit.GetComponent<RobotBackpackComponent>();
            RobotEquipmentComponent equipment = unit.GetComponent<RobotEquipmentComponent>();

            RobotItem weapon = equipment.GetItem(EquipPosition.Weapon);
            if (weapon != null)
            {
                if (weapon.Type == EItemType.Crossbows) itemList.Add(weapon);
            }

            itemList.AddRange(backpack.WhereFromItemType(EItemType.Crossbows, item => equipment.CanEquip(item)));

            return itemList;
        }


        private static List<RobotItem> GetBowsAndCrossbows(Unit unit)
        {
            List<RobotItem> itemList = new List<RobotItem>();
            RobotBackpackComponent backpack = unit.GetComponent<RobotBackpackComponent>();
            RobotEquipmentComponent equipment = unit.GetComponent<RobotEquipmentComponent>();

            RobotItem weapon = equipment.GetItem(EquipPosition.Weapon);
            if (weapon != null)
            {
                if (weapon.Type == EItemType.Bows || weapon.Type == EItemType.Crossbows) itemList.Add(weapon);
            }

            itemList.AddRange(backpack.WhereFromItemType(EItemType.Bows, item => equipment.CanEquip(item)));
            itemList.AddRange(backpack.WhereFromItemType(EItemType.Crossbows, item => equipment.CanEquip(item)));

            return itemList;
        }
        #endregion

    }
}
