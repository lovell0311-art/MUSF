using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;
using System.Linq;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 物品合成
    /// </summary>
    public partial class UIKnapsackComponent
    {
        private GameObject MergerContent;

        private KnapsackGrid[][] MergerGrids;
        public const int LENGTH_Merger_X = 8;
        public const int LENGTH_Merger_Y = 4;

        private ReferenceCollector reference_Merger;
        Text TopTitle, TopTipSucceed, TopTipMoney, FailedDeleTip, Title_Txt;
        Text Tips, TipsTxt;
        Button MergeBtn;
        public MergerMethod curMergerMethod = null;

        public List<MergerMethod> allMergerMethod = new List<MergerMethod>();//全部的合成方法

        public List<KnapsackDataItem> alreadyAddMergerItems = new List<KnapsackDataItem>();//当前 已经放入合成面板的物品

        public Dictionary<long, KnapsackDataItem> mergerItemList = new Dictionary<long, KnapsackDataItem>();//合成成功的物品
       
        public void Init_Merger(SynthesisData synthesisData = SynthesisData.Null) 
        {
            alreadyAddMergerItems.Clear();

            reference_Merger = Merger.GetReferenceCollector();
            Title_Txt = reference_Merger.GetText("Title_Txt");//合成标题
            Title_Txt.text = GetMethod();
            TopTitle = reference_Merger.GetText("TopTitle");//合成标题
            TopTipSucceed = reference_Merger.GetText("TopTipSucceed");//合成成功率
            TopTipMoney = reference_Merger.GetText("TopTipMoney");//合成所需的金币
            FailedDeleTip = reference_Merger.GetText("FailedDeleTip");//合成失败 材料消失提示
            Tips = reference_Merger.GetText("Tips");
            TipsTxt = reference_Merger.GetText("TipsTxt");
            MergeBtn = reference_Merger.GetButton("MergeBtn");
            MergeBtn.interactable = false;
            MergeBtn.onClick.AddSingleListener(MergerEvent);
            MergerContent = reference_Merger.GetGameObject("Grids");
            MergerGrids = new KnapsackGrid[LENGTH_Merger_X][];
            for (int i = 0; i < MergerGrids.Length; i++)
            {
                MergerGrids[i] = new KnapsackGrid[LENGTH_Merger_Y];
            }
            //初始化格子
            CreatGrid(LENGTH_Merger_X, LENGTH_Merger_Y, MergerContent.transform, E_Grid_Type.Gem_Merge, ref MergerGrids);
            GetAllMergerTips();
            GetAllMergerMethods(synthesisData);

            string GetMethod() => synthesisData switch
            {
                SynthesisData.YiBanSynthesis => "一般合成",
                SynthesisData.CiBangSynthesis => "翅膀合成",
                SynthesisData.WuQiSynthesis => "武器合成",
                SynthesisData.ZuoQiSynthesis => "坐骑合成",
                SynthesisData.MoJingShi => "魔晶石生成",
                SynthesisData.ZhuangBeiQiangHua => "装备强化",
                SynthesisData.MaYaWuQiSynthesis => "玛雅武器合成",
                SynthesisData.YouAnYiBanSynthesis => "进阶合成",
                SynthesisData.ZaiShengGen => "再生宝石生成",
                SynthesisData.GemExchange => "兑换U币",
                SynthesisData.ZhuoYueAttributesRandom => "卓越装备属性成长",
                SynthesisData.RemoveRegeneration => "去除再生宝石属性",
                _=>string.Empty
            };

            ///加载所有合成方法
            void GetAllMergerMethods(SynthesisData synthesisData = SynthesisData.Null) 
            {
                List<Type> types = Game.EventSystem.GetTypes();
                for (int i = 0, length=types.Count; i < length; i++)
                {
                    Type type = types[i];
                    object[] attrs=type.GetCustomAttributes(typeof(MergerSystemAttribute),false);
                    if (attrs.Length == 0) continue;
                    MergerSystemAttribute attribute = attrs[0] as MergerSystemAttribute;
                    if (allMergerMethod.Exists(r => r.GetType() == attribute.GetType()))
                    {
                        Log.DebugRed($"已经存在 该合成方法：{attribute.GetType()}");
                    }
                    object o = Activator.CreateInstance(type);
                    if (!(o is MergerMethod mergerMethod))
                    {
                        //Log.DebugRed($"{o.GetType().FullName} 没有继承 MergerMethod");
                        continue;
                    }
                    //Log.DebugGreen($"o.GetType().FullName:{o.GetType().FullName}");
                    mergerMethod.PriorityLev = attribute.Prioritylev;
                    if((int)(attribute.Prioritylev /100) == synthesisData.ToInt32())
                    {
                       // Log.DebugBrown("合成" + (int)(attribute.Prioritylev / 100) + ":synthesisData" + synthesisData.ToInt32()+"::数据"+ attribute.Prioritylev);
                        allMergerMethod.Add(mergerMethod);
                    }
                }
                allMergerMethod.Sort((a,b)=>a.PriorityLev.CompareTo(b.PriorityLev));

            }
            
            void GetAllMergerTips()
            {
                int congifId = (int)synthesisData == 1 ? 1 : (int)(synthesisData - 1) * 100;
                Log.DebugGreen("合成提示"+congifId.ToString());
                SynthesisTips_InfoConfig synthesisTips_InfoConfig = ConfigComponent.Instance.GetItem<SynthesisTips_InfoConfig>(congifId);
                string tipstext = null;
                foreach (var item in synthesisTips_InfoConfig.SonId)
                {
                    SynthesisTips_InfoConfig synthesisTips_InfoConfig1 = ConfigComponent.Instance.GetItem<SynthesisTips_InfoConfig>(item);
                    if (synthesisTips_InfoConfig1 != null)
                    {
                        tipstext += $"<color=red>{synthesisTips_InfoConfig1.FunctionName}</color>:{synthesisTips_InfoConfig1.Desk}\n";
                    }
                }
                TipsTxt.text = tipstext;
            }

            ResetTips();
        }

        /// <summary>
        /// 请求合成
        /// </summary>
        private void MergerEvent() 
        {
           
            MergerAsync().Coroutine();
            mergerItemList.Clear();
            async ETVoid MergerAsync()
            {
               // C2G_ItemsSynthesis c2G_Items = new C2G_ItemsSynthesis();
                var allItemUUIDList=new List<long>();
                bool protect = false;
                for (int i = 0, length= alreadyAddMergerItems.Count; i < length; i++)
                {
                    allItemUUIDList.Add(alreadyAddMergerItems[i].UUID);
                    //c2G_Items.AllItemUUID.Add(alreadyAddMergerItems[i].UUID);
                    Log.DebugGreen($"合成材料：{alreadyAddMergerItems[i].ConfigId}");
                    if (alreadyAddMergerItems[i].ConfigId==320318|| alreadyAddMergerItems[i].ConfigId == 320141)
                    {
                        protect = true;
                    }
                }
               // c2G_Items.MethodConfigID = curMergerMethod.mergerMethodId;
                Log.DebugGreen($"请求合成：{curMergerMethod.mergerMethodId}  shuliang: {allItemUUIDList.Count}");
                G2C_ItemsSynthesis g2C_ItemsSynthesis = (G2C_ItemsSynthesis)await SessionComponent.Instance.Session.Call(new C2G_ItemsSynthesis
                {
                 AllItemUUID=new Google.Protobuf.Collections.RepeatedField<long> {allItemUUIDList},
                 MethodConfigID= curMergerMethod.mergerMethodId,
                 ClientFinalR =curMergerMethod.SuccessRate
                });
                if (g2C_ItemsSynthesis.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ItemsSynthesis.Error.GetTipInfo());
                  //  Log.DebugGreen($"g2C_ItemsSynthesis.Error：{g2C_ItemsSynthesis.Error}");
                }
                else
                {
                    CleanMerger();
                    Log.DebugBrown($"合成成功： {g2C_ItemsSynthesis.Result}   {g2C_ItemsSynthesis.AddedItem.Count}");
                    if (g2C_ItemsSynthesis.Result || ((!g2C_ItemsSynthesis.Result) && g2C_ItemsSynthesis.AddedItem.count >= 1))
                    {
                        if (((!g2C_ItemsSynthesis.Result) && g2C_ItemsSynthesis.AddedItem.count >= 1))
                        {
                             UIComponent.Instance.VisibleUI(UIType.UIHint,"1合成失败");
                        }
                        else{
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "合成成功");
                        }
                        
                        //显示合成后的物品
                        for (int i = 0, length = g2C_ItemsSynthesis.AddedItem.Count; i < length; i++)
                        {
                           
                            var item = g2C_ItemsSynthesis.AddedItem[i];
                          //  Log.DebugGreen($"X:{item.PosInBackpackX} Y:{item.PosInBackpackY} W:{item.Width} H:{item.Height}");
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
                            AddItem(knapsackDataItem, type: E_Grid_Type.Gem_Merge);
                            mergerItemList[item.ItemUID]=knapsackDataItem;
                        }
                    }
                    else
                    {
                        if (protect==true)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "合成失败,装备已保护");
                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "合成失败,装备已破碎");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 移除已经放入合成面板的物品
        /// </summary>
        void RemoveMergerItem()
        {
            if (alreadyAddMergerItems.Exists(r => r.UUID == originArea.UUID))
            {
                var item = alreadyAddMergerItems.Find(r => r.UUID == originArea.UUID);
                item.Dispose();
                alreadyAddMergerItems.Remove(item);
                
            }
          
        }
       public void AddMergerItem(KnapsackDataItem item)
        {
            if (!alreadyAddMergerItems.Exists(r => r.UUID == item.UUID))
            {
                alreadyAddMergerItems.Add(item);
              
            }

        }
        /// <summary>
        /// 刷新 合成方法
        /// </summary>
        public void RefreshMergerMethods(KnapsackDataItem item) 
        {
            curMergerMethod = null;
            if (item == null)
            {

                return;
            }
            //if (item==null&&alreadyAddMergerItems.Count == 0)
            //{
                
            //    return;
            //}
            for (int i = 0, length = allMergerMethod.Count; i < length; i++)
            {
               var mergermethod = allMergerMethod[i].Init(alreadyAddMergerItems);
               Log.DebugRed($"allMergerMethod[i].Init(alreadyAddMergerItems):{mergermethod.CheckItems.Count}");
               mergermethod.AddCheackItem(item);
             //   Log.DebugRed($"allMergerMethod[i].AddCheackItem(alreadyAddMergerItems):{mergermethod.CheckItems.Count}");
                if (mergermethod.CanUserThisMergerMethod())
                {
                 //   Log.DebugBrown($"curMergerMethod方法 {mergermethod}");
                    curMergerMethod = mergermethod;
                    if (mergermethod.IsHideSynTopSucess)//隐藏成功 几率
                    {
                        TopTipSucceed.gameObject.SetActive(false);
                    }
                    break;
                }
            }
        }
        //重置面板显示的文字
        public void ResetTips() 
        {
            TopTitle.text = string.Empty;
            TopTipSucceed.text = $"合成时成功率：{0}%";
            TopTipMoney.text = $"必要的金币：0({roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin):N})";
            Tips.text = $"<color=red>请把合成材料放上去</color>";
            //是否可以合成
            MergeBtn.interactable = false;
        }
        /// <summary>
        /// 更新面板上的显示信息
        /// </summary>
        public void UpdateTips()
        {
            //Log.DebugBrown($"curMergerMethod == null:{curMergerMethod == null}");
            if (curMergerMethod == null)
            {
                ResetTips();
                return;
            }
            //显示合成标题
            TopTitle.text = curMergerMethod.Title;
            //成功率
            TopTipSucceed.text = $"合成时成功率：{curMergerMethod.SuccessRate}%";
            //所需金币
            string color = curMergerMethod.Money > roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin)?ColorTools.GetColorHtmlString(Color.red): ColorTools.GetColorHtmlString(Color.white);
            TopTipMoney.text = $"必要的金币：<color={color}>{curMergerMethod.Money:N}</color>({roleEntity.Property.GetProperValue(E_GameProperty.GoldCoin):N})";
            //更新显示 所需材料
            Tips.text = string.Empty;
            for (int i = 0, length=curMergerMethod.textBom.Count; i < length; i++)
            {
                Tips.text += curMergerMethod.textBom[i] + "\n";
            }
            //强化失败装备是否销毁
            FailedDeleTip.gameObject.SetActive(curMergerMethod.FailedDelete);
            //是否可以合成
            MergeBtn.interactable = curMergerMethod.IsCanMerger;
        }

        /// <summary>
        /// 合成面板上是否还有 物品
        /// </summary>
        /// <returns>有物品 返回 true</returns>
        public bool MergerPanelHavaItem() 
        {
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Gem_Merge);
            for (int i = 0; i < LENGTH_Merger_Y; i++)
            {
                for (int j = 0; j < LENGTH_Merger_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    if (grid.IsOccupy)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint,"请把合成面板上的 物品放回背包");
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 清理合成面板
        /// </summary>
        public void CleanMerger()
        {
            GetKnapsackGrid(ref grids, ref LENGTH_X, ref LENGTH_Y, E_Grid_Type.Gem_Merge);
            for (int i = 0; i < LENGTH_Merger_Y; i++)
            {
                for (int j = 0; j < LENGTH_Merger_X; j++)
                {
                    KnapsackGrid grid = grids[j][i];
                    if (grid.IsOccupy)
                    {
                        RemoveItem(grid.Data, true);
                    }
                }

            }
            for (int i = 0; i < alreadyAddMergerItems.Count; i++)
            {
                alreadyAddMergerItems[i].Dispose();
            }
            alreadyAddMergerItems.Clear();
            mergerItemList.Clear();
         
        }
    }
}