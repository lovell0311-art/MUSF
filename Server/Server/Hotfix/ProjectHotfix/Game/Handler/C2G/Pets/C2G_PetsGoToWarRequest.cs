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
    public class C2G_PetsGoToWarRequestHandler : AMActorRpcHandler<C2G_PetsGoToWarRequest, G2C_PetsGoToWarResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetsGoToWarRequest b_Request, G2C_PetsGoToWarResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetsGoToWarRequest b_Request, G2C_PetsGoToWarResponse b_Response, Action<IMessage> b_Reply)
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

            if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out var PetsInfo) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1604);
                b_Reply(b_Response);
                return false;
            }

            if (mGamePlayer.Pets != null && mGamePlayer.Pets.dBPetsData.PetsId == b_Request.PetsID)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1605);
                b_Reply(b_Response);
                return false;
            }

            if (mGamePlayer.Pets != null)
            {
                Pets pets = mGamePlayer.Pets;
                if (mGamePlayer.PetsList.ContainsKey(pets.dBPetsData.PetsId))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1606);
                    b_Reply(b_Response);
                    return false;
                }
                pets.dBPetsData.PetsUseState = 0;
                mGamePlayer.PetsList.Add(pets.dBPetsData.PetsId, pets);
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                mWriteDataComponent.Save(pets.dBPetsData, dBProxy).Coroutine();

                MapComponent mOldMapComponent = pets.CurrentMap;
                if (mOldMapComponent != null)
                {
                    var mEndTheWay = mOldMapComponent.GetFindTheWay2D(pets);
                    mOldMapComponent.QuitMap(mEndTheWay, mGamePlayer.Pets);
                }
                mGamePlayer.Pets = null;
            }

            {
                PetsInfo.UnitData.Angle = mGamePlayer.UnitData.Angle;
                PetsInfo.GamePlayer = mGamePlayer;
                PetsInfo.Pathlist = null;
                PetsInfo.dBPetsData.PetsUseState = 1;

                mGamePlayer.Pets = PetsInfo;
                mGamePlayer.PetsList.Remove(b_Request.PetsID);
                if (PetsInfo.IsDeath)
                {
                    // 无需对死亡状态进行处理
                    // 宠物在上线后，已经将死亡的宠物添加 RebirthComponent
                }
                else
                {
                    // 宠物是活着的
                    var equipComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
                    equipComponent?.ApplyEquipProp();

                    MapComponent targetMapComponent = mGamePlayer.CurrentMap;
                    if (targetMapComponent != null)
                    {
                        var mFindTheWay = targetMapComponent.GetFindTheWay2D(mGamePlayer);
                        targetMapComponent.MoveSendNotice(null, mFindTheWay, PetsInfo);
                    }
                }

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                mWriteDataComponent.Save(PetsInfo.dBPetsData, dBProxy).Coroutine();
            }

            b_Reply(b_Response);
            return true;

        }
    }
}