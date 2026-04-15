using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIAnnouncementComponentAwake : AwakeSystem<UIAnnouncementComponent>
    {
        public override void Awake(UIAnnouncementComponent self)
        {
            self.Awake();
        }
    }
    /// <summary>
    /// ¹«¸æÃæ°å
    /// </summary>
    public class UIAnnouncementComponent : Component
    {


        public void Awake()
        {
            ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            //TitleConfig_CnnounInfoConfig titleConfig_Cnnoun = ConfigComponent.Instance.GetItem<TitleConfig_CnnounInfoConfig>(1);
            //collector.GetText("Content").text = titleConfig_Cnnoun.AnnounContent;
        }







        //Toggle music_tog;
        //public void Awake()
        //{
        //    ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
        //    collector.GetButton("startBtn").onClick.AddSingleListener(SartBtnClick);
        //    music_tog = collector.GetToggle("Music_Tog");
        //    GameSetInfo gameSetInfo = LocalDataJsonComponent.Instance.gameSetInfo;
        //    music_tog.onValueChanged.AddSingleListener(ChangAudioState);
        //    music_tog.isOn = !gameSetInfo.CloseMusic;

        //}
        //public void SartBtnClick()
        //{
        //    UIComponent.Instance.Remove(UIType.UIAnnouncement);
        //    UIComponent.Instance.VisibleUI(UIType.UILogin);
        //}
        //public void ChangAudioState(bool ison)
        //{
        //    GameSetInfo gameSetInfo = LocalDataJsonComponent.Instance.gameSetInfo;
        //    gameSetInfo.CloseMusic = !ison;
        //    LocalDataJsonComponent.Instance.gameSetInfo.CloseMusic = !ison;
        //    LocalDataJsonComponent.Instance.SavaData(gameSetInfo,LocalJsonDataKeys.GameSetInfo);
        //}
    }
}