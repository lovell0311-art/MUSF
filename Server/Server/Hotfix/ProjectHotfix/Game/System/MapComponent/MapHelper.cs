using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static class MapHelper
    {
        public static void MapEntityEnter(this MapComponent self,MapEntity entity,int posX,int posY)
        {
            if (self.MapEntityInMap(entity))
            {
                Log.Error($"Entity 重复请求进入 Map , entity.Id={entity.InstanceId}");
                return;
            }
            MapCellAreaComponent mapCellArea = self.GetMapCellFieldByPos(posX, posY);
            if(mapCellArea == null)
            {
                Log.Error($"Entity 进入 Map 错误的 Pos, entity.Id={entity.InstanceId} posX={posX} posY={posY}");
                return;
            }

            entity.Position.x = posX;
            entity.Position.y = posY;

            self.AddMapEntity(mapCellArea, entity);
        }

        public static void MapEntityLeave(this MapComponent self, MapEntity entity)
        {
            if (!self.MapEntityInMap(entity)) return;

            self.RemoveMapEntity(entity);
        }

        public static bool MapEntityInMap(this MapComponent self, MapEntity entity)
        {
            return self.AllEntities.ContainsKey(entity.InstanceId);
        }

        public static MapEntity GetMapEntityByInstanceId(this MapComponent self,long instanceId)
        {
            if(self.AllEntities.TryGetValue(instanceId,out MapEntity entity))
            {
                return entity;
            }
            return null;
        }

        private static void AddMapEntity(this MapComponent self, MapCellAreaComponent cellArea, MapEntity enteredEntity)
        {
            self.AllEntities.Add(enteredEntity.InstanceId, enteredEntity);
            enteredEntity.Parent = cellArea;
            if (enteredEntity is MapItem mapItem)
            {
                cellArea.MapItemRes.Add(mapItem.InstanceId, mapItem);
                ETModel.EventType.NSMapEntity.EnterMap.Instance.self = enteredEntity;
                ETModel.EventType.NSMapEntity.EnterMap.Instance.map = self;
                Root.EventSystem.OnRun("NSMapEntity.EnterMap.MapItem", ETModel.EventType.NSMapEntity.EnterMap.Instance);
            }


        }

        private static void RemoveMapEntity(this MapComponent self, MapEntity leavedEntity)
        {
            if (leavedEntity is MapItem mapItem)
            {
                leavedEntity.Parent.MapItemRes.Remove(mapItem.InstanceId);
                ETModel.EventType.NSMapEntity.LeaveMap.Instance.self = leavedEntity;
                ETModel.EventType.NSMapEntity.LeaveMap.Instance.map = self;
                Root.EventSystem.OnRun("NSMapEntity.LeaveMap.MapItem", ETModel.EventType.NSMapEntity.LeaveMap.Instance);
            }
            self.AllEntities.Remove(leavedEntity.InstanceId);
            leavedEntity.Parent = null;
        }



    }
}
