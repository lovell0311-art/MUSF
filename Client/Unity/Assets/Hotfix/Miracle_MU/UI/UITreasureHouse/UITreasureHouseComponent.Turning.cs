using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UITreasureHouseComponent
    {
        public ReferenceCollector pageTurningcollector;
        public Text PageTxt;
        public int CurAllPage = 1;
        public int CurPage = 1;
        public void PageTurning()
        {
            pageTurningcollector = collector.GetGameObject("PageTurning").GetReferenceCollector();
            PageTxt = pageTurningcollector.GetText("PageTxt");
            //上一页
            pageTurningcollector.GetButton("UpPageBtn").onClick.AddSingleListener(() =>
            {
                if(CurPage == 1) 
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"当前已经是第一页！");
                    return; 
                }
                NextPage(CurPage - 1, pageType).Coroutine();
            });
            //下一页
            pageTurningcollector.GetButton("DownPageBtn").onClick.AddSingleListener(() =>
            {
                if (CurPage == CurAllPage) 
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "当前已经是最后一页！");
                    return;
                }
                NextPage(CurPage + 1, pageType).Coroutine();
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">当前页数</param>
        public void SetCurPage(int index)
        {
            CurPage = index;
            PageTxt.text = $"{index}/{CurAllPage}";
        }
    }

}
