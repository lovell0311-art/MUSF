using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;
using UnityEditor;
using System.Runtime.InteropServices;

namespace ETHotfix
{

    /// <summary>
    /// 附近的玩家
    /// </summary>
    public partial class UITeamComponent
    {

        public ScrollRect NearPlayersScrollrect;
        public GameObject NearPlayersContent;
        List<OtherPlayerInfo> NearOtherPlayersList;
        UICircularScrollView<OtherPlayerInfo> NearPlayersScrollView;
        public void Init_NearPlayer() 
        {
            ReferenceCollector collector = NearPlayerPanel.GetReferenceCollector();
            NearPlayersScrollrect = collector.GetImage("NearOtherPlayerScrollView").GetComponent<ScrollRect>();
            NearPlayersScrollrect.gameObject.SetActive(true);
            NearPlayersContent = collector.GetGameObject("Content");
            NearOtherPlayersList = new List<OtherPlayerInfo>();
            Init_NearPlayersScrollView();
           

            void Init_NearPlayersScrollView()
            {
                NearPlayersScrollView = ComponentFactory.Create<UICircularScrollView<OtherPlayerInfo>>();
                NearPlayersScrollView.ItemInfoCallBack = InitNearOtherPlayerCallBack;
                NearPlayersScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
                NearPlayersScrollView.IninContent(NearPlayersContent, NearPlayersScrollrect);
               // Log.DebugBrown($"NearPlayersScrollrect.gameObject.activeSelf:{NearPlayersScrollrect.gameObject.activeSelf}");
            }

            void InitNearOtherPlayerCallBack(GameObject go, OtherPlayerInfo info)
            {
                if (info == null) return;
               // Log.DebugBrown($"{info.roleName}");
                go.transform.Find("Name").GetComponent<Text>().text = info.roleName;
                go.transform.Find("Lev").GetComponent<Text>().text = "Lv."+info.roleLev.ToString();
                go.transform.Find("Post").GetComponent<Text>().text = ((E_RoleType)info.roleType).GetRoleName(info.OccupationLevel);
                go.transform.Find("War").GetComponent<Text>().text = String.IsNullOrEmpty(info.warName)?"暂未加入战盟": info.warName;
                Button button = go.transform.Find("InviteBtn").GetComponent<Button>();

                button.gameObject.SetActive((TeamDatas.MyTeamState!=null&&TeamDatas.MyTeamState.IsCaptain)||TeamDatas.MyTeamState==null);//只有队长才能邀请 队员

                button.interactable = !info.isInvite;
                button.transform.Find("Text").GetComponent<Text>().text = info.isInvite? "已邀请":"邀请";
                button.onClick.AddSingleListener(() => 
                {
                    InvitePlayerEnterTeam().Coroutine();
                });

                ///邀请其他玩家 入伍
                async ETVoid InvitePlayerEnterTeam()
                {
                    G2C_InvitePlayerEnterTeam g2C_InvitePlayer = (G2C_InvitePlayerEnterTeam)await SessionComponent.Instance.Session.Call(new C2G_InvitePlayerEnterTeam
                    {
                        PlayerGameUserId = info.roleUUId//被邀请玩家的UUID
                    });
                    if (g2C_InvitePlayer.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_InvitePlayer.Error.GetTipInfo());
                      //  Log.DebugBrown($"{g2C_InvitePlayer.Message}");
                    }
                    else
                    {
                        // 邀请结果推送至G2C_InvitePlayerEnterTeam_notice
                        button.transform.Find("Text").GetComponent<Text>().text = "已邀请";
                        info.isInvite = true;
                    }
                }
            }

        }
        /// <summary>
        /// 获取 附近的玩家
        /// </summary>
        public void InitNearPlayers()
        {
            GetNearPlayer().Coroutine();

            async ETVoid GetNearPlayer() 
            {
                G2C_GetNearbyPlayerList g2C_GetNearbyPlayerList = (G2C_GetNearbyPlayerList)await SessionComponent.Instance.Session.Call(new C2G_GetNearbyPlayerList { }) ;
                if (g2C_GetNearbyPlayerList.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetNearbyPlayerList.Error.GetTipInfo());
                }
                else
                {
                    NearOtherPlayersList.Clear();
                    for (int i = 0, length= g2C_GetNearbyPlayerList.PlayerList.Count; i < length; i++)
                    {
                       
                        var item = g2C_GetNearbyPlayerList.PlayerList[i];
                        if (item.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID) continue;//不显示自己
                      //  Log.DebugBrown($"附近的玩家：{item.Name}");
                        NearOtherPlayersList.Add(new OtherPlayerInfo
                        {
                            roleUUId = item.GameUserId,
                            roleName = item.Name,
                            roleLev = item.Level,
                            roleType =item.PlayerTypeId,
                            warName = item.WarAllianceName,
                            OccupationLevel=item.OccupationLevel,
                            TeamId=item.TeamId,
                            isInvite = false
                        });
                    }
                    //按照等级降序
                    NearOtherPlayersList.Sort((m1, m2) =>
                    {
                        return m2.roleLev.CompareTo(m1.roleLev);
                    });

                    NearPlayersScrollView.Items = NearOtherPlayersList;
                }
            }
           
        }


    }
}
