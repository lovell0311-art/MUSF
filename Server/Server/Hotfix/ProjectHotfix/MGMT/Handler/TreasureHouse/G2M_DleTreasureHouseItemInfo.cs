using Aop.Api.Domain;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using ETModel.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using TencentCloud.Ecm.V20190719.Models;
using TencentCloud.Ssa.V20180608.Models;
using TencentCloud.Tci.V20190318.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_DleTreasureHouseItemInfoHandler : AMActorRpcHandler<G2M_DleTreasureHouseItemInfo, M2G_DleTreasureHouseItemInfo>
    {
        protected override async Task<bool> Run(G2M_DleTreasureHouseItemInfo b_Request, M2G_DleTreasureHouseItemInfo b_Response, Action<IMessage> b_Reply)
        {
            var component = Root.MainFactory.GetCustomComponent<MGMTTreasureHouse>();
            if (component == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }

            if (b_Request.Game == b_Request.UserID)
            {
                var Item = component.GetItem(b_Request.Uid, b_Request.MaxTyp, b_Request.MinTyp);
                if (Item != null)
                {
                    if (component.DelItem(Item))
                    {
                        b_Response.AreaId = Item.mAreaId;
                        b_Response.Uid = Item.Uid;
                        b_Response.ConfigID = Item.ConfigId;
                        b_Response.Cnt = Item.Cnt;
                        b_Reply(b_Response);
                        return true;
                    }
                }
            }
            else
            {
                if (component.CheckPlayerItemList(b_Request.Game, b_Request.Uid, out int Max, out int Min))
                {
                    var Item = component.GetItem(b_Request.Uid, Max, Min);
                    if (Item != null)
                    {
                        if (Item.Price <= b_Request.Crystal)
                        {
                            if (component.DelItem(Item))
                            {
                                int mAreaId = (int)(b_Request.AppendData >> 16);
                                M2G_SetPlayerTransactionRecord m2G_SetPlayerTransactionRecord = new M2G_SetPlayerTransactionRecord();
                                m2G_SetPlayerTransactionRecord.GameUserID = b_Request.Game;
                                m2G_SetPlayerTransactionRecord.MAreaId = mAreaId;
                                m2G_SetPlayerTransactionRecord.ItemList = new GMTreasureHouseItemInfo 
                                { 
                                    Uid = Item.Uid,
                                    UserID = Item.UserID,
                                    Name = Item.Name,
                                    Class = Item.Class,
                                    Excellent = Item.Excellent,
                                    Enhance = Item.Enhance,
                                    Readdition = Item.Readdition,
                                    Price = Item.Price,
                                    AreaId = Item.mAreaId,
                                    Cnt = Item.Cnt,
                                };

                                b_Response.AreaId = Item.mAreaId;
                                b_Response.Uid = Item.Uid;
                                b_Response.ConfigID = Item.ConfigId;
                                b_Response.Cnt = Item.Cnt;
                                b_Reply(b_Response);

                                if (await component.SetDBTHRecord(1, mAreaId, b_Request.Game, Item.UserID, Item.Uid, Item.Name, Item.Price))
                                    await component.SetDBTHRecord(0, Item.mAreaId, Item.UserID, b_Request.Game, Item.Uid, Item.Name, Item.Price);
                                
                                component.SendMessage(m2G_SetPlayerTransactionRecord);

                                // TODO 玩家行为 交易
                                async Task WriteTradeLog()
                                {
                                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                    var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);

                                    long userId = 0;
                                    long gameUserId = b_Request.Game;
                                    long targetUserId = 0;
                                    long targetGameUserId = Item.UserID;
                                    var dataList = await mDBProxy.Query<DBGamePlayerData>(p => p.Id == gameUserId);
                                    if (dataList != null || dataList.Count > 0)
                                    {
                                        DBGamePlayerData data = dataList[0] as DBGamePlayerData;
                                        userId = data.UserId;
                                    }
                                    dataList = await mDBProxy.Query<DBGamePlayerData>(p => p.Id == targetGameUserId);
                                    if (dataList != null || dataList.Count > 0)
                                    {
                                        DBGamePlayerData data = dataList[0] as DBGamePlayerData;
                                        targetUserId = data.UserId;
                                    }

                                    // TODO 交易魔晶
                                    DBTradeLog dBTradeLog = new DBTradeLog();
                                    dBTradeLog.UserId = userId;
                                    dBTradeLog.GameUserId = gameUserId;
                                    dBTradeLog.TargetUserId = targetUserId;
                                    dBTradeLog.TargetGameUserId = targetGameUserId;
                                    int Value1 = (int)Math.Round(Item.Price * 0.05f);
                                    if (Value1 <= 0) Value1 = 1;
                                    dBTradeLog.Yuanbao = Item.Price - Value1;
                                    dBTradeLog.Str = $"#藏宝阁# 向玩家交易魔晶";
                                    dBTradeLog.CreateTime = TimeHelper.Now();
                                    DBLogHelper.Write(dBTradeLog);
                                    // TODO 交易物品
                                    dBTradeLog = new DBTradeLog();
                                    dBTradeLog.UserId = targetUserId;
                                    dBTradeLog.GameUserId = targetGameUserId;
                                    dBTradeLog.TargetUserId = userId;
                                    dBTradeLog.TargetGameUserId = gameUserId;
                                    dBTradeLog.ItemUid = Item.Uid;
                                    dBTradeLog.Str = $"#藏宝阁# 向玩家交易物品 (uid={Item.Uid},id={Item.Cnt},name={Item.Name})";
                                    dBTradeLog.CreateTime = TimeHelper.Now();
                                    DBLogHelper.Write(dBTradeLog);
                                }
                                WriteTradeLog().Coroutine();
                                return true;
                            }
                            else
                            {
                                b_Response.Error = 3307;
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                        else
                        {
                            b_Response.Error = 3312;
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    else
                    {
                        b_Response.Error = 3311;
                        b_Reply(b_Response);
                        return false;
                    }
                }
                else
                {
                    b_Response.Error = 3307;
                    b_Reply(b_Response);
                    return false;
                }
            }
            b_Response.Error = 3307;
            b_Reply(b_Response);
            return true;
        }
    }
}