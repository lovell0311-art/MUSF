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
    public class C2G_GetPetsInfoRequestHandler : AMActorRpcHandler<C2G_GetPetsInfoRequest, G2C_GetPetsInfoResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetPetsInfoRequest b_Request, G2C_GetPetsInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetPetsInfoRequest b_Request, G2C_GetPetsInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
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

            void UpdataPetsTime(PetsInfo petsInfo, Pets pets)
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                mWriteDataComponent.Save(pets.dBPetsData, dBProxy).Coroutine();

                if (pets.dBPetsData.IsDisabled == 1)
                {
                    G2C_InsertPetsMessage g2C_InsertPetsMessage = new G2C_InsertPetsMessage();
                    g2C_InsertPetsMessage.State = 1;
                    g2C_InsertPetsMessage.MessageID = 1610;
                    mPlayer.Send(g2C_InsertPetsMessage);
                }
            }

            if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out var pets) != false)
            {
                PetsInfo petsInfo = pets.GetPetsInfo(out bool bSuccess);
                if (bSuccess)
                    UpdataPetsTime(petsInfo, pets);
                b_Response.Info = petsInfo;
            }

            if (mGamePlayer.Pets != null && mGamePlayer.Pets.InstanceId == b_Request.PetsID)
            {
                PetsInfo petsInfo = mGamePlayer.Pets.GetPetsInfo(out bool bSuccess);

                if (bSuccess)
                    UpdataPetsTime(petsInfo, mGamePlayer.Pets);
                b_Response.Info = petsInfo;
            }

            b_Reply(b_Response);
            return true;

        }
    }
}