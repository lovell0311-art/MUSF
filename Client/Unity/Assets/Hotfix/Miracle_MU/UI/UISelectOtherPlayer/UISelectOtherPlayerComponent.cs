using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Reflection;

namespace ETHotfix
{
    [ObjectSystem]
    public class UISelectOtherPlayerComponentAwake : AwakeSystem<UISelectOtherPlayerComponent>
    {
        public override void Awake(UISelectOtherPlayerComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("Panel").onClick.AddSingleListener(() =>
            {
                self.ClearViewRoleEquips();
                UIComponent.Instance.Remove(UIType.UISelectOtherPlayer);
            });
            self.collector.GetButton("InviteBtn").onClick.AddSingleListener(() => self.InviteTeam().Coroutine());
            self.collector.GetButton("AddFriendBtn").onClick.AddSingleListener(() => self.AddFriend().Coroutine());
            self.collector.GetButton("ToViewRoleBtn").onClick.AddSingleListener(() => self.ToViewRoleEquips());
            ///请求交易
            self.collector.GetButton("TradeBtn").onClick.AddSingleListener(async () =>
            {
                //if(self.roleEntity.MaxMonthluCardTimeSpan.TotalSeconds <= 0 && !TitleManager.allTitles.Exists(x => x.TitleId == 60005))
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, "没有特权卡或主宰无双称号无法交易！");
                //}
                if (UnitEntityComponent.Instance.LocalRole.MaxMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "没有赞助卡禁止面对面交易");
                    return;
                }
                G2C_InvitePlayerExchange g2C_InvitePlayerExchange = (G2C_InvitePlayerExchange)await SessionComponent.Instance.Session.Call(new C2G_InvitePlayerExchange
                {
                    PlayerGameUserId = self.SelectroleEntity.Id,
                });
                if (g2C_InvitePlayerExchange.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_InvitePlayerExchange.Error);
                    Log.DebugBrown($"失败：{g2C_InvitePlayerExchange.Error}");
                }
                else
                {
                    //邀请结果推送G2C_InvitePlayerExchange_notice
                    UIComponent.Instance.Remove(UIType.UISelectOtherPlayer);
                }
            });

            self.Init_();

            self.SetCurSelectRole(null);

        }
    }
    public partial class UISelectOtherPlayerComponent : Component
    {
        public ReferenceCollector collector;

        public RoleEntity SelectroleEntity;

        public RoleEntity roleEntity => UnitEntityComponent.Instance.LocalRole;//本地玩家
        public void SetCurSelectRole(RoleEntity role) => SelectroleEntity = role;

        public async ETVoid InviteTeam()
        {
            //Log.DebugBrown($"邀请  {SelectroleEntity.Id} 玩家组队");
            ///邀请其他玩家 入伍

            G2C_InvitePlayerEnterTeam g2C_InvitePlayer = (G2C_InvitePlayerEnterTeam)await SessionComponent.Instance.Session.Call(new C2G_InvitePlayerEnterTeam
            {
                PlayerGameUserId = SelectroleEntity.Id//被邀请玩家的UUID
            });
            if (g2C_InvitePlayer.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_InvitePlayer.Error.GetTipInfo());
              //  Log.DebugBrown($"{g2C_InvitePlayer.Message}");
            }
            else
            {
                // 邀请结果推送至G2C_InvitePlayerEnterTeam_notice

            }

        }
        /// <summary>
        /// 添加好友
        /// </summary>
        /// <returns></returns>
        public async ETVoid AddFriend()
        {
           // Log.DebugBrown($"添加  {SelectroleEntity.Id} 好友");
            G2C_AddFriendsResponse g2C_ = (G2C_AddFriendsResponse)await SessionComponent.Instance.Session.Call(new C2G_AddFriendsRequest()
            {
                GameUserId = SelectroleEntity.Id,
                CharName = SelectroleEntity.RoleName
            });
            if (g2C_.Error != 0)
            {
              
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_.Error.GetTipInfo()}");
            }
            else
            {
               
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"好友申请发送成功");
            }
        }


        public void GivingItem()
        {
           // Log.DebugBrown($"打开赠送 界面");
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            ClickSelectUnitEntityComponent.Instance.ClearSelectUnit();
            ClearViewRoleEquips();
        }

    }
}