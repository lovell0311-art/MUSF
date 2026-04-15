using Aop.Api.Domain;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Mongodb.V20190725.Models;
using TencentCloud.Ocr.V20181119.Models;

namespace ETHotfix
{
    [Timer(TimerType.CommissuralAreaSignal)]
    public class JointAreaSignalDetectionTimer : ATimer<JointAreaSignalDetectionComponent>
    {
        public override void Run(JointAreaSignalDetectionComponent self)
        {
            self.CheckCoincidentdata().Coroutine();
        }
    }
    [ObjectSystem]
    public class JointAreaSignalDetectionSystem : DestroySystem<JointAreaSignalDetectionComponent>
    {
        public override void Destroy(JointAreaSignalDetectionComponent self)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref self._timerId);
        }
    }
    [EventMethod(typeof(JointAreaSignalDetectionComponent), EventSystemType.INIT)]
    public class JointAreaSignalDetectionEventOnInit : ITEventMethodOnInit<JointAreaSignalDetectionComponent>
    {
        public void OnInit(JointAreaSignalDetectionComponent b_Component)
        {            
            b_Component._timerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.CommissuralAreaSignal, b_Component);
        }
    }
    public static class JointAreaSignalDetectionComponentSystem
    {
        public static async Task CheckCoincidentdata(this JointAreaSignalDetectionComponent self)
        {
            if (!self.OneOfCompatibility)
            {
                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
                var Info = await dBProxy.Query<DBCoincidentdata>(P => P.IsDispose != 1);
                if (Info != null && Info.Count >= 1)
                {
                    foreach (var Data in Info)
                    {
                        var Area = Data as DBCoincidentdata;
                        if (Area.OldAreaId == OptionComponent.Options.ZoneId || Area.NewAreaId == OptionComponent.Options.ZoneId)
                        {
                            self.OneOfCompatibility = true;
                            if (OptionComponent.Options.AppType == AppType.MGMT)
                            {
                                await Task.Delay(60000);
                                Area.IsDispose = 1;
                                await dBProxy.Save(Area);
                                //通知DB关闭
                                var DBServerList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.DB);
                                foreach (var Db in DBServerList)
                                {
                                    if (Db.ZoneId == OptionComponent.Options.ZoneId)
                                    {
                                        Session mSession = Game.Scene.GetComponent<NetInnerComponent>().Get(Db.ServerInnerIP);
                                        mSession.Send(new M2D_ExitDBServer(){ AreaId = Db.ZoneId });
                                    }
                                }
                                
                                System.Environment.Exit(0);

                            }
                            else if (OptionComponent.Options.AppType == AppType.Game)
                            {
                                Help_GameServerHelper.ShutdownWait(1000 * 30).Coroutine();
                            }
                        }
                    }
                }
            }
        }
    }
}
