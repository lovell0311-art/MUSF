using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BaiTanLookLookRequestHandler : AMActorRpcHandler<C2G_BaiTanLookLookRequest, G2C_BaiTanLookLookResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BaiTanLookLookRequest b_Request, G2C_BaiTanLookLookResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }


        protected override async Task<bool> Run(C2G_BaiTanLookLookRequest b_Request, G2C_BaiTanLookLookResponse b_Response, Action<IMessage> b_Reply)
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

            C_Stall mStallComponent = null;
            if (mMapCellField.MapStallDic.TryGetValue(b_Request.BaiTanInstanceId, out mStallComponent) == false)
            {
                for (int i = 0, len = mMapCellField.AroundFieldArray.Length; i < len; i++)
                {
                    var mMapCellFieldTemp = mMapCellField.AroundFieldArray[i];

                    if (mMapCellFieldTemp.MapStallDic.TryGetValue(b_Request.BaiTanInstanceId, out mStallComponent))
                    {
                        break;
                    }
                }
            }
            if (mStallComponent == null)
            {
                // 查找不到你的摊位
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2109);
                b_Reply(b_Response);
                return false;
            }

            var mPlayerManageComponent = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();
            Player mTargetPlayer = null;
            for (int i = 0, len = mServerArea.VirtualIdlist.Count; i < len; i++)
            {
                int mTargetAreaId = mServerArea.VirtualIdlist[i] >> 16;
                mTargetPlayer = mPlayerManageComponent.Get(mTargetAreaId, b_Request.BaiTanInstanceId);
                if (mTargetPlayer != null)
                {
                    break;
                }
            }
            if (mTargetPlayer == null)
            {
                // 查找不到你的摊主
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2115);
                b_Reply(b_Response);
                return false;
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            DataCacheManageComponent mDataCacheComponent = mTargetPlayer.AddCustomComponent<DataCacheManageComponent>();

            var mDataCache_Stall = mDataCacheComponent.Get<DBStallItem>();
            if (mDataCache_Stall == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mTargetPlayer.GameAreaId);
                mDataCache_Stall = await HelpDb_DBStallItem.Init(mTargetPlayer, mDataCacheComponent, dBProxy2);
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

            if (mData.StallItemlist.Count > 0)
            {
                var mMapStallKey = mData.StallItemlist.Keys.ToArray();
                for (int j = 0, jlen = mMapStallKey.Length; j < jlen; j++)
                {
                    var mKey = mMapStallKey[j];
                    var mPrice = mData.StallItemlist[mKey];

                    if (mStallComponent.keyValuePairs.TryGetValue(mKey, out var mItem) == false)
                    {
                        mItem = ItemFactory.CreateFormDB(mKey, mTargetPlayer);
                        if (mItem == null)
                        {
                            continue;
                        }

                        mStallComponent.keyValuePairs[mKey] = mItem;
                    }
                    //mItem.SetProp(EItemValue.SellMoney, mPrice);
                    C2G_BaiTanItemMessage mBaiTanInfoMessage = new C2G_BaiTanItemMessage();
                    mBaiTanInfoMessage.ItemUUID = mItem.ItemUID;
                    mBaiTanInfoMessage.ConfigId = mItem.ConfigID;
                    mBaiTanInfoMessage.PosInBackpackX = mItem.data.posX;
                    mBaiTanInfoMessage.PosInBackpackY = mItem.data.posY;
                    mBaiTanInfoMessage.Price = mPrice.Item1;
                    mBaiTanInfoMessage.Price2 = mPrice.Item2;

                    b_Response.Prop.Add(mBaiTanInfoMessage);
                }
            }

            if (mData.StallItemlist.Count > 0)
            {
                var mMapStallKey = mData.StallItemlist.Keys.ToArray();
                for (int j = 0, jlen = mMapStallKey.Length; j < jlen; j++)
                {
                    var mKey = mMapStallKey[j];
                    var mPrice = mData.StallItemlist[mKey];

                    if (mStallComponent.keyValuePairs.TryGetValue(mKey, out var mItem))
                    {
                        mItem.SendAllPropertyData(mPlayer, ItemPropertyNotice.BaiTan);
                        mItem.SendAllEntryAttr(mPlayer, ItemPropertyNotice.BaiTan);
                    }
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}