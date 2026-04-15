using UnityEngine;
using ETModel;
using System;
using DG.Tweening;

using System.Linq;

namespace ETHotfix
{
    [MessageHandler]
    public partial class G2C_MovePos_notice_Handler : AMHandler<G2C_MovePos_notice>
    {
        protected override  void Run(ETModel.Session session, G2C_MovePos_notice message)
        {
            if (GlobalDataManager.IsEnteringGame || message.GameUserId == RoleArchiveInfoManager.Instance.LoadRoleUUID)
            {
                LoginStageTrace.Append(
                    $"MovePos notice gameUserId={message.GameUserId} loadRole={RoleArchiveInfoManager.Instance.LoadRoleUUID} " +
                    $"view={message.ViewId} unitType={message.UnitType} map={message.MapId} x={message.X} y={message.Y} " +
                    $"needMove={message.IsNeedMove} scene={SceneComponent.Instance?.CurrentSceneName} entering={GlobalDataManager.IsEnteringGame}");
            }


          //  Log.DebugBrown("打印转生==>G2C_MovePos_notice" + JsonHelper.ToJson(message));
            int ViewId = message.ViewId;
            int UnitType=message.UnitType; 

            //卡里玛 七层
            if (message.MapId >= 103 && message.MapId <= 109) 
            {
                message.MapId = 103;
            }

         
            //加载场景
            if (message.GameUserId == RoleArchiveInfoManager.Instance.LoadRoleUUID)
            {
                try
                {
                    if (message.MapId != 0)
                    {
                        if (GlobalDataManager.IsEnteringGame && GlobalDataManager.EnterGameMovePosProcessed)
                        {
                            LoginStageTrace.Append(
                                $"MovePos local duplicate skip map={message.MapId} x={message.X} y={message.Y} view={message.ViewId} needMove={message.IsNeedMove}");
                            return;
                        }

                        if (GlobalDataManager.IsEnteringGame)
                        {
                            GlobalDataManager.EnterGameMovePosProcessed = true;
                        }

                        //  UnitEntityComponent.Instance.LocalRole.IsDead = true;
                        SceneName sceneName = (SceneName)message.MapId;
                        LoginStageTrace.Append($"MovePos local scene-load map={sceneName} x={message.X} y={message.Y} needMove={message.IsNeedMove}");
                        //开始加载场景
                        CameraFollowComponent.Instance.ChangeScene = true;
                        UnitEntityComponent.Instance.LocalRole.GetComponent<UnitEntityPathComponent>()?.StopMove();
                        SceneComponent.Instance.LoadScene(sceneName.ToString(), callback: CheckSafe);//加载场景
                        UnitEntityComponent.Instance.LocalRole.ChangeRolePos(AstarComponent.Instance.GetNode(message.X, message.Y), callback: CheckSafe); //改变玩家的位置
                        Game.EventCenter.EventTrigger<int>(EventTypeId.CHARGE_MAP, (int)sceneName);
                        //玩家被击
                        if (message.IsNeedMove == 1)
                        {
                            UnitEntityComponent.Instance.LocalRole.Position = AstarComponent.Instance.GetVectory3(message.X, message.Y).GroundPos();
                            UnitEntityComponent.Instance.LocalRole.CurrentNodePos = AstarComponent.Instance.GetNode(message.X, message.Y);
                            if (UIComponent.Instance.Get(UIType.UISceneLoading) != null)
                            {
                                TimerComponent.Instance.RegisterTimeCallBack(1300, () => { UIComponent.Instance.Remove(UIType.UISceneLoading); });
                            }
                            return;
                        }

                        //显示称号和战盟

                        if (UnitEntityComponent.Instance.LocalRole.GetComponent<UIUnitEntityHpBarComponent>() is UIUnitEntityHpBarComponent uIUnitEntityHpBar)
                        {
                            uIUnitEntityHpBar.SetEntityWarName(message.WallTitle);
                            uIUnitEntityHpBar.SetEntityTitle(message.Title);
                        }
                        //设置本地玩家的坐标信息
                        if (UnitEntityComponent.Instance.LocalRole.IsDead)
                        {
                            UnitEntityComponent.Instance.LocalRole.LocalRoleRevive(AstarComponent.Instance.GetVectory3(message.X, message.Y));//玩家复活
                            UIMainComponent.Instance?.HideDeadMask();//隐藏死亡遮罩
                            UnitEntityComponent.Instance.LocalRole.IsDead = false;
                        }
                    }
                    else
                    {
                        LoginStageTrace.Append($"MovePos local skip-zero-map gameUserId={message.GameUserId}");
                    }
                }
                catch (Exception e)
                {
                    LoginStageTrace.Append($"MovePos local exception type={e.GetType().Name} message={e.Message}");
                    throw;
                }

                //判断是否在安全区
                void CheckSafe()
                {
                   
                    Vector2Int vector2Int = new Vector2Int((int)UnitEntityComponent.Instance.LocalRole.Position.x / 2, (int)UnitEntityComponent.Instance.LocalRole.Position.z / 2);
                    //矫正高度
                    UnitEntityComponent.Instance.LocalRole.Position = UnitEntityComponent.Instance.LocalRole.Position.GroundPos();
                    bool isSafe = SafeAreaComponent.Instances.IsSafeAreas(vector2Int);//是否在安全区
                   
                    if (isSafe != UnitEntityComponent.Instance.LocalRole.IsSafetyZone)
                    {
                        UnitEntityComponent.Instance.LocalRole.IsSafetyZone = isSafe;
                        UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>()?.EnterSafeArea(isSafe);
                    }
                    //设置周围玩家状态 
                    var allRoles = UnitEntityComponent.Instance.RoleEntityDic.Values.ToList();
                    foreach (var roleEntity in allRoles)
                    {
                        if (UnitEntityComponent.Instance.RoleEntityDic.ContainsKey(roleEntity.Id) == false) continue;
                        if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID) continue;
                        roleEntity.Position = roleEntity.Position.GroundPos();
                        roleEntity.GetComponent<RoleEquipmentComponent>()?.EnterSafeArea(isSafe);
                    }
                    var allNpcs = UnitEntityComponent.Instance.NPCEntityDic.Values.ToList();
                    foreach (var npc in allNpcs)
                    {
                        if (UnitEntityComponent.Instance.NPCEntityDic.ContainsKey(npc.Id) == false) continue;
                        npc.Position = npc.Position.GroundPos();

                    }
                    var allMonsters = UnitEntityComponent.Instance.MonsterEntityDic.Values.ToList();
                    foreach (var monster in allMonsters)
                    {
                        if (UnitEntityComponent.Instance.MonsterEntityDic.ContainsKey(monster.Id) == false) continue;
                        monster.Position = monster.Position.GroundPos();
                    }

                }
            }
            else 
            {

                ///message.ViewId:->0:视野区域中 1：进入视野区域 2：离开视野区域
                ///message.UnitType:->0:玩家 1：怪物 2：NPC 3:宠物 4：召唤兽

                //销毁
                if (ViewId == 2)
                {
                   
                    UnitEntityComponent.Instance.Remove(message.GameUserId);
                }
                //玩家进入视野
                else if (ViewId == 1 && UnitType == 0)
                {
                    if (UnitEntityComponent.Instance.AllUnitEntityDic.ContainsKey(message.GameUserId) || GlobalDataManager.ChangeSceneIsChooseRole) return;

                    RoleEntity role = UnitEntityFactory.CreatRemoteRole(message.GameUserId, (E_RoleType)message.ModelId, message.NickName, message.Title, message.WallTitle, AstarComponent.Instance.GetNode(message.X, message.Y), message.Angle, message.OccupationLevel);

                    role.Property.ChangeProperValue(E_GameProperty.PkNumber, message.PkNumber);//PK点数
                    role.GetComponent<UIUnitEntityHpBarComponent>().ChangeNameColor(role.GetRedNameColor());
                    role.GetComponent<UIUnitEntityHpBarComponent>().SetElesReincarnation(message.ReincarnateCnt);
                    if (SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(message.X, message.Y)) != role.IsSafetyZone)
                    {
                        bool isSafe = SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(message.X, message.Y));
                        role.IsSafetyZone = isSafe;
                        role.GetComponent<RoleEquipmentComponent>().EnterSafeArea(isSafe);
                    }
                    UnitEntityComponent.Instance.SetUnitObjState(GlobalDataManager.IsHideRole, role);

                }
                //怪物进入视野
                else if (ViewId == 1 && UnitType == 1)
                {
                    if (UnitEntityComponent.Instance.AllUnitEntityDic.ContainsKey(message.GameUserId))
                    {
                        return;
                        //原地复活
                    }
                   
                    UnitEntityFactory.CreatMonster(message.GameUserId, message.ModelId, AstarComponent.Instance.GetNode(message.X, message.Y), message.Angle);
                   
                }
                //NPC
                else if (ViewId == 1 && UnitType == 2)
                {
                    if (UnitEntityComponent.Instance.AllUnitEntityDic.ContainsKey(message.GameUserId)) return;
                    UnitEntityFactory.CreatNPC(message.GameUserId, message.ModelId, AstarComponent.Instance.GetNode(message.X, message.Y), message.Angle);
                }
                //宠物
                else if (ViewId == 1 && UnitType == 3)
                {
                   
                    if (UnitEntityComponent.Instance.PetEntityDic.TryGetValue(message.GameUserId, out PetEntity petEntity))
                    {
                        petEntity.GetComponent<UnitEntityPathComponent>()?.StopMove();
                        petEntity.CurrentNodePos = AstarComponent.Instance.GetNode(message.X, message.Y);
                        petEntity.Position = AstarComponent.Instance.GetVectory3(AstarComponent.Instance.GetNode(message.X, message.Y)).GroundPos();

                    }
                    else
                    {
                       
                        UnitEntityFactory.CreatPet(message.GameUserId, message.ModelId, AstarComponent.Instance.GetNode(message.X, message.Y), message.Angle, message.OwnerGameUserId, message.HpMaxValue, message.MpMaxValue);
                    }

                    //缓存宠物
                    if (message.OwnerGameUserId == UnitEntityComponent.Instance.LocaRoleUUID)
                    {
                        PetArchiveInfoManager.Instance.petId = message.GameUserId;
                        PetArchiveInfoManager.Instance.Name = message.NickName;
                        PetArchiveInfoManager.Instance.HpMaxValue = message.HpMaxValue;
                        PetArchiveInfoManager.Instance.MpMaxValue = message.MpMaxValue;
                    }
                }
                //召唤兽
                else if (ViewId == 1 && (UnitType == 5 || UnitType == 4))
                {
                    if (UnitEntityComponent.Instance.AllUnitEntityDic.ContainsKey(message.GameUserId)) return;
                  UnitEntityFactory.CreatSummon(message.GameUserId, message.ModelId, AstarComponent.Instance.GetNode(message.X, message.Y), message.Angle, message.OwnerGameUserId);

                   
                }
                //玩家视野范围内移动
                else if (ViewId == 0 && UnitType == 0)
                {
                    if (UnitEntityComponent.Instance.RoleEntityDic.TryGetValue(message.GameUserId, out RoleEntity roleEntity))
                    {
                        //判断是否在安全区
                        if (SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(message.X, message.Y)) != roleEntity.IsSafetyZone)
                        {
                            bool isSafe = SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(message.X, message.Y));
                            roleEntity.IsSafetyZone = isSafe;
                            roleEntity.GetComponent<RoleEquipmentComponent>().EnterSafeArea(isSafe);
                        }
                        if (message.IsNeedMove == 1) //被击
                        {
                            roleEntity.CurrentNodePos = AstarComponent.Instance.GetNode(message.X, message.Y);
                            roleEntity.Position = AstarComponent.Instance.GetVectory3(message.X, message.Y);

                        }
                        else
                        {
                            roleEntity.GetComponent<UnitEntityPathComponent>()?.StartMove(AstarComponent.Instance.GetNode(message.X, message.Y)).Coroutine();
                        }
                    }
                }
                //怪物
                else if (ViewId == 0 && UnitType == 1)
                {
                    if (UnitEntityComponent.Instance.MonsterEntityDic.TryGetValue(message.GameUserId, out MonsterEntity monster))
                    {
                        if (message.IsNeedMove == 1) //被击
                        {
                            if (monster.IsDead == false)
                            {
                                var monsterpos = AstarComponent.Instance.GetVectory3(message.X, message.Y).GroundPos();
                                monster.Game_Object.transform.DOMove(monsterpos, 1);
                                monster.CurrentNodePos = AstarComponent.Instance.GetNode(message.X, message.Y);
                            }

                        }
                        else
                        {

                            if (monster.GetComponent<UnitEntityPathComponent>() != null && !monster.IsDead)
                            {
                                monster.GetComponent<UnitEntityPathComponent>().StartMove(AstarComponent.Instance.GetNode(message.X, message.Y)).Coroutine();
                            }
                        }
                    }
                }
                //宠物
                else if (ViewId == 0 && UnitType == 3)
                {

                    if (UnitEntityComponent.Instance.PetEntityDic.TryGetValue(message.GameUserId, out PetEntity petEntity))
                    {
                        //得到当前实体
                        if (message.IsNeedMove == 1) //被击
                        {
                            petEntity.GetComponent<UnitEntityPathComponent>().StopMove();
                            petEntity.Position = AstarComponent.Instance.GetVectory3(message.X, message.Y).GroundPos();
                            petEntity.CurrentNodePos = AstarComponent.Instance.GetNode(message.X, message.Y);
                        }
                        else
                        {
                            petEntity.GetComponent<UnitEntityPathComponent>()?.StartMove(AstarComponent.Instance.GetNode(message.X, message.Y)).Coroutine();
                        }
                    }
                    else
                    {
                      
                        if (message.MapId != 0)
                        {
                            //不存在 创建
                            UnitEntityFactory.CreatPet(message.GameUserId, message.ModelId, AstarComponent.Instance.GetNode(message.X, message.Y), message.Angle, message.OwnerGameUserId, message.HpMaxValue, message.MpMaxValue);
                        }
                    }

                }
                //召唤兽
                else if (ViewId == 0)
                {
                    if (UnitEntityComponent.Instance.SummonEntityDic.TryGetValue(message.GameUserId, out SummonEntity summon))
                    {
                        if (message.IsNeedMove == 1) //被击
                        {
                            UnitEntityComponent.Instance.Get<SummonEntity>(message.GameUserId).Position = AstarComponent.Instance.GetVectory3(AstarComponent.Instance.GetNode(message.X, message.Y)).GroundPos();
                           
                            summon.CurrentNodePos = AstarComponent.Instance.GetNode(message.X, message.Y);

                        }
                        else
                        {
                            summon.GetComponent<UnitEntityPathComponent>()?.StartMove(AstarComponent.Instance.GetNode(message.X, message.Y)).Coroutine();
                        }
                    }
                    
                }
                //Boss
                //Boss提示
                else if (ViewId == 10&& UnitType == 0)
                {
                    if (ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(message.ModelId) is EnemyConfig_InfoConfig enemy)
                    {

                        UIMainComponent.Instance.ShowNotice($"Boss[<color=red>{enemy.Name}</color>]在<color=red>{SceneNameExtension.GetSceneName((SceneName)message.MapId)}</color>[{message.X},{message.Y}]出现");
                    }
                       
                  
                }
                else if (ViewId == 11&& UnitType == 0)
                {
                    if (ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(message.ModelId) is EnemyConfig_InfoConfig enemy)
                    {

                        UIMainComponent.Instance.ShowNotice($"Boss[<color=red>{enemy.Name}</color>]在{SceneNameExtension.GetSceneName((SceneName)message.MapId)}[{message.X},{message.Y}]被玩家[<color=red>{message.NickName}</color>]击败");
                    }
                }

            }
        }
    }
}
