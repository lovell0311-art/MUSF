using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoadScene_ChooseRole)]
    public class Event_LoadScene_ChooseRole : AEvent
    {
        public override void Run()
        {
            
            //调整摄像机的位置
            CameraComponent.Instance.MainCamera.transform.SetPositionAndRotation(new Vector3(-5f,7,25), Quaternion.Euler(0, 180, 0));
            CameraComponent.Instance.MainCamera.farClipPlane = 1000;
            CameraComponent.Instance.MainCamera.fieldOfView = 45;
            Game.Scene.GetComponent<UIComponent>().VisibleUI(RoleArchiveInfoManager.Instance.Count() != 0?UIType.UIChooseRole: UIType.UICreatRole);
           
        }

    }
}