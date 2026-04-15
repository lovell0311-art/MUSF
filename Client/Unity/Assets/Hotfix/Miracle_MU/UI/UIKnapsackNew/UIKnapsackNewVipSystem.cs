using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{

    public static class UIKnapsackNewVipSystem
    {

        public static async ETTask InitVip(this UIKnapsackNewComponent self)
        {
            string res = "NPCShop";
            //加载对应面板
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //实例化面板 
            GameObject vip = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            vip.transform.SetParent(self.plane.transform, false);
            vip.transform.localPosition = Vector3.zero;
            vip.transform.localScale = Vector3.one;
            vip.gameObject.SetActive(true);

            self.vipCollector = vip.GetReferenceCollector();
            self.NpcShopGrids = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_NpcShop_X][];
            for (int i = 0; i < self.NpcShopGrids.Length; i++)
            {
                self.NpcShopGrids[i] = new KnapsackNewGrid[UIKnapsackNewComponent.LENGTH_NpcShop_Y];
            }
            self.NpcShopContent = self.vipCollector.GetGameObject("Grids");
            //初始化格子
            self.CreatGrid(UIKnapsackNewComponent.LENGTH_NpcShop_X, UIKnapsackNewComponent.LENGTH_NpcShop_Y, self.NpcShopContent.transform, E_Grid_Type.Shop, ref self.NpcShopGrids);
            self.RepairBtn = self.vipCollector.GetButton("RepairBtn");//维修按钮
            self.RepairBtn.gameObject.SetActive(false);
            self.RepairBtn.onClick.AddSingleListener(() =>
            {
                var npc = UnitEntityComponent.Instance.Get<NPCEntity>(self.CurNpcUUid);
                self.RepairEquips(self.CurNpcUUid, npc.CurrentNodePos.x, npc.CurrentNodePos.z);
            });

            //金币
            self.icontxt = self.vipCollector.GetText("icon");
            self.icontxt.text = self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin).ToString();

            Game.EventCenter.EventListenner<long>(EventTypeId.GLOD_CHANGE, self.ChangeGlogIcon);

            Transform btns = self.vipCollector.GetGameObject("Btns").transform;
            for (int i = 0, length = btns.childCount; i < length; i++)
            {
                Toggle button = btns.GetChild(i).GetComponent<Toggle>();
                var str = button.name.Split('_');
                int count = int.Parse(System.Text.RegularExpressions.Regex.Replace(str[1], @"[^0-9]+", ""));
                E_Medicine medicine = str[0] == "HP" ? E_Medicine.HP : E_Medicine.MP;
                button.onValueChanged.AddSingleListener((isOn) =>
                {
                    if (isOn)
                    {
                        self.curMedicine = medicine;
                        self.curMedicineCount = count;
                    }
                });
            }


            self.vipCollector.GetToggle("SmallToggle").onValueChanged.AddSingleListener((value) => { self.ChangeMedicineType(value, E_MedicineType.Small); });
            self.vipCollector.GetToggle("MiddleToggle").onValueChanged.AddSingleListener((value) => { self.ChangeMedicineType(value, E_MedicineType.Medium); });
            self.vipCollector.GetToggle("BigToggle").onValueChanged.AddSingleListener((value) => { self.ChangeMedicineType(value, E_MedicineType.Large); });
            self.vipCollector.GetButton("BtnSell").onClick.AddSingleListener(() => { self.OnSellClick(); });

            self.vipCollector.GetButton("BtnHouse").onClick.AddSingleListener(async () =>
            {
                
                self.InitWareHouse().Coroutine();

                self.ClearVip();
            });
        }

        /// <summary>
        /// 初始化商店的物品
        /// </summary>
        /// <param name="itemDic">商店物品字典</param>
        /// <param name="npcUid">商店NPC的UUID</param>
        public static void InitShopEquip(this UIKnapsackNewComponent self, Dictionary<long, KnapsackDataItem> itemDic, long npcUid, E_BuyType buyType)
        {
            self.CurNpcUUid = npcUid;
            self.buyType = buyType;

            foreach (var item in itemDic.Values)
            {
                self.AddItem(item, type: E_Grid_Type.Shop);
            }
        }

        public static void ChangeGlogIcon(this UIKnapsackNewComponent self, long icon)
        {
            self.icontxt.text = icon.ToString();
        }

        public static void ClearVip(this UIKnapsackNewComponent self)
        {
            Game.EventCenter.RemoveEvent<long>(EventTypeId.GLOD_CHANGE, self.ChangeGlogIcon);
            self.ReleaseGridCollectionVisuals(self.NpcShopGrids);
            if (self.vipCollector)
            {
                GameObject.Destroy(self.vipCollector.gameObject);
                self.vipCollector = null;
            }

            self.isOpenVipPlane = false;

            self.ClearSell();
        }

        /// <summary>
        /// 维修装备
        /// </summary>
        public static void RepairEquips(this UIKnapsackNewComponent self, params object[] args)
        {
            //那些装备要维修
            var RepairEquipList = new List<int>();//需要维修的装备
            var RepairNeedMoney = 0;//维修需要的价格
           // Log.DebugBrown("玩家当前穿戴的装备数量" + self.EquipmentComponent.curWareEquipsData_Dic.Count+"::::"+ self.roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.Count);
            //foreach (var item in self.roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic)
            //{
            //    Log.DebugBrown("打印穿戴装备的key" + item.Key);
            //}
            for (E_Grid_Type i = E_Grid_Type.Weapon, length = E_Grid_Type.Pet; i < length; i++)
            {
                self.EquipmentComponent ??= self.roleEntity.GetComponent<RoleEquipmentComponent>();
                if (self.roleEntity.GetComponent<RoleEquipmentComponent>().curWareEquipsData_Dic.TryGetValue(i, out KnapsackDataItem knapsackData))
                {
                    if (i == E_Grid_Type.Guard)//守护不能维修
                        continue;
                    
                   // Log.DebugBrown("打印装备" + knapsackData.name + "id" + knapsackData.ConfigId + "金额" + knapsackData.GetProperValue(E_ItemValue.RepairMoney));
                    if (knapsackData.GetProperValue(E_ItemValue.RepairMoney) is int money)
                    {
                        if (money == 0)
                        {
                            //维修价格为 0 不能维修
                            continue;
                        }
                        else
                        {
                            RepairEquipList.Add((int)i);
                            RepairNeedMoney += money;
                        }
                    }
                }
            }
            if (RepairEquipList.Count == 0)
            {
                Log.DebugBrown("2222222222");
                //没有装备需要维修
                UIComponent.Instance.VisibleUI(UIType.UIHint, "没有装备 需要维修");
                return;
            }
            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
            uIConfirm.SetTipText($"确定花费<color=red>{RepairNeedMoney}</color>金币 维修装备？");
            uIConfirm.AddActionEvent(async () =>
            {
                //金币是否 足够
                if (RepairNeedMoney > self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足");
                    return;
                }
                //请求维修
                G2C_RepairEquipItemResponse g2C_RepairEquip = (G2C_RepairEquipItemResponse)await SessionComponent.Instance.Session.Call(new C2G_RepairEquipItemRequest
                {
                    EquipPosition = new Google.Protobuf.Collections.RepeatedField<int> { RepairEquipList },//指定装备栏id (维修价格大于0 就可以维修)
                    NpcUID = args.Length == 0 ? 0 : (long)args[0],
                    NpcPosX = args.Length == 0 ? 0 : (int)args[1],
                    NpcPosY = args.Length == 0 ? 0 : (int)args[2],
                });
                if (g2C_RepairEquip.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RepairEquip.Error.GetTipInfo());
                }
                else
                {
                    RepairEquipList = null;
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "维修成功");
                }

            });
            uIConfirm.AddCancelEventAction(() =>
            {
                RepairEquipList = null;
            });
        }

        public static void OnVipDragToPackage(this UIKnapsackNewComponent self)
        {
            self.ChangeItemPos(async () =>
            {
                //判断金币是否足够 购买改物品
                if (self.originArea.ItemData.GetProperValue(E_ItemValue.BuyMoney) > self.roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "金币不足 无法购买");
                    self.ResetGridObj();
                    return;
                }

                /////玩家购买NPC商店的物品到背包 购买成功会推送物品进入背包
                G2C_BuyItemFromNPCShop g2C_BuyItemFromNPC = (G2C_BuyItemFromNPCShop)await SessionComponent.Instance.Session.Call(new C2G_BuyItemFromNPCShop
                {
                    NPCUUID = self.CurNpcUUid,
                    ItemUUID = self.originArea.UUID,
                    PosInBackpackX = self.curChooseArea.Point1.x,
                    PosInBackpackY = self.curChooseArea.Point1.y,
                    RemoteBuy = (int)self.buyType  //0  默认购买=1//远程

                });
                if (g2C_BuyItemFromNPC.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BuyItemFromNPC.Error.GetTipInfo());
                    self.ResetGridObj();
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "购买成功");
                    //物品回到 上传面板
                    self.ResetGridObj();
                }
            });
        }

        public static void ChangeMedicineType(this UIKnapsackNewComponent self,bool value, E_MedicineType medicineType)
        {
            if (value == false) return;
            self.curMedicineType = medicineType;
        }

        public static void OnSellClick(this UIKnapsackNewComponent self)
        {
            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
            uIConfirm.SetTipText($"确定购买<color=red>{self.GetTip(self.curMedicine, self.curMedicineCount)}</color>?");
            uIConfirm.AddActionEvent(async () =>
            {
                G2C_QuickPurchaseResponse g2C_Quick = (G2C_QuickPurchaseResponse)await SessionComponent.Instance.Session.Call(new C2G_QuickPurchaseRequest
                {
                    Cnt = self.curMedicineCount,
                    ItemConfigId = self.GetConfigId(self.curMedicineType, self.curMedicine)
                });
                if (g2C_Quick.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Quick.Error.GetTipInfo());
                }
            });
        }

        public static string GetTip(this UIKnapsackNewComponent self,E_Medicine medicine, int count)
        {

            return GetStr(self.curMedicineType, medicine);

            string GetStr(E_MedicineType medicineType, E_Medicine medicine) => (medicineType, medicine) switch
            {
                (E_MedicineType.Small, E_Medicine.HP) => $"小瓶治疗药水{count}组",
                (E_MedicineType.Medium, E_Medicine.HP) => $"中瓶治疗药水{count}组",
                (E_MedicineType.Large, E_Medicine.HP) => $"大瓶治疗药水{count}组",
                (E_MedicineType.Small, E_Medicine.MP) => $"小瓶魔力药水{count}组",
                (E_MedicineType.Medium, E_Medicine.MP) => $"中瓶魔力药水{count}组",
                (E_MedicineType.Large, E_Medicine.MP) => $"大瓶魔力药水{count}组",
            };
        }


        /// <summary>
        /// 获取 对应药瓶的ConfigId
        /// </summary>
        /// <param name="medicineType"></param>
        /// <param name="medicine"></param>
        /// <returns></returns>
        public static int GetConfigId(this UIKnapsackNewComponent self,E_MedicineType medicineType, E_Medicine medicine) => (medicineType, medicine) switch
        {
            (E_MedicineType.Small, E_Medicine.HP) => 310002,
            (E_MedicineType.Medium, E_Medicine.HP) => 310003,
            (E_MedicineType.Large, E_Medicine.HP) => 310004,
            (E_MedicineType.Small, E_Medicine.MP) => 310005,
            (E_MedicineType.Medium, E_Medicine.MP) => 310006,
            (E_MedicineType.Large, E_Medicine.MP) => 310007,
        };
    }
}
