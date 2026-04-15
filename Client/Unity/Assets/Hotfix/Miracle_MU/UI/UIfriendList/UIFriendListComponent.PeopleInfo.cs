using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIFriendListComponent : Component
    {
        public Text NicName,Teams,Level,Job;
        public Button JoinTeam, ApplyJoin, DelectFriend, BlackFriend;
        public void PeopleInfoAwake()
        {
            peopleReferenceCollector = all_ReferenceCollector.GetGameObject("PeopleInfoPanel").GetReferenceCollector();
            peopleReferenceCollector.GetButton("CloseBG").onClick.AddSingleListener(delegate
            {
                peopleReferenceCollector.gameObject.SetActive(false);
            });
            NicName = peopleReferenceCollector.GetText("NicName");
            Teams = peopleReferenceCollector.GetText("Teams");
            Level = peopleReferenceCollector.GetText("Level");
            Job = peopleReferenceCollector.GetText("Job");
            JoinTeam = peopleReferenceCollector.GetButton("JoinTeam");
            ApplyJoin = peopleReferenceCollector.GetButton("ApplyJoin");
            DelectFriend = peopleReferenceCollector.GetButton("DelectFriend");
            BlackFriend = peopleReferenceCollector.GetButton("BlackFriend");
        }
        public void SetPeopleInfo(FriendInfo friendInfo)
        {
            Log.DebugBrown($"SetPeopleInfo：查看->{friendInfo.NickName},职业：{friendInfo.Job}");
            NicName.text = friendInfo.NickName;//名字
            Level.text = "等级" + friendInfo.Level.ToString();//等级
            Job.text = friendInfo.Job;//职业
            Teams.text = !string.IsNullOrEmpty(friendInfo.Zhanmeng)? friendInfo.Zhanmeng + "    " + friendInfo.Identity:string.Empty;
            JoinTeam.gameObject.SetActive(false);
            ApplyJoin.gameObject.SetActive(false);
            //加入队伍监听
            JoinTeam.onClick.AddSingleListener(delegate
            {
                if (nowFriendInfo.State != "在线")
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"好友[{nowFriendInfo.NickName}]不在线");
                    return;
                }
                if (string.IsNullOrEmpty(friendInfo.Teams))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "没有加入队伍");
                }
                Log.DebugGreen($"申请加入{friendInfo.NickName}队伍");
            });
            //加入战盟监听
            ApplyJoin.onClick.AddSingleListener(delegate
            {
                if (nowFriendInfo.State != "在线")
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"好友[{nowFriendInfo.NickName}]不在线");
                    return;
                }
                if (string.IsNullOrEmpty(friendInfo.Zhanmeng))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "对方还未加入战盟");
                }
                Log.DebugGreen($"申请入盟{friendInfo.NickName}  friendInfo.Zhanmeng:{friendInfo.Zhanmeng}");
               // UIComponent.Instance.Get(UIType.UIWarAlliance).GetComponent<UIWarAllianceComponent>().RequestAddWarAsync(1).Coroutine();
                RequestAlliance().Coroutine();


                async ETVoid RequestAlliance() 
                {
                    G2C_AddWarAllianceResponse g2C_AddWar = (G2C_AddWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_AddWarAllianceRequest
                    {
                        WarAllianceID = 1
                    });
                    if (g2C_AddWar.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddWar.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "已申请");
                    }
                }
            });
            //删除好友监听
            DelectFriend.onClick.AddSingleListener(delegate
            {
                switch (Cur_E_Friends)
                {
                    case E_FriendsTogNewType.Black:
                        RequestDelectFriend(0, 1, friendInfo).Coroutine();
                        break;
                    case E_FriendsTogNewType.Friend:
                        RequestDelectFriend(0, 4, friendInfo).Coroutine();
                        break;
                    default:
                        break;
                }
            });
            //拉黑按钮监听
            BlackFriend.onClick.AddSingleListener(delegate
            {
                switch (Cur_E_Friends)
                {
                    case E_FriendsTogNewType.Black:
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"玩家[{friendInfo.NickName}]已经在黑名单中了");
                        break;
                    case E_FriendsTogNewType.Friend:
                        RequestDelectFriend(1, 4, friendInfo).Coroutine();
                        break;
                    default:
                        break;
                }
            });
        }
        public async ETTask RequestDelectFriend(int type, int listType, FriendInfo friendInfo)
        {
            try
            {
                G2C_DeleteOrBlockFriendResponse g2C_Delete = (G2C_DeleteOrBlockFriendResponse)await SessionComponent.Instance.Session.Call(new C2G_DeleteOrBlockFriendRequest()
                {
                    GameUserId = friendInfo.UUID,//好友ID
                    ListType = listType,//好友列表
                    Type = type//0拉黑,1删除
                });
                if (g2C_Delete.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Delete.Error.GetTipInfo());
                }
                else
                { 
                    switch (type)
                    {
                        case 0:
                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"删除好友[{friendInfo.NickName}]成功");
                            break;
                        case 1:
                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"拉黑好友[{friendInfo.NickName}]成功");
                            break;
                        default:
                            break;
                    }
                    //操作后,隐藏当前面板
                    peopleReferenceCollector.gameObject.SetActive(false);
                    //重新请求当前列表
                    RequestFriendList(listType).Coroutine();
                }
            }
            catch (System.Exception e)
            {
                Log.DebugRed(e.ToString());
            }
            
        }
    }

}
