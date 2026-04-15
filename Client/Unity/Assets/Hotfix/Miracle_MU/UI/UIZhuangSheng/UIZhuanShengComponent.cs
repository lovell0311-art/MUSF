using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIZhuanShengComponentAwake : AwakeSystem<UIZhuanShengComponent>
    {
        public override void Awake(UIZhuanShengComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.Awake();
        }
    }

    public class UIZhuanShengComponent : Component
    {


        public ReferenceCollector collector;
        public int Cnt = 0;
        // public List<ItemInfo> itemInfos = new List<ItemInfo>();
        public Text needItemInfo, title,needItemInfo1;
        public bool satisfy = true;
        private Dictionary<int, int> NeedDic = new Dictionary<int, int>();
        private Dictionary<int, int> NeedDic_buff = new Dictionary<int, int>();
        internal void Awake()
        {
            title = collector.GetText("title");
            needItemInfo = collector.GetText("needItemInfo");
            needItemInfo1 = collector.GetText("needItemInfo1");
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIZhuanSheng);
            });
            //确认转生
            collector.GetButton("GitTaskBtn").onClick.AddSingleListener(() =>
            {
                if (satisfy == false)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "未满足转职条件！");
                }
                else
                {
                    Reincarnate().Coroutine();
                }
            });
            OpenReincarnate();
        }
        /// <summary>
        /// 属性数据
        /// </summary>

        public void Property()
        {
            Reincarnate_InfoConfig config = ConfigComponent.Instance.GetItem<Reincarnate_InfoConfig>(UnitEntityComponent.Instance.LocalRole.Reincarnation + 1);
            NeedDic_buff = GlobalDataManager.GetDictionary(config.ReincarnateBuf);
            string str = "";
            str += $"{"大师点数:"+config.MasterPoints+'\n'+"转生属性点:"+config.ReincarnatePoints}";
            //foreach (var item in NeedDic_buff)
            //{
            //    if (item.Key == 2401)
            //    {
            //        str += $"{"防御力提高"}：<color=yellow>{""+(float)item.Value/100}%</color>\n";
            //    }
            //    else if (item.Key == 37)
            //    {
            //        str += $"{"伤害吸收（守护）提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //    else if (item.Key == 3)
            //    {
            //        str += $"{"攻击力提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //    else if (item.Key == 9)
            //    {
            //        str += $"{"伤害减少提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //    else if (item.Key == 1)
            //    {
            //        str += $"{"卓越一击提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //    else if (item.Key == 55)
            //    {
            //        str += $"{"幸运一击提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //    else if (item.Key == 503)
            //    {
            //        str += $"{"生命增加提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //    else if (item.Key == 56)
            //    {
            //        str += $"{"双倍攻击提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //    else if (item.Key == 12)
            //    {
            //        str += $"{"获取金币率提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //    else if (item.Key == 42)
            //    {
            //        str += $"{"无视防御提高"}：<color=yellow>{"" + (float)item.Value / 100}%</color>\n";
            //    }
            //}
            needItemInfo1.text = str;


        }


        /// <summary>
        /// 获取当前的转身数据
        /// </summary>
        /// <returns></returns>
        public void OpenReincarnate()
        {
            UnitEntityComponent.Instance.LocalRole.GetComponent<UIUnitEntityHpBarComponent>().SetReincarnation();
            title.text = $"转生次数：{UnitEntityComponent.Instance.LocalRole.Reincarnation}转";
            Reincarnate_InfoConfig config = ConfigComponent.Instance.GetItem<Reincarnate_InfoConfig>(UnitEntityComponent.Instance.LocalRole.Reincarnation + 1);
            Log.DebugBrown("转生次数" + UnitEntityComponent.Instance.LocalRole.Reincarnation);
            NeedDic = GlobalDataManager.GetDictionary(config.ReincarnationMaterial);

            string Desk = null;
            //(int)UnitEntityComponent.Instance.LocalRole.Level >= config.RestrictionLevel
            if (config.DemandGold!=0)
            {
                if ((long)UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.GoldCoin) >=config.DemandGold)
                {
                    Desk += $"{"金币"}：<color=green>{config.DemandGold}/{(long)UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.GoldCoin)}</color>\n";
                }
                else
                {
                    Desk += $"{"金币"}：<color=red>{config.DemandGold}/{(long)UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.GoldCoin)}</color>\n";
                    satisfy = false;
                }
            }
            if (config.DemandCrystal != 0)
            {
                if ((long)UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing) >= config.DemandCrystal)
                {
                    Desk += $"{"魔晶"}：<color=green>{config.DemandCrystal}/{(long)UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing)}</color>\n";
                }
                else
                {
                    Desk += $"{"魔晶"}：<color=red>{config.DemandCrystal}/{(long)UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing)}</color>\n";
                    satisfy = false;
                }
            }
            //foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)
            //{
            //   // Log.DebugBrown("背包数据" + item1.GetProperValue(E_ItemValue.Quantity) + ":count" + item1.ConfigId);
            //}
            if (true)
            {
                foreach (var item in NeedDic)//需要的材料
                {

                    bool jk = false;
                    foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
                    {
                        
                        if (item1.ConfigId == item.Key)
                        {
                          //  Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                            ((long)item.Key).GetItemInfo_Out(out Item_infoConfig item_Info);
                            if (item1.GetProperValue(E_ItemValue.Quantity) >= item.Value)
                            {
                                Desk += $"{item_Info.Name}：<color=green>{item.Value}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                                jk = true;
                                break;
                            }
                        }
                    }

                    if (jk==false)
                    {
                        ((long)item.Key).GetItemInfo_Out(out Item_infoConfig item_Info);
                        Desk += $"{item_Info.Name}：<color=red>{item.Value}/{0}</color>\n";
                    }
                    //if (!KnapsackItemsManager.KnapsackItems.ContainsKey(item.Key))
                    //{
                    //    Log.DebugBrown("111111");
                    //    ((long)item.Key).GetItemInfo_Out(out Item_infoConfig item_Info);
                    //    Desk += $"{item_Info.Name}：<color=red>{item.Value}/{0}</color>\n";
                    //    satisfy = false;
                    //}
                    //else
                    //{
                    //    foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)//背包拥有的材料
                    //    {
                    //        if (item1.ConfigId == item.Key)
                    //        {
                    //            Log.DebugBrown("打印数据" + item1.GetProperValue(E_ItemValue.Quantity));
                    //            if (item1.GetProperValue(E_ItemValue.Quantity) >= item.Value)
                    //            {
                    //                Desk += $"{item1.name}：<color=green>{item.Value}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    //            }
                    //            else
                    //            {
                    //                Desk += $"{item1.name}：<color=red>{item.Value}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                    //                satisfy = false;
                    //            }
                    //        }
                    //    }
                    //}
                   // Desk += $"{"转生成功获得以下奖励："}：<color=green>{item.Value}/{item1.GetProperValue(E_ItemValue.Quantity)}</color>\n";
                }

                needItemInfo.text = Desk;
            }
            else
            {
                Log.DebugBrown("等级不足！");
            }
            Property();
            //G2C_OpenReincarnate g2C_Open = (G2C_OpenReincarnate)await SessionComponent.Instance.Session.Call(new C2G_OpenReincarnate() { });
            //if (g2C_Open.Error != 0)
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Open.Error.GetTipInfo());
            //}
            //else
            //{
            //    UIRoleInfoInfo.ZhuanShengCount = g2C_Open.Cnt - 1;
            //    Cnt = g2C_Open.Cnt;
            //    if (Cnt > 100)
            //    {
            //        title.text = $"已满级";
            //    }
            //    else
            //    {
            //        title.text = $"下次转生：{Cnt}转";
            //    }
            //    string Desk = null;
            //    foreach (var item in g2C_Open.ItemInfoList)
            //    {
            //        ((long)item.ConfigId).GetItemInfo_Out(out Item_infoConfig item_Info);
            //        if (item.Cnt1 >= item.Cnt2)
            //        {
            //            Desk += $"{item_Info.Name}：<color=green>{item.Cnt1}/{item.Cnt2}</color>\n";
            //        }
            //        else
            //        {
            //            Desk += $"{item_Info.Name}：<color=red>{item.Cnt1}/{item.Cnt2}</color>\n";
            //            satisfy = false;
            //        }
            //    }
            //    needItemInfo.text = Desk;
            //}
        }
        public async ETTask Reincarnate()
        {
            G2C_ReincarnateResponse g2C_Open = (G2C_ReincarnateResponse)await SessionComponent.Instance.Session.Call(new C2G_ReincarnateRequest() { });
            if (g2C_Open.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Open.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "转生成功！");
                UnitEntityComponent.Instance.LocalRole.Reincarnation = g2C_Open.ReincarnateCnt;
                OpenReincarnate();
            }
        }


    }

}
