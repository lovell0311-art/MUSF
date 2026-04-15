using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Batch.V20170312.Models;
using TencentCloud.Waf.V20180125.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenPetsInterfaceRequestHandler : AMActorRpcHandler<C2G_OpenPetsInterfaceRequest, G2C_OpenPetsInterfaceResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenPetsInterfaceRequest b_Request, G2C_OpenPetsInterfaceResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenPetsInterfaceRequest b_Request, G2C_OpenPetsInterfaceResponse b_Response, Action<IMessage> b_Reply)
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

            b_Response.Current = new PetsInfo();
            int mLevel = 0;
            bool IsFor = true;
            if (mGamePlayer.Pets != null && mGamePlayer.Pets.InstanceId != 0)
            {
                b_Response.Current = mGamePlayer.Pets.GetPetsInfo(out bool SetDB);
                b_Response.IsToWar = 1;
                IsFor = false;
            }

            if (mGamePlayer.PetsList.Count > 0)
            {
                PetsRankInfo petsRankInfo = new PetsRankInfo { PetsConfigID = 0, PetsID = 0, PetsLevel = 0, PetsTrialTime = 0, DeathTime = 0, IsDeath = 0 };
                foreach (var pets in mGamePlayer.PetsList)
                {
                    PetsRankInfo petsInfo = new PetsRankInfo();
                    petsInfo = pets.Value.GetPetsRankInfo();

                    if (petsInfo.PetsLevel > mLevel && IsFor)
                    {
                        mLevel = petsInfo.PetsLevel;
                        b_Response.Current = pets.Value.GetPetsInfo(out bool SetDB);

                        if (petsRankInfo.PetsID != petsInfo.PetsID)
                        {
                            if (petsRankInfo.PetsID != 0)
                                b_Response.List.Add(petsRankInfo);

                            petsRankInfo = petsInfo;
                        }
                    }
                    else
                    {
                        b_Response.List.Add(petsInfo);
                    }
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}