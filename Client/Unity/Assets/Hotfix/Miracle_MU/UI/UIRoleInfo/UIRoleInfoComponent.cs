using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public static class UIRoleInfoInfo
    {

        public static int ZhuanShengCount = 0;
    }
    [ObjectSystem]
    public class UIRoleInfoComponentAwake : AwakeSystem<UIRoleInfoComponent>
    {
        public override void Awake(UIRoleInfoComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIRoleInfo));
            self.InitRoleInfos();
            self.Init_RoleAtr();

            self.GetRoleProperties().Coroutine();
            self.GetPlayerSecondaryAttribute().Coroutine();
            self.Init_AddPoint();
            self.Init_CountDown();
            self.SelectTitle();

            self.SetBeginnerGuide();

            self.InittransBtn();

            //self.SetArriteRedDot();

            self.InitRecommendAddPoint();
            self.RecommendAddPoint();

            //UIRoleInfoData.GetAttribute(recommendAdd, out roleAttribute);

        }
    }
    [ObjectSystem]
    public class UIRoleInfoComponentStart : StartSystem<UIRoleInfoComponent>
    {
        public override void Start(UIRoleInfoComponent self)
        {
            //self.ShowRoleMode();
            self.RegisterDragEvent();
        }
    }
   // [ObjectSystem]
    //public class UIRoleInfoComponentUpdate : UpdateSystem<UIRoleInfoComponent>
    //{
    //    public override void Update(UIRoleInfoComponent self)
    //    {
    //        //if (self.roleEntity_Obj != null && self.isDrag && self.lastRatePos.x != Input.mousePosition.x)
    //        //{
    //        //    float offPos_x = self.lastRatePos.x - Input.mousePosition.x;
    //        //    self.roleEntity_Obj.transform.Rotate(0, offPos_x, 0);
    //        //    self.lastRatePos = Input.mousePosition;
    //        //}
            

    //    }
    //}

    /// <summary>
    /// 本地玩家属性 组件
    /// </summary>
    public partial class UIRoleInfoComponent : Component
    {
        public ReferenceCollector collector;
        public bool isDrag = false;//是否开始旋转角色
        public Vector3 lastRatePos;//上一次旋转的为位置
        //public GameObject roleEntity_Obj;
        public RoleEntity roleEntity = UnitEntityComponent.Instance.LocalRole;

        public Text roleName, roleLev, rolePost, roleLevPoints, ZhuanShengCountTxt;

        public Transform Infos;

        public GameObject Content;

        public Dictionary<E_RoleType, GameObject> RoleInfoDic = new Dictionary<E_RoleType, GameObject>();//玩家属性面板字典
        public List<GameObject> items = new List<GameObject>();

        //public GameObject UIBeginnerGuideJianshi, UIBeginnerGuideGongJianShou, UIBeginnerGuideMoFaShi;

        public Button recommendBtn;

        public long AddPointValue = 0;
        #region 变身戒指开启/关闭
        public Button transBtn, fashionBtn;
        KnapsackDataItem item = null;
        private string PlayerPrefstr = "hidefashion";
        public void InittransBtn()
        {
            PlayerPrefstr = $"{PlayerPrefstr}_{roleEntity.Id}";
            transBtn = collector.GetButton("TransBtn");
            transBtn.GetComponentInChildren<Text>().text = PlayerPrefs.GetInt(PlayerPrefstr) == 0 ? "关闭时装" : "显示时装";
            //是否有变身戒指
            var equip = roleEntity.GetComponent<RoleEquipmentComponent>();

            transBtn.gameObject.SetActive(false);

            //for (E_Grid_Type i = E_Grid_Type.LeftRing; i <= E_Grid_Type.RightRing; i++)
            //{
            //    if (equip.curWareEquipsData_Dic.TryGetValue(i, out item))
            //    {
            //        //Log.Info(JsonHelper.ToJson(item));

            //        if (ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)item.ConfigId) != null)
            //        {
            //            if (ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)item.ConfigId).IsTransRing == 1)
            //            {
            //                transBtn.gameObject.SetActive(true);
            //                break;
            //            }
            //        }
            //    }
            //}
            //transBtn.onClick.AddSingleListener(() =>
            //{
            //    if (transBtn.GetComponentInChildren<Text>().text == "关闭时装")
            //    {

            //        equip.HideFashion(item.Slot);
            //        transBtn.GetComponentInChildren<Text>().text = "显示时装";
            //        PlayerPrefs.SetInt(PlayerPrefstr, 1);

            //    }
            //    else if (transBtn.GetComponentInChildren<Text>().text == "显示时装")
            //    {
            //        transBtn.GetComponentInChildren<Text>().text = "关闭时装";
            //        PlayerPrefs.SetInt(PlayerPrefstr, 0);

            //        equip.ShowFashion_Ring(item.ConfigId, item.GetProperValue(E_ItemValue.Level), (E_Grid_Type)item.Slot);
            //    }

            //    //刷新时装
            //    GameObject.Destroy(roleEntity_Obj);
            //    ShowRoleMode();

            //});

            //时装
            fashionBtn = collector.GetButton("FashionBtn");
            fashionBtn.onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIRoleInfo);
                //  UIComponent.Instance.VisibleUI(UIType.UIFashion);
            });
        }
        #endregion

        /// <summary>
        /// 引导
        /// </summary>
        public void SetBeginnerGuide()
        {
            //UIBeginnerGuideJianshi = collector.GetImage("UIBeginnerGuideJianshi").gameObject;
            //UIBeginnerGuideGongJianShou = collector.GetImage("UIBeginnerGuideGongJianShou").gameObject;
            //UIBeginnerGuideMoFaShi = collector.GetImage("UIBeginnerGuideMoFaShi").gameObject;


        }

        public void RecommendAddPoint()
        {
            recommendBtn = collector.GetButton("recommendBtn");
            recommendBtn.onClick.AddSingleListener(() =>
            {
                if (AddPointValue == 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "没有可用的属性点");
                    return;
                }
                ShowRecommendAddPoint();
                TitleImage.gameObject.SetActive(false);
            });
            //recommendBtn.gameObject.SetActive(roleEntity.RoleType == E_RoleType.Archer || roleEntity.RoleType == E_RoleType.Swordsman || roleEntity.RoleType == E_RoleType.Magician);
            //recommendBtn.gameObject.SetActive(roleEntity.RoleType == E_RoleType.Archer || roleEntity.RoleType == E_RoleType.Swordsman || roleEntity.RoleType == E_RoleType.Magician);
        }


        public void SetArriteRedDot()
        {
            RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Attribute, 0);
            UIMainComponent.Instance.RedDotFriendCheack();
        }


        public void InitRoleInfos()
        {
            for (int i = (int)E_RoleType.Magician; i <= (int)E_RoleType.GrowLancer; i++)
            {
                var typename = (E_RoleType)i;
                GameObject infoobj = collector.GetGameObject(typename.ToString());
                infoobj.SetActive(false);
                RoleInfoDic[typename] = infoobj;
            }
        }

        public GameObject GetRoleInfoPanel()
        {
            GameObject infopanel = null;
            for (int i = (int)E_RoleType.Magician; i <= (int)E_RoleType.GrowLancer; i++)
            {
                var typename = (E_RoleType)i;
                if (RoleInfoDic.TryGetValue(typename, out GameObject gameObject))
                {
                    if (typename == this.roleEntity.RoleType)
                    {
                        infopanel = gameObject;
                        infopanel.SetActive(true);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }


            }
            return infopanel;
        }

        /// <summary>
        /// 显示角色模型
        /// </summary>
        public void ShowRoleMode()
        {
            //roleEntity_Obj = GameObject.Instantiate<GameObject>(UnitEntityComponent.Instance.LocalRole.Game_Object);

            //RoleEquipmentComponent roleEquipmentComponent = roleEntity.GetComponent<RoleEquipmentComponent>();

            //for (int i = 0; i < roleEntity_Obj.transform.childCount; i++)
            //{
            //    var item = roleEntity_Obj.transform.GetChild(i);
            //    if (item.name.Contains("Suit"))
            //    {
            //        foreach (var itemEquip in roleEquipmentComponent.curWareEquipsData_Dic)
            //        {

            //            var iteminfo = itemEquip.Value;
            //            iteminfo.ConfigId.GetItemInfo_Out(out iteminfo.item_Info);
            //            if (iteminfo.item_Info.ResName == item.name)
            //            {
            //                item.gameObject.SetWorld(itemEquip.Value.GetProperValue(E_ItemValue.Level));
            //                break;
            //            }
            //        }
            //    }

            //}

            //roleEntity_Obj.transform.SetParent(UnitEntityComponent.Instance.LocalRole.roleTrs.parent);
            //roleEntity_Obj.transform.localScale = Vector3.one;
            //roleEntity_Obj.transform.eulerAngles = Vector3.up * 180;
            //Vector3 pos = collector.GetGameObject("modePoint").transform.position;
            //pos.z = 95;
            //roleEntity_Obj.transform.localPosition = pos;
            //roleEntity_Obj.SetLayer(LayerNames.UI);

            //ResetMesh().Coroutine();
            //SetWeapon();

            //void SetWeapon()
            //{
            //    //将武器 放到后背
            //    ReferenceCollector collector = roleEntity_Obj.GetReferenceCollector();
            //    RoleEquipmentComponent roleEquipment = UnitEntityComponent.Instance.LocalRole.GetComponent<RoleEquipmentComponent>();
            //    if (roleEquipment.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Weapon, out KnapsackDataItem dataItem))
            //    {
            //        if (roleEntity.IsSafetyZone || roleEntity.RoleType == E_RoleType.Gladiator) return;
            //        Transform weaponR = collector.GetGameObject("R_Weapon").transform;
            //        if (weaponR.childCount == 0) return;
            //        Transform weapon = weaponR.GetChild(weaponR.childCount - 1);
            //        weapon.gameObject.SetBack(dataItem.GetProperValue(E_ItemValue.Level));

            //        weapon.gameObject.SetLayer(LayerNames.UI);

            //        weapon.SetParent(collector.GetGameObject("RightBackpos").transform, false);
            //        weapon.transform.localPosition = Vector3.zero;
            //        weapon.transform.localRotation = Quaternion.identity;
            //        weapon.transform.localScale = Vector3.one;
            //    }
            //    if (roleEquipment.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Shield, out KnapsackDataItem dataItem_Shild))
            //    {
            //        if (roleEntity.IsSafetyZone || roleEntity.RoleType == E_RoleType.Gladiator) return;
            //        Transform weaponL = collector.GetGameObject("L_Weapon").transform;
            //        if (weaponL.childCount == 0) return;
            //        Transform shild = weaponL.GetChild(weaponL.childCount - 1);
            //        dataItem_Shild.Id.GetItemInfo_Out(out Item_infoConfig item_Info);
            //        bool isleftback = item_Info.TwoHand == 0&&dataItem_Shild.ItemType!=(int)E_ItemType.Shields&& dataItem_Shild.ItemType != (int)E_ItemType.Arrow;
            //        shild.gameObject.SetBack(dataItem_Shild.GetProperValue(E_ItemValue.Level), isleftback);

            //        shild.gameObject.SetLayer(LayerNames.UI);

            //        shild.SetParent(collector.GetGameObject("LeftBackPos").transform, false);
            //        shild.transform.localPosition = Vector3.zero;
            //        shild.transform.localRotation = Quaternion.identity;
            //        shild.transform.localScale = Vector3.one;
            //    }
            //    if (roleEquipment.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Wing, out KnapsackDataItem dataItem_Wing))
            //    {

            //    }

            //}
        }

        public async ETTask ResetMesh()
        {
            //await TimerComponent.Instance.WaitAsync(1);
            //if (roleEntity_Obj != null)
            //{
            //    for(int i = 0; i < roleEntity_Obj.transform.childCount; i++)
            //    {
            //        roleEntity_Obj.transform.GetChild(i).gameObject.SetActive(false);
            //    }
            //}
            //await TimerComponent.Instance.WaitAsync(1);
            //if (roleEntity_Obj != null)
            //{
            //    for (int i = 0; i < roleEntity_Obj.transform.childCount; i++)
            //    {
            //        roleEntity_Obj.transform.GetChild(i).gameObject.SetActive(true);
            //    }
            //}
            //roleEntity_Obj.transform.Find("TopPoint").gameObject.SetActive(false);
        }

        public void Init_RoleAtr()
        {
            roleName = collector.GetText("roleName");
            rolePost = collector.GetText("rolePost");
            roleLev = collector.GetText("roleLev");
            roleLevPoints = collector.GetText("rolePoints");
            ZhuanShengCountTxt = collector.GetText("ZhuanShengCount");
            Content = collector.GetGameObject("Content");

        }
        /// <summary>
        /// 刷新玩家的属性
        /// </summary>
        public void RefreshRoleProperty()
        {
            roleName.text = $"<color={this.roleEntity.GetRedNameColor()}>" + roleEntity.RoleName + "</color>";//玩家昵称
            rolePost.text = roleEntity.RoleType.GetRoleName(roleEntity.ClassLev);//角色职位
            roleLev.text = roleEntity.Level.ToString() + "级";//等级
            roleLevPoints.text = roleEntity.Property.GetProperValue(E_GameProperty.FreePoint).ToString();//等级点数
            AddPointValue = roleEntity.Property.GetProperValue(E_GameProperty.FreePoint);
            recommendBtn.transform.Find("Light").gameObject.SetActive(AddPointValue > 0);
            recommendBtn.transform.Find("AddPointRedDot").gameObject.SetActive(AddPointValue > 0);
            var infoObj = GetRoleInfoPanel();

            for (int i = 0, length = infoObj.transform.childCount; i < length; i++)
            {
                var item = infoObj.transform.GetChild(i);
                if (item.name.Contains("_Add"))
                {
                    E_GameProperty property = (E_GameProperty)int.Parse(item.name.Split('_')[1]);
                    SetChild(item, roleEntity.Property.GetProperValue(property).ToString());
                }
                else
                {
                    int propertyId = int.Parse(item.name.Split('_')[2]);
                    Text textvalue = item.Find("value").GetComponent<Text>();
                    switch (propertyId)
                    {
                        case 22://攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.MinAtteck)} ~ {roleEntity.Property.GetProperValue(E_GameProperty.MaxAtteck)}";//攻击力
                            break;
                        case 27://攻击成功率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.AtteckSuccessRate)}";//攻击成功率
                            break;
                        case 28://PVP攻击率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.PVPAtteckSuccessRate)}";//PK攻击成功率
                            break;
                        case 35://防御力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.Defense)}";//防御力）
                            break;
                        case 37://攻击速率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.AttackSpeed)}/{GetMaxAttackSpeed()}";//攻击速率
                            break;
                        case 31://防御率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.DefenseRate)}";//防御率
                            break;
                        case 33://PVP防御率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.PVPDefenseRate)}";//PVP防御率
                            break;
                        case 42://技能攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.SkillAddition)}%";//技能攻击力%
                            break;
                        case 100://诅咒力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.MinDamnationAtteck)}~{roleEntity.Property.GetProperValue(E_GameProperty.MaxDamnationAtteck)}";//诅咒力
                            break;
                        case 110://格斗家近战攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.AdvanceAttackPower)}";//格斗家近战攻击力
                            break;
                        case 111://格斗家神兽攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.SacredBeast)}";//格斗家神兽攻击力
                            break;
                        case 112://范围攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.RangeAttack)}";//范围攻击力
                            break;
                        case 113://梦幻骑士惩处攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.DreamRiderPenalize)}";//梦幻骑士惩处攻击力
                            break;
                        case 114://梦幻骑士激怒攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.DreamRiderIrritate)}";//梦幻骑士激怒攻击力
                            break;
                        case 24://魔力

                            if (roleEntity.Property.GetProperValue(E_GameProperty.MagicRate_Increase) is long value && value != 0)
                            {
                                textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.MinMagicAtteck)}~{roleEntity.Property.GetProperValue(E_GameProperty.MaxMagicAtteck)}(+{roleEntity.Property.GetProperValue(E_GameProperty.MaxMagicAtteck) * value / 100})";//魔力%
                            }
                            else
                            {
                                textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.MinMagicAtteck)}~{roleEntity.Property.GetProperValue(E_GameProperty.MaxMagicAtteck)}";//魔力%
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            int GetMaxAttackSpeed()
            {
                switch (this.roleEntity.RoleType)
                {
                    case E_RoleType.Magician:
                        return 284;
                    case E_RoleType.Swordsman:
                        return 288;
                    case E_RoleType.Archer:
                        return 275;
                    case E_RoleType.Magicswordsman:
                        return 351;
                    case E_RoleType.Holymentor:
                        return 450;
                    case E_RoleType.Summoner:
                        return 188;
                    case E_RoleType.Gladiator:
                        return 441;
                    case E_RoleType.GrowLancer:
                        return 273;
                    case E_RoleType.Runemage:
                        return 0;
                    case E_RoleType.StrongWind:
                        return 0;
                    case E_RoleType.Gunners:
                        return 0;
                    case E_RoleType.WhiteMagician:
                        return 0;
                    case E_RoleType.WomanMagician:
                        return 0;
                    default:
                        return 0;
                }

            }

        }
        public void SetChild(Transform trs, string value)
        {
            trs.Find("value").GetComponent<Text>().text = value;
            trs.Find("add").GetComponent<Button>().onClick.AddSingleListener(() => { AddPoint(trs).Coroutine(); });
            trs.Find("addDiy").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                if (this.roleEntity.Property.GetProperValue(E_GameProperty.FreePoint) <= 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "剩余属性点数为0");
                    return;
                }
                ShowAddPointPanel(trs);
            });
            if (UIRoleInfoData.RecommendkeyValues.TryGetValue(trs.name.Split('_')[0], out int number))
            {
                if (trs.Find("add/RedDot") != null)
                    trs.Find("add/RedDot").gameObject.SetActive(number > 0 && this.roleEntity.Property.GetProperValue(E_GameProperty.FreePoint) > 0);
            }
        }

        /// <summary>
        /// 根据属性名字 获取对应的Id
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public int GetPropertyId(Transform ts) => ts.name.Split('_')[0] switch
        {
            "Strength" => 1,//力量
            "Intell" => 2,//智力
            "PhyStrength" => 4,//体力
            "Agile" => 3,//敏捷
            "Command" => 5,//统率
            _ => 0
        };
        /// <summary>
        /// 根据属性名字 获取对应的Id
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public string GetPropertyName(Transform ts) => ts.name.Split('_')[0] switch
        {
            "Strength" => "力量",//力量
            "Intell" => "智力",//智力
            "PhyStrength" => "体力",//体力
            "Agile" => "敏捷",//敏捷
            "Command" => "统率",//统率
            _ => string.Empty
        };

        /// <summary>
        /// 旋转角色 
        /// </summary>

        public void RegisterDragEvent()
        {
            GameObject drag = collector.GetImage("DragRoleEvent").gameObject;
            UGUITriggerProxy proxy = drag.GetComponent<UGUITriggerProxy>();
            proxy.OnBeginDragEvent = () =>
            {
                isDrag = true;
                lastRatePos = Input.mousePosition;
            };
            proxy.OnEndDragEvent += () =>
            {
                isDrag = false;
                lastRatePos = Vector3.zero;
            };
        }

        /// <summary>
        /// 获取角色 的属性
        /// </summary>
        /// <returns></returns>
        public async ETVoid GetRoleProperties()
        {
            G2C_PlayerPropertyResponse c2G_Player = (G2C_PlayerPropertyResponse)await SessionComponent.Instance.Session.Call(new C2G_PlayerPropertyRequest { SelectId = 0 });
            if (c2G_Player.Error != 0)
            {
                Log.DebugRed($"获取玩家属性报错：{c2G_Player.Error.GetTipInfo()}");
            }
            else
            {
                // UIRoleInfoInfo.ZhuanShengCount = c2G_Player.Reincarnate;
                //  ZhuanShengCountTxt.text = $"转生次数;{UIRoleInfoInfo.ZhuanShengCount}次";
                foreach (G2C_BattleKVData item in c2G_Player.Info)
                {
                    // Log.DebugBrown($"玩家属性：{item.Key} -> {item.Value}");
                    roleEntity.Property.Set(item);
                    if ((E_GameProperty)item.Key == E_GameProperty.FreePoint)
                    {
                        // Log.DebugGreen($"等级点数：{item.Value}");
                        roleEntity.Property.ChangeProperValue(E_GameProperty.FreePoint, item.Value);
                    }
                }
                //刷新玩家属性
                RefreshRoleProperty();
            }
        }

        public async ETTask GetPlayerSecondaryAttribute()
        {
            G2C_GetPlayerSecondaryAttribute c2G_Player = (G2C_GetPlayerSecondaryAttribute)await SessionComponent.Instance.Session.Call(new C2G_GetPlayerSecondaryAttribute { });
            if (c2G_Player.Error != 0)
            {
                Log.DebugRed($"获取玩家二级属性报错：{c2G_Player.Error.GetTipInfo()}");
            }
            else
            {

                for (int i = items.Count - 1; i >= 0; i--)
                {
                    GameObject.Destroy(items[i]);
                }
                GameObject fab = Content.transform.GetChild(0).gameObject;
                fab.SetActive(false);

                List<E_GameProperty> ary = new List<E_GameProperty>()
                {
                    E_GameProperty.LucklyAttackRate,
                    E_GameProperty.ExcellentAttackRate,
                    E_GameProperty.InjuryValueRate_2,
                    E_GameProperty.InjuryValueRate_3,
                    E_GameProperty.AttackIgnoreDefenseRate,
                    E_GameProperty.AttackIgnoreAbsorbRate,
                    E_GameProperty.IgnoreAbsorbRate,
                    E_GameProperty.ReboundRate,
                    E_GameProperty.LucklyAttackHurtValueIncrease,
                    E_GameProperty.ExcellentAttackHurtValueIncrease,
                    E_GameProperty.InjuryValueRate_Increase,
                    E_GameProperty.InjuryValueRate_Reduce,
                    E_GameProperty.InjuryValue_Reduce,
                    E_GameProperty.BackInjuryRate,
                    E_GameProperty.HurtValueAbsorbRate,
                    E_GameProperty.DefenseRate,
                    E_GameProperty.PVPDefenseRate,
                    E_GameProperty.ReplyHp,
                    E_GameProperty.KillEnemyReplyHpRate,
                    E_GameProperty.ReplyAllHpRate,
                    E_GameProperty.ReplyMp,
                    E_GameProperty.KillEnemyReplyMpRate,
                    E_GameProperty.ReplyAllMpRate,
                    E_GameProperty.MpConsumeRate_Reduce,
                    E_GameProperty.ReplyAG,
                    E_GameProperty.AgConsumeRate_Reduce,
                    E_GameProperty.ReplySD,
                    E_GameProperty.KillEnemyReplySDRate,
                    E_GameProperty.ReplyAllSdRate,
                    E_GameProperty.HitSdRate,
                    E_GameProperty.AttackSdRate,
                    E_GameProperty.SDAttackIgnoreRate,
                    E_GameProperty.ShacklesRate,
                    E_GameProperty.ShacklesResistanceRate,
                    E_GameProperty.ReallyDefense,
                    E_GameProperty.DefenseShieldRate,
                    E_GameProperty.AddGoldCoinRate_Increase,
                    E_GameProperty.DamageAbsPct_Guard,
                    E_GameProperty.DamageAbsPct_Wing,
                    E_GameProperty.DamageAbsPct_Mounts,
                    E_GameProperty.DamageAbsPct_Pets,
                    E_GameProperty.DisregardHarmReductionPct
                };
                List<G2C_BattleKVData> battleinfos = c2G_Player.Info.ToList();
            //    Log.DebugBrown("数据" + c2G_Player.Info.ToList().Count + ":::" + ary.Count);
            //    Log.DebugBrown("打印"+JsonHelper.ToJson(c2G_Player.Info));
                for (int i = 0; i < ary.Count; i++)
                {
                    string str = "";
                    string dd = "%";
                    float b = 0f;
                    switch (ary[i])
                    {

                        case E_GameProperty.DamageAbsPct_Wing:
                            {
                                str = "翅膀伤害吸收";
                                b = 1;
                                break;
                            }
                        case E_GameProperty.DamageAbsPct_Guard:
                            {
                                str = "守护伤害吸收";
                                b = 1;
                                break;
                            }
                        case E_GameProperty.DamageAbsPct_Mounts:
                            {
                                str = "坐骑伤害吸收";
                                b = 1;
                                break;
                            }
                        case E_GameProperty.DamageAbsPct_Pets:
                            {
                                str = "宠物伤害吸收";
                                b = 1;
                                break;
                            }

                        case E_GameProperty.InjuryValueRate_2:
                            {
                                str = "双倍伤害几率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.InjuryValueRate_3:
                            {
                                str = "三倍伤害几率";
                                b = 1;
                                break;
                            }
                        case E_GameProperty.LucklyAttackRate:
                            {
                                str = "幸运一击伤害几率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.ExcellentAttackRate:
                            {
                                str = "卓越一击伤害几率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.AttackIgnoreDefenseRate:
                            {
                                str = "无视防御几率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.AttackIgnoreAbsorbRate:
                            {
                                str = "攻击时无视吸收几率";
                                b = 1;
                                break;
                            }
                        case E_GameProperty.IgnoreAbsorbRate:
                            {
                                str = "无视伤害吸收";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.DefenseRate:
                            {
                                str = "防御率";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.PVPDefenseRate:
                            {
                                str = "PVP防御率";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.ReboundRate:
                            {
                                str = "反弹几率";
                                b = 1;
                                break;
                            }
                        case E_GameProperty.LucklyAttackHurtValueIncrease:
                            {
                                str = "幸运一击伤害增加量";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.ExcellentAttackHurtValueIncrease:
                            {
                                str = "卓越一击伤害增加量";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.InjuryValueRate_Increase:
                            {
                                str = "伤害提高率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.InjuryValueRate_Reduce:
                            {
                                str = "伤害减少率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.InjuryValue_Reduce:
                            {
                                str = "伤害减少量";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.BackInjuryRate:
                            {
                                str = "伤害反射率";
                                b = 1;
                                break;
                            }

                        case E_GameProperty.HurtValueAbsorbRate:
                            {
                                str = "伤害吸收率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.ReallyDefense:
                            {
                                str = "真实防御";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.HitSdRate:
                            {
                                str = "被击时Sd比率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.AttackSdRate:
                            {
                                str = "攻击时Sd比率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.SDAttackIgnoreRate:
                            {
                                str = "SD无视攻击率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.ShacklesRate:
                            {
                                str = "束缚几率";
                                b = 100;
                                break;
                            }
                        case E_GameProperty.ShacklesResistanceRate:
                            {
                                str = "束缚抵抗几率";
                                b = 100;
                                break;
                            }
                        case E_GameProperty.ShieldHurtAbsorb:
                            {
                                str = "盾牌伤害吸收量";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.DefenseShieldRate:
                            {
                                str = "防盾几率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.AddGoldCoinRate_Increase:
                            {
                                str = "获得金币增加率";
                                b = 0.1f;
                                break;
                            }
                        case E_GameProperty.MagicRate_Increase:
                            {
                                str = "魔杖魔力提升率";
                                b = 100;
                                break;
                            }
                        case E_GameProperty.GridBlockRate:
                            {
                                b = 10000;
                                str = "格挡几率";
                                break;
                            }
                        case E_GameProperty.GuardShieldRate:
                            {
                                str = "守护盾几率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.ReplyHpRate:
                            {
                                str = "生命恢复几率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.ReplyMpRate:
                            {
                                str = "魔力恢复几率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.ReplyAGRate:
                            {
                                str = "AG恢复几率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.ReplySDRate:
                            {
                                str = "SD恢复几率";
                                b = 10000;
                                break;
                            }
                        case E_GameProperty.ReplyHp:
                            {
                                str = "生命自动恢复量";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.ReplyMp:
                            {
                                str = "魔力自动恢复量";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.ReplyAG:
                            {
                                str = "AG自动恢复量";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.ReplySD:
                            {
                                str = "SD自动恢复量";
                                dd = "";
                                break;
                            }
                        case E_GameProperty.MpConsumeRate_Reduce:
                            {
                                str = "技能蓝耗减少率";
                                b = 1;
                                break;
                            }
                        case E_GameProperty.AgConsumeRate_Reduce:
                            {
                                str = "AG消耗减少率";
                                b = 1;
                                break;
                            }
                        case E_GameProperty.KillEnemyReplyHpRate:
                            {
                                str = "击杀怪物生命恢复率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.KillEnemyReplyMpRate:
                            {
                                str = "击杀怪物魔力恢复率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.KillEnemyReplySDRate:
                            {
                                str = "击杀怪物SD恢复率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.ReplyAllHpRate:
                            {
                                str = "生命完全恢复几率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.ReplyAllMpRate:
                            {
                                str = "魔力完全恢复几率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.ReplyAllSdRate:
                            {
                                str = "SD完全恢复几率";
                                b = 0.01f;
                                break;
                            }
                        case E_GameProperty.DisregardHarmReductionPct:
                            {
                                str = "削弱对方减伤率";
                                b = 0.01f;
                                break;
                            }
                    }
                    if (str != "")
                    {
                        GameObject obj = GameObject.Instantiate(fab, Content.transform);
                        obj.SetActive(true);
                        Text desTxt = obj.transform.Find("DesTxt").GetComponent<Text>();
                        Text numTxt = obj.transform.Find("NumTxt").GetComponent<Text>();

                        //for (int j = 0; j < battleinfos.Count; j++)
                        //{
                        //    for (int k = 0; k < ary.Count; k++)
                        //    {
                        //        if (battleinfos[j].Key == (int)ary[k])
                        //        {
                        //            var info = battleinfos[j];
                        //          //  Log.DebugBrown("打印" + info.Key + "vale" + info.Value);
                        //            float num = 0;
                        //            if (info.Value !=0)
                        //            {
                        //                if (dd == "%")
                        //                {
                        //                    if (info.Value != 0)
                        //                    {
                        //                        num = (float)(info.Value * b);
                        //                        numTxt.text = num.ToString("F2") + dd;
                        //                        Log.DebugBrown("数值" + info.Value);
                        //                    }
                        //                    else
                        //                    {

                        //                        numTxt.text = 0 + dd;
                        //                    }

                        //                }
                        //                else
                        //                {
                        //                    if (info.Value != 0)
                        //                    {
                        //                        num = (float)(info.Value);
                        //                        numTxt.text = num + dd;
                        //                    }
                        //                    else
                        //                    {
                        //                        numTxt.text = 0 + dd;
                        //                    }

                        //                }

                        //            }
                        //            else
                        //            {
                        //                numTxt.text = "0" + dd;
                        //            }
                        //            desTxt.text = str;
                        //            break;
                        //        }
                        //    }
                        //}



                        var info = battleinfos.Find(p => p.Key == (int)ary[i]);
                        float num = 0;
                        if (info != null)
                        {
                            if (dd == "%")
                            {
                              //    Log.DebugBrown("百分比" + info.Value + "::" + str);
                                if (info.Value != 0)
                                {
                                    num = (float)(info.Value * b);
                                    numTxt.text = num.ToString("F2") + dd;
                                }
                                else
                                {

                                    numTxt.text = 0 + dd;
                                }

                            }
                            else
                            {
                              //     Log.DebugBrown("非百分比" + info.Value + "::" + str);
                                if (info.Value != 0)
                                {
                                    num = (float)(info.Value);
                                    numTxt.text = num + dd;
                                }
                                else
                                {
                                    numTxt.text = 0 + dd;
                                }

                            }

                        }
                        else
                        {
                            numTxt.text = "0" + dd;
                        }
                        desTxt.text = str;
                    }
                }
                //for (int i = 0, length = c2G_Player.Info.Count; i < length; i++)
                //{
                //    //G2C_BattleKVData itemdata = c2G_Player.Info[i];
                //    //roleEntity.Property.Set(itemdata);

                //}
                //// UIRoleInfoInfo.ZhuanShengCount = c2G_Player.Reincarnate;
                ////  ZhuanShengCountTxt.text = $"转生次数;{UIRoleInfoInfo.ZhuanShengCount}次";
                //foreach (G2C_BattleKVData item in c2G_Player.Info)
                //{
                //    // Log.DebugBrown($"玩家属性：{item.Key} -> {item.Value}");
                //    roleEntity.Property.Set(item);
                //    if ((E_GameProperty)item.Key == E_GameProperty.FreePoint)
                //    {
                //        // Log.DebugGreen($"等级点数：{item.Value}");
                //        roleEntity.Property.ChangeProperValue(E_GameProperty.FreePoint, item.Value);
                //    }
                //}
                ////刷新玩家属性
                //RefreshRoleProperty();
            }
        }





        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            //if (roleEntity_Obj != null)
            //{
            //    //ResourcesComponent.Instance.RecycleGameObject(roleEntity_Obj);
            //    GameObject.Destroy(roleEntity_Obj);
            //}
            roleEntity = null;
            collector = null;
            UIMainComponent.Instance.UpdateMCCountDownAction = null;
            UIMainComponent.Instance.UpdateMaxCCountDownAction = null;
            UIMainComponent.Instance.UpdateInSituCdAction = null;
            UIMainComponent.Instance.UpdateRedNameCdAction = null;

        }
    }
}
