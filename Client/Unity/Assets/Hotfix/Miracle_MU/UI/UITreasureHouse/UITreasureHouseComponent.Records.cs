using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UITreasureHouseComponent
    {
        public UICircularScrollView<TransactionRecord> uICircular_Records;
        public List<TransactionRecord> RecordsList = new List<TransactionRecord>();
        public ScrollRect TradingScrollView;
        public GameObject TradingContent;
        public void Records()
        {
            TradingScrollView = rightListCollector.GetImage("TradingScrollView").GetComponent<ScrollRect>();
            TradingContent = rightListCollector.GetGameObject("TradingContent");
            UICircularRecordsInit();
        }
        /// <summary>
        /// 初始化物品滑动框
        /// </summary>
        public void UICircularRecordsInit()
        {
            uICircular_Records = ComponentFactory.Create<UICircularScrollView<TransactionRecord>>();
            uICircular_Records.InitInfo(E_Direction.Vertical, 1, 0, 0);
            uICircular_Records.ItemInfoCallBack = InitRecordsList;
            uICircular_Records.IninContent(TradingContent, TradingScrollView);
        }

        private void InitRecordsList(GameObject item, TransactionRecord arg2)
        {
            string typeRecords = arg2.Type == 0 ? "出售" : "购买";
            int price = 0;
            if(arg2.Type == 0)
            {
                price = arg2.ActualPrice;
            }
            else
            {
                price = arg2.Price;
            }
            item.transform.Find("Text").GetComponent<Text>().text = $"{arg2.ItemName} {typeRecords}{price}";
        }
    }

}
