using ETModel;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace ETHotfix
{
    public partial class UISelectServerComponent
    {

        private ReferenceCollector AnnouncCollector;//¹«¸æÃæ°å
        public UICircularScrollView<AccouncInfo> uICircular_Anncounc;
        public List<AccouncInfo> accouncInfos = new List<AccouncInfo>();
        public GameObject AnncouncContent;
        public ScrollRect View;
        public void AnnouncInit()
        {
            AnnouncCollector = collector.GetImage("AnnouncPanel").gameObject.GetReferenceCollector();
            AnnouncCollector.GetButton("Close").onClick.AddSingleListener(() =>
            {
                AnnouncCollector.gameObject.SetActive(false);
            });
            AnnouncCollector.gameObject.SetActive(false);
            AnncouncContent = AnnouncCollector.GetGameObject("AnncouncContent");
            View = AnnouncCollector.GetImage("AnncouncScrollView").GetComponent<ScrollRect>();
            var Config_Cnnoun = ConfigComponent.Instance.GetAll<Announc_InfoConfig>();

            accouncInfos.Clear();
            foreach (Announc_InfoConfig announc_Info in Config_Cnnoun.Cast<Announc_InfoConfig>())
            {
                AccouncInfo accouncInfo = new AccouncInfo();
                accouncInfo.Id = announc_Info.Id;
                accouncInfo.Type = announc_Info.AnnounType;
                accouncInfo.Name = announc_Info.AnnounName;
                accouncInfo.Content = announc_Info.AnnounContent;
                accouncInfos.Add(accouncInfo);
            }
            ChangeContent(null,accouncInfos.First());
            InitUICircular_Pet();
        }
        public void InitUICircular_Pet()
        {
            uICircular_Anncounc = ComponentFactory.Create<UICircularScrollView<AccouncInfo>>();
            uICircular_Anncounc.InitInfo(E_Direction.Vertical, 1, 0, 10);
            uICircular_Anncounc.ItemInfoCallBack = InitAnncouncItem;
            uICircular_Anncounc.ItemClickCallBack = ChangeContent;
            uICircular_Anncounc.IninContent(AnncouncContent, View);
            uICircular_Anncounc.Items = accouncInfos;
        }

        private void InitAnncouncItem(GameObject item, AccouncInfo announc_Info)
        {
            item.transform.Find("ActiveImage").gameObject.SetActive(announc_Info.Type == 2);
            item.transform.Find("AnncouncImage").gameObject.SetActive(announc_Info.Type == 1);
            item.transform.Find("Name").GetComponent<Text>().text = announc_Info.Name;
        }
        public void ChangeContent(GameObject item, AccouncInfo announc_Info)
        {
            AnnouncCollector.GetText("AnncouncContentTxt").text = announc_Info.Content;
            TimerComponent.Instance.RegisterTimeCallBack(100,
                () =>
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(AnnouncCollector.GetText("AnncouncContentTxt").transform.parent.GetComponentInParent<RectTransform>());
                }
            );
           
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
            uICircular_Anncounc.Dispose();

        }
    }

}
