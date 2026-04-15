using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using ETModel.Robot;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SetPlayerTitleHandler : AMActorRpcHandler<C2G_SetPlayerTitleRequest, G2C_SetPlayerTitleResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SetPlayerTitleRequest b_Request, G2C_SetPlayerTitleResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SetPlayerTitleRequest b_Request, G2C_SetPlayerTitleResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return false;
            }
           
            var Title = mPlayer.GetCustomComponent<PlayerTitle>();
            if (Title != null)
            {
                if (Title.SetTitle(b_Request.UseTitle))
                {
                    var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
                    var mData = mGamePlayer.UnitData;
                    if (mPlayer == null || mGamePlayer == null)
                    {
                        b_Reply(b_Response);
                        return false;
                    }
                    var mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mData.Index, mPlayer.GameUserId);
                    if (mapComponent == null)
                    {
                        b_Reply(b_Response);
                        return false;
                    }

                    mPlayer.Send(new G2C_MovePos_notice()
                    {
                        UnitType = (int)E_Identity.Hero,
                        GameUserId = mGamePlayer.InstanceId,
                        //MapId = mGamePlayer.UnitData.Index,
                        X = mData.X,
                        Y = mData.Y,
                        Angle = mData.Angle,
                        IsNeedMove = 0,
                        Title = mGamePlayer.Data.Title,
                        WallTitle = mGamePlayer.Data.WallTile,
                        ReincarnateCnt = mGamePlayer.Data.ReincarnateCnt
                    });
                    var mFindTheWaySource = mapComponent.GetFindTheWay2D(mData.X, mData.Y);
                    var mMapCellTarget  = mapComponent.GetFindTheWay2D(mData.X, mData.Y);
                    mapComponent.MoveSendNotice(mFindTheWaySource, mMapCellTarget, mGamePlayer);

                    var equipComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
                    if (equipComponent != null)
                    {
                        equipComponent.ApplyEquipProp();
                    }
                    b_Reply(b_Response);
                    return true;
                }
            }


            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
            b_Reply(b_Response);
            return false;
        }
    }
}