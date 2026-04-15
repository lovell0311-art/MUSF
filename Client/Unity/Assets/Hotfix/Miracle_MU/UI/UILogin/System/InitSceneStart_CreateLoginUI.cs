using ETModel;
using UnityEngine;
namespace ETHotfix
{
	[Event(EventIdType.InitSceneStart)]
	public class InitSceneStart_CreateLoginUI: AEvent
	{
		public override void Run()
		{
            LoginStageTrace.Clear();
            LoginStageTrace.Append("InitSceneStart_CreateLoginUI enter");
            UIComponent.Instance.Remove(UIType.UISceneLoading);
            LoginStageTrace.Append("InitSceneStart_CreateLoginUI remove UISceneLoading");
            //显示公告 界面
            //UIComponent.Instance.VisibleUI(UIType.UIAnnouncement);
            OnHookSetInfoTools.Init();
            LoginStageTrace.Append("InitSceneStart_CreateLoginUI visible UILogin");
            UIComponent.Instance.VisibleUI(UIType.UILogin);
            LoginStageTrace.Append("InitSceneStart_CreateLoginUI finish");
        }
    }
}
