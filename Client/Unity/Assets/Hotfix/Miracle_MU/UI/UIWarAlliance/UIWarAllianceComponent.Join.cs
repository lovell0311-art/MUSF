using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;
using System.Linq;

namespace ETHotfix
{

    /// <summary>
    /// 战盟结构 类
    /// </summary>
    public class WarInfo
    {
        public Struct_WarAllinceInfo struct_WarAllince;//战盟信息 结构体
        public int State;//状态  0 可以申请 1审核中
    }

    /// <summary>
    /// 加入战盟 模块
    /// </summary>
    public partial class UIWarAllianceComponent
    {
        public InputField InputField_WarName;

        ReferenceCollector collector_Join;
        public List<WarInfo> SearchWarInfoList;
        public ScrollRect JoinScrolView;
        Transform JoinContent;
        UICircularScrollView<WarInfo> WarJoinInfoScrollView;
        Button UpPageBtn, DownPageBtn;
        public void Init_Join()
        {
            SearchWarInfoList=new List<WarInfo>();
            collector_Join = JoinPanel.GetReferenceCollector();
            InputField_WarName = collector_Join.GetInputField("Search_InputField");
            JoinScrolView = collector_Join.GetImage("WarScrollView").GetComponent<ScrollRect>();
            JoinContent = collector_Join.GetGameObject("Content").transform;
            UpPageBtn = collector_Join.GetButton("UpPageBtn");
            DownPageBtn = collector_Join.GetButton("DownPageBtn");
            InputField_WarName.onValueChanged.AddSingleListener((value) =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    WarJoinInfoScrollView.Items = WarAllianceDatas.WarLists;
                    return;
                }
                if (SystemUtil.IsInvaild(value))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "昵称包含非法字符 请您重新输入");
                    InputField_WarName.text = string.Empty;
                    return;
                }
            });
            UpPageBtn.onClick.AddSingleListener(() =>
            {
                if (WarAllianceDatas.WarLists.Count >= 1)
                {
                    JoinWarAsync(0).Coroutine();
                }
            });
            DownPageBtn.onClick.AddSingleListener(() =>
            {
                if (WarAllianceDatas.WarLists.Count >= 10)
                {
                    JoinWarAsync(1).Coroutine();
                }
            });
            ///搜索战盟
            collector_Join.GetButton("SearchBtn").onClick.AddSingleListener(() =>
            {
                //搜索战盟
                SearchWar(InputField_WarName.text).Coroutine();

            });
            Init_WarJoinInfoScrollView();

            
        }
        public void Init_WarJoinInfoScrollView()
        {
            WarJoinInfoScrollView = ComponentFactory.Create<UICircularScrollView<WarInfo>>();
            WarJoinInfoScrollView.ItemInfoCallBack = InitWarJoinCallBack;
            WarJoinInfoScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
            WarJoinInfoScrollView.IninContent(JoinContent.gameObject, JoinScrolView);
        }

        private void InitWarJoinCallBack(GameObject Item, WarInfo info)
        {
            Item.transform.Find("name").GetComponent<Text>().text = info.struct_WarAllince.WarAllianceName;//战盟 名字
           
            Item.transform.Find("leader").GetComponent<Text>().text = info.struct_WarAllince.LeaderName;//战盟 盟主
            Item.transform.Find("count").GetComponent<Text>().text = info.struct_WarAllince.WarAllianceLevel.ToString();//战盟 成员数量
            Button button = Item.transform.Find("state").GetComponent<Button>();//是由已经申请
            //button.transform.Find("Text").GetComponent<Text>().text = info.State == 0 ? "申请" : "审核中";
            //button.interactable = info.State == 0;
            button.onClick.AddSingleListener(() =>
            {

                Init_LookInfo(info);
                /*
                RequestAddWarAsync(info.struct_WarAllince.WarAllianceID).Coroutine(); 

                async ETVoid RequestAddWarAsync(long warUUID)
                {
                    G2C_AddWarAllianceResponse g2C_AddWar = (G2C_AddWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_AddWarAllianceRequest
                    {
                        WarAllianceID = warUUID
                    });
                    if (g2C_AddWar.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddWar.Error.GetTipInfo());
                    }
                    else
                    {
                        //UIComponent.Instance.VisibleUI(UIType.UIHint, "已申请");
                        //button.transform.Find("Text").GetComponent<Text>().text = "审核中";
                        //button.interactable = false;
                        //info.State = 1;
                    }
                }
                */
            });

        }
    }
}