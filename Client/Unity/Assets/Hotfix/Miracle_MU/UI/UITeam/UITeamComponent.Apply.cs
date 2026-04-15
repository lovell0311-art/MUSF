using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 队伍 申请 列表
    /// </summary>
    public partial class UITeamComponent
    {

        public ScrollRect ApplyPlayersScrollrect;
        public GameObject ApplyPlayersContent;
        
        public UICircularScrollView<OtherPlayerInfo> ApplyPlayersScrollView;
        public void Init_Apply() 
        {
            ReferenceCollector collector = ApplyPanel.GetReferenceCollector();
            ApplyPlayersScrollrect = collector.GetImage("ApplyScrollView").GetComponent<ScrollRect>();
            ApplyPlayersContent = collector.GetGameObject("Content");
            Init_ApplyPlayersScrollView();

            void Init_ApplyPlayersScrollView()
            {
                ApplyPlayersScrollView = ComponentFactory.Create<UICircularScrollView<OtherPlayerInfo>>();
                ApplyPlayersScrollView.ItemInfoCallBack = InitNearOtherPlayerCallBack;
                ApplyPlayersScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
                ApplyPlayersScrollView.IninContent(ApplyPlayersContent, ApplyPlayersScrollrect);
            }

            void InitNearOtherPlayerCallBack(GameObject go, OtherPlayerInfo info) 
            {
                go.transform.Find("Name").GetComponent<Text>().text = info.roleName;
                go.transform.Find("Lev").GetComponent<Text>().text = "Lv." + info.roleLev.ToString();
                go.transform.Find("Post").GetComponent<Text>().text = ((E_RoleType)info.roleType).GetRoleName(info.OccupationLevel);
                go.transform.Find("Invite").GetComponent<Text>().text = "请求加入队伍";
                ///拒绝 玩家的入伍申请
                go.transform.Find("RefusedBtn").GetComponent<Button>().onClick.AddSingleListener(() => 
                {
                    ReplyPlayerApply(false).Coroutine();
                });
                ///同意玩家的入伍申请
                go.transform.Find("AgreeBtn").GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    ReplyPlayerApply(true).Coroutine();
                });


                /// 同意 或拒绝 玩家的 入伍申请
                async ETVoid ReplyPlayerApply(bool isAgree)
                {
                    G2C_ReplyPlayerApply g2C_Reply = (G2C_ReplyPlayerApply)await SessionComponent.Instance.Session.Call(new C2G_ReplyPlayerApply
                    {
                        PlayerGameUserId=info.roleUUId,
                        IsAgree=isAgree,
                        IsAuto=false
                    });
                    if (g2C_Reply.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Reply.Error.GetTipInfo());
                    }
                    else
                    {
                        /// 从好友 申请 列表中 移除 该玩家
                        if (TeamDatas.ApplyPlayersList.Exists(r => r.roleUUId == info.roleUUId)) 
                        {
                            var member = TeamDatas.ApplyPlayersList.Find(r => r.roleUUId == info.roleUUId);
                            TeamDatas.ApplyPlayersList.Remove(member);
                        }
                        ApplyPlayersScrollView.Items = TeamDatas.ApplyPlayersList;
                    }
                }
            }


            
        }

        /// <summary>
        /// 显示 申请 列表
        /// </summary>
        public void ShowApplyList() 
        {
            ApplyPlayersScrollView.Items = TeamDatas.ApplyPlayersList;
        }
    }
}
