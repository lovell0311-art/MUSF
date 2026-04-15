using UnityEngine;
using ETModel;
using ILRuntime.Runtime;
using DG.Tweening;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class KnapsackItemEntityAwake : AwakeSystem<KnapsackItemEntity, ItemDropDataInfo>
    {
        public override void Awake(KnapsackItemEntity self, ItemDropDataInfo itemDropData)
        {
            self.dropData = itemDropData;
            self.dropData.Key.ToInt64().GetItemInfo_Out(out self.item_Info);//获取物品配置表
            self.Namebuilder = new StringBuilder();
            self.Game_Object = ResourcesComponent.Instance.LoadGameObject(self.item_Info.ResName.StringToAB(), self.item_Info.ResName);//加载 物品模型资源

            if (self.Game_Object == null)
            {
                Log.DebugRed($"{self.item_Info.ResName} 模型资源 不存在");
                //self.Game_Object = ResourcesComponent.Instance.LoadGameObject("Weapon_leishenzhijian".StringToAB(), "Weapon_leishenzhijian");
                return;
            }

            if (self.item_Info.Id / 10000 == (int)E_ItemType.Helms)
            {
                self.Game_Object.SetBack(itemDropData.Level);//默认 为白装
            }
            else
            {
                self.Game_Object.SetWorld(itemDropData.Level);//默认 为白装
            }
            self.Game_Object.HideCaiDai();//隐藏彩带

            self.Game_Object.SetLayer(LayerNames.LOCALROLE);
            self.astarNode = AstarComponent.Instance.GetNode(itemDropData.PosX, itemDropData.PosY);
            Vector3 pos = AstarComponent.Instance.GetVectory3(itemDropData.PosX, itemDropData.PosY); //格子坐标 转为三维坐标

            self.Game_Object.transform.position = pos.GroundVector3Pos(self.GetOffset().pos_Y);
            self.Game_Object.transform.position = new Vector3(self.Game_Object.transform.position.x, Mathf.Clamp(self.Game_Object.transform.position.y, self.Game_Object.transform.position.y, 1.5f), self.Game_Object.transform.position.z);
            self.Game_Object.transform.position.GroundPos();
            self.Game_Object.transform.localRotation = Quaternion.Euler(self.GetOffset().rotation);
            var y = self.Game_Object.transform.position.y;
            //物品 蹦出来的效果
            if (self.Game_Object != null)
            {
                self.tween = self.Game_Object.transform.DOMoveY(y + 7, .5f).OnComplete(() =>
                {
                    if (self.Game_Object != null)
                        self.Game_Object?.transform.DOMoveY(y, .3f);
                });
            }

            if (HUDComponent.Instance == null)
            {
                UIComponent.Instance.VisibleUI(UIType.UI_HUD);
            }
            self.toolTip = HUDComponent.Instance.FetchToolTip();
            self.text = self.toolTip.GetComponentInChildren<Text>();

            /* self.barComponent= self.AddComponent<UIUnitEntityHpBarComponent>();

             //self.barComponent?.ChangePos(GetOffset().itemNameHeight);//改变
             if (itemDropData.Key == 320106)//门
             {
                 self.barComponent?.ChangePos(self.dropData.PosX, self.dropData.PosY, 10);//改变
             }
             else
             {
                 self.barComponent?.ChangePos(self.dropData.PosX, self.dropData.PosY, (int)self.GetOffset().pos_Y+ (int)self.Game_Object.transform.position.y);//改变
                 self.SetItemName();
             }*/
            //显示名字
            // self.SetItemName();

            //显示名字
            self.SetItemNameHud();
            self.RefreshPosition();

            Game.EventCenter.EventListenner<AstarNode>(EventTypeId.LOCALROLE_GRIDCHANGE, self.PickUpItem);


        }
    }

    [ObjectSystem]
    public class KnapsackItemEntityUpdate : UpdateSystem<KnapsackItemEntity>
    {
        public override void Update(KnapsackItemEntity self)
        {

            self.RefreshPosition();
        }
    }
    /// <summary>
    /// 背包物品掉落实体
    /// </summary>
    public class KnapsackItemEntity : UnitEntity
    {
        public ItemDropDataInfo dropData;
        /// <summary>
        /// 是否是套装
        /// </summary>
        public bool IsSuit => (dropData.Quality & 1 << 4) == 1 << 4;
        /// <summary>
        /// 是否是卓越装备
        /// </summary>
        public bool IsHaveExecllentEntry => (dropData.Quality & 1 << 3) == 1 << 3;
        /// <summary>
        /// 是否有幸运装备
        /// </summary>
        public bool IsHaveLuckyEntry => (dropData.Quality & 1 << 2) == 1 << 2;
        /// <summary>
        /// 是否有镶嵌属性
        /// </summary>
        public bool IsHaveInlayEntry => (dropData.Quality & 1 << 5) == 1 << 5;

        public Tween tween;

        public Item_infoConfig item_Info;
        public AstarNode astarNode;

        public UIUnitEntityHpBarComponent barComponent;//显示名字组件

        public StringBuilder Namebuilder;

        public GameObject toolTip;
        public Text text;

        //获取装备 距离地面的高度
        public (float pos_Y, Vector3 rotation) GetOffset()
        {
            switch ((E_EquipmentPart)item_Info.Slot)
            {
                case E_EquipmentPart.Helmet://头盔
                    return (pos_Y: -3.5f, rotation: Vector3.zero);
                case E_EquipmentPart.Armor://铠甲
                    return (pos_Y: -2f, Vector3.zero);
                case E_EquipmentPart.HandGuard://护手
                    return (pos_Y: -1.5f, Vector3.zero);
                case E_EquipmentPart.Leggings://护腿
                    return (pos_Y: -1.5f, Vector3.zero);
                case E_EquipmentPart.Boots://靴子
                    return (pos_Y: 0, Vector3.zero);
                case E_EquipmentPart.Weapon://武器
                    if (Game_Object.name.Contains("gong"))
                        return (pos_Y: 1.5f, new Vector3(0, 0, 90));
                    else if (Game_Object.name.Contains("nu"))
                        return (pos_Y: 1.5f, new Vector3(-60, -90, 180));
                    else
                        return (pos_Y: 1.5f, new Vector3(-15, 0, 90));
                case E_EquipmentPart.Shield://盾牌、箭筒
                    if (Game_Object.name.Contains("shu"))
                        return (pos_Y: .5f, new Vector3(90, 0, 0));
                    else if (Game_Object.name.Contains("jiantong"))
                        return (pos_Y: .5f, new Vector3(0, 0, 90));
                    else
                        return (pos_Y: .5f, new Vector3(35, 15, -70));
                case E_EquipmentPart.Wing://翅膀
                    return (pos_Y: 0.5f, new Vector3(90, 0, 0));

                default:
                    break;
            }

            return (1.5f, Vector3.zero);
        }
        /// <summary>
        /// 拾取 掉落的装备物品
        /// </summary>
        /// <param name="node">本地玩家 当前的格子坐标</param>
        public void PickUpItem(AstarNode node = null)
        {

            if (RoleOnHookComponent.Instance.IsOnHooking) return;// 不挂机时 才能使用 移动站立到物品的位置 拾取物品

            if (KnapsackItemsManager.IsPackBackpack) return;//正在整理背包

            if (KnapsackItemsManager.LastItemIsComplete == false)
            {
                //  Log.DebugRed("上一个物品还未拾取完成");
                return;//上一个物品还未拾取完成
            }

            int configId = dropData.Key;
            if (item_Info == null) return;
            //金币直接拾取
            if (configId == 320294 || configId == 320316)
            {
                PickUpDropItemAsync(Vector2Int.zero).Coroutine();
                return;
            }
            // Log.DebugGreen($"是否可以拾取 {item_Info.Name} 物品：{astarNode.Compare(node)}");
            if (node != null && (astarNode.Compare(node) || PositionHelper.Distance(node, astarNode) < 2))
            {
                //位置相同 可以拾取
                if (IscanPickUp().Item1)
                {
                    PickUpDropItemAsync(IscanPickUp().Item2).Coroutine();
                }
            }

            ///检测背包 空间是否足够
            (bool, Vector2Int) IscanPickUp()
            {
                // Vector2Int? index = KnapsackTools.GetKnapsackItemUnoccupied(KnapsackTools.CreateGridData(0, 0, item_Info.X - 1, item_Info.Y - 1));
                Vector2Int index = KnapsackTools.GetKnapsackItemPos(this.item_Info.X, this.item_Info.Y);
                if (index.x == -1)
                {

                    // UIComponent.Instance.VisibleUI(UIType.UIHint, "背包空间不足");
                    return (false, Vector2Int.zero);
                }
                else
                {
                    // Log.DebugGreen($"请求拾取{item_Info.Name}：{index}");
                    return (true, index);
                }

            }

            ///请求 拾取物品
            async ETVoid PickUpDropItemAsync(Vector2Int vector2Int)
            {
                KnapsackItemsManager.LastItemIsComplete = false;
                int level = dropData.Level - 1;

                try
                {
                    G2C_BattlePickUpDropItemResponse g2C_BattlePickUp = (G2C_BattlePickUpDropItemResponse)await SessionComponent.Instance.Session.Call(new C2G_BattlePickUpDropItemRequest
                    {
                        PosX = dropData.PosX,
                        PosY = dropData.PosY,
                        InstanceId = this.Id,
                        PosInBackpackX = vector2Int.x,
                        PosInBackpackY = vector2Int.y
                    });
                    if (g2C_BattlePickUp.Error != 0)
                    {
                       // UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BattlePickUp.Error.GetTipInfo());
                    }
                    else
                    {
                        if (configId != 320106)
                        {
                            SoundComponent.Instance.PlaySkill("jiandongxi");

                            //自动出售
                            if (RecycleEquipTools.AutoRecycle && UIMainComponent.Instance.HookTog.isOn)
                            {
                                if (KnapsackItemsManager.KnapsackItems.TryGetValue(g2C_BattlePickUp.ItemUid, out KnapsackDataItem knapsack))
                                {
                                    RecycleEquipTools.AutoSell(knapsack);
                                }
                                else
                                {
                                    //Log.Info("------------------------物品暂未进入背包 " + item_Info?.Name + " uuid = " + g2C_BattlePickUp.ItemUid);
                                    if (g2C_BattlePickUp.ItemUid != 0)
                                    {
                                        KnapsackItemsManager.WaitingForAutomaticRecyclings.Add(g2C_BattlePickUp.ItemUid);
                                    }
                                }
                            }
                            else
                            {
                                Log.DebugGreen($"{item_Info?.Name}-拾取成功");
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{item_Info?.Name}-拾取成功");
                            }
                        }
                        else
                        {

                            ChangeScene(10300 + level).Coroutine();
                            async ETVoid ChangeScene(int mapId)
                            {
                                G2C_MapDeliveryResponse response = (G2C_MapDeliveryResponse)await SessionComponent.Instance.Session.Call(new C2G_MapDeliveryRequest
                                {
                                    MapId = mapId
                                });
                                if (response.Error != 0)
                                {

                                    UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                                }
                            }
                        }


                    }
                }
                finally
                {
                    KnapsackItemsManager.LastItemIsComplete = true;
                }




            }
        }

        public void AutoPickUp()
        {
            //Log.DebugGreen($"请求拾取 物品：{item_Info.Name}  item_Info == null:{item_Info == null}");

            //金币直接拾取
            if (dropData.Key == 320294 || dropData.Key == 320316)
            {
                PickUpDropItemAsync(Vector2Int.zero).Coroutine();
                return;
            }
            if (KnapsackItemsManager.LastItemIsComplete == false) return;//上一个物品还未拾取完成
            KnapsackItemsManager.LastItemIsComplete = false;

            if (item_Info == null)
            {
                KnapsackItemsManager.LastItemIsComplete = true;
                return;
            }
            //Vector2Int? index = KnapsackTools.GetKnapsackItemUnoccupied(KnapsackTools.CreateGridData(0, 0, item_Info.X - 1, item_Info.Y - 1));
            Vector2Int index = KnapsackTools.GetKnapsackItemPos(this.item_Info.X, this.item_Info.Y);
            if (index.x == -1)
            {
                //Log.DebugRed("拾取装备背包放不下:" + item_Info.Name);
                // UIComponent.Instance.VisibleUI(UIType.UIHint, "背包空间不足");
                KnapsackItemsManager.LastItemIsComplete = true;
                return;
            }
            // Log.DebugGreen($"请求拾取{item_Info.Name}：{index}");
            if (dropData.Key != 320106)
            {
                //Log.Info("请求拾取  " + item_Info.Name);
                PickUpDropItemAsync(index).Coroutine();
            }
            else
            {
                //Log.Info("请求拾取 dropData.Key " + dropData.Key);
                KnapsackItemsManager.LastItemIsComplete = true;
            }
            ///请求 拾取物品
            async ETVoid PickUpDropItemAsync(Vector2Int vector2Int)
            {
                try
                {
                    G2C_BattlePickUpDropItemResponse g2C_BattlePickUp = (G2C_BattlePickUpDropItemResponse)await SessionComponent.Instance.Session.Call(new C2G_BattlePickUpDropItemRequest
                    {
                        PosX = dropData.PosX,
                        PosY = dropData.PosY,
                        InstanceId = this.Id,
                        PosInBackpackX = vector2Int.x,
                        PosInBackpackY = vector2Int.y
                    });
                    if (g2C_BattlePickUp.Error != 0)
                    {
                        //if (g2C_BattlePickUp.Error != 1014)
                        //{
                        //    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BattlePickUp.Error.GetTipInfo());
                        //}
                        //Log.Info("拾取失败----------- error="+ g2C_BattlePickUp.Error +"  msg=" + g2C_BattlePickUp.Message + "   name="+ item_Info?.Name);
                    }
                    else
                    {
                        //Log.Info("拾取成功----------- " + item_Info?.Name);
                        if (dropData != null)
                        {
                            SoundComponent.Instance.PlaySkill("jiandongxi");

                        }
                        //自动出售
                        if (RecycleEquipTools.AutoRecycle && UIMainComponent.Instance.HookTog.isOn)
                        {
                            if (KnapsackItemsManager.KnapsackItems.TryGetValue(g2C_BattlePickUp.ItemUid, out KnapsackDataItem knapsack))
                            {
                                RecycleEquipTools.AutoSell(knapsack);
                            }
                            else
                            {
                                //Log.Info("------------------------物品暂未进入背包 " + item_Info?.Name + " uuid = "+ g2C_BattlePickUp.ItemUid);
                                if (g2C_BattlePickUp.ItemUid != 0)
                                {
                                    KnapsackItemsManager.WaitingForAutomaticRecyclings.Add(g2C_BattlePickUp.ItemUid);
                                }
                            }

                        }
                        else
                        {
                            Log.DebugGreen($"{item_Info?.Name}-拾取成功");
                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"{item_Info.Name}-拾取成功");
                        }
                    }
                }
                finally
                {
                    KnapsackItemsManager.LastItemIsComplete = true;
                }
            }

        }
        /// <summary>
        /// 设置装备的名字
        /// </summary>

        public void SetItemName()
        {
            barComponent ??= AddComponent<UIUnitEntityHpBarComponent>();
            PlaySound();
            if (dropData.Key == 320294)//金币
            {
                barComponent.SetEntityName($"{item_Info.Name} {dropData.Value}", color: "#ceb40c");//金色
                //播放金币掉落 音效
                SoundComponent.Instance.PlaySkill("jianjinbi");
            }
            else if (dropData.Key == 320106)//卡利马 门
            {
                if (dropData.KillerId.Count == 0) return;
                if (UnitEntityComponent.Instance.Get<RoleEntity>(dropData.KillerId.First()) is RoleEntity _roleentity)
                {
                    barComponent.SetEntityName($"<color=#00FF00>[{_roleentity.RoleName}]</color> 的{item_Info.Name} {dropData.Level}");
                }
                else
                {
                    barComponent.SetEntityName($"<color=#FF0000>[未知的]</color> {item_Info.Name} {dropData.Level}");
                }
            }
            else
            {
                Namebuilder.Clear();
                if ((dropData.Quality & 1 << 4) == 1 << 4)
                {
                    //有套装
                    var setinfo = ConfigComponent.Instance.GetItem<SetItem_TypeConfig>(dropData.SetId);
                    Namebuilder.Append($"{setinfo.SetName}\t{item_Info.Name}");
                }
                else
                {
                    Namebuilder.Append($"{item_Info.Name}");
                }


                if ((dropData.Quality & 1 << 0) == 1 << 0)
                {
                    //有技能
                    Namebuilder.Append($"\t+技能");
                }
                if ((dropData.Quality & 1 << 2) == 1 << 2)
                {
                    //有幸运
                    Namebuilder.Append($"\t+幸运");
                }
                if ((dropData.Quality & 1 << 3) == 1 << 3)
                {
                    //有卓越
                    Namebuilder.Append($"\t+卓越");
                }

                if ((dropData.Quality & 1 << 5) == 1 << 5)
                {
                    //有镶嵌
                    Namebuilder.Append($"\t+镶嵌");
                }

                barComponent.SetEntityName($"{Namebuilder}", color: GetItemColor());
            }


            string GetItemColor()
            {
                if ((dropData.Quality & 1 << 5) == 1 << 5)
                {
                    //镶嵌（紫色）
                    return "#22075e";
                }
                else if ((dropData.Quality & 1 << 4) == 1 << 4)
                {
                    //套装（翠绿色）
                    return "#04e504";
                }
                else if ((dropData.Quality & 1 << 3) == 1 << 3)
                {
                    //卓越（浅绿色）
                    return "#08ae04";
                }
                else if ((dropData.Quality & 1 << 2) == 1 << 2)
                {
                    //幸运（蓝色）
                    return "#5b8cba";
                }
                else
                {
                    //白色
                    return "#ffffff";
                }
            }
            //物品掉落音效
            void PlaySound()
            {
                if (item_Info.Id / 10000 == 28)//宝石
                {
                    SoundComponent.Instance.PlaySkill("baoshidiaoluo");
                }
                else//物品掉落
                {
                    SoundComponent.Instance.PlaySkill("EquipDrop");
                }
            }
        }
        public void SetItemNameHud()
        {
            PlaySound();
            if (dropData.Key == 320294)//金币
            {
                text.text = $"<color=#ceb40c>{item_Info.Name} {dropData.Value}</color>";//金色
                //播放金币掉落 音效
                SoundComponent.Instance.PlaySkill("jianjinbi");
            }
            else if (dropData.Key == 320106)//卡利马 门
            {
                if (dropData.KillerId.Count == 0) return;
                if (UnitEntityComponent.Instance.Get<RoleEntity>(dropData.KillerId.First()) is RoleEntity _roleentity)
                {

                    text.text = $"<color=#00FF00>[{_roleentity.RoleName}]</color>的{item_Info.Name} {dropData.Level}";
                }
                else
                {
                    text.text = $"<color=#FF0000>[未知的]</color>{item_Info.Name} {dropData.Level}";
                }
            }
            else
            {
                Namebuilder.Clear();
                if ((dropData.Quality & 1 << 4) == 1 << 4)
                {
                    //有套装
                    var setinfo = ConfigComponent.Instance.GetItem<SetItem_TypeConfig>(dropData.SetId);
                    Namebuilder.Append($"{setinfo.SetName}\t{item_Info.Name}");
                }
                else
                {
                    Namebuilder.Append($"{item_Info.Name}");
                }


                if ((dropData.Quality & 1 << 0) == 1 << 0)
                {
                    //有技能
                    Namebuilder.Append($"\t+技能");
                }
                if ((dropData.Quality & 1 << 2) == 1 << 2)
                {
                    //有幸运
                    Namebuilder.Append($"\t+幸运");
                }
                if ((dropData.Quality & 1 << 3) == 1 << 3)
                {
                    //有卓越
                    Namebuilder.Append($"\t+卓越");
                }

                if ((dropData.Quality & 1 << 5) == 1 << 5)
                {
                    //有镶嵌
                    Namebuilder.Append($"\t+镶嵌");
                }

                text.text = $"<color={GetItemColor()}>{Namebuilder}</color>";
            }


            string GetItemColor()
            {
                if ((dropData.Quality & 1 << 5) == 1 << 5)
                {
                    //镶嵌（紫色）
                    return "#22075e";
                }
                else if ((dropData.Quality & 1 << 4) == 1 << 4)
                {
                    //套装（翠绿色）
                    return "#04e504";
                }
                else if ((dropData.Quality & 1 << 3) == 1 << 3)
                {
                    //卓越（浅绿色）
                    return "#08ae04";
                }
                else if ((dropData.Quality & 1 << 2) == 1 << 2)
                {
                    //幸运（蓝色）
                    return "#5b8cba";
                }
                else
                {
                    //白色
                    return "#ffffff";
                }
            }
            //物品掉落音效
            void PlaySound()
            {
                if (item_Info.Id / 10000 == 28)//宝石
                {
                    SoundComponent.Instance.PlaySkill("baoshidiaoluo");
                }
                else//物品掉落
                {
                    SoundComponent.Instance.PlaySkill("EquipDrop");
                }
            }
        }

        public void RefreshPosition()
        {
            if (toolTip == null) return;
            Vector3 scrPos = CameraComponent.Instance.MainCamera.WorldToScreenPoint(this.Game_Object.transform.position + Vector3.up * (Mathf.Abs(GetOffset().pos_Y)));
            toolTip.transform.position = CameraComponent.Instance.UICamera.ScreenToWorldPoint(scrPos);
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            Game.EventCenter.RemoveEvent<AstarNode>(EventTypeId.LOCALROLE_GRIDCHANGE, PickUpItem);
            // dropData = null;
            tween.Kill();
            Namebuilder = null;
            barComponent = null;
            if (toolTip != null)
            {
                HUDComponent.Instance.RecycleToolTip(toolTip);
                toolTip = null;
            }
        }
    }
}