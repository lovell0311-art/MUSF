using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 出售
    /// </summary>
    public partial class UIKnapsackComponent
    {
        ReferenceCollector collectorData_Recycle;

        //可回收的装备集合
        List<KnapsackDataItem> RecycleEquipList = new List<KnapsackDataItem>();
        

        long glodcoinCount;//卖出获得的金币

        Transform content, verticaItem;
        List<string> itemNames = new List<string>();
        Text coin;
        public void Init_Recycle()
        {
            collectorData_Recycle = Sell.GetReferenceCollector();

            content = collectorData_Recycle.GetGameObject("Content").transform;
            verticaItem = content.GetChild(0);
            coin = collectorData_Recycle.GetText("glodcoin");

            FiltrateEquip();
            ShowSellEquipList();
          

            //单词条卓越装备回收

            collectorData_Recycle.GetToggle("Toggle").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.SingleExcellence = value;
                if (value)
                {
                    var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
                    foreach (var item in list)
                    {
                        if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
                        if (item.IsSingleExcellence())
                        {
                            AddRecycleEquip(item);
                        }
                    }
                }
                else
                {
                    for (int i = RecycleEquipList.Count - 1; i >= 0; i--)
                    {
                        KnapsackDataItem item = RecycleEquipList[i];
                        if (item.IsSingleExcellence())
                        {
                            RemoveRecycleEquip(item);
                        }
                    }
                }
            });
            collectorData_Recycle.GetToggle("Toggle").isOn = RecycleEquipTools.SingleExcellence;
            //白装回收
           
            collectorData_Recycle.GetToggle("Toggle (1)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.WhiteSuit = value;
                if (value)
                {
                    var list= KnapsackItemsManager.KnapsackItems.Values.ToList();
                    foreach (var item in list)
                    {
                        if(KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id)==false) continue;
                        //卓越 幸运、套装、洞转、橙装、追加属性、再生属性、技能
                        if (item.IsWhiteSuit())
                        {
                            AddRecycleEquip(item);
                        }
                    }
                }
                else
                {
                    for (int i = RecycleEquipList.Count - 1; i >= 0; i--)
                    {
                        KnapsackDataItem item = RecycleEquipList[i];
                        if (item.IsWhiteSuit())
                        {
                            RemoveRecycleEquip(item);
                        }
                    }
                }
            });
            collectorData_Recycle.GetToggle("Toggle (1)").isOn = RecycleEquipTools.WhiteSuit;
            //双词条卓越装备回收
            collectorData_Recycle.GetToggle("Toggle (2)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.DoubleExcellence = value;
                if (value)
                {
                    var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
                    foreach (var item in list)
                    {
                        if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
                        if (item.IsDoubleExcellence())
                        {
                            AddRecycleEquip(item);
                        }
                    }
                }
                else
                {
                    for (int i = RecycleEquipList.Count - 1; i >= 0; i--)
                    {
                        KnapsackDataItem item = RecycleEquipList[i];
                        if (item.IsDoubleExcellence())
                        {
                            RemoveRecycleEquip(item);
                        }
                    }
                }
            });
            collectorData_Recycle.GetToggle("Toggle (2)").isOn = RecycleEquipTools.DoubleExcellence;
            //技能书

            collectorData_Recycle.GetToggle("Toggle (3)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.SkillBook = value;
                if (value)
                {
                    var list= KnapsackItemsManager.KnapsackItems.Values.ToList();
                    foreach (var item in list)
                    {
                        if(KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id)==false) continue;
                        if (item.IsSkillBook())
                        {
                            AddRecycleEquip(item);
                        }
                    }
                }
                else
                {
                    for (int i = RecycleEquipList.Count - 1; i >= 0; i--)
                    {
                        KnapsackDataItem item = RecycleEquipList[i];
                        if (item.IsSkillBook())
                        {
                            RemoveRecycleEquip(item);
                        }
                    }
                }
            });
            collectorData_Recycle.GetToggle("Toggle (3)").isOn = RecycleEquipTools.SkillBook;

            //强化装备
            
            collectorData_Recycle.GetToggle("Toggle (5)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.IntensifyEquip = value;
                if (value)
                {
                    var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
                    foreach (var item in list)
                    {
                        if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
                        if (item.IsIntensifyEquip())
                        {
                            AddRecycleEquip(item);
                        }
                    }
                }
                else
                {
                    for (int i = RecycleEquipList.Count - 1; i >= 0; i--)
                    {
                        KnapsackDataItem item = RecycleEquipList[i];
                        if (item.IsIntensifyEquip())
                        {
                            RemoveRecycleEquip(item);
                        }
                    }
                }
            });
            collectorData_Recycle.GetToggle("Toggle (5)").isOn = RecycleEquipTools.IntensifyEquip;


            //回收幸运装备
            //collectorData_Recycle.GetToggle("Toggle (6)").onValueChanged.AddSingleListener(value =>
            //{
            //    RecycleEquipTools.IsLucky = value;
            //    if (value)
            //    {
            //        var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
            //        foreach (var item in list)
            //        {
            //            if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
            //            if (item.Islucky())
            //            {
            //                AddRecycleEquip(item);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = RecycleEquipList.Count - 1; i >= 0; i--)
            //        {
            //            KnapsackDataItem item = RecycleEquipList[i];
            //            if (item.Islucky())
            //            {
            //                AddRecycleEquip(item);
            //            }
            //        }
            //    }
            //});

            //collectorData_Recycle.GetToggle("Toggle (6)").isOn = RecycleEquipTools.IsLucky;

            ////回收附带技能的装备
            //collectorData_Recycle.GetToggle("Toggle (7)").onValueChanged.AddSingleListener(value =>
            //{
            //    RecycleEquipTools.IsSkillId = value;
            //    if (value)
            //    {
            //        var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
            //        foreach (var item in list)
            //        {
            //            if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
            //            if (item.IsSkill())
            //            {
            //                AddRecycleEquip(item);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = RecycleEquipList.Count - 1; i >= 0; i--)
            //        {
            //            KnapsackDataItem item = RecycleEquipList[i];
            //            if (item.IsSkill())
            //            {
            //                AddRecycleEquip(item);
            //            }
            //        }
            //    }
            //});

            //collectorData_Recycle.GetToggle("Toggle (7)").isOn = RecycleEquipTools.IsSkillId;
            ////回收小瓶红蓝 药品

            //collectorData_Recycle.GetToggle("Toggle (4)").onValueChanged.AddSingleListener(value =>
            //{
            //    RecycleEquipTools.Red_BlueMedicine = value;
            //    if (value)
            //    {
            //        var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
            //        foreach (var item in list)
            //        {
            //            if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
            //            if (item.IsRed_BlueMedicine())
            //            {
            //                AddRecycleEquip(item);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = RecycleEquipList.Count - 1; i >= 0; i--)
            //        {
            //            KnapsackDataItem item = RecycleEquipList[i];
            //            if (item.IsRed_BlueMedicine())
            //            {
            //                RemoveRecycleEquip(item);
            //            }
            //        }
            //    }
            //});
            //collectorData_Recycle.GetToggle("Toggle (4)").isOn = RecycleEquipTools.Red_BlueMedicine;


            //collectorData_Recycle.GetToggle("autorecycle").onValueChanged.AddSingleListener(value => RecycleEquipTools.AutoRecycle = value);
            //collectorData_Recycle.GetToggle("autorecycle").isOn = RecycleEquipTools.AutoRecycle;


            ////回收
            //collectorData_Recycle.GetButton("sellbtn").onClick.AddSingleListener(async () =>
            //{
            //    RecycleEquipTools.Sava();
            //    if (RecycleEquipList.Count == 0) return;
            //    foreach (var item in RecycleEquipList)
            //    {
            //        // Log.DebugGreen($"出售：{item.ConfigId} {item.item_Info.Name}");
            //        //确定将 物品 出售
            //        G2C_SellingItemToNPCShop g2C_SellingItemToNPC = (G2C_SellingItemToNPCShop)await SessionComponent.Instance.Session.Call(new C2G_SellingItemToNPCShop
            //        {
            //            NPCShopID = CurNpcUUid, //商店NPC Id
            //            ItemUUID = item.UUID //卖出的物品的 UUID
            //        });
            //        if (g2C_SellingItemToNPC.Error != 0)
            //        {
            //            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SellingItemToNPC.Error.GetTipInfo());
            //        }
            //    }
            //    RecycleEquipList.Clear();
            //    coin.text = string.Empty;
                
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, $"获得收益金币 + {glodcoinCount}");
            //});

           
        }
        public void AddRecycleEquip(KnapsackDataItem item)
        {
            if (RecycleEquipList.Contains(item) == false)
            {
                RecycleEquipList.Add(item);
                ShowSellEquipList();
                ChangeGlodcoin(item.GetProperValue(E_ItemValue.SellMoney));
            }
        }
        public void RemoveRecycleEquip(KnapsackDataItem item)
        {

            if (RecycleEquipList.Contains(item))
            {
                RecycleEquipList.Remove(item);
                ShowSellEquipList();
                ChangeGlodcoin(item.GetProperValue(E_ItemValue.SellMoney), false);
            }

            
           

        }
        void ChangeGlodcoin(long value, bool isAdd = true)
        {
            if (isAdd)
            {

                coin.text = $"{glodcoinCount += value}";
            }
            else
            {

                coin.text = $"{Mathf.Abs(glodcoinCount -= value)}";
            }
        }
        void FiltrateEquip()
        {
            RecycleEquipList.Clear();
            var list = KnapsackItemsManager.KnapsackItems.Values.ToList();

            foreach (var item in list)
            {
                if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
                //翅膀不能卖
                if (item.IsCanSell() == false) continue;

                if (RecycleEquipTools.SingleExcellence)
                {
                    if (item.IsSingleExcellence())
                    {
                        AddRecycleEquip(item);
                    }
                }
                if (RecycleEquipTools.WhiteSuit)
                {
                    //卓越 幸运、套装、洞转、橙装、追加属性、再生属性、技能
                    if (item.IsWhiteSuit())
                    {
                        AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.DoubleExcellence)
                {
                    if (item.IsDoubleExcellence())
                    {
                        AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.SkillBook)
                {
                    if (item.IsSkillBook())
                    {
                        AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.IntensifyEquip)
                {
                    if (item.IsIntensifyEquip())
                    {
                        AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.Red_BlueMedicine)
                {
                    if (item.IsRed_BlueMedicine())
                    {
                        AddRecycleEquip(item);
                    }
                }
                //if (RecycleEquipTools.IsLucky)
                //{
                //    if (item.Islucky())
                //    {
                //        AddRecycleEquip(item);
                //    }
                //}
                if (RecycleEquipTools.IsSkillId)
                {
                    if (item.IsSkill())
                    {
                        AddRecycleEquip(item);
                    }
                }
            }
        }
      
        public void ShowSellEquipList()
        {
            int atrCount = RecycleEquipList.Count;
            int introChildCount = content.childCount;
            if (introChildCount > atrCount)//隐藏多余的Item
            {
                for (int i = atrCount; i < introChildCount; i++)
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < atrCount; i++)
            {
                Transform item;
                if (i < introChildCount)
                {
                    item = content.GetChild(i);
                    item.gameObject.SetActive(true);

                }
                else
                {
                    item = GameObject.Instantiate<Transform>(verticaItem, content);
                }
                itemNames.Clear();
                RecycleEquipList[i].GetItemName(ref itemNames);
                item.GetComponent<Text>().text = itemNames[1] + "售价：" + RecycleEquipList[i].GetProperValue(E_ItemValue.SellMoney) + "金币";
            }
            coin.text = $"{glodcoinCount}";
        }
        public void CleanRecycle()
        {
            RecycleEquipList.Clear();
            glodcoinCount = 0;
           
        }
    }
}
