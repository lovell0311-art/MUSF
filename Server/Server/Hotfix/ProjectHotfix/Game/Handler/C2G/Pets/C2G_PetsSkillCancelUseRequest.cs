using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using TencentCloud.Mps.V20190612.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PetsSkillCancelUseRequestHandler : AMActorRpcHandler<C2G_PetsSkillCancelUseRequest, G2C_PetsSkillCancelUseResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetsSkillCancelUseRequest b_Request, G2C_PetsSkillCancelUseResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetsSkillCancelUseRequest b_Request, G2C_PetsSkillCancelUseResponse b_Response, Action<IMessage> b_Reply)
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

            var mGamePalyer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePalyer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }
            if (mGamePalyer.Pets != null && mGamePalyer.Pets.dBPetsData.PetsId == b_Request.PetsID)
            {
                if (mGamePalyer.Pets.SkillGroup.TryGetValue(mGamePalyer.Pets.dBPetsData.UseSkillID, out var SkillInfo) != false)
                {
                    SkillInfo = mGamePalyer.Pets.SkillCurrent;
                    mGamePalyer.Pets.dBPetsData.UseSkillID = 0;
                    mGamePalyer.Pets.SkillCurrent = new C_HeroSkillSource();

                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                    mWriteDataComponent.Save(mGamePalyer.Pets.dBPetsData, dBProxy).Coroutine();
                    b_Reply(b_Response);
                    return true;
                }
            }
            else if (mGamePalyer.PetsList.TryGetValue(b_Request.PetsID, out var Petsinfo) != false)
            {
                if (Petsinfo.SkillGroup.TryGetValue(Petsinfo.dBPetsData.UseSkillID, out var SkillInfo) != false)
                {
                    SkillInfo = Petsinfo.SkillCurrent;
                    Petsinfo.dBPetsData.UseSkillID = 0;
                    Petsinfo.SkillCurrent = new C_HeroSkillSource();

                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                    mWriteDataComponent.Save(Petsinfo.dBPetsData, dBProxy).Coroutine();
                    b_Reply(b_Response);
                    return true;
                }
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1604);
            b_Reply(b_Response);
            return false;
        }
    }
}