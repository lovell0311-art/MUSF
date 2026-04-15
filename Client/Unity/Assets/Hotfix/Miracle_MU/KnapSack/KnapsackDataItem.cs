using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 背包Item数据实体
    /// </summary>
    [Serializable]
    public  partial class KnapsackDataItem : Entity
    {
        /// <summary>物品所属玩家的id</summary>
        public long GameUserId { get; set; }
        /// <summary>物品唯一的UUID</summary>
        public long UUID { get; set; }
        /// <summary>物品的配置表ID</summary>
        public long ConfigId { get; set; }

        public Item_infoConfig item_Info;
       
        /// <summary>背包起始位置_X</summary>
        public int PosInBackpackX { get; set; }
        /// <summary>背包起始位置_Y</summary>
        public int PosInBackpackY { get; set; }

        //整理背包使用
        public int tempPosInBackpackX;
        public int tempPosInBackpackY;

        /// <summary>物品类型</summary>
        private int itemtype;
        public int ItemType 
        {
         get {
                if (itemtype == 0)
                { 
                return (int)ConfigId / 10000; 
                }
                return itemtype;
            }
        set { 
                itemtype = value; 
            }
        }

        /*
         物品所属的装备部
         1->武器
         2->盾牌
         3->头盔
         4->铠甲
         5->护腿
         6->护手
         7->靴子
         8->翅膀
         9->守护
         10->项链
         11->左戒指
         12->右戒指
         13->旗帜
         14->宠物
         15->背部武器
         16->背部盾牌
         */
        public int Slot;
      
        /// <summary>物品x占用格子</summary>
        public int X;
        /// <summary>物品Y占用格子</summary>
        public int Y;
        /// <summary>物品所需页数（仓库使用）</summary>
        public int Page;

        #region 基础属性
        /// <summary>
        /// 装备属性字典
        /// key:属性
        /// value:属性对应的值
        /// </summary>

        public Dictionary<int, int> ItemValueDic = new Dictionary<int, int>();

        /// <summary>
        /// 属性覆盖
        /// </summary>
        /// <param name="knapsackDataItem"></param>
        public void OverrideProperties(KnapsackDataItem knapsackDataItem) 
        {
            ItemValueDic = knapsackDataItem.ItemValueDic;
        }
        /// <summary>
        /// 更新物品的属性
        /// </summary>
        /// <param name="struct_"></param>
        public void Set(Struct_Property struct_)
        { 
            ItemValueDic[struct_.PropID] = struct_.Value;
            
        }
        public void Set(int key,int value)
        { 
            ItemValueDic[key] =value;
            
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int GetProperValue(E_ItemValue itemType)
        {
            if (ItemValueDic.TryGetValue((int)itemType, out int value))
                return value;
            else return 0;

        }
        /// <summary>
        /// 改变属性
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="value"></param>
        public void SetProperValue(E_ItemValue itemType, int value)
        {
            ItemValueDic[(int)itemType] = value;
        } 
        #endregion

        /// <summary>
        /// 清除属性
        /// </summary>
        public void ClearProper() 
        {
            ItemValueDic.Clear();
            ExecllentEntryDic.Clear();
            ExtraEntryDic.Clear();
            GameUserId = 0;
            UUID = 0;
            ConfigId = -1;
            PosInBackpackX = -1;
            PosInBackpackY = -1;
            Slot = -1;
            X = -1;
            Y = -1;
            Page =-1;
        }

        readonly RoleEntity roleEntity = UnitEntityComponent.Instance.LocalRole;
        //物品名字
        public void GetItemName(ref List<string> list)
        {
             ConfigId.GetItemInfo_Out(out item_Info);
            string lev = this.GetProperValue(E_ItemValue.Level) > 0 ? "+" + this.GetProperValue(E_ItemValue.Level) : "";
            string itemName;
            list.Add("");
            if (GetSuitName().Item1)//有套装属性
            {
                if (IsHaveExecllentEntry)
                {
                    //有卓越属性
                  //  itemName = $"<color={GetSuitName().Item3}><size=25>卓越的\t{GetSuitName().Item2} {item_Info.Name} {lev}</size></color>";
                    itemName = $"卓越的\t{GetSuitName().Item2} {item_Info.Name} {lev}";
                    var strings = SplitStringIntoMultipart(itemName, 15);
                    for (int j = 0; j < strings.Length; j++)
                    {
                        list.Add($"<color={GetSuitName().Item3}><size=25>{strings[j]}</size></color>");
                    }
                }
                else
                {
                   // itemName = $"<color={GetSuitName().Item3}><size=25>{GetSuitName().Item2} {item_Info.Name} {lev}</size></color>";
                    itemName = $"{GetSuitName().Item2} {item_Info.Name} {lev}";
                    var strings = SplitStringIntoMultipart(itemName, 15);
                    for (int j = 0; j < strings.Length; j++)
                    {
                        list.Add($"<color={GetSuitName().Item3}><size=25>{strings[j]}</size></color>");
                    }
                }
            }
            else if (IsHaveExecllentEntry) //卓越属性
            {
                itemName = $"<color={ColorTools.ExcellenceItemColor}>卓越的\t{item_Info.Name}\t{lev}</color>";
                list.Add(itemName);
            }
            else
            {
                //默认装备
                itemName = $"<color={ColorTools.NormalItemNameColor}>{item_Info.Name} {lev}</color>";
                list.Add(itemName);
            }
          //  list.Add(itemName);
            list.Add("");
         
            if(this.GetProperValue(E_ItemValue.IsBind) == 1|| this.GetProperValue(E_ItemValue.IsBind) == 2)
            list.Add($"<color={ColorTools.IsBindItemColor}>[绑定]</color>");//物品是否绑定
            if (this.GetProperValue(E_ItemValue.IsTask) == 1)
                list.Add($"<color={ColorTools.IsBindItemColor}>[任务物品]</color>");//物品是否是任务物品
            if (this.GetProperValue(E_ItemValue.IsUsing) == 1)
                list.Add($"<color={ColorTools.IsBindItemColor}>[使用中]</color>");//物品是否是任务物品
            list.Add("");
        }

        //获取装备数量
        public void GetItemCount(ref List<string> list) 
        {
            if (GetProperValue(E_ItemValue.Quantity) > 1)
            {
                list.Add($"数量：{this.GetProperValue(E_ItemValue.Quantity)}");
            }
        }
        ///购买价格
        public void GetItemBuyPrice(ref List<string> list)
        {
            
            list.Add($"购买价格：{this.GetProperValue(E_ItemValue.BuyMoney)} 金币");
        }
        //出售价格
        public void GetItemSellPrice(ref List<string> list)
        {
           
            list.Add($"出售价格：{this.GetProperValue(E_ItemValue.SellMoney)} 金币");
        }
        //获取摊位 购买价格
        public void GetItemsStallBuyPrice(ref List<string> list)
        {
            list.Add($"购买价格：{this.GetProperValue(E_ItemValue.Stall_BuyPrice)} 金币");
            list.Add($"购买价格：{this.GetProperValue(E_ItemValue.Stall_BuyMoJingPrice)} 魔晶");
            list.Add("");
        }
        //获取摊位 出售价格
        public void GetItemsStallSellPrice(ref List<string> list)
        {
            list.Add($"出售价格：{this.GetProperValue(E_ItemValue.Stall_SellPrice)} 金币");
            list.Add($"出售价格：{this.GetProperValue(E_ItemValue.Stall_SellMoJingPrice)} 魔晶");
            list.Add("");
        }
        //公共基本属性
        public void GetItemCommonBaseAtr(ref List<string> list)
        {
            if (this.GetProperValue(E_ItemValue.DamageMin) is int DamageMin && DamageMin != 0)
            {
                var str = item_Info.TwoHand == 1 ? $"<color={ColorTools.IsBindItemColor}>双手手攻击力</color>" : $"<color={ColorTools.IsBindItemColor}>单手攻击力</color>";
                list.Add($"<color=#74a3e9>{str}：{DamageMin} ~ {this.GetProperValue(E_ItemValue.DamageMax)}</color>");
            }
            if (this.GetProperValue(E_ItemValue.AttackSpeed) is int AttackSpeed && AttackSpeed != 0)
                list.Add($"攻击速度：{AttackSpeed}");
            if (this.GetProperValue(E_ItemValue.Defense) is int Defense && Defense != 0)
                list.Add($"防御力：{Defense}");
            if (this.GetProperValue(E_ItemValue.DefenseRate) is int DefenseRate && DefenseRate != 0)
                list.Add($"防御率：{DefenseRate}");
            if (this.GetProperValue(E_ItemValue.DurabilityMax) is int DurabilityMax && DurabilityMax != 0)
                list.Add($"耐久：[{this.GetProperValue(E_ItemValue.Durability)}/{DurabilityMax}]");
            if (this.GetProperValue(E_ItemValue.RequireLevel) is int RequireLevel && RequireLevel != 0)
            {
                var difValue = RequireLevel - roleEntity.Property.GetProperValue(E_GameProperty.Level);
                list.Add(difValue <= 0 ? $"所需等级：{RequireLevel}" : $"<color={ColorTools.CanNotWareItemColor}>所需等级：{RequireLevel} (还需 {Mathf.Abs(difValue)})</color>");
            }
            if (this.GetProperValue(E_ItemValue.RequireStrength) is int RequireStrength && RequireStrength != 0)
            {
                var difValue = RequireStrength - roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength);
                list.Add(difValue <= 0 ? $"所需力量：{RequireStrength}" : $"<color={ColorTools.CanNotWareItemColor}>所需力量：{RequireStrength} (还需 {Mathf.Abs(difValue)})</color>");
            }
            if (this.GetProperValue(E_ItemValue.RequireAgile) is int RequireAgile && RequireAgile != 0)
            {
                var difValue = RequireAgile - roleEntity.Property.GetProperValue(E_GameProperty.Property_Agility);
                list.Add(difValue <= 0 ? $"所需敏捷：{RequireAgile}" : $"<color={ColorTools.CanNotWareItemColor}>所需敏捷：{RequireAgile} (还需 {Mathf.Abs(difValue)})</color>");
            }
            if (this.GetProperValue(E_ItemValue.RequireEnergy) is int RequireEnergy && RequireEnergy != 0)
            {
                var difValue = RequireEnergy - roleEntity.Property.GetProperValue(E_GameProperty.Property_Willpower);
                list.Add(difValue <= 0 ? $"所需智力：{RequireEnergy}" : $"<color={ColorTools.CanNotWareItemColor}>所需智力：{RequireEnergy} (还需 {Mathf.Abs(difValue)})</color>");
            }
            if (this.GetProperValue(E_ItemValue.RequireVitality) is int RequireVitality && RequireVitality != 0)
            {
                var difValue = RequireVitality - roleEntity.Property.GetProperValue(E_GameProperty.Property_BoneGas);
                list.Add(difValue <= 0 ? $"所需体力：{RequireVitality}" : $"<color={ColorTools.CanNotWareItemColor}>所需体力：{RequireVitality} (还需 {Mathf.Abs(difValue)})</color>");
            }
            if (this.GetProperValue(E_ItemValue.RequireCommand) is int RequireCommand && RequireCommand != 0)
            {
                var difValue = RequireCommand - roleEntity.Property.GetProperValue(E_GameProperty.Property_Command);
                list.Add(difValue <= 0 ? $"所需统率：{RequireCommand}" : $"<color={ColorTools.CanNotWareItemColor}>所需统率：{RequireCommand} (还需 {Mathf.Abs(difValue)})</color>");
            }
            if (this.GetProperValue(E_ItemValue.MagicDamage) is int MagicDamage && MagicDamage != 0)
            {
                list.Add($"<color=#2067a1>魔力提高{MagicDamage}%</color>");
            }
            if (this.GetProperValue(E_ItemValue.Curse) is int Curse && Curse != 0)
            {
                list.Add($"<color=#2067a1>诅咒提高{Curse}%</color>");
            }
            if (this.GetProperValue(E_ItemValue.UpPetAttackPct) is int UpPetAttackPct && UpPetAttackPct != 0)
            {
                list.Add($"<color=#2067a1>宠物攻击力提高{UpPetAttackPct}%</color>");
            }
            list.Add("");
        }
        //等级需求
        public void GetLevNeed(ref List<string> list)
        {
            if (item_Info.ReqLvl is int ReqLvl && ReqLvl != 0)
            {
                //玩家判断等级 是否满足装备 需求等级
                var difValue = ReqLvl - roleEntity.Property.GetProperValue(E_GameProperty.Level);
                list.Add(difValue <= 0 ? $"等级需求：{ReqLvl}" : $"<color={ColorTools.CanNotWareItemColor}>等级需求：{ReqLvl} (还需 {Math.Abs(difValue)})</color>");
            }
        }

        /// <summary>
        /// 获取那些职业 可以使用改装备
        /// </summary>
        /// <param name="list"></param>
        public void GetUserType(ref List<string> list)
        {
            if (string.IsNullOrEmpty(item_Info.UseRole)) return;
            Log.DebugBrown("职业" + item_Info.UseRole);
            //是否全职业可用
            if (item_Info.UseRole.StringToDictionary().Count == 13&&this.ItemType!=(int)E_ItemType.Mounts)
            {
                list.Add("全职业 可使用");
            }
            else
            {
                foreach (var item in item_Info.UseRole.StringToDictionary())
                {
                   // Log.DebugBrown("key" + item.Key + ":value" + item.Value);
                    if (string.IsNullOrEmpty(GetUserRoleName(item.Key, item.Value))) continue;
                    
                  
                    string st = GetUserRoleName(item.Key, item.Value) + " 可以使用";
                    //职业不是本地玩家 或者 本地玩家的转职等级 不满足条件
                    if (item.Key != (int)UnitEntityComponent.Instance.LocalRole.RoleType|| item.Value> (int)UnitEntityComponent.Instance.LocalRole.ClassLev)
                        st = $"<color={ColorTools.CanNotWareItemColor}>{st}</color>";
                    list.Add(st);
                }
            }
            list.Add("");
        }
        /// <summary>
        /// 获取职业类型
        /// </summary>
        /// <param name="Roletype"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetUserRoleName(int Roletype, int type) => ((E_RoleType)Roletype) switch
        {
            E_RoleType.Magician => type == 0 ? "魔法师" : (type <= 2 ? "魔导师" : (type == 3 ? "神导师、" : "大法师")),
            E_RoleType.Swordsman => type == 0 ? "剑士" : (type <= 2 ? "骑士" : (type == 3 ? "神骑士" : "圣殿武士")),
            E_RoleType.Archer => type == 0 ? "弓箭手" : (type <= 2 ? "圣射手" : (type == 3 ? "神射手" : "精灵游侠")),
            E_RoleType.Magicswordsman => type<= 2? "魔剑士" : (type == 3 ? "剑圣" : "魔骑士"),
            E_RoleType.Holymentor => type <= 2 ? "圣导师" : (type == 3 ? "祭祀" : "大领主"),
            E_RoleType.Summoner => type == 0 ? "召唤术师" : (type <= 2 ? "召唤导师" : (type == 3 ? "召唤巫师" : "大召唤师")),
            E_RoleType.Gladiator => type <= 3 ? "格斗家" : (type == 3 ? "格斗大师" : "格斗宗师"),
            E_RoleType.GrowLancer => type <= 2 ? "梦幻骑士" : (type == 3 ? "魅影骑士" : "光辉骑士"),
          //  E_RoleType.Runemage => type == 0 ? "符文法师" : (type <= 2 ? "秘咒法师" : (type == 3 ? "奥术大师" : "灵魂咏者")),
          //  E_RoleType.StrongWind => type == 0 ? "疾风" : (type <= 2 ? "逐影" : (type == 3 ? "碎梦" : "斩灵")),
          //  E_RoleType.Gunners => type == 0 ? "火枪手" : (type <= 2 ? "狙击手" : (type == 3 ? "枪术大师" : "枪神")),
          //  E_RoleType.WhiteMagician => type == 0 ? "白发魔女" : (type <= 2 ? "光导师" : (type == 3 ? "闪耀导师" : "光明法师")),
          //  E_RoleType.WomanMagician => type == 0 ? "女法师" : (type <= 2 ? "战争法师" : (type == 3 ? "大女法师" : "迷雾法师")),
            _ => string.Empty
        };
        //技能
        public void GetItemSkill(ref List<string> list)
        {
            if (GetProperValue(E_ItemValue.SkillId) is int value && value != 0)
            {
                //赤焰兽
                if (item_Info.Id == 260018)
                {
                    switch (roleEntity.RoleType)
                    {
                        case E_RoleType.Magician:
                            list.Add($"技能：黑龙波 (MP:90)");
                            list.Add("");
                            break;
                        case E_RoleType.Swordsman:
                        case E_RoleType.Magicswordsman:
                            list.Add($"技能：天雷闪闪 (MP:15)");
                            list.Add("");
                            break;
                        case E_RoleType.Archer:
                            list.Add($"技能：双重箭 (MP:5)");
                            list.Add("");
                            break;
                        case E_RoleType.Holymentor:
                            break;
                        case E_RoleType.Summoner:
                            break;
                        case E_RoleType.Gladiator:
                            break;
                        case E_RoleType.GrowLancer:
                            break;
                        case E_RoleType.Runemage:
                            break;
                        case E_RoleType.StrongWind:
                            break;
                        case E_RoleType.Gunners:
                            break;
                        case E_RoleType.WhiteMagician:
                            break;
                        case E_RoleType.WomanMagician:
                            break;
                        default:
                            break;
                    }
                    return;
                }
                value.GetSkillInfos__Out(out SkillInfos skillInfos);
                list.Add($"技能：{skillInfos.Name} (MP:{skillInfos.Consume.GetValue(1)})");
                list.Add("");

            }
        }

      
        /// <summary>
        /// 获取备注信息
        /// </summary>
        /// <param name="list"></param>
        public void GetRemarks(ref List<string> list)
        {
            if (!string.IsNullOrEmpty(item_Info.Prompt)) 
            {
                if (item_Info.Prompt.Contains('_'))
                {
                    var infos = item_Info.Prompt.Split('_');

                    for (int i = 0; i < infos.Length; i++)
                    {
                     
                        list.Add(infos[i]);
                    }
                }
                else
                {
                    var strings = SplitStringIntoMultipart(item_Info.Prompt, 13);
                    for (int j = 0; j < strings.Length; j++)
                    {
                        list.Add(strings[j]);
                    }
                }
                
            }
        }

        /// <summary>
        /// 获取门票开启时间
        /// </summary>
        /// <param name="list"></param>
        public void GetAdmissionTicketOpenTime(ref List<string> list) 
        {
            if (this.ConfigId == 320014|| this.ConfigId==320099)//恶魔广场入场券
            {
                BattleCopyConfig_OpenConfig _OpenConfig = ConfigComponent.Instance.GetItem<BattleCopyConfig_OpenConfig>(1);
                list.Add("<color=red>开放时间</color>");
                list.Add($"{_OpenConfig.OpenTime1.Replace('+',':')}、{_OpenConfig.OpenTime2.Replace('+', ':')}、{_OpenConfig.OpenTime3.Replace('+', ':')}、{_OpenConfig.OpenTime4.Replace('+', ':')}、{_OpenConfig.OpenTime5.Replace('+', ':')}");
                
                list.Add($"{_OpenConfig.OpenTime6.Replace('+', ':')}、{_OpenConfig.OpenTime7.Replace('+', ':')}、{_OpenConfig.OpenTime8.Replace('+', ':')}、{_OpenConfig.OpenTime9.Replace('+', ':')}、{_OpenConfig.OpenTime10.Replace('+', ':')}"); 

                list.Add($"{_OpenConfig.OpenTime11.Replace('+', ':')}、{_OpenConfig.OpenTime12.Replace('+', ':')}、{_OpenConfig.OpenTime13.Replace('+', ':')}、{_OpenConfig.OpenTime14.Replace('+', ':')}、{_OpenConfig.OpenTime15.Replace('+', ':')}"); 

                list.Add($"{_OpenConfig.OpenTime16.Replace('+', ':')}、{_OpenConfig.OpenTime17.Replace('+', ':')}、{_OpenConfig.OpenTime18.Replace('+', ':')}");//、{_OpenConfig.OpenTime19.Replace('+', ':')}

            }
            else if (this.ConfigId == 320015|| this.ConfigId == 320303)//血色城堡入场券
            {
                BattleCopyConfig_OpenConfig _OpenConfig = ConfigComponent.Instance.GetItem<BattleCopyConfig_OpenConfig>(2);
                list.Add("<color=red>开放时间</color>");
                list.Add($"{_OpenConfig.OpenTime1.Replace('+', ':')}、{_OpenConfig.OpenTime2.Replace('+', ':')}、{_OpenConfig.OpenTime3.Replace('+', ':')}、{_OpenConfig.OpenTime4.Replace('+', ':')}、{_OpenConfig.OpenTime5.Replace('+', ':')}");

                list.Add($"{_OpenConfig.OpenTime6.Replace('+', ':')}、{_OpenConfig.OpenTime7.Replace('+', ':')}、{_OpenConfig.OpenTime8.Replace('+', ':')}、{_OpenConfig.OpenTime9.Replace('+', ':')}、{_OpenConfig.OpenTime10.Replace('+', ':')}");

                list.Add($"{_OpenConfig.OpenTime11.Replace('+', ':')}、{_OpenConfig.OpenTime12.Replace('+', ':')}、{_OpenConfig.OpenTime13.Replace('+', ':')}、{_OpenConfig.OpenTime14.Replace('+', ':')}、{_OpenConfig.OpenTime15.Replace('+', ':')}");

                list.Add($"{_OpenConfig.OpenTime16.Replace('+', ':')}、{_OpenConfig.OpenTime17.Replace('+', ':')}、{_OpenConfig.OpenTime18.Replace('+', ':')}");
            }
        }

        /// <summary>
        /// 获取有效时间
        /// </summary>
        /// <param name="list"></param>
        public void GetVaildTime(ref List<string> list)
        {
            if (this.GetProperValue(E_ItemValue.ValidTime) == 0) return;
           //TimeSpan timeSpan=  TimeHelper.GetSpacingTime_Seconds(this.GetProperValue(E_ItemValue.ValidTime));
          //  Log.DebugGreen($"有效时间:{timeSpan.Days}:{timeSpan.Hours}:{timeSpan.Minutes}");
            long time=this.GetProperValue(E_ItemValue.ValidTime)- TimeHelper.GetNowSecond();
            list.Add($"有效时间：{Mathf.Floor(time / (60 * 60 * 24))}天{Mathf.Floor((time % (60 * 60 * 24)) / (60 * 60))}时{Mathf.Floor((time % (60 * 60)) / 60)}分");

        }

        /// <summary>
        /// 根据配置表设置装备的基础属性
        /// </summary>
        public void SetBaseAtr() 
        {
           ConfigId.GetItemInfo_Out(out item_Info);
           SetProperValue(E_ItemValue.RequireLevel, item_Info.ReqLvl);
           SetProperValue(E_ItemValue.DamageMin, item_Info.DamageMin);
           SetProperValue(E_ItemValue.DamageMax, item_Info.DamageMax);
           SetProperValue(E_ItemValue.AttackSpeed, item_Info.AttackSpeed);
           SetProperValue(E_ItemValue.Durability, item_Info.Durable);
           SetProperValue(E_ItemValue.Defense, item_Info.Defense);
           SetProperValue(E_ItemValue.DefenseRate, item_Info.DefenseRate);
           SetProperValue(E_ItemValue.RequireStrength, item_Info.ReqStr);
           SetProperValue(E_ItemValue.RequireAgile, item_Info.ReqAgi);
           SetProperValue(E_ItemValue.RequireEnergy, item_Info.ReqEne);
           SetProperValue(E_ItemValue.RequireVitality, item_Info.ReqVit);
           SetProperValue(E_ItemValue.RequireCommand, item_Info.ReqCom);
           SetProperValue(E_ItemValue.MagicDamage, item_Info.MagicPct);
           SetProperValue(E_ItemValue.Curse, item_Info.Curse);
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            ClearProper();
        }

    }
}