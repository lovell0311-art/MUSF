using ETModel;

namespace ETHotfix
{
    //¶¥ºÅÍ¨Öª
    [MessageHandler]
    public class Gate2C_LoginFromOtherDevices_Handler : AMHandler<Gate2C_LoginFromOtherDevices>
    {
        protected override void Run(ETModel.Session session, Gate2C_LoginFromOtherDevices message)
        {
           UIConfirmComponentExtend.GetUIConfirmComponent(tiptype: BanType.OFFLINE.ToString());
            GlobalDataManager.IsOFFLINE = true;
            CameraFollowComponent.Instance.followTarget = null;
        }
    }
}
