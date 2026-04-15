using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 藏宝图开启 通知队友
    /// </summary>
    [MessageHandler]
    public class G2C_CangBaoTuOpen_notice_Handler : AMHandler<G2C_CangBaoTuOpen_notice>
    {
        protected override void Run(ETModel.Session session, G2C_CangBaoTuOpen_notice message)
        {
            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
            uIConfirm.SetTipText($"<color=red>{message.PlayerName}</color>已开启藏宝图，是否进入？");
            Timer time = TimerComponent.Instance.RegisterTimeCallBack(10000, () => { UIComponent.Instance.Remove(UIType.UIConfirm); });
            uIConfirm.AddActionEvent(async () => 
            {
                G2C_EnterBattleCopyResponse g2C_EnterBattle = (G2C_EnterBattleCopyResponse)await SessionComponent.Instance.Session.Call(new C2G_EnterBattleCopyRequest
                {
                    Level = 0,
                    Type = 4,
                    NPCID = message.NpcID,
                    PlayerID=message.PlayerID
                });
                if (g2C_EnterBattle.Error != 0)
                {
                    TimerComponent.Instance.RemoveTimer(time.Id);
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_EnterBattle.Error.GetTipInfo());
                }
                else
                {
                 
                }
            });
           
        }
    }
}
