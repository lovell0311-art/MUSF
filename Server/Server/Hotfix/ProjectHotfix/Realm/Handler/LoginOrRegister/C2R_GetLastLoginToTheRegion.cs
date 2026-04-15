using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class C2R_GetLastLoginToTheRegionHandler : AMRpcHandler<C2R_GetLastLoginToTheRegion, R2C_GetLastLoginToTheRegion>
    {
        protected override async Task<bool> CodeAsync(Session session, C2R_GetLastLoginToTheRegion request, R2C_GetLastLoginToTheRegion response, Action<IMessage> reply)
        {
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

            string address = session.RemoteAddress.Address.ToString();

            string str = request.Uid;
            DBAccountInfo dbLoginInfo = null;
            {// 获取一个账号
                List<ComponentWithId> list = new List<ComponentWithId>();
                switch (request.Type)
                {
                    case 1://恺英
                        list = await mDBProxy.Query<DBAccountInfo>(p => p.XYAccountNumber == str);
                        break;
                    case 2://抖音
                        if(long.TryParse(str,out long UserId))
                            list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == UserId);
                        break;
                    case 3://手Q
                        list = await mDBProxy.Query<DBAccountInfo>(p => p.ShouQAccountNumber == str);
                        break;
                    default:
                        list = await mDBProxy.Query<DBAccountInfo>(p => p.Phone == str);
                        break;

                }

                if (list.Count > 0)
                {
                    dbLoginInfo = list[0] as DBAccountInfo;
                }

            }
            if (dbLoginInfo == null)
            {
                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                reply(response);
                return false;
            }
            /*
            var AutoArer = Root.MainFactory.GetCustomComponent<AutoAreaComponent>();
            if (AutoArer == null)
            {
                response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                reply(response);
                return false;
            }
                int Region = 1;
                int NewAreaId = 1;
                bool UpData = true;
                foreach (var Info in AutoArer.keyValuePairs)
                {
                    if (dbLoginInfo.LastLoginToTheRegion == Info.Key)
                        UpData = false;

                    if (NewAreaId <= Info.Key)
                        NewAreaId = Info.Key;
                }
            if (UpData)
            {
                if (dbLoginInfo.LastLoginToLine == 0)
                    dbLoginInfo.LastLoginToTheRegion = NewAreaId;
                else
                    dbLoginInfo.LastLoginToTheRegion = 1;

                dbLoginInfo.LastLoginToLine = 1;
            }
            
           */

            if (dbLoginInfo.LastLoginToTheRegion == 2|| dbLoginInfo.LastLoginToTheRegion == 3|| dbLoginInfo.LastLoginToTheRegion == 4)
                dbLoginInfo.LastLoginToTheRegion = 1;
            if (dbLoginInfo.LastLoginToTheRegion == 0)
            {
                dbLoginInfo.LastLoginToTheRegion = 1;
                dbLoginInfo.LastLoginToLine = 1;
            }
            response.GameAreaId = dbLoginInfo.LastLoginToTheRegion;
            response.Gameline = dbLoginInfo.LastLoginToLine;
            reply(response);
            return false;
        }
    }
}