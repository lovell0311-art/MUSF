using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoadScene_TianKongZhiCheng)]
    public class Event_LoadScene_TianKongZhiCheng : AEvent
    {
        public override void Run()
        {
            //加载场景中的 音效
            SoundComponent.Instance.GetCurSceneSounds();
            //加载当前场景中的音效
            SoundComponent.Instance.LoadSceneAudioRefrence($"Audio_{SceneName.TianKongZhiCheng.EnumToString<SceneName>()}".StringToAB(), $"Audio_{SceneName.TianKongZhiCheng.EnumToString<SceneName>()}").Coroutine();
            //播放背景音乐
            SoundComponent.Instance.PlayBgSound(SceneName.TianKongZhiCheng.EnumToString<SceneName>());
          
        }
    }
}
