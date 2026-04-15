using ETModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using System.Numerics;
using CustomFrameWork.Component;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using TencentCloud.Dc.V20180410.Models;
using Aop.Api.Domain;
using System.Net;
using TencentCloud.Ccc.V20200210.Models;

namespace ETHotfix
{
    public static class BattleCopyRoomSystem
    {

        public static bool JoinRoom(this BattleCopyRoom self, long gameUserId, string name)
        {
            if (self.playerDic.Count >= 10) return false;

            self.playerDic.Add(gameUserId, name);
            return true;
        }
        public static async Task CreateLvMob(this BattleCopyRoom self, int Level, int MobId, int X, int Y)
        {
            int RR = new Random().Next(1, 21);//20分之1的概率随机一个boos
            //RR = 10;
            if (RR == 10)
            {
                var mMapComponent = self.mapComponent;
                var Bat = self.mapComponent.GetCustomComponent<EnemyComponent>();
                var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;

                if (mJsonDic.TryGetValue(MobId, out var mEnemyConfig))
                {
                    var mEnemy = Root.CreateBuilder.GetInstance<Enemy>(false);
                    mEnemy.Awake(Bat);

                    string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(mEnemyConfig);
                    Enemy_InfoConfig enemy_InfoConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<Enemy_InfoConfig>(jsonStr);
                    enemy_InfoConfig.Lvl = enemy_InfoConfig.Lvl * Level;
                    enemy_InfoConfig.HP = enemy_InfoConfig.HP * Level;
                    enemy_InfoConfig.MP = enemy_InfoConfig.MP * Level;
                    enemy_InfoConfig.DmgMin = enemy_InfoConfig.DmgMin * Level;
                    enemy_InfoConfig.DmgMax = enemy_InfoConfig.DmgMax * Level;
                    enemy_InfoConfig.Def = enemy_InfoConfig.Def * Level;
                    enemy_InfoConfig.AttRate = enemy_InfoConfig.AttRate * Level ;
                    enemy_InfoConfig.BloRate = enemy_InfoConfig.BloRate * Level ;

                    //enemy_InfoConfig.DropDicDic = new Dictionary<int, int>(mEnemyConfig.DropDicDic);
                    enemy_InfoConfig.AttackTypeDic = new Dictionary<int, int>(mEnemyConfig.AttackTypeDic);
                    enemy_InfoConfig.AttackTypeDicSum = mEnemyConfig.AttackTypeDicSum;
                    enemy_InfoConfig.Ran2 = mEnemyConfig.Ran2;

                    mEnemy.SetConfig(enemy_InfoConfig);
                    mEnemy.SetInstanceId(mEnemy.Id);
                    mEnemy.AfterAwake();
                    mEnemy.AwakeSkill();
                    mEnemy.DataUpdateSkill();

                    mEnemy.SourcePosX = X;
                    mEnemy.SourcePosY = Y;

                    Bat.AllEnemyDic[mEnemy.InstanceId] = mEnemy;

                    var mFindTheWay = mMapComponent.GetFindTheWay2D(mEnemy.SourcePosX, mEnemy.SourcePosY);
                    if (mFindTheWay == null)
                    {
                        throw new Exception($"怪物出生点为 null MapId:{Bat.Parent.MapId} {X} {Y} id:{mEnemy.InstanceId}");
                    }
                    else if (mFindTheWay.IsStaticObstacle)
                    {
                        Log.Warning($"怪物出生点是障碍物 MapId:{Bat.Parent.MapId} {X} {Y} id:{mEnemy.InstanceId}");
                    }

                    mMapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);
                }
            }
        }
        public static async Task StateGameAsync(this BattleCopyRoom self)
        {
            if ((self.playerDic.Count == 0)) return;

            //int mapId = self.Parent.DecisionTransferPointId(self.type);
            
            //MapManageComponent mapManageComponent = Root.MainFactory.GetCustomComponent<MapManageComponent>();
            //MapComponent mapComponent = mapManageComponent.Copy(mapId);
            //if (mapComponent == null)
            //{
            //    Log.Error($"查找不到副本地图");
            //    return;
            //}
            //self.mapComponent = mapComponent;
            int level = self.level;

            switch ((CopyType)self.type)
            {
                case CopyType.DemonSquae:
                    {
                        var mJsonMonsterDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleCopy_MonsterConfigJson>().JsonDic;
                        mJsonMonsterDic.TryGetValue(level, out BattleCopy_MonsterConfig config);
                        await self.CreateLvMob(level, EMobInfo.MobConfig, EMobInfo.X, EMobInfo.Y);
                        self.mapComponent.monsterId = config.Number1[0];
                        self.mapComponent.GetCustomComponent<EnemyComponent>().InitMapEnemy(config.Number1[0], self.level, config.Number1[1]);

                        await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(180000);
                        self.mapComponent.monsterId = config.Number2[0];
                        self.mapComponent.GetCustomComponent<EnemyComponent>().InitMapEnemy(config.Number2[0], self.level, config.Number2[1]);

                        await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(180000);
                        self.mapComponent.monsterId = config.Number3[0];
                        self.mapComponent.GetCustomComponent<EnemyComponent>().InitMapEnemy(config.Number3[0], self.level, config.Number3[1]);

                        await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(240000);
                        self.mapComponent.monsterId = config.Number4[0];
                        self.mapComponent.GetCustomComponent<EnemyComponent>().InitMapEnemy(config.Number4[0], self.level, config.Number4[1]);

                        break;
                    }
                case CopyType.RedCastle:
                    {
                        int count = self.playerDic.Count;
                        self.round++;
                        self.needNum = 40 * count;
                        if (self.needNum > 200) self.needNum = 200;
                        if (self.needNum > 0)
                        {
                            self.NotifyRoomsChange(new G2C_CopyRoomKillMonster_notice()
                            {
                                KilledNumber = self.currentNum,
                                MaxNumber = self.needNum
                            });
                        }
                        self.mapComponent.GetCustomComponent<EnemyComponent>().InitMapEnemy(self.level);
                        await self.CreateLvMob(level, RMobInfo.MobConfig, RMobInfo.X, RMobInfo.Y);
                        break;
                    }
                    /*case CopyType.AncientBattlefield:
                        {
                            //int count = self.playerDic.Count;
                            //self.needNum = 40 * count;
                            //if (self.needNum > 200) self.needNum = 200;

                            self.mapComponent.GetCustomComponent<EnemyComponent>().InitMapEnemy();
                        }
                        break;*/

            }

        }

        public static void CalculateDemonSquareRank(this BattleCopyRoom self)
        {
            Dictionary<long, CopyRankData> rankDic = new Dictionary<long, CopyRankData>(10);
            foreach (var data in self.playerDic)
            {
                long userId = data.Key;
                rankDic.Add(userId, self.Parent.copyRankDataDic[userId]);
            }
            int rank = 0;
            rankDic = rankDic.OrderBy(u => u.Value.Score).ToDictionary(p => p.Key, o => o.Value);
            var config = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleCopy_RankConfigJson>().JsonDic;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            int gameAreaId = self.Parent.Parent.Parent.GameAreaId;
            DBProxyComponent dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, gameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(gameAreaId);

            G2C_BattleCopyRanks_notice message = new G2C_BattleCopyRanks_notice();
            message.BatteCopyRankDatas = new RepeatedField<BatteCopyRankData>();

            for (int i = 0; i < rankDic.Count; i++)
            {
                rank++;
                long userId = rankDic.ElementAt(i).Key;
                CopyRankData copyRankData = rankDic.ElementAt(i).Value;
                int sumExp = copyRankData.Exp;
                int sumScore = copyRankData.Score;
                int sumCoin = copyRankData.Coin;

                int multiple = config[rank].Multiple;
                int addExp = sumScore * self.Parent.config.EXPRate * multiple;
                int addCoin = sumScore * self.Parent.config.CoinRate * multiple;

                Player player = self.Parent.players[userId];
                GamePlayer mGameplayer = player.GetCustomComponent<GamePlayer>();
                mGameplayer.AddExprience(addExp);

                mGameplayer.UpdateCoin(E_GameProperty.GoldCoin, addCoin, "不知道是啥");
                mWriteDataComponent.Save(mGameplayer.Data, dBProxy).Coroutine();

                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                mGoldCoinData.Key = (int)E_GameProperty.GoldCoinChange;
                mGoldCoinData.Value = addCoin;
                mChangeValueMessage.Info.Add(mGoldCoinData);
                player.Send(mChangeValueMessage);

                int finalExp = sumExp + addExp;
                int finalCoin = sumCoin + addCoin;

                message.BatteCopyRankDatas.Add(new BatteCopyRankData()
                {
                    GameUserId = userId,
                    Rank = rank,
                    Score = sumScore,
                    Coin = finalCoin,
                    EXP = finalExp,
                    Name = self.playerDic[userId],
                });
            }

            if (message.BatteCopyRankDatas.Count <= 0)
            {
                //Log.Error("发送副本排行榜数据异常");
                return;
            }

            //封装排行榜数据发送给玩家
            foreach (var data in self.playerDic)
            {
                Player player = self.Parent.players[data.Key];
                player.Send(message);
            }
        }
        public static void DleSend(this BattleCopyRoom self, long GameuserID)
        {
            long timer = Help_TimeHelper.GetCurrenTimeStamp();

            if (self.type == 4) return;

            int totalExp = 0;
            Dictionary<int, BattleCopy_RedCastleConfig> mJsonOpenDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleCopy_RedCastleConfigJson>().JsonDic;
            mJsonOpenDic.TryGetValue(self.level, out BattleCopy_RedCastleConfig config);
            if (config == null)
            {
                Log.Error("血色城堡结算时，房间找不到对应等级经验配置系数");
                return;
            }

            CopyRoomState state = (CopyRoomState)self.round;
            if (state >= CopyRoomState.wave2Over) totalExp += config.Door;
            if (state >= CopyRoomState.CommitWeapon) totalExp += config.Crystal;
            if (state >= CopyRoomState.Accomplish)
            {
                totalExp += config.Weapon;

                int second = Help_TimeHelper.GetSecond(timer, self.Parent.endTimer);
                if (second > 0)
                {
                    int addExp = second *= config.Time;
                    totalExp += addExp;
                }
            }

            int finallyExp = totalExp * self.Parent.config.EXPRate;
            int finallyCoin = totalExp * self.Parent.config.CoinRate;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            int gameAreaId = self.Parent.Parent.Parent.GameAreaId;
            DBProxyComponent dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, gameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(gameAreaId);

            G2C_RedCastleSettlement_notice message = new G2C_RedCastleSettlement_notice();
            message.State = 0;
            message.BatteCopyRankDatas = new RepeatedField<BatteCopyRankData>() { new BatteCopyRankData()
            {
                EXP = finallyExp,
                Coin =finallyCoin,
            }};

            Player player = self.Parent.players[GameuserID];
            GamePlayer mGameplayer = player.GetCustomComponent<GamePlayer>();
            BackpackComponent backpackCpt = player.GetCustomComponent<BackpackComponent>();
            Item mayaGemstone = ItemFactory.Create(280001, player.GameAreaId);

            mGameplayer.AddExprience(finallyExp);
            mGameplayer.UpdateCoin(E_GameProperty.GoldCoin, finallyCoin, "血色城堡副本奖励");
            mWriteDataComponent.Save(mGameplayer.Data, dBProxy).Coroutine();


            player.Send(message);

            self.Parent.players.Remove(GameuserID);
            self.playerDic.Remove(GameuserID);
        }


        public static void QuitRoom(this BattleCopyRoom self, long gameUserId)
        {
            if (!self.playerDic.ContainsKey(gameUserId)) return;

            self.playerDic.Remove(gameUserId);

            if (self.AttackList.Contains(gameUserId))
            {
                self.AttackList.Remove(gameUserId);
            }
        }

        public static void ClearRoom(this BattleCopyRoom self)
        {
            if (self.playerDic.Count == 0) return;
            long key = self.playerDic.ElementAt(0).Key;
            Player player = self.Parent.players[key];
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(player.SourceGameAreaId);
            if (mServerArea == null)
            {
                Log.Error("数据库异常 找不到区分 无法复活");
                return;
            }


            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            List<long> UserList = new List<long>(self.playerDic.Keys);
            for (int i = 0 ,len = self.playerDic.Count; i < len; i++)
            {
                long userId = UserList[i];
                //_ = self.Parent.QuitMap(userId, mDBProxyManagerComponent, mServerArea);
                self.Parent.QuitRoom(userId);//退出地图前检查天使大剑
                self.Parent.QuitMap(userId, mDBProxyManagerComponent, mServerArea);
                self.Parent.RemoveUser(userId);
            }

            self.mapComponent.Dispose();
            self.playerDic.Clear();
            self.AttackList.Clear();
            self.needNum = 0;
            self.currentNum = 0;
            self.IsLock = false;
            self.DoorNum = 0;
            self.CoffinNum = 0;
            self.round = 0;
        }

        public static void RecordKilledMonsters(this BattleCopyRoom self, long userId, long monsterId)
        {
            if (self.IsLock) return;
            //如果是已经攻破大门
            if ((CopyRoomState)self.round == CopyRoomState.wave2Over)
            {
                if (monsterId != 160 && monsterId != 166 && monsterId != 172 && monsterId != 178 && monsterId != 184 && monsterId != 190
                    && monsterId != 196)
                {
                    return;   //不是骷灵法师 直接退出
                }
            }

            if (!self.AttackList.Contains(userId))
            {
                self.AttackList.Add(userId);
            }

            self.currentNum++;
            if (self.currentNum >= self.needNum)
            {
                self.IsLock = true;

                self.round++;

                if ((CopyRoomState)self.round == CopyRoomState.DoorBroken)
                {
                    self.DoorNum = 20 * self.AttackList.Count;
                    if (self.DoorNum > 100) self.DoorNum = 100;
                    self.AttackList.Clear();
                    G2C_NumberOfttacks_notice g2C_NumberOfttacks_Notice = new G2C_NumberOfttacks_notice();
                    g2C_NumberOfttacks_Notice.State = 0;
                    g2C_NumberOfttacks_Notice.Cnt = self.DoorNum;
                    self.NotifyRoomsChange(g2C_NumberOfttacks_Notice);
                }
                else if ((CopyRoomState)self.round == CopyRoomState.CrystalBroken)
                {
                    self.CoffinNum = 20 * self.AttackList.Count;
                    if (self.CoffinNum > 100) self.CoffinNum = 100;
                    self.AttackList.Clear();
                    G2C_NumberOfttacks_notice g2C_NumberOfttacks_Notice = new G2C_NumberOfttacks_notice();
                    g2C_NumberOfttacks_Notice.State = 1;
                    g2C_NumberOfttacks_Notice.Cnt = self.CoffinNum;
                    self.NotifyRoomsChange(g2C_NumberOfttacks_Notice);
                }
            }
            if (self.needNum > 0)
            {
                self.NotifyRoomsChange(new G2C_CopyRoomKillMonster_notice()
                {
                    KilledNumber = self.currentNum,
                    MaxNumber = self.needNum
                });
            }
        }
        /// <summary>
        /// 检查副本状态，判定对大门和水晶棺的攻击是否有效
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool AttackCopy1bstacle(this BattleCopyRoom self, int Num, out bool EndNum)
        {
            bool Value = false;
            if (self.currentNum >= self.needNum)
            {
                if ((CopyRoomState)self.round == CopyRoomState.DoorBroken)
                {
                    if (Num >= self.DoorNum)
                    {
                        self.DoorNum = 0;
                        Value = true;
                        self.round++;//SetCopyState();
                        foreach (long i in self.playerDic.Keys)
                        {
                            Player player = self.Parent.players[i];
                            ETModel.EventType.BreakThroughTheGate.Instance.player = player;
                            ETModel.EventType.BreakThroughTheGate.Instance.transferPointId = 0;
                            Root.EventSystem.OnRun("GamePlayerBreakThroughTheGate", ETModel.EventType.BreakThroughTheGate.Instance);
                        }
                    }
                }
                if ((CopyRoomState)self.round == CopyRoomState.CrystalBroken)
                {
                    if (Num >= self.CoffinNum)
                    {
                        self.CoffinNum = 0;
                        Value = true;
                        self.round++;
                    }
                }

                EndNum = Value;
                return true;
            }
            EndNum = false;
            return false;
        }
        /// <summary>
        /// 修改副本状态，用于攻破大门或者攻破水晶
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool SetCopyState(this BattleCopyRoom self, bool IsSubmit = false)
        {
            if ((CopyRoomState)self.round == CopyRoomState.DoorBroken)
                self.round++;
            if ((CopyRoomState)self.round == CopyRoomState.CrystalBroken && IsSubmit)
                self.round++;
            return true;
        }
        /// <summary>
        /// 检查需要攻击次数，判定是否进入下个状态
        /// </summary>
        /// <returns></returns>
        /*public static bool SetDoorNum(this BattleCopyRoom self,int Num)
        {
            if ((CopyRoomState)self.round == CopyRoomState.wave1Over && Num >= self.DoorNum)
            {
                self.DoorNum = 0;
                return true;
            }
            else if ((CopyRoomState)self.round == CopyRoomState.wave2Over && Num >= self.CoffinNum)
            {
                self.CoffinNum = 0;
                return true;
            }
            return false;
        }*/
        public static int AttackCopyObstacle(this BattleCopyRoom self, long userId)
        {
            //已经攻破大门
            if ((CopyRoomState)self.round == CopyRoomState.wave2Over)
            {
                //self.DoorNum--;
                if (self.DoorNum <= 0)
                {
                    self.DoorNum = 0;
                    //开始计算第二波怪物
                    self.currentNum = 0;
                    int number = self.playerDic.Count;
                    self.needNum = number * 2;
                    if (self.needNum > 10) self.needNum = 10;

                    self.NotifyRoomsChange(new G2C_CopyRoomStateUpdate_notice()
                    {
                        State = 1,
                    });

                    self.NotifyRoomsChange(new G2C_CopyRoomKillMonster_notice()
                    {
                        KilledNumber = self.currentNum,
                        MaxNumber = self.needNum
                    });
                    //self.round++;//= (int)CopyRoomState.DoorBroken;

                    self.IsLock = false;
                }
            }
            //攻破水晶棺材
            else if ((CopyRoomState)self.round == CopyRoomState.CommitWeapon)
            {
                //self.CoffinNum--;
                if (self.CoffinNum <= 0)
                {
                    self.CoffinNum = 0;
                    self.NotifyRoomsChange(new G2C_CopyRoomStateUpdate_notice()
                    {
                        State = 2,
                    });

                    /*Vector2 pos = new Vector2(); //临时 读取配置表
                    int x = 0;
                    int y = 0;
                    var mDropItem = Root.CreateBuilder.GetInstance<MapItem, int, int, int>(320304, x, y);
                    mDropItem.Count = 1;
                    Player player = self.Parent.players[userId];

                    var mMapComponent = self.mapComponent;
                    var mFindTheWay = mMapComponent.GetFindTheWay2D(x, y);
                    mMapComponent.SetObstacle(mFindTheWay, false);
                    //掉落物品ToDo
                    var mMapCellArea = mMapComponent.GetMapCellField(mFindTheWay);
                    mMapCellArea.AddDrop(mDropItem);

                    G2C_ItemDrop_notice mItemDrop_notice = new G2C_ItemDrop_notice();
                    G2C_ItemDropData mDropItemMessage = new G2C_ItemDropData();
                    mDropItemMessage.Key = mDropItem.InstanceId;
                    mDropItemMessage.Value = mDropItem.Count;
                    mDropItemMessage.PosX = mDropItem.X;
                    mDropItemMessage.PosY = mDropItem.Y;
                    mDropItemMessage.InstanceId = mDropItem.Id;
                    mItemDrop_notice.Info.Add(mDropItemMessage);
                    mMapComponent.SendNotice(mFindTheWay, mItemDrop_notice);
                    //self.round++;//= (int)CopyRoomState.CrystalBroken;*/
                }
            }
            return 0;
        }

        public static void NotifyRoomsChange(this BattleCopyRoom self, IActorMessage b_ActorMessage)
        {
            foreach (var data in self.playerDic)
            {
                long userId = data.Key;
                if (!self.Parent.players.ContainsKey(userId)) continue;
                Player player = self.Parent.players[userId];
                player.Send(b_ActorMessage);
            }
        }
        public static void RedBaoShi(this BattleCopyRoom self)
        {
            foreach (var data in self.playerDic)
            {
                Player player = self.Parent.players[data.Key];
                BackpackComponent backpackCpt = player.GetCustomComponent<BackpackComponent>();
                Item mayaGemstone = ItemFactory.Create(280001, player.GameAreaId);

                //判断宝石是否能加入到背包
                if (!backpackCpt.AddItem(mayaGemstone, "血色城堡副本奖励"))
                {
                    MailInfo mailinfo = new MailInfo();
                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                    mailinfo.MailName = "血色城堡奖励";
                    mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailinfo.MailContent = "背包空间不足";
                    mailinfo.MailState = 0;
                    mailinfo.ReceiveOrNot = 0;
                    mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    MailItem mailItem = new MailItem();
                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                    itemCreateAttr.Quantity = 1;
                    mailItem.ItemConfigID = 280001;
                    mailItem.ItemID = 0;
                    mailItem.CreateAttr = itemCreateAttr;
                    mailinfo.MailEnclosure.Add(mailItem);
                    MailSystem.SendMail(player.GameUserId, mailinfo, player.GameAreaId).Coroutine();
                }
            }
        }
    
        public static void RedCastleSettlement(this BattleCopyRoom self)
        {
            long timer = Help_TimeHelper.GetCurrenTimeStamp();

            int totalExp = 0;
            Dictionary<int, BattleCopy_RedCastleConfig> mJsonOpenDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleCopy_RedCastleConfigJson>().JsonDic;
            mJsonOpenDic.TryGetValue(self.level, out BattleCopy_RedCastleConfig config);
            if (config == null)
            {
                Log.Error("血色城堡结算时，房间找不到对应等级经验配置系数");
                return;
            }

            CopyRoomState state = (CopyRoomState)self.round;
            if (state >= CopyRoomState.wave2Over) totalExp += config.Door;
            if (state >= CopyRoomState.CommitWeapon) totalExp += config.Crystal;
            if (state >= CopyRoomState.Accomplish)
            {
                totalExp += config.Weapon;

                int second = Help_TimeHelper.GetSecond(timer, self.Parent.endTimer);
                if (second > 0)
                {
                    int addExp = second *= config.Time;
                    totalExp += addExp;
                }
            }

            int finallyExp = totalExp * self.Parent.config.EXPRate;
            int finallyCoin = totalExp * self.Parent.config.CoinRate;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            int gameAreaId = self.Parent.Parent.Parent.GameAreaId;
            DBProxyComponent dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, gameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(gameAreaId);

            G2C_RedCastleSettlement_notice message = new G2C_RedCastleSettlement_notice();
            message.State = 1;
            message.BatteCopyRankDatas = new RepeatedField<BatteCopyRankData>() { new BatteCopyRankData()
            {
                EXP = finallyExp,
                Coin =finallyCoin,
            } };

            //发送分数给房间玩家    
            foreach (var data in self.playerDic)
            {
                Player player = self.Parent.players[data.Key];
                GamePlayer mGameplayer = player.GetCustomComponent<GamePlayer>();
                //BackpackComponent backpackCpt = player.GetCustomComponent<BackpackComponent>();
                //Item mayaGemstone = ItemFactory.Create(280001, player.GameAreaId);

                //判断宝石是否能加入到背包
                //if (!backpackCpt.AddItem(mayaGemstone, "血色城堡副本奖励"))
                //{
                //    MailInfo mailinfo = new MailInfo();
                //    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                //    mailinfo.MailName = "血色城堡奖励";
                //    mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                //    mailinfo.MailContent = "背包空间不足";
                //    mailinfo.MailState = 0;
                //    mailinfo.ReceiveOrNot = 0;
                //    mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                //    MailItem mailItem = new MailItem();
                //    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                //    itemCreateAttr.Quantity = 1;
                //    mailItem.ItemConfigID = 280001;
                //    mailItem.ItemID = 0;
                //    mailItem.CreateAttr = itemCreateAttr;
                //    mailinfo.MailEnclosure.Add(mailItem);
                //    MailSystem.SendMail(player.GameUserId, mailinfo, gameAreaId).Coroutine();
                //    //Root.MainFactory.GetCustomComponent<MailComponent>().SendMail(player.GameUserId, mailinfo, gameAreaId);
                //}

                mGameplayer.AddExprience(finallyExp);

                mGameplayer.UpdateCoin(E_GameProperty.GoldCoin, finallyCoin, "血色城堡副本奖励");
                mWriteDataComponent.Save(mGameplayer.Data, dBProxy).Coroutine();

                player.Send(message);
            }
        }
    }
}
