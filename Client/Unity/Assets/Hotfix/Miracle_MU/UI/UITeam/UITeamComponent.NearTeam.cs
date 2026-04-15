using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;

namespace ETHotfix
{

    /// <summary>
    /// 附近 的队伍
    /// </summary>
    public partial class UITeamComponent
    {

        public ScrollRect NearTeamScrollrect;
        public GameObject NearTeamContent;

        UICircularScrollView<NearTeamInfo> NearTeamScrollView;
        public  void Init_NearTeam()
        {
            ReferenceCollector collector = NearTeamPanel.GetReferenceCollector();
            NearTeamScrollrect = collector.GetImage("NearTeamScrollView").GetComponent<ScrollRect>();
            NearTeamContent = collector.GetGameObject("Content");
            Init_WarApplyInfoScrollView();


            void Init_WarApplyInfoScrollView()
            {
                NearTeamScrollView = ComponentFactory.Create<UICircularScrollView<NearTeamInfo>>();
                NearTeamScrollView.ItemInfoCallBack = InitNearTeamCallBack;
                NearTeamScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
                NearTeamScrollView.IninContent(NearTeamContent, NearTeamScrollrect);
            }
        }

        private void InitNearTeamCallBack(GameObject Item, NearTeamInfo Info)
        {
            Item.transform.Find("TeamName").GetComponent<Text>().text=Info.TeamName;
            Item.transform.Find("WarName").GetComponent<Text>().text="无";//TODO 战盟 需要提供一个 更具玩家的uuid 获取玩家战盟的接口
            Item.transform.Find("Count").GetComponent<Text>().text = Info.TeamMemberCount.ToString();
            Button button = Item.transform.Find("ApplyBtn").GetComponent<Button>();
            button.interactable = !Info.isInvite;
            button.transform.Find("Text").GetComponent<Text>().text = Info.isInvite?"已申请":"申请";
            button.onClick.AddSingleListener(() =>
            {
                if (TeamDatas.MyTeamState != null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"你已加入其他队伍 无法申请该队伍");
                    return;
                }
                ApplyToJoinTeam().Coroutine();
            });
            // 自己的队伍
            if (TeamDatas.MyTeamState != null)
            {
                //自己是队长
                if (TeamDatas.MyTeamState.IsCaptain && TeamDatas.MyTeamState.UserName == Info.TeamName)
                {
                    button.transform.Find("Text").GetComponent<Text>().text = "已加入";
                    button.interactable = false;
                }
                else
                {
                    //自己 不是队长 遍历队伍 找到队长 
                    foreach (var item in TeamDatas.OtherTeamMemberStatusList)
                    {
                        if (item.IsCaptain && item.UserName == Info.TeamName)
                        {
                            button.transform.Find("Text").GetComponent<Text>().text = "已加入";
                            button.interactable = false;
                        }
                    }
                }
            }

           

            /// <summary>
            /// 申请 加入队伍
            /// </summary>
            /// <param name="gameUserId">要进入的队伍玩家ID</param>
            /// <returns></returns>
            async ETVoid ApplyToJoinTeam()
            {
                G2C_ApplyToJoinTheTeam g2C_ApplyToJoin = (G2C_ApplyToJoinTheTeam)await SessionComponent.Instance.Session.Call(new C2G_ApplyToJoinTheTeam
                {
                    PlayerGameUserId = Info.uid
                });
                if (g2C_ApplyToJoin.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ApplyToJoin.Error.GetTipInfo());
                }
                else
                {
                    Info.isInvite = true;
                    button.transform.Find("Text").GetComponent<Text>().text = "已申请";
                    //结果推送至G2C_ApplyToJoinTheTeam_notice
                }
            }
        }
    }
}
