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
    public class C2G_BattlePickUpDropItemRequestHandler : AMActorRpcHandler<C2G_BattlePickUpDropItemRequest, G2C_BattlePickUpDropItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BattlePickUpDropItemRequest b_Request, G2C_BattlePickUpDropItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_BattlePickUpDropItemRequest b_Request, G2C_BattlePickUpDropItemResponse b_Response, Action<IMessage> b_Reply)
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
            if (mGamePlayer.IsDeath)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1000);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("请等待复活后尝试拾取!");
                b_Reply(b_Response);
                return false;
            }

            MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
            if (mapComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(518);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }

            MapItem mapItem = mapComponent.GetMapEntityByInstanceId(b_Request.InstanceId) as MapItem;
            if (mapItem == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1014);
                b_Reply(b_Response);
                return false;
            }

            var mFindTheWay = mapComponent.GetFindTheWay2D(mGamePlayer);
            if ((new Vector2Int(mFindTheWay.X, mFindTheWay.Y) - mapItem.Position).sqrMagnitude > ConstData.DROP_MAXDISTANCE * ConstData.DROP_MAXDISTANCE)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1005);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("距离超过捡取距离,不能拾取!");
                b_Reply(b_Response);
                return false;
            }

            long mNowTimeTick = Help_TimeHelper.GetNowSecond();

            // 玩家不是击杀者
            if (mapItem.KillerId.Contains(mPlayer.GameUserId) == false)
            {
                if (mapItem.ProtectTick > mNowTimeTick)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1010);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("物品处于保护时间,非击杀者不能拾取!");
                    b_Reply(b_Response);
                    return false;
                }
            }

            long itemUid = 0;
            BackpackComponent mBackpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            // 如果捡取的是金币
            if (mapItem.ConfigId == 320294)
            {
                //mGamePlayer.Data.GoldCoin += mapItem.Count;
                mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, mapItem.Count, "捡的");

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(mGamePlayer.Data, dBProxy).Coroutine();

                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                mGoldCoinData.Key = (int)E_GameProperty.GoldCoinChange;
                mGoldCoinData.Value = mapItem.Count;
                mChangeValueMessage.Info.Add(mGoldCoinData);

                mGoldCoinData = new G2C_BattleKVData();
                mGoldCoinData.Key = (int)E_GameProperty.GoldCoin;
                mGoldCoinData.Value = mGamePlayer.Data.GoldCoin;
                mChangeValueMessage.Info.Add(mGoldCoinData);

                mPlayer.Send(mChangeValueMessage);
                mapItem.Dispose();
                ETModel.EventType.ChecJinBiChangeInBackpack.Instance.player = mPlayer;
                Root.EventSystem.OnRun("ChecJinBiChangeInBackpack", ETModel.EventType.ChecJinBiChangeInBackpack.Instance);
            }
            else if (mapItem.ConfigId == 320316)
            {
                if (!mPlayer.GetCustomComponent<PlayerShopMallComponent>().SetMiracleCoin(mapItem.Count,"地图拾取"))
                {
                    Log.Debug($"奇迹币增加失败 GameUserID{mPlayer.GameUserId} 奇迹币数量{mapItem.Count}");
                }
                mapItem.Dispose();
            }
            else if (mapItem.ConfigId == 320106)//失落的地图  捡起后不入背包，直接消耗掉
            {
                // 玩家不是击杀者或物品所属者,不能拾取失落的地图
                if (mapItem.Item != null && mapItem.Item.data.GameUserId != mPlayer.GameUserId)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1010);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("物品处于保护时间,非击杀者不能拾取!");
                    b_Reply(b_Response);
                    return false;
                }
                else if (mapItem.KillerId.Contains(mPlayer.GameUserId) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1010);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("物品处于保护时间,非击杀者不能拾取!");
                    b_Reply(b_Response);
                    return false;
                }
                mapItem.Dispose();
            }
            else if (mBackpackComponent.CanAddItem(mapItem.ConfigId, b_Request.PosInBackpackX, b_Request.PosInBackpackY))
            {
                Item item = null;
                if (mapItem.Item != null)
                {
                    if (mapItem.Item.GetProp(EItemValue.IsTask) == 1)
                    {
                        // 是任务物品
                        if (!GameTasksHelper.CanPickUpTaskItem(mPlayer, mapItem.Item))
                        {
                            // 任务已完成，无法拾取任务区物品
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1017);
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                 

                    item = mapItem.Item;
                    if(mapItem.OwnerGameUserId != 0 && mapItem.OwnerGameUserId != mPlayer.GameUserId)
                    {
                        DBTradeLog dBTradeLog = DBLogHelper.CreateDBTradeLog(mPlayer);
                        dBTradeLog.UserId = mapItem.OwnerUserId;
                        dBTradeLog.GameUserId = mapItem.OwnerGameUserId;
                        dBTradeLog.Str = $"#场景# 地面拾取玩家丢弃的物品 ({item.ToLogString()})";
                        dBTradeLog.ItemUid = item.ItemUID;
                        DBLogHelper.Write(dBTradeLog);
                    }

                    // 将物品置空，防止物品被释放
                    mapItem.SetItem(null);
                }
                else
                {
                    // 拾取
                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                    itemCreateAttr.Quantity = mapItem.Count;
                    itemCreateAttr.Level = mapItem.Level;
                    itemCreateAttr.OptListId = mapItem.OptListId;
                    itemCreateAttr.OptLevel = mapItem.OptLevel;
                    if ((mapItem.Quality & 1) == 1)
                    {
                        // 有技能
                        itemCreateAttr.HaveSkill = true;
                    }
                    if ((mapItem.Quality & 1 << 2) == 1 << 2)
                    {
                        // 有幸运
                        itemCreateAttr.HaveLucky = true;
                    }
                    if ((mapItem.Quality & 1 << 3) == 1 << 3)
                    {
                        // 有卓越
                        itemCreateAttr.CustomAttrMethod.Add("ItemAddExcAttr");
                    }
                    if ((mapItem.Quality & 1 << 4) == 1 << 4)
                    {
                        // 有套装
                        itemCreateAttr.SetId = mapItem.SetId;
                    }
                    if ((mapItem.Quality & 1 << 5) == 1 << 5)
                    {
                        // 有镶嵌
                        itemCreateAttr.CustomAttrMethod.Add("ItemAddSocketHoleCount");
                    }
                    item = ItemFactory.Create(mapItem.ConfigId, mPlayer.GameAreaId, itemCreateAttr);

                }
                mGamePlayer.SendItem(1, item,monsterId: mapItem.MonsterConfigId).Coroutine();//广播拾取道具
                mapItem.Dispose();
                List<int> list = new List<int>() { 270008, 270009, 270010, 270011, 270012, 270013 };
                if(list.Contains(item.ConfigID) && item.GetProp(EItemValue.FluoreAttr) == 0)
                {
                    var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
                    int Id = excAttrEntryManager.TryGetYingGuangBaoShi(item.ConfigID);
                    item.SetProp(EItemValue.FluoreAttr,Id);
                }

                //添加背包组件
                if (mBackpackComponent.AddItem(item, b_Request.PosInBackpackX, b_Request.PosInBackpackY, "地面拾取",true) == false)
                {
                    // 运行到这里，说明背包仓库代码已经失控了.
                    Log.Error($"拾取物品错误（地面 -> 背包）");
                    // 玩家反馈后，通过日志恢复物品
                    Log.PLog("Error", $"拾取物品错误（地面 -> 背包），mPlayer.UserId={mPlayer.UserId} mPlayer.GameUserId={mPlayer.GameUserId} item=({item.ToLogString()})");
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);//指定位置装不下物品
                    b_Reply(b_Response);
                    return false;
                }
                if(!item.IsDisposeable)
                {
                    itemUid = item.ItemUID;
                }
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1011);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包空间不足,不能拾取!");
                b_Reply(b_Response);
                return false;
            }


            await ETModel.ET.TimerComponent.Instance.WaitFrameAsync();
            b_Response.ItemUid = itemUid;
            b_Reply(b_Response);
            return true;
        }
        /// <summary>
        /// 失落的地图捡取判断
        /// </summary>
        /// <returns></returns>
        public static bool CheakKunDun(MapItem mapItem)
        {

            return false;
        }
    }


}