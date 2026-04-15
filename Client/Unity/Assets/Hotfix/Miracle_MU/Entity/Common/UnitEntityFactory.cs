using ETModel;
using ILRuntime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace ETHotfix
{

    /// <summary>
    /// 创建实体组件
    /// </summary>
    public static class UnitEntityFactory
    {
        private const float DefaultCameraAngleH = -135f;
        private const float DefaultCameraAngleV = 45f;
        private const float DefaultCameraDistance = 35f;


        /// <summary>
        /// 创建本地玩家实体
        /// </summary>
        /// <returns>本地玩家实体RoleEntity</returns>
        public static RoleEntity CreatLocalRole()
        {
            RoleArchiveInfo roleArchiveInfo = RoleArchiveInfoManager.Instance.curSelectRoleArchiveInfo;
            long uuid = roleArchiveInfo.UUID;
            E_RoleType roleType = (E_RoleType)roleArchiveInfo.RoleType;
/*#if UNITY_EDITOR
            //弓箭手，魔法师，剑士才有新手引导，其他职业没有新手引导
            if (roleType != E_RoleType.Archer && roleType != E_RoleType.Magician && roleType != E_RoleType.Swordsman)
            {
                Guidance_Define.IsBeginnerGuide = false;
            }
#else
            //弓箭手，魔法师，剑士才有新手引导，其他职业没有新手引导
            if (roleType != E_RoleType.Archer && roleType != E_RoleType.Magician && roleType != E_RoleType.Swordsman)
            {
                Guidance_Define.IsBeginnerGuide = false;
            }
            else
            {
                Guidance_Define.IsBeginnerGuide = true;
            }
#endif*/
            string roleName = roleType.GetRoleResName();
            GameObject role = ResourcesComponent.Instance.LoadGameObject(roleName.StringToAB(), roleName);
            role.tag = Tags.LocaRole;
            role.transform.Find("Role").tag = Tags.LocaRole;
            role.SetLayer(LayerNames.LOCALROLE);

            RoleEntity roleEntity = ComponentFactory.CreateWithId<RoleEntity, GameObject>(uuid, role);
            roleEntity.RoleType = roleType;
            roleEntity.RoleName = roleArchiveInfo.Name;

            //roleEntity.AddComponent<CameraFollowComponent>();
            CameraFollowComponent.Instance.followTarget = role.transform;//role.transform.GetChild(0)
            ResetGameplayCameraView();
            LoginStageTrace.AppendWorldSnapshot("CreatLocalRole after reset");

            UnitEntityComponent.Instance.LocalRole = roleEntity;
            LoginStageTrace.Append("CreatLocalRole local-role-assigned");

            TryAddLocalRoleStep("AnimatorComponent", () => roleEntity.AddComponent<AnimatorComponent>());//动画组件
            TryAddLocalRoleStep("RoleMoveControlComponent", () => roleEntity.AddComponent<RoleMoveControlComponent>());
            TryAddLocalRoleStep("TurnComponent", () => roleEntity.AddComponent<TurnComponent>());
            TryAddLocalRoleStep("UnitEntityMoveComponent", () => roleEntity.AddComponent<UnitEntityMoveComponent>());
            TryAddLocalRoleStep("UnitEntityPathComponent", () => roleEntity.AddComponent<UnitEntityPathComponent>());
            TryAddLocalRoleStep("SiegeWarfareComponent", () => roleEntity.AddComponent<SiegeWarfareComponent>());//攻城战组件
            TryAddLocalRoleStep("SitDownStoolsComponent", () => roleEntity.AddComponent<SitDownStoolsComponent>());//老板娘坐下
            TryAddLocalRoleStep("UnitEntityHitTextComponent", () => roleEntity.AddComponent<UnitEntityHitTextComponent>().SetColor(Color.red));
            TryAddLocalRoleStep("RoleSkillComponent", () => roleEntity.AddComponent<RoleSkillComponent>());//技能组件
            TryAddLocalRoleStep("RoleOnHookComponent", () => roleEntity.AddComponent<RoleOnHookComponent>());//挂机组件
            TryAddLocalRoleStep("BufferComponent", () => roleEntity.bufferComponent = roleEntity.AddComponent<BufferComponent>());//Buffer组件
            TryAddLocalRoleStep("UIUnitEntityHpBarComponent", () => roleEntity.AddComponent<UIUnitEntityHpBarComponent>().SetEntityName($"<b>{roleArchiveInfo.Name}</b>", ColorTools.GetColorHtmlString(Color.yellow)));
            TryAddLocalRoleStep("RoleStallUpComponent", () => roleEntity.AddComponent<RoleStallUpComponent>());
            TryAddLocalRoleStep("ClickSelectUnitEntityComponent", () => roleEntity.AddComponent<ClickSelectUnitEntityComponent>());//选择实体组件
            TryAddLocalRoleStep("RoleEquipmentComponent", () => roleEntity.AddComponent<RoleEquipmentComponent>());//添加装备组件 显示装备

            //角色升级
            TryAddLocalRoleStep("UploadRoleInfo", () =>
            {
                if (ETModel.Init.instance.e_SDK == E_SDK.SHOU_Q)
                {
                    SdkCallBackComponent.Instance.sdkUtility.UploadRoleInfo(new string[] { $"{roleEntity.RoleName}", $"{GlobalDataManager.EnterZoneID}_{GlobalDataManager.EnterLineID}", $"{roleEntity.ClassLev}", $"{GlobalDataManager.EnterZoneName}", $"{uuid}", $"{GlobalDataManager.ShouQUUID}", "" });
                }
                else if (ETModel.Init.instance.e_SDK == E_SDK.HaXi)
                {
                    Log.Info("-------------------------------UploadRoleInfo ");
                    SdkCallBackComponent.Instance.sdkUtility.UploadRoleInfo(new string[] {
                        roleEntity.RoleName, // 角色名
                        roleEntity.Level.ToString(),// 角色等级
                        GlobalDataManager.EnterZoneID.ToString(),// 服务器ID
                        GlobalDataManager.EnterZoneName,// 服务器名
                        uuid.ToString(),// 角色ID
                        roleEntity.Property.GetProperValue(E_GameProperty.MoJing).ToString(),// 角色账户余额
                        "0",// 角色创建时间(秒)
                        "0", // 帮派/工会ID
                        "0",  // 帮派/工会
                        "0",    // 角色VIP等级
                        "0",     // 战斗力
                        roleEntity.ClassLev.ToString(),    // 转生等级
                        roleEntity.RoleType.ToString(),    // 角色职业
                    });
                }
            });

            UnitEntityComponent.Instance.Add(roleEntity);
            TryAddLocalRoleStep("TaskDatas.GetAllTask", () => TaskDatas.GetAllTask());//获取所有任务
            
            return roleEntity;
        }

        private static void TryAddLocalRoleStep(string step, Action action)
        {
            try
            {
                action?.Invoke();
                LoginStageTrace.Append($"CreatLocalRole step-ok {step}");
            }
            catch (Exception e)
            {
                LoginStageTrace.Append($"CreatLocalRole step-failed {step} type={e.GetType().Name} message={e.Message}");
                Log.Error($"CreatLocalRole step-failed {step}: {e}");
            }
        }

        private static void ResetGameplayCameraView()
        {
            CameraFollowComponent followComponent = CameraFollowComponent.Instance;
            if (followComponent == null)
            {
                return;
            }

            followComponent.curAngleH = DefaultCameraAngleH;
            followComponent.curAngleV = DefaultCameraAngleV;
            followComponent.distance = DefaultCameraDistance;
            followComponent.h = 1f;
            followComponent.ChangeScene = true;

            LocalDataJsonComponent.Instance.SavaData(new CameraInfo
            {
                curAngleH = DefaultCameraAngleH,
                curAngleV = DefaultCameraAngleV,
                distance = DefaultCameraDistance
            }, LocalJsonDataKeys.CameraInfo);
        }

        /// <summary>
        /// 创建远程玩家
        /// </summary>
        /// <param name="uuid">远程玩家的UUID</param>
        /// <param name="roleType">远程玩家的角色类型</param>
        /// <param name="roleName">远程玩家的角色名</param>
        /// <param name="pos">远程玩家的坐标</param>
        /// <returns>远程玩家实体RoleEntity</returns>
        public static RoleEntity CreatRemoteRole(long uuid, E_RoleType roleType, string roleName,long Title,string WarTitle, AstarNode NodePos, int angle,int ClassLev)
        {
            string roleResName = roleType.GetRoleResName();
            GameObject role = ResourcesComponent.Instance.LoadGameObject(roleResName.StringToAB(), roleResName);
            role.tag = Tags.Player;
            role.transform.Find("Role").tag = Tags.Player;
            role.SetLayer(LayerNames.ROLE);


            var pos = AstarComponent.Instance.GetVectory3(NodePos.x, NodePos.z);

            RoleEntity roleEntity = ComponentFactory.CreateWithId<RoleEntity, GameObject>(uuid, role);

            roleEntity.CurrentNodePos = NodePos;
            roleEntity.RoleType = roleType;
            roleEntity.RoleName = roleName;
            roleEntity.Property.ChangeProperValue(E_GameProperty.OccupationLevel,ClassLev);//转职等级

            
            //显示角色名字
            roleEntity.AddComponent<UIUnitEntityHpBarComponent>().SetEntityName(roleName);
            roleEntity.GetComponent<UIUnitEntityHpBarComponent>().SetEntityWarName(WarTitle);
            roleEntity.GetComponent<UIUnitEntityHpBarComponent>().SetEntityTitle(Title);
            roleEntity.AddComponent<AnimatorComponent>();//动画组件
         
            roleEntity.AddComponent<SiegeWarfareComponent>();//攻城战组件
            roleEntity.AddComponent<SitDownStoolsComponent>();//老板娘坐下

            roleEntity.AddComponent<UnitEntityMoveComponent>();
            roleEntity.AddComponent<TurnComponent>();
            roleEntity.AddComponent<UnitEntityPathComponent>();
            roleEntity.AddComponent<RoleSkillComponent>();//技能组件
            roleEntity.AddComponent<BufferComponent>();//Buffer组件
            roleEntity.AddComponent<UnitEntityHitTextComponent>().SetColor(Color.red);
            
            roleEntity.AddComponent<RoleStallUpComponent>();
            roleEntity.IsSafetyZone = SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int((int)pos.x / 2, (int)pos.z / 2));//是否再安全区
            try
            {

                roleEntity.AddComponent<RoleEquipmentComponent>().GetWareEquips().Coroutine();//装备组件
            }
            catch (Exception e)
            {
                Log.DebugGreen($"获取玩家属性装备报错 -> {e}");
                throw;
            }

            UnitEntityComponent.Instance.Add(roleEntity);
            
            role.transform.localPosition = pos.GroundPos();
            role.transform.localRotation = Quaternion.Euler(0, angle, 0);
            //TODO 装备隐藏

            //role.SetActive(!LocalDataJsonComponent.Instance.gameSetInfo.HideRole);
           
            return roleEntity;
        }
        /// <summary>
        /// 创建怪物实体
        /// </summary>
        /// <param name="uuid">怪物的UUID</param>
        /// <param name="configId">怪物的配置表ID</param>
        /// <returns>怪物实体MonsterEntity</returns>
        public static  void CreatMonster(long uuid, int configId, AstarNode NodePos, float angle)
        {
            EnemyConfig_InfoConfig enemy = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(configId);
            if (enemy == null)
            {
                Log.DebugRed($"{configId}--怪物配置不存在  uuid:{uuid}");
                return;
            }
            if (string.IsNullOrEmpty(enemy.ResName))
            {
                Log.DebugBrown($"{enemy.Name} 对应的模型资源不存在");
                return;
            }
            GameObject monsterObj = ResourcesComponent.Instance.LoadGameObject(enemy.ResName.StringToAB(), enemy.ResName);
            if (monsterObj == null)
            {
                monsterObj = ResourcesComponent.Instance.LoadGameObject("Monster_YouLong".StringToAB(), "Monster_YouLong");
            }
        
            //monsterObj.transform.localPosition = Vector3.zero;
            var Pos = AstarComponent.Instance.GetVectory3(NodePos);
            monsterObj.transform.localPosition = Pos.GroundPos();
            monsterObj.transform.localRotation = Quaternion.Euler(0, angle, 0);
            
            MonsterEntity monster = ComponentFactory.CreateWithId<MonsterEntity, GameObject, EnemyConfig_InfoConfig>(uuid, monsterObj, enemy);
            monster.CurrentNodePos = NodePos;
           
            monster.AddComponent<AnimatorComponent>();//动画组件
         
            monster.AddComponent<TurnComponent>();
            monster.AddComponent<UnitEntityMoveComponent>();
            monster.AddComponent<UnitEntityPathComponent>();
            monster.AddComponent<UnitEntityHitTextComponent>().SetColor(Color.yellow);
            monster.AddComponent<BufferComponent>();//Buffer组件

            if (enemy.Monster_Type == 1|| enemy.Monster_Type==6)
            {
                //显示Boss
                UIMainComponent.Instance?.ShowBossHp(enemy.Name);
            }
            else
            {
                //显示角色名字
                monster.AddComponent<UIUnitEntityHpBarComponent>().SetMonsterName(enemy.Name);
               monster.GetComponent<UIUnitEntityHpBarComponent>().ChangeState(false);
            }

            UnitEntityComponent.Instance.Add(monster);
           
        }
        //创建召唤兽
        public static SummonEntity CreatSummon(long uuid, int configId, AstarNode NodePos, float angle,long OwnerGameUserId)
        {
            EnemyConfig_InfoConfig enemy = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(configId);
            if (enemy == null)
            {
                Log.DebugRed($"{configId}--怪物不存在");
                return null;
            }
            if (string.IsNullOrEmpty(enemy.ResName))
            {
                Log.DebugBrown($"{enemy.Name} 对应的模型资源不存在");
                return null;
            }

            GameObject monsterObj = ResourcesComponent.Instance.LoadGameObject(enemy.ResName.StringToAB(), enemy.ResName);
            SummonEntity summon = ComponentFactory.CreateWithId<SummonEntity, GameObject, EnemyConfig_InfoConfig>(uuid, monsterObj, enemy);
            summon.AddComponent<AnimatorComponent>();//动画组件
          
            summon.AddComponent<TurnComponent>();
            summon.AddComponent<UnitEntityMoveComponent>();
            summon.AddComponent<UnitEntityPathComponent>();
            summon.AddComponent<UnitEntityHitTextComponent>().SetColor(Color.yellow);
            summon.AddComponent<BufferComponent>();//Buffer组件
            summon.CurrentNodePos = NodePos;
            var Pos = AstarComponent.Instance.GetVectory3(NodePos.x, NodePos.z);
            monsterObj.transform.localPosition = Pos.GroundPos();
            monsterObj.transform.localRotation = Quaternion.Euler(0, angle, 0);

            if (enemy.Id == 547)
            {
                monsterObj.transform.localPosition = Pos.GroundPos();
                summon.AddComponent<UIUnitEntityHpBarComponent>().ShowNpcName($"{enemy.Name}");
            }
            else
            {
                //显示角色名字
                summon.AddComponent<UIUnitEntityHpBarComponent>().SetMonsterName($"{enemy.Name}");
            }

            if (enemy.Name == "天鹰")
            {
                summon.GetComponent<UIUnitEntityHpBarComponent>().SetEntityName("天鹰");
            }
            Log.DebugBrown("天鹰创建了召唤兽");
            if (OwnerGameUserId == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                //Log.DebugGreen($"缓存本地玩家的召唤兽");
                UnitEntityComponent.Instance.LocalRole.summonEntity = summon;
                monsterObj.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                monsterObj.SetLayer(LayerNames.ROLE);
            }

            UnitEntityComponent.Instance.Add(summon);
            //AstartComponentExtend.MoveSendNotice(null, NodePos, uuid);

            return summon;
        }
        /// <summary>
        /// 创建NPC
        /// </summary>
        /// <param name="uuid">实体UUID</param>
        /// <param name="configID">配置表ID</param>
        /// <param name="pos">坐标</param>
        /// <param name="angle">角度</param>
        /// <returns>NPC实体NPCEntity</returns>
        public static NPCEntity CreatNPC(long uuid, long configID, AstarNode NodePos, float angle)
        {
            if (NodePos == null)
            {
                Log.DebugRed($"CreatNPC NodePos is null, uuid:{uuid}, config:{configID}");
                return null;
            }

            Npc_InfoConfig infoConfig = ConfigComponent.Instance.GetItem<Npc_InfoConfig>((int)configID);
            if (infoConfig == null)
            {
                Log.DebugRed($"CreatNPC config missing, uuid:{uuid}, config:{configID}");
                return null;
            }
            if (string.IsNullOrEmpty(infoConfig.ResName))
            {
                Log.DebugBrown($"{infoConfig.Name} 对应的模型资源不存在");
                return null;
            }
            GameObject npcobj = ResourcesComponent.Instance.LoadGameObject(infoConfig.ResName.StringToAB(), infoConfig.ResName);
            if (npcobj == null)
            {
                Log.DebugRed($"CreatNPC load model fail, uuid:{uuid}, config:{configID}, res:{infoConfig.ResName}");
                return null;
            }

            npcobj.SetLayer(LayerNames.NPC);

            var pos = AstarComponent.Instance.GetVectory3(NodePos.x, NodePos.z);
            npcobj.transform.localPosition = pos;
            npcobj.transform.localRotation = Quaternion.Euler(0, angle, 0);
           
            npcobj.transform.GroundPos();//矫正高度
        //    Log.DebugGreen($"{infoConfig.Name} {npcobj.transform.position}");
            NPCEntity nPCEntity = ComponentFactory.CreateWithId<NPCEntity, GameObject, int>(uuid, npcobj, (int)configID);
            nPCEntity.CurrentNodePos = NodePos;
            //显示角色名字
            nPCEntity.AddComponent<UIUnitEntityHpBarComponent>().ShowNpcName($"{infoConfig.Name}");
            UnitEntityComponent.Instance.Add(nPCEntity);

            //AstartComponentExtend.MoveSendNotice(null, NodePos, uuid);

            return nPCEntity;

        }
        /// <summary>
        /// 创建宠物
        /// </summary>
        /// <param name="uuid">实体UUID</param>
        /// <param name="configID">配置表ID</param>
        /// <param name="pos">坐标</param>
        /// <param name="angle">角度</param>
        /// <returns>NPC实体NPCEntity</returns>
        public static PetEntity CreatPet(long uuid, long configID, AstarNode NodePos, float angle,long OwnerGameUserId,long HpMaxValue, long MpMaxValue)
        {
            try
            {
                if (NodePos == null)
                {
                    Log.DebugRed($"CreatPet NodePos is null, uuid:{uuid}, config:{configID}");
                    return null;
                }

                Pets_InfoConfig infoConfig = ConfigComponent.Instance.GetItem<Pets_InfoConfig>((int)configID);
                if (infoConfig == null)
                {
                    Log.DebugRed($"CreatPet config missing, uuid:{uuid}, config:{configID}");
                    return null;
                }

                //Log.DebugGreen($"创建宠物{infoConfig.Name}");
                if (string.IsNullOrEmpty(infoConfig.PetModleAsset))
                {
                    Log.DebugBrown($"{infoConfig.Name} 对应的模型资源不存在");
                    return null;
                }
                GameObject petobj = ResourcesComponent.Instance.LoadGameObject(infoConfig.PetModleAsset.StringToAB(), infoConfig.PetModleAsset);
                if (petobj == null)
                {
                    Log.DebugRed($"CreatPet load model fail, uuid:{uuid}, config:{configID}, res:{infoConfig.PetModleAsset}");
                    return null;
                }
                if (OwnerGameUserId != UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    petobj.SetLayer(LayerNames.ROLE);
                }
                else
                {
                    petobj.SetLayer(LayerNames.LOCALROLE);
                }


                var pos = AstarComponent.Instance.GetVectory3(NodePos.x, NodePos.z);

                petobj.transform.localPosition = pos;
                petobj.transform.localRotation = Quaternion.Euler(0, angle, 0);
                // Log.DebugGreen($"矫正高度：{npcobj.transform.position}");
                petobj.transform.GroundPos();//矫正高度

                petobj.SetActive(true);
                //Log.DebugBrown($"矫正高度：{npcobj.transform.position}");
                PetEntity petEntity = ComponentFactory.CreateWithId<PetEntity, GameObject, Pets_InfoConfig>(uuid, petobj, infoConfig);
                petEntity.CurrentNodePos = NodePos;
                if (OwnerGameUserId != -1)
                {
                    petEntity.RoleId = OwnerGameUserId;
                    petEntity.MaxHp = HpMaxValue;
                    petEntity.MaxMp = MpMaxValue;
                }
                //显示宠物名字
                UnitEntity unitEntity = UnitEntityComponent.Instance.Get<UnitEntity>(OwnerGameUserId);
                if (unitEntity != null && unitEntity is RoleEntity roleEntity)
                {
                    UIUnitEntityHpBarComponent uIUnitEntityHpBarComponent = petEntity.AddComponent<UIUnitEntityHpBarComponent>(); 
                   // uIUnitEntityHpBarComponent.SetEntityName($"<color={ColorTools.PetNameColor}>{infoConfig.Name}</color>【{roleEntity.RoleName}】");
                    uIUnitEntityHpBarComponent.SetMonsterName($"<color={ColorTools.PetNameColor}>{infoConfig.Name}</color>【{roleEntity.RoleName}】");
                    uIUnitEntityHpBarComponent.SetOderLayer(1);
                }
                petEntity.AddComponent<AnimatorComponent>();//动画组件
              
                petEntity.AddComponent<UnitEntityMoveComponent>();
                petEntity.AddComponent<TurnComponent>();
                petEntity.AddComponent<UnitEntityPathComponent>();
                petEntity.AddComponent<UnitEntityHitTextComponent>().SetColor(Color.yellow);
                //petEntity.AddComponent<UIUnitEntityHpBarComponent>();
                petEntity.AddComponent<BufferComponent>();//Buffer组件

                UnitEntityComponent.Instance.Add(petEntity);
               
                //AstartComponentExtend.MoveSendNotice(null, NodePos, uuid);
                if (OwnerGameUserId != UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    petobj.SetActive(!LocalDataJsonComponent.Instance.gameSetInfo.HideRole);
                }
                return petEntity;
            }
            catch (Exception e)
            {
                Log.DebugGreen($"宠物报错 {e.StackTrace}\n{e.Message}");
                throw;
            }
            

        }
        /// <summary>
        /// 创建宠物 （无攻击）
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="configId"></param>
        /// <param name="OwnerGameUserId"></param>
        /// <param name="NodePos"></param>
        /// <returns></returns>
        public static void CreatPet(long uuid, long configId,long OwnerGameUserId, AstarNode NodePos)
        {
            Item_PetConfig item_Pet = ConfigComponent.Instance.GetItem<Item_PetConfig>((int)configId);
            if (string.IsNullOrEmpty(item_Pet.ResName))
            {
                Log.DebugBrown($"{item_Pet.ResName} 对应的模型资源不存在");
                return;
            }
            var resName = item_Pet.ResName.Replace("_beibaoUI", "").Trim();
            GameObject petobj = ResourcesComponent.Instance.LoadGameObject(resName.StringToAB(), resName);
            if (OwnerGameUserId != UnitEntityComponent.Instance.LocaRoleUUID)
            {
                petobj.SetLayer(LayerNames.ROLE);
            }
            else
            {
                petobj.SetLayer(LayerNames.LOCALROLE);
            }
            PetEntity petEntity = ComponentFactory.CreateWithId<PetEntity, GameObject>(uuid, petobj);
            //显示宠物名字
            UnitEntity unitEntity = UnitEntityComponent.Instance.Get<UnitEntity>(OwnerGameUserId);
            if (NodePos != null)
            {
                NodePos = GetNearNode(NodePos,3);
                var pos = AstarComponent.Instance.GetVectory3(NodePos.x, NodePos.z);
                petobj.transform.localPosition = pos;
                petEntity.CurrentNodePos = NodePos;
            }
            else
            {
                petobj.transform.localPosition = unitEntity.Game_Object.transform.position;
                
            }
            petobj.transform.localRotation = unitEntity.Game_Object.transform.rotation;
            petobj.transform.GroundPos();//矫正高度

            petobj.SetActive(true);
            if (unitEntity != null && unitEntity is RoleEntity roleEntity)
            {
                UIUnitEntityHpBarComponent uIUnitEntityHpBarComponent = petEntity.AddComponent<UIUnitEntityHpBarComponent>();
                uIUnitEntityHpBarComponent.SetEntityName($"<color={ColorTools.PetNameColor}>{item_Pet.Name}</color>【{roleEntity.RoleName}】");
                uIUnitEntityHpBarComponent.SetOderLayer(1);
            }
            petEntity.AddComponent<AnimatorComponent>();//动画组件
            petEntity.AddComponent<UnitEntityMoveComponent>();
            petEntity.AddComponent<TurnComponent>();
            petEntity.AddComponent<UnitEntityPathComponent>();
            UnitEntityComponent.Instance.Add(petEntity);
            
        }


        public static List<PetEntity> petList = new List<PetEntity>();
        public static void CreatPet_ChooseRole(long uuid, long configId, long OwnerGameUserId,RoleEntity role)
        {
            Item_PetConfig item_Pet = ConfigComponent.Instance.GetItem<Item_PetConfig>((int)configId);
            if (string.IsNullOrEmpty(item_Pet.ResName))
            {
                Log.DebugBrown($"{item_Pet.ResName} 对应的模型资源不存在");
                return;
            }
            var resName = item_Pet.ResName.Replace("_beibaoUI", "").Trim();
            GameObject petobj = ResourcesComponent.Instance.LoadGameObject(resName.StringToAB(), resName);
            if (OwnerGameUserId != UnitEntityComponent.Instance.LocaRoleUUID)
            {
                petobj.SetLayer(LayerNames.ROLE);
            }
            else
            {
                petobj.SetLayer(LayerNames.LOCALROLE);
            }

          //  Debug.Log("当前位置" + role.Game_Object.transform.position + "LLL" + role.Game_Object.transform.name);
            var pos = role.Game_Object.transform.position + (Vector3.left) * 2;
            pos.y = 1f;
            petobj.transform.localPosition = pos;
           // petobj.transform.localRotation = role.Game_Object.transform.rotation;
            petobj.transform.position = new Vector3(-2, 1, 4);// role.Game_Object.transform.position;
            petobj.transform.localRotation = new Quaternion(0, 0, 0, 0);
           // Log.Debug("kkk" + role.Game_Object.transform.position.x);
            //petobj.transform.GroundPos();//矫正高度
            petobj.SetActive(true);
            petobj.transform.SetParent(role.Game_Object.transform, false);
            PetEntity petEntity = ComponentFactory.CreateWithId<PetEntity, GameObject>(uuid, petobj);
           
            petEntity.AddComponent<AnimatorComponent>();//动画组件

            petList.Add(petEntity);
        }

        /// <summary>
        /// 获取nearNode 周围的随机位置
        /// </summary>
        /// <param name="nearNode"></param>
        /// <returns></returns>
        public static AstarNode GetNearNode(AstarNode nearNode, int randomvalue)
        {

            AstarNode astarNode = null;

            for (int i = 0; i < 10; i++)
            {
                int x = RandomHelper.RandomNumber(-randomvalue, randomvalue);
                int y = RandomHelper.RandomNumber(-randomvalue, randomvalue);

                if (x == y && x == 0)
                {
                    continue;
                }
                AstarNode node = AstarComponent.Instance.GetNode(nearNode.x + x, nearNode.z + y);
                if (node == null) continue;
                if (node.isWalkable == false) continue;
                if (IsNull(node) == false)
                {
                    continue;
                }
                return node;
            }

            return astarNode;

            bool IsNull(AstarNode node)
            {

                if (node.isWalkable == false)
                {
                    return false;
                }

                List<RoleEntity> allentity = UnitEntityComponent.Instance.RoleEntityDic.Values.ToList();

                for (int k = 0; k < allentity.Count; k++)
                {
                    var item = allentity[k];
                    if (node.Compare(item.CurrentNodePos))
                    {
                        astarNode ??= node;
                        //当前格子有装备
                        return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// 创建 掉落物品
        /// </summary>
        /// <param name="ItemDropData">掉落物品 的属性结构</param>

        public static KnapsackItemEntity CreatKnapsackItemEntity(ItemDropDataInfo ItemDropData, AstarNode NodePos)
        {
            KnapsackItemEntity knapsackData = ComponentFactory.CreateWithId<KnapsackItemEntity, ItemDropDataInfo>(ItemDropData.InstanceId, ItemDropData);
            knapsackData.CurrentNodePos = NodePos;
            UnitEntityComponent.Instance.Add(knapsackData);
            //AstartComponentExtend.MoveSendNotice(null, NodePos, knapsackData.Id);
            return knapsackData;
        }
    }
}
