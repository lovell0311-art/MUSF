using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BaiTanSetNameRequestHandler : AMActorRpcHandler<C2G_BaiTanSetNameRequest, G2C_BaiTanSetNameResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BaiTanSetNameRequest b_Request, G2C_BaiTanSetNameResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_BaiTanSetNameRequest b_Request, G2C_BaiTanSetNameResponse b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get((int)(b_Request.AppendData >> 16), b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }


            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            DataCacheManageComponent mDataCacheComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();

            var mDataCache_Stall = mDataCacheComponent.Get<DBStallItem>();
            if (mDataCache_Stall == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mDataCache_Stall = await HelpDb_DBStallItem.Init(mPlayer, mDataCacheComponent, dBProxy2);
            }
            var mData = mDataCache_Stall.OnlyOne();
            if (mData == null )
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2100);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return true;
            }

            if (mData.StallItemlist == null) mData.DeSerialize();

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer.UnitData.Index != 1)
            {
                // 该地图不能摆摊
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2102);
                b_Reply(b_Response);
                return false;
            }
            if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(1, out var mapComponent) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2103);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常");
                b_Reply(b_Response);
                return false;
            }

            if (mapComponent.TryGetPosX(mGamePlayer.UnitData.X) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2104);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常x,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            var mFindTheWaySource = mapComponent.GetFindTheWay2D(mGamePlayer);
            if (mFindTheWaySource == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2105);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            if (mFindTheWaySource.IsSafeArea == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2120);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("摆摊需要在安全区!");
                b_Reply(b_Response);
                return false;
            }
            MapCellAreaComponent mMapCellField = mapComponent.GetMapCellField(mFindTheWaySource);
            if (mMapCellField == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2107);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置区域数据异常y,不能攻击!");
                b_Reply(b_Response);
                return false;
            }

            if (mMapCellField.MapStallDic.TryGetValue(mData.GameUserId, out var mStall) == false)
            {
                mStall = mMapCellField.MapStallDic[mData.GameUserId] = Root.CreateBuilder.GetInstance<C_Stall, long>(mData.GameUserId);
            }

            mStall.SetStallName(b_Request.BaiTanName);

            b_Reply(b_Response);
            return true;
        }
    }
}