using ETModel;
using ETModel.Robot;
using ETModel.Robot.EventType;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    // Hp变动，使用药瓶
    [NumericWatcher((int)E_GameProperty.PROP_HP)]
    public class NumericWatcher_Hp_UseItem : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            //if (args.New <= 0) return;
            // 单位死亡
            Unit unit = args.Parent as Unit;
            if (unit == null || unit.IsDisposed) return;
            Scene clientScene = unit.ClientScene();
            if (UnitHelper.GetLocalUnitFromClientScene(clientScene).Id != unit.Id) return;

            if (!CanUseItem(unit)) return;
            // 本地玩家，等级变动
            async Task UseItem()
            {
                for(int i =0;i<10;++i)
                {
                    using CoroutineLock coLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.RobotHpUseItem, unit.Id);
                    if (!CanUseItem(unit)) return;
                    RobotBackpackComponent backpack = unit.GetComponent<RobotBackpackComponent>();

                    RobotItem item = null;
                    List<RobotItem> itemList = new List<RobotItem>();
                    if(item == null)
                    {
                        itemList = backpack.GetItemsByConfigId(310045);    // 苹果
                        if (itemList.Count != 0)
                        {
                            item = itemList[0];
                        }
                    }
                    if (item == null)
                    {
                        itemList = backpack.GetItemsByConfigId(310002);    // 小瓶治疗药水
                        if (itemList.Count != 0)
                        {
                            item = itemList[0];
                        }
                    }
                    if (item == null)
                    {
                        itemList = backpack.GetItemsByConfigId(310003);    // 中瓶治疗药水
                        if (itemList.Count != 0)
                        {
                            item = itemList[0];
                        }
                    }
                    if (item == null)
                    {
                        itemList = backpack.GetItemsByConfigId(310004);    // 大瓶治疗药水
                        if (itemList.Count != 0)
                        {
                            item = itemList[0];
                        }
                    }
                    if (item == null) return;

                    bool ret = await RobotBackpackHelper.UseItem(unit, itemList[0]);
                    if (ret == false) return;
                }
           
            }

            UseItem().Coroutine();
        }

        public static bool CanUseItem(Unit localUnit)
        {
            NumericComponent numeric = localUnit.GetComponent<NumericComponent>();
            int hp = numeric.GetAsInt((int)E_GameProperty.PROP_HP);
            int hpMax = numeric.GetAsInt((int)E_GameProperty.PROP_HP_MAX);
            if (hp <= 0) return false;  // 玩家死亡
            if (hpMax <= 0) return false;
            if ((hp / (float)hpMax) > 0.8) return false;
            return true;
        }
    }
}
