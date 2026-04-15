using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_UsePetsSkillRequestHandler : AMActorRpcHandler<C2G_UsePetsSkillRequest, G2C_UsePetsSkillResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_UsePetsSkillRequest b_Request, G2C_UsePetsSkillResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_UsePetsSkillRequest b_Request, G2C_UsePetsSkillResponse b_Response, Action<IMessage> b_Reply)
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
            /*if (mGamePalyer != null && mGamePalyer.Data.Level < 50)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1600);
                b_Reply(b_Response);
                return false;
            }*/

            void UsePetsSkill(Pets pets, int SkillID)
            {
                if (pets.dBPetsData.UseSkillID != 0)
                {
                    if (pets.SkillGroup.TryGetValue(pets.dBPetsData.UseSkillID, out var SkillInfo) != false)
                    {
                        pets.dBPetsData.UseSkillID = 0;
                        SkillInfo = pets.SkillCurrent;
                    }
                }

                //if (mGamePalyer.Pets.SkillGroup.ContainsKey(b_Request.SkillID) != false)
                {
                    if (pets.SkillGroup.TryGetValue(b_Request.SkillID, out var SkillInfo) != false)
                    {
                        pets.dBPetsData.UseSkillID = b_Request.SkillID;
                        pets.SkillCurrent = SkillInfo;
                    }
                }
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                mWriteDataComponent.Save(pets.dBPetsData, dBProxy).Coroutine();
            }

            if (mGamePalyer.Pets != null && mGamePalyer.Pets.dBPetsData.PetsId == b_Request.PetsID)
            {
                if (mGamePalyer.Pets.dBPetsData.UseSkillID == b_Request.SkillID)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1607);
                    b_Reply(b_Response);
                    return false;
                }
                UsePetsSkill(mGamePalyer.Pets, b_Request.SkillID);

                b_Reply(b_Response);
                return true;
            }
            else if (mGamePalyer.PetsList.TryGetValue(b_Request.PetsID, out var Petsinfo) != false)
            {
                if (Petsinfo.dBPetsData.UseSkillID == b_Request.SkillID)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1607);
                    b_Reply(b_Response);
                    return false;
                }
                UsePetsSkill(Petsinfo, b_Request.SkillID);

                b_Reply(b_Response);
                return true;
            }
            b_Reply(b_Response);
            return false;
        }
    }
}