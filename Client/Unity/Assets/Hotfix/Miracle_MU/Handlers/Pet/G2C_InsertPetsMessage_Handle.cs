using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_InsertPetsMessage_Handle : AMHandler<G2C_InsertPetsMessage>
    {
        protected override void Run(ETModel.Session session, G2C_InsertPetsMessage message)
        {
            if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
            {
                mainComponent.ShowSiegeWarfareNotice(message.MessageID.GetTipInfo());
            }
        }
    }

}
