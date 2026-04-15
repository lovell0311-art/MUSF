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
    public class C2G_GetPetsExcellentInfoRequestHandler : AMActorRpcHandler<C2G_GetPetsExcellentInfoRequest, G2C_GetPetsExcellentInfoResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetPetsExcellentInfoRequest b_Request, G2C_GetPetsExcellentInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetPetsExcellentInfoRequest b_Request, G2C_GetPetsExcellentInfoResponse b_Response, Action<IMessage> b_Reply)
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

            if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out var pets) != false)
            {
                b_Response.ExcellentList.AddRange(pets.dBPetsData.ExcellentId);
            }

            if (mGamePlayer.Pets != null && mGamePlayer.Pets.InstanceId == b_Request.PetsID)
            {
                b_Response.ExcellentList.AddRange(mGamePlayer.Pets.dBPetsData.ExcellentId);
            }
            b_Reply(b_Response);
            return true;
        }
    }
}