using ETModel;
using ETModel.Robot;
using ETModel.Robot.EventType;
using CustomFrameWork;
using System;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    [NumericWatcher((int)E_GameProperty.PROP_HP)]
    public class NumericWatcher_Hp_RemoveUnit : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            if (args.New > 0) return;
            // 单位死亡
            Unit unit = args.Parent as Unit;
            if (unit == null) return;
            Scene clientScene = unit.ClientScene();
            Log.Debug($"[{clientScene.Name}] Unit 死亡:{unit.Id}");
            switch (unit.UnitType)
            {
                case UnitType.Player:
                    {
                        Unit localUnit = clientScene.GetComponent<PlayerComponent>().LocalUnit;
                        if(unit.Id != localUnit.Id)
                        {
                            unit.Dispose();
                        }
                    }
                    break;
                case UnitType.Pet:
                    {
                        Unit localUnit = clientScene.GetComponent<PlayerComponent>().LocalUnit;
                        RobotPetComponent pet = unit.GetComponent<RobotPetComponent>();
                        if (pet.OwnerId == localUnit.Id)
                        {
                            // 是自己的宠物
                            async Task OpenPetsWindows()
                            {
                                RobotPetsWindowsComponent petsWin = clientScene.GetComponent<RobotPetsWindowsComponent>();
                                ETCancellationToken cancellationToken = new ETCancellationToken();
                                await petsWin.OpenPetsWindowsAsync(cancellationToken);
                            }
                            OpenPetsWindows().Coroutine();
                        }
                        unit.Dispose();
                    }
                    break;
                default:
                    {
                        unit.Dispose();
                    }
                    break;
            }
        }
    }
}
