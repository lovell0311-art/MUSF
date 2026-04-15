using ILRuntime.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 套装属性
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 获取套装属性
        /// </summary>
        /// <param name="list"></param>
        public void GetSuitAtr(ref List<string> list)
        {
            if (GetProperValue(E_ItemValue.SetId) is int value && value == 0)
                return;
            //获取套装配置表
            SetItem_TypeConfig setItem_TypeConfig = ConfigComponent.Instance.GetItem<SetItem_TypeConfig>(value);
            if (setItem_TypeConfig == null) 
            {
                Log.DebugRed($"{value} 套装配置不存在");
                return;
            }
            //玩家当前穿戴的装备
            List<KnapsackDataItem> wareEquipList = roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.Values.ToList();
            list.AddRange(GetSuitEquips());//获取套装装备
            list.AddRange(GetSuitAtrs());//获取套装 属性词条

            ///获取套装装备信息
            List<string> GetSuitEquips()
            {
                List<string> list = new List<string>
                {
                    $"<color={ColorTools.Suit_Title}>套装道具装备信息</color>",
                    $"<color={ColorTools.Suit_Equip_Effective}>{setItem_TypeConfig.SetName} 套装</color>"
                };
                //添加套装名字
                for (int i = 0, length= setItem_TypeConfig.ItemsId.Count; i < length; i++)
                {
                    setItem_TypeConfig.ItemsId[i].ToInt64().GetItemInfo_Out(out Item_infoConfig item_Info);
                    list.Add($"<color={EquipColor(setItem_TypeConfig.ItemsId[i])}>{setItem_TypeConfig.SetName} {item_Info.Name}</color>");
                }
                list.Add("");
                return list;
            }
            ///获取套装 属性信息
            List<string> GetSuitAtrs() 
            {
                List<string> list = new List<string> 
                {
                    $"<color={ColorTools.Suit_Title}>套装道具属性信息</color>",
                };
                //以此获取 套装属性
                for (int i = 2; i < 11; i++)
                {
                    var atrlist = GetAtrLists(i);
                    if (atrlist == null || atrlist.Count == 0)
                        continue;
                    list.Add($"<color={ColorTools.Suit_Equip_Effective}>{i}套装效果</color>");
                    list.AddRange(GetSuitAtr(atrlist,i));
                }
                list.Add("");
                return list;
            }
            ///获取套装属性集合
            List<int> GetAtrLists(int suitCount) => suitCount switch 
            {
             2=> setItem_TypeConfig.AttrId2,
             3=> setItem_TypeConfig.AttrId3,
             4=> setItem_TypeConfig.AttrId4,
             5=> setItem_TypeConfig.AttrId5,
             6=> setItem_TypeConfig.AttrId6,
             7=> setItem_TypeConfig.AttrId7,
             8=> setItem_TypeConfig.AttrId8,
             9=> setItem_TypeConfig.AttrId9,
             10=> setItem_TypeConfig.AttrId10,
             _=>null,
            };

            ///根据套装id集合 获取套装属性词条
            List<string> GetSuitAtr(List<int> suits, int index)
            {
                List<string> list = new List<string>();
                for (int i = 0, length=suits.Count; i < length; i++)
                {
                    var suitatr = ConfigComponent.Instance.GetItem<ItemAttrEntry_SetConfig>(suits[i]);
                    if (suits == null) 
                    {
                        Log.DebugRed($"{suits[i]} 套装属性 词条不存在");
                        continue;
                    }
                    list.Add($"<color={GetSuitAtrColorStr(index)}>{suitatr.Name}</color>");
                }
                return list;
            }
            ///获取套装 属性词条颜色 Index =》几套 套装属性
            string GetSuitAtrColorStr(int index)
            {
                int count = 0;//穿戴装备数量

                List<int> setIds = new List<int>();
                for (int i = 0, length = wareEquipList.Count; i < length; i++)
                {
                 
                    //有套装属性 才显示
                    if (setItem_TypeConfig.ItemsId.Contains(wareEquipList[i].ConfigId.ToInt32())&& wareEquipList[i].GetProperValue(E_ItemValue.SetId) == GetProperValue(E_ItemValue.SetId))
                    {
                        //属于 当前套装
                        if (setIds.Contains(wareEquipList[i].ConfigId.ToInt32())==false)
                        { 
                         setIds.Add(wareEquipList[i].ConfigId.ToInt32());
                        }
                        
                        //++count;
                    }
                }
                count = setIds.Count;
                return count>=index ? ColorTools.Suit_Atr_Effective: ColorTools.Suit_Invalid;
        }
            ///是否穿戴了该套装装备
            string EquipColor(long configId)
            {
                return wareEquipList.Exists(r=>r.ConfigId== configId)?ColorTools.Suit_Equip_Effective:ColorTools.Suit_Invalid;
            }

        }

        /// <summary>
        /// 获取套装名字 颜色
        /// </summary>
        /// <returns></returns>
        public (bool, string, string) GetSuitName()
        {
            if (GetProperValue(E_ItemValue.SetId) is int value && value == 0)
                return (false, string.Empty, string.Empty);
            //获取套装配置表
            SetItem_TypeConfig setItem_TypeConfig = ConfigComponent.Instance.GetItem<SetItem_TypeConfig>(value);
            if (setItem_TypeConfig == null)
            {
                
                return (false, string.Empty, string.Empty);
            }
            return (true, setItem_TypeConfig.SetName, "#56abda");
        }

    }
}