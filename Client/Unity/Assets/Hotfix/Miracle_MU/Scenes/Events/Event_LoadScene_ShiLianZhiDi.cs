using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoadScene_ShiLianZhiDi)]
    public class Event_LoadScene_ShiLianZhiDi : AEvent
    {
        public override void Run()
        {
            //加载场景中的 音效
            SoundComponent.Instance.GetCurSceneSounds();
         
        }
     

    }

}