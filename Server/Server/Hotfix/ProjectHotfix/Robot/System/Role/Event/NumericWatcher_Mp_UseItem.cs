using ETModel;
using ETModel.Robot;
using ETModel.Robot.EventType;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    // Mp变动，使用药瓶
    [NumericWatcher((int)E_GameProperty.PROP_MP)]
    public class NumericWatcher_Mp_UseItem : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            //if (args.New >= 0) return;
            // 单位死亡
            Unit unit = args.Parent as Unit;
            if (unit == null || unit.IsDisposed) return;
            Scene clientScene = unit.ClientScene();

            if (UnitHelper.GetLocalUnitFromClientScene(clientScene).Id != unit.Id) return;

            if (!CanUseItem(unit)) return;
            // 本地玩家，等级变动
            async Task UseItemCoroutine()
            {
                for(int i=0;i<10;++i)
                {
                    using CoroutineLock coLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.RobotMpUseItem, unit.Id);
                    if (!CanUseItem(unit)) return;
                    RobotBackpackComponent backpack = unit.GetComponent<RobotBackpackComponent>();

                    RobotItem item = null;
                    List<RobotItem> itemList = new List<RobotItem>();
                    if (item == null)
                    {
                        itemList = backpack.GetItemsByConfigId(310005);    // 小瓶魔力药水
                        if (itemList.Count != 0)
                        {
                            item = itemList[0];
                        }
                    }
                    if (item == null)
                    {
                        itemList = backpack.GetItemsByConfigId(310006);    // 中瓶魔力药水
                        if (itemList.Count != 0)
                        {
                            item = itemList[0];
                        }
                    }
                    if (item == null)
                    {
                        itemList = backpack.GetItemsByConfigId(310007);    // 大瓶魔力药水
                        if (itemList.Count != 0)
                        {
                            item = itemList[0];
                        }
                    }
                    if (item == null) return;

                    bool ret = await RobotBackpackHelper.UseItem(unit, item);
                    if (ret == false) return;
                }
            }

            UseItemCoroutine().Coroutine();
        }

        public static bool CanUseItem(Unit localUnit)
        {
            NumericComponent numeric = localUnit.GetComponent<NumericComponent>();
            int mp = numeric.GetAsInt((int)E_GameProperty.PROP_MP);
            int mpMax = numeric.GetAsInt((int)E_GameProperty.PROP_MP_MAX);
            if (mpMax <= 0) return false;
            if ((mp / (float)mpMax) > 0.8) return false;
            return true;
        }
    }
}
