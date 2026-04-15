
using ETModel;

using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using DG.Tweening;


namespace ETHotfix
{
    [ObjectSystem]
    public class RoleMoveComponentAwake : AwakeSystem<RoleMoveControlComponent>
    {
        public override void Awake(RoleMoveControlComponent self)
        {
            self.roleEntity = self.GetParent<RoleEntity>();

        }
    }
    [ObjectSystem]
    public class RoleMoveComponentStart : StartSystem<RoleMoveControlComponent>
    {
        public override void Start(RoleMoveControlComponent self)
        {
            self.animatorComponent = self.roleEntity.GetComponent<AnimatorComponent>();
            self.moveComponent = self.roleEntity.GetComponent<UnitEntityMoveComponent>();
            self.stallUpComponent = self.roleEntity.GetComponent<RoleStallUpComponent>();
            self.equipmentComponent = self.roleEntity.GetComponent<RoleEquipmentComponent>();
            self.UnitEntityPathComponent = self.roleEntity.GetComponent<UnitEntityPathComponent>();

            self.RegiserRockerEvent();
            self.TryAlignSpawnPosition("start");
        }
    }
    [ObjectSystem]
    public class RoleMoveComponentUpdate : UpdateSystem<RoleMoveControlComponent>
    {
        public override void Update(RoleMoveControlComponent self)
        {

            self.Move().Coroutine();
        }
    }


    /// <summary>
    /// 本地玩家移动控制组件
    /// </summary>

    public class RoleMoveControlComponent : Component
    {
        /// <summary>
        /// 是否处于导航状态
        /// </summary>
      //  public bool IsNavigation => this.roleEntity?.GetComponent<UnitEntityPathComponent>()?.CancellationTokenSource != null;
        public bool IsNavigation = false;

        /// <summary>
        /// 是否可以重合站立
        /// </summary>
        public bool Overlap = false;

        public Vector3 MoveDir = Vector3.zero;//移动方向
        public bool isPressJoy = false;//是否按下摇杆
        public float rotatespeed = 10f;//角色旋转速度
        float rotate_Y = 0;//摄像机旋转后的当前角度

        public Vector2 MoveDir_Vector2 = Vector2.zero;


        public AnimatorComponent animatorComponent;
        public AnimationEventProxy animationEventProxy;
        public RoleEntity roleEntity;
        public UnitEntityPathComponent UnitEntityPathComponent;

        AstarNode NextastarNode;//下一个格子
        public Vector3 TargetPos = Vector3.zero;

        public Vector3 ignoreInputTargetPos = Vector3.zero;
        public bool IsIgnoreInputTargetPos = false;//是否自动避障（像该点移动时 会忽略玩家输入的方向）

        public UIMainComponent mainComponent;

        public UnitEntityMoveComponent moveComponent;

        public string Effect_Nav = "Effect_Nav_track";//点击地面特效

        public RoleStallUpComponent stallUpComponent;

        public RoleEquipmentComponent equipmentComponent;

        public CharacterController characterController;

        public bool IsRun => (equipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Boots) || equipmentComponent.curWareEquipsData_Dic.ContainsKey(E_Grid_Type.Mounts)) && roleEntity.IsSafetyZone == false;//装备了鞋子或使用坐骑、并且不在安全区 才能跑

        public float ReadyToRunTimer;
        public bool IsReadyToRun = false;


        public bool IsInTransf = false;

        public int fuBenStata = 0;

        public bool IsCanMove = true;
        private bool hasSpawnAlignmentAttempt = false;

        public int skillLayer;
        //状态信息
        public AnimatorStateInfo stateinfo;
        #region 摇杆事件
        public void RegiserRockerEvent()
        {
            //  Game.EventManager.AddEventHandler<Vector2>(EventTypeId.UI_JOYSTICK_VALUE, ChangeMoveDir);
            Game.EventCenter.EventListenner<Vector2>(EventTypeId.UI_JOYSTICK_VALUE, ChangeMoveDir);
            Game.EventCenter.EventListenner(EventTypeId.UI_JOYSTICK_UP, PointerUp);
            Game.EventCenter.EventListenner(EventTypeId.UI_JOYSTICK_DOWN, PointerDown);
        }
        private void ChangeMoveDir(Vector2 dir)
        {
            if (DeviceUtil.IsNetworkReachability() == false)
            {
                MoveDir = Vector3.zero;
                UIComponent.Instance.VisibleUI(UIType.UIHint, "网络异常 已断开连接");
                return;

            }

            MoveDir.x = dir.x;
            MoveDir.z = dir.y;
            //角色移动方向 与摄像机视角一致
            rotate_Y = CameraComponent.Instance.MainCamera.transform.eulerAngles.y;
            MoveDir = Quaternion.Euler(0, rotate_Y, 0) * MoveDir;
            MoveDir_Vector2 = GetJoystickDirection(new Vector2(MoveDir.x, MoveDir.z)); ;
        }


        public Vector2 GetJoystickDirection(Vector2 pos)
        {
            var rad = Mathf.Atan2(pos.y, pos.x);
            if ((rad >= -Mathf.PI / 8 && rad < 0) || (rad >= 0 && rad < Mathf.PI / 8))
            {
                //   Log.DebugBrown($"右");
                return new Vector2Int(1, 0);

            }
            else if (rad >= Math.PI / 8 && rad < 3 * Math.PI / 8)
            {
                // Log.DebugBrown($"右上");
                return new Vector2Int(1, 1);
            }
            else if (rad >= 3 * Math.PI / 8 && rad < 5 * Math.PI / 8)
            {

                // Log.DebugBrown($"上");
                return new Vector2Int(0, 1);
            }
            else if (rad >= 5 * Math.PI / 8 && rad < 7 * Math.PI / 8)
            {
                // Log.DebugBrown($"左上");
                return new Vector2Int(-1, 1);
            }
            else if ((rad >= 7 * Math.PI / 8 && rad < Math.PI) || (rad >= -Math.PI && rad < -7 * Math.PI / 8))
            {
                // Log.DebugBrown($"左");
                return new Vector2Int(-1, 0);
            }
            else if (rad >= -7 * Math.PI / 8 && rad < -5 * Math.PI / 8)
            {
                // Log.DebugBrown($"左下");
                return new Vector2Int(-1, -1);
            }
            else if (rad >= -5 * Math.PI / 8 && rad < -3 * Math.PI / 8)
            {
                // Log.DebugBrown($"下");
                return new Vector2Int(0, -1);
            }
            else
            {
                // Log.DebugBrown($"右下");
                return new Vector2Int(1, -1);
            }

        }


        private void PointerUp()
        {
            // Log.DebugGreen("摇杆抬起");
            Application.targetFrameRate = 60;
            MoveDir = Vector3.zero;
            isPressJoy = false;

        }

        private void PointerDown()
        {
            //  Log.DebugGreen("摇杆按下");
            Application.targetFrameRate = 60;
            MoveDir = Vector3.zero;
            isPressJoy = true;
            RoleOnHookComponent onHookComponent = RoleOnHookComponent.Instance;
            UIMainComponent uiMainComponent = UIMainComponent.Instance;

            if (onHookComponent != null && onHookComponent.IsOnHooking)
            {
                if (uiMainComponent != null && uiMainComponent.HookTog != null)
                {
                    uiMainComponent.HookTog.isOn = false;
                }
            }
            if (IsNavigation)
            {
                // Log.DebugGreen("摇杆移动");
                //主动控制玩家移动时 若当前正在寻路 则停止寻路
                UnitEntityPathComponent.StopMove();
            }
            if (uiMainComponent != null && uiMainComponent.curSkillEntity != null)
                uiMainComponent.curSkillEntity = null;
        }
        #endregion


        //点击地面移动
        public void ClickGroundMove()
        {
            Vector3 screenPosition = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
            ClickGroundMove(screenPosition);
        }

        public void ClickGroundMove(Vector3 screenPosition)
        {
            Log.DebugGreen("ClickGroundMove begin");
            Log.Error($"MoveClick step0 screen={screenPosition.x:F1},{screenPosition.y:F1},{screenPosition.z:F1}");
            try
            {
            if (roleEntity == null)
            {
                roleEntity = this.GetParent<RoleEntity>();
                if (roleEntity == null)
                {
                    Log.Error("ClickGroundMove aborted: roleEntity is null");
                    return;
                }
            }

            if (UnitEntityPathComponent == null)
            {
                UnitEntityPathComponent = roleEntity.GetComponent<UnitEntityPathComponent>();
            }

            if (stallUpComponent == null)
            {
                stallUpComponent = roleEntity.GetComponent<RoleStallUpComponent>();
            }

            if (UnitEntityPathComponent == null || AstarComponent.Instance == null)
            {
                Log.Error($"ClickGroundMove aborted: path={UnitEntityPathComponent != null} astar={AstarComponent.Instance != null}");
                return;
            }
            Log.Error($"MoveClick step1 role={roleEntity != null} path={UnitEntityPathComponent != null} stall={stallUpComponent != null}");
            TryAlignSpawnPosition("click");
            if (ismove == false || IsInTransf)
            {
                // Log.DebugRed($"在传送区域");
                return;
            }

            RoleOnHookComponent onHookComponent = RoleOnHookComponent.Instance;
            UIMainComponent uiMainComponent = UIMainComponent.Instance;
            if (onHookComponent != null && onHookComponent.IsOnHooking)
            {
                if (uiMainComponent != null && uiMainComponent.HookTog != null)
                {
                    uiMainComponent.HookTog.isOn = false;
                }
                if (IsNavigation)
                {
                    //主动控制玩家移动时 若当前正在寻路 则停止寻路
                    UnitEntityPathComponent.StopMove();

                }
            }

            if (uiMainComponent != null && uiMainComponent.curSkillEntity != null)
                uiMainComponent.curSkillEntity = null;
            Log.Error($"MoveClick step2 hook={onHookComponent != null} uiMain={uiMainComponent != null} main={mainComponent != null}");

            if (roleEntity.IsDead || IsInTransf) return;
            if (TimeHelper.Now() <= GlobalDataManager.AttackSpaceTime)
            {
                // Log.DebugRed($"正在攻击");
                return;
            }
            if (stallUpComponent != null && stallUpComponent.IsStallUp)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "当前处于摆摊状态 无法移动");
                return;
            }
            if (TryCastMapPoint(screenPosition, out Vector3 pos))
            {
                Log.DebugGreen($"ClickGroundMove hit {pos.x:F1},{pos.y:F1},{pos.z:F1}");

                AstarNode targetnode = AstarComponent.Instance.GetNodeVector(pos);
                if (targetnode == null)
                {
                    Log.DebugGreen("ClickGroundMove blocked");
                    return;
                }

                if (targetnode.isWalkable == false)
                {
                    targetnode = FindNearestWalkableNode(targetnode);
                    if (targetnode == null)
                    {
                        Log.DebugGreen("ClickGroundMove blocked");
                        return;
                    }

                    Log.DebugGreen($"ClickGroundMove snap {targetnode.x},{targetnode.z}");
                }
                Log.Error($"MoveClick step3 target={targetnode.x},{targetnode.z} walkable={targetnode.isWalkable}");
                if (ResourcesComponent.Instance != null)
                {
                    GameObject nav = ResourcesComponent.Instance.LoadEffectObject(Effect_Nav.StringToAB(), Effect_Nav, 200);
                    if (nav != null)
                    {
                        nav.transform.position = AstarComponent.Instance.GetVectory3(targetnode.x, targetnode.z) + Vector3.up * .5f;
                    }
                }
                if (mainComponent == null && UIComponent.Instance != null && UIComponent.Instance.Get(UIType.UIMainCanvas) != null)
                {
                    mainComponent = UIComponent.Instance.Get(UIType.UIMainCanvas).GetComponent<UIMainComponent>();
                }

                Log.Error($"MoveClick step4 current={roleEntity.CurrentNodePos != null} main={mainComponent != null}");
                if (mainComponent != null && mainComponent.HookTog != null && mainComponent.HookTog.isOn)
                    mainComponent.HookTog.isOn = false;

                EnsureCurrentNodeIsWalkable();

                Log.DebugGreen($"MoveDiag pre-nav current={roleEntity.CurrentNodePos?.x},{roleEntity.CurrentNodePos?.z} target={targetnode.x},{targetnode.z} walkable={targetnode.isWalkable}");
                UnitEntityPathComponent.NavTarget(targetnode);
                Log.DebugGreen($"MoveDiag post-nav current={roleEntity.CurrentNodePos?.x},{roleEntity.CurrentNodePos?.z} target={targetnode.x},{targetnode.z} navigating={IsNavigation}");
                Log.DebugGreen($"ClickGroundMove nav DIAG2 {targetnode.x},{targetnode.z}");

            }
            else
            {
                Log.DebugGreen("ClickGroundMove miss-map");
            }
            }
            catch (Exception e)
            {
                Log.Error($"ClickGroundMove failed role={roleEntity != null} path={UnitEntityPathComponent != null} stall={stallUpComponent != null} hook={RoleOnHookComponent.Instance != null} main={mainComponent != null} uiMain={UIMainComponent.Instance != null} uiComp={UIComponent.Instance != null} astar={AstarComponent.Instance != null} pos={screenPosition.x:F1},{screenPosition.y:F1},{screenPosition.z:F1} error={e}");
            }

        }

        public void TryAlignSpawnPosition(string reason, bool force = false)
        {
            if (!force && hasSpawnAlignmentAttempt)
            {
                return;
            }

            if (roleEntity == null || AstarComponent.Instance == null)
            {
                return;
            }

            AstarNode currentNode = roleEntity.CurrentNodePos ?? AstarComponent.Instance.GetNodeVector(roleEntity.Position);
            if (currentNode == null)
            {
                return;
            }

            if (!currentNode.isWalkable)
            {
                AstarNode repairedNode = FindNearestWalkableNode(currentNode);
                if (repairedNode != null)
                {
                    currentNode = repairedNode;
                }
            }

            Vector3 expectedPosition = AstarComponent.Instance.GetVectory3(currentNode.x, currentNode.z).GroundPos();
            Vector3 actualPosition = roleEntity.Position;
            AstarNode worldNode = AstarComponent.Instance.GetNodeVector(actualPosition);
            bool worldNodeInvalid = worldNode == null || !worldNode.isWalkable;
            bool nodeMismatch = worldNodeInvalid || worldNode.x != currentNode.x || worldNode.z != currentNode.z;
            float gap = Vector3.Distance(actualPosition, expectedPosition);

            roleEntity.CurrentNodePos = currentNode;
            hasSpawnAlignmentAttempt = true;

            if (!force && !nodeMismatch && gap <= 1.1f)
            {
                return;
            }

            roleEntity.Position = expectedPosition;
            UIMainComponent.Instance?.ChangeRolePosTxt(currentNode);
            Log.DebugGreen(
                $"MoveDiag spawn-align reason={reason} current={currentNode.x},{currentNode.z} " +
                $"worldNode={worldNode?.x},{worldNode?.z} gap={gap:F2} force={force}");
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
                Log.DebugGreen($"MoveDiag repair-current node={repairedNode.x},{repairedNode.z} world={roleEntity.Position.x:F1},{roleEntity.Position.y:F1},{roleEntity.Position.z:F1}");
                return;
            }

            roleEntity.CurrentNodePos = currentNode;
            Log.DebugGreen($"MoveDiag keep-current node={roleEntity.CurrentNodePos?.x},{roleEntity.CurrentNodePos?.z} world={roleEntity.Position.x:F1},{roleEntity.Position.y:F1},{roleEntity.Position.z:F1}");
        }

        private static bool TryCastMapPoint(Vector3 screenPosition, out Vector3 hitPoint)
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(screenPosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 500, 1 << LayerMask.NameToLayer(LayerNames.MAP)))
                {
                    hitPoint = hit.point;
                    return true;
                }
            }

            hitPoint = Vector3.zero;
            return false;
        }

        private AstarNode FindNearestWalkableNode(AstarNode centerNode, int searchRadius = 12)
        {
            if (centerNode == null)
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

        public float movespacetime;
        public CancellationTokenSource CancellationTokenSource;
        public bool ismove = true;
        public async ETVoid Move()
        {
            if (!hasSpawnAlignmentAttempt)
            {
                TryAlignSpawnPosition("update");
            }

            if (RoleOnHookComponent.Instance.IsOnHooking) return;
            if (roleEntity.IsDead)
            {
                //  Log.DebugGreen($"玩家死亡");
                return;
            }

            if (ismove == false || IsInTransf)
            {
                // Log.DebugRed($"在传送区域");
                return;
            }
            if (TimeHelper.Now() <= GlobalDataManager.AttackSpaceTime)
            {
                //Log.DebugRed($"正在攻击");
                return;
            }

            if (IsNavigation)
            {
                return;
            }
            if (RoleOnHookComponent.Instance.IsOnHooking && RoleOnHookComponent.Instance.NavTarget)
            {
                RoleOnHookComponent.Instance.NavTarget = false;
            }

            if (isPressJoy || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                if (TaskDatas.AutoNavCallBack != null)
                    TaskDatas.AutoNavCallBack = null;
                //关闭挂机 
                if (RoleOnHookComponent.Instance.IsOnHooking)
                {
                    /*UIComponent.Instance.VisibleUI(UIType.UIHint, "请先暂停挂机 再移动");
                    return;*/
                    RoleOnHookComponent.Instance.NavTarget = false;
                    UIMainComponent.Instance.HookTog.isOn = false;
                    UnitEntityPathComponent.StopMove();
                }


            }
            //控制本地玩家移动
#if UNITY_EDITOR_WIN || UNITY_EDITOR
            if (!isPressJoy)//未只用摇杆
            {

                MoveDir.x = Input.GetAxisRaw("Horizontal");
                MoveDir.z = Input.GetAxisRaw("Vertical");
                if (MoveDir != Vector3.zero)
                {
                    Application.targetFrameRate = 60;
                }
                //角色移动方向 与摄像机视角一致
                rotate_Y = CameraComponent.Instance.MainCamera.transform.eulerAngles.y;
                MoveDir = Quaternion.Euler(0, rotate_Y, 0) * MoveDir;
                MoveDir_Vector2 = GetJoystickDirection(new Vector2(MoveDir.x, MoveDir.z)); ;
            }
#endif


            if (IsIgnoreInputTargetPos)
            {
                ismove = false;
                this.UnitEntityPathComponent.StartMove(NextastarNode).Coroutine();
                await TimerComponent.Instance.WaitAsync(this.moveComponent.needTime);//this.moveComponent.needTime
                ismove = true;
                IsIgnoreInputTargetPos = false;
            }
            else
            {

                if (MoveDir == Vector3.zero)
                {
                    IsReadyToRun = false;
                    if (animatorComponent == null) return;

                    //拥有坐骑
                    /*if (animatorComponent.GetBoolParameterValue(MotionType.IsMount.ToString()) && this.roleEntity.Game_Object.transform.localPosition.z == .5)
                    {
                        this.roleEntity.Game_Object.transform.localPosition += Vector3.forward * -.75f;

                    }*/
                    if (animatorComponent.GetBoolParameterValue(MotionType.IsMove.ToString()))///关闭 走路
                    {

                        animatorComponent.SetBoolValue(MotionType.IsMove.ToString(), false);
                        if (IsIdle().Item1 == false && !SiegeWarfareData.SiegeWarfareIsStart)
                        {
                            UnitEntityPathComponent.NavTarget(IsIdle().Item2);
                        }
                    }
                    if (animatorComponent.GetBoolParameterValue(MotionType.IsRun.ToString()))///关闭 奔跑
                    {

                        animatorComponent.SetBoolValue(MotionType.IsRun.ToString(), false);
                        if (IsIdle().Item1 == false)
                        {
                            UnitEntityPathComponent.NavTarget(IsIdle().Item2);
                        }
                    }
                    if (animatorComponent.GetBoolParameterValue(MotionType.IsSwim.ToString()))///关闭 游泳
                    {

                        animatorComponent.SetBoolValue(MotionType.IsSwim.ToString(), false);
                        if (IsIdle().Item1 == false)
                        {
                            UnitEntityPathComponent.NavTarget(IsIdle().Item2);
                        }
                    }
                    animatorComponent.Idle();
                    return;
                }
                else
                {

                    UnitEntityComponent.Instance.curAttackEntity = -1;
                    TryAlignSpawnPosition("move-input");
                    EnsureCurrentNodeIsWalkable();

                    if (stallUpComponent.IsStallUp)
                    {
                        animatorComponent.Idle();
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "当前处于摆摊状态 无法移动");
                        return;
                    }

                    if (IsRun)
                    {
                        if (IsReadyToRun == false)
                        {
                            IsReadyToRun = true;
                            ReadyToRunTimer = Time.time + 2;
                        }
                        if (IsReadyToRun)
                        {
                            if (Time.time > ReadyToRunTimer)
                            {
                                animatorComponent.Run();
                            }
                            else
                            {
                                animatorComponent.Walk();
                            }
                        }
                    }
                    else
                    {
                        animatorComponent.Walk();
                    }

                }

                NextastarNode = AstarComponent.Instance.GetNextNode(roleEntity.CurrentNodePos, MoveDir_Vector2);//下一个格子
                if (NextastarNode == null)
                {
                    IsIgnoreInputTargetPos = false;
                    animatorComponent?.Idle();
                    return;
                }

                //判断是否可以行走
                if (NextastarNode.isWalkable)//可行走
                {
                    ismove = false;
                    //移动到先传送区域
                    if (TransferPointTools.Instances.IsTransferAreas(new Vector2Int(this.roleEntity.CurrentNodePos.x, this.roleEntity.CurrentNodePos.z)) == false && TransferPointTools.Instances.IsTransferAreas(new Vector2Int(NextastarNode.x, NextastarNode.z)))
                    {
                        ////场景加载进度
                        //UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                        //进入传送区域 显示场景加载面板
                        // Log.DebugGreen($"进入传送区域:{NextastarNode}");
                        // SendMovePos(NextastarNode).Coroutine();
                        NewSendMovePos(NextastarNode).Coroutine();
                        //场景加载进度
                        //   UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                        IsInTransf = true;
                        animatorComponent.Idle();
                        UIMainComponent.Instance.ResetRocker();
                        PointerUp();
                        //this.moveComponent.moveTcs = null;
                        this.UnitEntityPathComponent.CancellationTokenSource?.Cancel();
                        TimerComponent.Instance.RegisterTimeCallBack(100, () => { IsInTransf = false; ismove = true; });
                        return;
                    }
                    //  Log.DebugRed($"{this.roleEntity.CurrentNodePos.Map.Id} {this.roleEntity.CurrentNodePos}  下一个坐标点：{NextastarNode.Map.Id} ： {NextastarNode}");
                    this.UnitEntityPathComponent.StartMove(NextastarNode).Coroutine();
                    await TimerComponent.Instance.WaitAsync(this.moveComponent.needTime);//this.moveComponent.needTime
                    ismove = true;
                }
                else
                {
                    //不能行走
                    //自动查询周围可行走的格子
                    NextastarNode = AstarComponent.Instance.GetCanMoveAstarNode(roleEntity.CurrentNodePos, NextastarNode);
                    //重新计算方向
                    IsIgnoreInputTargetPos = NextastarNode != null;
                }
            }


        }


        /// <summary>
        /// 当前 位置是否 可以站立（两个玩家 不能重合）
        /// </summary>
        /// <returns></returns>
        private (bool, AstarNode) IsIdle()
        {
            UnitEntityComponent.Instance.GetMinDistanceRoleEntity_Out(out RoleEntity role);//获取附近 最近的玩家
            if (role != null)
            {
                var dis = PositionHelper.Distance2D(role.Position, roleEntity.Position);
                //Log.DebugBrown($"最近的玩家  {role.RoleName} 之间的距离：{dis}");
                if (dis < 2)
                {
                    for (int i = -1; i < 1; i++)
                    {
                        for (int j = -1; j < 1; j++)
                        {

                            if (i == 0 && j == 0) continue;
                            Vector3 vector = role.Position.Vector3ToInt() + new Vector3Int(i, 0, j) * 2;
                            var nearNode = role.CurrentNodePos;
                            //AstarNode node = AstarComponent.Instance.GetNodeVector(vector.x, vector.z);
                            AstarNode node = AstarComponent.Instance.GetNode(nearNode.x + i, nearNode.z + j);
                            if (node != null && node.isWalkable)
                            {
                                //可以站立

                                //UnitEntityPathComponent.NavTarget(vector);//移动到该地点
                                return (false, node);
                            }
                        }
                    }

                }
                else
                {
                    return (true, null);
                }

            }
            return (true, null);
        }

        public void Move2Near()
        {
            for (int i = -1; i < 1; i++)
            {
                for (int j = -1; j < 1; j++)
                {

                    if (i == 0 && j == 0) continue;
                    Vector3 vector = roleEntity.Position.Vector3ToInt() + new Vector3Int(i, 0, j) * 2;
                    AstarNode node = AstarComponent.Instance.GetNodeVector(vector.x, vector.z);
                    if (node != null && node.isWalkable)
                    {
                        //可以站立
                        UnitEntityPathComponent.NavTarget(node);//移动到该地点
                        return;
                        //UnitEntityComponent.Instance.GetMinDistanceRoleEntity_Out(out RoleEntity role);//获取附近 最近的玩家
                        //if (role != null)
                        //{
                        //    AstarNode node1 = AstarComponent.Instance.GetNodeVector(role.Position.x,role.Position.z);
                        //    if (!node1.Compare(node))
                        //    {
                        //        UnitEntityPathComponent.NavTarget(role.Position);
                        //    }
                        //}
                    }
                }
            }

        }

        /// <summary>
        /// 改变方向
        /// </summary>
        /// <param name="TargetPos"></param>
        public void SetRotation(Vector3 TargetPos)
        {
            roleEntity.Rotation = PositionHelper.GetVector3ToQuaternion(roleEntity.Position, TargetPos);
        }
        public void StopMove()
        {
            MoveDir = Vector3.zero;
            UnitEntityPathComponent.StopMove();
            IsIgnoreInputTargetPos = false;
        }
        float spacetime = 0;
        G2C_MovePosResponse g2C_MovePosResponse;
        C2G_MovePosRequest c2G_MovePosRequest = new C2G_MovePosRequest();
        /// <summary> 
        /// 向服务端发送 玩家当前的格子坐标信息
        /// </summary>
        /// <returns></returns>
        public async ETVoid SendMovePos(AstarNode node)
        {

            if (Time.time > spacetime)
            {
                //每隔.5秒 检测拾取物品
                Game.EventCenter.EventTrigger<AstarNode>(EventTypeId.LOCALROLE_GRIDCHANGE, roleEntity.CurrentNodePos);
                spacetime = Time.time + .5f;
            }
            UIMainComponent.Instance?.ChangeRolePosTxt(roleEntity.CurrentNodePos);
            try
            {
                string remote = SessionComponent.Instance?.Session?.session?.RemoteAddress?.ToString() ?? "null";
                Log.DebugGreen($"SendMovePos request current={roleEntity.CurrentNodePos?.x},{roleEntity.CurrentNodePos?.z} target={node?.x},{node?.z} angle={(int)roleEntity.Game_Object.transform.parent.eulerAngles.y} remote={remote}");
                LoginStageTrace.Append($"MoveDiag SendMovePos request current={roleEntity.CurrentNodePos?.x},{roleEntity.CurrentNodePos?.z} target={node?.x},{node?.z} angle={(int)roleEntity.Game_Object.transform.parent.eulerAngles.y} remote={remote}");
                //Log.DebugGreen($"请求移动目标点 ({node.x},{node.z})");
                c2G_MovePosRequest.X = node.x;
                c2G_MovePosRequest.Y = node.z;
                c2G_MovePosRequest.Angle = (int)roleEntity.Game_Object.transform.parent.eulerAngles.y;
                g2C_MovePosResponse = (G2C_MovePosResponse)await SessionComponent.Instance.Session.Call(c2G_MovePosRequest);
                Log.DebugGreen($"SendMovePos response err={g2C_MovePosResponse.Error} pos={g2C_MovePosResponse.X},{g2C_MovePosResponse.Y} angle={g2C_MovePosResponse.Angle}");
                LoginStageTrace.Append($"MoveDiag SendMovePos response err={g2C_MovePosResponse.Error} pos={g2C_MovePosResponse.X},{g2C_MovePosResponse.Y} angle={g2C_MovePosResponse.Angle}");

                if (g2C_MovePosResponse.Error != 0)//506 正在攻击
                {
                    //StopNavigation?.Invoke();
                    if (g2C_MovePosResponse.Error == 506)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MovePosResponse.Error.GetTipInfo());
                    }
                    if (g2C_MovePosResponse.X != 0)
                    {
                        roleEntity.CurrentNodePos = AstarComponent.Instance.GetNode(g2C_MovePosResponse.X, g2C_MovePosResponse.Y);
                        StopMove();
                        Log.DebugRed($"g2C_MovePosResponse.Error:{g2C_MovePosResponse.Error.GetTipInfo()}  {g2C_MovePosResponse.Error}  g2C_MovePosResponse.X:{g2C_MovePosResponse.X}:{g2C_MovePosResponse.Y}  node:{node}");
                    }


                    // curGridIndex = lastGridIndex;

                    //主动控制玩家移动时 若当前正在寻路 则停止寻路
                    if (g2C_MovePosResponse.X != 0)
                    {
                        //  NextastarNode = AstarComponent.Instance.GetNode(g2C_MovePosResponse.X, g2C_MovePosResponse.Y);
                        // IsIgnoreInputTargetPos = true;
                    }
                    else
                    {
                        //   UnitEntityPathComponent.Path.Add(AstarComponent.Instance.GetVectory3(g2C_MovePosResponse.X, g2C_MovePosResponse.Y)) ;
                    }

                }
                else
                {
                    //Log.DebugGreen($"允许玩家移动:{g2C_MovePosResponse.X}-{g2C_MovePosResponse.Y}");
                    //判断是否在安全区
                    if (SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(roleEntity.CurrentNodePos.x, roleEntity.CurrentNodePos.z)) != roleEntity.IsSafetyZone)
                    {
                        bool isSafe = SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(roleEntity.CurrentNodePos.x, roleEntity.CurrentNodePos.z));
                        UnitEntityComponent.Instance.LocalRole.IsSafetyZone = isSafe;

                        UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>().EnterSafeArea(isSafe);
                        //   Log.DebugBrown($"玩家是否在安全区：{UnitEntityComponent.Instance.LocalRole.IsSafetyZone}");
                    }
                    //龙王旗帜 刷怪物 到怪物范围 隐藏副本信息
                    if (GlobalDataManager.astarNode != null && GlobalDataManager.MapId == SceneComponent.Instance.CurrentSceneIndex)
                    {
                        int x = GlobalDataManager.astarNode.x;
                        int y = GlobalDataManager.astarNode.z;
                        if (x > node.x - 5 && x < node.x + 5 && y > node.z - 5 && y < node.z + 5)
                        {
                            UIMainComponent.Instance?.HideFuBenInfo();
                        }
                    }
                }
            }
            catch (Exception e)
            {

                Log.DebugRed($"SendMovePos exception target={node?.x},{node?.z} error={e}");
                LoginStageTrace.Append($"MoveDiag SendMovePos exception target={node?.x},{node?.z} error={e.GetType().Name}:{e.Message}");
            }

        }

        private IEnumerator ShowA()
        {
            yield return new WaitForSeconds(1);
            Log.DebugBrown("本地111记录的区域" + PlayerPrefs.GetString("pos") + "当前的地图" + SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
        }
        /// <summary> 
        /// 向服务端发送 玩家当前的格子坐标信息
        /// </summary>
        /// <returns></returns>
        public async ETVoid NewSendMovePos(AstarNode node)
        {

            if (Time.time > spacetime)
            {
                //每隔.5秒 检测拾取物品
                Game.EventCenter.EventTrigger<AstarNode>(EventTypeId.LOCALROLE_GRIDCHANGE, roleEntity.CurrentNodePos);
                spacetime = Time.time + .5f;
            }
            UIMainComponent.Instance?.ChangeRolePosTxt(roleEntity.CurrentNodePos);
            try
            {
                //Log.DebugGreen($"请求移动目标点 ({node.x},{node.z})");
                c2G_MovePosRequest.X = node.x;
                c2G_MovePosRequest.Y = node.z;
                c2G_MovePosRequest.Angle = (int)roleEntity.Game_Object.transform.parent.eulerAngles.y;
                g2C_MovePosResponse = (G2C_MovePosResponse)await SessionComponent.Instance.Session.Call(c2G_MovePosRequest);

                Log.DebugGreen("错误码提示" + g2C_MovePosResponse.Error + "::当前所在区域" + SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());


                if (g2C_MovePosResponse.Error != 0)
                {
                    // UIComponent.Instance.VisibleUI("等级不足，传送错误");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足，传送错误");
                    PlayerPrefs.SetInt("one", node.x);
                    PlayerPrefs.SetInt("two", node.z);
                    return;
                    //场景加载进度
                    //  UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                }
                if (g2C_MovePosResponse.Error == 0)
                {
                    //NewPos().Coroutine();
                    //async ETVoid NewPos()
                    //{
                    //    Log.DebugBrown("本地111记录的区域" + PlayerPrefs.GetString("pos") + "当前的地图" + SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                    //}
                    //   ShowA();
                    Log.DebugBrown("本地记录的区域" + PlayerPrefs.GetString("pos") + "当前的地图" + SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                    if (PlayerPrefs.GetInt("one") <= node.x + 3 && PlayerPrefs.GetInt("one") >= node.x - 3 || PlayerPrefs.GetInt("two") <= node.z + 3 && PlayerPrefs.GetInt("two") >= node.z - 3)
                    {
                        Log.DebugGreen("本地不满足传送");
                    }
                    else
                    {
                        NewPos().Coroutine();
                        async ETVoid NewPos()
                        {
                            // Log.DebugBrown("本地111记录的区域" + PlayerPrefs.GetString("pos") + "当前的地图" + SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                        }
                        if (PlayerPrefs.GetString("pos") == null)
                        {
                            return;
                        }
                        if (PlayerPrefs.GetString("pos") != SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName())
                        {
                            Log.DebugBrown("本地1");
                            //PlayerPrefs.SetString("pos", SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                            //if (SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.AnNingChi.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.kalima_map.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.DiXiaCheng.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.ShiLuoZhiTa.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.BingFengGu.GetSceneName())
                            //{
                            //    Debug.Log("本地1特殊地图");
                            //}
                            //else
                            //{
                            //    Log.DebugBrown("本地12");
                            //  //  PlayerPrefs.SetString("pos", SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                            //    UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                            //}
                            PlayerPrefs.SetString("pos", SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                            UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                        }
                        else
                        {
                            if (PlayerPrefs.GetString("pos") == SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName())
                            {
                                Debug.Log("本地在同一地图，不需要加载");
                            }
                            else
                            {
                                //特殊地图在同一个地图，不需要加载
                                if (SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.AnNingChi.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.kalima_map.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.GuZhanChang.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.DiXiaCheng.GetSceneName())
                                {
                                    Debug.Log("本地特殊地图");
                                }
                                else
                                {
                                    Log.DebugBrown("本地2");
                                    UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                                }
                            }
                        }



                    }

                    //DOTween.To(() => 0, x =>
                    //{

                    //}, 0, 1).OnComplete(() =>
                    //{
                    //    Log.DebugBrown("本地111记录的区域" + PlayerPrefs.GetString("pos") + "当前的地图" + SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName());
                    //});

                    //if (PlayerPrefs.GetInt("score")!= 1)
                    //{
                    //    Log.DebugGreen("传送");
                    //   // UIComponent.Instance.VisibleUI(UIType.UISceneLoading);

                    //}
                    //else
                    //{
                    //    PlayerPrefs.SetInt("score", 2);
                    //}
                }
                //if (PlayerPrefs.GetInt("score")!=1)
                //{
                //    if (g2C_MovePosResponse.Error ==0)
                //    {
                //        PlayerPrefs.SetInt("score", 2);
                //        Log.DebugGreen("传送");
                //        UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                //    }
                //}

                if (g2C_MovePosResponse.Error != 0)//506 正在攻击
                {
                    //StopNavigation?.Invoke();
                    if (g2C_MovePosResponse.Error == 506)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MovePosResponse.Error.GetTipInfo());
                    }
                    if (g2C_MovePosResponse.X != 0)
                    {
                        roleEntity.CurrentNodePos = AstarComponent.Instance.GetNode(g2C_MovePosResponse.X, g2C_MovePosResponse.Y);
                        StopMove();
                        Log.DebugRed($"g2C_MovePosResponse.Error:{g2C_MovePosResponse.Error.GetTipInfo()}  {g2C_MovePosResponse.Error}  g2C_MovePosResponse.X:{g2C_MovePosResponse.X}:{g2C_MovePosResponse.Y}  node:{node}");
                    }


                    // curGridIndex = lastGridIndex;

                    //主动控制玩家移动时 若当前正在寻路 则停止寻路
                    if (g2C_MovePosResponse.X != 0)
                    {
                        //  NextastarNode = AstarComponent.Instance.GetNode(g2C_MovePosResponse.X, g2C_MovePosResponse.Y);
                        // IsIgnoreInputTargetPos = true;
                    }
                    else
                    {
                        //   UnitEntityPathComponent.Path.Add(AstarComponent.Instance.GetVectory3(g2C_MovePosResponse.X, g2C_MovePosResponse.Y)) ;
                    }

                }
                else
                {
                    //Log.DebugGreen($"允许玩家移动:{g2C_MovePosResponse.X}-{g2C_MovePosResponse.Y}");
                    //判断是否在安全区
                    if (SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(roleEntity.CurrentNodePos.x, roleEntity.CurrentNodePos.z)) != roleEntity.IsSafetyZone)
                    {
                        bool isSafe = SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int(roleEntity.CurrentNodePos.x, roleEntity.CurrentNodePos.z));
                        UnitEntityComponent.Instance.LocalRole.IsSafetyZone = isSafe;

                        UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>().EnterSafeArea(isSafe);
                        //   Log.DebugBrown($"玩家是否在安全区：{UnitEntityComponent.Instance.LocalRole.IsSafetyZone}");
                    }
                    //龙王旗帜 刷怪物 到怪物范围 隐藏副本信息
                    if (GlobalDataManager.astarNode != null && GlobalDataManager.MapId == SceneComponent.Instance.CurrentSceneIndex)
                    {
                        int x = GlobalDataManager.astarNode.x;
                        int y = GlobalDataManager.astarNode.z;
                        if (x > node.x - 5 && x < node.x + 5 && y > node.z - 5 && y < node.z + 5)
                        {
                            UIMainComponent.Instance?.HideFuBenInfo();
                        }
                    }
                }
            }
            catch (Exception e)
            {

                Log.DebugRed(e.ToString());
            }

        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            RedDotManagerComponent.SavalocalRedDot();
            Game.EventCenter.RemoveEvent<Vector2>(EventTypeId.UI_JOYSTICK_VALUE, ChangeMoveDir);
            Game.EventCenter.RemoveEvent(EventTypeId.UI_JOYSTICK_UP, PointerUp);
            Game.EventCenter.RemoveEvent(EventTypeId.UI_JOYSTICK_DOWN, PointerDown);
            base.Dispose();
        }
    }
}
