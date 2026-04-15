using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoadScene_CangBaoTu)]
    public class Event_LoadScene_CangBaoTu : AEvent
    {
        public override void Run()
        {
            //加载场景中的 音效
            SoundComponent.Instance.GetCurSceneSounds();
            //加载当前场景中的音效
            /*  SoundComponent.Instance.LoadSceneAudioRefrence($"Audio_{SceneName.cangbaotu.EnumToString<SceneName>()}".StringToAB(), $"Audio_{SceneName.cangbaotu.EnumToString<SceneName>()}").Coroutine();
              //播放背景音乐
              SoundComponent.Instance.PlayBgSound(SceneName.cangbaotu.EnumToString<SceneName>());*/
            UIMainComponent.Instance.CountDownAction += BossDownTime;
        }

        float waitTimenexttime = 1;
        int waitTime = 60;
        public void BossDownTime() 
        {
            if (Time.time > waitTimenexttime)
            {
                UIMainComponent.Instance.ShowFuBenInfo($"<藏宝图Boss>:<color=red>{--waitTime}</color>秒后苏醒");
              
                waitTimenexttime = Time.time + 1;
              
                if (waitTime <= 0)
                {
                    UIMainComponent.Instance.HideFuBenInfo();
                    waitTime = 60;
                    UIMainComponent.Instance.CountDownAction -= BossDownTime;
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"藏宝图Boss 已苏醒");
                }
            }
        }
    }

}