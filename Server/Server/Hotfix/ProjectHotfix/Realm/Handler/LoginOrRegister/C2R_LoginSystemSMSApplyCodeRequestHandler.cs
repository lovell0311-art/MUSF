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
    public class C2R_LoginSystemSMSApplyCodeRequestHandler : AMRpcHandler<C2R_LoginSystemSMSApplyCodeRequest, R2C_LoginSystemSMSApplyCodeResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2R_LoginSystemSMSApplyCodeRequest b_Request, R2C_LoginSystemSMSApplyCodeResponse b_Response, Action<IMessage> b_Reply)
        {
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            int mDay = DateTime.UtcNow.Day;
            ESMSCodeType codeType = ESMSCodeType.Register;
            switch (b_Request.UseType)
            {
                case 0://注册
                    {
                        List<ComponentWithId> mResult = await mDBProxy.Query<DBAccountInfo>(o => o.Phone == b_Request.PhoneNumber);
                        if (mResult.Count > 0)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(101);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号已存在!");
                            b_Reply(b_Response);
                            return false;
                        }
                        codeType = ESMSCodeType.Register;
                    }
                    break;
                case 1://修改密码
                    {
                        List<ComponentWithId> mResult = await mDBProxy.Query<DBAccountInfo>(o => o.Phone == b_Request.PhoneNumber);
                        if (mResult.Count <= 0)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                            b_Reply(b_Response);
                            return false;
                        }
                        codeType = ESMSCodeType.ResetPasswd;
                    }
                    break;
                default:
                    break;
            }

            SMSMessageComponent msg = Root.MainFactory.GetCustomComponent<SMSMessageComponent>();
            string msgResult = await msg.CreateCode(codeType,b_Request.PhoneNumber, b_Request.CountryCode);

            if (msgResult == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(107);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("发送失败!");
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}