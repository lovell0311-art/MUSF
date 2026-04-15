using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        public GameObject SiegeWarfareNoticeObj;
        public Text SiegeWarfareinfotxt;
        public float SiegeWarfareidtTime = 0;
        public void Init_SiegeWarfareNotice()
        {
            SiegeWarfareNoticeObj = ReferenceCollector_Main.GetImage("SiegeWarfareNotice").gameObject;
            ReferenceCollector collector = SiegeWarfareNoticeObj.GetReferenceCollector();
            SiegeWarfareinfotxt = collector.GetText("infotxt");
            SiegeWarfareNoticeObj.SetActive(false);
        }
        public void ShowSiegeWarfareNotice(string info)
        {
            SiegeWarfareNoticeObj.SetActive(true);
            SiegeWarfareinfotxt.text = info;
            SiegeWarfareidtTime = 4;
        }
        public void HideText()
        {
            if (SiegeWarfareidtTime >= 0)
            {
                SiegeWarfareidtTime -= Time.deltaTime;
            }
            else
            {
                SiegeWarfareNoticeObj.SetActive(false);
            }
        }
    }

}
