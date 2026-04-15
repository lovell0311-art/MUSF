using CustomFrameWork;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class DBCharacterDroplimit : DBBase
    {
        /// <summary>
        /// 所属玩家，Player的GameUserId
        /// </summary>
        public long GameUserId { get; set; }
        /// <summary>
        /// 宝石数
        /// </summary>
        public short GemstoneCnt { get; set; }
        public long GemstoneTime { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public long GDGTime { get; set; }
        /// <summary>
        /// 羽毛数
        /// </summary>
        public short FeatherCnt { get; set; }
        public long FeatherTime { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public long FDGTime { get; set; }
        /// <summary>
        /// 复古没有藏宝图碎片数，修改成坐骑材料
        /// </summary>
        public short CangBaotuSPCnt { get; set; }
        public long CangBaotuSPTime { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public long CBTDGTime { get; set; }
        /// <summary>
        /// 卓越装备数
        /// </summary>
        public short ExcellenceCnt { get; set; }
        public long ExcellenceTime { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public long EDGTime { get; set; }
        /// <summary>
        /// 套装数
        /// </summary>
        public short SuitCnt { get; set; }
        public long SuitTime { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public long SDGTime { get; set; }
        /// <summary>
        /// 奇迹币数量
        /// </summary>
        public int MiracleCoinCnt { get; set; }
        public long MiracleCoinTime { get; set; }
        /// <summary>
        /// 工作室账号限制
        /// </summary>
        public int Restrict { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public long UpdataTiem { get; set; }
        /// <summary>
        /// 保底掉落
        /// </summary>
        public string MinimumGuarantee { get; set; } = "";
        /// <summary>
        /// Dictionary<(int,int),int>根据需求设置<(Id,上限次数),当前次数>可以是<(MobId,Cnt),Cnt>也可以是<(ItemId,Cnt),Cnt>
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<(int,int),int> MGLsit = new Dictionary<(int, int), int>();
        /// <summary>
        /// 0:普通用户1:小月卡2:大月卡 Time= 当天的零点
        /// </summary>
        /// <param name="IsVIP"></param>
        public DBCharacterDroplimit(int IsVIP = 0,int CoinCnt = 200,long Time = 0)
        {
            long NowTime = Help_TimeHelper.GetNowSecond();
            switch (IsVIP)
            {
                //case 1:
                //    {
                //        GameUserId = 0;
                //        GemstoneCnt = 2;
                //        FeatherCnt = 1;
                //        CangBaotuSPCnt = 5;
                //        ExcellenceCnt = 2;
                //        SuitCnt = 10;
                //        MinimumGuarantee = "";
                //        MGLsit = new Dictionary<(int,int), int>();
                //        //UpdataTiem = Help_TimeHelper.GetNowSecond() + 86400;
                //    }
                //    break;
                case 2:
                    {
                        GameUserId = 0;
                        GemstoneCnt = 2;
                        GemstoneTime = NowTime + 86400;
                        FeatherCnt = 1;
                        FeatherTime = NowTime + 604800;
                        CangBaotuSPCnt = 1;
                        CangBaotuSPTime = NowTime + 86400;
                        ExcellenceCnt = 1;
                        ExcellenceTime = NowTime + 86400;
                        SuitCnt = 2;
                        SuitTime = NowTime + 86400;
                        MiracleCoinCnt = CoinCnt;
                        MiracleCoinTime = Time + 86400;
                        MinimumGuarantee = "";
                        MGLsit = new Dictionary<(int, int), int>();
                        //UpdataTiem = Help_TimeHelper.GetNowSecond() + 86400;
                    }
                    break;
                default:
                    {
                        //GameUserId = 0;
                        //GemstoneCnt = 1;
                        //FeatherCnt = 1;
                        //CangBaotuSPCnt = 2;
                        //ExcellenceCnt = 1;
                        //SuitCnt = 1;
                        //MinimumGuarantee = "";
                        //MGLsit = new Dictionary<(int, int), int>();
                        //UpdataTiem = Help_TimeHelper.GetNowSecond() + 86400;
                        GameUserId = 0;
                        GemstoneCnt = 1;
                        GemstoneTime = NowTime + 86400;
                        FeatherCnt = 1;
                        FeatherTime = NowTime + 604800;
                        CangBaotuSPCnt = 1;
                        CangBaotuSPTime = NowTime + 604800;
                        ExcellenceCnt = 1;
                        ExcellenceTime = NowTime + 86400;
                        SuitCnt = 1;
                        SuitTime = NowTime + 86400;
                        MiracleCoinCnt = 0;
                        MiracleCoinTime = Time + 86400;
                        MinimumGuarantee = "";
                        MGLsit = new Dictionary<(int, int), int>();
                    }
                    break; 
            }
        }
    }
}
