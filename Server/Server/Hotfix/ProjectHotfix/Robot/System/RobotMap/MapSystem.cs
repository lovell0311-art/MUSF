using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using System.Linq;
using CustomFrameWork;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class MapDestroySystem : DestroySystem<Map>
    {
        public override void Destroy(Map self)
        {
            foreach(Unit unit in self.UnitDict.Values.ToArray()) unit.Dispose();

            self.UnitDict.Clear();
            self.PlayerDict.Clear();
            self.MonsterDict.Clear();
            self.ItemDict.Clear();
            self.PetDict.Clear();
            self.SummonedDict.Clear();
            self.NpcDict.Clear();
            self.LeavedUnitQueue.Clear();

            self.Astar = null;
        }

    }


    public static partial class MapSystem
    {
        public static bool AddUnit(this Map self, Unit unit)
        {
            Scene clientScene = self.ClientScene();
            if (unit.CurrentMap != null)
            {
                Log.Warning($"[{clientScene.Name}] 重复进入地图，unit.Id={unit.Id}");
                return false;
            }
            if (!self.UnitDict.TryAdd(unit.Id, unit))
            {
                Log.Warning($"[{clientScene.Name}] 重复进入地图，unit.Id={unit.Id}");
                return false;
            }
            Log.Debug($"[{clientScene.Name}] unit 进入视野:{unit.Id}");
            unit.CurrentMap = self;

            switch (unit.UnitType)
            {
                case UnitType.Player: self.PlayerDict.Add(unit.Id, unit); break;
                case UnitType.Monster: self.MonsterDict.Add(unit.Id, unit); break;
                case UnitType.Item: self.ItemDict.Add(unit.Id, unit); break;
                case UnitType.Pet: self.PetDict.Add(unit.Id, unit); break;
                case UnitType.Summoned: self.SummonedDict.Add(unit.Id, unit); break;
                case UnitType.Npc: self.NpcDict.Add(unit.Id, unit); break;
                default: break;
            }
            return true;
        }

        public static Unit GetUnit(this Map self,long id)
        {
            Unit unit = null;
            self.UnitDict.TryGetValue(id, out unit);
            return unit;
        }

        public static bool RemoveUnit(this Map self,long id)
        {
            if (self.UnitDict.TryGetValue(id, out Unit unit))
            {
                switch(unit.UnitType)
                {
                    case UnitType.Player: self.PlayerDict.Remove(unit.Id); break;
                    case UnitType.Monster: self.MonsterDict.Remove(unit.Id); break;
                    case UnitType.Item: self.ItemDict.Remove(unit.Id); break;
                    case UnitType.Pet: self.PetDict.Remove(unit.Id); break;
                    case UnitType.Summoned: self.SummonedDict.Remove(unit.Id); break;
                    case UnitType.Npc: self.NpcDict.Remove(unit.Id); break;
                    default:break;
                }

                self.UnitDict.Remove(id);
                unit.CurrentMap = null;
                Scene clientScene = self.ClientScene();
                Log.Debug($"[{clientScene.Name}] unit 离开视野:{unit.Id}");
                unit.GetComponent<ObjectWait>().Notify(new Wait_Unit_LeaveMap());
                return true;
            }
            return false;
        }


    }
}
