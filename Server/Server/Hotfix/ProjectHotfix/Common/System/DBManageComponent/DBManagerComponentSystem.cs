using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;


namespace ETHotfix
{
    public static class DBManagerComponentSystem
    {
        public static DBComponent GetZoneDB(this DBManagerComponent self, DBType dBType, int zone)
        {
            if (self.DBCacheDic.TryGetValue(dBType, out var keyValuePairs) == false)
            {
                keyValuePairs = self.DBCacheDic[dBType] = new Dictionary<int, DBComponent>();
            }
            // 目标区服
            if (keyValuePairs.TryGetValue(zone, out DBComponent dbComponent))
            {
                if (dbComponent != null && dbComponent.IsDisposeable == false)
                {
                    return dbComponent;
                }
            }

            var mConfigs = CustomFrameWork.Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Server_DataConfigJson>().JsonDic.Values.ToArray();
            for (int i = 0, len = mConfigs.Length; i < len; i++)
            {
                var mStartZoneConfigs = mConfigs[i];
                if (mStartZoneConfigs.DBConnection == "")
                {
                    throw new Exception($"dBType:{dBType}  zone: {zone} not found mongo connect string");
                }

                if (OptionComponent.Options.AppId == mStartZoneConfigs.AppId //是我这个服务器
                    && zone == mStartZoneConfigs.DBZone   //对应区
                    && (int)dBType == mStartZoneConfigs.DBType)  //对应数据库类型
                {
                    dbComponent = Root.CreateBuilder.GetInstance<DBComponent>();
                    dbComponent.OnInit(mStartZoneConfigs.DBType, mStartZoneConfigs.DBZone);

                    keyValuePairs[zone] = dbComponent;
                }
            }

            return dbComponent;
        }
    }
}