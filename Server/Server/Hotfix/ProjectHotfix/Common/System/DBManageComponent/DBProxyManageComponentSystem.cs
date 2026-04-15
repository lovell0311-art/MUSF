
using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    public static class DBProxyManageComponentSystem
    {
        public static DBProxyComponent GetZoneDB(this DBProxyManagerComponent self, DBType dBType, int zone)
        {
            if (self.DBCacheDic.TryGetValue(dBType, out var keyValuePairs) == false)
            {
                keyValuePairs = self.DBCacheDic[dBType] = new Dictionary<int, DBProxyComponent>();
            }
            // 目标区服
            if (keyValuePairs.TryGetValue(zone, out DBProxyComponent dbComponent))
            {
                if (dbComponent != null && dbComponent.IsDisposeable == false)
                {
                    return dbComponent;
                }
            }
            var mConfigs = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Server_DataConfigJson>().JsonDic.Values.ToArray();
            for (int i = 0, len = mConfigs.Length; i < len; i++)
            {
                Server_DataConfig mStartZoneConfigs = mConfigs[i];
                //options.AppId == mStartZoneConfigs.AppId //是我这个服务器
                if (zone == mStartZoneConfigs.DBZone   //对应区
                    && (int)dBType == mStartZoneConfigs.DBType)  //对应数据库类型
                {
                    if (mStartZoneConfigs.DBConnection == "")
                    {
                        throw new Exception($"dBType:{dBType}  zone: {zone} not found mongo connect string");
                    }

                    dbComponent = Root.CreateBuilder.GetInstance<DBProxyComponent>();
                    dbComponent.OnInit(mStartZoneConfigs.DBType, mStartZoneConfigs.DBZone);

                    // 后面修改成允许 每个区多个代理
                    // 目前一个区一个类型 一个代理
                    keyValuePairs[zone] = dbComponent;
                }
            }
            return dbComponent;
        }
    }
}