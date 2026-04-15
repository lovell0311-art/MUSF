using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PetsRestRequestHandler : AMActorRpcHandler<C2G_PetsRestRequest, G2C_PetsRestResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetsRestRequest b_Request, G2C_PetsRestResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetsRestRequest b_Request, G2C_PetsRestResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return false;
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }
            if (mGamePlayer.Pets != null && mGamePlayer.Pets.dBPetsData.PetsId == b_Request.PetsID)
            {
                Pets pets = mGamePlayer.Pets;
                if (mGamePlayer.PetsList.ContainsKey(pets.dBPetsData.PetsId))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1606);
                    b_Reply(b_Response);
                    return false;
                }
                pets.IsAttacking = false;
                pets.dBPetsData.PetsUseState = 0;
                pets.Pathlist = null;
                mGamePlayer.PetsList.Add(pets.dBPetsData.PetsId, pets);

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                mWriteDataComponent.Save(pets.dBPetsData, dBProxy).Coroutine();

                pets.CurrentMap?.Leave(pets);
                mGamePlayer.Pets = null;
                var equipComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
                if (equipComponent != null)
                {
                    equipComponent.ApplyEquipProp();
                }

                if (mGamePlayer.CurrentMap != null)
                {
                    mGamePlayer.RemoveHealthState(E_BattleSkillStats.FangYuHuZhao, mGamePlayer.CurrentMap.GetCustomComponent<BattleComponent>(), true);
                    mGamePlayer.UpdateHealthState();

                    pets.RemoveHealthState(E_BattleSkillStats.FangYuHuZhao, mGamePlayer.CurrentMap.GetCustomComponent<BattleComponent>(), true);
                    pets.UpdateHealthState();
                }

                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1604);
            b_Reply(b_Response);
            return false;
        }
    }
}