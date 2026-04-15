using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix.Robot
{
    public static class UnitFactory
    {
        public static Unit CreateMonster(Scene clientScene, long id,int configId)
        {
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (!readConfig.GetJson<Enemy_InfoConfigJson>().JsonDic.TryGetValue(configId, out Enemy_InfoConfig config))
            {
                Log.Error($"[{clientScene.Name}] 怪物配置不存在，configId={configId}");
                return null;
            }

            Unit unit = ComponentFactory.CreateWithId<Unit>(id);
            unit.UnitType = UnitType.Monster;
            unit.AddComponent<ClientSceneComponent, Scene>(clientScene);
            unit.AddComponent<ObjectWait>();
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<NumericComponent>();
            unit.AddComponent<RobotMonsterComponent>().Config = config;
            return unit;
        }

        public static Unit CreatePet(Scene clientScene, long id,int configId,long ownerId)
        {
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (!readConfig.GetJson<Pets_InfoConfigJson>().JsonDic.TryGetValue(configId, out Pets_InfoConfig config))
            {
                Log.Error($"[{clientScene.Name}] 宠物配置不存在，configId={configId}");
                return null;
            }
            Unit unit = ComponentFactory.CreateWithId<Unit>(id);
            unit.UnitType = UnitType.Pet;
            unit.AddComponent<ClientSceneComponent, Scene>(clientScene);
            unit.AddComponent<ObjectWait>();
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<NumericComponent>();
            unit.AddComponent<RobotPetComponent, Pets_InfoConfig>(config).OwnerId = ownerId;
            return unit;
        }

        public static Unit CreateSummoned(Scene clientScene, long id, int configId)
        {
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (!readConfig.GetJson<Enemy_InfoConfigJson>().JsonDic.TryGetValue(configId, out Enemy_InfoConfig config))
            {
                Log.Error($"[{clientScene.Name}] 怪物配置不存在，configId={configId}");
                return null;
            }
            Unit unit = ComponentFactory.CreateWithId<Unit>(id);
            unit.UnitType = UnitType.Summoned;
            unit.AddComponent<ClientSceneComponent, Scene>(clientScene);
            unit.AddComponent<ObjectWait>();
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<NumericComponent>();
            unit.AddComponent<RobotMonsterComponent>().Config = config;
            return unit;
        }

        public static Unit CreatePlayer(Scene clientScene, long id,E_GameOccupation RoleType)
        {
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (!readConfig.GetJson<CreateRole_InfoConfigJson>().JsonDic.TryGetValue((int)RoleType, out CreateRole_InfoConfig config))
            {
                Log.Error($"[{clientScene.Name}] 角色配置不存在，RoleType={RoleType}");
                return null;
            }
            Unit unit = ComponentFactory.CreateWithId<Unit>(id);
            unit.UnitType = UnitType.Player;
            unit.AddComponent<ClientSceneComponent, Scene>(clientScene);
            unit.AddComponent<ObjectWait>();
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<NumericComponent>();
            unit.AddComponent<RobotRoleComponent,CreateRole_InfoConfig>(config);
            unit.AddComponent<AttackComponent>();
            unit.AddComponent<RobotBackpackComponent>();
            unit.AddComponent<RobotEquipmentComponent>();
            unit.AddComponent<RobotSkillComponent>();

            return unit;
        }

        public static Unit CreateItem(Scene clientScene, long id, G2C_ItemDropData itemData)
        {
            ItemConfigManagerComponent itemConfigManager = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>();
            ItemConfig config = itemConfigManager.Get(itemData.Key);
            if(config == null)
            {
                Log.Error($"[{clientScene.Name}] 无法找到物品配置，configId={itemData.Key}");
                return null;
            }
            Unit unit = ComponentFactory.CreateWithId<Unit>(id);
            unit.IgnoreCollision = true;
            unit.UnitType = UnitType.Item;
            unit.AddComponent<ClientSceneComponent, Scene>(clientScene);
            unit.AddComponent<ObjectWait>();
            RobotItemComponent itemComponent = unit.AddComponent<RobotItemComponent, ItemConfig>(config);
            itemComponent.Count = itemData.Value;
            itemComponent.Quality = itemData.Quality;
            itemComponent.ProtectTick = itemData.ProtectTick;
            itemComponent.Level = itemData.Level;
            itemComponent.SetId = itemData.SetId;
            itemComponent.KillerId.AddRange(itemData.KillerId);

            return unit;
        }

        public static Unit CreateNpc(Scene clientScene,long id)
        {
            Unit unit = ComponentFactory.CreateWithId<Unit>(id);
            unit.UnitType = UnitType.Npc;
            unit.AddComponent<ClientSceneComponent, Scene>(clientScene);
            unit.AddComponent<ObjectWait>();
            unit.AddComponent<RobotNpcComponent>();
            return unit;
        }
    }
}
