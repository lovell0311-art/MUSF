using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;

namespace ETHotfix
{
    [ObjectSystem]
    public class NPCEntityAwake : AwakeSystem<NPCEntity, GameObject, int>
    {
        public override void Awake(NPCEntity self, GameObject game, int id)
        {
            self.Awake(game, id);
        }
    }
   
    /// <summary>
    /// NPC实体
    /// </summary>
    public class NPCEntity : UnitEntity
    {
        public int configId;//配置表id
        public Npc_InfoConfig _InfoConfig;
    
        public RoleEntity roleEntity;

        /// <summary>
        /// 商店NPC的物品 字典
        /// </summary>
        public readonly Dictionary<long, KnapsackDataItem> NPCShopDic=new Dictionary<long, KnapsackDataItem>();
        public void Awake(GameObject game, int id)
        {
            roleEntity = UnitEntityComponent.Instance.LocalRole;
            this.Game_Object = game;
            configId = id;
            NPCShopDic.Clear();
            _InfoConfig = ConfigComponent.Instance.GetItem<Npc_InfoConfig>(configId);
          
        }
        /// <summary>
        /// 点击事件
        /// </summary>
        public async void ClickEvent()
        {

            if (PositionHelper.Distance2D(this.CurrentNodePos, UnitEntityComponent.Instance.LocalRole.CurrentNodePos) >= 9)
            {
               // Log.DebugRed("距离太远了");
                return;
            }
            switch (_InfoConfig.Id)
            {
                case 10026://荧之石管理员
                    UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Inlay);
                    break;
                case 10028://战盟使者罗兰斯
                    if (roleEntity.Level >= 6)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIWarAlliance);
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                    }
                    break;
                case 10009://玛雅哥布林
                    //UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge);
                    UIComponent.Instance.VisibleUI(UIType.UISynthesis, SynthesisType.General);
                    break; 
                case 10048://杰瑞敦
                    //UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge);
                    UIComponent.Instance.VisibleUI(UIType.UISynthesis, SynthesisType.ZhuoYue);
                    break;
                case 10020://冰风谷 副本大天使
                    UIComponent.Instance.VisibleUI(UIType.UIFuBen);
                    break; 
                case 10018://冰风谷 一转NPC
                    if (roleEntity.RoleType == E_RoleType.Magicswordsman || roleEntity.RoleType == E_RoleType.Holymentor || roleEntity.RoleType == E_RoleType.Magicswordsman || roleEntity.RoleType == E_RoleType.Gladiator || roleEntity.RoleType == E_RoleType.GrowLancer)
                    {
                        break;
                    }
                    if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.Level) < 150)//一转 150级以上
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint,"等级不足 无法转职");
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.OccupationLevel) > 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "你已经完成一转");
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UICareerChangePanel);
                    }
                    break;
                case 10041://勇者大陆 二转NPC 
                    if (roleEntity.RoleType == E_RoleType.Magicswordsman || roleEntity.RoleType == E_RoleType.Holymentor || roleEntity.RoleType == E_RoleType.Magicswordsman || roleEntity.RoleType == E_RoleType.Gladiator || roleEntity.RoleType == E_RoleType.GrowLancer)
                    {
                        break;
                    }
                    if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.Level) < 220)//二转 220级以上
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足 无法转职");
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.OccupationLevel) ==0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "请先完成一转");
                    }
                    else if (roleEntity.RoleType != E_RoleType.Swordsman &&
                        UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.OccupationLevel) > 1)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "你已经完成二转");
                    }
                    else if(roleEntity.RoleType == E_RoleType.Swordsman &&
                        UnitEntityComponent.Instance.LocalRole.OwnSkills.Contains(122)) // 骑士第二阶段任务 学会连击技能
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "你已经完成二转");
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UICareerChangePanel);
                    }
                    break;
                case 10037://狼魂要塞 三转NPC
                    if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.Level) < 380)//三转 400级以上
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足 无法转职");
                    }
                    else if (roleEntity.RoleType == E_RoleType.Magicswordsman || roleEntity.RoleType == E_RoleType.Holymentor || roleEntity.RoleType == E_RoleType.Gladiator || roleEntity.RoleType == E_RoleType.GrowLancer)
                    {
                        //直接三转
                        if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.OccupationLevel) > 2)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "你已经完成三转");
                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UICareerChangePanel);
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.OccupationLevel) == 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "请先完成一转");
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.OccupationLevel) == 1)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "请先完成二转");
                    }
                    else if (roleEntity.RoleType == E_RoleType.Swordsman &&
                        UnitEntityComponent.Instance.LocalRole.OwnSkills.Contains(122) == false) // 骑士二转第二阶段任务
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "请先完成二转");
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.OccupationLevel) > 2)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "你已经完成三转");
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UICareerChangePanel);
                    }
                    break;
                case 10053:
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();

                    if (KnapsackItemsManager.GetIncludeValue(320304))//如果背包里有大天使
                    {
                       
                        G2C_CommitArchangelWeaponRequest g2C_CommitArchangel = (G2C_CommitArchangelWeaponRequest)await SessionComponent.Instance.Session.Call(new C2G_CommitArchangelWeaponRequest(){});
                        if (g2C_CommitArchangel.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_CommitArchangel.Error.GetTipInfo());
                        }
                        else
                        {
                            UIMainComponent.Instance.StartFubenCountDown(false,"血色城堡", 20, false);
                            uIConfirmComponent.SetTipText($"伟大的勇士！您使我获得了新的力量，可以继续阻止魔族的入侵企图。为了表示谢意，让勇士们一起分享我的战斗经验吧！");
                        }
                    }
                    else if (UIMainComponent.Instance.weaponIsReturn)
                    {
                        uIConfirmComponent.SetTipText($"非常感谢帮我找回了武器，这里十分危险，勇士们快点离开血色城堡吧！");
                    }
                    else
                    {
                        uIConfirmComponent.SetTipText($"你是修炼中的勇士。我相信你的勇气，打败这些怪物，把我的武器拿回来。");
                    }
                    break;
                case 10040://试炼之地

                    G2C_OpenClimbingTowerNPC g2C_OpenClimbing = (G2C_OpenClimbingTowerNPC)await SessionComponent.Instance.Session.Call(new C2G_OpenClimbingTowerNPC { });
                    if (g2C_OpenClimbing.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenClimbing.Error.GetTipInfo());
                    }
                    else
                    {
                        // 试炼之地面板
                        UIComponent.Instance.VisibleUI(UIType.UIShiLianZhiDi, g2C_OpenClimbing.Amount, g2C_OpenClimbing.Expend);
                        UIComponent.Instance.Get(UIType.UIShiLianZhiDi).GetComponent<UIShiLianZhiDiComponent>().InitRankInfo(g2C_OpenClimbing.Ranking.ToList());
                    }

                    break;
                case 10006://转生npc

                   // UIComponent.Instance.VisibleUI(UIType.UIZhuanSheng);
                    break;
                default:
                    break;
            }
            switch (_InfoConfig.NpcType)
            {
                case 2://仓库
                    UIComponent.Instance.VisibleUI(UIType.UIKnapsack, $"{E_KnapsackState.KS_Ware_House}");
                    break;
                case 1://商店
                case 3://商店
                    if (_InfoConfig.Id == 10024)
                    {
                        //少女安娜 
                        UIComponent.Instance.VisibleUI(UIType.UIBuyMedicine);
                        break;
                    }

                    GetShopItems().Coroutine();
                    break;
                case 6://任务
                    UIComponent.Instance.VisibleUI(UIType.UITask,E_TaskType.HuntingTask);
                    UIComponent.Instance.Get(UIType.UITask).GetComponent<UITaskComponent>().IsNpc = true;
                    break;
                case 7://藏宝图雕像
                    UIComponent.Instance.VisibleUI(UIType.UITreasureMap, new string[] { $"{this.Id}", $"{this.configId}" });
                    break;

            }
            ///获取当前商店NPC商店的全部物品
            async ETVoid GetShopItems() 
            {
                if (NPCShopDic.Count != 0) 
                {
                    UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Shop);
                    UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>().Init_ShopEquip(NPCShopDic, this.Id,E_BuyType.Normal);
                    //类型为3 可以修理装备
                    UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>().RepairBtn.gameObject.SetActive(_InfoConfig.NpcType==3);
                    return;
                };

                G2C_GetShopNPCItems g2C_GetShop = (G2C_GetShopNPCItems) await SessionComponent.Instance.Session.Call(new C2G_GetShopNPCItems { NPCUUID=this.Id});
                if (g2C_GetShop.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetShop.Error.GetTipInfo());
                }
                else
                {
                    NPCShopDic.Clear();
                    for (int i = 0, length= g2C_GetShop.AllItems.Count; i < length; i++)
                    {
                        var item = g2C_GetShop.AllItems[i];
                       
                        KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(item.ItemUID);
                        knapsackDataItem.GameUserId = item.GameUserId;//玩家的UID
                        knapsackDataItem.UUID = item.ItemUID;//装备的UID
                        knapsackDataItem.ConfigId = item.ConfigID;//装备配置表id
                        knapsackDataItem.ItemType = item.Type;//装备类型
                       
                        knapsackDataItem.PosInBackpackX = item.PosInBackpackX;//装备在背包中的起始格子 坐标
                        knapsackDataItem.PosInBackpackY = item.PosInBackpackY;
                        knapsackDataItem.X = item.Width;//装备所占的格子
                        knapsackDataItem.Y = item.Height;
                        knapsackDataItem.SetProperValue(E_ItemValue.Quantity, item.Quantity);//装备的数量
                        knapsackDataItem.SetProperValue(E_ItemValue.Level, item.ItemLevel);//装备的等级
                        NPCShopDic[item.ItemUID] = knapsackDataItem;
                    }
                    //设置 NPC商城物品的属性
                    for (int i = 0, length= g2C_GetShop.AllProperty.Count; i < length; i++)
                    {
                        
                        var item = g2C_GetShop.AllProperty[i];
                       // Log.DebugBrown($"item.ItemUUID:{item.ItemUUID}");
                        for (int p = 0,pcount= item.PropList.Count; p < pcount; p++)
                        {
                            NPCShopDic[item.ItemUUID].Set(item.PropList[p]);
                        }
                       
                        for (int e = 0, count = item.ExecllentEntry.Count; e < count; e++)
                        {
                            NPCShopDic[item.ItemUUID].SetExecllentEntry(item.ExecllentEntry[e]);
                        }
                        for (int e = 0, count = item.SpecialEntry.Count; e < count; e++)
                        {
                            NPCShopDic[item.ItemUUID].SetSpecialEntry(item.SpecialEntry[e]);
                        }
                    }
                   
                    UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Shop);
                    UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>().Init_ShopEquip(NPCShopDic, this.Id,E_BuyType.Normal);
                    //类型为3 可以修理装备
                    UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>().RepairBtn.gameObject.SetActive(_InfoConfig.NpcType == 3);
                }
            }
        }

        public void ClearNpcShop() 
        {
            foreach (var item in NPCShopDic)
            {
                item.Value.Dispose();
            }
           
            NPCShopDic.Clear();
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose(); 
            _InfoConfig = null;
          
        }
    }
}
