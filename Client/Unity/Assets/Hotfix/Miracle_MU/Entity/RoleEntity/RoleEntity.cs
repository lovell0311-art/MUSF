using UnityEngine;
using ETModel;
using System;

using DG.Tweening;

namespace ETHotfix
{
    [ObjectSystem]
    public class RoleEntityAwake : AwakeSystem<RoleEntity, GameObject>
    {
        public override void Awake(RoleEntity self, GameObject gameObject)
        {
            self.Awake(gameObject);
        }
    }
    [ObjectSystem]
    public class LHRoleEntityAwake : AwakeSystem<RoleEntity, AstarNode>
    {
        public override void Awake(RoleEntity self, AstarNode a)
        {
            self.LHAwake(a);
        }
    }

    /// <summary>
    /// 玩家实体
    /// </summary>
    [HideInInspector]
    public class RoleEntity : UnitEntity
    {
        /// <summary>
        /// 角色类型
        /// </summary>
        private E_RoleType roleType;
        //玩家昵称
        public string RoleName;
        /// <summary>
        /// 玩家的 转职等级 默认为0 未转职
        /// </summary>
        public int ClassLev => (int)Property.GetProperValue(E_GameProperty.OccupationLevel);
        /// <summary>
        /// 玩家的等级
        /// </summary>
        public int Level => (int)Property.GetProperValue(E_GameProperty.Level);
        /// <summary>
        /// 联盟名字
        /// </summary>
        public string unionName;
        /// <summary>
        /// 联盟职位
        /// </summary>
        public string unionPost;

        /// <summary>
        /// 是否在队伍中
        /// </summary>
        public bool IsInTeam = false;

        /// <summary>
        /// 首充 状态
        /// 
        /// </summary>
        public int RechargeStates = -1;
        /// <summary>
        /// 首充Dictionary<int, int> <档次，金额>
        /// </summary>
        public string RechargeRecord;

        /// <summary>
        /// 转生的当前次数
        /// </summary>
        public int Reincarnation = 0;
        public E_RoleType RoleType
        {
            get { return roleType; }
            set { roleType = value; }
        }
        /// <summary>
        /// 实体的技能信息缓存到本地的文件名
        /// </summary>
        public string LocalSkillFillName => Id + "SkillSlotInfo";
        /// <summary>
        /// 是否在完全区
        /// </summary>
        public bool IsSafetyZone = false;

        /// <summary>
        /// 小月卡倒计时
        /// </summary>
        public TimeSpan MinMonthluCardTimeSpan = new TimeSpan();
        //大月卡
        public TimeSpan MaxMonthluCardTimeSpan = new TimeSpan();
        //原地复活CD
        public TimeSpan InsiteTimeSpan = new TimeSpan();
        //是否有原地复活特权
        public bool InSitu => MaxMonthluCardTimeSpan.TotalSeconds > 0 || TitleManager.allTitles.Exists(x => x.TitleId == 60005);//MinMonthluCardTimeSpan.TotalSeconds > 0 || 

        /// <summary>
        /// 召唤兽
        /// </summary>
        public SummonEntity summonEntity = null;

        public Transform roleTrs;

        public string layerTag;
        public string layer;
        public BufferComponent bufferComponent;
        public RoleStallUpComponent roleStallUpComponent;
        public void LHAwake(AstarNode a)
        {
            CurrentNodePos = a;
            this.DelayTime = 0;
        }

        public void Init(E_RoleType roleType, string roleName, long title, string warTitle, int angle, int classLev, string tag, string layer)
        {
            layerTag = tag;
            RoleType = roleType;
            RoleName = roleName;
            this.layer = layer;
            Property.ChangeProperValue(E_GameProperty.OccupationLevel, classLev);//转职等级

            var pos = AstarComponent.Instance.GetVectory3(CurrentNodePos.x, CurrentNodePos.z);

            AddComponent<SiegeWarfareComponent>();//攻城战组件
            AddComponent<SitDownStoolsComponent>();//老板娘坐下
            //初始化摆摊组件
            roleStallUpComponent = AddComponent<RoleStallUpComponent, int>(1);


            AddComponent<UnitEntityMoveComponent>();
            AddComponent<TurnComponent>();

            bufferComponent = AddComponent<BufferComponent>();//Buffer组件
            AddComponent<UnitEntityHitTextComponent>().SetColor(Color.red);


            IsSafetyZone = SafeAreaComponent.Instances.IsSafeAreas(new Vector2Int((int)pos.x / 2, (int)pos.z / 2));//是否再安全区

            string roleResName = roleType.GetRoleResName();
            ResourcesComponent.Instance.LoadGameObjectAsync(roleResName.StringToAB(), roleResName, () => { LoadRole(angle, title, warTitle); }).Coroutine();
        }

        public void LoadRole(int angle, long title, string warTitle)
        {

            string roleResName = RoleType.GetRoleResName();

            if (this.IsDead || this.IsDisposed)
            {
                Log.Info("角色已经销毁" + roleResName);
                return;
            }

            GameObject roleObj = ResourcesComponent.Instance.LoadGameObject(roleResName.StringToAB(), roleResName);
            if (roleObj == null)
            {
                //monsterObj = ResourcesComponent.Instance.LoadGameObject("Monster_YouLong".StringToAB(), "Monster_YouLong");
                Log.Info("未查找到怪物包体");
                return;
            }
            this.Game_Object = roleObj.transform.GetChild(0).gameObject;//对应的角色模型
            roleTrs = roleObj.transform;
            roleObj.tag = layerTag;
            roleObj.transform.GetChild(0).tag = layerTag;
            roleObj.SetLayer(layer);

            var pos = AstarComponent.Instance.GetVectory3(CurrentNodePos.x, CurrentNodePos.z);


            UIUnitEntityHpBarComponent hpBarEntity = AddComponent<UIUnitEntityHpBarComponent>();
            //显示角色名字
            hpBarEntity.SetEntityName(RoleName);
            hpBarEntity.SetEntityWarName(warTitle);
            hpBarEntity.SetEntityTitle(title);
            hpBarEntity.ChangeNameColor(GetRedNameColor());

            AddComponent<AnimatorComponent>();//动画组件

            AddComponent<RoleSkillComponent>();//技能组件
            AddComponent<UnitEntityPathComponent>();
            roleStallUpComponent.Init(hpBarEntity);

            AddComponent<RoleEquipmentComponent>().GetWareEquips().Coroutine();//装备组件
            bufferComponent.InitBuffer();

            roleObj.transform.localPosition = pos.GroundPos();
            roleObj.transform.localRotation = Quaternion.Euler(0, angle, 0);
        }


        public void Awake(GameObject gameObject)
        {
            this.Game_Object = gameObject.transform.GetChild(0).gameObject;//对应的角色模型
            roleTrs = gameObject.transform;
            this.DelayTime = 0;

        }

        /// <summary>
        ///  玩家使用 坐骑时  玩家的高度会变化
        /// </summary>
        public override Vector3 Position
        {
            get
            {
                if (this.Game_Object == null)
                {
                    return AstarComponent.Instance.GetVectory3(this.CurrentNodePos);
                }
                //  return Game_Object.transform.parent.position;
                return roleTrs.position;
            }
            set
            {
                if (Game_Object != null)
                {
                    //  Game_Object.transform.parent.transform.localPosition = value;
                    roleTrs.position = value;
                }
                // Game_Object.transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 玩家使用 坐骑时  玩家的高度会变化
        /// </summary>
        public override Quaternion Rotation
        {
            get
            {
                //  return Game_Object.transform.parent.rotation;
                return roleTrs.rotation;
            }
            set
            {
                if (Game_Object != null)
                {
                    /* Game_Object.transform.parent.rotation = value;
                     Game_Object.transform.localRotation = Quaternion.identity;*/
                    roleTrs.rotation = value;
                }
            }
        }

        /// <summary>
        /// 玩家实体 死亡
        /// </summary>
        public override void Dead()
        {
            base.Dead();
            //if(SiegeWarfareData.currole != null && SiegeWarfareData.currole == this)
            //{
            //    this.GetComponent<AnimatorComponent>()?.SetBoolValue("SiegeSitDown", false); 
            //    SiegeWarfareComponent.Instance.LeveaThrone(this.Game_Object).Coroutine();
            //}
            // Log.DebugBrown($"玩家实体死亡:{this.RoleName}");
            if (roleType == E_RoleType.Summoner || roleType == E_RoleType.Archer || roleType == E_RoleType.GrowLancer)//播放 死亡音效
            {
                SoundComponent.Instance.PlaySkill("WomanDead");
            }
            else
            {
                SoundComponent.Instance.PlaySkill("ManDead");
            }
            this.GetComponent<AnimatorComponent>()?.SetTrigger(MotionType.Dead);//播放死亡动画 

            if (this.Id == UnitEntityComponent.Instance.LocaRoleUUID)//本地玩家
            {
                IsDead = true;
                UIMainComponent.Instance.HookTog.isOn = false;
                this.GetComponent<BufferComponent>().ClearBuffer();
                //  Game.EventManager.BroadcastEvent(EventTypeId.LOCALROLE_DEAD);//广播本地 玩家死亡
                Game.EventCenter.EventTrigger(EventTypeId.LOCALROLE_DEAD);//广播本地 玩家死亡
                this.GetComponent<UnitEntityPathComponent>().moveNodes.Clear();//清空移动点
                this.GetComponent<UnitEntityPathComponent>().Path.Clear();//清空移动点
                this.GetComponent<UnitEntityPathComponent>().StopMove();
                TaskDatas.AutoNavCallBack = null;
                //this.GetComponent<RoleEquipmentComponent>().ChangeMountState(true);
                this.GetComponent<BufferComponent>().RemoveAllBuffer();

                if (SceneComponent.Instance.CurrentSceneIndex == (int)SceneName.ShiLianZhiDi)
                {
                    UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                }
                else
                {


                    //原地复活特权
                    // Log.DebugBrown($"InSitu:{InSitu}  InsiteTimeSpan.TotalSeconds:{InsiteTimeSpan.TotalSeconds}");
                    // string scenceName = SceneNameExtension.GetSceneName(SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>());
                    //  Log.DebugBrown($"死亡地图:{scenceName}");
                    if (InSitu && InsiteTimeSpan.TotalSeconds <= 0)
                    {
                        //拥有原地复活特权 
                        UIMainComponent.Instance.ShowInSitu();
                    }
                    else
                    //}else if (scenceName == "血色城堡")
                    //{
                    //    //现实任务失败界面
                    //    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    //    uIConfirmComponent.SetTipText($"");
                    //    uIConfirmComponent.AddActionEvent(() =>
                    //    {

                    //    });
                    //}
                    //else
                    {
                        //安全区 复活
                        UIMainComponent.Instance.ShowDeadMask();
                    }
                }

            }
            else
            {
                this.Dispose();
            }

        }
        /// <summary>
        /// 本地玩家复活
        /// </summary>
        /// <param name="pos"></param>
        public void LocalRoleRevive(Vector3 pos)
        {

            UIMainComponent.Instance.SetMask(false);
            IsDead = false;
            this.GetComponent<AnimatorComponent>().SetTrigger(MotionType.Revive);
            this.Position = pos.GroundPos();
            UIMainComponent.Instance.ChangeRolePosTxt(AstarComponent.Instance.GetNodeVector(pos.x, pos.z));

            if (IsDead)
            {
                if (this.GetComponent<RoleOnHookComponent>() is RoleOnHookComponent roleOnHookComponent && roleOnHookComponent.IsOnHooking)
                {
                    roleOnHookComponent.IsOnHooking = false;
                    roleOnHookComponent.isReturn = false;
                    roleOnHookComponent.IsAttack = false;
                    UIMainComponent.Instance.HookTog.isOn = false;
                }
            }
        }

        public void ChangeRolePos(AstarNode nodepos, Action callback = null)
        {

            if (this.GetComponent<UnitEntityMoveComponent>().Moving)
            {
                this.GetComponent<UnitEntityMoveComponent>().Stop();
            }
            this.GetComponent<AnimatorComponent>().Idle();
            //this.Game_Object.transform.parent.localPosition= AstarComponent.Instance.GetVectory3(nodepos).GroundPos();
            this.roleTrs.localPosition = AstarComponent.Instance.GetVectory3(nodepos).GroundPos();

            UIMainComponent.Instance.ChangeRolePosTxt(nodepos);
            //AstartComponentExtend.MoveSendNotice(this.CurrentNodePos, nodepos, this.Id);
            this.CurrentNodePos = nodepos;
            callback?.Invoke();

        }
        /// <summary>
        ///根据PK 惩罚点数获取 角色名称显示颜色
        /// </summary>
        /// <returns></returns>
        public string GetRedNameColor()
        {
            var point = this.Property.GetProperValue(E_GameProperty.PkNumber);
            if (point > 43200)//极恶（深红色）
            {
                return "#f70004";
            }
            else if (point > 21600)//邪恶（红色）
            {
                return "#f72533";
            }
            else if (point > 1)//罪恶(黄色）
            {
                return "#f7f700";
            }
            else
            {
                return "#FFFFFF";//善良（白色）
            }
        }
        public override void Dispose()
        {
            //if (this.Game_Object != null && this.Game_Object.activeSelf==false)
            //{
            //    this.Game_Object.SetActive(true);
            //}
            if (this.IsDisposed) return;



            base.Dispose();

            RoleName = String.Empty;
            IsSafetyZone = false;
            MinMonthluCardTimeSpan = new TimeSpan();
            MaxMonthluCardTimeSpan = new TimeSpan();
            InsiteTimeSpan = new TimeSpan();
            summonEntity = null;

        }

    }
}
