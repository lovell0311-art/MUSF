using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.HttpProto;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq;
using Aop.Api.Domain;

namespace ETHotfix
{
	[HttpHandler(AppType.GM, "/api/cdkey/")]
    public class Http_GM_CDKey : AHttpHandler
    {
        const long CDKeyAdd = 1;
        // 添加兑换码奖品类型
        [Post]  // Url-> /api/cdkey/AddType
        public async Task<HttpResult> AddType(HttpListenerRequest req, CDKeyAddTypeParam param)
        {
            if(param == null)
            {
                return Error(msg: "参数错误");
            }
            if(param.RewardList.Count == 0)
            {
                return Error(msg: "RewardList.Count == 0");
            }

            DBProxyComponent dbProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            int rewardType = 1000;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.GM, CDKeyAdd))
            {
                using ListComponent<BsonDocument> pipeline = ListComponent<BsonDocument>.Create();
                BsonDocument sort = new BsonDocument()
                {
                    { "RewardType",-1}
                };
                pipeline.Add(new BsonDocument("$sort", sort));
                pipeline.Add(new BsonDocument("$limit", 1));
                List<BsonDocument> results = await dbProxy.Aggregate<DBRCodeTypeData>(pipeline);
                if(results.Count > 0)
                {
                    rewardType = results[0]["RewardType"].AsInt32;
                    ++rewardType;
                }

                DBRCodeTypeData data = new DBRCodeTypeData();
                data.Id = IdGeneraterNew.Instance.GenerateUnitId(param.ZoneId);
                data.CodeType = (int)RedemptionCodeType.OneType;
                data.RewardType = rewardType;
                data.RewardStr = Help_JsonSerializeHelper.Serialize(param.RewardList, true);

                await dbProxy.Save(data);
            }

            return Ok();
        }


        // 添加兑换码
        [Post]  // Url-> /api/cdkey/AddCode
        public async Task<HttpResult> AddCode(HttpListenerRequest req, CDKeyAddCodeParam param)
        {
            if (param == null)
            {
                return Error(msg: "参数错误");
            }

            DBProxyComponent dbProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            var results = await dbProxy.Query<DBRCodeTypeData>(p=>p.RewardType == param.RewardType);
            if(results.Count <= 0)
            {
                return Error(msg: $"RewardType 不存在，RewardType={param.RewardType}");
            }
            DBRCodeTypeData dbRCodeTypeData = results[0] as DBRCodeTypeData;


            using ListComponent<(long id,string cdkey)> cdkeyList = ListComponent<(long, string)>.Create();
            for (int i = 0; i < param.Count; ++i)
            {
                cdkeyList.Add((IdGeneraterNew.Instance.GenerateUnitId(param.ZoneId),
                    ""));
                
            }

            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();
            foreach(var kv in cdkeyList)
            {
                async Task<bool> Insert((long id, string cdkey) kv)
                {
                    using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CDKeyGen, kv.id % 10))  // 最大同时写入10条
                    {
                        Log.Console($"id:{kv.id}");
                        DBRCodeData data = new DBRCodeData()
                        {
                            Id = kv.id,
                            CodeType = (int)RedemptionCodeType.OneType,
                            RewardType = param.RewardType,
                            CodeStr = CDKeyGenerater.CreateNumber(RedemptionCodeType.OneType, param.RewardType),
                            RewardStr = dbRCodeTypeData.RewardStr,
                            TimeTick = 0,
                            UseCount = 1,
                        };
                        await dbProxy.Save(data);
                    }
                    return true;
                }
                tasks.Add(Insert(kv));
            }

            await TaskHelper.WaitAll(tasks);

            return Ok(data:new
            {
                FirstId = cdkeyList.First().id.ToString(),
                LastId = cdkeyList.Last().id.ToString(),
            });
        }
       
    }


    public static class CDKeyGenerater
    {
        public static string CreateNumber(RedemptionCodeType b_CodeType, int b_RewardType)
        {
            var mCodeType = (int)b_CodeType;

            // 随机头
            string mRandomHead = Help_RandomHelper.RangeString(2, Encoding.UTF8).ToUpper();

            // 随机补位字符
            string mSourceStr = "HIJKLMNOPQRSTUVWXYZ";
            var mRangIndex = Help_RandomHelper.Range(0, mSourceStr.Length);
            var mRangValue = mSourceStr[mRangIndex];

            // 兑换码类型
            string mCodeTypeStr = mCodeType.ToString("X").PadLeft(3, mRangValue).ToUpper();

            mRangIndex = Help_RandomHelper.Range(0, mSourceStr.Length);
            mRangValue = mSourceStr[mRangIndex];
            string mContext = b_RewardType.ToString("X").PadLeft(3, mRangValue).ToUpper();

            mRangIndex = Help_RandomHelper.Range(0, mSourceStr.Length);
            mRangValue = mSourceStr[mRangIndex];
            // 唯一id
            long uid = Help_UniqueValueHelper.GetUniqueValueByTime();
//             string mUniqueValueTime = uid.ToString("X").PadLeft(16, mRangValue).ToUpper();
//             Log.Console($"[{GetInvCodeByUID(uid,16)}] {mUniqueValueTime}");
            string number = string.Join("G", mRandomHead, mCodeTypeStr, mContext, GetInvCodeByUID(uid,16));

            return number;
        }

        public static string GetInvCodeByUID(long uid, int length)
        {
            string AlphanumericSet = "ETMHAR5NWZX6IY7B9VSJ3UDCP482FQLK";
            byte[] slIdx = new byte[length];
            string code = "";
            for (int i = 0; i < length; ++i)
            {
                slIdx[i] = (byte)(uid % AlphanumericSet.Length);
                byte idx = (byte)((slIdx[i] + (byte)i * slIdx[0]) % (byte)AlphanumericSet.Length);
                code += AlphanumericSet[idx];
                uid = uid / AlphanumericSet.Length;
            }
            return code;
        }

    }
}
