using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoadScene_KanTeLuFeiXu)]
    public class Event_LoadScene_KanTeLuFeiXu : AEvent
    {
        public override void Run()
        {
            //加载场景中的 音效
            SoundComponent.Instance.GetCurSceneSounds();
            //加载当前场景中的音效
            SoundComponent.Instance.LoadSceneAudioRefrence($"Audio_{SceneName.KanTeLuFeiXu.EnumToString<SceneName>()}".StringToAB(), $"Audio_{SceneName.KanTeLuFeiXu.EnumToString<SceneName>()}").Coroutine();
            //播放背景音乐
            SoundComponent.Instance.PlayBgSound(SceneName.KanTeLuFeiXu.EnumToString<SceneName>());
          
        }
    }
}
