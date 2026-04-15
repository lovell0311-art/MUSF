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
    public class C2G_AttributeChangeRequestHandler : AMActorRpcHandler<C2G_AttributeChangeRequest, G2C_AttributeChangeResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AttributeChangeRequest b_Request, G2C_AttributeChangeResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AttributeChangeRequest b_Request, G2C_AttributeChangeResponse b_Response, Action<IMessage> b_Reply)
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
            if (mGamePlayer != null)
            {
                if (mGamePlayer.Pets != null)
                {
                    G2C_AttributeChangeMessage g2C_AttributeChangeMessage = new G2C_AttributeChangeMessage();
                    g2C_AttributeChangeMessage.PetsName = mGamePlayer.Pets.dBPetsData.PetsName;
                    g2C_AttributeChangeMessage.PetsHP = mGamePlayer.Pets.GetNumerialFunc(E_GameProperty.PROP_HP);
                    g2C_AttributeChangeMessage.PetsHPMax = mGamePlayer.Pets.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    g2C_AttributeChangeMessage.PetsMP = mGamePlayer.Pets.GetNumerialFunc(E_GameProperty.PROP_MP);
                    g2C_AttributeChangeMessage.PetsMPMax = mGamePlayer.Pets.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                    g2C_AttributeChangeMessage.IsDeath = mGamePlayer.Pets.IsDeath ? 1 : 0;
                    mPlayer.Send(g2C_AttributeChangeMessage);
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}