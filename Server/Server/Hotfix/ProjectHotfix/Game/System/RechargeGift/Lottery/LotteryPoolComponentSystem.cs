using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ETHotfix
{
    [EventMethod(typeof(LotteryPoolComponent), EventSystemType.INIT)]
    public class LotteryPoolComponentInitSystem : ITEventMethodOnInit<LotteryPoolComponent>
    {
        public void OnInit(LotteryPoolComponent self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(LotteryPoolComponent), EventSystemType.LOAD)]
    public class LotteryPoolComponentLoadSystem : ITEventMethodOnLoad<LotteryPoolComponent>
    {
        public override void OnLoad(LotteryPoolComponent self)
        {
            self.OnInit();
        }
    }

    [FriendOf(typeof(LotteryPoolComponent))]
    public static class LotteryPoolComponentSystem
    {
        public static void OnInit(this LotteryPoolComponent self)
        {
            self.giftPool.Clear();
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<Lottery_ItemInfoConfigJson>().JsonDic;
            foreach(Lottery_ItemInfoConfig config in jsonDic.Values)
            {
                self.giftPool.Add(config.Id, config.Weight);
            }
            if(self.giftPool.Count == 0)
            {
                Log.Error($"'Lottery_ItemInfoConfig' 配置错误");
            }
        }

        public static async Task<(int err,int isSendMail,List<long>ids)> StartLottery(this LotteryPoolComponent self,Player player,int count = 1)
        {
            // 参数错误，不支持的抽奖次数
            if (count <= 0) return (3500, 0,null);
            LotteryComponent lotteryCom = player.GetCustomComponent<LotteryComponent>();
            List<long> configIds = new List<long>();
            for(int i = 0; i < count; i++)
            {
                lotteryCom.Data.TotalCount += 1;
                if (lotteryCom.Data.TotalCount >= ConstLottery.LotteryCountMax)
                {
                    // 保底奖品
                    configIds.Add(1);
                    lotteryCom.Data.TotalCount = 0;
                }
                else
                {
                    self.giftPool.TryGetValue(out int id);
                    configIds.Add(id);
                }
            }
            lotteryCom.SaveDB();

            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<Lottery_ItemInfoConfigJson>().JsonDic;

            string nickName = player.GetCustomComponent<GamePlayer>().Data.NickName;
            long createTime = Help_TimeHelper.GetNow();
            using ListComponent<Item> allItem = ListComponent<Item>.Create();
            foreach(long id in configIds)
            {
                jsonDic.TryGetValue((int)id, out var info);
                Item item = ItemFactory.TryCreate(info.ItemId, player.GameAreaId, info.ToItemCreateAttr());
                if(item == null)
                {
                    player.PLog($"'Lottery_ItemInfoConfig' 配置错误，无法生成物品 id={id} info.ItemId={info.ItemId}");
                    Log.Error($"'Lottery_ItemInfoConfig' 配置错误，无法生成物品 id={id} info.ItemId={info.ItemId}");
                    continue;
                }
                allItem.Add(item);
                DBLotteryLog dBLotteryLog = new DBLotteryLog();
                dBLotteryLog.UserId = player.UserId;
                dBLotteryLog.GameUserId = player.GameUserId;
                dBLotteryLog.NickName = nickName;
                dBLotteryLog.CreateTime = createTime;
                dBLotteryLog.Desc = $"抽中了 {item.GetClientName()}";
                DBLogHelper.Write(dBLotteryLog,player.GameAreaId);
            }
            // 配置错误，抽奖物品丢失，请联系客服
            if (allItem.Count == 0) return (3502, 0,null);

            // TODO 添加物品到背包
            {
                BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();

                using ItemsBoxStatus.LockList lockList = ItemsBoxStatus.LockList.Create();
                using ListComponent<(int x, int y)> posList = ListComponent<(int x, int y)>.Create();
                List<ItemsBoxStatus.Lock> list1 = lockList;
                List<(int x, int y)> list2 = posList;
                if (backpack.CanAddItemManyAndLock(allItem, ref list1, ref list2))
                {
                    lockList.Dispose();
                    // TODO 添加到背包
                    for (int i = 0; i < allItem.Count; ++i)
                    {
                        Item item = allItem[i];
                        (int x, int y) pos = posList[i];
                        if (!backpack.AddItem(item, pos.x, pos.y, "抽奖获得"))
                        {
                            // 运行到这里，说明代码出问题了
                            player.PLog($"抽奖完成后，添加道具到背包失败! ({item.ToLogString()})");
                            Log.Error($"抽奖完成后，添加道具失败!");
                            continue;
                        }
                    }
                    return (0,0,configIds);
                }
            }

            // TODO 通过邮件发送
            {
                MailInfo mailInfo = new MailInfo();
                async Task SendMail()
                {
                    // 发送邮件
                    mailInfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                    mailInfo.MailName = "抽奖领取奖品通知";
                    mailInfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailInfo.MailContent = "<color=#FFFFFF00>缩进</color>由于您的背包无法放下抽奖获得的道具，我们将奖品通过邮件发送给您。请清理背包后领取！";
                    mailInfo.MailState = 0;
                    mailInfo.ReceiveOrNot = 0;
                    mailInfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    await MailSystem.SendMail(player.GameUserId, mailInfo, player.GameAreaId,"抽奖获取");
                }

                foreach (Item item in allItem)
                {
                    if (mailInfo == null) mailInfo = new MailInfo();

                    item.data.InComponent = EItemInComponent.Mail;
                    item.data.posX = 0;
                    item.data.posY = 0;
                    item.data.posId = 0;
                    item.data.GameUserId = player.GameUserId;
                    item.data.UserId = player.UserId;
                    await item.SaveDBAsync(player);
                    ItemFactory.DleData(item.ItemUID,player);

                    mailInfo.MailEnclosure.Add(item.ToMailItem());

                    if (mailInfo.MailEnclosure.Count >= 6)
                    {
                        // 一个邮件最多可以添加 6 个附件
                        await SendMail();
                        mailInfo = null;
                    }
                }
                if (mailInfo != null)
                {
                    await SendMail();
                    mailInfo = null;
                }
            }
            // 背包已满，奖品已通过邮件发送。请查看邮件!
            return (0,1,configIds);
        }

        public static async Task<List<LotteryLog>> GetHistoryLotteryLog(this LotteryPoolComponent self, Player player,long gameUserId,long startLogId,long count)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DBProxyComponent dBProxy = mDBProxyManager.GetZoneDB(DBType.Log, player.GameAreaId);
            using ListComponent<BsonDocument> pipeline = ListComponent<BsonDocument>.Create();

            long logId = (startLogId == 0) ? long.MaxValue : startLogId;

            BsonDocument match = null;
            if (gameUserId == 0)
            {
                match = new BsonDocument()
                {
                    { "_id" ,new BsonDocument("$lt",new BsonInt64(logId))}
                };

            }
            else
            {
                match = new BsonDocument()
                {
                    { "GameUserId" ,new BsonInt64(gameUserId)},
                    { "_id" ,new BsonDocument("$lt",new BsonInt64(logId))}
                };
            }

            // 降序
            BsonDocument sort = new BsonDocument()
            {
                { "_id",-1}
            };


            /*
                [
	                {$match:{
		                GameUserId:NumberLong("1722066759391707137"),
		                _id:{$lt:NumberLong("9223372036854775807")}
                    }},
                    {$sort:{_id:-1}},
                    {$limit:20}
                ]
            */
            pipeline.Add(new BsonDocument("$match", match));
            pipeline.Add(new BsonDocument("$sort", sort));
            pipeline.Add(new BsonDocument("$limit", count));

            List<LotteryLog> lotteryLogs = new List<LotteryLog>();
            List<BsonDocument> results = await dBProxy.Aggregate<DBLotteryLog>(pipeline);
            // 将结果转为升序
            for(int i = results.Count - 1; i >= 0;--i)
            {
                BsonDocument log = results[i];
                lotteryLogs.Add(new LotteryLog()
                {
                    LogId = log["_id"].AsInt64,
                    UserId = log["UserId"].AsInt64,
                    GameUserId = log["GameUserId"].AsInt64,
                    NickName = log["NickName"].AsString,
                    Desc = log["Desc"].AsString
                });
            }

            return lotteryLogs;
        }

        public static async Task<List<LotteryLog>> GetLotteryLog(this LotteryPoolComponent self, Player player, long gameUserId, long endLogId, long count)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DBProxyComponent dBProxy = mDBProxyManager.GetZoneDB(DBType.Log, player.GameAreaId);
            using ListComponent<BsonDocument> pipeline = ListComponent<BsonDocument>.Create();

            long logId = endLogId;

            BsonDocument match = null;
            if (gameUserId == 0)
            {
                match = new BsonDocument()
                {
                    { "_id" ,new BsonDocument("$gt",new BsonInt64(logId))}
                };

            }
            else
            {
                match = new BsonDocument()
                {
                    { "GameUserId" ,new BsonInt64(gameUserId)},
                    { "_id" ,new BsonDocument("$gt",new BsonInt64(logId))}
                };
            }

            // 升序
            BsonDocument sort = new BsonDocument()
            {
                { "_id", 1 }
            };


            /*
                [
	                {$match:{
		                GameUserId:NumberLong("1722066759391707137"),
		                _id:{$gt:NumberLong("0")}
                    }},
                    {$sort:{_id:1}},
                    {$limit:20}
                ]
            */
            pipeline.Add(new BsonDocument("$match", match));
            pipeline.Add(new BsonDocument("$sort", sort));
            pipeline.Add(new BsonDocument("$limit", count));

            List<LotteryLog> lotteryLogs = new List<LotteryLog>();
            List<BsonDocument> results = await dBProxy.Aggregate<DBLotteryLog>(pipeline);

            for (int i = 0; i < results.Count; ++i)
            {
                BsonDocument log = results[i];
                lotteryLogs.Add(new LotteryLog()
                {
                    LogId = log["_id"].AsInt64,
                    UserId = log["UserId"].AsInt64,
                    GameUserId = log["GameUserId"].AsInt64,
                    NickName = log["NickName"].AsString,
                    Desc = log["Desc"].AsString
                });
            }

            return lotteryLogs;
        }
    }
}
