using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class C2G_RealNameAuthenticationRequestHandler : AMRpcHandler<C2G_RealNameAuthenticationRequest, G2C_RealNameAuthenticationResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2G_RealNameAuthenticationRequest request, G2C_RealNameAuthenticationResponse response, Action<IMessage> reply)
        {
            string mIdCardCord = request.IdNum.TrimStart().TrimEnd();
            string mName = request.Name.TrimStart().TrimEnd();

            if (TestCardId(mIdCardCord) == false) return true;

            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            var list = await mDBProxy.Query<DBAccountInfo>(p => p.Phone == request.Account);
            if (list == null || list.Count <= 0)
            {
                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                reply(response);
                return true;
            }

            var dbLoginInfo = list[0] as DBAccountInfo;
            var mNowTick = Help_TimeHelper.GetNowSecond();
            if (dbLoginInfo.LastIdInspect > mNowTick)
            {
                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(133);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                reply(response);
                return true;
            }
            dbLoginInfo.LastIdInspect = mNowTick + 5;
            if (dbLoginInfo.IdInspect != 0)
            {
                //response.Error = ErrorCodeHotfix.ERR_AccountAlreadyExists;
                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(122);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                reply(response);
                return true;
            }

            var mIdCardlist = await mDBProxy.Query<DBIdInspect>(p => p.IdCard == mIdCardCord);
            if (mIdCardlist != null && mIdCardlist.Count > 10)
            {
                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(123);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                reply(response);
                return true;
            }

            bool TestCardId(string cardId)
            {
                if (cardId.Length != 18)
                {
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                    reply(response);
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
                        response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                        reply(response);
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
                            response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                            reply(response);
                            return false;
                            //return "身份证最后一位校验错误!";
                        }
                    }
                    else
                    {
                        response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                        reply(response);
                        return false;
                        //return "出生日期验证失败!";
                    }
                }
                else
                {
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(131);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号或密码错误!");
                    reply(response);
                    return false;
                    //return "身份证号格式错误!";
                }
            }
            string StrResponse = await RealName.RealNameRequest(dbLoginInfo.Id.ToString(), mIdCardCord,mName);
            RealNameResponse msgResult = Help_JsonSerializeHelper.DeSerialize<RealNameResponse>(StrResponse);
            switch (msgResult.errcode)
            {
                case 0:
                    {
                        if (msgResult.data.result.status == 0)
                        {
                            dbLoginInfo.IdInspect = 1;
                            dbLoginInfo.IdCard = mIdCardCord;
                            dbLoginInfo.Name = mName;
                            dbLoginInfo.Pi = msgResult.data.result.pi;
                            bool mSaveResult = await mDBProxy.Save(dbLoginInfo);
                            if (mSaveResult == false)
                            {
                                Log.Error($"dbLoginInfo 保存失败!  Id:{dbLoginInfo.Id}  ");
                                //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(124);
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
                                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(125);
                                //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                            }
                            //return true;
                        }
                        Log.PLog($"身份认证 mResult.code:{msgResult.errcode} mResult.data.result:{msgResult.data.result} {msgResult.errmsg}");
                    }
                    break;
                default:
                    {
                        Log.PLog($"游戏方身份认证 mResult.code:{msgResult.errcode} {msgResult.errmsg} Data.status{msgResult.data.result.status} Data.Pi{msgResult.data.result.pi}");
                        response.Error = 127;
                        bool mSaveResult = await mDBProxy.Save(dbLoginInfo);
                        if (mSaveResult == false)
                        {
                            Log.Error($"dbLoginInfo 保存失败!  Id:{dbLoginInfo.Id}  ");
                            //response.Error = ErrorCodeHotfix.ERR_SMSInspectError;
                            response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(124);
                            //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号保存失败");
                        }
                    }
                    break;
            }

            reply(response);
            return true;
        }
    }
}