using UnityEngine;
using UnityEngine.UI;
using ETModel;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace ETHotfix
{
    [ObjectSystem]
    public class UISkillBtnEntityAwake : AwakeSystem<UISkillBtnEntity, int, Transform>
    {
        public override void Awake(UISkillBtnEntity self, int skillconfigId, Transform btn)
        {
            self.Awake(skillconfigId, btn);
        }
    }
    [ObjectSystem]
    public class UISkillBtnEntityAttackAwake : AwakeSystem<UISkillBtnEntity, Transform>
    {
        public override void Awake(UISkillBtnEntity self, Transform a)
        {
            self.Awake(a);
        }
    }
    [ObjectSystem]
    public class UISkillBtnEntityStart : StartSystem<UISkillBtnEntity>
    {
        public override void Start(UISkillBtnEntity self)
        {
            self.Start();
        }
    }

    [ObjectSystem]
    public class UISkillBtnEntityUpdate : UpdateSystem<UISkillBtnEntity>
    {
        public override void Update(UISkillBtnEntity self)
        {

            if (self.curAttackEntity != null && (self.controlComponent.isPressJoy) || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)//按下移动键 清空当前选择的攻击目标
            {
                self.curAttackEntity = null;
                /*  if (self.controlComponent.IsNavigation) 
                  {
                      self.controlComponent.StopNavigation();
                  }*/
            }

            if (!self.IsMasking)
            {
                return;
            }
           
            self.Timer -= Time.deltaTime;//剩余时间
            if (!self.IsAttack)
            {
                // self.mask.fillAmount = 1 - (self.curTime / self.CoolTime);
                self.mask.fillAmount -= Time.deltaTime / self.cdtime;//填充值减去当前帧的一部分
                                                                     //self.cdText.text = Math.Round(self.CoolTime - self.curTime, 1).ToString();
                                                                     // self.Timer -= Time.deltaTime;//剩余时间
                self.cdText.text = self.Timer.ToString("0.0");

            }

            if (self.Timer < 0.01f)
            {
                if (!self.IsAttack)
                {
                    self.cdText.text = string.Empty;
                    self.curTime = 0;
                    self.mask.fillAmount = 0;
                }

                self.IsMasking = false;
                self.btn.interactable = true;
                self.SpaceTime = 0;

            }

        }
    }
    /// <summary>
    /// 技能按钮实体
    /// </summary>
    public class UISkillBtnEntity : Entity
    {

        public Transform skillBtn;
        public Button btn;
        public int slotId;//卡槽ID

        public Image mask;// 技能遮罩
        public Text cdText;
        public bool IsMasking = false;//是否处于cd中
        public int SkillType;//技能类型 1主动技能   2 辅助技能

        public int skill_ConfigID;// 配置表ID
        public int SkillDamageRange =0;//获取该技能的伤害半径 单位 m （伤害半径 不为0 则表示可以原地释放技能）
        public int SkillAttackRange;//攻击范围 （（skill_Info.Distance）表示格子数 一个等于2米） 
        public float CoolTime;//cd时间(时间戳 秒)//+skill_Info.DamageWait/1000
        

        public float Timer = .5f;
        public float cdtime = 0;

        public float SpaceTime = 0f;//技能之间的间隔时间

        public float curTime = 0;

        public SkillInfos skill_Info;// 技能配置表信息

        public RoleEntity RoleEntity => UnitEntityComponent.Instance.LocalRole;//本地玩家
        public UnitEntity curAttackEntity = null;//当前攻击的实体
        public RoleSkillComponent skillComponent;//玩家技能组件
        public UnitEntityPathComponent pathComponent;//实体移动路径组件

      

        public RoleMoveControlComponent controlComponent;

        public AstarNode TargetAstarNode;//目标格子

        public bool IsAttack = false;//是否是普攻
        CreateRoleConfig_InfoConfig roleinfo;
        RoleEquipmentComponent equipmentComponent;

        public float nextUseTime=0;//下一次使用的时间

        bool isPress = false;//是否手动按下

        public float bufferNextUseTime = 0;
     
        public void Awake(int skillConfigId, Transform skillbtn)
        {
            skill_ConfigID = skillConfigId;
            skillBtn = skillbtn;
            skill_Info = new SkillInfos();
            skillConfigId.GetSkillInfos_RoleType_Ref(RoleEntity.RoleType, ref skill_Info);
            Image skillIconImage = skillBtn.GetComponent<Image>();
            Sprite iconSprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, skill_Info.Icon);
            if (skillIconImage != null)
            {
                skillIconImage.sprite = iconSprite;
                skillIconImage.overrideSprite = null;
                skillIconImage.material = null;
                skillIconImage.type = Image.Type.Simple;
                skillIconImage.enabled = true;
                skillIconImage.raycastTarget = true;
                skillIconImage.color = iconSprite != null ? Color.white : new Color(1f, 1f, 1f, 0.02f);
            }

            if (iconSprite == null)
            {
                string roleTypeName = RoleEntity != null ? RoleEntity.RoleType.ToString() : "null";
                Log.Debug($"SkillIcon missing role={roleTypeName} skillId={skill_ConfigID} icon={skill_Info.Icon} btn={skillbtn.name}");
            }
           
            IsMasking = false;
            this.btn = skillBtn.GetComponent<Button>();
            skillBtn.GetComponent<Button>().onClick.AddSingleListener(() => 
            {
                //  if (TimeHelper.Now() <= GlobalDataManager.AttackSpaceTime) return;
                isPress = true;
                OnSkillBtnClick();
                isPress = false;
            });
            //cd遮罩
            mask = skillbtn.Find("mask").GetComponent<Image>();
            cdText = skillbtn.Find("Text").GetComponent<Text>();
            cdText.text = string.Empty;

            IsAttack = false;
           
            SkillAttackRange = skill_Info.Distance;

            if (UIMainComponent.Instance != null)
            {
                if (UIMainComponent.Instance.DaShiSkillAtr.TryGetValue(skill_ConfigID, out int range))
                {
                    SkillAttackRange = range;
                }
            }

            SkillDamageRange = skill_Info.OtherData.GetValue(2);
            CoolTime = skill_Info.CoolTime / 1000;
            SkillType = skill_Info.skillType;
            nextUseTime =Time.time;
            Game.EventCenter.EventListenner(EventTypeId.LOCALROLE_DEAD, delegate { curAttackEntity = null; });
         
        }
        public void Awake(Transform attackbtn)
        {
            skill_ConfigID = 0;
            this.btn = attackbtn.GetComponent<Button>();
            attackbtn.GetComponent<Button>().onClick.AddSingleListener(delegate
            {
                //if (TimeHelper.Now() <= GlobalDataManager.AttackSpaceTime) return;
                if (SiegeWarfareData.CurroleId == RoleEntity.Id && SiegeWarfareData.SiegeWarfareIsStart)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "坐在宝座上无法攻击");
                    return;
                }
                skill_ConfigID = 0;
                IsAttack = true;
                OnSkillBtnClick();
            }
             );
            skill_Info = new SkillInfos { Name = "普攻" };
            IsAttack = true;
            roleinfo = ConfigComponent.Instance.GetItem<CreateRoleConfig_InfoConfig>((int)RoleEntity.RoleType);
            
            SkillAttackRange =GetNorAttackRange();
            CoolTime =0;

            nextUseTime = Time.time;
            // Game.EventManager.AddEventHandler(EventTypeId.LOCALROLE_DEAD, delegate { curAttackEntity = null; });
            Game.EventCenter.EventListenner(EventTypeId.LOCALROLE_DEAD, delegate { curAttackEntity = null; });

        }

        public void Start()
        {
            skillComponent = this.RoleEntity.GetComponent<RoleSkillComponent>();
            pathComponent = this.RoleEntity.GetComponent<UnitEntityPathComponent>();
            controlComponent = this.RoleEntity.GetComponent<RoleMoveControlComponent>();
            equipmentComponent = RoleEntity.GetComponent<RoleEquipmentComponent>();

        }

        /// <summary>
        /// 获取普攻距离
        /// </summary>
        /// <returns></returns>
        public int GetNorAttackRange()
        {
            int range=2;//单位 m
            switch (this.RoleEntity.RoleType)
            {
                case E_RoleType.Magician:
                case E_RoleType.Swordsman:
                case E_RoleType.Magicswordsman:
                case E_RoleType.Holymentor:
                case E_RoleType.Summoner:
                case E_RoleType.Gladiator:
                case E_RoleType.GrowLancer:
                case E_RoleType.Runemage:
                case E_RoleType.StrongWind:
                case E_RoleType.Gunners:
                case E_RoleType.WhiteMagician:
                case E_RoleType.WomanMagician:
                    range = roleinfo.AttackDistance-1;
                    break;
                case E_RoleType.Archer:
                    //弓箭手 装备了弓或者驽 攻击距离为 8格
                    equipmentComponent??=RoleEntity.GetComponent<RoleEquipmentComponent>();
                    if (equipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Weapon, out KnapsackDataItem knapsackData))
                    {
                       knapsackData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                        if (item_Info.Type == (int)E_ItemType.Bows || item_Info.Type == (int)E_ItemType.Crossbows)
                        {
                            range = roleinfo.AttackDistance+5;
                        }
                    }
                    else
                    {
                        //其他武器 则为 2格
                        range = roleinfo.AttackDistance-1;
                    }
                    break;

            }
            return range;

        }

        public void ChangeSkillAttackRange() 
        {
            //弓箭手 装备了弓或者驽 攻击距离为 8格
            if (equipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Weapon, out KnapsackDataItem knapsackData))
            {
                knapsackData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                if (item_Info.Type == (int)E_ItemType.Bows || item_Info.Type == (int)E_ItemType.Crossbows)
                {
                    SkillAttackRange = roleinfo.AttackDistance * 4-1;
                }
            }
            else
            {
                //其他武器 则为 2格
                SkillAttackRange = roleinfo.AttackDistance;
            }
        }
        /// <summary>
        /// 技能按钮按下事件
        /// </summary>
        public void OnSkillBtnClick()
        {
           
            if (RoleOnHookComponent.Instance.IsOnHooking&& skill_Info.skillType == 1) return; //挂机状态下 不能主动点击释放技能

            
            if (controlComponent.IsNavigation) return;//上一个技能正在寻路到目标实体

            if (UIMainComponent.Instance.IsCanUse == false) return;
            if (TimeHelper.Now() <= GlobalDataManager.AttackSpaceTime) return;

            if (!IsCanUser()) return;

            //攻击距离为 0 则为Buffer
            if (SkillAttackRange == 0 || (skill_Info != null && skill_Info.skillType == 2)|| skill_Info.Name.Contains("瞬间移动"))//2 辅助技能
            {
                if (skill_Info.Name.Contains("瞬间移动"))
                {

                    for (int i = skill_Info.Distance; i >= 0; i--)
                    {
                        var pos = RoleEntity.CurrentPos + RoleEntity.Game_Object.transform.parent.forward * (i * 2);
                        var node = AstarComponent.Instance.GetNodeVector(pos);
                        if (node.isWalkable == false)
                        {
                            continue;
                        }
                        else
                        {
                          
                            if (PositionHelper.Distance2D(RoleEntity.CurrentNodePos, node) > SkillAttackRange * 2) continue;
                          
                            TargetAstarNode = node;
                            break;
                        }
                    }
                }

                curAttackEntity = ClickSelectUnitEntityComponent.Instance.curSelectUnit;
              
                #region 小挪移 暂时注释掉
                /* if (skill_Info.Name.Contains("小挪移"))
                         {
                             if (curAttackEntity.EntityType == E_EntityType.Role)
                             {
                                 var info = TeamDatas.OtherTeamMemberStatusList.Find(r => r.GameUserId == curAttackEntity.Id);
                                 if (info == null)
                                 {
                                     UIComponent.Instance.VisibleUI(UIType.UIHint, "选择一名友军 才能释放该技能");
                                     return;
                                 }
                                 else
                                 {
                                     RequestAttackUnitEntity().Coroutine();
                                 }
                             }
                             else
                             {
                                 UIComponent.Instance.VisibleUI(UIType.UIHint, "该技能 只能对玩家释放");
                                 return;
                             }
                         }*/
                #endregion
                if (curAttackEntity != null && PositionHelper.Distance2D(curAttackEntity.Position, RoleEntity.Position) > SkillAttackRange)
                {
                    curAttackEntity = null;
                }
              
                
                RequestAttackUnit(curAttackEntity, ref UIMainComponent.Instance.IsCanUse,targetNode:TargetAstarNode);
                return;
            }

            UnitEntityComponent.Instance.GetMinDistacneEntity(ref curAttackEntity);

            
            // 判断实体是否在技能的攻击范围内

            if (curAttackEntity != null && PositionHelper.Distance(curAttackEntity.CurrentNodePos, this.RoleEntity.CurrentNodePos) > SkillAttackRange)
            {
                //攻击范围外 向怪物移动-》移动到 目标点后 释放技能
                pathComponent.NavTarget(curAttackEntity.CurrentNodePos);
                UIMainComponent.Instance.curSkillEntity = this;
            }
            else
            {
                RequestAttackUnit(curAttackEntity,ref UIMainComponent.Instance.IsCanUse);
            }

        }
        C2G_AttackRequest g_AttackRequest = new C2G_AttackRequest();
        G2C_AttackResponse g2C_Attack;
        public void RequestAttackUnit(UnitEntity unitEntity, ref bool IsUse,AstarNode targetNode= null)
        {
            
            IsUse = true;
            AttackUnitAsyn().Coroutine();

            
            async ETVoid AttackUnitAsyn() 
            {
                g_AttackRequest.GameUserId = unitEntity == null ? 0 : unitEntity.Id;//攻击 目标的 UUId
                g_AttackRequest.AttackType = skill_ConfigID;//技能类型 0 普攻 1 技能
                g_AttackRequest.Tick = TimeHelper.ClientNow();
                if (targetNode != null)
                {
                    //技能 需要填目标点
                    g_AttackRequest.PosX = targetNode.x;
                    g_AttackRequest.PosY = targetNode.z;
                }
                else
                {
                    g_AttackRequest.PosX = unitEntity == null ? RoleEntity.CurrentNodePos.x : unitEntity.CurrentNodePos.x;
                    g_AttackRequest.PosY = unitEntity == null ? RoleEntity.CurrentNodePos.z : unitEntity.CurrentNodePos.z;
                }
                // Log.DebugYellow($"请求攻击：{unitEntity?.Id}  {skill_ConfigID}->{skill_Info?.Name} {unitEntity?.GetType()}");
              //  Log.DebugRed($"请求移动：{targetNode == null} {targetNode.x}:{targetNode.z}   g_AttackRequest.PosX:{g_AttackRequest.PosX} {g_AttackRequest.PosY}");
                g2C_Attack = (G2C_AttackResponse)await SessionComponent.Instance.Session.Call(g_AttackRequest);
                if (!IsAttack)
                {
                    IsMasking = true;//技能按下
                    cdtime = CoolTime;
                    Timer = CoolTime;
                }
                if (g2C_Attack.Error != 0)
                {
                  //  Log.DebugBrown($"{g2C_Attack.Error}->{g2C_Attack.Error.GetTipInfo()} -> {unitEntity?.Id}");
                    curAttackEntity = null;
                
                    if (RoleOnHookComponent.Instance.IsOnHooking)
                    {
                        RoleOnHookComponent.Instance.ClearCurAttackEntity();
                        RoleOnHookComponent.Instance.IsAttack = false;
                    }
                    GlobalDataManager.AttackSpaceTime = g2C_Attack.Tick;

                    if (g2C_Attack.Error == 401 || g2C_Attack.Error == 404 || g2C_Attack.Error == 405 || g2C_Attack.Error == 411 || g2C_Attack.Error == 413 || g2C_Attack.Error == 412 || g2C_Attack.Error == 414) 
                    {
                       
                        return;
                    }

                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Attack.Error.GetTipInfo());
                }
                else
                {
                    if (ClickSelectUnitEntityComponent.Instance.curSelectUnit is RoleEntity && skill_Info.skillType == 2)
                    {
                        ClickSelectUnitEntityComponent.Instance.ClearSelectUnit();
                        //RoleEntity.GetComponent<AnimatorComponent>().SetBoolValue("SiegeSitDown", false);

                    }
                    if (RoleOnHookComponent.Instance.IsOnHooking)
                    {
                        
                        RoleOnHookComponent.Instance.IsAttack = false;
                    }
                }

              
            }
        }
        /// <summary>
        /// 该技能是否可以使用
        /// </summary>
        /// <returns></returns>
        public bool IsCanUser()
        {
            if (RoleEntity.IsDead || IsMasking)
            {

                return false;
            }

            //安全区不能使用
            if (this.RoleEntity.IsSafetyZone)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "安全区不能使用技能");
                return false;
            }

            if (RoleOnHookComponent.Instance.IsOnHooking && UIMainComponent.Instance.onhookNoUseSkillList.Contains(skill_Info.Name))
            {
                return false;
            }

            //判断蓝量、AG是否足够
            if (skill_Info.Consume.GetValue(1) > RoleEntity.Property.GetProperValue(E_GameProperty.PROP_MP))
            {

                if (UIMainComponent.Instance.medicineEntity_Mp.MedicineList.Count > 0)
                {
                    if (skill_Info.Consume.GetValue(1) > RoleEntity.Property.GetProperValue(E_GameProperty.PROP_MP_MAX))
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "魔法值不足 无法使用");
                    }

                    UIMainComponent.Instance.medicineEntity_Mp.UserMedicine();

                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "魔法值不足 无法使用");
                }
                return false;
            }
            if (skill_Info.Consume.GetValue(2) > RoleEntity.Property.GetProperValue(E_GameProperty.PROP_AG))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "AG不足 无法使用");
                return false;
            }
            if (skill_Info.UseStandard.GetValue(1) > RoleEntity.Property.GetProperValue(E_GameProperty.Level))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足 无法使用");
                return false;
            }
            if (skill_Info.UseStandard.GetValue(2) > RoleEntity.Property.GetProperValue(E_GameProperty.Property_Strength))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "力量不足 无法使用");
                return false;
            }

            if (skill_Info.UseStandard.GetValue(3) > RoleEntity.Property.GetProperValue(E_GameProperty.Property_Willpower))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "智力不足 无法使用");
                return false;
            }
            if (skill_Info.UseStandard.GetValue(4) > RoleEntity.Property.GetProperValue(E_GameProperty.Property_Agility))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "敏捷不足 无法使用");
                return false;
            }

            if (skill_Info.UseStandard.GetValue(6) > RoleEntity.Property.GetProperValue(E_GameProperty.Property_Command))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "统率不足 无法使用");
                return false;
            }
            if (skill_Info.UseStandard.GetValue(5) > RoleEntity.Property.GetProperValue(E_GameProperty.Property_BoneGas))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "体力不足 无法使用");
                return false;
            }


            if (skill_Info.skillType == 2 && isPress)
            {
                //手动释放 辅助技能
                return true;
            }
            
            if (RoleEntity.RoleType == E_RoleType.Archer)
            {
                if (equipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Weapon, out KnapsackDataItem knapsackData))
                {
                    equipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Shield,out KnapsackDataItem Arrow);
                    knapsackData.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                    if (item_Info.Type == (int)E_ItemType.Bows)
                    {

                        //装备了弓
                        if (Arrow == null || ((Arrow != null && Arrow.ItemType != (int)E_ItemType.Arrow) && Arrow.ConfigId != 40019 && Arrow.ConfigId != 60008 && Arrow.ConfigId != 60000))
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "请装备弓箭");
                            return false;
                        }
                      

                    }
                    else if (item_Info.Type == (int)E_ItemType.Crossbows)
                    {
                        //装备了驽
                        if (Arrow == null ||(Arrow != null && Arrow.ConfigId != 50012&& Arrow.ConfigId != 60008 && Arrow.ConfigId != 60000))
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "请装备弩箭");
                            return false;
                        }
                    }
                }

                if (RoleOnHookComponent.Instance.IsOnHooking&&skill_Info.Name.Contains("召唤"))
                {
                  
                    return RoleEntity.summonEntity == null;
                }

                if (skill_Info.Id == 202)//治疗（生命值低于50%才能使用）
                {
                    //队伍中的队友 生命值低于50%使用治疗术
                    if (TeamDatas.MyTeamState != null)
                    {
                        for (int i = 0, length = TeamDatas.OtherTeamMemberStatusList.Count; i < length; i++)
                        {
                            TeamMateProperty teamMateProperty = TeamDatas.OtherTeamMemberStatusList[i].Status;
                            if ((float)teamMateProperty.HP / teamMateProperty.HPMax <= 0.5)
                            {
                                return true;
                            }
                        }
                    }

                    if (Time.time > bufferNextUseTime)
                    {
                        if (OnHookSetInfoTools.IsAutoBufferCd_10)
                        {
                            bufferNextUseTime = Time.time + 10;
                        }
                        else if (OnHookSetInfoTools.IsAutoBufferCd_20)
                        {
                            bufferNextUseTime = Time.time + 20;
                        }
                        else
                        {
                            bufferNextUseTime = Time.time + 30;
                        }
                        return true;
                    }
                    else
                    {
                        if (UIMainComponent.Instance.Hp.fillAmount < .5)
                        {
                            return true;
                        }
                        return false;
                    }
                  
                    
                }
                else if (skill_Info.Id == 217)//SD（SD值低于50%才能使用）
                {
                    if (UIMainComponent.Instance.sd.fillAmount > .5)
                    {
                        return false;
                    }
                    else
                    {
                        return true; 
                    }
                }

            }
            if (skill_Info.Id == 217)//防护值恢复
            {
              
                if ((float)RoleEntity.Property.GetProperValue(E_GameProperty.PROP_SD_MAX) / RoleEntity.Property.GetProperValue(E_GameProperty.PROP_SD) < .5f)
                {
                    return true;
                }
            }

            if (skill_Info.skillType == 2)//Buffer技能
            {

                if (RoleEntity.RoleType == E_RoleType.Archer)
                {

                    if (Time.time > bufferNextUseTime)
                    {
                        if (OnHookSetInfoTools.IsAutoBufferCd_10)
                        {
                            bufferNextUseTime = Time.time + 10;
                        }
                        else if (OnHookSetInfoTools.IsAutoBufferCd_20)
                        {
                            bufferNextUseTime = Time.time + 20;
                        }
                        else
                        {
                            bufferNextUseTime = Time.time + 30;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {

                    return Time.time >= nextUseTime;
                }
           
            }

            return true;
        }

       

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            IsMasking = false;

            if (cdText != null)
                cdText.text = string.Empty;
            curTime = 0;
            IsMasking = false;
            if (mask != null)
                mask.fillAmount = 0;
            btn.interactable = true;

            curAttackEntity = null;
            skillComponent = null;


            if (skillBtn != null)
            {
                skillBtn.GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, "+");
                skillBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            }
            Game.EventCenter.RemoveEvent(EventTypeId.LOCALROLE_DEAD, delegate { curAttackEntity = null; });
         
        }

    }
}
