using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class C2R_ResetPasswordByPhoneNumberRequestHandler : AMRpcHandler<C2R_ResetPasswordByPhoneNumberRequest, R2C_ResetPasswordByPhoneNumberResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2R_ResetPasswordByPhoneNumberRequest request, R2C_ResetPasswordByPhoneNumberResponse response, Action<IMessage> reply)
        {
            {// 校验账号是不是手机号
                if (!Int64.TryParse(request.PhoneNumber, out Int64 account))
                {
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(100);
                    //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号错误,请重新输入!");
                    reply(response);
                    return false;
                }
            }

            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var mDBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            string remoteIp = session.RemoteAddress.Address.ToString();
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.PhoneNumber.Trim().GetHashCode()))
            {
                if (!Root.MainFactory.GetCustomComponent<SMSMessageComponent>().Verify(ESMSCodeType.ResetPasswd, request.PhoneNumber, request.VerificationCode))
                {
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(105);
                    reply(response);
                    return false;
                }
                var list = await mDBProxy.Query<DBAccountInfo>(p => p.Phone == request.PhoneNumber);
                if (list.Count == 0)
                {
                    // 账号不存在
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                    reply(response);
                    return false;
                }

                DBAccountInfo dbLoginInfo = list[0] as DBAccountInfo;
                if (request.Idcard == "" || request.Idcard.Length != 6 || dbLoginInfo.IdCard.EndsWith(request.Idcard) == false)
                {
                    // 身份验证失败
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(136);
                    reply(response);
                    return false;
                }
                dbLoginInfo.Password = request.NewPassword;
                if ((await mDBProxy.Save(dbLoginInfo)) == false)
                {
                    // 服务异常，保存数据异常
                    response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21001);
                    reply(response);
                    return false;
                }

                DbChangePasswordLog dbChangePasswordLog = new DbChangePasswordLog()
                {
                    LogId = Help_UniqueValueHelper.GetServerUniqueValue(),
                    UserId = dbLoginInfo.Id,
                    GameServerId = OptionComponent.Options.AppId,

                    LastLoginIP = remoteIp,
                    DeviceType = request.DeviceType,
                    CPUType = request.CPUType,
                    BaseVersion = request.BaseVersion,
                    OSType = request.OSType,
                    ChannelId = request.ChannelId,

                    UpdateTime2 = System.DateTime.UtcNow
                };

                var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Log, DBProxyComponent.CommonDBId);

                dBProxy.Save(dbChangePasswordLog).Coroutine();
            }

            reply(response);
            return true;
        }


    }
}