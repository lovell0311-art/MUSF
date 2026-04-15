using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {
        /// <summary>
        /// 加载掉落限制
        /// </summary>
        /// <param name="b_GamePlayer"></param>
        /// <returns></returns>
        public async static Task LoadingLimitation(this GamePlayer b_GamePlayer)
        {
            int state = 0;
            state = b_GamePlayer.Player.GetCustomComponent<PlayerShopMallComponent>().GetPlayerShopState(DeviationType.MaxMonthlyCard) ? 2 : 0;
            int CoinCnt = 750;
            if (!b_GamePlayer.Player.GetCustomComponent<PlayerTitle>().CheckTitle(60012))
                CoinCnt = 1500;

            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dateTime = dateTime.ToUniversalTime();
            // 获取时间戳（Unix 时间戳，以秒为单位）
            long timestamp = (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            b_GamePlayer.dBCharacterDroplimit = new DBCharacterDroplimit(state, CoinCnt, timestamp);
            b_GamePlayer.dBCharacterDroplimit.GameUserId = b_GamePlayer.Player.GameUserId;
            b_GamePlayer.dBCharacterDroplimit.Id = IdGeneraterNew.Instance.GenerateUnitId(b_GamePlayer.Player.GameAreaId);

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_GamePlayer.Player.GameAreaId);
            DataCacheManageComponent dataCacheManage = b_GamePlayer.Player.GetCustomComponent<DataCacheManageComponent>();
            var Droplimit = dataCacheManage.Get<DBCharacterDroplimit>();
            if (Droplimit == null)
            {
                Droplimit = await dataCacheManage.Add<DBCharacterDroplimit>(dBProxy2, p => p.GameUserId == b_GamePlayer.Player.GameUserId);
            }

            var Info = Droplimit.DataQuery(p => p.GameUserId == b_GamePlayer.Player.GameUserId);
            if (Info != null && Info.Count == 1)
            {
                b_GamePlayer.dBCharacterDroplimit = Info[0];
                if (b_GamePlayer.dBCharacterDroplimit.MinimumGuarantee == null)
                {
                    b_GamePlayer.dBCharacterDroplimit.MinimumGuarantee = new string("");
                    b_GamePlayer.dBCharacterDroplimit.MGLsit = new Dictionary<(int, int), int>();
                }
                if (b_GamePlayer.dBCharacterDroplimit.MinimumGuarantee != "")
                {
                    b_GamePlayer.dBCharacterDroplimit.MGLsit = b_GamePlayer.dBCharacterDroplimit.MinimumGuarantee.Split(';')
                                                                .Select(x => x.Split(',')).ToDictionary(x => (int.Parse(x[0]),
                                                                int.Parse(x[1])), x => int.Parse(x[2]));
                }

                //在线调整时间
                if (b_GamePlayer.dBCharacterDroplimit.MiracleCoinTime > 60000000000)
                {
                    b_GamePlayer.dBCharacterDroplimit.MiracleCoinCnt = CoinCnt;
                    b_GamePlayer.dBCharacterDroplimit.MiracleCoinTime = timestamp;
                }
            }
            else
            {
                Droplimit.DataAdd(b_GamePlayer.dBCharacterDroplimit);
                await dBProxy2.Save(b_GamePlayer.dBCharacterDroplimit);
            }    
            //b_GamePlayer.SetDropItemDB();

            b_GamePlayer.Player.AddCustomComponent<CheckDropItemTimeComponent>();
        }
        public static void SetDropItemDB(this GamePlayer b_GamePlayer)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_GamePlayer.Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_GamePlayer.Player.GameAreaId);
            mWriteDataComponent.Save(b_GamePlayer.dBCharacterDroplimit, dBProxy2).Coroutine();
        }
        public static void SetMGDropItem(this GamePlayer b_GamePlayer,(int,int) Id)
        {
            if (b_GamePlayer.dBCharacterDroplimit.MGLsit == null) 
                b_GamePlayer.dBCharacterDroplimit.MGLsit = new Dictionary<(int, int), int>();

            if (b_GamePlayer.dBCharacterDroplimit != null)
            {
                if (b_GamePlayer.dBCharacterDroplimit.MGLsit != null)
                {
                    if (b_GamePlayer.dBCharacterDroplimit.MGLsit.ContainsKey(Id))
                    {
                        b_GamePlayer.dBCharacterDroplimit.MGLsit[Id]++;
                    }
                    else
                    {
                        b_GamePlayer.dBCharacterDroplimit.MGLsit.Add(Id,1);
                    }
                    b_GamePlayer.dBCharacterDroplimit.MinimumGuarantee = string.Join(";", b_GamePlayer.dBCharacterDroplimit.MGLsit.Select(x => $"{x.Key.Item1},{x.Key.Item2},{x.Value}"));
                    b_GamePlayer.SetDropItemDB();
                }
            }
        }
        public static void BuyVipSetDrop(this GamePlayer b_GamePlayer)
        {
            var ShopInfo = b_GamePlayer.Player.GetCustomComponent<PlayerShopMallComponent>();
            long DBId = b_GamePlayer.dBCharacterDroplimit.Id;
            string Updata = b_GamePlayer.dBCharacterDroplimit.MinimumGuarantee;
            var Title = b_GamePlayer.Player.GetCustomComponent<PlayerTitle>();
            int state = 0;
            if (ShopInfo != null)
            {
                state += ShopInfo.GetPlayerShopState(DeviationType.MaxMonthlyCard) ? 2 : 0;
            }
            int CoinCnt = 750;
            if (Title != null  && !Title.CheckTitle(60012))
                CoinCnt = 1500;

            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dateTime = dateTime.ToUniversalTime();
            // 获取时间戳（Unix 时间戳，以秒为单位）
            long timestamp = (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            b_GamePlayer.dBCharacterDroplimit = new DBCharacterDroplimit(state, CoinCnt, timestamp);
            b_GamePlayer.dBCharacterDroplimit.Id = DBId;
            b_GamePlayer.dBCharacterDroplimit.GameUserId = b_GamePlayer.Player.GameUserId;
            if(Updata != "")
            b_GamePlayer.dBCharacterDroplimit.MGLsit = Updata.Split(';').Select(x => x.Split(',')).ToDictionary(x => (int.Parse(x[0]), int.Parse(x[1])),x => int.Parse(x[2]));
            b_GamePlayer.SetDropItemDB();
        }
        /// <summary>
        /// 检查掉落
        /// </summary>
        /// <param name="b_GamePlayer"></param>
        /// <param name="ItemConfig"></param>
        public static bool CheckDrop(this GamePlayer b_GamePlayer, MapItem item,int MobLv =0)
        {
            if (b_GamePlayer.dBCharacterDroplimit == null) return true;
            if (item == null) return true;
            if (b_GamePlayer.Data.Level >= 300 && MobLv <= 70) return false;
            if (b_GamePlayer.Data.Level >= 400 && MobLv <= 90) return false;
            if (b_GamePlayer.dBCharacterDroplimit.Restrict == 1) return false;
            long DTime = Help_TimeHelper.GetNowSecond();
            List<int> ints = new List<int>() { 320006,  320007, 320008, 320009, 320010, 260008, 320310,  320311, 320312, 320315, 320313, 320413 };
            //检查宝石不包含荧光宝石
            EItemType Type = (EItemType)(item.ConfigId / 10000);
            if (Type == EItemType.Gemstone)
            {
                if (b_GamePlayer.dBCharacterDroplimit.GemstoneCnt == -1) return true;
                if (b_GamePlayer.dBCharacterDroplimit.GemstoneCnt == 0)
                {
                    Log.Info($"Player:{b_GamePlayer.Player.GameUserId} Item:{item.ConfigId} 宝石掉落到达上限");
                    return false;
                }
                if (b_GamePlayer.dBCharacterDroplimit.GDGTime != 0 && DTime - b_GamePlayer.dBCharacterDroplimit.GDGTime < 0) return false;
                b_GamePlayer.dBCharacterDroplimit.GDGTime = DTime + 7200;
                b_GamePlayer.dBCharacterDroplimit.GemstoneCnt--;
                b_GamePlayer.SetDropItemDB();
                return true;
            }
            //检查羽毛
            if (item.ConfigId == 320297)
            {
                if (b_GamePlayer.dBCharacterDroplimit.FeatherCnt == -1) return true;
                if (b_GamePlayer.dBCharacterDroplimit.FeatherCnt == 0)
                {
                    Log.Info($"Player:{b_GamePlayer.Player.GameUserId} Item:{item.ConfigId} 羽毛掉落到达上限");
                    return false;
                }
                if(b_GamePlayer.dBCharacterDroplimit.FDGTime != 0 && DTime - b_GamePlayer.dBCharacterDroplimit.FDGTime<0 ) return false;
                b_GamePlayer.dBCharacterDroplimit.FDGTime = DTime + 7200;
                
                b_GamePlayer.dBCharacterDroplimit.FeatherCnt--;
                b_GamePlayer.SetDropItemDB();
                return true;
            }
            //藏宝图碎片
            if (ints.Contains(item.ConfigId))
            {
                if (b_GamePlayer.dBCharacterDroplimit.CangBaotuSPCnt == -1) return true;
                if (b_GamePlayer.dBCharacterDroplimit.CangBaotuSPCnt == 0)
                {
                    Log.Info($"Player:{b_GamePlayer.Player.GameUserId} Item:{item.ConfigId} 坐骑材料掉落到达上限");
                    return false;
                }
                if (b_GamePlayer.dBCharacterDroplimit.CBTDGTime != 0 && DTime - b_GamePlayer.dBCharacterDroplimit.CBTDGTime < 0) return false;
                b_GamePlayer.dBCharacterDroplimit.CBTDGTime = DTime + 7200;
                b_GamePlayer.dBCharacterDroplimit.CangBaotuSPCnt--;
                b_GamePlayer.SetDropItemDB();
                return true;
            }
            //困顿印记
            //if (item.ConfigId == 320107)
            //{
            //    if (b_GamePlayer.dBCharacterDroplimit.KunDunYingJiCnt == -1) return true;
            //    if (b_GamePlayer.dBCharacterDroplimit.KunDunYingJiCnt == 0)
            //    {
            //        Log.Info($"Player:{b_GamePlayer.Player.GameUserId} Item:{item.ConfigId} 困顿印记掉落到达上限");
            //        return false;
            //    }

            //    b_GamePlayer.dBCharacterDroplimit.KunDunYingJiCnt--;
            //    b_GamePlayer.SetDropItemDB();
            //    return true;
            //}
            //卓越装备数
            if ((item.Quality & 1 << 3) == 1 << 3)
            {
                if (b_GamePlayer.dBCharacterDroplimit.ExcellenceCnt == -1) return true;
                if (b_GamePlayer.dBCharacterDroplimit.ExcellenceCnt == 0)
                {
                    Log.Info($"Player:{b_GamePlayer.Player.GameUserId} Item:{item.ConfigId} 卓越装备掉落到达上限");
                    return false;
                }
                if (b_GamePlayer.dBCharacterDroplimit.EDGTime != 0 && DTime - b_GamePlayer.dBCharacterDroplimit.EDGTime < 0) return false;
                b_GamePlayer.dBCharacterDroplimit.EDGTime = DTime + 7200;
                b_GamePlayer.dBCharacterDroplimit.ExcellenceCnt--;
                b_GamePlayer.SetDropItemDB();
                return true;
            }
            //套装装备数
            if ((item.Quality & 1 << 4) == 1 << 4)
            {
                if (b_GamePlayer.dBCharacterDroplimit.SuitCnt == -1) return true;
                if (b_GamePlayer.dBCharacterDroplimit.SuitCnt == 0)
                {
                    Log.Info($"Player:{b_GamePlayer.Player.GameUserId} Item:{item.ConfigId} 套装装备掉落到达上限");
                    return false;
                }
                if (b_GamePlayer.dBCharacterDroplimit.SDGTime != 0 && DTime - b_GamePlayer.dBCharacterDroplimit.SDGTime < 0) return false;
                b_GamePlayer.dBCharacterDroplimit.SDGTime = DTime + 7200;
                b_GamePlayer.dBCharacterDroplimit.SuitCnt--;
                b_GamePlayer.SetDropItemDB();
                return true;
            }
            /*
             * 不限制奇迹币数量
             * if (item.ConfigId == 320316)
            {
                var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>();
                var info = ServerInfo?.GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
                if (info.IsVIP == 1 || b_GamePlayer.CurrentMap.MapId == 17)
                {
                    if (b_GamePlayer.dBCharacterDroplimit.MiracleCoinCnt == -1) return true;
                    if (b_GamePlayer.dBCharacterDroplimit.MiracleCoinCnt == 0)
                    {
                        Log.Info($"Player:{b_GamePlayer.Player.GameUserId} Item:{item.ConfigId} 奇迹币掉落到达上限");
                        return false;
                    }

                    b_GamePlayer.dBCharacterDroplimit.MiracleCoinCnt -= item.Count;
                    if (b_GamePlayer.dBCharacterDroplimit.MiracleCoinCnt < -1)
                        b_GamePlayer.dBCharacterDroplimit.MiracleCoinCnt = 0;
                    b_GamePlayer.SetDropItemDB();
                    return true;
                }
                return false;
            }*/
            return true;
        }
        /// <summary>
        /// 检查保底掉落
        /// </summary>
        /// <param name="b_GamePlayer"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static int CheckDrop(this GamePlayer b_GamePlayer,(int,int) Id)
        {
            if (b_GamePlayer.dBCharacterDroplimit == null) return 0;
            if (Id.Item1 <= 0) return 0;

            if (b_GamePlayer.dBCharacterDroplimit.MGLsit.TryGetValue(Id, out int Cnt))
            {
                if (Id.Item2 < Cnt)
                {
                    b_GamePlayer.dBCharacterDroplimit.MGLsit.Remove(Id);
                    b_GamePlayer.dBCharacterDroplimit.MinimumGuarantee = string.Join(";", b_GamePlayer.dBCharacterDroplimit.MGLsit.Select(x => $"{x.Key.Item1},{x.Key.Item2},{x.Value}"));
                    b_GamePlayer.SetDropItemDB();
                    return Id.Item1;
                }

            }
            return 0;
        }

        public static void DeleteDrop(this GamePlayer b_GamePlayer, (int, int) Id)
        {
            b_GamePlayer.dBCharacterDroplimit.MGLsit.Remove(Id);
            b_GamePlayer.dBCharacterDroplimit.MinimumGuarantee = string.Join(";", b_GamePlayer.dBCharacterDroplimit.MGLsit.Select(x => $"{x.Key.Item1},{x.Key.Item2},{x.Value}"));
            b_GamePlayer.SetDropItemDB();
        }
    }
}