using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;

namespace ETHotfix
{
    /// <summary>
    /// 战盟 审核申请模块
    /// </summary>
    public partial class UIWarAllianceComponent
    {
        List<WarMemberInfo> WarApplyInfos = new List<WarMemberInfo>();
        public ScrollRect applyScrolView;
        Transform applyContent;

        ReferenceCollector collector_Apply;
        UICircularScrollView<WarMemberInfo> WarApplyInfoScrollView;

        public WarMemberInfo CurChooseWarApplyInfo = null;//当前选择的 成员信息

        public void Init_Apply()
        {
            collector_Apply = Apply.GetReferenceCollector();
            applyScrolView = collector_Apply.GetImage("ApplyScrollView").GetComponent<ScrollRect>();
            applyContent = collector_Apply.GetGameObject("Content").transform;
            collector_Apply.GetButton("AgreeAllBtn").onClick.AddSingleListener(() =>
            {
                List<long> gameIDs = new List<long>();
                WarApplyInfos.ForEach(n => gameIDs.Add(n.UUID));
                 WarAllianceAgreeOrReject(gameIDs, 1).Coroutine();
                
            });//全部拒绝
            collector_Apply.GetButton("RefusedAllBtn").onClick.AddSingleListener(() => 
            {
                List<long> gameIDs = new List<long>();
                WarApplyInfos.ForEach(n => gameIDs.Add(n.UUID));
                WarAllianceAgreeOrReject(gameIDs, 0).Coroutine();

            });//全部同意
            Init_WarApplyInfoScrollView();
        }
        public void Init_WarApplyInfoScrollView()
        {
            WarApplyInfoScrollView = ComponentFactory.Create<UICircularScrollView<WarMemberInfo>>();
            WarApplyInfoScrollView.ItemInfoCallBack = InitWarApplyCallBack;
            WarApplyInfoScrollView.InitInfo(E_Direction.Vertical, 1, 0,10);
            WarApplyInfoScrollView.IninContent(applyContent.gameObject, applyScrolView);
        }

        private void InitWarApplyCallBack(GameObject go, WarMemberInfo info)
        {
            CurChooseWarApplyInfo = info;
            go.transform.Find("Name").GetComponent<Text>().text = info.Name;
            go.transform.Find("Lev").GetComponent<Text>().text = info.Lev.ToString();
            List<long> gameIDs = new List<long>() { info.UUID };
            
            //拒绝 
            go.transform.Find("RefusedBtn").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                 WarAllianceAgreeOrReject(gameIDs, 0).Coroutine();

            });
            //同意
            go.transform.Find("AgreeBtn").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
               
                 WarAllianceAgreeOrReject(gameIDs, 1).Coroutine();
            });

        }
    }
}