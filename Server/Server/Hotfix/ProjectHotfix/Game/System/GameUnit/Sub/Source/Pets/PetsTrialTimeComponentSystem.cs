using ETModel;
using ETModel.ET;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Drawing;


namespace ETHotfix
{
    [Timer(TimerType.PetsTrialTime)]
    public class PetsTrialTimeTimer : ATimer<PetsTrialTimeComponent>
    {
        public override void Run(PetsTrialTimeComponent self)
        {
            Pets pet = self.Parent;
            GamePlayer gamePlayer = pet.GamePlayer;
            if (gamePlayer.Player.OnlineStatus != EOnlineStatus.Online) return;

            if (gamePlayer.Pets != null && gamePlayer.Pets.Id == pet.Id)
            {
                G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                mAttackResultNotice.AttackTarget = pet.InstanceId;
                mAttackResultNotice.HpValue = 0;
                pet.CurrentMap?.SendNotice(pet, mAttackResultNotice);

                pet.dBPetsData.IsDisabled = 1;
                pet.CurrentMap?.Leave(pet);
                gamePlayer.Pets = null;


                G2C_InsertPetsMessage g2C_InsertPetsMessage = new G2C_InsertPetsMessage();
                g2C_InsertPetsMessage.State = 2;
                g2C_InsertPetsMessage.MessageID = 1610;
                gamePlayer.Player.Send(g2C_InsertPetsMessage);
            }
            else
            {
                pet.dBPetsData.IsDisabled = 1;
                gamePlayer.PetsList.Remove(pet.dBPetsData.PetsId);
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)gamePlayer.Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)gamePlayer.Player.GameAreaId);
            mWriteDataComponent.Save(pet.dBPetsData, dBProxy).Coroutine();


            pet.Dispose();
        }
    }

    [FriendOf(typeof(PetsTrialTimeComponent))]
    [EventMethod(typeof(PetsTrialTimeComponent), EventSystemType.INIT)]
    public class PetsTrialTimeComponentEventOnInit : ITEventMethodOnInit<PetsTrialTimeComponent>
    {
        public void OnInit(PetsTrialTimeComponent self)
        {
            Pets pet = self.Parent;
            if(pet.dBPetsData.PetsTrialTime <= 0)
            {
                Log.Error($"宠物不需要添加 'PetsTrialTimeComponent' 组件 petsId:{pet.dBPetsData.PetsId}");
                return;
            }
            self.timerId = TimerComponent.Instance.NewOnceTimer(pet.dBPetsData.PetsTrialTime * 1000, TimerType.PetsTrialTime, self);
        }
    }

    [FriendOf(typeof(PetsTrialTimeComponent))]
    [EventMethod(typeof(PetsTrialTimeComponent), EventSystemType.DISPOSE)]
    public class PetsTrialTimeComponentEventOnDispose : ITEventMethodOnDispose<PetsTrialTimeComponent>
    {
        public override void OnDispose(PetsTrialTimeComponent self)
        {
            TimerComponent.Instance.Remove(ref self.timerId);
        }
    }

    [FriendOf(typeof(PetsTrialTimeComponent))]
    public static class PetsTrialTimeComponentSystem
    {

    }
}
