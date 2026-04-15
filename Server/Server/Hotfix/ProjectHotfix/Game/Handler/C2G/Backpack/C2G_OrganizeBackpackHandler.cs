
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OrganizeBackpackHandler : AMActorRpcHandler<C2G_OrganizeBackpack,
        G2C_OrganizeBackpack>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OrganizeBackpack b_Request, G2C_OrganizeBackpack b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OrganizeBackpack b_Request, G2C_OrganizeBackpack b_Response, Action<IMessage> b_Reply)
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

            BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpack == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }

            {
                // 检查是否多物品或者少物品
                if(b_Request.ItemsNewPosition.Count != backpack.mItemDict.Count)
                {
                    // 参数错误，传入的物品数量不对
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(740);
                    b_Reply(b_Response);
                    return false;
                }

                List<long> itemUidList = backpack.mItemDict.Keys.ToList();

                using ListComponent<long> itemUidList2 = ListComponent<long>.Create();
                foreach(var itemNewPos in b_Request.ItemsNewPosition)
                {
                    itemUidList2.Add(itemNewPos.ItemUID);
                }

                if(itemUidList.Intersect(itemUidList2).Count() != itemUidList.Count)
                {
                    // 参数错误，传入的物品Uid无法对应
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(741);
                    b_Reply(b_Response);
                    return false;
                }

                using ItemsBoxStatus itemBox = backpack.mItemBox.Clone();
                // 需要考虑到有格子锁，不能直接用Clear
                foreach(Item item in backpack.mItemDict.Values)
                {
                    itemBox.RemoveItem(item.ConfigData.X, item.ConfigData.Y, item.data.posX, item.data.posY);
                }

                foreach (var itemNewPos in b_Request.ItemsNewPosition)
                {
                    Item item = backpack.mItemDict[itemNewPos.ItemUID];
                    if (!itemBox.AddItem(item.ConfigData.X,item.ConfigData.Y,itemNewPos.PosInBackpackX,itemNewPos.PosInBackpackY))
                    {
                        // 参数错误，新坐标无法放下物品
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(742);
                        b_Reply(b_Response);
                        return false;
                    }
                }

                // 检查完成，可以整理
                // 有格子锁，只能移除后再添加
                foreach (Item item in backpack.mItemDict.Values)
                {
                    backpack.mItemBox.RemoveItem(item.ConfigData.X, item.ConfigData.Y, item.data.posX, item.data.posY);
                }

                foreach (var itemNewPos in b_Request.ItemsNewPosition)
                {
                    Item item = backpack.mItemDict[itemNewPos.ItemUID];
                    item.data.posX = itemNewPos.PosInBackpackX;
                    item.data.posY = itemNewPos.PosInBackpackY;
                    backpack.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, itemNewPos.PosInBackpackX, itemNewPos.PosInBackpackY);
                    item.OnlySaveDB();
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}