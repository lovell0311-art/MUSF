using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    public static class DBLogHelper
    {
        public static void Write(DBBase db,int zoneId = 0)
        {
            db.Id = IdGeneraterNew.Instance.GenerateId();

            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Log,zoneId);
            dBProxy.Save(db).Coroutine();
        }

        public static DBTradeLog CreateDBTradeLog(this Player self,Player targetPlayer)
        {
            DBTradeLog db = new DBTradeLog();
            db.CreateTime = TimeHelper.Now();
            db.UserId = self.UserId;
            db.GameUserId = self.GameUserId;
            db.TargetUserId = targetPlayer.UserId;
            db.TargetGameUserId = targetPlayer.GameUserId;
            db.GameServerId = OptionComponent.Options.AppId;
            return db;
        }

        public static DBTradeLog CreateDBTradeLog(Player targetPlayer)
        {
            DBTradeLog db = new DBTradeLog();
            db.CreateTime = TimeHelper.Now();
            db.TargetUserId = targetPlayer.UserId;
            db.TargetGameUserId = targetPlayer.GameUserId;
            db.GameServerId = OptionComponent.Options.AppId;
            return db;
        }
    }
}
