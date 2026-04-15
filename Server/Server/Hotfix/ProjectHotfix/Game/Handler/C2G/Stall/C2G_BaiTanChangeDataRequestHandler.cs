using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BaiTanChangeDataRequestHandler : AMActorRpcHandler<C2G_BaiTanChangeDataRequest, G2C_BaiTanChangeDataResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BaiTanChangeDataRequest b_Request, G2C_BaiTanChangeDataResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_BaiTanChangeDataRequest b_Request, G2C_BaiTanChangeDataResponse b_Response, Action<IMessage> b_Reply)
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
            if (mData == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2100);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return true;
            }

            if (mData.StallItemlist == null) mData.DeSerialize();

            if (mData.StallItemlist.TryGetValue(b_Request.Prop.ItemUUID, out var mInstanceId) == false)
            {
                // 摊位不存在该物品
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2108);
                b_Reply(b_Response);
                return false;
            }

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
            C_FindTheWay2D mFindTheWaySource = mapComponent.GetFindTheWay2D(mGamePlayer);
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

            if (mMapCellField.MapStallDic.TryGetValue(mData.GameUserId, out var mStallComponent) == false)
            {
                // 查找不到你的摊位
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2109);
                b_Reply(b_Response);
                return false;
            }

            if (mStallComponent.keyValuePairs.TryGetValue(b_Request.Prop.ItemUUID, out var mItem) == false)
            {
                mItem = ItemFactory.CreateFormDB(b_Request.Prop.ItemUUID, mPlayer);
                if (mItem == null)
                {
                    // 查找不到你的摊位
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2110);
                    b_Reply(b_Response);
                    return false;
                }

                mStallComponent.keyValuePairs[b_Request.Prop.ItemUUID] = mItem;
            }

            var oldPrice = mData.StallItemlist[b_Request.Prop.ItemUUID];
            if ((oldPrice.Item1 != b_Request.Prop.Price) ||
                (oldPrice.Item2 != b_Request.Prop.Price2))
            {
                if (b_Request.Prop.Price < 0 || b_Request.Prop.Price > 999999999 || b_Request.Prop.Price2 < 0 || b_Request.Prop.Price2 > 999999)
                {
                    // 该地图不能摆摊
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2119);
                    b_Reply(b_Response);
                    return false;
                }
                if (b_Request.Prop.Price == 0 && b_Request.Prop.Price2 == 0)
                {
                    // 该地图不能摆摊
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2119);
                    b_Reply(b_Response);
                    return false;
                }
            }

            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);

            bool mChangeDataPos = false;
            bool mChangeData = false;
            int posX = mItem.data.posX, posY = mItem.data.posY;
            if (mItem.data.posX != b_Request.Prop.PosInBackpackX)
            {
                mChangeDataPos = true;
                mItem.data.posX = b_Request.Prop.PosInBackpackX;
            }
            if (mItem.data.posY != b_Request.Prop.PosInBackpackY)
            {
                mChangeDataPos = true;
                mItem.data.posY = b_Request.Prop.PosInBackpackY;
            }
            if (mItem.data.GameUserId != mPlayer.GameUserId)
            {
                mChangeData = true;
                mItem.data.GameUserId = mPlayer.GameUserId;
            }
            if (mChangeDataPos)
            {
                mStallComponent.mItemBox.RemoveItem(mItem.ConfigData.X, mItem.ConfigData.Y, posX, posY);
                mStallComponent.mItemBox.AddItem(mItem.ConfigData.X, mItem.ConfigData.Y, mItem.data.posX, mItem.data.posY);

                mWriteDataComponent.Save(mItem.data, dBProxy).Coroutine();
            }
            else if (mChangeData)
            {
                mWriteDataComponent.Save(mItem.data, dBProxy).Coroutine();
            }

            if ((oldPrice.Item1 != b_Request.Prop.Price) ||
                (oldPrice.Item2 != b_Request.Prop.Price2))
            {
                mData.StallItemlist[b_Request.Prop.ItemUUID] = (b_Request.Prop.Price, b_Request.Prop.Price2);
                mData.Serialize();
                mWriteDataComponent.Save(mData, dBProxy).Coroutine();
            }

            b_Reply(b_Response);
            return true;
        }
    }
}