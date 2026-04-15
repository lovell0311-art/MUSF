using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIMainComponent_Update : UpdateSystem<UIMainComponent>
    {
        private static void InvokeCountDownActionsSafely(UIMainComponent self)
        {
            if (self.CountDownAction == null)
            {
                return;
            }

            foreach (Delegate handler in self.CountDownAction.GetInvocationList())
            {
                Action action = handler as Action;
                if (action == null)
                {
                    continue;
                }

                try
                {
                    action();
                }
                catch (Exception e)
                {
                    self.CountDownAction -= action;
                    Log.Error($"UIMain CountDownAction removed failed handler={action.Method.Name}: {e}");
                }
            }
        }
       
        public override void Update(UIMainComponent self)
        {
            if (self == null || self.IsDisposed || GlobalDataManager.ChangeSceneIsChooseRole)
            {
                return;
            }

            self.FollowTeamMember();
            InvokeCountDownActionsSafely(self);


            self.UpdateMiniMap();
            self.EnsureBottomButtonListCanScroll();
            self.UpdateMountShortcutFallback();

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (GlobalDataManager.IsLocalServerAddress(GlobalDataManager.LoginConnetIP))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIAddEquipMents);
                }
            }

        }
    }
}
