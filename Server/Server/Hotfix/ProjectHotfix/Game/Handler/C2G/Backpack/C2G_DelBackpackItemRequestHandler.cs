using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;

using UnityEngine;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_DelBackpackItemRequestHandler : AMActorRpcHandler<C2G_DelBackpackItemRequest, G2C_DelBackpackItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_DelBackpackItemRequest b_Request, G2C_DelBackpackItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_DelBackpackItemRequest b_Request, G2C_DelBackpackItemResponse b_Response, Action<IMessage> b_Reply)
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
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            GamePlayer mGameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            /*if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mGameplayer.UnitData.Index, out var mMapComponent) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }*/
            MapComponent mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGameplayer.UnitData.Index, mPlayer.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }

            var mFindTheWay = mMapComponent.GetFindTheWay2D(b_Request.PosInSceneX, b_Request.PosInSceneY);
            if(mFindTheWay == null || mFindTheWay.IsStaticObstacle == true)
            {
                // 无法丢弃到目标点
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1016);
                b_Reply(b_Response);
                return false;
            }
            Vector2 vec = new Vector2(b_Request.PosInSceneX - mGameplayer.UnitData.X, b_Request.PosInSceneY - mGameplayer.UnitData.Y);
            if (vec.sqrMagnitude > 10 * 10)
            {
                // 距离超过丢弃距离
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1015);
                b_Reply(b_Response);
                return false;
            }
            
            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }

            // TODO 物品状态限制 - 丢弃
            Item item = backpackComponent.GetItemByUID(b_Request.ItemUUID);
            if(item != null)
            {
                if (item.GetProp(EItemValue.IsBind) != 0)
                {
                    // 绑定物品无法丢弃
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3100);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsTask) != 0)
                {
                    // 任务物品无法丢弃
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3101);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsUsing) != 0)
                {
                    // 使用中的物品无法丢弃
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3102);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsLocking) != 0)
                {
                    // 锁定的物品无法丢弃
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3103);
                    b_Reply(b_Response);
                    return false;
                }
                //交易限制取消丢弃也取消2024.12.19调整只有18的小月卡不让丢道具
                if (!mPlayer.GetCustomComponent<PlayerShopMallComponent>().GetPlayerShopState(DeviationType.MaxMonthlyCard))
                {
                    if (item.HaveExcellentOption() || item.HaveSetOption() || item.Type == EItemType.Pets || item.Type == EItemType.Wing || item.Type == EItemType.Mounts || item.Type == EItemType.Gemstone)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2522);
                        b_Reply(b_Response);
                        return false;
                    }

                    if (item.ConfigData.Sell == 1 && !item.IsArmor() && !item.IsWeapon() && !item.IsAccessory())
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3304);
                        b_Reply(b_Response);
                        return false;
                    }
                }
            }
            

            if (!backpackComponent.DiscardItemToGround(b_Request.ItemUUID, mMapComponent, b_Request.PosInSceneX, b_Request.PosInSceneY,EMapItemCreateType.Discard, "丢弃物品到地面"))
            {
                b_Response.Error = ErrorCodeHotfix.ERR_BackpackCantDeleteItemError;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包找不到要删除的物品!ID:" + b_Request.ItemUUID);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}