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
    public class C2G_OpenNewPetsUIHandler : AMActorRpcHandler<C2G_OpenNewPetsUI, G2C_OpenNewPetsUI>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenNewPetsUI b_Request, G2C_OpenNewPetsUI b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenNewPetsUI b_Request, G2C_OpenNewPetsUI b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1600);
                b_Reply(b_Response);
                return false;
            }
            if (mGamePlayer.Pets != null)
            {
                NewPetsInfo info = new NewPetsInfo
                {
                    PetsConfigID = mGamePlayer.Pets.dBPetsData.ConfigID,
                    PetsID = mGamePlayer.Pets.dBPetsData.PetsId,
                    ISBattle = mGamePlayer.Pets.dBPetsData.PetsUseState,
                    DaoQiTime = mGamePlayer.Pets.dBPetsData.PetsTrialTime
                };
                info.SkillID.AddRange(mGamePlayer.Pets.dBPetsData.SkillId);
                info.Excellent.AddRange(mGamePlayer.Pets.dBPetsData.ExcellentId);
                b_Response.PetList.Add(info);
            }
            foreach (var info in mGamePlayer.PetsList)
            {
                NewPetsInfo info1 = new NewPetsInfo
                {
                    PetsConfigID = info.Value.dBPetsData.ConfigID,
                    PetsID = info.Value.dBPetsData.PetsId,
                    ISBattle = info.Value.dBPetsData.PetsUseState,
                    DaoQiTime = info.Value.dBPetsData.PetsTrialTime
                };
                info1.SkillID.AddRange(info.Value.dBPetsData.SkillId);
                info1.Excellent.AddRange(info.Value.dBPetsData.ExcellentId);
                b_Response.PetList.Add(info1);
            }

             b_Reply(b_Response);
            return true;
        }
    }
}