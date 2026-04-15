using ETModel;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;
using static UnityEditor.Progress;

namespace ETHotfix
{
    public static class UIKnapsackNewComponentSellSystem
    {
        public static async ETTask InitSell(this UIKnapsackNewComponent self)
        {
            if (self.isOpenSell)
            {
                Log.Info("自动回收界面已经打开");
                return;
            }
            self.isOpenSell = true;
            string res = "Sell";
            //加载对应面板
            await ResourcesComponent.Instance.LoadGameObjectAsync(res.StringToAB(), res);
            //实例化面板 
            GameObject sell = ResourcesComponent.Instance.LoadGameObject(res.StringToAB(), res);
            sell.transform.SetParent(self.plane.transform, false);
            sell.transform.localPosition = Vector3.zero;
            sell.transform.localScale = Vector3.one;
            self.sellCollector = sell.GetReferenceCollector();


            self.content = self.sellCollector.GetGameObject("Content").transform;
            self.verticaItem = self.content.GetChild(0);
            self.coin = self.sellCollector.GetText("glodcoin");

            self.FiltrateEquip();
            self.ShowSellEquipList();

            self.sellCollector.GetToggle("Toggle").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.SingleExcellence = value;
                self.OnRecycleClick(1, value);
            });
            self.sellCollector.GetToggle("Toggle").isOn = RecycleEquipTools.SingleExcellence;

            self.sellCollector.GetToggle("Toggle (1)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.WhiteSuit = value;
                self.OnRecycleClick(2, value);
            });
            self.sellCollector.GetToggle("Toggle (1)").isOn = RecycleEquipTools.WhiteSuit;

            self.sellCollector.GetToggle("Toggle (2)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.DoubleExcellence = value;
                self.OnRecycleClick(3, value);
            });
            self.sellCollector.GetToggle("Toggle (2)").isOn = RecycleEquipTools.DoubleExcellence;

            self.sellCollector.GetToggle("Toggle (3)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.SkillBook = value;
                self.OnRecycleClick(4, value);
            });
            self.sellCollector.GetToggle("Toggle (3)").isOn = RecycleEquipTools.SkillBook;

            self.sellCollector.GetToggle("Toggle (7)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.IsSkillId = value;
                self.OnRecycleClick(5, value);
            });
            self.sellCollector.GetToggle("Toggle (7)").isOn = RecycleEquipTools.IsSkillId;

            self.sellCollector.GetToggle("Toggle (5)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.IntensifyEquip = value;
                self.OnRecycleClick(6, value);
            });
            self.sellCollector.GetToggle("Toggle (5)").isOn = RecycleEquipTools.IntensifyEquip;

            self.sellCollector.GetToggle("Toggle (4)").onValueChanged.AddSingleListener(value =>
            {
                RecycleEquipTools.Red_BlueMedicine = value;
                self.OnRecycleClick(7, value);
            });
            self.sellCollector.GetToggle("Toggle (4)").isOn = RecycleEquipTools.Red_BlueMedicine;


            //回收
            self.sellCollector.GetButton("sellbtn").onClick.AddSingleListener(async () =>
            {
                RecycleEquipTools.Sava();
                if (self.RecycleEquipList.Count == 0) return;
                foreach (var item in self.RecycleEquipList)
                {
                    // Log.DebugGreen($"出售：{item.ConfigId} {item.item_Info.Name}");
                    //确定将 物品 出售
                    G2C_SellingItemToNPCShop g2C_SellingItemToNPC = (G2C_SellingItemToNPCShop)await SessionComponent.Instance.Session.Call(new C2G_SellingItemToNPCShop
                    {
                        NPCShopID = self.CurNpcUUid, //商店NPC Id
                        ItemUUID = item.UUID //卖出的物品的 UUID
                    });
                    if (g2C_SellingItemToNPC.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SellingItemToNPC.Error.GetTipInfo());
                    }
                }
                self.RecycleEquipList.Clear();
                self.coin.text = string.Empty;

                UIComponent.Instance.VisibleUI(UIType.UIHint, $"获得收益金币 + {self.glodcoinCount}");
            });
        }

        public static void ClearSell(this UIKnapsackNewComponent self)
        {
            self.isOpenSell = false;
            if (self.sellCollector)
            {
                GameObject.Destroy(self.sellCollector.gameObject);
                self.sellCollector = null;
            }
        }

        public static void OnRecycleClick(this UIKnapsackNewComponent self, int index, bool value)
        {
            if (value)
            {

                var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
                foreach (var item in list)
                {
                    if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;

                    if ((item.IsSingleExcellence() && index == 1) //单词条卓越装备回收
                        || (item.IsWhiteSuit() && index == 2)  //白装回收
                        || (item.IsDoubleExcellence() && index == 3) //双词条卓越装备回收
                        || (item.IsSkillBook() && index == 4) //技能书
                        || (item.IsSkill() && index == 5)   //回收技能/幸运装备
                        || (item.IsIntensifyEquip() && index == 6) //强化装备
                        || (item.IsRed_BlueMedicine() && index == 7)) //回收小瓶红蓝 药品
                    {
                        self.AddRecycleEquip(item);
                    }
                }
            }
            else
            {
                for (int i = self.RecycleEquipList.Count - 1; i >= 0; i--)
                {
                    KnapsackDataItem item = self.RecycleEquipList[i];
                    if ((item.IsSingleExcellence() && index == 1) //单词条卓越装备回收
                         || (item.IsWhiteSuit() && index == 2)  //白装回收
                         || (item.IsDoubleExcellence() && index == 3) //双词条卓越装备回收
                         || (item.IsSkillBook() && index == 4) //技能书
                         || (item.IsSkill() && index == 5)   //回收技能/幸运装备
                         || (item.IsIntensifyEquip() && index == 6) //强化装备
                         || (item.IsRed_BlueMedicine() && index == 7)) //回收小瓶红蓝 药品
                    {
                        self.RemoveRecycleEquip(item);
                    }
                }

            }

            RecycleEquipTools.Sava();
        }

        public static void FiltrateEquip(this UIKnapsackNewComponent self)
        {
            self.RecycleEquipList.Clear();
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
                        self.AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.WhiteSuit)
                {
                    //卓越 幸运、套装、洞转、橙装、追加属性、再生属性、技能
                    if (item.IsWhiteSuit())
                    {
                        self.AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.DoubleExcellence)
                {
                    if (item.IsDoubleExcellence())
                    {
                        self.AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.SkillBook)
                {
                    if (item.IsSkillBook())
                    {
                        self.AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.IntensifyEquip)
                {
                    if (item.IsIntensifyEquip())
                    {
                        self.AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.Red_BlueMedicine)
                {
                    if (item.IsRed_BlueMedicine())
                    {
                        self.AddRecycleEquip(item);
                    }
                }

                if (RecycleEquipTools.IsSkillId)
                {
                    if (item.IsSkill())
                    {
                        self.AddRecycleEquip(item);
                    }
                }
            }
        }

        public static void AddRecycleEquip(this UIKnapsackNewComponent self, KnapsackDataItem item)
        {
            if (self.RecycleEquipList.Contains(item) == false)
            {
                self.RecycleEquipList.Add(item);
                self.ShowSellEquipList();
                self.ChangeGlodcoin(item.GetProperValue(E_ItemValue.SellMoney));
            }
        }

        public static void RemoveRecycleEquip(this UIKnapsackNewComponent self, KnapsackDataItem item)
        {
            if (self.RecycleEquipList.Contains(item))
            {
                self.RecycleEquipList.Remove(item);
                self.ShowSellEquipList();
                self.ChangeGlodcoin(item.GetProperValue(E_ItemValue.SellMoney), false);
            }
        }

        public static void ShowSellEquipList(this UIKnapsackNewComponent self)
        {
            int atrCount = self.RecycleEquipList.Count;
            int introChildCount = self.content.childCount;
            if (introChildCount > atrCount)//隐藏多余的Item
            {
                for (int i = atrCount; i < introChildCount; i++)
                {
                    self.content.GetChild(i).gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < atrCount; i++)
            {
                Transform item;
                if (i < introChildCount)
                {
                    item = self.content.GetChild(i);
                    item.gameObject.SetActive(true);

                }
                else
                {
                    item = GameObject.Instantiate<Transform>(self.verticaItem, self.content);
                }
                self.itemNames.Clear();
                self.RecycleEquipList[i].GetItemName(ref self.itemNames);
                item.GetComponent<UnityEngine.UI.Text>().text = self.itemNames[1] + "售价：" + self.RecycleEquipList[i].GetProperValue(E_ItemValue.SellMoney) + "金币";
            }
            self.coin.text = $"{self.glodcoinCount}";
        }

        public static void ChangeGlodcoin(this UIKnapsackNewComponent self, long value, bool isAdd = true)
        {
            if (isAdd)
            {

                self.coin.text = $"{self.glodcoinCount += value}";
            }
            else
            {

                self.coin.text = $"{Mathf.Abs(self.glodcoinCount -= value)}";
            }
        }
    }
}
