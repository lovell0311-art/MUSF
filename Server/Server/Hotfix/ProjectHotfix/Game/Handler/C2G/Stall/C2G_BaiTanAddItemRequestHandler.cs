using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BaiTanAddItemRequestHandler : AMActorRpcHandler<C2G_BaiTanAddItemRequest, G2C_BaiTanAddItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BaiTanAddItemRequest b_Request, G2C_BaiTanAddItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_BaiTanAddItemRequest b_Request, G2C_BaiTanAddItemResponse b_Response, Action<IMessage> b_Reply)
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

            if (b_Request.Prop.Price < 0 || b_Request.Prop.Price > int.MaxValue || b_Request.Prop.Price2 < 0 || b_Request.Prop.Price2 > 999999)
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                b_Reply(b_Response);
                return false;
            }

            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);

            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            //从背包拿出物品
            var item = backpackComponent.GetItemByUID(b_Request.Prop.ItemUUID);
            if (item == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(807);
                b_Reply(b_Response);
                return false;
            }

            // TODO 物品状态限制 - 摆摊
            if (item.GetProp(EItemValue.IsBind) != 0)
            {
                // 绑定物品无法移到摊位
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3108);
                b_Reply(b_Response);
                return false;
            }
            if (item.GetProp(EItemValue.IsTask) != 0)
            {
                // 任务物品无法移到摊位
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3109);
                b_Reply(b_Response);
                return false;
            }
            if (item.GetProp(EItemValue.IsUsing) != 0)
            {
                // 使用中的物品无法移到摊位
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3110);
                b_Reply(b_Response);
                return false;
            }
            if (item.GetProp(EItemValue.IsLocking) != 0)
            {
                // 锁定的物品无法移到摊位
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3111);
                b_Reply(b_Response);
                return false;
            }
            if (item.GetProp(EItemValue.ValidTime) != 0)
            {
                // 时限的物品无法移到摊位
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3121);
                b_Reply(b_Response);
                return false;
            }
            if (mStallComponent.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, b_Request.Prop.PosInBackpackX, b_Request.Prop.PosInBackpackY) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                b_Reply(b_Response);
                return false;
            }

            backpackComponent.RemoveItem(item, "移到自己摊位");

            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            if (mStallComponent.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, b_Request.Prop.PosInBackpackX, b_Request.Prop.PosInBackpackY) == false)
            {
                // 不应该失败，如果失败了，就是数据有问题
                Log.Error($"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 移动物品到摊位失败!({item.ToLogString()})");
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                b_Reply(b_Response);
                return false;
            }

            if (mData.StallItemlist.ContainsKey(item.ItemUID) == false)
            {
                mData.StallItemlist[item.ItemUID] = (b_Request.Prop.Price, b_Request.Prop.Price2);
            }
            item.data.posX = b_Request.Prop.PosInBackpackX;
            item.data.posY = b_Request.Prop.PosInBackpackY;
            item.data.GameUserId = mPlayer.GameUserId;
            item.data.UserId = mPlayer.UserId;
            item.data.InComponent = EItemInComponent.Stall;

            mWriteDataComponent.Save(item.data, dBProxy).Coroutine();

            mData.Serialize();
            mWriteDataComponent.Save(mData, dBProxy).Coroutine();

            item.SendAllPropertyData(mPlayer, ItemPropertyNotice.BaiTan);
            item.SendAllEntryAttr(mPlayer, ItemPropertyNotice.BaiTan);
            b_Reply(b_Response);
            return true;
        }
    }
}