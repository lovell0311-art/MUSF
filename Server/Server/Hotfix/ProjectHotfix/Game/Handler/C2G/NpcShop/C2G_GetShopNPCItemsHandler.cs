using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetShopNPCItemsHandler : AMActorRpcHandler<C2G_GetShopNPCItems, G2C_GetShopNPCItems>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetShopNPCItems b_Request, G2C_GetShopNPCItems b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetShopNPCItems b_Request, G2C_GetShopNPCItems b_Response, Action<IMessage> b_Reply)
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
            MapComponent mapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(mPlayer.GetCustomComponent<GamePlayer>().UnitData.Index);
            NpcComponent npcComponent = mapComponent.GetCustomComponent<NpcComponent>();
            if (npcComponent.AllNpcDic.TryGetValue(b_Request.NPCUUID, out GameNpc npc))
            {
                foreach (var item in npc.ShopComponent.mItemDict)
                {
                    b_Response.AllItems.Add(npc.ShopComponent.Item2BackpackStatusData(item.Value));
                    b_Response.AllProperty.Add(item.Value.ToItemAllProperty());
                }
            }
            else {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1801);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("找不到NPC!");
                b_Reply(b_Response);
                return false;
            }
            b_Reply(b_Response);
            return true;
        }
    }
}