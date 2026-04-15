
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MoveBackpackItemRequestHandler : AMActorRpcHandler<C2G_MoveBackpackItemRequest,
        G2C_MoveBackpackItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MoveBackpackItemRequest b_Request, G2C_MoveBackpackItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_MoveBackpackItemRequest b_Request, G2C_MoveBackpackItemResponse b_Response, Action<IMessage> b_Reply)
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
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }


            //DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            //DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetComponent<DataCacheManageComponent>();
            //if (mDataCacheManageComponent == null) mDataCacheManageComponent = mPlayer.AddComponent<DataCacheManageComponent>();
            //var dataCache = mDataCacheManageComponent.Get<DBBackpackItem>();
            //if (dataCache == null)
            //{
            //    var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            //    dataCache = await mDataCacheManageComponent.Add<DBBackpackItem>(dBProxy2, p => p.GameUserId == mPlayer.GameUserId && p.IsDispose == 0);
            //}

            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }

            //检测是否有该物品
            //var delItems = dataCache.DataQuery(p => p.ItemData.ItemUID == b_Request.ItemUUID && 
            //p.IsDispose == 0);
            //if (delItems.Count > 0)
            if (backpackComponent.mItemDict.TryGetValue(b_Request.ItemUUID, out Item itemData) == false)
            {
                //b_Response.PropId = b_Request.ItemUUID;
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(703);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包找不到要删除的物品!ID:" + b_Request.ItemUUID);
                b_Reply(b_Response);
                return false;
            }
            if (backpackComponent.MoveItem(itemData, b_Request.PosInBackpackX, b_Request.PosInBackpackY) == false)
            {
                //b_Response.PropId = b_Request.ItemUUID;
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(708);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("移动物品失败!ID:" + b_Request.ItemUUID);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}