using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class C2G_IdCardInspectRequestHandler : AMRpcHandler<C2G_IdCardInspectRequest, G2C_IdCardInspectResponse>
    {
        protected override async Task<bool> CodeAsync(Session b_Connect, C2G_IdCardInspectRequest b_Request, G2C_IdCardInspectResponse b_Response, Action<IMessage> b_Reply)
        {
            string mIdCardCord = b_Request.IdCardCord.TrimStart().TrimEnd();
            string mName = b_Request.Name.TrimStart().TrimEnd();

            if (TestCardId(mIdCardCord) == false) return true;

            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            var list = await mDBProxy.Query<DBAccountInfo>(p => p.Phone == b_Request.Account.ToString());
            if (list == null || list.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                b_Reply(b_Response);
                return true;
            }

            var dbLoginInfo = list[0] as DBAccountInfo;
            var mNowTick = Help_TimeHelper.GetNowSecond();
            if (dbLoginInfo.LastIdInspect > mNowTick)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(133);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                b_Reply(b_Response);
                return true;
            }
            dbLoginInfo.LastIdInspect = mNowTick + 5;
            if (dbLoginInfo.IdInspect != 0)
            {
                //response.Error = ErrorCodeHotfix.ERR_AccountAlreadyExists;
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(122);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                b_Reply(b_Response);
                return true;
            }

            var mIdCardlist = await mDBProxy.Query<DBIdInspect>(p => p.IdCard == mIdCardCord);
            if (mIdCardlist != null && mIdCardlist.Count > 10)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(123);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                b_Reply(b_Response);
                return true;
            }

            bool TestCardId(string cardId)
            {
                if (cardId.Length != 18)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                    b_Reply(b_Response);
                    return false;
                }
                string pattern = @"^\d{17}(?:\d|X)$";
                string birth = cardId.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                // 加权数组,用于验证最后一位的校验数字
                int[] arr_weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
                // 最后一位计算出来的校验数组，如果不等于这些数字将不正确
                string[] id_last = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
                int sum = 0;
                //通过循环前16位计算出最后一位的数字
                for (int i = 0; i < 17; i++)
                {
                    var mtemp = cardId[i];
                    if (int.TryParse(mtemp.ToString(), out var mvalue) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                        b_Reply(b_Response);
                        return false;
                    }
                    sum += arr_weight[i] * mvalue;
                }
                // 实际校验位的值
                int result = sum % 11;
                // 首先18位格式检查
                if (Regex.IsMatch(cardId, pattern))
                {   // 出生日期检查
                    if (DateTime.TryParse(birth, out time))
                    {
                        // 校验位检查
                        if (id_last[result] == cardId[17].ToString())
                        {
                            return true;
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                            b_Reply(b_Response);
                            return false;
                            //return "身份证最后一位校验错误!";
                        }
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                        b_Reply(b_Response);
                        return false;
                        //return "出生日期验证失败!";
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                    b_Reply(b_Response);
                    return false;
                    //return "身份证号格式错误!";
                }
                return false;
            }
            string debugJson = Newtonsoft.Json.JsonConvert.SerializeObject(b_Request);
            Log.Info($"#实名认证# {debugJson}");
            SMSMessageComponent msg = Root.MainFactory.GetCustomComponent<SMSMessageComponent>();
            var msgResult = await msg.SFZYZ2(mIdCardCord, mName);

            switch (msgResult.code)
            {
                case 200:
                    {
                        if (msgResult.data.result == 0)
                        {
                            dbLoginInfo.IdInspect = 1;
                            dbLoginInfo.IdCard = mIdCardCord;
                            dbLoginInfo.Name = mName;
                            bool mSaveResult = await mDBProxy.Save(dbLoginInfo);
                            if (mSaveResult == false)
                            {
                                Log.Error($"dbLoginInfo 保存失败!  Id:{dbLoginInfo.Id}  ");
                                //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(124);
                                //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                            }

                            mSaveResult = false;
                            DBIdInspect dBIdInspect = new DBIdInspect()
                            {
                                Id = IdGeneraterNew.Instance.GenerateUnitId(DBComponent.CommonDBId),
                                Name = mName,
                                IdCard = mIdCardCord
                            };
                            mSaveResult = await mDBProxy.Save(dBIdInspect);
                            if (mSaveResult == false)
                            {
                                Log.Error($"DBIdInspect 保存失败!  Name:{dBIdInspect.Name}  IdCard:{dBIdInspect.IdCard} ");
                                //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(125);
                                //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                            }
                            //return true;
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(134);
                        }
                        Log.PLog($"身份认证 mResult.code:{msgResult.code} mResult.data.result:{msgResult.data.result} {msgResult.msg}");
                    }
                    break;
                case 400:
                    {
                        Log.PLog($"身份认证 mResult.code:{msgResult.code} {msgResult.msg}");
                        b_Response.Error = 126;
                        bool mSaveResult = await mDBProxy.Save(dbLoginInfo);
                        if (mSaveResult == false)
                        {
                            Log.Error($"dbLoginInfo 保存失败!  Id:{dbLoginInfo.Id}  ");
                            //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(124);
                            //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                        }
                    }
                    break;
                case 500:
                    {
                        Log.PLog($"身份认证 mResult.code:{msgResult.code} {msgResult.msg}");
                        b_Response.Error = 127;
                        bool mSaveResult = await mDBProxy.Save(dbLoginInfo);
                        if (mSaveResult == false)
                        {
                            Log.Error($"dbLoginInfo 保存失败!  Id:{dbLoginInfo.Id}  ");
                            //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(124);
                            //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                        }
                    }
                    break;
                case 501:
                    {
                        Log.PLog($"身份认证 mResult.code:{msgResult.code} {msgResult.msg}");
                        b_Response.Error = 128;
                        bool mSaveResult = await mDBProxy.Save(dbLoginInfo);
                        if (mSaveResult == false)
                        {
                            Log.Error($"dbLoginInfo 保存失败!  Id:{dbLoginInfo.Id}  ");
                            //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(124);
                            //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                        }
                    }
                    break;
                case 999:
                    {
                        Log.PLog($"身份认证 mResult.code:{msgResult.code} {msgResult.msg}");
                        b_Response.Error = 129;
                        bool mSaveResult = await mDBProxy.Save(dbLoginInfo);
                        if (mSaveResult == false)
                        {
                            Log.Error($"dbLoginInfo 保存失败!  Id:{dbLoginInfo.Id}  ");
                            //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(124);
                            //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                        }
                    }
                    break;
                default:
                    {
                        Log.Error($"身份认证 mResult.code:{msgResult.code} {msgResult.msg}");
                        b_Response.Error = 130;
                        bool mSaveResult = await mDBProxy.Save(dbLoginInfo);
                        if (mSaveResult == false)
                        {
                            Log.Error($"dbLoginInfo 保存失败!  Id:{dbLoginInfo.Id}  ");
                            //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(124);
                            //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                        }
                    }
                    break;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}