using ETModel;

namespace ETHotfix
{

    /// <summary>
    /// 交易邀请 通知
    /// </summary>
    [MessageHandler]
    public class G2C_InvitePlayerExchange_notice_Handler : AMHandler<G2C_InvitePlayerExchange_notice>
    {
        protected override void Run(ETModel.Session session, G2C_InvitePlayerExchange_notice message)
        {
            ///十秒后 自动取消交易
            Timer timer = TimerComponent.Instance.RegisterTimeCallBack(10000, () => 
            {
                ReplyPlayerExchangeInvite(false).Coroutine();
                UIComponent.Instance.Remove(UIType.UIConfirm);

            });

            //正在合成或者镶嵌无法交易
            if (UIKnapsackComponent.Instance != null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,$"{message.PlayerName} 请求与你交易");
                //取消交易
                ReplyPlayerExchangeInvite(false).Coroutine();
                return;
            }

            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
            uIConfirm.SetTipText($"<color=red>{message.PlayerName}</color> 请求交易\n是否同意？\n\n\n<color=red>同意后会关闭打开的其他界面</color>");
            uIConfirm.AddActionEvent( () => 
            {
                if (UnitEntityComponent.Instance.LocalRole.MaxMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "没有赞助卡禁止面对面交易");
                    return;
                }
                //确认交易
                ReplyPlayerExchangeInvite(true).Coroutine();
                TimerComponent.Instance.RemoveTimer(timer.Id);
            });
            uIConfirm.AddCancelEventAction(() => 
            {
                //取消交易
                ReplyPlayerExchangeInvite(false).Coroutine();
            });
            //同意或拒绝玩家申请  返回成功则开始交易，打开交易面板
            async ETVoid ReplyPlayerExchangeInvite(bool isAgree)
            {
                G2C_ReplyPlayerExchangeInvite g2C_ReplyPlayer = (G2C_ReplyPlayerExchangeInvite)await SessionComponent.Instance.Session.Call(new C2G_ReplyPlayerExchangeInvite
                {
                    PlayerGameUserId = message.PlayerGameUserId,
                    IsAgree =isAgree
                });
                if (g2C_ReplyPlayer.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReplyPlayer.Error.GetTipInfo());
                   
                }
                else
                {
                    if (isAgree)
                    {
                       
                        if (UIComponent.Instance.Get(UIType.UIKnapsack) != null) 
                        {
                            UIKnapsackComponent.Instance.CloseKnapsack();
                        }
                        UIComponent.Instance.Remove(UIType.UISelectOtherPlayer);
                        UIComponent.Instance.RemoveAllNew();
                        UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Trade);
                        UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>().SetOtherInfo(g2C_ReplyPlayer.PlayerName, g2C_ReplyPlayer.PlayerWarAllianceName, g2C_ReplyPlayer.PlayerLevel);
                    }
                }
            }
        }
    }
}
