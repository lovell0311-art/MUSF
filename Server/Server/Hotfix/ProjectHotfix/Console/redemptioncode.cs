
using ETModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
using System.Text;

namespace ETHotfix
{
    [ConsoleCommandLineAttribute(ConsoleCommandLinePath.redemptioncode)]
    public class ConsoleCommandLine_redemptioncode : C_ConsoleCommandLine
    {
        public override async Task Run(string b_Contex)
        {
            switch (b_Contex)
            {
                case ConsoleCommandLinePath.redemptioncode:
                    {
                        Log.Console($"进入了{b_Contex} 子命令!!!");
                    }
                    break;
            }
        }
        public override async Task Run(ModeContexCommandlineComponent b_Component, string b_Contex)
        {
            switch (b_Contex)
            {
                case ConsoleCommandLinePath.redemptioncode:
                    {//插入区服数据

                        DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        //Console.WriteLine($"mDBProxyManagerComponent:{mDBProxyManagerComponent}");

                        var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

                        for (int i = 0; i < 5; i++)
                        {
                            int sd = 5;

                            string mCodeStr = CreateNumber(RedemptionCodeType.LimitCount, sd);

                            DBRCodeData m = new DBRCodeData()
                            {
                                Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),

                                CodeStr = mCodeStr,
                                RewardStr = "[{\"ItemConfigID\":320407,\"CreateAttr\":{\"Quantity\":1,\"IsBind\":1,\"ValidTime\":0}}]",
                                CodeType = (int)RedemptionCodeType.LimitCount,
                                RewardType = sd,
                                TimeTick = Help_TimeHelper.GetNow() + 1000L * 60 * 60 * 24 * 30,
                                UseCount = 1
                            };
                            bool msaveresult = await dBProxy.Save(m);

                            Console.WriteLine($"code: {mCodeStr}  saveresult: {msaveresult}");
                        }


                        string CreateNumber(RedemptionCodeType b_CodeType, int b_RewardType)
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
                            string mUniqueValueTime = Help_UniqueValueHelper.GetUniqueValueByTime().ToString("X").PadLeft(16, mRangValue).ToUpper();

                            string number = string.Join("G", mRandomHead, mCodeTypeStr, mContext, mUniqueValueTime);

                            return number;
                        }

                        Log.Console($"插入数据 : ");
                    }
                    break;
                default:
                    {
                        string[] ss = b_Contex.Split(" ");
                        string configName = ss[1];




                        Log.Console($"{configName}");
                    }
                    break;
            }
        }
    }
}
