using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoadScene_HuanShuYuan)]
    public class Event_LoadScene_HuanShuYuan : AEvent
    {
        public override void Run()
        {
            //加载场景中的 音效
            SoundComponent.Instance.GetCurSceneSounds();
            //加载当前场景中的音效
            SoundComponent.Instance.LoadSceneAudioRefrence($"Audio_{SceneName.HuanShuYuan.EnumToString<SceneName>()}".StringToAB(), $"Audio_{SceneName.HuanShuYuan.EnumToString<SceneName>()}").Coroutine();
            //播放背景音乐
            SoundComponent.Instance.PlayBgSound(SceneName.HuanShuYuan.EnumToString<SceneName>());

        }
    }
}
