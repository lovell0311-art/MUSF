
using UnityEngine;
using ETModel;
using System.Linq;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [ObjectSystem]
    public class RoleOnHookComponentAwake : AwakeSystem<RoleOnHookComponent>
    {
        public override void Awake(RoleOnHookComponent self)
        {
            self.roleEntity = self.GetParent<RoleEntity>();
            self.Awake();
            self.skillComponent = self.roleEntity.GetComponent<RoleSkillComponent>();
            self.IsOnHooking = false;
        }
    }
    [ObjectSystem]
    public class RoleOnHookComponentStart : StartSystem<RoleOnHookComponent>
    {
        public override void Start(RoleOnHookComponent self)
        {
            self.controlComponent = self.roleEntity.GetComponent<RoleMoveControlComponent>();
            self.pathComponent = self.roleEntity.GetComponent<UnitEntityPathComponent>();
           
            if (UIComponent.Instance.Get(UIType.UIMainCanvas) != null)
            {
                if (UIMainComponent.Instance.HookTog.isOn)
                {
                    self.IsOnHooking = true;
                }
            }
            self.animatorComponent = self.roleEntity.GetComponent<AnimatorComponent>();
            self.stateInfo = self.animatorComponent.Animator.GetCurrentAnimatorStateInfo(4);
            self.SkillstateInfo = self.animatorComponent.Animator.GetCurrentAnimatorStateInfo(5);

            OnHookSetInfoTools.ShowEffectAction = self.ShowOnHookEffect;
            OnHookSetInfoTools.HideEffectAction = self.HideHookEffect;
        }
    }
    [ObjectSystem]
    public class RoleOnHookComponentUpdate : UpdateSystem<RoleOnHookComponent>
    {
        public override void Update(RoleOnHookComponent self)
        {
            if (self.IsOnHooking && Time.time > self.curSpaceTime)
            {
               // Log.DebugGreen("检查物品是否可以拾取");
                self.AutoPickUpItem();
                self.curSpaceTime=Time.time+self.spaceTime;
            }

            if (self.IsOnHooking && self.NavTarget && (self.controlComponent == null || self.controlComponent.IsNavigation == false))
            {
                self.NavTarget = false;
            }

            if (self.IsOnHooking == false || self.IsAttack || self.NavTarget || (OnHookSetInfoTools.IsUseSkill == false && OnHookSetInfoTools.IsUseAttack == false))
            {
             //   Log.DebugYellow($"self.IsOnHooking:{self.IsOnHooking} self.IsAttack:{self.IsAttack} self.NavTarget:{self.NavTarget} self.OnHookSetInfoTools.IsUseSkill:{self.OnHookSetInfoTools.IsUseSkill} self.OnHookSetInfoTools.IsUseAttack:{self.OnHookSetInfoTools.IsUseAttack}");
                return;
            }

            /*if (self.IsAttacking())
            {
                return;
            }*/

            if (self.isReturn && self.controlComponent.IsNavigation)
            {
               
                return;//处于返回原点中
            }
            else
            {
                self.isReturn = false;
            }

            //正在自动寻路
            if (self.controlComponent.IsNavigation)
            {
                if (self.curAttackEntity != null && UnitEntityComponent.Instance.CurAttackEntityIsDieOrLeveView(self.curAttackEntity) == false)
                {
                    //当前目标已经被其他玩家 击杀
                    self.curAttackEntity = null;
                    self.NavTarget = false;
                    self.pathComponent.StopMove();
                    return;
                }
                if (self.curAttackEntity != null)
                {
                    var dis = PositionHelper.Distance(self.curAttackEntity.CurrentNodePos, self.roleEntity.CurrentNodePos);
                    //玩家已经移动到挂机原点圈的边缘 则返回原点
                    if (self.IsReturen && dis >= 10)
                    {

                        self.curAttackEntity = null;
                        self.pathComponent.StopMove();
                        self.pathComponent.NavTarget(self.StartPos);
                        self.isReturn = true;
                       
                        return;

                    }

                    if (self.skillBtnEntity != null)
                    {

                        //怪物在伤害范围内  可以原地释放的技能
                        if (self.skillBtnEntity.SkillDamageRange != 0 && dis <= self.skillBtnEntity.SkillDamageRange)
                        {
                            self.pathComponent.StopMove();
                            self.skillBtnEntity.RequestAttackUnit(self.curAttackEntity, ref self.IsAttack);
                            return;
                        }

                        //怪物在攻击范围内
                        if (self.skillBtnEntity.SkillAttackRange != 0 && dis <= self.skillBtnEntity.SkillAttackRange)
                        {

                            //进入可攻击范围
                            self.pathComponent.StopMove();
                            self.skillBtnEntity.RequestAttackUnit(self.curAttackEntity, ref self.IsAttack);
                            return;
                        }
                    }

                    return;

                }


            }

            if (TimeHelper.Now() <= GlobalDataManager.AttackSpaceTime)
            {
                return;
            }
            if (self.IsReturen)//返回原点
            {
                UnitEntityComponent.Instance.GetMinDistacneEntity(ref self.curAttackEntity, self.StartPos, 10);
                if (self.curAttackEntity == null)//周围没有怪物
                {
                    if (PositionHelper.Distance(self.roleEntity.CurrentNodePos, self.StartPos) >= 1)
                    {
                        self.waitReurenTime += Time.deltaTime;
                        if (self.waitReurenTime >= 2)//等待2秒 返回原点
                        {
                            self.waitReurenTime = 0;
                            self.pathComponent.NavTarget(self.StartPos);
                            self.isReturn = true;
                            return;
                        }
                    }
                }
            }
            else
            {
                UnitEntityComponent.Instance.GetMinDistacneEntity(ref self.curAttackEntity);
            }

            if (self.curAttackEntity == null) return;

            //使用技能
            if (OnHookSetInfoTools.IsUseSkill)
            {
                if (self.skillCount == 0)
                {
                    self.skillBtnEntity = null;
                }
                self.curSkillIndex = self.curSkillIndex >= self.skillCount ? 0 : self.curSkillIndex;
                if (self.skillCount == 0)
                {
                    self.skillBtnEntity = null;
                }
                for (; self.curSkillIndex < self.skillCount;)
                {
                    self.skillBtnEntity = null;
                    self.curSkillIndex = self.curSkillIndex > self.skillCount ? 0 : self.curSkillIndex;
                   
                    if (!self.Skills[self.curSkillIndex].IsCanUser())
                    {
                        self.curSkillIndex++;
                        continue;
                    }
                   
                    self.skillBtnEntity = self.Skills[self.curSkillIndex];
                    self.curSkillIndex = ++self.curSkillIndex >= self.skillCount ? 0 : self.curSkillIndex;
                    break;
                }
           
                if (self.skillBtnEntity == null)
                {
                    if (OnHookSetInfoTools.IsUseAttack)
                        self.skillBtnEntity = self.mainComponent.attackEntity;
                    else return;
                }
            }
            else if (OnHookSetInfoTools.IsUseAttack)
            {
                self.skillBtnEntity = self.mainComponent.attackEntity;
            }


            if (self.skillBtnEntity.skill_Info.skillType!=2&&self.curAttackEntity == null)
            {
                self.curSkillIndex--;
                return;
            }
            if (self.skillBtnEntity.skill_Info.skillType == 2)//辅助技能
            {
                self.skillBtnEntity.RequestAttackUnit(self.curAttackEntity, ref self.IsAttack);
            }
            else if (OnHookSetInfoTools.IsOriginOnHook == false && PositionHelper.Distance(self.curAttackEntity.CurrentNodePos, self.roleEntity.CurrentNodePos) > self.skillBtnEntity.SkillAttackRange)
            {

                //怪物与玩家之间的距离大于 当前技能的攻击范围
                self.NavTarget = true;
                self.pathComponent.NavTarget(self.curAttackEntity.CurrentNodePos);

            }
            else
            {
                //攻击实体
                self.skillBtnEntity.RequestAttackUnit(self.curAttackEntity, ref self.IsAttack);
            }

        }

    }
    /// <summary>
    /// 挂机组件
    /// </summary>
    public class RoleOnHookComponent : Component
    {
        public RoleEntity roleEntity;

        public UnitEntity curAttackEntity = null;

        public int curSkillIndex = 0;


        public static RoleOnHookComponent Instance;

        public bool IsOnHooking = false;//是否处于挂机状态

        public UIMainComponent mainComponent;

    
        //是否正在攻击
        public bool IsAttack = false;


        public float waitReurenTime = 0;

        public bool IsReturen = false;//是否返回挂机原点

        public AstarNode StartPos;//挂机原点
        public void Awake() => Instance = this;

        public string HookEffectResName = "Effect_OnHookBackpos";//挂机特效 资源名
        GameObject HookEffect;//挂机特效

        public UnitEntityPathComponent pathComponent;
        public bool isReturn = false;

        public RoleMoveControlComponent controlComponent;
        public bool NavTarget = false;//是否正在寻路

        public RoleSkillComponent skillComponent;
        public UISkillBtnEntity skillBtnEntity = null;

        public List<UISkillBtnEntity> Skills = new List<UISkillBtnEntity>();
        public int skillCount;

       
        public bool IsCanAutoPickUpGoldCoin = true;//是否可以自动拾取金币

         public AnimatorComponent animatorComponent;
        public AnimatorStateInfo stateInfo,SkillstateInfo;


        //间隔自动拾取
        public float spaceTime =5;//每隔五秒自动遍历一下拾取装备
        public float curSpaceTime = Time.time;

        public void ChangeSkillInfo()
        {
            //Dictionary<int, UISkillBtnEntity> skillDic = UIMainComponent.Instance.AllskillBtnsDic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => (UISkillBtnEntity)p.Value);
            Skills.Clear();
            Skills = UIMainComponent.Instance.AllskillBtnsDic.Values.ToList();
            Skills.Sort((a, b) => 
            {
                //按卡槽顺序升序
              return a.slotId.CompareTo(b.slotId);
            });
         
            skillCount = Skills.Count;
            curSkillIndex = 0;
        }

      
        /// <summary>
        /// 重置当前 攻击的对象
        /// </summary>
        public void ClearCurAttackEntity()
        {
            curAttackEntity = null;
            UnitEntityComponent.Instance.curAttackEntity = -1;
            UnitEntityComponent.Instance.curAttackUnitEntity = null;
        }

        /// <summary>
        /// 开始挂机
        /// </summary>
        public void StartOnHook()
        {
            Application.targetFrameRate = 30;
            IsOnHooking = true;
            NavTarget = false;
            IsAttack = false;
            isReturn = false;
            waitReurenTime = 0;
            ClickSelectUnitEntityComponent.Instance?.ClearSelectUnit();
           
            IsReturen = OnHookSetInfoTools.IsReturnOrigin;
            curSkillIndex = 0;
            ClearCurAttackEntity();
            UnitEntityComponent.Instance.CanNotPkRoleList.Clear();
            mainComponent = UIMainComponent.Instance;
            if (OnHookSetInfoTools.IsReturnOrigin)
                ShowOnHookEffect();

            skillBtnEntity = null;
            curAttackEntity = null;

            //按照卡槽顺序 排序
            /*  Dictionary<int, UISkillBtnEntity> skillDic = UIMainComponent.Instance.AllskillBtnsDic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => (UISkillBtnEntity)p.Value);

              Skills = skillDic.Values.ToList();*/

         
            Skills = UIMainComponent.Instance.AllskillBtnsDic.Values.ToList();
            Skills.Sort((a, b) =>
            {
                //按卡槽顺序升序
                return a.slotId.CompareTo(b.slotId);
            });
            skillCount = Skills.Count;

        }

        /// <summary>
        /// 暂停挂机
        /// </summary>
        public void StopOnHook()
        {
          
            IsOnHooking = false;
            IsAttack = false;
            NavTarget = false;
            IsReturen = false;
            isReturn = false;
            waitReurenTime = 0;
            ClearCurAttackEntity();
            UnitEntityComponent.Instance.CanNotPkRoleList.Clear();
            pathComponent.StopMove();
            if (HookEffect != null)
                HookEffect.SetActive(false);
        }
        /// <summary>
        /// 是否处于攻击中
        /// </summary>
        /// <returns></returns>
        public bool IsAttacking() 
        {
            if (stateInfo.IsName("Idle") == false || SkillstateInfo.IsName("Idle") == false)
            {
                Log.DebugGreen($"是否处于攻击状体：true");
                return true;
            }
            else
            {
                Log.DebugYellow($"是否处于攻击状体：false");
            }
            return false;
        }
        /// <summary>
        /// 显示挂机特效
        /// </summary>
        public void ShowOnHookEffect()
        {
            if (IsOnHooking == false) return;
            HookEffect = HookEffect != null ? HookEffect : ResourcesComponent.Instance.LoadGameObject(HookEffectResName.StringToAB(), HookEffectResName);
            StartPos = roleEntity.CurrentNodePos;
           
            HookEffect.transform.position = roleEntity.Position + Vector3.up * .5F;
            HookEffect.SetActive(true);
            IsReturen = true;
        }
        //隐藏 返回原地挂机特效
        public void HideHookEffect()
        {
            if (HookEffect != null)
                HookEffect.SetActive(false);
            IsReturen = false;
        }
        /// <summary>
        /// 挂机时 自动使用药品
        /// </summary>
        /// <param name="Ratio"></param>
        public void UseMedicnieHp(float Ratio)
        {

            if (UIMainComponent.Instance.medicineEntity_Hp != null)
            {
                if (OnHookSetInfoTools.IsAuto_30 && Ratio <= .3f)
                {
                    UIMainComponent.Instance.medicineEntity_Hp.UserMedicine();
                }
                else if (OnHookSetInfoTools.IsAuto_50 && Ratio <= .5f)
                {
                    UIMainComponent.Instance.medicineEntity_Hp.UserMedicine();
                }
                else if (OnHookSetInfoTools.IsAuto_80 && Ratio <= .8f)
                {
                    UIMainComponent.Instance.medicineEntity_Hp.UserMedicine();
                }
                else
                {
                    if (Ratio <= .4f)
                    {
                        UIMainComponent.Instance.medicineEntity_Hp.UserMedicine();
                    }
                }
            }
        }

        public void UseMedicineMp(float Ratio)
        {
            if (UIMainComponent.Instance.medicineEntity_Mp!=null&& Ratio <= .4)
            {
                UIMainComponent.Instance.medicineEntity_Mp.UserMedicine();
            }
        }

        /// <summary>
        /// 是否可以拾取该物品
        /// </summary>
        /// <param name="knapsackItem"></param>
        /// <returns></returns>
        public bool IsCanAutoPick(KnapsackItemEntity item)
        {
            if (item.item_Info.Id == 320294||item.item_Info.Id== 320316)//金币
            {
               // return this.IsCanAutoPickUpGoldCoin;
                return true;
            }
            if (OnHookSetInfoTools.IsDiyPicUp)
            {
                //自定义拾取
                if (OnHookSetInfoTools.DIYPickList.Count != 0 && string.IsNullOrEmpty(item.item_Info.Name) == false)
                {

                    for (int d = 0, count = OnHookSetInfoTools.DIYPickList.Count; d < count; d++)
                    {
                        if (item.item_Info.Name.Contains("金币")) continue;
                        if (item.item_Info.Name.Contains(OnHookSetInfoTools.DIYPickList[d]))
                        {

                            return true;

                        }
                    }

                }
            }

            if (OnHookSetInfoTools.IsPickUpSuitEquip)//拾取套装
            {
                if (item.IsSuit || item.IsHaveExecllentEntry)
                {
                    return true;
                }
            }
            if (OnHookSetInfoTools.IsPickUpGems)//拾取宝石 原石
            {
                if (item.item_Info.Id / 10000 == (int)E_ItemType.Gemstone || item.item_Info.Id / 10000 == (int)E_ItemType.FGemstone)
                    return true;
            }
            if (OnHookSetInfoTools.IsPickUpMountMat)//拾取坐骑材料
            {
                if (KnapsackItemsManager.MountMatDic.ContainsKey(item.item_Info.Id))
                    return true;
            }
            if (OnHookSetInfoTools.IsPickUpYuMao)//拾取羽毛
            {
                if (KnapsackItemsManager.YuMaoDic.ContainsKey(item.item_Info.Id))
                    return true;
            }
            if (OnHookSetInfoTools.IsPickUpLuck)//拾取幸运装备
            {
                if (item.IsHaveLuckyEntry)
                {
                    return true;
                }
            }
            if (OnHookSetInfoTools.IsPickUpSkill)//拾取技能装备
            {
                if ((item.dropData.Quality & 1 << 0) == 1 << 0)
                {
                    return true;
                }
            }
            if (OnHookSetInfoTools.IsPickUpAllEquip)//拾取所有道具
            {
                return true;
            }
           
            return false;
        }
        /// <summary>
        /// 自动拾取物品
        /// </summary>
        public void AutoPickUpItem()
        {
            if (KnapsackItemsManager.IsPackBackpack || IsOnHooking == false) return;
            var knapsackitemList = UnitEntityComponent.Instance.KnapsackItemEntityDic.Values.ToList();

            foreach (var item in knapsackitemList)
            {
                if (UnitEntityComponent.Instance.KnapsackItemEntityDic.ContainsKey(item.Id) == false) continue;

                if (item.dropData.KillerId.Contains(roleEntity.Id) == false|| item.dropData.CreatType==1 ||  KnapsackItemsManager.DisDropItemList.Contains(item.dropData.InstanceId)) continue;//正在整理背包、玩家丢弃的物品、玩家不是拾取

                if (PositionHelper.Distance(this.roleEntity.CurrentNodePos, item.CurrentNodePos) > 20)
                {
                    continue;
                }

                if (item.item_Info.Id == 320294 || item.item_Info.Id == 320316)//金币
                {
                    item.AutoPickUp();
                    continue;
                  /*  if (this.IsCanAutoPickUpGoldCoin)
                    {
                        item.AutoPickUp();
                    }
                    else
                    {
                        continue;
                    }*/
                }
               

                if (OnHookSetInfoTools.IsPickUpSuitEquip)//拾取套装
                {
                    if (item.IsSuit || item.IsHaveExecllentEntry)
                    {
                        item.AutoPickUp();
                        continue;
                    }
                }
                if (OnHookSetInfoTools.IsPickUpGems)//拾取宝石 原石
                {
                    if (item.item_Info.Type == (int)E_ItemType.Gemstone || item.item_Info.Id / 10000 == (int)E_ItemType.FGemstone)
                    {
                        item.AutoPickUp();
                        continue;
                    }
                }
                if (OnHookSetInfoTools.IsPickUpMountMat)//拾取坐骑材料
                {
                    if (KnapsackItemsManager.MountMatDic.ContainsKey(item.item_Info.Id))
                    {
                        item.AutoPickUp();
                        continue;
                    }
                }
                if (OnHookSetInfoTools.IsPickUpYuMao)//拾取羽毛
                {
                    if (KnapsackItemsManager.YuMaoDic.ContainsKey(item.item_Info.Id))
                    {
                        item.AutoPickUp();
                        continue;
                    }
                }
                if (OnHookSetInfoTools.IsPickUpLuck)//拾取幸运装备
                {
                    if (item.IsHaveLuckyEntry)
                    {
                        item.AutoPickUp();
                        continue;
                    }
                }
                if (OnHookSetInfoTools.IsPickUpSkill)//拾取技能装备
                {
                    if ((item.dropData.Quality & 1 << 0) == 1 << 0)
                    {
                        item.AutoPickUp();
                        continue;
                    }
                }
                if (OnHookSetInfoTools.IsPickUpAllEquip)//拾取所有道具
                {
                    item.AutoPickUp();
                    continue;
                }
                //自定义拾取
                if (OnHookSetInfoTools.IsDiyPicUp)
                {
                    if (OnHookSetInfoTools.DIYPickList.Count != 0 && string.IsNullOrEmpty(item.item_Info.Name) == false)
                    {

                        for (int d = 0, count = OnHookSetInfoTools.DIYPickList.Count; d < count; d++)
                        {
                            if (item.item_Info.Name.Contains(OnHookSetInfoTools.DIYPickList[d]))
                            {

                                item.AutoPickUp();
                                break;
                            }
                        }
                        continue;
                    }
                }
            }
        }


        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();

            if (Instance == this)
            {
                Instance = null;
            }
           
            if (HookEffect != null)
                ResourcesComponent.Instance.DestoryGameObjectImmediate(HookEffect, HookEffectResName.StringToAB());
            IsOnHooking = false;

            mainComponent = null;

            OnHookSetInfoTools.ShowEffectAction =null;
            OnHookSetInfoTools.HideEffectAction = null;
        }

    }
}
