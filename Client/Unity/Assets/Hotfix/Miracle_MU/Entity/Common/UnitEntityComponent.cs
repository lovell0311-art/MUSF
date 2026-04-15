using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;

namespace ETHotfix
{

    /// <summary>
    /// 战斗类型 （默认和平模式）
    /// </summary>
    public enum E_BattleType
    {
        Peace,//和平
        Whole,//全体
        Friendly,//友方

    }


    [ObjectSystem]
    public class UnitEntityComponentAwake : AwakeSystem<UnitEntityComponent>
    {
        public override void Awake(UnitEntityComponent self)
        {
            self.Awake();
        }
    }


    public enum E_AttackType
    {
        All,//全部实体
        Monster,//怪物实体
        Role,//玩家实体
    }
    /// <summary>
    /// 实体管理类
    /// </summary>
    public class UnitEntityComponent : Component
    {
        public static UnitEntityComponent Instance;
        private RoleEntity localrole;//本地玩家
        public long LocaRoleUUID => RoleArchiveInfoManager.Instance.curSelectRoleUUID;//本地玩家的uuid
        /// <summary>
        /// 所有实体字典
        /// </summary>
        public Dictionary<long, UnitEntity> AllUnitEntityDic = new Dictionary<long, UnitEntity>();

        

        public Dictionary<long, MonsterEntity> MonsterEntityDic = new Dictionary<long, MonsterEntity>();//怪物字典
        public Dictionary<long, RoleEntity> RoleEntityDic = new Dictionary<long, RoleEntity>();//玩家字典
        public Dictionary<long, PetEntity> PetEntityDic = new Dictionary<long, PetEntity>();//宠物字典
        public Dictionary<long, SummonEntity> SummonEntityDic = new Dictionary<long, SummonEntity>();//召唤兽
        public Dictionary<long, NPCEntity> NPCEntityDic = new Dictionary<long, NPCEntity>();//NPC
        public Dictionary<long, KnapsackItemEntity> KnapsackItemEntityDic = new Dictionary<long, KnapsackItemEntity>();//装备字典
        public Dictionary<long, UnitEntity> AttackEntityDic = new Dictionary<long, UnitEntity>();//可以攻击的实体
      
        /// <summary>
        /// 攻击类型
        /// </summary>
        public E_AttackType attackType = E_AttackType.All;


        public RoleEntity LocalRole
        {
            get
            {
                return this.localrole;
            }
            set
            {
                this.localrole = value;
                if (this.localrole != null)
                {
                    this.localrole.Parent = this;
                }
            }
        }
        /// <summary>
        /// 不能参与PK的实体
        /// </summary>
        public List<long> CanNotPkRoleList = new List<long>();

        public long curAttackEntity;//当前被击实体的uuid

        public UnitEntity curAttackUnitEntity;//当前攻击的实体

        public void Awake()
        {
            Instance = this;

            //十分钟 清理一次 600000
          //  TimerComponent.Instance.RegisterTimeCallBack(180000, TimeClear);

        }
        /// <summary>
        /// 刷新模型显示
        /// </summary>
        /// <param name="isshow"></param>
        public void RefreshModel(bool isshow)
        {
            return;
            foreach (var role in RoleEntityDic)
            {
             
                if (role.Key == LocaRoleUUID) continue;
               
                //隐藏/显示 装备
                RoleEquipmentComponent roleEquipmentComponent = role.Value.GetComponent<RoleEquipmentComponent>();
                ReferenceCollector collector = role.Value.Game_Object.GetReferenceCollector();
                bool isHaveFashion = false;
                //是否有时装
                for (E_Grid_Type i = E_Grid_Type.LeftRing; i <= E_Grid_Type.RightRing; i++)
                {
                    if (roleEquipmentComponent.curWareEquipsData_Dic.TryGetValue(i, out KnapsackDataItem ringitem))
                    {
                        Item_RingsConfig item_Ring = ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)ringitem.ConfigId);

                        if (item_Ring.IsTransRing == 1)
                        {
                            isHaveFashion = true;
                           
                            break;
                        }
                    }
                }
                List<int> curWare = new List<int>();
                //当前穿戴的装备
                foreach (var equip in roleEquipmentComponent.roleCurWare_EquipDic)
                {
                    if (equip.Key == E_Grid_Type.Mounts)
                    {
                        /* if (role.Value.IsSafetyZone)
                             equip.Value.SetActive(false); 
                         else
                             equip.Value.SetActive(isshow);*/

                        var obj = equip.Value.GetReferenceCollector().GetAssets(MonoReferenceType.Object);
                        foreach (GameObject item in obj.Cast<GameObject>())
                        {
                            if (item.name.Contains("RolePos") || item.name.Contains("rolePos")) continue;
                            if (role.Value.IsSafetyZone&& equip.Value.name != "Mount_YuJian")
                            {
                                if (item.GetComponent<SkinnedMeshRenderer>() is SkinnedMeshRenderer skinnedMeshRenderer && skinnedMeshRenderer != null)
                                {
                                    skinnedMeshRenderer.enabled = false;
                                }
                                else
                                {
                                    item.SetActive(false);
                                }
                                role.Value.GetComponent<AnimatorComponent>().SetBoolValue(MotionType.IsMount, false);
                            }
                            else
                            {
                                if (item.GetComponent<SkinnedMeshRenderer>() is SkinnedMeshRenderer skinnedMeshRenderer && skinnedMeshRenderer != null)
                                {
                                    skinnedMeshRenderer.enabled = isshow;
                                }
                                else
                                { 
                                 item.SetActive(isshow);
                                }
                                if (equip.Value.name != "Mount_YuJian")
                                {
                                    role.Value.GetComponent<AnimatorComponent>().SetBoolValue(MotionType.IsMount, isshow);
                                }
                                else
                                {
                                    role.Value.GetComponent<AnimatorComponent>().SetBoolValue(MotionType.IsMount, true);
                                }
                            }
                        }
                       
                        continue;
                    }
                    equip.Value.SetActive(isshow);
                    curWare.Add((int)equip.Key);
                }
                foreach (var defaultEquip in roleEquipmentComponent.roleDefault_EquipDic)
                {
                    if (curWare.Contains((int)defaultEquip.Key)) continue;
                    if (defaultEquip.Key >= E_Grid_Type.Helmet && defaultEquip.Key <= E_Grid_Type.Boots)
                    {
                        if (isHaveFashion == false)
                            defaultEquip.Value.SetActive(isshow);
                        else
                            defaultEquip.Value.SetActive(false);
                    }
                   
                }
                curWare.Clear();
                curWare = null;

               /* //坐骑
                if (roleEquipmentComponent.roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Mounts, out GameObject mount))
                {
                    if (role.Value.IsSafetyZone) { mount.SetActive(false); }
                    else
                        mount.SetActive(isshow);
                }*/

                //Buffer
                if (role.Value.GetComponent<BufferComponent>() is BufferComponent bufferComponent)
                {
                    foreach (var buffer in bufferComponent.BufferDic)
                    {
                        buffer.Value.SetActive(isshow);
                    }
                }

              if (roleEquipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Weapon, out KnapsackDataItem dataItem))
                 {

                     Transform weaponR = collector.GetGameObject("RightBackpos").transform;
                     
                     for (int r = 0; r < weaponR.childCount; r++)
                     {
                         weaponR.GetChild(r).gameObject.SetActive(isshow);
                     }

                 }
                if (roleEquipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Shield, out KnapsackDataItem dataItem_Shild))
                {

                    Transform weaponL = collector.GetGameObject("LeftBackPos").transform;
                  
                    for (int l = 0; l < weaponL.childCount; l++)
                    {
                        weaponL.GetChild(l).gameObject.SetActive(isshow);
                    }

                }
            }

            foreach (var pet in PetEntityDic)
            {
                if (pet.Value.RoleId == LocaRoleUUID) return;

                if (pet.Value.Game_Object != null)
                {

                    pet.Value.Game_Object.gameObject.SetActive(isshow);
                }
            }
           
        }

        public void SetUnitObjState(bool isshow,RoleEntity role) 
        {
            return;

            if (role.Id == LocaRoleUUID) return;
            //隐藏/显示 装备
            RoleEquipmentComponent roleEquipmentComponent = role.GetComponent<RoleEquipmentComponent>();
            if (role.Game_Object == null) return;
            ReferenceCollector collector = role.Game_Object.GetReferenceCollector();
            bool isHaveFashion = false;
            //是否有时装
            for (E_Grid_Type i = E_Grid_Type.LeftRing; i <= E_Grid_Type.RightRing; i++)
            {
                if (roleEquipmentComponent.curWareEquipsData_Dic.TryGetValue(i, out KnapsackDataItem ringitem))
                {
                    Item_RingsConfig item_Ring = ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)ringitem.ConfigId);

                    if (item_Ring.IsTransRing == 1)
                    {
                        isHaveFashion = true;
                        break;
                    }
                }
            }
            List<int> curWare = new List<int>();
            //当前穿戴的装备
            foreach (var equip in roleEquipmentComponent.roleCurWare_EquipDic)
            {
                if (equip.Key == E_Grid_Type.Mounts)
                {
                    var obj = equip.Value.GetReferenceCollector().GetAssets(MonoReferenceType.Object);
                    foreach (GameObject item in obj.Cast<GameObject>())
                    {
                        if (item.name.Contains("RolePos") || item.name.Contains("rolePos")) continue;
                        if (role.IsSafetyZone && equip.Value.name != "Mount_YuJian")
                        {
                            if (item.GetComponent<SkinnedMeshRenderer>() is SkinnedMeshRenderer skinnedMeshRenderer && skinnedMeshRenderer != null)
                            {
                                skinnedMeshRenderer.enabled = false;
                            }
                            else
                            {
                                item.SetActive(false);
                            }
                            role.GetComponent<AnimatorComponent>().SetBoolValue(MotionType.IsMount, false);
                        }
                        else
                        {
                            if (item.GetComponent<SkinnedMeshRenderer>() is SkinnedMeshRenderer skinnedMeshRenderer&& skinnedMeshRenderer!=null)
                            {

                             
                                skinnedMeshRenderer.enabled = isshow;
                            }
                            else
                            {
                                item.SetActive(isshow);
                            }
                            if (equip.Value.name != "Mount_YuJian")
                            {
                                role.GetComponent<AnimatorComponent>().SetBoolValue(MotionType.IsMount, isshow);
                            }
                            else
                            {
                                role.GetComponent<AnimatorComponent>().SetBoolValue(MotionType.IsMount, true);
                            }
                           
                        }
                    }
                    

                    continue;
                }

                equip.Value.SetActive(isshow);
                curWare.Add((int)equip.Key);
            }
            foreach (var defaultEquip in roleEquipmentComponent.roleDefault_EquipDic)
            {
                if (curWare.Contains((int)defaultEquip.Key)) continue;
                if (defaultEquip.Key >= E_Grid_Type.Helmet && defaultEquip.Key <= E_Grid_Type.Boots)
                {
                    if (isHaveFashion == false)
                        defaultEquip.Value.SetActive(isshow);
                    else
                        defaultEquip.Value.SetActive(false);
                }
                else
                {

                }
            }
            curWare.Clear();
            curWare = null;

           /* for (int i = 1; i < 16; i++)
            {
                E_Grid_Type e_Grid_ = (E_Grid_Type)i;

                if (roleEquipmentComponent.roleCurWare_EquipDic.TryGetValue(e_Grid_, out GameObject euipobj))
                {
                    euipobj.SetActive(isshow);
                    *//* if (isshow)
                     {
                         if (e_Grid_ == E_Grid_Type.Wing)
                         roleEquipmentComponent.animatorComponent.SetBoolValue(MotionType.IsWing,true);
                         else if (e_Grid_ == E_Grid_Type.Weapon)
                             roleEquipmentComponent.animatorComponent.SetBoolValue(MotionType.IsWeapon, true);
                     }*//*
                }
                else if (roleEquipmentComponent.roleDefault_EquipDic.TryGetValue(e_Grid_, out GameObject defaultObj))
                {
                    if (isHaveFashion == false)
                        defaultObj.SetActive(isshow);
                    else
                    {
                        defaultObj.SetActive(false);
                    }
                }
            }*/
            /*//坐骑
            if (roleEquipmentComponent.roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Mounts, out GameObject mount))
            {
                if (role.IsSafetyZone) { mount.SetActive(false); }
                else
                mount.SetActive(isshow);
            }*/

            //Buffer
            if (role.GetComponent<BufferComponent>() is BufferComponent bufferComponent)
            {
                foreach (var buffer in bufferComponent.BufferDic)
                {
                    buffer.Value.SetActive(isshow);
                }
            }
           

            if (roleEquipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Weapon, out KnapsackDataItem dataItem))
            {

                Transform weaponR = collector.GetGameObject("RightBackpos").transform;
                
                for (int r = 0; r < weaponR.childCount; r++)
                {
                    weaponR.GetChild(r).gameObject.SetActive(isshow);
                }

            }
            if (roleEquipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Shield, out KnapsackDataItem dataItem_Shild))
            {

                Transform weaponL = collector.GetGameObject("LeftBackPos").transform;
               
                for (int l = 0; l < weaponL.childCount; l++)
                {
                    weaponL.GetChild(l).gameObject.SetActive(isshow);
                }

            }
        }

        public void TimeClear()
        {
            GlobalDataManager.GCClear();
            TimerComponent.Instance.RegisterTimeCallBack(180000, TimeClear);
        }

        MonsterEntity lastmonster;
        float mindDis = 18;
        float nextdis = 1;
        //获取最近的怪物
        public void GetNearMonster()
        {
            attackType = attackType != E_AttackType.Monster ? E_AttackType.Monster : E_AttackType.All;
            ClickSelectUnitEntityComponent.Instance.ClearSelectUnit();
            if (attackType == E_AttackType.All)
            {
                curAttackUnitEntity = null;
                return;
            }
            if (MonsterEntityDic.Count == 0) return;
            //遍历所有敌人 计算距离比较
            var list = MonsterEntityDic.Values.ToList();
            foreach (var monsterKV in list)
            {
                var monster = monsterKV;
               
                if (lastmonster?.Id == monster.Id) continue;


                nextdis = PositionHelper.Distance(this.LocalRole.CurrentNodePos, monster.CurrentNodePos);//第i个怪物 与玩家之间的距离
                if (nextdis < mindDis)//将距离玩家最近的怪物 保留下来
                {
                    mindDis = (int)nextdis;//找到一个更近的
                    curAttackUnitEntity = monster;
                    lastmonster = monster;
                }
            }
            if (curAttackUnitEntity != null)
            {
                ClickSelectUnitEntityComponent.Instance.curSelectUnit = curAttackUnitEntity;
                ClickSelectUnitEntityComponent.Instance.SetSelectEffect();
            }
        
        }
        //获取附近的玩家
        public void GetNearRole()
        {
            attackType = attackType != E_AttackType.Role ? E_AttackType.Role : E_AttackType.All;
            ClickSelectUnitEntityComponent.Instance.ClearSelectUnit();
            if (attackType == E_AttackType.All)
            {
                curAttackUnitEntity = null;

                return;
            }


            if (RoleEntityDic.Count == 1) return;//本地玩家

            float mindDis = 18;
            int id = -1;
            //遍历所有敌人 计算距离比较
            foreach (var roleKV in RoleEntityDic)
            {

                if (AllUnitEntityDic.ContainsKey(roleKV.Key) == false) continue;

                //友方模式 不能攻击队友、好友、盟友
                if (TeamDatas.OtherTeamMemberStatusList.Exists(r => r.GameUserId == roleKV.Key) || FriendListData.FriendList.Exists(r => r.UUID == roleKV.Key) || WarAllianceDatas.WarMemberList.Exists(r => r.UUID == roleKV.Key))
                {
                    continue;
                }

                nextdis = PositionHelper.Distance(this.LocalRole.CurrentNodePos, roleKV.Value.CurrentNodePos);//第i个怪物 与玩家之间的距离
                if (nextdis < mindDis)//将距离玩家最近的怪物 保留下来
                {
                    mindDis = (int)nextdis;//找到一个更近的
                    curAttackUnitEntity = roleKV.Value;
                }
            }
            if (curAttackUnitEntity != null)
            {
                ClickSelectUnitEntityComponent.Instance.curSelectUnit = curAttackUnitEntity;
                ClickSelectUnitEntityComponent.Instance.SetSelectEffect();
            }

        }

        //缓存实体
        public void Add(UnitEntity unitEntity)
        {

            if (AllUnitEntityDic.ContainsKey(unitEntity.Id) == false)
            {
                AllUnitEntityDic[unitEntity.Id] = unitEntity;
            }
            if (unitEntity is SummonEntity summon)
            {
                SummonEntityDic[summon.Id] = summon;
                if (summon.Id != LocalRole.summonEntity.Id)
                {
                    AttackEntityDic[summon.Id] = summon;

                }
            }
            if (unitEntity is MonsterEntity monster)
            {
                MonsterEntityDic[monster.Id] = monster;
                AttackEntityDic[monster.Id] = monster;
               
            }
            else if (unitEntity is RoleEntity role)
            {
                RoleEntityDic[role.Id] = role;
              
                if (role.Id != LocaRoleUUID)
                {
                    AttackEntityDic[role.Id] = role;
                 
                }
            }
            else if (unitEntity is PetEntity pet)
            {
                PetEntityDic[pet.Id] = pet;
                if (pet.RoleId != LocaRoleUUID)
                {
                    AttackEntityDic[pet.Id] = pet;
                 
                }
            }
            else if (unitEntity is KnapsackItemEntity knapsack)
            {
                KnapsackItemEntityDic[knapsack.Id] = knapsack;
            }
            else if (unitEntity is NPCEntity npc)
            {
                NPCEntityDic[npc.Id] = npc;
            }
            
        }
        //移除实体
        public void Remove(long uuid)
        {
            if (SummonEntityDic.TryGetValue(uuid, out SummonEntity summonEntity))
            {
                if (LocalRole.summonEntity != null && LocalRole.summonEntity.Id == summonEntity.Id)
                {
                    LocalRole.summonEntity = null;
                }
                SummonEntityDic.Remove(uuid);
                summonEntity.Dispose();
            }
            if (MonsterEntityDic.TryGetValue(uuid, out MonsterEntity monster))
            {
                MonsterEntityDic.Remove(uuid);
             
                monster.Dispose();
            }
            else if (RoleEntityDic.TryGetValue(uuid, out RoleEntity role))
            {
               
                RoleEntityDic.Remove(uuid);
                role.Dispose();
            }
            else if (PetEntityDic.TryGetValue(uuid, out PetEntity pet))
            {
               
                PetEntityDic.Remove(uuid);
                pet.Dispose();
            }
            else if (KnapsackItemEntityDic.TryGetValue(uuid, out KnapsackItemEntity knapsackItem))
            {
                
                KnapsackItemEntityDic.Remove(uuid);
                knapsackItem.Dispose();
            }
            else if (NPCEntityDic.TryGetValue(uuid, out NPCEntity nPCEntity))
            {
                
                NPCEntityDic.Remove(uuid);
                nPCEntity.Dispose();
            }
            
          
            if (AttackEntityDic.TryGetValue(uuid, out UnitEntity attackunitEntity))
            {
               
                attackunitEntity.Dispose();
            }
            AttackEntityDic.Remove(uuid);
           // AttackEntityList.Remove(attackunitEntity);


            if (curAttackUnitEntity != null && curAttackUnitEntity.Id == uuid)
            {
                curAttackEntity = -1;
                curAttackUnitEntity = null;
            }
            if (RoleOnHookComponent.Instance != null &&
                RoleOnHookComponent.Instance.IsOnHooking &&
                RoleOnHookComponent.Instance.curAttackEntity != null &&
                RoleOnHookComponent.Instance?.curAttackEntity.Id == uuid)
            {
                RoleOnHookComponent.Instance.curAttackEntity = null;
            }
            ///当前选择的实体 死亡
            if (ClickSelectUnitEntityComponent.Instance != null && ClickSelectUnitEntityComponent.Instance.curSelectUnit != null && uuid == ClickSelectUnitEntityComponent.Instance.curSelectUnit.Id)
            {
                ClickSelectUnitEntityComponent.Instance.ClearSelectUnit();
            }

            if (AllUnitEntityDic.TryGetValue(uuid, out UnitEntity unitEntity))
            {

                AllUnitEntityDic.Remove(uuid);
                unitEntity.Dispose();

            }
           
        }
        //清理
        public void Clear(bool isclearLocaRole=true)
        {
            ClickSelectUnitEntityComponent.Instance?.ClearSelectUnit();
            curAttackEntity = -1;
            curAttackUnitEntity = null;
            AttackEntityDic.Clear();
            CanNotPkRoleList.Clear();

            //怪物字典
            List<long> monsterlist = new List<long>();
            var list = MonsterEntityDic.Values.ToList();
            foreach (var monster in list)
            {
                monsterlist.Add(monster.Id);
            }
            for (int i = 0; i < monsterlist.Count; i++)
            {
                MonsterEntityDic[monsterlist[i]].Dispose();
                MonsterEntityDic.Remove(monsterlist[i]);
            }
            MonsterEntityDic.Clear();
            //玩家
            List<RoleEntity> rolelist = new List<RoleEntity>();
            foreach (var role in RoleEntityDic)
            {
                rolelist.Add(role.Value);
            }
            for (int i = 0; i < rolelist.Count; i++)
            {
                //不清理本地玩家
                if (isclearLocaRole == false && rolelist[i].Id == LocaRoleUUID) continue;

                RoleEntityDic[rolelist[i].Id].Dispose();
                RoleEntityDic.Remove(rolelist[i].Id);

            }

            //NPC
            List<long> npclist = new List<long>();
            foreach (var npc in NPCEntityDic.Values)
            {
                npclist.Add(npc.Id);

            }
            for (int i = 0; i < npclist.Count; i++)
            {
                NPCEntityDic[npclist[i]].Dispose();
                NPCEntityDic.Remove(npclist[i]);
            }
            NPCEntityDic.Clear();
            //装备
            List<long> knapsackItemlist = new List<long>();
            foreach (var knapsackItem in KnapsackItemEntityDic.Values)
            {
                knapsackItemlist.Add(knapsackItem.Id);

            }
            for (int i = 0; i < knapsackItemlist.Count; i++)
            {
                KnapsackItemEntityDic[knapsackItemlist[i]].Dispose();
                KnapsackItemEntityDic.Remove(knapsackItemlist[i]);
            }
            KnapsackItemEntityDic.Clear();
            //召唤兽
            List<long> summonlist = new List<long>();
            foreach (var summon in SummonEntityDic.Values)
            {
                summonlist.Add(summon.Id);

            }
            for (int i = 0; i < summonlist.Count; i++)
            {
                SummonEntityDic[summonlist[i]].Dispose();
                SummonEntityDic.Remove(summonlist[i]);
            }
            SummonEntityDic.Clear();
            //宠物
            List<long> petlist = new List<long>();
            foreach (var pet in PetEntityDic.Values)
            {
                petlist.Add(pet.Id);

            }
            for (int i = 0; i < petlist.Count; i++)
            {
                PetEntityDic[petlist[i]].Dispose();
                PetEntityDic.Remove(petlist[i]);
            }
            PetEntityDic.Clear();

            List<UnitEntity> alllist = new List<UnitEntity>();
            foreach (var item in AllUnitEntityDic)
            {
                alllist.Add(item.Value);
            }
            for (int i = 0; i < alllist.Count; i++)
            {
                //不清理本地玩家
                if (isclearLocaRole == false && alllist[i].Id == LocaRoleUUID) continue;
                AllUnitEntityDic[alllist[i].Id].Dispose();
                AllUnitEntityDic.Remove(alllist[i].Id);
            }

            if (isclearLocaRole)
            {
                this.localrole = null;
            }

            if (UnitEntityFactory.petList.Count > 0)
            {
                List<PetEntity> chooseRolePets = UnitEntityFactory.petList.ToList();
                for (int i = 0; i < chooseRolePets.Count; i++)
                {
                    PetEntity chooseRolePet = chooseRolePets[i];
                    if (chooseRolePet != null && !chooseRolePet.IsDisposed)
                    {
                        chooseRolePet.Dispose();
                    }
                }

                UnitEntityFactory.petList.Clear();
            }

        }

        /// <summary>
        /// 根据uuid获取对应的实体
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public T Get<T>(long uid) where T : UnitEntity
        {

            if (this.AllUnitEntityDic.TryGetValue(uid, out UnitEntity entity))
            {
                return entity as T;
            }
            return null;
        }
        /// <summary>
        /// 根据GameObject 获取UnitEntity
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj"></param>
        /// <returns>对应的实体</returns>
        public T Get<T>(GameObject obj) where T : UnitEntity
        {
            if (obj == null) return null;
            foreach (var item in AllUnitEntityDic.Values)
            {
                if (item.Game_Object.GetInstanceID() == obj.GetInstanceID())
                {
                    return item as T;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取玩家周围全部实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns></returns>
        public List<T> GetAllEntity<T>() where T : UnitEntity
        {
            List<T> allentity = new List<T>();

            foreach (var item in AllUnitEntityDic)
            {
                if (item.Value is T t)
                {
                    allentity.Add(t);
                }
            }
            return allentity;

        }

        public void GetMinDistanceRoleEntity_Out(out RoleEntity entity)
        {
            entity = null;
            if (RoleEntityDic.ContainsKey(curAttackEntity))
            {
                entity = RoleEntityDic[curAttackEntity];
                return;
            }
          
            if (RoleEntityDic.Count == 0 || this.LocalRole == null) return;

            if (RoleEntityDic.Count == 1)
            {
                entity = null;
                return;
            };
            float mindDis = 0;//计算玩家 与第一个玩家之间的距离 将距离存到mindDis
            foreach (var role in RoleEntityDic)
            {
                if (role.Value.Id == LocaRoleUUID) continue;
                if (mindDis == 0)
                {
                    mindDis = PositionHelper.Distance2D(this.LocalRole.Position, role.Value.Position);
                }
                float tempdis = PositionHelper.Distance2D(this.LocalRole.Position, role.Value.Position);//第i个玩家 与玩家之间的距离
                if (tempdis <= mindDis) 
                {
                    mindDis = tempdis;
                    entity = role.Value;
                }
            }

            if (entity != null)
                curAttackEntity = entity.Id;
        }

        public KnapsackItemEntity GetKnapsackItemEntity(long uuid)
        {
            KnapsackItemEntity knapsackItemEntity = null;
            if (Get<KnapsackItemEntity>(uuid) is KnapsackItemEntity itemEntity)
            {
                knapsackItemEntity = itemEntity;
            }
            return knapsackItemEntity;
        }

        long unitID = -1;//与玩家最近的怪物的编号
        /// <summary>
        ///
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="originPos">挂机原点</param>
        /// <param name="distance">距离(格子数)=>默认15格（1格==2米）</param>
        public void GetMinDistacneEntity(ref UnitEntity entity, AstarNode originPos = null, int distance = 15)
        {
            unitID = -1;

            ///当前选择的实体 
            if (ClickSelectUnitEntityComponent.Instance.curSelectUnit != null)//当前选择的实体不为空
            {
                entity = ClickSelectUnitEntityComponent.Instance.curSelectUnit;
                bool canUseSelectedEntity =
                    entity != null
                    && CurAttackEntityIsDieOrLeveView(entity)
                    && CurEntityIsAttack(entity);

                if (canUseSelectedEntity && originPos != null)
                {
                    //原点挂机
                    canUseSelectedEntity = PositionHelper.Distance(originPos, entity.CurrentNodePos) < distance;
                }

                if (canUseSelectedEntity)
                {
                    curAttackUnitEntity = entity;
                    return;
                }

                ClickSelectUnitEntityComponent.Instance.ClearSelectUnit();
                entity = null;
            }

            if (originPos != null && curAttackUnitEntity != null)
            {
                var dis = PositionHelper.Distance(curAttackUnitEntity.CurrentNodePos, originPos);
                //玩家已经移动到挂机原点圈的边缘 则返回原点
                if (dis >= 10)
                {
                    curAttackUnitEntity = null;
                }
            }

            //当前正在攻击的实体是否为空
            if (curAttackUnitEntity != null
                && AllUnitEntityDic.ContainsKey(curAttackUnitEntity.Id)
                && CurAttackEntityIsDieOrLeveView(curAttackUnitEntity)
                && CurEntityIsAttack(curAttackUnitEntity))
            {
                entity = curAttackUnitEntity;
                return;
            }
            curAttackUnitEntity = null;
            int mindDis = originPos == null ? 14 : distance;//格子数
            switch (GlobalDataManager.BattleModel)
            {
                case E_BattleType.Peace://和平模式 只能攻击怪物
                                        //遍历所有敌人 计算距离比较
                    var list = MonsterEntityDic.Values.ToList();
                   foreach (var unit in list)
                    {
                        var monster = unit;
                        if (monster.Id == LocaRoleUUID) continue;
                        //实体为坐标点为不可行走区域
                        //if (monster is MonsterEntity && monster.CurrentNodePos.isWalkable == false)
                        //{
                        //    Log.DebugYellow($"{monster.Id} 在不可行走区域内：{monster.CurrentNodePos.ToString()}");
                        //    continue;
                        //};

                        // if (AllUnitEntityDic.ContainsKey(unit.Key) == false) continue;
                        if (CurEntityIsAttack(monster) == false) continue;

                        if (originPos == null)
                        {
                            nextdis = PositionHelper.Distance(this.LocalRole.CurrentNodePos, monster.CurrentNodePos);//第i个怪物 与玩家之间的距离
                            if (nextdis < mindDis)//将距离玩家最近的怪物 保留下来
                            {
                                mindDis = (int)nextdis;//找到一个更近的
                                unitID = monster.Id;
                            }
                        }
                        else
                        {
                            //原点挂机 

                            //判断挂机原点与怪物 之间是否有障碍物
                            /*  if (AstarComponent.Instance.IsHaveObstacle(originPos, unit.Value.CurrentNodePos))
                              {
                                 // Log.DebugRed($"有障碍物 ：{curentity.Id}");
                                  continue;
                              }*/

                            nextdis = PositionHelper.Distance(originPos, monster.CurrentNodePos);//第i个怪物 与挂机原点之间的距离
                            if (nextdis >= distance)
                            {
                                continue;
                            }
                            else
                            {
                                if (nextdis < mindDis)//将距离玩家最近的怪物 保留下来
                                {
                                    mindDis = (int)nextdis;//找到一个更近的
                                                           // id = i;
                                    unitID = monster.Id;
                                }
                            }
                        }

                    }
                    if (unitID != -1 && MonsterEntityDic.ContainsKey(unitID))
                    {
                        entity = MonsterEntityDic[unitID];
                    }
                    break;

                case E_BattleType.Whole:
                case E_BattleType.Friendly:
                    //遍历所有敌人 计算距离比较
                    foreach (var unit in AttackEntityDic)
                    {
                     
                        if (unit.Key == LocaRoleUUID) continue;
                        //实体为坐标点为不可行走区域
                        //if (unit is MonsterEntity && unit.CurrentNodePos.isWalkable == false)
                        //{
                        //    Log.DebugYellow($"{unit.Id} 在不可行走区域内：{unit.CurrentNodePos.ToString()}");
                        //    continue;
                        //};

                        if (CurEntityIsAttack(unit.Value) == false) continue;

                        if (originPos == null)
                        {
                            nextdis = PositionHelper.Distance(this.LocalRole.CurrentNodePos, unit.Value.CurrentNodePos);//第i个怪物 与玩家之间的距离
                            if (nextdis < mindDis)//将距离玩家最近的怪物 保留下来
                            {
                                mindDis = (int)nextdis;//找到一个更近的
                                unitID = unit.Key;
                            }
                        }
                        else
                        {
                            //判断挂机原点与怪物 之间是否有障碍物
                            /*  if (AstarComponent.Instance.IsHaveObstacle(originPos, unit.Value.CurrentNodePos))
                              {
                                 // Log.DebugRed($"有障碍物 ：{curentity.Id}");
                                  continue;
                              }*/

                            nextdis = PositionHelper.Distance(originPos, unit.Value.CurrentNodePos);//第i个怪物 与挂机原点之间的距离
                            if (nextdis >= distance)
                            {
                                continue;
                            }
                            else
                            {
                                if (nextdis < mindDis)//将距离玩家最近的怪物 保留下来
                                {
                                    mindDis = (int)nextdis;//找到一个更近的
                                    unitID = unit.Key;
                                }
                            }
                        }

                    }
                    if (unitID != -1 && AttackEntityDic.ContainsKey(unitID))
                    {
                        entity = AttackEntityDic[unitID];
                    }
                    break;

            }


            if (originPos != null && entity != null)
            {
                if (PositionHelper.Distance(originPos, entity.CurrentNodePos) > distance)//|| AstarComponent.Instance.IsHaveObstacle(originPos, entity.CurrentNodePos)
                {
                    entity = null;
                }
            }

            curAttackUnitEntity = entity;

        }
        //实体是否可以被攻击
        public bool CurEntityIsAttack(UnitEntity entity)
        {
            try
            {
                if (entity == null) return false;
                if (entity.IsDead) return false;
                if (entity is NPCEntity || entity is KnapsackItemEntity || entity.Id == LocaRoleUUID)//NPC、装备实体、本地玩家、本地玩家的宠物直接跳过
                {
                    return false;
                }
                if (entity is PetEntity pet && pet.RoleId == this.LocaRoleUUID)//本地玩家的宠物
                {
                    return false;
                }
                if (entity is SummonEntity sum && sum.Id == this.LocalRole.summonEntity?.Id)//本地玩家的 召唤兽
                {
                    return false;
                }

                if (entity is MonsterEntity monster && monster.CurrentNodePos.isWalkable == false)
                {
                    return false;
                }


                if (CanNotPkRoleList.Contains(entity.Id)) return false;

                switch (GlobalDataManager.BattleModel)
                {
                    case E_BattleType.Peace:

                        /* if (attackType == E_AttackType.Role)
                         {
                             return false;
                         }*/

                        if (entity is RoleEntity && attackType != E_AttackType.Role)
                        {
                            return false;
                        }
                        if (entity is SummonEntity)
                        {
                            return false;
                        }
                        break;
                    case E_BattleType.Whole:
                        return true;
                    case E_BattleType.Friendly:
                        //友方模式 不能攻击队友、好友、盟友
                        if (TeamDatas.OtherTeamMemberStatusList.Exists(r => r.GameUserId == entity.Id) || FriendListData.FriendList.Exists(r => r.UUID == entity.Id) || WarAllianceDatas.WarMemberList.Exists(r => r.UUID == entity.Id))
                        {
                            return false;
                        }
                        break;
                }

                return true;
            }
            catch (System.Exception e)
            {

                return false;
            }
           
        }


        /// <summary>
        /// 当前实体是否已经死亡
        /// </summary>
        /// <param name="unitEntity"></param>
        /// <returns>true:未死亡 false:死亡</returns>
        public bool CurAttackEntityIsDieOrLeveView(UnitEntity unitEntity)
        {
            return unitEntity != null && !unitEntity.IsDead && AllUnitEntityDic.ContainsKey(unitEntity.Id);
        }


        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            Clear();
        }
    }

}
