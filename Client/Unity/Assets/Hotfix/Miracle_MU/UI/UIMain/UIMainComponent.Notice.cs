using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 顶部 通知显示
    /// </summary>
    public partial class UIMainComponent
    {
        public GameObject NoticeObj;
        public Image infotxtBG, HornInfoTxtBG;
        public Text infotxt, HornInfoTxt;
        Queue<string> infoQueue = new Queue<string>();
        public bool isRollOver = true;
        public void Init_Notice() 
        {
            NoticeObj = ReferenceCollector_Main.GetGameObject("Notice");
            ReferenceCollector collector = NoticeObj.GetReferenceCollector();
            infotxtBG = collector.GetImage("infotxtBG");
            HornInfoTxtBG = collector.GetImage("HornInfoTxtBG");
            infotxt = infotxtBG.transform.Find("infotxt").GetComponent<Text>();
            HornInfoTxt = HornInfoTxtBG.transform.Find("HornInfoTxt").GetComponent<Text>();
            infotxtBG.gameObject.SetActive(false);
            HornInfoTxtBG.gameObject.SetActive(false);
        }

        public void ShowHornNotice(string info)
        {
            HornInfoTxt.rectTransform.anchoredPosition = new Vector2(NoticeObj.GetComponent<RectTransform>().rect.width, 0);
            HornInfoTxt.text = info;
            TextMove textMove = HornInfoTxt.GetComponent<TextMove>();
            textMove.RollOveraction = HornRollOver;
            textMove.speed = 50;
            NoticeObj.gameObject.SetActive(true);
            HornInfoTxtBG.gameObject.SetActive(true);
        }
        public void HornRollOver()
        {
            HornInfoTxtBG.gameObject.SetActive(false);
        }
        public void ShowNotice(string info)
        {
            infoQueue.Enqueue(info);
        }
        public void ShowNoticeInfo()
        {
            if (infoQueue.Count <= 0) return;
            if (isRollOver)
            {
                isRollOver = false;
                infotxt.text = infoQueue.Dequeue();
                TextMove textMove = infotxt.GetComponent<TextMove>();
                textMove.RollOveraction = RollOver;
                if (textMove == null)
                {
                    textMove.enabled = false;
                }
                textMove.speed = 50;
                NoticeObj.gameObject.SetActive(true);
                infotxtBG.gameObject.SetActive(true);
            }
        }
        //滚动完成
        public void RollOver()
        {
            isRollOver = true;
        }

    }
}
