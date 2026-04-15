using ILRuntime.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ETHotfix
{
    /// <summary>
    /// 合成方法基类
    /// </summary>
    public abstract partial class MergerMethod
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int PriorityLev;
        /// <summary>
        /// 合成面板下方的提示信息
        /// </summary>
        public List<string> textBom = new List<string>();
        public string Title = string.Empty;

        public int Money;//所需金币

        public int SuccessRate = 0;//成功率

        public int MaxSuccessRate = 100;//最大成功率

        public int mergerMethodId = 0;//合成方法的ID

        public string mergerMethod;//合成方法

        public bool FailedDelete = true;//合成失败后 是否删除

        public List<KnapsackDataItem> CheckItems = new List<KnapsackDataItem>();//需要检查的物品列表

        public bool IsHideSynTopSucess = false;//是否隐藏 合成成功率

        public bool IsCanUserLuckCharm = false;//是否可以使用幸运符咒、

        public Synthesis_InfoConfig SynthesisInfoConfig=>ConfigComponent.Instance.GetItem<Synthesis_InfoConfig>(mergerMethodId);//合成配置

        /// <summary>
        /// 是否可以 使用改合成方法
        /// </summary>
        public bool IsCanMerger = false;

        public MergerMethod Init(List<KnapsackDataItem> all_Items)
        {
            Clean();
            foreach (var item in all_Items)
            {
                CheckItems.Add(item);
               
            }
          
            return this;
        }

        public void Clean()
        {
            textBom.Clear();
            Money = 0;
            SuccessRate = 0;
            MaxSuccessRate = 0;
            FailedDelete = false;
            CheckItems.Clear();
        }

        //添加需要检查的物品
        public MergerMethod AddCheackItem(KnapsackDataItem item)
        {
         
            if (item != null)
            {
                CheckItems.Add(item);
            }
           
            return this;
        }
        //添加成功率
        public void AddSuccessRate(int value)
        {
            SuccessRate += value;
            SuccessRate = SuccessRate >= MaxSuccessRate ? MaxSuccessRate : SuccessRate;
        }

        protected void AddText(string str)
        {
            textBom.Add(str);
        }
        protected void RemoveText(int index,long configId)
        {
            textBom.RemoveAt(index);
            //UIKnapsackComponent.Instance.UpdateTips();
        }
        /// <summary>
        /// 标题
        /// </summary>
        /// <param name="str"></param>
        protected void AddTextTitle(string str)
        {
            Title = $"<color=#EEDA00>{str}</color>"; //黄色
        }
        /// <summary>
        /// 显示必要物品 提示信息
        /// </summary>
        /// <param name="str">提示类容</param>
        /// <param name="isHave">是否已经添加改物品</param>
        protected void AddMustItemInfoText(string str, bool isHave, bool isEnough = true)
        {
            // isHave ? isEnough ? "紫色" : "淡蓝" : "红色";
            string color = isHave ? isEnough ? "#FF32FF" : "#E6FFFF" : "#FF0202";
            AddText($"<color={color}>{str}</color>");
        }
        /// <summary>
        /// 显示辅助物品提示信息
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isHave"></param>
        protected void AddSubItemInfoText(string str, bool isHave)
        {
            //isHave ? "紫色" : "蓝色";
            string color = isHave ? "#FF32FF" : "#3214FF";
            AddText($"<color={color}>{str}</color>");
        }
        /// <summary>
        /// 判断合成面板中的物品是否可以使用此合成方法
        /// </summary>
        /// <returns></returns>
        public abstract bool CanUserThisMergerMethod();

        protected bool CheckItemCount()
        {
            if (CheckItems.Count != 0)
            {
                //IsCanMerger = false;
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// 面板中的物品 是否可以合成
        /// </summary>
        /// <returns></returns>
        public virtual bool CanMerger()
        {
            return false;
        }
        /// <summary>
        /// 是否拥有幸运符咒
        /// </summary>
        /// <param name="itemConfigId"></param>
        /// <param name="addSuccessrateValue"></param>
        /// <param name="IsMust"></param>
        /// <returns></returns>
        protected bool IsHaveLuckFuZhou(int addSuccessrateValue = 0, bool IsMust = true)
        {
            bool isHave = false;
            int count = 0;//最多十个
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
                if (CheckItems[i].ConfigId == 320120|| CheckItems[i].ConfigId ==320400)
                {
                    count += CheckItems[i].GetProperValue(E_ItemValue.Quantity);
                    if (count > 10)
                    {
                        break;
                    }
                    /*if (CheckItems[i].GetProperValue(E_ItemValue.Quantity) == 1)//数量为一 就移除
                        CheckItems.RemoveAt(i);
                    else
                        CheckItems[i].SetProperValue(E_ItemValue.Quantity, CheckItems[i].GetProperValue(E_ItemValue.Quantity) - 1);//数量减1*/

                    CheckItems.RemoveAt(i);
                  
                    AddSuccessRate(addSuccessrateValue);
                    isHave = true;

                    continue;
                }
            }
            if (IsMust)
                IsCanMerger = isHave;

            return isHave;
        }
        /// <summary>
        /// 是否拥有固定数量
        /// </summary>
        /// <param name="itemConfigId"></param>
        /// <param name="addSuccessrateValue"></param>
        /// <param name="IsMust"></param>
        /// <returns></returns>
        protected bool IsHaveGuDing(long itemConfigId,int needCount, int addSuccessrateValue = 0, bool IsMust = true)
        {
            bool isHave = false;
            int count = 0;//最多十个
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
                if (CheckItems[i].ConfigId == itemConfigId)
                {
                    count += CheckItems[i].GetProperValue(E_ItemValue.Quantity);
                    if (count > needCount)
                    {
                        break;
                    }
                    /*if (CheckItems[i].GetProperValue(E_ItemValue.Quantity) == 1)//数量为一 就移除
                        CheckItems.RemoveAt(i);
                    else
                        CheckItems[i].SetProperValue(E_ItemValue.Quantity, CheckItems[i].GetProperValue(E_ItemValue.Quantity) - 1);//数量减1*/

                    CheckItems.RemoveAt(i);
                    AddSuccessRate(addSuccessrateValue);
                    isHave = true;

                    continue;
                }
            }
            if (IsMust)
                IsCanMerger = isHave;

            return isHave;
        }
        /// <summary>
        /// 是否拥有魔晶石
        /// </summary>
        /// <param name="itemConfigId"></param>
        /// <param name="addSuccessrateValue"></param>
        /// <param name="IsMust"></param>
        /// <returns></returns>
        protected bool IsHaveMOJING_STONE(long itemConfigId, int addSuccessrateValue = 0, bool IsMust = true)
        {
            bool isHave = false;
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
                if (CheckItems[i].ConfigId == itemConfigId)
                {
                    
                    AddSuccessRate(addSuccessrateValue * CheckItems[i].GetProperValue(E_ItemValue.Quantity));
                    isHave = true;
                    CheckItems.RemoveAt(i);
                }
            }
            if (IsMust)
                IsCanMerger = isHave;

            return isHave;
        }
        /// <summary>
        /// 是否拥有已经添加改装备
        /// </summary>
        /// <param name="addSuccessrateValue">被检查的物品的配置表ID</param>
        /// <param name="itemConfigId"></param>
        /// <returns></returns>
        protected bool IsHaveItem(long itemConfigId, int addSuccessrateValue = 0, bool IsMust = true)
        {
            
            bool isHave = false;
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
                if (CheckItems[i].ConfigId == itemConfigId)
                {
                    if (CheckItems[i].GetProperValue(E_ItemValue.Quantity) != 1)//数量为一
                    {
                        continue;
                    }
                    /* CheckItems.RemoveAt(i);
                    else
                        CheckItems[i].SetProperValue(E_ItemValue.Quantity, CheckItems[i].GetProperValue(E_ItemValue.Quantity) - 1);//数量减1*/

                    AddSuccessRate(addSuccessrateValue * CheckItems[i].GetProperValue(E_ItemValue.Quantity));
                    CheckItems.RemoveAt(i);
                    isHave = true;
                    break;
                }
            }
            if (IsMust)
                IsCanMerger = isHave;

            return isHave;
        }
        /// <summary>
        /// 物品是否是 某个等级的物品
        /// </summary>
        /// <param name="itemConfigId">物品的configId</param>
        /// <param name="lev">物品的等级</param>
        /// <param name="addSuccessrateValue"></param>
        /// <param name="IsMust"></param>
        /// <returns></returns>
        protected bool IsHaveItem(long itemConfigId, int lev, int addSuccessrateValue = 0, bool IsMust = true)
        {
            bool isHave = false;
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
                if (CheckItems[i].ConfigId == itemConfigId && CheckItems[i].GetProperValue(E_ItemValue.Level) == lev)
                {
                    /*if (CheckItems[i].GetProperValue(E_ItemValue.Quantity) == 1)//数量为一 就移除
                        CheckItems.RemoveAt(i);
                    else
                        CheckItems[i].SetProperValue(E_ItemValue.Quantity, CheckItems[i].GetProperValue(E_ItemValue.Quantity) - 1);//数量减1*/

                    CheckItems.RemoveAt(i);
                    AddSuccessRate(addSuccessrateValue);
                    isHave = true;
                    break;
                }
            }
            if (IsMust)
                IsCanMerger = isHave;

            return isHave;
        }
        protected bool IsHaveItem(long itemConfigId, ref int lev, int addSuccessrateValue = 0, bool IsMust = true)
        {
            bool isHave = false;
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
                if (CheckItems[i].ConfigId == itemConfigId)
                {
                    lev = CheckItems[i].GetProperValue(E_ItemValue.Level);
                   /* if (CheckItems[i].GetProperValue(E_ItemValue.Quantity) == 1)//数量为一 就移除
                        CheckItems.RemoveAt(i);
                    else
                        CheckItems[i].SetProperValue(E_ItemValue.Quantity, CheckItems[i].GetProperValue(E_ItemValue.Quantity) - 1);//数量减1*/

                    CheckItems.RemoveAt(i);
                    AddSuccessRate(addSuccessrateValue);
                    isHave = true;
                    break;
                }
            }
            if (IsMust)
                IsCanMerger = isHave;

            return isHave;
        }
        protected bool IsHaveItem(long itemConfigId, int needCount, out int curCount, int addSuccessrateValue = 0, bool IsMust = true)
        {
            bool isHave = false;
            curCount = 0;
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
               
                if (CheckItems[i].ConfigId == itemConfigId)
                {
                    if (CheckItems[i].GetProperValue(E_ItemValue.Quantity) >needCount)
                    {
                        break;
                    }
                    curCount += CheckItems[i].GetProperValue(E_ItemValue.Quantity);
                    if (curCount >needCount)
                    {
                        break;
                    }
                    CheckItems.RemoveAt(i);
                    AddSuccessRate(addSuccessrateValue);
                    isHave = curCount>=needCount;
                }
            }
            if (IsMust)
                IsCanMerger = isHave;
            return isHave;
        }
        protected bool IsHaveItem(long itemConfigId, long needCount, int addSuccessrateValue = 0, bool IsMust = true)
        {
            bool isHave = false;
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
                if (CheckItems[i].ConfigId == itemConfigId)
                {
                    AddSuccessRate(addSuccessrateValue * CheckItems[i].GetProperValue(E_ItemValue.Quantity));
                    CheckItems.RemoveAt(i);
                    isHave = true;
                }
            }
            if (IsMust)
                IsCanMerger = isHave;
            return isHave;
        }
        /// <summary>
        /// 是否有 +N以上的物品
        /// </summary>
        /// <param name="lev">+N 等级</param>
        /// <param name="addSuccessrateValue"></param>
        /// <returns></returns>
        protected bool IsHaveItem_Lev(int lev, int addSuccessrateValue = 0, bool IsMust = true)
        {
            bool isHave = false;
            for (int i = CheckItems.Count - 1; i >= 0; i--)
            {
                if (CheckItems[i].GetProperValue(E_ItemValue.Level) >= lev)
                {
                    CheckItems.RemoveAt(i);
                    AddSuccessRate(addSuccessrateValue);
                    isHave = true;
                    break;
                }
            }
            if (IsMust)
                IsCanMerger = isHave;
            return isHave;
        }

    }
}