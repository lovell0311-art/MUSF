using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_QuickPurchaseRequestHandler : AMActorRpcHandler<C2G_QuickPurchaseRequest, G2C_QuickPurchaseResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_QuickPurchaseRequest b_Request, G2C_QuickPurchaseResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_QuickPurchaseRequest b_Request, G2C_QuickPurchaseResponse b_Response, Action<IMessage> b_Reply)
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
            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gameplayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            if (gameplayer.CurrentMap.MapId == 111)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(780);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("试炼之地无法打开仓库和商店");
                b_Reply(b_Response);
                return false;
            }
            MapComponent mapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(1);
            NpcComponent npcComponent = mapComponent.GetCustomComponent<NpcComponent>();
            if (mapComponent == null || npcComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                b_Reply(b_Response);
                return false;
            }
            foreach (var NpcInfo in npcComponent.AllNpcDic)
            {
                if (NpcInfo.Value.Config.Id == 10001)
                {
                    //if (npcComponent.AllNpcDic.TryGetValue((NpcInfo.Value.Id, out GameNpc npc))
                    foreach(var item in NpcInfo.Value.ShopComponent.mItemDict)
                    {
                        if (item.Value.ConfigID == b_Request.ItemConfigId)
                        {
                            //检查玩家是否有足够的金钱
                            var PlayerData = mPlayer.GetCustomComponent<GamePlayer>().Data;
                            int money = item.Value.GetProp(EItemValue.BuyMoney) * b_Request.Cnt;
                            if (PlayerData.GoldCoin >= money)
                            {
                                for (int i = 0; i < b_Request.Cnt; ++i)
                                {
                                    //检查玩家是否有足够的空间
                                    BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
                                    int posX = 0, posY = 0;
                                    if (backpack.mItemBox.CheckStatus(item.Value.ConfigData.X, item.Value.ConfigData.Y, ref posX, ref posY))
                                    {
                                        Item cloneItem = NpcInfo.Value.ShopComponent.CreateShopItem(NpcInfo.Value.ShopComponent.mShopConfigDict[item.Value.ItemUID], mPlayer.GameAreaId);
                                        if (backpack.AddItem(cloneItem, $"在NPC商店购买 NpcConfigId={NpcInfo.Value.Config.Id}", true))
                                        {
                                            //PlayerData.GoldCoin -= shopItem.GetProp(EItemValue.BuyMoney);
                                            mPlayer.GetCustomComponent<GamePlayer>().UpdateCoin(E_GameProperty.GoldCoin, -item.Value.GetProp(EItemValue.BuyMoney), $"在NPC商店购买 NpcConfigId={NpcInfo.Value.Config.Id}");

                                            //保存数据库
                                            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                                            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                                            mWriteDataComponent.Save(PlayerData, dBProxy2).Coroutine();

                                            var notice = new G2C_BackpackGoldChange_notice() { GameUserId = mPlayer.GameUserId, Gold = PlayerData.GoldCoin };
                                            mPlayer.Send(notice);
                                        }
                                        else
                                        {
                                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家背包空间不足!");
                                            b_Reply(b_Response);
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家背包空间不足!");
                                        b_Reply(b_Response);
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家金币不足!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                        //else
                        //{
                        //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1800);
                        //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("商店没有此商品!");
                        //    b_Reply(b_Response);
                        //    return false;
                        //}
                    }
                    //else
                    //{
                    //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1801);
                    //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("找不到NPC!");
                    //    b_Reply(b_Response);
                    //    return false;
                    //}
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}