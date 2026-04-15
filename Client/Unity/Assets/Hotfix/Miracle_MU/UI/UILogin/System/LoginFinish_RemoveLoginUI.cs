using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.LoginFinish)]
	public class LoginFinish_RemoveLoginUI : AEvent
	{
		public override void Run()
		{
			Log.Info("#LoginFinish# open select server ui start");
			LoginStageTrace.Append("LoginFinish_RemoveLoginUI start");
			UIComponent.Instance.Remove(UIType.UILogin);
			UIComponent.Instance.Remove(UIType.UILogin_XYSDK);
			UI ui = UIComponent.Instance.VisibleUI(UIType.UISelectServer);
			if (ui == null)
			{
				Log.Error("#LoginFinish# open select server ui failed");
				LoginStageTrace.Append("LoginFinish_RemoveLoginUI failed");
				UIComponent.Instance.VisibleUI(UIType.UIHint, "选服界面打开失败");
				return;
			}
			Log.Info("#LoginFinish# open select server ui finish");
			LoginStageTrace.Append("LoginFinish_RemoveLoginUI finish");
		}
	}

}
