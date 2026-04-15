using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_RemoteOpenRequestHandler : AMActorRpcHandler<C2G_RemoteOpenRequest, G2C_RemoteOpenResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_RemoteOpenRequest b_Request, G2C_RemoteOpenResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_RemoteOpenRequest b_Request, G2C_RemoteOpenResponse b_Response, Action<IMessage> b_Reply)
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }
            var PlayerShop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (PlayerShop == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }

            if (!PlayerShop.GetPlayerShopState(DeviationType.MaxMonthlyCard))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2202);
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GetCustomComponent<GamePlayer>().CurrentMap.MapId == 111)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(780);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("试炼之地无法打开仓库和商店");
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.Type == 0)
            {
                MapComponent mapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(1);
                NpcComponent npcComponent = mapComponent.GetCustomComponent<NpcComponent>();
                long NpcID = 0;
                foreach (var Npc in npcComponent.AllNpcDic)
                {
                    if (Npc.Value.Config.Id == 10001)
                        NpcID = Npc.Key;
                }
                if (npcComponent.AllNpcDic.TryGetValue(NpcID, out GameNpc npc))
                {
                    foreach (var item in npc.ShopComponent.mItemDict)
                    {
                        ItemInBackpack_Status itemInBackpack_Status = new ItemInBackpack_Status();
                        var Info = npc.ShopComponent.Item2BackpackStatusData(item.Value);
                        itemInBackpack_Status.GameUserId = Info.GameUserId;
                        itemInBackpack_Status.ItemUID = Info.ItemUID;
                        itemInBackpack_Status.ConfigID = Info.ConfigID;
                        itemInBackpack_Status.Type = Info.Type;
                        itemInBackpack_Status.PosInBackpackX = Info.PosInBackpackX;
                        itemInBackpack_Status.PosInBackpackY = Info.PosInBackpackY;
                        itemInBackpack_Status.Width = Info.Width;
                        itemInBackpack_Status.Height = Info.Height;
                        itemInBackpack_Status.Quantity = Info.Quantity;
                        itemInBackpack_Status.ItemLevel = Info.ItemLevel;
                        b_Response.AllItems.Add(itemInBackpack_Status);

                        ItemAllProperty itemAllProperty = new ItemAllProperty();
                        var Info2 = item.Value.ToItemAllProperty();
                        itemAllProperty.ItemUUID = Info2.ItemUUID;
                        foreach (var Prop in Info2.PropList)
                        {
                            Property property = new Property(); 
                            property.PropID = Prop.PropID;
                            property.Value = Prop.Value;
                            itemAllProperty.PropList.Add(property);
                        }
                        foreach (var AE in Info2.ExecllentEntry)
                        {
                            AttrEntry attrEntry = new AttrEntry();
                            attrEntry.PropId = AE.PropId;
                            attrEntry.Level = AE.Level;
                            itemAllProperty.ExecllentEntry.Add(attrEntry);
                        }
                        b_Response.AllProperty.Add(itemAllProperty);
                    }
                }
                b_Response.Type = 0;
                b_Response.NpcId = NpcID;
                b_Reply(b_Response);
                return true;
            }
            else
            {
                b_Response.Type = 1;
                b_Response.NpcId = 10004;
                b_Reply(b_Response);
                return true;
            }
        }
    }
}