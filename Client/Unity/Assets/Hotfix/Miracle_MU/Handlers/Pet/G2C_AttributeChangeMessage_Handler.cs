using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_AttributeChangeMessage_Handler : AMHandler<G2C_AttributeChangeMessage>
    {
        protected override void Run(ETModel.Session session, G2C_AttributeChangeMessage message)
        {
            return;
            UIMainComponent.Instance.SetPetHpMpValue(message.IsDeath, message.PetsName, message.PetsHP, message.PetsHPMax, message.PetsMP, message.PetsMPMax);
        }
    }

}
