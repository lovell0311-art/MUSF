using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;

namespace ETHotfix
{
    public partial class UIFriendListComponent : Component
    {
        /// <summary>
        /// 滑动框
        /// </summary>
        public ScrollRect ApplyView;
        /// <summary>
        /// 滑动框容器
        /// </summary>
        public GameObject ApplyContent;
        public UICircularScrollView<FriendInfo> applyUICircularScroll;
        public void ApplyAwake()
        {
            applyReferenceCollector = addfriendReferenceCollector.GetImage("FriendApply").gameObject.GetReferenceCollector();
            ApplyView = applyReferenceCollector.GetImage("ApplyView").gameObject.GetComponent<ScrollRect>();
            ApplyContent = applyReferenceCollector.GetGameObject("ApplyContent");

            
            ApplyUICircular();
        }

        public void ApplyInit()
        {
            RequestFriendList(3).Coroutine();
            //rps.SetInvoke(RedPointConst.friendMainAddFriendFriendApply, 0);
            //InitApplyList();
            applyUICircularScroll.Items = FriendListData.ApplyList;
            RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Friend_AddFirend_FirendApply, RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Friend_AddFirend_FirendApply) - 1);
            RedDotFriendCheack();
        }

        public void ApplyUICircular()
        {
            applyUICircularScroll = ComponentFactory.Create<UICircularScrollView<FriendInfo>>();
            applyUICircularScroll.InitInfo(E_Direction.Vertical,1,0,20);
            applyUICircularScroll.IninContent(ApplyContent, ApplyView);
            applyUICircularScroll.ItemInfoCallBack = InitApplyItem;
            applyUICircularScroll.Items = FriendListData.ApplyList;
        }

        private void InitApplyItem(GameObject item, FriendInfo friendinfo)
        {
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(-835, 150);
            item.transform.Find("NickName").GetComponent<Text>().text = friendinfo.NickName;
            item.transform.Find("Level").GetComponent<Text>().text = "Level:" + friendinfo.Level.ToString();
            item.transform.Find("RejectBtn").GetComponent<Button>().onClick.AddSingleListener(delegate
            {
                Reject_Firend(0, friendinfo);
            });
            item.transform.Find("AgreeBtn").GetComponent<Button>().onClick.AddSingleListener(delegate
            {
                Reject_Firend(1, friendinfo);
            });
        }

        public void Reject_Firend(int type, FriendInfo friendinfo)
        {
            RejectAsynv(type, friendinfo).Coroutine();
        }
        public async ETTask RejectAsynv(int type, FriendInfo friendinfo)
        {
            Debug.Log($"{type}同意或者拒绝好友申请!!!");
            G2C_AgreeToAddFriendResponse g2C_ = (G2C_AgreeToAddFriendResponse)await SessionComponent.Instance.Session.Call(new C2G_AgreeToAddFriendRequest
            {
                Type = type,
                GameUserId = friendinfo.UUID,
                CharName = friendinfo.NickName
            });
            if (g2C_.Error != 0)
            {
                switch (type)
                {
                    case 0:
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"拒绝同意好友失败{g2C_.Error.GetTipInfo()}");
                        break;
                    case 1:
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"同意好友失败{g2C_.Error.GetTipInfo()}");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case 0:
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"拒绝玩家[{friendinfo.NickName}]的好友申请");
                        break;
                    case 1:
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"同意玩家[{friendinfo.NickName}]的好友申请");
                        if(Cur_E_Friends == E_FriendsTogNewType.Friend)
                        {
                            RequestFriendList(4).Coroutine();
                        }
                        break;
                    default:
                        break;
                }
                RequestFriendList(3).Coroutine();
            }
        }
    }

}
