using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UITreasureHouseComponent
    {
        private List<string> ItemAtrList;
        public UICircularScrollView<string> uICircular_ItemInfo;
        public ScrollRect itemInfoScrollView;
        public GameObject itemInfoContent;
        public void ItemInfo()
        {
            itemInfoScrollView = rightListCollector.GetImage("BuyingScrollView").GetComponent<ScrollRect>();
            itemInfoContent = rightListCollector.GetGameObject("BuyingContent");
            rightListCollector.GetButton("BuyBtn").onClick.AddSingleListener(() =>
            {
                Log.DebugGreen("购买!");
                if(lastClickItemInfo != null)
                    BuyTreasureHouseItemInfo(lastClickItemInfo).Coroutine();
            });
            rightListCollector.GetButton("DelistBtn").onClick.AddSingleListener(() =>
            {
                Log.DebugGreen("下架!");
                if(lastClickItemInfo != null)
                    RemovedItems(lastClickItemInfo.Uid).Coroutine();
            });
            UICircularItemInfoInit();
        }
        /// <summary>
        /// 初始化物品滑动框
        /// </summary>
        public void UICircularItemInfoInit()
        {
            uICircular_ItemInfo = ComponentFactory.Create<UICircularScrollView<string>>();
            uICircular_ItemInfo.InitInfo(E_Direction.Vertical, 1, 0, 0);
            uICircular_ItemInfo.ItemInfoCallBack = InitItemInfoList;
            uICircular_ItemInfo.IninContent(itemInfoContent, itemInfoScrollView);
        }

        private void InitItemInfoList(GameObject arg1, string arg2)
        {
            arg1.transform.Find("Text").GetComponent<Text>().text = arg2;
        }

        public void GetAllAtrs(KnapsackDataItem dataItem)
        {
            if (ItemAtrList == null)
            {
                ItemAtrList = new List<string>(100);
            }
            ItemAtrList.Clear();
            dataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);

            dataItem.GetItemName(ref ItemAtrList);//装备 名字
            dataItem.GetItemCount(ref ItemAtrList);//数量

            if (dataItem.ConfigId == 260012)//黑王马
            {
                dataItem.GetHeiWangMaAtrs(ref ItemAtrList);
            }
            else if (dataItem.ConfigId == 260011)//炎狼兽之角 +幻影
            {
                dataItem.GetYangLangShouZhiJiaoHuanYingAtrs(ref ItemAtrList);
            }
            else if (dataItem.IsTreasureItem())
            {
                dataItem.GetTreasureAtrs(ref ItemAtrList);
            }
            else
            {
                dataItem.GetBaseAtrs(ref ItemAtrList);//基本属性(读表)
                dataItem.GetItemCommonBaseAtr(ref ItemAtrList);//基本属性
                dataItem.GetGemsAtr(ref ItemAtrList);//荧光宝石属性
                                                     // dataItem.GetLevNeed(ref ItemAtrList);//等级需求
                dataItem.GetUserType(ref ItemAtrList);//职业限制
                dataItem.GetExtraEntryAtr(ref ItemAtrList);//套装附带的额外属性
                dataItem.GetItemSkill(ref ItemAtrList);//技能
                dataItem.GetLuckyAtr(ref ItemAtrList);//幸运属性
                dataItem.GetAppendAtr(ref ItemAtrList);//追加属性
                dataItem.GetExecllentEntry(ref ItemAtrList);//卓越属性
                dataItem.GetSpecialEntry(ref ItemAtrList);//特殊属性-翅膀
                dataItem.GetReginAtr(ref ItemAtrList);//再生属性
                dataItem.GetInlayAtr(ref ItemAtrList);//镶嵌属性
                dataItem.GetSuitAtr(ref ItemAtrList);//套装属性
                dataItem.GetVaildTime(ref ItemAtrList);//有效时间
                dataItem.GetRemarks(ref ItemAtrList);//备注提示信息
                dataItem.GetAdmissionTicketOpenTime(ref ItemAtrList);//副本开放时间
            }
            uICircular_ItemInfo.Items = ItemAtrList;
        }
    }

}
