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
    public class C2G_PetsReleaseRequestHandler : AMActorRpcHandler<C2G_PetsReleaseRequest, G2C_PetsReleaseResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetsReleaseRequest b_Request, G2C_PetsReleaseResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetsReleaseRequest b_Request, G2C_PetsReleaseResponse b_Response, Action<IMessage> b_Reply)
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
            /*if (mGamePlayer != null && mGamePlayer.Data.Level < 50)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1600);
                b_Reply(b_Response);
                return false;
            }*/


            if (mGamePlayer.Pets != null && mGamePlayer.Pets.dBPetsData.PetsId == b_Request.PetsID)
            {
                mGamePlayer.Pets.dBPetsData.IsDisabled = 1;
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                mWriteDataComponent.Save(mGamePlayer.Pets.dBPetsData, dBProxy).Coroutine();

                MapComponent targetMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.Pets.UnitData.Index, mPlayer.GameUserId);
                if (targetMapComponent != null)
                {
                    var mFindTheWay = targetMapComponent.GetFindTheWay2D(mGamePlayer.Pets);

                    targetMapComponent.QuitMap(mFindTheWay, mGamePlayer.Pets);
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.AttackTarget = mGamePlayer.Pets.InstanceId;
                    mAttackResultNotice.HpValue = 0;
                    targetMapComponent.SendNotice(mGamePlayer.Pets, mAttackResultNotice);
                }
                mGamePlayer.Pets.Dispose();
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
                }
            }
            else if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out var PetsInfo) != false)
            {
                PetsInfo.dBPetsData.IsDisabled = 1;
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                mWriteDataComponent.Save(PetsInfo.dBPetsData, dBProxy).Coroutine();
                mGamePlayer.PetsList.Remove(b_Request.PetsID);
                PetsInfo.Dispose();
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1604);
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}