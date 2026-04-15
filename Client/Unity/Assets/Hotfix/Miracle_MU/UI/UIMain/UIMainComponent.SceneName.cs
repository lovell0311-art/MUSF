using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 场景名字显示
    /// </summary>
    public partial class UIMainComponent
    {
        public GameObject sceneBox;
        public Text scenetext;
        Timer timer;
        long showtime = 10000;//显示时间
        public void Init_SceneName() 
        {
            sceneBox = ReferenceCollector_Main.GetImage("SceneName").gameObject;
            scenetext = sceneBox.GetComponentInChildren<Text>();

            scenetext.text = SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName();
            sceneBox.SetActive(true);
            timer= TimerComponent.Instance.RegisterTimeCallBack(showtime, () => { sceneBox.SetActive(false); });
        }

        public void ShowSceneName(int sceneId) 
        {
            RemoveSceneName();
            scenetext.text = ((SceneName)sceneId).GetSceneName();
            sceneBox.SetActive(true);
            timer= TimerComponent.Instance.RegisterTimeCallBack(showtime, () => { sceneBox.SetActive(false); });
        }

        public void RemoveSceneName()
        {
            TimerComponent.Instance.RemoveTimer(timer.Id);
        }
    }
}
