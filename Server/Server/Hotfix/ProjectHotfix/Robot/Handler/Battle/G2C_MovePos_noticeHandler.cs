using System.Threading.Tasks;
using System;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using CustomFrameWork.Component;
using MongoDB.Bson;
using UnityEngine;

namespace ETHotfix.Robot
{
    [MessageHandler(AppType.Robot)]
    public class G2C_MovePos_noticeHandler : AMHandler<G2C_MovePos_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_MovePos_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_MovePos_notice message)
        {
            Scene clientScene = session.ClientScene();
            RobotMapComponent robotMap = clientScene.GetComponent<RobotMapComponent>();
            Unit localUnit = clientScene.GetComponent<PlayerComponent>().LocalUnit;
            if(localUnit == null)
            {
                Log.Warning($"[{clientScene.Name}] 收到移动包前，还没收到 G2C_StartGameGamePlayerResponse 消息");
                return false;
            }

            if(message.MapId != 0)
            {
                if (robotMap.CurrentMap == null)
                {
                    // 还没进入地图
                    await RobotMapHelper.MapChangeTo(clientScene, message.MapId, message.X, message.Y);
                }
                else if (robotMap.CurrentMap.MapId != message.MapId)
                {
                    try
                    {
                        // 地图变动
                        await RobotMapHelper.MapChangeTo(clientScene, message.MapId, message.X, message.Y);
                    }
                    catch (Exception e)
                    {
                        Log.Info($"[{clientScene.Name}] {e}");
                        throw e;
                    }
                }
            }

            if (message.GameUserId == localUnit.Id)
            {
                {
                    Unit currentUnit = robotMap.CurrentMap.GetUnit(message.GameUserId);
                    if(currentUnit == null)
                    {
                        Log.Error($"[{clientScene.Name}] currentUnit == null");
                        return false;
                    }
                    robotMap.CurrentMap.LocalUnitMove(currentUnit, new Vector2Int()
                    {
                        x = message.X,
                        y = message.Y,
                    });
                    currentUnit.GetComponent<MoveComponent>().StopMove();

                }
            }
            else
            {
                if(robotMap.CurrentMap == null)
                {
                    Log.Warning($"[{clientScene.Name}] 收到Unit位置变动信息,地图还未加载:{message.ToJson()}");
                    return false;
                }
                if(message.ViewId == 1)
                {
                    // TODO 单位进入视野
                    Unit unit = robotMap.CurrentMap.GetUnit(message.GameUserId);
                    if(unit != null)
                    {
                        Log.Warning($"[{clientScene.Name}] 收到重复进入视野消息,message.GameUserId={message.GameUserId}");
                        return false;
                    }
                    switch ((E_Identity)message.UnitType)
                    {
                        case E_Identity.Hero:
                            {
                                // 玩家
                                unit = UnitFactory.CreatePlayer(clientScene,message.GameUserId, (E_GameOccupation)message.ModelId);
                                RobotRoleComponent role = unit.GetComponent<RobotRoleComponent>();
                                role.Name = message.NickName;
                                role.OccupationLevel = message.OccupationLevel;
                                role.PkPoint = message.PkNumber;
                                NumericComponent numeric = unit.GetComponent<NumericComponent>();
                                numeric.SetNoEvent((int)E_GameProperty.PROP_HP_MAX, message.HpMaxValue);
                                numeric.SetNoEvent((int)E_GameProperty.PROP_HP, message.HpValue);
                            }
                            break;
                        case E_Identity.Enemy:
                            {
                                // 怪物
                                unit = UnitFactory.CreateMonster(clientScene, message.GameUserId, message.ModelId);
                                NumericComponent numeric = unit.GetComponent<NumericComponent>();
                                numeric.SetNoEvent((int)E_GameProperty.PROP_HP_MAX, message.HpMaxValue);
                                numeric.SetNoEvent((int)E_GameProperty.PROP_HP, message.HpValue);
                            }
                            break;
                        case E_Identity.Npc:
                            {
                                // npc
                                unit = UnitFactory.CreateNpc(clientScene, message.GameUserId);
                                unit.GetComponent<RobotNpcComponent>().ConfigId = message.ModelId;
                            }
                            break;
                        case E_Identity.Pet:
                            {
                                // 宠物
                                unit = UnitFactory.CreatePet(clientScene, message.GameUserId,message.ModelId,message.OwnerGameUserId);
                            }
                            break;
                        case E_Identity.Summoned:
                            {
                                // 召唤兽
                                unit = UnitFactory.CreateSummoned(clientScene, message.GameUserId, message.ModelId);
                                NumericComponent numeric = unit.GetComponent<NumericComponent>();
                                numeric.SetNoEvent((int)E_GameProperty.PROP_HP_MAX, message.HpMaxValue);
                                numeric.SetNoEvent((int)E_GameProperty.PROP_HP, message.HpValue);
                            }
                            break;
                        default:
                            Log.Warning($"[{clientScene.Name}] 未知的UnitType,message.GameUserId={message.GameUserId},UnitType={message.UnitType}");
                            break;
                    }
                    Log.Debug($"[{clientScene.Name}] 视野单位数={robotMap.CurrentMap.UnitDict.Count} MapId={robotMap.CurrentMap.MapId}");

                    if (unit == null)
                    {
                        return false;
                    }
                    try
                    {
                        robotMap.CurrentMap.UnitEnter(unit, new Vector2Int(message.X, message.Y));
                    }
                    catch (Exception e)
                    {
                        Log.Info($"[{clientScene.Name}] {e}");
                        throw e;
                    }

                }
                else if(message.ViewId == 2)
                {
                    // TODO 单位离开视野
                    Unit unit = robotMap.CurrentMap.GetUnit(message.GameUserId);
                    if(unit == null)
                    {
                        //Log.Warning($"[{clientScene.Name}] 收到重复离开视野消息,message.GameUserId={message.GameUserId}");
                        return false;
                    }
                    unit.Dispose();
                }
                else if(message.ViewId == 0)
                {
                    // TODO 单位移动
                    Unit unit = robotMap.CurrentMap.GetUnit(message.GameUserId);
                    if (unit == null)
                    {
                        Log.Warning($"[{clientScene.Name}] unit == null,message.GameUserId={message.GameUserId}");
                        return false;
                    }
                    unit.GetComponent<MoveComponent>().StopMove();
                    robotMap.CurrentMap.UnitMove(unit, new Vector2Int(message.X, message.Y));
                }
            }
            
            return false;
        }
    }
}
