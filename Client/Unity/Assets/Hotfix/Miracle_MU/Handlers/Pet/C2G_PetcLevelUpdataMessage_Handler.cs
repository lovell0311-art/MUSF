using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class C2G_PetcLevelUpdataMessage_Handler : AMHandler<C2G_PetcLevelUpdataMessage>
    {
        protected override void Run(ETModel.Session session, C2G_PetcLevelUpdataMessage message)
        {
            RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Pet, 1);
            UIMainComponent.Instance.RedDotFriendCheack();
        }
    }

}
