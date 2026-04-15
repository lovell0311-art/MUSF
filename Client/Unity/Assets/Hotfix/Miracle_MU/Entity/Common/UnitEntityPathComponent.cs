using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ETModel;
using System;
using UnityEngine.Profiling;
using UnityEngine.SocialPlatforms;

namespace ETHotfix
{
    [ObjectSystem]
    public class UnitEntityPathComponentAwake : AwakeSystem<UnitEntityPathComponent>
    {
        public override void Awake(UnitEntityPathComponent self)
        {
            self.unitEntity = self.GetParent<UnitEntity>();
            if (self.unitEntity is RoleEntity roleEntity)
            {
                self.roleEntity = roleEntity;
            }
            self.moveNodes.Clear();
            // self.CancellationTokenSource = new CancellationTokenSource();
            self.animatorComponent = self.Entity.GetComponent<AnimatorComponent>();

        }
    }
    [ObjectSystem]
    public class UnitEntityPathComponentStart : StartSystem<UnitEntityPathComponent>
    {
        public override void Start(UnitEntityPathComponent self)
        {
            self.RoleMoveControlComponent = self.unitEntity.GetComponent<RoleMoveControlComponent>() ?? null;
            self.roleEquipmentComponent = self.unitEntity.GetComponent<RoleEquipmentComponent>() ?? null;
            self.moveComponent = self.unitEntity.GetComponent<UnitEntityMoveComponent>();
        }
    }

    [ObjectSystem]
    public class UnitEntityPathComponentUpdate : UpdateSystem<UnitEntityPathComponent>
    {
        public override void Update(UnitEntityPathComponent self)
        {
            if (self.unitEntity is RoleEntity role && role != UnitEntityComponent.Instance.LocalRole && Time.time > self.idleTime)
            {
                self.animatorComponent.Idle();
            }
        }
    }

    // [ObjectSystem]
    public class UnitEntityPathComponentFixedUpdate : FixedUpdateSystem<UnitEntityPathComponent>
    {
        public override void FixedUpdate(UnitEntityPathComponent self)
        {
            if (self.moveNodes.Count != 0)
            {

                //  if (self.CancellationTokenSource != null) return;
                self.StartMove(self.moveNodes.Dequeue()).Coroutine();
            }
            else
            {
                if (self.unitEntity is RoleEntity role && self.moveComponent.Moving == false)
                {
                    if (role != UnitEntityComponent.Instance.LocalRole)
                    {
                        self.animatorComponent.Idle();
                    }
                }
            }

        }
    }

    /// <summary>
    /// 实体移动路径组件
    /// </summary>
    public class UnitEntityPathComponent : Component
    {
        public List<Vector3> Path = new List<Vector3>();//移动路径点集合

        public Vector3 Nextpos;

        public CancellationTokenSource CancellationTokenSource;

        public UnitEntityMoveComponent moveComponent;

        public float moveSpeed = 7.5f;//移动速度 单位 m/s
        public float movePetSpeed = 6f;//移动速度
        public float runSpeed = 14f;//奔跑速度

        public float slowratio = .8f;
        public float MoveSpeed
        {
            get
            {
                if (unitEntity is RoleEntity)//玩家的移动速度
                {
                    var speed = IsRun ? runSpeed : moveSpeed;
                    // var speed = moveSpeed;
                    if (unitEntity.IsSlowDown)
                        return speed * slowratio;
                    else
                        return speed;
                }
                else if (unitEntity is PetEntity)//怪物的移动速度
                {
                    PetEntity petEntity = UnitEntityComponent.Instance.Get<PetEntity>(unitEntity.Id);

                    movePetSpeed = (float)(2 * 1000) / petEntity.MoSpeed;

                    return movePetSpeed;
                }
                else
                {
                    if (unitEntity is MonsterEntity monster)//monster.ConfigInfo.MoSpeed 表示 移动一个所需的时间 （时间戳 毫秒）
                    {
                        //400ms=0.4s 1m->由v=s/t -> 1m/0.4s ->2.5m/s 
                        return (float)(2 * 1000) / (monster.MoveSpeed * (monster.IsSlowDown ? 1 + slowratio : 1));//v=s/t 得出速度
                    }
                    else if (unitEntity is SummonEntity summon)
                    {
                        return (float)(2 * 1000) / (summon.MoveSpeed * (summon.IsSlowDown ? 1 + slowratio : 1));//v=s/t 得出速度

                    }
                    else
                    {
                        return moveSpeed;
                    }
                }

            }
            set
            {
                moveSpeed = value;
            }
        }

        public AnimatorComponent animatorComponent;

        public UnitEntity unitEntity;

        public RoleEntity roleEntity;

        public Queue<AstarNode> moveNodes = new Queue<AstarNode>();

        public RoleMoveControlComponent RoleMoveControlComponent;

        public RoleEquipmentComponent roleEquipmentComponent;

        public Action PathfindingCallBack;//自动寻路 回调函数
        public AstarNode LastNavTarget;
        Vector3 vector3;

        public bool IsRun
        {
            get
            {
                if (roleEquipmentComponent != null)
                {
                    return unitEntity is RoleEntity roleEntity && (roleEquipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Boots) || roleEquipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Mounts)) && !roleEntity.IsSafetyZone;//是玩家实体 装备了 鞋子 并且不在安全区 才可以奔跑
                }
                else
                {
                    return false;
                }


            }
        }

        public float idleTime;
        public async ETVoid StartMove(AstarNode vector)
        {
            if (vector == null) return;

            if (unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                Log.DebugGreen($"StartMove single from {unitEntity.CurrentNodePos?.x},{unitEntity.CurrentNodePos?.z} to {vector.x},{vector.z}");
            }

            //取消之前的移动协程
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();

            }
            //  Profiler.BeginSample("StartMove");
            CancellationTokenSource = new CancellationTokenSource();
            // Profiler.EndSample();

            //   Profiler.BeginSample("animation");
            //播放移动动画
            if (unitEntity is RoleEntity entity)//当前实体是玩家 并且不在安全区  //&& entity != UnitEntityComponent.Instance.LocalRole
            {
                if (entity != UnitEntityComponent.Instance.LocalRole)
                {
                    idleTime = Time.time + .45f;
                    if (IsRun)
                    {
                        //   Profiler.BeginSample("run");
                        animatorComponent.Run();
                        //  Profiler.EndSample();
                    }
                    else
                    {
                        // Profiler.BeginSample("walk");
                        animatorComponent.Walk();
                        // Profiler.EndSample();
                    }
                }
            }
            else
            {
                // Profiler.BeginSample("SetBoolValue");
                animatorComponent.SetBoolValue(MotionType.IsMove.ToString(), !unitEntity.IsDead);
                //  Profiler.EndSample();
            }

            // Profiler.BeginSample("GetVectory3");
            vector3 = ((Vector3)AstarComponent.Instance.GetVectory3(vector.x, vector.z)).GroundPos();
            // Profiler.EndSample();

            //Profiler.EndSample();

            // Profiler.BeginSample("Turn");
            this.Entity.GetComponent<TurnComponent>().Turn(vector3);

            if (this.RoleMoveControlComponent != null)
            {
                this.RoleMoveControlComponent.SendMovePos(vector).Coroutine();
            }

            //Profiler.EndSample();


            //  Profiler.BeginSample("MoveSendNotice");

            //AstartComponentExtend.MoveSendNotice(unitEntity.CurrentNodePos, vector, unitEntity.Id);
            unitEntity.CurrentNodePos = vector;
            //  Profiler.EndSample();

            await this.Entity.GetComponent<UnitEntityMoveComponent>().MoveToAsync(vector3, MoveSpeed, this.CancellationTokenSource.Token);

            if (unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                Log.DebugGreen($"StartMove single complete world={unitEntity.Position.x:F1},{unitEntity.Position.y:F1},{unitEntity.Position.z:F1} node={unitEntity.CurrentNodePos?.x},{unitEntity.CurrentNodePos?.z}");
            }

            if (unitEntity is RoleEntity == false)
            {
                animatorComponent.SetBoolValue(MotionType.IsMove, false);
            }

            this.CancellationTokenSource.Dispose();
            this.CancellationTokenSource = null;
        }



        public async ETTask StartMove(List<AstarNode> nodes)
        {
            if (unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                int count = nodes != null ? nodes.Count : -1;
                string target = count > 0 ? $"{nodes[count - 1].x},{nodes[count - 1].z}" : "none";
                Log.DebugGreen($"StartMove path count={count} current={unitEntity.CurrentNodePos?.x},{unitEntity.CurrentNodePos?.z} target={target}");
            }

            if (nodes == null || nodes.Count == 0)
            {
                RoleMoveControlComponent.IsNavigation = false;
                PathfindingCallBack?.Invoke();
                PathfindingCallBack = null;
                return;
            }

            //取消之前的移动协程
            CancellationTokenSource?.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
            this.Path.Clear();
            for (int i = 0, length = nodes.Count; i < length; i++)
            {
                var node = nodes[i];
                this.Path.Add(AstarComponent.Instance.GetVectory3(node.x, node.z).GroundPos());
            }


            await StartMove(this.CancellationTokenSource.Token);

            if (this.CancellationTokenSource.IsCancellationRequested == false)
                animatorComponent.Idle();
            this.CancellationTokenSource.Dispose();
            this.CancellationTokenSource = null;
        }


        private async ETTask StartMove(CancellationToken cancellationToken)
        {
            RoleMoveControlComponent.IsNavigation = true;
            for (int i = 0, count = Path.Count; i < count; i++)
            {
                if (this.CancellationTokenSource == null) break;

                Vector3 vector = Path[i];
                var nextnode = AstarComponent.Instance.GetNodeVector((int)vector.x, (int)vector.z);

                if (unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    //移动到先传送区域
                    if (TransferPointTools.Instances.IsTransferAreas(new Vector2Int(this.roleEntity.CurrentNodePos.x, this.roleEntity.CurrentNodePos.z)) == false && TransferPointTools.Instances.IsTransferAreas(new Vector2Int(nextnode.x, nextnode.z)))
                    {
                        RoleMoveControlComponent.ismove = false;
                        //场景加载进度
                        //  UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                        //进入传送区域 显示场景加载面板
                        // RoleMoveControlComponent.SendMovePos(nextnode).Coroutine();
                        RoleMoveControlComponent.NewSendMovePos(nextnode).Coroutine();
                        RoleMoveControlComponent.IsInTransf = true;
                        animatorComponent.Idle();
                        UIMainComponent.Instance.ResetRocker();

                        //this.moveComponent.moveTcs = null;
                        RoleMoveControlComponent.IsNavigation = false;
                        PathfindingCallBack?.Invoke();
                        PathfindingCallBack = null;

                        RoleMoveControlComponent.UnitEntityPathComponent.CancellationTokenSource?.Cancel();
                        TimerComponent.Instance.RegisterTimeCallBack(100, () => { RoleMoveControlComponent.IsInTransf = false; RoleMoveControlComponent.ismove = true; });
                        return;
                    }
                }

                unitEntity.CurrentNodePos = nextnode;
                //播放移动动画
                if (IsRun)
                    animatorComponent.Run();
                else
                    animatorComponent.Walk();

                //是否在安全
                if (unitEntity is RoleEntity role)
                {
                    if (SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(role.CurrentNodePos.x, role.CurrentNodePos.z)) != role.IsSafetyZone)
                    {
                        bool isSafe = SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(role.CurrentNodePos.x, role.CurrentNodePos.z));
                        role.IsSafetyZone = isSafe;
                        role.GetComponent<RoleEquipmentComponent>().EnterSafeArea(isSafe);
                    }
                }

                if (RoleMoveControlComponent != null)
                {
                    RoleMoveControlComponent.SendMovePos(nextnode).Coroutine();
                }

                this.Entity?.GetComponent<TurnComponent>().Turn(vector);//调整方向
                await this.Entity.GetComponent<UnitEntityMoveComponent>().MoveToAsync(vector, MoveSpeed, cancellationToken);
                if (cancellationToken.IsCancellationRequested) break;
            }
            RoleMoveControlComponent.IsNavigation = false;
            PathfindingCallBack?.Invoke();
            PathfindingCallBack = null;

        }


        public void StopMove()
        {
            if (!(unitEntity is PetEntity) && RoleMoveControlComponent != null)
            {
                RoleMoveControlComponent.IsNavigation = false;
            }

            RoleOnHookComponent onHookComponent = RoleOnHookComponent.Instance;
            if (onHookComponent != null && onHookComponent.IsOnHooking)
            {
                onHookComponent.NavTarget = false;
            }

            if (moveComponent != null && moveComponent.Moving == false)
                moveComponent.Stop();

            //取消之前的移动协程
            CancellationTokenSource?.Cancel();
            this.Path.Clear();
            moveNodes.Clear();
            animatorComponent?.SetBoolValue(MotionType.IsMove.ToString(), false);
            animatorComponent?.SetBoolValue(MotionType.IsRun.ToString(), false);
            PathfindingCallBack = null;
        }
        /// <summary>
        /// 导航到目标点
        /// </summary>
        /// <param name="targetPos"></param>
        public void NavTarget(Vector3 targetPos, Action action = null)
        {
            PathfindingCallBack = action;
            AstarComponent.Instance.FindPath(roleEntity.Position, targetPos, AstarNavCallback);
        }
        public void NavTarget(AstarNode target, Action action = null)
        {
            Log.DebugGreen($"MoveDiag NavTarget enter current={roleEntity?.CurrentNodePos?.x},{roleEntity?.CurrentNodePos?.z} target={target?.x},{target?.z} walkable={target?.isWalkable}");
            if (target.isWalkable == false)
            {
                if (RoleOnHookComponent.Instance.IsOnHooking)
                {
                    RoleOnHookComponent.Instance.NavTarget = false;
                    RoleOnHookComponent.Instance.curAttackEntity = null;
                }
                //  Log.DebugRed("目标点为不可行走区域");
                return;
            }

            LastNavTarget = target;
            RoleMoveControlComponent.IsNavigation = true;
            PathfindingCallBack = action;
            EnsureCurrentNodeIsWalkable();

            if (roleEntity.CurrentNodePos == null || roleEntity.CurrentNodePos.isWalkable == false)
            {
                Log.DebugGreen($"MoveDiag NavTarget abort-current current={roleEntity.CurrentNodePos?.x},{roleEntity.CurrentNodePos?.z} target={target.x},{target.z}");
                RoleMoveControlComponent.IsNavigation = false;
                PathfindingCallBack?.Invoke();
                PathfindingCallBack = null;
                return;
            }

            List<AstarNode> directNodes = TryBuildDirectPath(roleEntity.CurrentNodePos, target);
            if (directNodes != null)
            {
                Log.DebugGreen($"NavTarget direct start={roleEntity.CurrentNodePos?.x},{roleEntity.CurrentNodePos?.z} target={target.x},{target.z} count={directNodes.Count}");
                if (directNodes.Count == 0)
                {
                    RoleMoveControlComponent.IsNavigation = false;
                    PathfindingCallBack?.Invoke();
                    PathfindingCallBack = null;
                    return;
                }

                StartMove(directNodes).Coroutine();
                return;
            }

            Log.DebugGreen($"NavTarget astar start={roleEntity.CurrentNodePos?.x},{roleEntity.CurrentNodePos?.z} target={target.x},{target.z}");
            AstarComponent.Instance.FindPath(roleEntity.CurrentNodePos, target, AstarNavCallback);

            if (RoleOnHookComponent.Instance.IsOnHooking)
            {
                RoleOnHookComponent.Instance.NavTarget = false;
            }
        }
        private void AstarNavCallback(List<AstarNode> nodes)
        {
            if (unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                Log.DebugGreen($"AstarNavCallback count={(nodes != null ? nodes.Count : -1)} current={unitEntity.CurrentNodePos?.x},{unitEntity.CurrentNodePos?.z} target={LastNavTarget?.x},{LastNavTarget?.z}");
            }

            if ((nodes == null || nodes.Count == 0) && LastNavTarget != null)
            {
                List<AstarNode> directNodes = TryBuildDirectPath(roleEntity.CurrentNodePos, LastNavTarget);
                if (directNodes != null && directNodes.Count > 0)
                {
                    Log.DebugGreen($"AstarNavCallback fallback-direct count={directNodes.Count} target={LastNavTarget.x},{LastNavTarget.z}");
                    StartMove(directNodes).Coroutine();
                    return;
                }
            }

            StartMove(nodes).Coroutine();

        }

        private List<AstarNode> TryBuildDirectPath(AstarNode start, AstarNode target, int maxSteps = 24)
        {
            if (start == null || target == null)
            {
                return null;
            }

            if (start == target || (start.x == target.x && start.z == target.z))
            {
                return new List<AstarNode>();
            }

            if (Mathf.Max(Mathf.Abs(target.x - start.x), Mathf.Abs(target.z - start.z)) > maxSteps)
            {
                return null;
            }

            List<AstarNode> result = new List<AstarNode>();
            HashSet<int> visited = new HashSet<int> { (start.x << 16) ^ start.z };
            AstarNode current = start;
            int guard = 0;

            while ((current.x != target.x || current.z != target.z) && guard++ < maxSteps)
            {
                int stepX = GetStepValue(target.x - current.x);
                int stepZ = GetStepValue(target.z - current.z);
                AstarNode next = AstarComponent.Instance.GetNode(current.x + stepX, current.z + stepZ);
                if (next == null || !next.isWalkable)
                {
                    return null;
                }

                if (stepX != 0 && stepZ != 0)
                {
                    AstarNode sideX = AstarComponent.Instance.GetNode(current.x + stepX, current.z);
                    AstarNode sideZ = AstarComponent.Instance.GetNode(current.x, current.z + stepZ);
                    if (sideX == null || !sideX.isWalkable || sideZ == null || !sideZ.isWalkable)
                    {
                        return null;
                    }
                }

                int key = (next.x << 16) ^ next.z;
                if (!visited.Add(key))
                {
                    return null;
                }

                result.Add(next);
                current = next;
            }

            if (current.x != target.x || current.z != target.z)
            {
                return null;
            }

            return result;
        }

        private static int GetStepValue(int delta)
        {
            if (delta > 0)
            {
                return 1;
            }

            if (delta < 0)
            {
                return -1;
            }

            return 0;
        }

        private void EnsureCurrentNodeIsWalkable()
        {
            if (roleEntity == null || AstarComponent.Instance == null)
            {
                return;
            }

            AstarNode currentNode = roleEntity.CurrentNodePos ?? AstarComponent.Instance.GetNodeVector(roleEntity.Position);
            if (currentNode != null && currentNode.isWalkable)
            {
                roleEntity.CurrentNodePos = currentNode;
                return;
            }

            AstarNode repairedNode = FindNearestWalkableNode(currentNode);
            if (repairedNode != null)
            {
                roleEntity.CurrentNodePos = repairedNode;
                Log.DebugGreen($"MoveDiag PathRepair node={repairedNode.x},{repairedNode.z} world={roleEntity.Position.x:F1},{roleEntity.Position.y:F1},{roleEntity.Position.z:F1}");
                return;
            }

            roleEntity.CurrentNodePos = currentNode;
        }

        private AstarNode FindNearestWalkableNode(AstarNode centerNode, int searchRadius = 12)
        {
            if (centerNode == null || AstarComponent.Instance == null)
            {
                return null;
            }

            for (int radius = 1; radius <= searchRadius; ++radius)
            {
                AstarNode nearestNode = null;
                int nearestDistance = int.MaxValue;

                for (int dx = -radius; dx <= radius; ++dx)
                {
                    for (int dy = -radius; dy <= radius; ++dy)
                    {
                        if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius)
                        {
                            continue;
                        }

                        AstarNode candidate = AstarComponent.Instance.GetNode(centerNode.x + dx, centerNode.z + dy);
                        if (candidate == null || candidate.isWalkable == false)
                        {
                            continue;
                        }

                        int distance = dx * dx + dy * dy;
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestNode = candidate;
                        }
                    }
                }

                if (nearestNode != null)
                {
                    return nearestNode;
                }
            }

            return null;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            moveNodes.Clear();
            CancellationTokenSource?.Cancel();
            this.CancellationTokenSource?.Dispose();
            this.CancellationTokenSource = null;
            this.Path.Clear();
            moveNodes.Clear();
            animatorComponent.SetBoolValue(MotionType.IsMove.ToString(), false);
            PathfindingCallBack = null;
        }

    }
}
