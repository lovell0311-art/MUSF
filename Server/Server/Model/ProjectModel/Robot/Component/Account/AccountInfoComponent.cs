
namespace ETModel.Robot
{
    [ObjectSystem]
    public class AccountInfoComponentDestroySystem : DestroySystem<AccountInfoComponent>
    {
        public override void Destroy(AccountInfoComponent self)
        {
            self.UserId = 0;
            self.Phone = "";
            self.GateAddress = "";
            self.GateId = 0;
            self.GateKey = "";
        }
    }



    public class AccountInfoComponent : Component
    {
        public long UserId;
        public string Phone;


        public string GateAddress;
        public long GateId;
        public string GateKey;

    }
}
