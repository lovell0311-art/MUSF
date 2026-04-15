using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoadScene_DiXiaCheng)]
    public class Event_LoadScene_DiXiaCheng : AEvent
    {
        public override void Run()
        {
            //加载场景中的 音效
            SoundComponent.Instance.GetCurSceneSounds();
            //加载当前场景中的音效
            SoundComponent.Instance.LoadSceneAudioRefrence($"Audio_{SceneName.DiXiaCheng.EnumToString<SceneName>()}".StringToAB(), $"Audio_{SceneName.DiXiaCheng.EnumToString<SceneName>()}").Coroutine();
            //播放背景音乐
            SoundComponent.Instance.PlayBgSound(SceneName.DiXiaCheng.EnumToString<SceneName>());
        
        }
    }
}
