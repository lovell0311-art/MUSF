using ETModel;
using System;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIConfirmComponent
    {
        //public Action RewardEventAction;//副本确定 回调函数
        public Text fuBenTitle, ExpTxt,CoinTxt;
        public void InitFuBenReward()
        {
            //if (FuBenRewardPanel == null)
            //{
            //    ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            //    FuBenRewardPanel = collector.GetImage("FuBenReward").gameObject;//副本奖励面板
            //}
            ReferenceCollector collectorFuBen = FuBenRewardPanel.GetReferenceCollector();
            fuBenTitle = collectorFuBen.GetText("fuBenTitle");
            ExpTxt = collectorFuBen.GetText("ExpTxt");
            CoinTxt = collectorFuBen.GetText("CoinTxt");
            collectorFuBen.GetButton("YesBtn").onClick.AddSingleListener(() =>
            {
                OnFuBenbYesbtnClick();
            });
        }
        private void OnFuBenbYesbtnClick()
        {
            //RewardEventAction?.Invoke();
            HidePanel();
        }
    }

}
