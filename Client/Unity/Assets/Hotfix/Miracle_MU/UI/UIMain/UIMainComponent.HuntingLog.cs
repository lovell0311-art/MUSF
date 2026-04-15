using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

using UnityEngine.EventSystems;

namespace ETHotfix 
{
    
    /// <summary>
    /// 狩猎日志
    /// </summary>
public partial class UIMainComponent
{
        public GameObject HuntingPanel;
        public ReferenceCollector huntingRef;
        Text hunttime, demage, monstercount, getexp, gethp;
        public float CountTime = 0;

        public long demageCount;
        public long killMonsterCount;
        public long GetExpCount;

        bool isChange = false;
        GameObject Infos;

        UGUITriggerProxy triggerProxy;

        private RectTransform dragTarget;//可以拖动的UI区域


        public void Init_HuntingLog() 
        {
            HuntingPanel = ReferenceCollector_Main.GetGameObject("HuntingLog").gameObject;
            huntingRef = HuntingPanel.GetReferenceCollector();
            hunttime = huntingRef.GetText("time");
            demage = huntingRef.GetText("demage");
            monstercount = huntingRef.GetText("monstercount");
            getexp = huntingRef.GetText("exp");
            gethp = huntingRef.GetText("hp");
            HuntingPanel.SetActive(false);

            Infos = huntingRef.GetImage("Infos").gameObject;
            huntingRef.GetButton("close").onClick.AddSingleListener(() => { Infos.SetActive(false); });
            huntingRef.GetButton("open").onClick.AddSingleListener(() => { Infos.SetActive(true); });

            triggerProxy = HuntingPanel.GetComponent<UGUITriggerProxy>();

            dragTarget = HuntingPanel.GetComponent<RectTransform>();


            triggerProxy.OnDragEvents = OnDrag;


            triggerProxy.OnPointerDownEvents = OnPointerDown;



        }

        public void OnDrag(PointerEventData eventData)

        {

            // 移动拖拽框的位置

            //  dragTarget.anchoredPosition += eventData.delta / canvas.scaleFactor;

            Vector2 newPos = dragTarget.anchoredPosition + eventData.delta / canvas.scaleFactor;

            float x = Mathf.Clamp(newPos.x, 0, Screen.width / canvas.scaleFactor - dragTarget.sizeDelta.x);

            float y = Mathf.Clamp(newPos.y, 0, Screen.height / canvas.scaleFactor - dragTarget.sizeDelta.y);

            dragTarget.anchoredPosition = new Vector2(x, y);


        }


        public void OnPointerDown(PointerEventData eventData)

        {

            // 把当前选中的拖拽框显示在最前面

            dragTarget.SetAsLastSibling();

        }


        public void ShowHuntLog() 
        {
            isChange=true;
          //  HuntingPanel.SetActive(true);
            CountTime = 0;
            demageCount = 0;
            killMonsterCount = 0;
            GetExpCount = 0;
            demage.text = string.Empty;
            monstercount.text = string.Empty;
            getexp.text = string.Empty;
            gethp.text = string.Empty;
            CountDownAction += KeepTimeEvent;
        }
        public void HideHuntLog()
        {
            isChange = false;
            HuntingPanel.SetActive(false);
            CountDownAction -= KeepTimeEvent;
        }

        void KeepTimeEvent() 
        {
            CountTime += Time.deltaTime;
            hunttime.text= TimerComponent.Instance.ToTimeFormat((int)CountTime);
        }
        //更新每秒伤害
        public void ChangeDemageCount(long value) 
        {
            if (isChange == false) return;
            demageCount += value;
            demage.text = $"{(demageCount / CountTime).ToString("0.00")}";
        }  
        public void ChangeKillMonsterCount(long value) 
        {
            if (isChange == false) return;
            killMonsterCount += value;
            gethp.text = killMonsterCount.ToString();
            monstercount.text = $"{(killMonsterCount / CountTime).ToString("0.00")}";
        }  
        public void ChangeGetExp(long value) 
        {
            if (isChange == false) return;
            GetExpCount += value;
            getexp.text = $"{(GetExpCount / CountTime).ToString("0.00")}";
        }
}
}