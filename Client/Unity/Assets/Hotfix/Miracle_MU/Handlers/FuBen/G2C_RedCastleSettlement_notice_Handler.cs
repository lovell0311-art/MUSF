using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_RedCastleSettlement_notice_Handler : AMHandler<G2C_RedCastleSettlement_notice>
    {
        protected override void Run(ETModel.Session session, G2C_RedCastleSettlement_notice message)
        {
            //发放排行榜默认归还了大天使
            UIMainComponent.Instance.weaponIsReturn = true;
            UIMainComponent.Instance.StartFubenCountDown(false,"血色城堡", 0, false);
            //UIComponent.Instance.VisibleUI(UIType.UIFuBenPaiHangBang);
            //UIComponent.Instance.Get(UIType.UIFuBenPaiHangBang).GetComponent<UIFuBenPaiHangBangComponent>().SetValue(message);
            TimerComponent.Instance.RegisterTimeCallBack(4000, () =>
            {

                UIConfirmComponent confirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent(E_TipPanelType.FuBenReward);
                confirmComponent.fuBenTitle.text = message.State == 0?"任务失败！请下次再来！": "祝贺勇士们圆满完成血色城堡任务！";
                confirmComponent.ExpTxt.text = $"奖励经验值：{message.BatteCopyRankDatas[0].EXP}";
                confirmComponent.CoinTxt.text = $"奖励金币：{message.BatteCopyRankDatas[0].Coin}";
                UIMainComponent.Instance.ExitFuBenBtn.gameObject.SetActive(false);
            });
            
        }
    }


}
