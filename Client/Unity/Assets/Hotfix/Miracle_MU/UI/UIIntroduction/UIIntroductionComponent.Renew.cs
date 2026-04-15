using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIIntroductionComponent
    {
        public ReferenceCollector RenewCollector;
        public Text RenewTitle;
        public int Renewgold;
        public int Renewday;
        public string Renewname;
        public void Renew()
        {
            RenewCollector = collector.GetImage("RenewPanel").gameObject.GetReferenceCollector();
            RenewCollector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                ShowRenewPanel(false);
            });
            RenewTitle = RenewCollector.GetText("Title");
           
            RenewCollector.GetButton("SellBtn").onClick.AddSingleListener(() =>
            {

                VerticalRenewAction?.Invoke();
                ShowRenewPanel(false);
            });
            ShowRenewPanel(false);
        }
        public void SetRenewData(int renewgold = 0, int renewday = 0,string name = "")
        {
            Renewgold = renewgold;
            Renewday = renewday;
            Renewname = name;
        }
        public void ShowRenewPanel(bool show)
        {
            if (show)
            {
                RenewTitle.text = $"是否花费{Renewgold}魔晶续费{Renewname}{Renewday}天";
            }
            RenewCollector.gameObject.SetActive(show);
        }
    }

}
