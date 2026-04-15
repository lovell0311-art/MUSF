using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 荧光宝石强化
    /// </summary>
    public partial class UIKnapsackComponent
    {
        ReferenceCollector collector_YingGuangBaoShiQiangHua;
        readonly int Glod_QiangHua = 1_000_000;
        readonly int Success_QiangHua = 0;
        Text SecceedRateTxt;//成功率
        public void Init_YingGuangBaoShiQiangHua()
        {
            collector_YingGuangBaoShiQiangHua = YingGuangBaoShiQiangHuaPanel.GetReferenceCollector();
            InitItemGrid(collector_YingGuangBaoShiQiangHua, 3, E_InlayType.YingGuangBaoShiQiangHua, Glod_QiangHua, Success_QiangHua);
            SecceedRateTxt = collector_YingGuangBaoShiQiangHua.GetText("Success");
        }
    }
}
