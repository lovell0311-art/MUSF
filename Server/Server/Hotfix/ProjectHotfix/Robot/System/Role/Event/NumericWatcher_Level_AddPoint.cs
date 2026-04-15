using ETModel;
using ETModel.Robot;
using ETModel.Robot.EventType;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    // 等级变动，获取玩家属性
    [NumericWatcher((int)E_GameProperty.Level)]
    public class NumericWatcher_Level_AddPoint : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            if (args.New <= 0) return;
            // 单位死亡
            Unit unit = args.Parent as Unit;
            if (unit == null || unit.IsDisposed) return;
            Scene clientScene = unit.ClientScene();

            if (UnitHelper.GetLocalUnitFromClientScene(clientScene).Id != unit.Id) return;

            // 本地玩家，等级变动
            async Task AddPoint()
            {
                bool ret = await RobotRoleHelper.RequestPlayerProperty(unit);
                if (ret == false) return;
                RobotRoleComponent role = unit.GetComponent<RobotRoleComponent>();
                int sum = role.AddPointWeight.Strength;
                sum += role.AddPointWeight.Agility;
                sum += role.AddPointWeight.Willpower;
                sum += role.AddPointWeight.BoneGas;
                sum += role.AddPointWeight.Command;
                if (sum <= 0) return;
                Dictionary<E_GameProperty, int> addPointNumber = new Dictionary<E_GameProperty, int>();

                int freePoint = unit.GetComponent<NumericComponent>().GetAsInt((int)E_GameProperty.FreePoint);
                addPointNumber[E_GameProperty.Property_Strength] = 0;
                addPointNumber[E_GameProperty.Property_Agility] = 0;
                addPointNumber[E_GameProperty.Property_Willpower] = 0;
                addPointNumber[E_GameProperty.Property_BoneGas] = 0;
                addPointNumber[E_GameProperty.Property_Command] = 0;
                for (int i = 0; i < freePoint; i++)
                {
                    int randomWeight = RandomHelper.RandomNumber(0, sum);
                    int weight = role.AddPointWeight.Strength;
                    if (randomWeight < weight)
                    {
                        addPointNumber[E_GameProperty.Property_Strength] += 1;
                        continue;
                    }
                    weight += role.AddPointWeight.Agility;
                    if (randomWeight < weight)
                    {
                        addPointNumber[E_GameProperty.Property_Agility] += 1;
                        continue;
                    }
                    weight += role.AddPointWeight.Willpower;
                    if (randomWeight < weight)
                    {
                        addPointNumber[E_GameProperty.Property_Willpower] += 1;
                        continue;
                    }
                    weight += role.AddPointWeight.BoneGas;
                    if (randomWeight < weight)
                    {
                        addPointNumber[E_GameProperty.Property_BoneGas] += 1;
                        continue;
                    }
                    weight += role.AddPointWeight.Command;
                    if (randomWeight < weight)
                    {
                        addPointNumber[E_GameProperty.Property_Command] += 1;
                        continue;
                    }
                }


                foreach (var kv in addPointNumber)
                {
                    if (kv.Value <= 0) continue;
                    ret = await RobotRoleHelper.AddPoint(unit, kv.Key, kv.Value);
                    if (ret == false) return;
                }
            }

            AddPoint().Coroutine();
        }
    }
}
