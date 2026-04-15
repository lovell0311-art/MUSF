using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BuyItemFromNPCShopHandler : AMActorRpcHandler<C2G_BuyItemFromNPCShop, G2C_BuyItemFromNPCShop>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BuyItemFromNPCShop b_Request, G2C_BuyItemFromNPCShop b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_BuyItemFromNPCShop b_Request, G2C_BuyItemFromNPCShop b_Response, Action<IMessage> b_Reply)
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

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

            var mPkNumber = mGamePlayer.GetNumerial(E_GameProperty.PkNumber);
            if (mPkNumber > 43200)
            {// 极恶
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1803);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }
            else if (mPkNumber > 21600)
            {// 红 
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1803);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }
            else if (mPkNumber > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1803);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }


            MapComponent mapComponent = null;
            NpcComponent npcComponent = null;

            if (b_Request.RemoteBuy == 0)
            {
                var PlayerShop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
                if (PlayerShop != null && !PlayerShop.GetPlayerShopState(DeviationType.MaxMonthlyCard))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2202);
                    b_Reply(b_Response);
                    return false;
                }
                mapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(1);
                npcComponent = mapComponent.GetCustomComponent<NpcComponent>();
            }
            else if (b_Request.RemoteBuy == 1)
            {
                mapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(mPlayer.GetCustomComponent<GamePlayer>().UnitData.Index);
                npcComponent = mapComponent.GetCustomComponent<NpcComponent>();
            }
            if (mapComponent == null || npcComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                b_Reply(b_Response);
                return false;
            }
            if (mapComponent.MapId == 111)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(780);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("试炼之地无法打开仓库和商店");
                b_Reply(b_Response);
                return false;
            }
            if (npcComponent.AllNpcDic.TryGetValue(b_Request.NPCUUID, out GameNpc npc))
            {
                if (npc.ShopComponent.mItemDict.TryGetValue(b_Request.ItemUUID, out Item shopItem))
                {
                    //检查玩家是否有足够的金钱
                    var PlayerData = mPlayer.GetCustomComponent<GamePlayer>().Data;
                    if (PlayerData.GoldCoin >= shopItem.GetProp(EItemValue.BuyMoney))
                    {
                        //检查玩家是否有足够的空间
                        BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
                        if (backpack.mItemBox.CheckStatus(shopItem.ConfigData.X, shopItem.ConfigData.Y, b_Request.PosInBackpackX, b_Request.PosInBackpackY))
                        {
                            Item cloneItem = npc.ShopComponent.CreateShopItem(npc.ShopComponent.mShopConfigDict[shopItem.ItemUID], mPlayer.GameAreaId);
                            // TODO 任务道具判断
                            switch (cloneItem.ConfigID)
                            {
                                case 320102:
                                case 320103:
                                case 320104:
                                case 320105:
                                case 320130:
                                case 320305:
                                case 320306:
                                case 320128:
                                case 320127:
                                case 320307:
                                    /*帝王之书
                                    断魂之剑
                                    精灵之泪
                                    先知之魂
                                    深渊之眼
                                    荣誉戒指
                                    暗黑之石
                                    丛林召唤者的角
                                    炽炎魔的火种
                                    天魔菲尼斯的羽毛
                                    */
                                    {
                                        // 是任务物品
                                        if (!GameTasksHelper.CanPickUpTaskItem(mPlayer, cloneItem))
                                        {
                                            // 无法购买，现在用不到此任务道具
                                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1805);
                                            b_Reply(b_Response);
                                            return false;
                                        }
                                        cloneItem.SetProp(EItemValue.IsTask, 1);
                                    }
                                    break;

                            }

                            if (backpack.AddItem(cloneItem, b_Request.PosInBackpackX, b_Request.PosInBackpackY, $"在NPC商店购买 NpcConfigId={npc.Config.Id}",true))
                            {
                                //PlayerData.GoldCoin -= shopItem.GetProp(EItemValue.BuyMoney);
                                mPlayer.GetCustomComponent<GamePlayer>().UpdateCoin(E_GameProperty.GoldCoin, -shopItem.GetProp(EItemValue.BuyMoney), $"在NPC商店购买 NpcConfigId={npc.Config.Id}");

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
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家金币不足!");
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1800);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("商店没有此商品!");
                        b_Reply(b_Response);
                        return false;
                    }
                }
                else
                {
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