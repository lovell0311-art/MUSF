using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 显示中奖物品
    /// </summary>
    public partial class UIChouJiangComponent
    {
        public GameObject ShowPrizepanel;
        public ReferenceCollector referenceCollector;
        public Transform prizes;
        public  GameObject tips;

        public void Init_Show() 
        {
            ShowPrizepanel = reference.GetImage("DrawPrizes").gameObject;
            referenceCollector=ShowPrizepanel.GetComponent<ReferenceCollector>();
            prizes = referenceCollector.GetGameObject("prizes").transform;
            tips = referenceCollector.GetText("tips").gameObject;
            tips.SetActive(false);
            referenceCollector.GetButton("closebtn").onClick.AddSingleListener(() => 
            {
                ShowPrizepanel.SetActive(false);
            });
            ShowPrizepanel.SetActive(false);
        }

        public void Show_Prizes() 
        {
            ShowPrizepanel.SetActive(true) ;
            for (int i = PrizeList.Count; i < prizes.childCount; i++)
            {
                prizes.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < PrizeList.Count; i++)
            {
                var id = PrizeList[i];
                if (PrizeDic.TryGetValue(id, out Prize prize))
                {
                    Log.DebugGreen($"奖品ID:{id}");
                    var trs = prizes.GetChild(i).gameObject;
                    trs.transform.Find("icon").GetComponent<Image>().sprite = spriteAtlas.GetSprite(prize.iconRes);
                    trs.transform.Find("name").GetComponent<Text>().text =prize.name;
                    trs.SetActive(true);
                }
            }
        }
    }
}
