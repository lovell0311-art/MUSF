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
    public class C2G_AddAttributePointRequestHandler : AMActorRpcHandler<C2G_AddAttributePointRequest, G2C_AddAttributePointResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AddAttributePointRequest b_Request, G2C_AddAttributePointResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AddAttributePointRequest b_Request, G2C_AddAttributePointResponse b_Response, Action<IMessage> b_Reply)
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
            void AddPoint(Pets pets, int PetsAttributeType, int Value)
            {
                switch (PetsAttributeType)
                {
                    case 1://力量
                        {
                            pets.dBPetsData.PetsSTR += Value;
                            pets.DataUpdateProperty(3);
                        }
                        break;
                    case 2:
                        {
                            pets.dBPetsData.PetsPINT += Value;
                            pets.DataUpdateProperty(5);
                        }
                        break;
                    case 3:
                        {
                            pets.dBPetsData.PetsDEX += Value;
                            pets.DataUpdateProperty(4);
                        }
                        break;
                    case 4:
                        {
                            pets.dBPetsData.PetsPSTR += Value;
                            pets.DataUpdateProperty(2);
                        }
                        break;
                }
                pets.dBPetsData.AttributePoint -= Value;

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                mWriteDataComponent.Save(pets.dBPetsData, dBProxy).Coroutine();
            }

            if (mGamePlayer != null)
            {
                if (b_Request.PetsAttributeType <= 4)
                {
                    if (mGamePlayer.Pets != null && mGamePlayer.Pets.InstanceId == b_Request.PetsID)
                    {
                        if (mGamePlayer.Pets.dBPetsData.AttributePoint >= b_Request.PetsAddPoint)
                        {
                            AddPoint(mGamePlayer.Pets, b_Request.PetsAttributeType, b_Request.PetsAddPoint);
                            b_Response.Info = new PetsInfo();
                            b_Response.Info = mGamePlayer.Pets.GetPetsInfo(out bool SetDB);
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1603);
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    else if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out var PetsInfo) != false)
                    {
                        if (PetsInfo.dBPetsData.AttributePoint >= b_Request.PetsAddPoint)
                        {
                            AddPoint(PetsInfo, b_Request.PetsAttributeType, b_Request.PetsAddPoint);
                            b_Response.Info = new PetsInfo();
                            b_Response.Info = PetsInfo.GetPetsInfo(out bool SetDB);
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1603);
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1604);
                        b_Reply(b_Response);
                        return false;
                    }
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}