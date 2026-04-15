using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        public GameObject Pk;
        Timer Pktimer;

        public void Init_PK() 
        {

            Pk = ReferenceCollector_Main.GetImage("Pk").gameObject;
            Pk.SetActive(false);
        }

        public void ShowPkState() 
        {
            Pk.SetActive(true);
            TimerComponent.Instance.RemoveTimer(Pktimer.Id);
            Pktimer = TimerComponent.Instance.RegisterTimeCallBack(500, () => { Pk.SetActive(false); });
        }
    }
}
