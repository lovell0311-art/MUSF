
using ETModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [ConsoleCommandLineAttribute(ConsoleCommandLinePath.area)]
    public class ConsoleCommandLine_Area : C_ConsoleCommandLine
    {
        public override async Task Run(string b_Contex)
        {
            switch (b_Contex)
            {
                case ConsoleCommandLinePath.area:
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
                case ConsoleCommandLinePath.area:
                    {//插入区服数据
                        {
                            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                            //Console.WriteLine($"mDBProxyManagerComponent:{mDBProxyManagerComponent}");

                            var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

                            List<ComponentWithId> mres = await dBProxy.Query<DBGameAreaData>(p => p.GameAreaId == 1);
                            //if (mres == null || mres.Count == 0)
                            {
#if DEVELOP
                                var msaveres = await dBProxy.Save(new DBGameAreaData()
                                {
                                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                                    GameAreaId = 1,
                                    RealLine = 1,
                                    NickName = "大天使删档内测1线",
                                    CreateTime = 0,
                                    State = 1
                                });
                                msaveres = await dBProxy.Save(new DBGameAreaData()
                                {
                                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                                    GameAreaId = 1,
                                    RealLine = 2,
                                    NickName = "大天使删档内测2线",
                                    CreateTime = 0,
                                    State = 1
                                });
                                msaveres = await dBProxy.Save(new DBGameAreaData()
                                {
                                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                                    GameAreaId = 1,
                                    RealLine = 3,
                                    NickName = "大天使删档内测3线",
                                    CreateTime = 0,
                                    State = 1
                                });
#else
                                var msaveres = await dBProxy.Save(new DBGameAreaData()
                                {
                                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                                    GameAreaId = 1,
                                    RealLine = 1,
                                    NickName = "大天使删档内测1线",
                                    CreateTime = 0,
                                    State = 1
                                });
                                msaveres = await dBProxy.Save(new DBGameAreaData()
                                {
                                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                                    GameAreaId = 1,
                                    RealLine = 2,
                                    NickName = "大天使删档内测2线",
                                    CreateTime = 0,
                                    State = 1
                                });
                                msaveres = await dBProxy.Save(new DBGameAreaData()
                                {
                                    Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId),
                                    GameAreaId = 1,
                                    RealLine = 3,
                                    NickName = "大天使删档内测3线",
                                    CreateTime = 0,
                                    State = 1
                                });
#endif
                                Log.Info($"msaveres : {msaveres.ToString()}");
                            }
                        }

                        if (false)
                        {
                            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                            var mDBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);

                            await mDBProxy.Save(new DBShowId()
                            {
                                Id = IdGeneraterNew.Instance.GenerateUnitId(DBProxyComponent.CommonDBId)

                            });
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
