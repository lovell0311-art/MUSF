using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.U2D;
using System;
using System.Linq;
using static NPOI.HSSF.Util.HSSFColor;
using static UnityEditor.Progress;
using static UnityEditor.ShaderData;

namespace ETHotfix
{
    [ObjectSystem]
    public class RoleEquipmentComponentAwake : AwakeSystem<RoleEquipmentComponent>
    {
        public override void Awake(RoleEquipmentComponent self)
        {
           // Log.DebugGreen($"{self.GetParent<UnitEntity>().Id}  {self.GetParent<RoleEntity>().Id}");
            self.roleEntity = self.GetParent<RoleEntity>();

            self.animatorComponent = self.roleEntity.GetComponent<AnimatorComponent>();

            self.Init_DefaultEquips();
            self.obj = GameObject.Find("0019");
            // self.GetWareEquips().Coroutine();
        }
    }

    [ObjectSystem]
    public class RoleEquipmentComponentUpdate : UpdateSystem<RoleEquipmentComponent>
    {
        public override void Update(RoleEquipmentComponent self)
        {
            //处理屋顶问题

            self.tiems += Time.deltaTime;

            if(SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName()=="冰风谷")
            {
                if (self.tiems >= 2 && self.obj1 == null)
                {
                    self.obj1 = GameObject.Find("lingwu1");
                    self.tiems = 0;
                    if (self.obj1 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 372 && self.roleEntity.Game_Object.transform.position.x <= 386 && self.roleEntity.Game_Object.transform.position.z >= 92 && self.roleEntity.Game_Object.transform.position.z <= 100)
                        {
                            self.obj1.gameObject.SetActive(false);
                            self.ObjBool1 = true;
                        }
                    }

                    if (self.ObjBool1 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 372 && self.roleEntity.Game_Object.transform.position.x <= 386 && self.roleEntity.Game_Object.transform.position.z >= 92 && self.roleEntity.Game_Object.transform.position.z <= 100)
                        {

                        }
                        else
                        {
                            self.obj1.gameObject.SetActive(true);
                            self.ObjBool1 = false;
                        }

                    }
                }
                if (self.obj1 != null)
                {
                    if (self.obj1 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 372 && self.roleEntity.Game_Object.transform.position.x <= 386 && self.roleEntity.Game_Object.transform.position.z >= 92 && self.roleEntity.Game_Object.transform.position.z <= 100)
                        {
                            self.obj1.gameObject.SetActive(false);
                            self.ObjBool1 = true;
                        }
                    }

                    if (self.ObjBool1 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 372 && self.roleEntity.Game_Object.transform.position.x <= 386 && self.roleEntity.Game_Object.transform.position.z >= 92 && self.roleEntity.Game_Object.transform.position.z <= 100)
                        {

                        }
                        else
                        {
                            self.obj1.gameObject.SetActive(true);
                            self.ObjBool1 = false;
                        }

                    }
                }



                if (self.tiems >= 2 && self.obj2 == null)
                {
                    self.obj2 = GameObject.Find("lingwu2");
                    self.tiems = 0;
                    if (self.obj2 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 404 && self.roleEntity.Game_Object.transform.position.x <= 416 && self.roleEntity.Game_Object.transform.position.z >= 110 && self.roleEntity.Game_Object.transform.position.z <= 124)
                        {
                            self.obj2.gameObject.SetActive(false);
                            self.ObjBool2 = true;
                        }
                    }

                    if (self.ObjBool2 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 404 && self.roleEntity.Game_Object.transform.position.x <= 416 && self.roleEntity.Game_Object.transform.position.z >= 110 && self.roleEntity.Game_Object.transform.position.z <= 124)
                        {

                        }
                        else
                        {
                            self.obj2.gameObject.SetActive(true);
                            self.ObjBool2 = false;
                        }

                    }
                }
                if (self.obj2 != null)
                {
                    if (self.obj2 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 404 && self.roleEntity.Game_Object.transform.position.x <= 416 && self.roleEntity.Game_Object.transform.position.z >= 110 && self.roleEntity.Game_Object.transform.position.z <= 124)
                        {
                            self.obj2.gameObject.SetActive(false);
                            self.ObjBool2 = true;
                        }
                    }

                    if (self.ObjBool2 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 404 && self.roleEntity.Game_Object.transform.position.x <= 416 && self.roleEntity.Game_Object.transform.position.z >= 110 && self.roleEntity.Game_Object.transform.position.z <= 124)
                        {

                        }
                        else
                        {
                            self.obj2.gameObject.SetActive(true);
                            self.ObjBool2 = false;
                        }

                    }
                }



                if (self.tiems >= 2 && self.obj3 == null)
                {
                    self.obj3 = GameObject.Find("lingwu3");
                    self.tiems = 0;
                    if (self.obj3 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 448 && self.roleEntity.Game_Object.transform.position.x <= 464 && self.roleEntity.Game_Object.transform.position.z >= 76 && self.roleEntity.Game_Object.transform.position.z <= 86)
                        {
                            self.obj3.gameObject.SetActive(false);
                            self.ObjBool3 = true;
                        }
                    }

                    if (self.ObjBool3 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 448 && self.roleEntity.Game_Object.transform.position.x <= 464 && self.roleEntity.Game_Object.transform.position.z >= 76 && self.roleEntity.Game_Object.transform.position.z <= 86)
                        {

                        }
                        else
                        {
                            self.obj3.gameObject.SetActive(true);
                            self.ObjBool3 = false;
                        }

                    }
                }
                if (self.obj3 != null)
                {
                    if (self.obj3 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 448 && self.roleEntity.Game_Object.transform.position.x <= 464 && self.roleEntity.Game_Object.transform.position.z >= 76 && self.roleEntity.Game_Object.transform.position.z <= 86)
                        {
                            self.obj3.gameObject.SetActive(false);
                            self.ObjBool3 = true;
                        }
                    }

                    if (self.ObjBool3 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 448 && self.roleEntity.Game_Object.transform.position.x <= 464 && self.roleEntity.Game_Object.transform.position.z >= 76 && self.roleEntity.Game_Object.transform.position.z <= 86)
                        {

                        }
                        else
                        {
                            self.obj3.gameObject.SetActive(true);
                            self.ObjBool3 = false;
                        }

                    }
                }




                if (self.tiems >= 2 && self.obj4 == null)
                {
                    self.obj4 = GameObject.Find("lingwu4");
                    self.tiems = 0;
                    if (self.obj4 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 452 && self.roleEntity.Game_Object.transform.position.x <= 470 && self.roleEntity.Game_Object.transform.position.z >= 40 && self.roleEntity.Game_Object.transform.position.z <= 54)
                        {
                            self.obj4.gameObject.SetActive(false);
                            self.ObjBool4 = true;
                        }
                    }

                    if (self.ObjBool4 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 452 && self.roleEntity.Game_Object.transform.position.x <= 470 && self.roleEntity.Game_Object.transform.position.z >= 40 && self.roleEntity.Game_Object.transform.position.z <= 54)
                        {

                        }
                        else
                        {
                            self.obj4.gameObject.SetActive(true);
                            self.ObjBool4 = false;
                        }

                    }
                }
                if (self.obj4 != null)
                {
                    if (self.obj4 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 452 && self.roleEntity.Game_Object.transform.position.x <= 470 && self.roleEntity.Game_Object.transform.position.z >= 40 && self.roleEntity.Game_Object.transform.position.z <= 54)
                        {
                            self.obj4.gameObject.SetActive(false);
                            self.ObjBool4 = true;
                        }
                    }

                    if (self.ObjBool4 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 452 && self.roleEntity.Game_Object.transform.position.x <= 470 && self.roleEntity.Game_Object.transform.position.z >= 40 && self.roleEntity.Game_Object.transform.position.z <= 54)
                        {

                        }
                        else
                        {
                            self.obj4.gameObject.SetActive(true);
                            self.ObjBool4 = false;
                        }

                    }
                }




                if (self.tiems >= 2 && self.obj5 == null)
                {
                    self.obj5 = GameObject.Find("lingwu5");
                    self.tiems = 0;
                    if (self.obj5 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 408 && self.roleEntity.Game_Object.transform.position.x <= 428 && self.roleEntity.Game_Object.transform.position.z >= 26 && self.roleEntity.Game_Object.transform.position.z <= 62)
                        {
                            self.obj5.gameObject.SetActive(false);
                            self.ObjBool5 = true;
                        }
                    }

                    if (self.ObjBool5 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 408 && self.roleEntity.Game_Object.transform.position.x <= 428 && self.roleEntity.Game_Object.transform.position.z >= 26 && self.roleEntity.Game_Object.transform.position.z <= 62)
                        {

                        }
                        else
                        {
                            self.obj5.gameObject.SetActive(true);
                            self.ObjBool5 = false;
                        }

                    }
                }
                if (self.obj5 != null)
                {
                    if (self.obj5 != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 408 && self.roleEntity.Game_Object.transform.position.x <= 428 && self.roleEntity.Game_Object.transform.position.z >= 26 && self.roleEntity.Game_Object.transform.position.z <= 62)
                        {
                            self.obj5.gameObject.SetActive(false);
                            self.ObjBool5 = true;
                        }
                    }

                    if (self.ObjBool5 == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 408 && self.roleEntity.Game_Object.transform.position.x <= 428 && self.roleEntity.Game_Object.transform.position.z >= 26 && self.roleEntity.Game_Object.transform.position.z <= 62)
                        {

                        }
                        else
                        {
                            self.obj5.gameObject.SetActive(true);
                            self.ObjBool5 = false;
                        }

                    }
                }


            }
            else if(SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == "勇者大陆")
            {
                if (self.tiems >= 2 && self.obj == null)
                {
                    self.obj = GameObject.Find("0019");
                    self.tiems = 0;
                    if (self.obj != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 242 && self.roleEntity.Game_Object.transform.position.x <= 256 && self.roleEntity.Game_Object.transform.position.z >= 242 && self.roleEntity.Game_Object.transform.position.z <= 272)
                        {
                            self.obj.gameObject.SetActive(false);
                            self.ObjBool = true;
                        }
                    }

                    if (self.ObjBool == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 242 && self.roleEntity.Game_Object.transform.position.x <= 256 && self.roleEntity.Game_Object.transform.position.z >= 242 && self.roleEntity.Game_Object.transform.position.z <= 272)
                        {

                        }
                        else
                        {
                            self.obj.gameObject.SetActive(true);
                            self.ObjBool = false;
                        }

                    }
                }
                if (self.obj != null)
                {
                    if (self.obj != null)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 242 && self.roleEntity.Game_Object.transform.position.x <= 256 && self.roleEntity.Game_Object.transform.position.z >= 242 && self.roleEntity.Game_Object.transform.position.z <= 272)
                        {
                            self.obj.gameObject.SetActive(false);
                            self.ObjBool = true;
                        }
                    }

                    if (self.ObjBool == true)
                    {
                        if (self.roleEntity.Game_Object.transform.position.x >= 242 && self.roleEntity.Game_Object.transform.position.x <= 256 && self.roleEntity.Game_Object.transform.position.z >= 242 && self.roleEntity.Game_Object.transform.position.z <= 272)
                        {

                        }
                        else
                        {
                            self.obj.gameObject.SetActive(true);
                            self.ObjBool = false;
                        }

                    }
                }

            }







        }
    }

    [ObjectSystem]
    public class RoleEquipmentComponentStart : StartSystem<RoleEquipmentComponent>
    {
        public override void Start(RoleEquipmentComponent self)
        {
            //self.Init_DefaultEquips();
        }
    }

    /// <summary>
    /// 玩家装备 组件
    /// </summary>
    public class RoleEquipmentComponent : Component
    {

        /// <summary>
        /// 角色默认穿戴装备字典
        /// </summary>

        public Dictionary<E_Grid_Type, GameObject> roleDefault_EquipDic = new Dictionary<E_Grid_Type, GameObject>();
        public Dictionary<E_Grid_Type, Mesh> roleDefault_MeshDic = new Dictionary<E_Grid_Type, Mesh>();
        public MeshFilter legMesh;
        //套装护盾效果
        private GameObject HuDuneffect_zbtx;

        /// <summary>
        /// 角色当前穿戴的装备
        /// </summary>
        public Dictionary<E_Grid_Type, GameObject> roleCurWare_EquipDic = new Dictionary<E_Grid_Type, GameObject>();


        /// <summary>
        /// 玩家 当前穿戴的装备对应 的属性字典
        /// </summary>
        public Dictionary<E_Grid_Type, KnapsackDataItem> curWareEquipsData_Dic = new Dictionary<E_Grid_Type, KnapsackDataItem>();

        public RoleEntity roleEntity;//玩家实体


        public AnimatorComponent animatorComponent;

        public long curMountConfigId = 0;//当前坐骑的配置表ID

        public List<GameObject> Skill_Effect_HuoFengHuangQiXuan = new List<GameObject>();


        /// <summary>
        /// 弓箭手 穿戴某些头盔时 不隐藏默认头盔
        /// 这里 添加一下 就行了 key:装备配置表ID value:装备名字
        /// 穿戴时 不会隐藏默认头
        /// </summary>
        private readonly Dictionary<long, string> ShowArcherHelmetList = new Dictionary<long, string>{
            { 170012, "风之盔" },{ 170013,"精灵之盔"},{ 170010,"藤盔"},{ 170011,"天蚕之盔"},{ 170002,"革盔"},
            { 170055,"法魂玄灵之盔"},{ 170136,"银心魔幻之盔"},{ 170000,"青铜盔"},{ 170076,"野望之盔"},{ 170030,"飓风之盔"}
        };
        public float tiems = 0;
        public GameObject obj,obj1,obj2,obj3,obj4,obj5,obj6;
        public bool ObjBool = false;
        public bool ObjBool1 = false;
        public bool ObjBool2 = false;
        public bool ObjBool3 = false;
        public bool ObjBool4 = false;
        public bool ObjBool5 = false;
        /// <summary>
        /// 获取默认的装备
        /// </summary>
        public void Init_DefaultEquips()
        {

            ReferenceCollector collector = roleEntity.Game_Object.GetReferenceCollector();
            roleDefault_EquipDic.Clear();
            var obj = collector.GetAssets(MonoReferenceType.Object);
            foreach (GameObject item in obj)
            {
                if (item.GetComponent<Animator>() != null)
                {
                    animatorComponent.AddSubAnimator(item.GetComponent<Animator>());
                }

                switch (item.name)
                {
                    case "HandGuard"://护手
                        roleDefault_EquipDic[E_Grid_Type.HandGuard] = item;
                        roleDefault_MeshDic[E_Grid_Type.HandGuard] = item.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                        break;
                    case "Leggings"://护腿
                        roleDefault_EquipDic[E_Grid_Type.Leggings] = item;
                        roleDefault_MeshDic[E_Grid_Type.Leggings] = item.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                        break;
                    case "Armor"://铠（衣服）
                        roleDefault_EquipDic[E_Grid_Type.Armor] = item;
                        roleDefault_MeshDic[E_Grid_Type.Armor] = item.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                        break;
                    case "Helmet"://头盔
                        roleDefault_EquipDic[E_Grid_Type.Helmet] = item;
                        roleDefault_MeshDic[E_Grid_Type.Helmet] = item.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                        break;
                    case "Boots"://鞋子
                        roleDefault_EquipDic[E_Grid_Type.Boots] = item;
                        roleDefault_MeshDic[E_Grid_Type.Boots] = item.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                        break;
                    case "R_Weapon"://右手武器节点
                        roleDefault_EquipDic[E_Grid_Type.Weapon] = item;
                        break;
                    case "L_Weapon"://左手武器节点
                        roleDefault_EquipDic[E_Grid_Type.Shield] = item;
                        break;
                    case "LeftBackPos"://背部武器 左边节点
                        roleDefault_EquipDic[E_Grid_Type.LeftBackPos] = item;
                        break;
                    case "RightBackpos"://背部武器 右边节点
                        roleDefault_EquipDic[E_Grid_Type.RightBackPos] = item;
                        break;
                    case "ChiBangParent"://翅膀节点
                        roleDefault_EquipDic[E_Grid_Type.Wing] = item;
                        break;
                    case "FlagParent"://旗帜节点
                        roleDefault_EquipDic[E_Grid_Type.Flag] = item;
                        break;
                    case "WristBand"://手环节点
                        roleDefault_EquipDic[E_Grid_Type.WristBand] = item;
                        break;
                    case "effect_zbtx"://装备护盾效果
                        HuDuneffect_zbtx = item;
                        HuDuneffect_zbtx.SetActive(false);
                        break;
                }
                if (item.name.Contains("HFHQX"))
                    Skill_Effect_HuoFengHuangQiXuan.Add(item);
            }
        }

        /// <summary>
        /// 缓存当前穿戴的装备 属性数据
        /// </summary>
        /// <param name="type">装备部位</param>
        /// <param name="dataItem">对应的装备属性数据</param>
        public void CacheEquipment(E_Grid_Type type, KnapsackDataItem dataItem)
        {
            curWareEquipsData_Dic[type] = dataItem;

            ChangeLoaclRoleProperty(type, true);

        }
        /// <summary>
        /// 移除当前的穿戴的装备 属性数据
        /// </summary>
        /// <param name="type">装备部位</param>
        public void RemoveCacheEquipment(E_Grid_Type type)
        {

            if (curWareEquipsData_Dic.ContainsKey(type))
            {
                curWareEquipsData_Dic[type].Dispose();
                curWareEquipsData_Dic.Remove(type);

                ChangeLoaclRoleProperty(type, false);

            }
        }
        /// <summary>
        /// 改变本地属性
        /// </summary>
        /// <param name="grid_Type"></param>
        /// <param name="state"></param>
        public void ChangeLoaclRoleProperty(E_Grid_Type grid_Type, bool state)
        {
            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                ///改变弓箭手的普攻距离
                if (grid_Type == E_Grid_Type.Weapon && roleEntity.RoleType == E_RoleType.Archer)
                {
                    UIMainComponent.Instance?.attackEntity.ChangeSkillAttackRange();
                }
                //else if (grid_Type == E_Grid_Type.Guard)
                //{
                //    RoleOnHookComponent.Instance.IsCanAutoPickUpGoldCoin = state;
                //}
            }
        }


        /// <summary>
        /// 离开、进入安全区时 武器的位置
        /// </summary>
        /// <param name="issafeArea">是否 在安全区</param>
        public void EnterSafeArea(bool issafeArea = true)
        {
            if (roleEntity.RoleType == E_RoleType.Gladiator)//格斗家
            {
                if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Wing, out GameObject wing1))
                {
                    animatorComponent.SetBoolValue(MotionType.IsWing, !issafeArea);//只有在安全区 才播放翅膀动画
                }
                //更新坐骑状态
                ChangeMountState(issafeArea);
                return;
            }

            //进入安全区 手上的武器、盾牌 放到背部。 
            //animatorComponent.SetBoolValue(MotionType.IsWing, issafeArea);//只有在安全区 才播放翅膀动画
            if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Wing, out GameObject wing))
            {
                animatorComponent.SetBoolValue(MotionType.IsWing, !issafeArea);//只有在安全区 才播放翅膀动画
            }
            //Log.DebugGreen($"测试动画{roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Wing, out GameObject wing1)}  {!issafeArea}");
            if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Weapon, out GameObject weapon))
            {
                if (curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Weapon, out KnapsackDataItem dataItem))
                {
                    dataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig item_);  //得到对应的配置表信息
                    if (roleEntity.RoleType == E_RoleType.Archer)//弓箭手
                    {

                        animatorComponent.SetBoolValue(MotionType.IsNu, item_.Type == (int)E_ItemType.Crossbows && !issafeArea);//当前武器是否是弩
                        animatorComponent.SetBoolValue(MotionType.IsGong, item_.Type == (int)E_ItemType.Bows && !issafeArea);//当前武器是否是弓
                        animatorComponent.SetBoolValue(MotionType.IsWeapon, item_.Type != (int)E_ItemType.Crossbows && item_.Type != (int)E_ItemType.Bows && !issafeArea);//当前武器不是弓、弩
                    }
                    else
                    {
                        string motion = item_.TwoHand == 1 ? MotionType.IsDoubleWeapon : MotionType.IsWeapon;
                        animatorComponent.SetBoolValue(motion, !issafeArea);
                    }
                }

                //当前装备了武器 进入安全区 则将手上的武器 放到背部右
                Transform weaponParent = issafeArea ? roleDefault_EquipDic[E_Grid_Type.RightBackPos].transform : roleDefault_EquipDic[E_Grid_Type.Weapon].transform;
                weapon.transform.SetParent(weaponParent, false);
                if (issafeArea ) weapon.SetBack(dataItem.GetProperValue(E_ItemValue.Level)); else weapon.SetWorld(dataItem.GetProperValue(E_ItemValue.Level));

                if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    weapon.SetLayer(LayerNames.LOCALROLE);
                }
                else
                {
                    weapon.SetLayer(LayerNames.ROLE);
                }

                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.transform.localScale = Vector3.one;
            }
            if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Shield, out GameObject shile))
            {
                //当前装备了盾牌 进入安全区 则将手上的盾牌 放到背部左
                if (curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Shield, out KnapsackDataItem dataItem))
                {
                    animatorComponent.SetBoolValue(MotionType.IsShield, !issafeArea);
                }

                if (roleEntity.RoleType == E_RoleType.Archer)//弓箭手
                {
                    if (dataItem.ConfigId == 40019 || (dataItem.ConfigId == 50012 || (dataItem.ConfigId / 10000 == (int)E_ItemType.Arrow)))
                    {

                    }
                    else
                    {
                        Transform shileParent = issafeArea ? roleDefault_EquipDic[E_Grid_Type.LeftBackPos].transform : roleDefault_EquipDic[E_Grid_Type.Shield].transform;
                        shile.transform.SetParent(shileParent, false);
                        if (issafeArea)
                        {

                            dataItem.Id.GetItemInfo_Out(out Item_infoConfig item_Info);
                            bool isleftback = item_Info.TwoHand == 0 && dataItem.ItemType != (int)E_ItemType.Shields && dataItem.ItemType != (int)E_ItemType.Arrow;
                            shile.SetBack(dataItem.GetProperValue(E_ItemValue.Level), isleftback);
                        }
                        else shile.SetWorld(dataItem.GetProperValue(E_ItemValue.Level));

                        if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                        {
                            shile.SetLayer(LayerNames.LOCALROLE);
                        }
                        else
                        {
                            shile.SetLayer(LayerNames.ROLE);
                        }

                        shile.transform.localPosition = Vector3.zero;
                        shile.transform.localRotation = Quaternion.identity;
                        shile.transform.localScale = Vector3.one;
                    }
                    
                }
                else if (roleEntity.RoleType == E_RoleType.Gladiator)//格斗家
                {
                    Transform shileParent = issafeArea ? roleDefault_EquipDic[E_Grid_Type.LeftBackPos].transform : roleDefault_EquipDic[E_Grid_Type.Shield].transform;
                    shile.transform.SetParent(shileParent, false);
                    //if (issafeArea)
                    //{

                    //    dataItem.Id.GetItemInfo_Out(out Item_infoConfig item_Info);
                    //    bool isleftback = item_Info.TwoHand == 0 && dataItem.ItemType != (int)E_ItemType.Shields && dataItem.ItemType != (int)E_ItemType.Arrow;
                    //    shile.SetBack(dataItem.GetProperValue(E_ItemValue.Level), false);
                    //}
                    //else 
                        shile.SetWorld(dataItem.GetProperValue(E_ItemValue.Level));

                    if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                    {
                        shile.SetLayer(LayerNames.LOCALROLE);
                    }
                    else
                    {
                        shile.SetLayer(LayerNames.ROLE);
                    }

                    shile.transform.localPosition = Vector3.zero;
                    shile.transform.localRotation = Quaternion.identity;
                    shile.transform.localScale = Vector3.one;
                }
                else
                {
                    Transform shileParent = issafeArea ? roleDefault_EquipDic[E_Grid_Type.LeftBackPos].transform : roleDefault_EquipDic[E_Grid_Type.Shield].transform;
                    shile.transform.SetParent(shileParent, false);
                    if (issafeArea)
                    {

                        dataItem.Id.GetItemInfo_Out(out Item_infoConfig item_Info);
                        bool isleftback = item_Info.TwoHand == 0 && dataItem.ItemType != (int)E_ItemType.Shields && dataItem.ItemType != (int)E_ItemType.Arrow;
                        shile.SetBack(dataItem.GetProperValue(E_ItemValue.Level), isleftback);
                    }
                    else shile.SetWorld(dataItem.GetProperValue(E_ItemValue.Level));

                    if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                    {
                        shile.SetLayer(LayerNames.LOCALROLE);
                    }
                    else
                    {
                        shile.SetLayer(LayerNames.ROLE);
                    }

                    shile.transform.localPosition = Vector3.zero;
                    shile.transform.localRotation = Quaternion.identity;
                    shile.transform.localScale = Vector3.one;
                }

            }
            //更新坐骑状态
            ChangeMountState(issafeArea);

        }

        /// <summary>
        /// 穿戴装备
        /// </summary>
        /// <param name="euipmentIndex">装备的配置表ID</param>
        /// <param name="lev">装备的等级</param>
        public void WareEquipMent(long equipmentIndex, int lev, bool isHideFashion = false, bool isHaveFashion = false)
        {

            //隐藏默认的装备
            equipmentIndex.GetItemInfo_Out(out Item_infoConfig item_);  //得到对应的配置表信息

            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{equipmentIndex}");
                return;
            }
            //隐藏默认装备
            if (roleDefault_EquipDic.TryGetValue((E_Grid_Type)item_.Slot, out GameObject obj))
            {
                if (!ShowArcherHelmetList.ContainsKey(equipmentIndex))
                {
                    obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
                    /*  if ((roleEntity.RoleType == E_RoleType.Magician||roleEntity.RoleType==E_RoleType.Magicswordsman) && item_.Slot == (int)E_Grid_Type.Leggings)
                    {
                        //魔法师 穿戴套装 隐藏默认护腿时，武器动画异常
                        legMesh = obj.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                        obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
                    }
                    else
                    {
                        obj.SetActive(false);
                    }*/
                }
            }
            if (isHideFashion == false)
            {
                for (E_Grid_Type i = E_Grid_Type.LeftRing; i <= E_Grid_Type.RightRing; i++)
                {
                    if (curWareEquipsData_Dic.TryGetValue(i, out KnapsackDataItem ringitem))
                    {
                        Item_RingsConfig item_Ring = ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)ringitem.ConfigId);

                        if (item_Ring.IsTransRing == 1)
                        {
                            return;
                        }
                    }
                }
                if (isHaveFashion)
                {
                    return;
                }
            }

            if (roleCurWare_EquipDic.TryGetValue((E_Grid_Type)item_.Slot, out GameObject game))
            {

                game.SetWorld(lev);
             
                return;
            }
            //显示新装备
            GameObject equipObj = ResourcesComponent.Instance.LoadGameObject(item_.ResName.StringToAB(), item_.ResName);
            equipObj.SetWorld(lev);

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                equipObj.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                equipObj.SetLayer(LayerNames.ROLE);
            }

            equipObj.transform.SetParent(roleEntity.Game_Object.transform, false);
            equipObj.transform.localPosition = Vector3.zero;
            equipObj.transform.localRotation = Quaternion.identity;
            equipObj.transform.localScale = Vector3.one;

            //应用当前动画

            /*if (!equipObj.transform.Find("World").TryGetComponent<Animator>(out var animator))
            {
              animator = equipObj.transform.Find("World").childCount > 0 ? equipObj.transform.Find("World").gameObject.AddComponent<Animator>() : equipObj.AddComponent<Animator>();
            }*/


            Animator animator;
            if (equipObj.GetComponent<Animator>()!=null)
            {
                animator = equipObj.GetComponent<Animator>();
            }
            else
            {
                if (equipObj.transform.Find("World").childCount > 0)
                {
                    if (equipObj.transform.Find("World").gameObject.GetComponent<Animator>() == null)
                    {
                        animator = equipObj.transform.Find("World").gameObject.AddComponent<Animator>();
                    }
                    else
                    {
                        animator = equipObj.transform.Find("World").gameObject.GetComponent<Animator>();
                    }
                }
                else
                {
                    animator = equipObj.AddComponent<Animator>();
                }
            }
           
            animator.runtimeAnimatorController = animatorComponent.Animator.runtimeAnimatorController;


            //是否持有武器
            animator.SetBool(MotionType.IsWeapon, animatorComponent.Animator.GetBool(MotionType.IsWeapon));
            //是否持有双手武器
            animator.SetBool(MotionType.IsDoubleWeapon, animatorComponent.Animator.GetBool(MotionType.IsDoubleWeapon));
            //是否有翅膀
            animator.SetBool(MotionType.IsWing, animatorComponent.Animator.GetBool(MotionType.IsWing));
            //是否有坐骑
            animator.SetBool(MotionType.IsMount, animatorComponent.Animator.GetBool(MotionType.IsMount));
            _ = equipObj.GetComponent<AnimationEventProxy>() ?? equipObj.AddComponent<AnimationEventProxy>();

            if (roleEntity.RoleType == E_RoleType.Archer)
            {
                //是否有弓
                animator.SetBool(MotionType.IsGong.ToString(), animatorComponent.Animator.GetBool(MotionType.IsGong.ToString()));
                //是否有驽
                animator.SetBool(MotionType.IsNu.ToString(), animatorComponent.Animator.GetBool(MotionType.IsNu.ToString()));
            }

            animatorComponent.AddSubAnimator(animator);

            //AnimatorStateInfo stateInfo = animatorComponent.Animator.GetCurrentAnimatorStateInfo(0);
            //animator.Play(stateInfo.fullPathHash, 0, stateInfo.normalizedTime);


            //添加到 当前穿戴的装备 字典
            roleCurWare_EquipDic[(E_Grid_Type)item_.Slot] = equipObj;
            ShowCaiDai();


            ///检查是否 穿戴了套装
            void ShowCaiDai()
            {
                //HuDuneffect_zbtx.SetActive(CheckLev());
                /*bool isShow = CheckLev();
                if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Armor, out GameObject armor))
                {
                    if (isShow) armor.ShowCaiDai();
                    else armor.HideCaiDai();
                }*/

                ///装备是否达到了 15级
                bool CheckLev()
                {
                    bool isshow = true;
                    for (int i = (int)E_Grid_Type.Helmet, length = (int)E_Grid_Type.Boots; i <= length; i++)
                    {
                        if (!curWareEquipsData_Dic.ContainsKey((E_Grid_Type)i))
                        {
                            isshow = false;
                            break;
                        }
                        if (curWareEquipsData_Dic.TryGetValue((E_Grid_Type)i, out KnapsackDataItem knapsackDataItem))
                        {
                            if (knapsackDataItem.GetProperValue(E_ItemValue.Level) < 13)
                            {
                                isshow = false;
                                break;
                            }
                        }
                    }
                    return isshow;
                }
            }

        }

        /// <summary>
        /// 卸载装备
        /// </summary>
        /// <param name="slot">卡槽位置</param>
        public void UnLoadEquipment(int slot)
        {

          

            //回收当前穿戴的装备
            if (roleCurWare_EquipDic.TryGetValue((E_Grid_Type)slot, out GameObject equipmemtObj))
            {

                //移除动画控制器
                if ((E_Grid_Type)slot != E_Grid_Type.Mounts && equipmemtObj.transform.Find("World")?.GetComponent<Animator>() != null)
                {
                    animatorComponent.RemoveSubAnimator(equipmemtObj.transform.Find("World").childCount > 0 ? equipmemtObj.transform.Find("World").GetComponent<Animator>() : equipmemtObj.GetComponent<Animator>());

                    equipmemtObj.transform.Find("World").GetComponent<Animator>().runtimeAnimatorController = null;
                }

                //更新动画控制器 状态
                switch ((E_Grid_Type)slot)
                {
                    case E_Grid_Type.Weapon:
                        if (roleEntity.RoleType == E_RoleType.Archer)
                        {
                            animatorComponent.SetBoolValue(MotionType.IsNu, false);//当前武器是否是弩
                            animatorComponent.SetBoolValue(MotionType.IsGong, false);//当前武器是否是弓
                            animatorComponent.SetBoolValue(MotionType.IsWeapon, false);//当前武器
                        }
                        else
                        {
                            animatorComponent.SetBoolValue(MotionType.IsWeapon, false);
                            animatorComponent.SetBoolValue(MotionType.IsDoubleWeapon, false);
                            // if (roleEntity.RoleType == E_RoleType.Magician|| roleEntity.RoleType == E_RoleType.Magicswordsman)//魔法师
                            {
                                animatorComponent.SetBoolValue(MotionType.IsSingleCane, false);
                                animatorComponent.SetBoolValue(MotionType.IsDoubelCane, false);//双手杖
                            }
                        }
                        break;
                    case E_Grid_Type.Shield:
                        animatorComponent.SetBoolValue(MotionType.IsShield, false);
                        animatorComponent.SetBoolValue(MotionType.IsLeftWeapon, false);
                        break;
                    case E_Grid_Type.Wing:
                        animatorComponent.SetBoolValue(MotionType.IsWing, false);
                        break;
                    case E_Grid_Type.Guard:
                        UIMainComponent.Instance?.ShowGuard(isshow: false, guardConfigId: 000);
                        GuardMoveComponent guardMoveComponent = equipmemtObj.GetComponent<GuardMoveComponent>();
                        GameObject.Destroy(guardMoveComponent);
                        break;
                    case E_Grid_Type.Pet:
                        UIMainComponent.Instance?.ShowGuard(isshow: false, guardConfigId: 000);
                        GuardMoveComponent PetMoveComponent = equipmemtObj.GetComponent<GuardMoveComponent>();
                        GameObject.Destroy(PetMoveComponent);
                        break;
                    case E_Grid_Type.RightRing:
                    case E_Grid_Type.LeftRing:
                        DestroyFashion();
                        break;
                    case E_Grid_Type.Mounts:
                        animatorComponent.SetBoolValue(MotionType.IsMount, false);
                        animatorComponent.SetBoolValue(MotionType.IsMountIdel, false);
                        this.roleEntity.Game_Object.transform.SetParent(this.roleEntity.roleTrs);
                        this.roleEntity.Game_Object.transform.localPosition = Vector3.zero;
                        this.roleEntity.Game_Object.transform.localRotation = Quaternion.identity;
                        animatorComponent.ChangeAnimationLayerWeight(1f);
                        break;
                    case E_Grid_Type.TianYing:
                        GuardMoveComponent tianyingMoveComponent = equipmemtObj.GetComponent<GuardMoveComponent>();
                        GameObject.Destroy(tianyingMoveComponent);
                        break;
                    default:
                        break;
                }

                ResourcesComponent.Instance.RecycleGameObject(equipmemtObj);

                //从当前穿戴的装备 字典中移除 该装备
                roleCurWare_EquipDic.Remove((E_Grid_Type)slot);
                //显示默认装备
                if (roleDefault_EquipDic.TryGetValue((E_Grid_Type)slot, out GameObject obj))
                {
                    if (roleDefault_MeshDic.TryGetValue((E_Grid_Type)slot, out Mesh mesh))
                    {
                        obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
                    }
                    /* if ((roleEntity.RoleType == E_RoleType.Magician || roleEntity.RoleType == E_RoleType.Magicswordsman) && slot == (int)E_Grid_Type.Leggings)
                  {

                      obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = legMesh;
                      legMesh = null;
                  }
                  obj.SetActive(true);*/
                }
                BianShenJieZhi();
                if (HuDuneffect_zbtx != null)
                {
                    HuDuneffect_zbtx.SetActive(false);
                }
                //HideCaiDai();
                ///检查是否 穿戴了套装
                void HideCaiDai()
                {
                    //未穿戴 铠甲
                    if (!roleCurWare_EquipDic.ContainsKey(E_Grid_Type.Armor))
                    {
                        return;
                    }
                    bool isShow = true;

                    for (int i = (int)E_Grid_Type.Helmet; i <= (int)E_Grid_Type.Boots; i++)
                    {
                        if (!roleCurWare_EquipDic.ContainsKey((E_Grid_Type)i))
                        {
                            isShow = false;
                        }
                        if (curWareEquipsData_Dic.TryGetValue((E_Grid_Type)i, out KnapsackDataItem knapsackDataItem))
                        {
                            if (knapsackDataItem.GetProperValue(E_ItemValue.Level) < 15)
                            {
                                isShow = false;
                            }
                        }
                    }

                    if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Armor, out GameObject armor))
                    {
                        if (isShow) armor.ShowCaiDai();
                        else armor.HideCaiDai();
                    }


                }


                void BianShenJieZhi()
                {
                    //变身戒指切换

                    if ((E_Grid_Type)slot == E_Grid_Type.LeftRing || (E_Grid_Type)slot == E_Grid_Type.RightRing)

                    {

                        if (roleCurWare_EquipDic.ContainsKey(E_Grid_Type.RightRing) == false && curWareEquipsData_Dic.ContainsKey(E_Grid_Type.RightRing))

                        {

                            var item = curWareEquipsData_Dic[E_Grid_Type.RightRing];

                            if (ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)item.ConfigId)?.IsTransRing == 1)

                            {

                                ShowFashion(item.ConfigId, item.GetProperValue(E_ItemValue.Level), E_Grid_Type.RightRing);

                            }

                        }

                    }

                }
            }

        }


        /// <summary>
        /// 佩戴武器
        /// </summary>
        /// <param name="weaponIndex">配置表id</param>
        /// <param name="lev">装备等级</param>
        public void WareEquipMent_Weapon(long weaponIndex, int lev)
        {

            //得到对应的配置表信息
            weaponIndex.GetItemInfo_Out(out Item_infoConfig item_);
            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{weaponIndex}");
                return;
            }
            bool isSafe = roleEntity.IsSafetyZone;
            if (roleEntity.RoleType == E_RoleType.Gladiator)
            {
                isSafe = false;
            }
            //是否在安全区域?背部武器节点：右手武器节点
            Transform weaponParent = isSafe ? roleDefault_EquipDic[E_Grid_Type.RightBackPos].transform : roleDefault_EquipDic[E_Grid_Type.Weapon].transform;
            // Log.DebugYellow($"武器父节点：{weaponParent.name}");
            GameObject weapon = ResourcesComponent.Instance.LoadGameObject(item_.ResName.StringToAB(), item_.ResName);//加载对应的武器模型

            if (isSafe) weapon.SetBack(lev);
            else weapon.SetWorld(lev);

            weapon.transform.SetParent(weaponParent, false);

            // Log.DebugGreen($"{roleEntity.Id== UnitEntityComponent.Instance.LocaRoleUUID}  {roleEntity.Id} {UnitEntityComponent.Instance.LocaRoleUUID}");
            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                weaponParent.gameObject.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                weaponParent.gameObject.SetLayer(LayerNames.ROLE);
            }

            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;
            if (roleEntity.RoleType == E_RoleType.Archer)//弓箭手
            {
                animatorComponent.SetBoolValue(MotionType.IsNu, item_.Type == (int)E_ItemType.Crossbows && !isSafe);//当前武器是否是弩
                animatorComponent.SetBoolValue(MotionType.IsGong, item_.Type == (int)E_ItemType.Bows && !isSafe);//当前武器是否是弓
                animatorComponent.SetBoolValue(MotionType.IsWeapon, item_.Type != (int)E_ItemType.Crossbows && item_.Type != (int)E_ItemType.Bows && !isSafe);//当前武器不是弓、弩
            }
            else
            {
                //是否是双手武器
                string motion = item_.TwoHand == 1 ? MotionType.IsDoubleWeapon : MotionType.IsWeapon;
                animatorComponent.SetBoolValue(motion, !isSafe);


                //  if (roleEntity.RoleType == E_RoleType.Magician|| roleEntity.RoleType == E_RoleType.Magicswordsman)//魔法师
                {
                    var typeId = item_.Id / 10000;
                    //权杖或魔杖
                    if (typeId == (int)E_ItemType.Scepter || typeId == (int)E_ItemType.Staffs)
                    {
                        animatorComponent.SetBoolValue(MotionType.IsSingleCane.ToString(), !isSafe && item_.TwoHand != 1);//单手杖

                        animatorComponent.SetBoolValue(MotionType.IsDoubelCane.ToString(), !isSafe && item_.TwoHand == 1);//双手杖
                    }
                }
            }
            roleCurWare_EquipDic[E_Grid_Type.Weapon] = weapon;
        }

        /// <summary>
        /// 装备盾牌
        /// </summary>
        /// <param name="shieldId"></param>
        /// <param name="lev"></param>
        public void WareEquipMent_Shield(long shieldId, int lev)
        {

            shieldId.GetItemInfo_Out(out Item_infoConfig item_); ;//得到对应的配置表信息

            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{shieldId}");
                return;
            }
            bool isSafe = roleEntity.IsSafetyZone;
            if (roleEntity.RoleType == E_RoleType.Gladiator)
            {
                isSafe = false;
            }
            //是否在安全区域?背部武器节点：右手武器节点
            Transform shieldParent = isSafe ? roleDefault_EquipDic[E_Grid_Type.LeftBackPos].transform : roleDefault_EquipDic[E_Grid_Type.Shield].transform;
            GameObject shield = ResourcesComponent.Instance.LoadGameObject(item_.ResName.StringToAB(), item_.ResName);//加载对应的武器模型


            if (roleEntity.RoleType == E_RoleType.Archer)//弓箭手
            {
                //弓箭、弩箭、箭筒 一直背在背上
                if (shieldId == 40019 || shieldId == 50012 || item_.Id / 10000 == (int)E_ItemType.Arrow)
                {
                    shield.SetBack(lev);
                    shieldParent = roleDefault_EquipDic[E_Grid_Type.LeftBackPos].transform;
                }
                else
                {
                    if (isSafe) shield.SetBack(lev);
                    else shield.SetWorld(lev);
                }
            }
            else if (roleEntity.RoleType == E_RoleType.Gladiator)//弓箭手
            {
                shield.SetWorld(lev);
            }
            else
            {

                if (isSafe)
                {
                    bool isleftback = item_.TwoHand == 0 && item_.Id / 10000 != (int)E_ItemType.Shields && item_.Id / 10000 != (int)E_ItemType.Arrow;

                    shield.SetBack(lev, isleftback);
                }
                else shield.SetWorld(lev);
            }


            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                shield.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                shield.SetLayer(LayerNames.ROLE);
            }

            shield.transform.SetParent(shieldParent, false);
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localRotation = Quaternion.identity;
            shield.transform.localScale = Vector3.one;
            //是否是双手武器
            if (item_.Id / 10000 == (int)E_ItemType.Shields)
            {
                animatorComponent.SetBoolValue(MotionType.IsShield, !isSafe);
            }
            else
            {
                animatorComponent.SetBoolValue(MotionType.IsLeftWeapon, !isSafe);
            }
            roleCurWare_EquipDic[E_Grid_Type.Shield] = shield;
        }

        /// <summary>
        /// 装备翅膀
        /// </summary>
        /// <param name="wingId"></param>
        /// <param name="lev"></param>
        public void WareEquipMent_Wing(long wingId, int lev)
        {
            wingId.GetItemInfo_Out(out Item_infoConfig item_); ;//得到对应的配置表信息
            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{wingId}");
                return;
            }
            if (roleCurWare_EquipDic.TryGetValue((E_Grid_Type)item_.Slot, out GameObject game))
            {
                game.SetWorld(lev);
                return;
            }
            Transform wingParent = roleDefault_EquipDic[E_Grid_Type.Wing].transform;
            GameObject wing = ResourcesComponent.Instance.LoadGameObject(item_.ResName.StringToAB(), item_.ResName);//加载对应的武器模型

            wing.SetWorld(lev);
            wing.transform.SetParent(wingParent, false);

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                wing.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                wing.SetLayer(LayerNames.ROLE);
            }

            wing.transform.localPosition = Vector3.zero;
            wing.transform.localRotation = Quaternion.identity;
            wing.transform.localScale = Vector3.one;
            bool isSafe = roleEntity.IsSafetyZone;
            animatorComponent.SetBoolValue(MotionType.IsWing, !isSafe);//只有在安全区 才播放翅膀动画
            roleCurWare_EquipDic[E_Grid_Type.Wing] = wing;
        }
        /// <summary>
        /// 装备手环
        /// </summary>
        /// <param name="flagId"></param>
        /// <param name="lev"></param>
        public void WareEquipMent_WristBand(long wristbandId, int lev)
        {
            wristbandId.GetItemInfo_Out(out Item_infoConfig item_);
            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{wristbandId}");
                return;
            }
            Transform wristbandParent = roleDefault_EquipDic[E_Grid_Type.WristBand].transform;
            GameObject wristband = ResourcesComponent.Instance.LoadGameObject(item_.ResName.StringToAB(), item_.ResName);

            wristband.SetWorld(lev);
            wristband.transform.SetParent(wristbandParent, false);

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                wristband.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                wristband.SetLayer(LayerNames.ROLE);
            }

            wristband.transform.localPosition = Vector3.zero;
            wristband.transform.localRotation = Quaternion.identity;

            roleCurWare_EquipDic[E_Grid_Type.WristBand] = wristband;
        }
        /// <summary>
        /// 装备旗帜
        /// </summary>
        /// <param name="flagId"></param>
        /// <param name="lev"></param>
        public void WareEquipMent_Flag(long flagId, int lev)
        {
            flagId.GetItemInfo_Out(out Item_infoConfig item_); ;//得到对应的配置表信息
            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{flagId}");
                return;
            }
            Transform flagParent = roleDefault_EquipDic[E_Grid_Type.Flag].transform;
            GameObject flag = ResourcesComponent.Instance.LoadGameObject(item_.ResName.StringToAB(), item_.ResName);//加载对应的旗帜模型

            flag.SetWorld(lev);
            flag.transform.SetParent(flagParent, false);

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                flag.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                flag.SetLayer(LayerNames.ROLE);
            }


            flag.transform.localPosition = Vector3.zero;
            flag.transform.localRotation = Quaternion.identity;
            roleCurWare_EquipDic[E_Grid_Type.Flag] = flag;
        }
        /// <summary>
        /// 显示宠物
        /// </summary>
        /// <param name="petId"></param>
        /// <param name="lev"></param>
        public void ShowPet(long petId, int lev, long uuid, bool isChooseRole = false)
        {

            if (isChooseRole)
            {
                UnitEntityFactory.CreatPet_ChooseRole(uuid, petId, roleEntity.Id, roleEntity);
            }
            else
            {
                return;
                //玩家 周围两个
                UnitEntityFactory.CreatPet(uuid, petId, roleEntity.Id, roleEntity?.CurrentNodePos);
            }

            return;
            petId.GetItemInfo_Out(out Item_infoConfig item_); ;//得到对应的配置表信息
            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{petId}");
                return;
            }
            //   var resName = item_.ResName.Replace("_beibaoUI","").Trim();
            GameObject pet = ResourcesComponent.Instance.LoadGameObject(item_.ResName.StringToAB(), item_.ResName);//加载对应的宠物模型

            pet.SetWorld(lev);

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                pet.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                pet.SetLayer(LayerNames.ROLE);
            }
            GuardMoveComponent guardMove = pet.transform.GetComponent<GuardMoveComponent>() ?? pet.transform.gameObject.AddComponent<GuardMoveComponent>();
            guardMove.SetFollowGameObj(this.roleEntity.Game_Object);
            roleCurWare_EquipDic[E_Grid_Type.Pet] = pet;

        }

        public void ChangePetPos() 
        {
            if (curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Pet, out KnapsackDataItem pet))
            {
                return;
                PetEntity petEntity=   UnitEntityComponent.Instance.Get<PetEntity>(pet.Id);
                AstarNode node = UnitEntityFactory.GetNearNode(roleEntity.CurrentNodePos,3);
                petEntity.Game_Object.transform.position= AstarComponent.Instance.GetVectory3(node.x, node.z);
                petEntity.Game_Object.transform.rotation = roleEntity.Game_Object.transform.rotation;
                petEntity.CurrentNodePos=node;
               
            }
        }
        /// <summary>
        /// 显示守护
        /// </summary>
        /// <param name="guardId"></param>
        /// <param name="lev"></param>
        public void ShowGuard(long guardId, int lev)
        {
            guardId.GetItemInfo_Out(out Item_infoConfig item_); ;//得到对应的配置表信息
            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{guardId}");
                return;
            }
            GameObject guard = ResourcesComponent.Instance.LoadGameObject(item_.ResName.StringToAB(), item_.ResName);//加载对应的守护模型

            guard.SetWorld(lev);

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                guard.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                guard.SetLayer(LayerNames.ROLE);
            }

            GuardMoveComponent guardMove = guard.transform.GetComponent<GuardMoveComponent>() ?? guard.transform.gameObject.AddComponent<GuardMoveComponent>();
            guardMove.SetFollowGameObj(this.roleEntity.Game_Object);
            roleCurWare_EquipDic[E_Grid_Type.Guard] = guard;

        }
        /// <summary>
        ///  显示时装
        /// </summary>
        /// <param name="fashionId"></param>
        /// <param name="lev"></param>
        public void ShowFashion(long fashionId, int lev, E_Grid_Type type,bool isShowFashionWing=false)
        {
           
            // Fashion_InfoConfig fashion_Info = ConfigComponent.Instance.GetItem<Fashion_InfoConfig>((int)fashionId);
            Item_RingsConfig fashion_Info = ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)fashionId);

          //  Log.DebugGreen($"显示时装  ：{PlayerPrefs.GetInt($"hidefashion_{roleEntity.Id}")} {fashion_Info.IsTransRing}  {fashion_Info.Name} {PlayerPrefs.GetInt($"hidefashion_{roleEntity.Id}")}");

            if (fashion_Info == null || fashion_Info.IsTransRing == 0) return;//不为时装戒指

            //是否显示变身戒指
            if (PlayerPrefs.GetInt($"hidefashion_{roleEntity.Id}") == 1)
            {
                //不显示 时装
                return;
            }

            if (type == E_Grid_Type.RightRing && roleCurWare_EquipDic.ContainsKey(E_Grid_Type.LeftRing))
            {

                return;
            }

            if (type == E_Grid_Type.LeftRing && roleCurWare_EquipDic.TryGetValue(E_Grid_Type.RightRing, out GameObject RinghtequipmemtObj))
            {
                // DestroyFashion();
                animatorComponent.RemoveSubAnimator(RinghtequipmemtObj.GetComponent<Animator>());
                RinghtequipmemtObj.GetComponent<Animator>().runtimeAnimatorController = null;
                ResourcesComponent.Instance.RecycleGameObject(RinghtequipmemtObj);
                roleCurWare_EquipDic.Remove(E_Grid_Type.RightRing);
            }


            //隐藏默认装备

            foreach (var defaultEquip in roleDefault_EquipDic)
            {
                if (defaultEquip.Key < E_Grid_Type.Helmet || defaultEquip.Key > E_Grid_Type.Boots)
                {
                    continue;
                }
                defaultEquip.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;

                /*if (defaultEquip.Value.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    defaultEquip.Value.GetComponent<SkinnedMeshRenderer>().enabled = false;
                }*/

                /* if ((roleEntity.RoleType == E_RoleType.Magician || roleEntity.RoleType == E_RoleType.Magicswordsman) && defaultEquip.Key ==E_Grid_Type.Leggings)
               {
                   //魔法师 穿戴套装 隐藏默认护腿时，武器动画异常
                   legMesh = defaultEquip.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                   defaultEquip.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
                   continue;
               }

               defaultEquip.Value.SetActive(false);*/
            }
            //隐藏 头盔 铠甲 护手 护腿 靴子
            for (int i = (int)E_Grid_Type.Helmet; i <= (int)E_Grid_Type.Boots; i++)
            {
                if (roleCurWare_EquipDic.TryGetValue((E_Grid_Type)i, out GameObject equipmemtObj))
                {
                    ResourcesComponent.Instance.RecycleGameObject(equipmemtObj);
                    animatorComponent.RemoveSubAnimator(equipmemtObj.GetComponent<Animator>());
                    equipmemtObj.GetComponent<Animator>().runtimeAnimatorController = null;
                    roleCurWare_EquipDic.Remove((E_Grid_Type)i);
                }
            }

            string fashionResName = fashion_Info.FashionResName.StringToDictionary_String()[GetRoleResName()];
            if (string.IsNullOrEmpty(fashionResName))
            {
                Log.DebugRed($"{fashionId}--时装不存在");
                return;
            }
            GameObject CurFaShion = ResourcesComponent.Instance.LoadGameObject(fashionResName.StringToAB(), fashionResName);

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                CurFaShion.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                CurFaShion.SetLayer(LayerNames.ROLE);
            }

            CurFaShion.GetReferenceCollector().GetGameObject("Stage_1").SetActive(lev>=9);
            CurFaShion.transform.SetParent(this.roleEntity.Game_Object.transform);
            CurFaShion.transform.localPosition = Vector3.zero;
            CurFaShion.transform.localRotation = Quaternion.identity;
            CurFaShion.transform.localScale = Vector3.one;

            //判断玩家是否佩戴翅膀
            if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Wing, out GameObject wing))//角色 佩戴了 翅膀
            {

                if (CurFaShion.GetReferenceCollector()?.GetGameObject("Wing") != null)//当前时装带有翅膀
                {
                    //显示时装翅膀
                    ResourcesComponent.Instance.RecycleGameObject(wing);
                    roleCurWare_EquipDic.Remove(E_Grid_Type.Wing);
                    CurFaShion.GetReferenceCollector().GetGameObject("Wing").SetActive(true);
                }

            }
            else
            {
                //角色为佩戴 翅膀
                if (CurFaShion.GetReferenceCollector()?.GetGameObject("Wing") != null)//当前时装带有翅膀 
                {
                    //隐藏时装翅膀
                    CurFaShion.GetReferenceCollector().GetGameObject("Wing").SetActive(false);
                }

            }

            //应用当前动画
            Animator animator = CurFaShion.GetComponent<Animator>();
            if (animator == null)
            {
                CurFaShion.AddComponent<Animator>();
            }
            animator.runtimeAnimatorController = animatorComponent.Animator.runtimeAnimatorController;
            AnimatorStateInfo stateInfo = animatorComponent.Animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(stateInfo.fullPathHash, 0, stateInfo.normalizedTime);
            //是否持有武器
            animator.SetBool(MotionType.IsWeapon, animatorComponent.Animator.GetBool(MotionType.IsWeapon));
            //是否持有双手武器
            animator.SetBool(MotionType.IsDoubleWeapon, animatorComponent.Animator.GetBool(MotionType.IsDoubleWeapon));
            animator.SetBool(MotionType.IsMount, animatorComponent.Animator.GetBool(MotionType.IsMount));
            //是否有翅膀
            animator.SetBool(MotionType.IsWing, animatorComponent.Animator.GetBool(MotionType.IsWing));
            _ = CurFaShion.GetComponent<AnimationEventProxy>() ?? CurFaShion.AddComponent<AnimationEventProxy>();
            animatorComponent.AddSubAnimator(animator);

            roleCurWare_EquipDic[type] = CurFaShion;

            int GetRoleResName() => roleEntity.RoleType switch
            {
                E_RoleType.Magician => 0,
                E_RoleType.Swordsman => 0,
                E_RoleType.Archer => 1,
                E_RoleType.Magicswordsman => 0,
                E_RoleType.Holymentor => 0,
                E_RoleType.Summoner => 1,
                _ => 0
            };
        }
        /// <summary>
        /// 销毁时装
        /// </summary>
        public void DestroyFashion()
        {
            //显示默认装备
            foreach (var defaultEquip in roleDefault_EquipDic)
            {
                if (roleDefault_MeshDic.TryGetValue(defaultEquip.Key, out Mesh mesh))
                {
                    defaultEquip.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
                }

                //显示魔法师的护腿
                /*   if ((roleEntity.RoleType == E_RoleType.Magician || roleEntity.RoleType == E_RoleType.Magicswordsman) && defaultEquip.Key == E_Grid_Type.Leggings && legMesh != null)
                   {
                       defaultEquip.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh = legMesh;
                       legMesh = null;
                   }*/
                defaultEquip.Value.SetActive(true);
            }

            //显示当前 已经穿戴的装备
            foreach (var item in curWareEquipsData_Dic.Values)
            {
              
                switch ((E_Grid_Type)item.Slot)
                {
                    case E_Grid_Type.Helmet:
                    case E_Grid_Type.Armor:
                    case E_Grid_Type.Leggings:
                    case E_Grid_Type.HandGuard:
                    case E_Grid_Type.Boots:
                        WareEquipMent(item.ConfigId, item.GetProperValue(E_ItemValue.Level));
                        break;
                    case E_Grid_Type.Wing:
                        WareEquipMent_Wing(item.ConfigId, item.GetProperValue(E_ItemValue.Level));
                        break;
                }

            }
        }
        //隐藏时装
        public void HideFashion(int slot) 
        {
            //显示默认装备
            foreach (var defaultEquip in roleDefault_EquipDic)
            {

                if (roleDefault_MeshDic.TryGetValue(defaultEquip.Key, out Mesh mesh))
                {
                    defaultEquip.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
                }
                //显示魔法师的护腿
              /*  if ((roleEntity.RoleType == E_RoleType.Magician || roleEntity.RoleType == E_RoleType.Magicswordsman) && defaultEquip.Key == E_Grid_Type.Leggings && legMesh != null)
                {
                    defaultEquip.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh = legMesh;
                    legMesh = null;
                }*/
                defaultEquip.Value.SetActive(true);
            }

            roleCurWare_EquipDic.TryGetValue((E_Grid_Type)slot, out GameObject equipmemtObj);
          
            animatorComponent.RemoveSubAnimator(equipmemtObj.GetComponent<Animator>());
            equipmemtObj.GetComponent<Animator>().runtimeAnimatorController = null;
            ResourcesComponent.Instance.RecycleGameObject(equipmemtObj);

            //显示当前 已经穿戴的装备
            foreach (var item in curWareEquipsData_Dic.Values)
            {
                switch ((E_Grid_Type)item.Slot)
                {
                    case E_Grid_Type.Helmet:
                    case E_Grid_Type.Armor:
                    case E_Grid_Type.Leggings:
                    case E_Grid_Type.HandGuard:
                    case E_Grid_Type.Boots:
                        WareEquipMent(item.ConfigId, item.GetProperValue(E_ItemValue.Level),true);
                        break;
                    case E_Grid_Type.Wing:
                        WareEquipMent_Wing(item.ConfigId, item.GetProperValue(E_ItemValue.Level));
                        break;
                }

            }
        }

       
        /// <summary>
        /// 显示已经穿戴的装备
        /// </summary>
        /// <param name="equipList">已经穿戴的装备集合</param>
        public void UpdateRoleEquipment(List<G2C_LoginSystemEquipItemMessage> equipList)
        {
            bool isHaveFashion = false;
            foreach (var item in equipList)
            {
                switch ((E_Grid_Type)item.SlotID)
                {
                    case E_Grid_Type.Weapon:
                        WareEquipMent_Weapon(item.ConfigID, item.ItemLevel);
                        break;
                    case E_Grid_Type.LeftRing:
                    case E_Grid_Type.RightRing:
                        if (isHaveFashion == false)
                        {
                            isHaveFashion = ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)item.ConfigID).IsTransRing == 1;
                        }
                      
                        ShowFashion(item.ConfigID, item.ItemLevel, (E_Grid_Type)item.SlotID);
                        break;
                    case E_Grid_Type.Shield:
                        WareEquipMent_Shield(item.ConfigID, item.ItemLevel);
                        break;
                    case E_Grid_Type.Helmet:
                    case E_Grid_Type.Armor:
                    case E_Grid_Type.Leggings:
                    case E_Grid_Type.HandGuard:
                    case E_Grid_Type.Boots:
                       
                        WareEquipMent(item.ConfigID, item.ItemLevel, PlayerPrefs.GetInt($"hidefashion_{roleEntity.Id}") == 1, isHaveFashion);
                        break;
                    case E_Grid_Type.Wing:
                        WareEquipMent_Wing(item.ConfigID, item.ItemLevel);
                        break;
                    case E_Grid_Type.Guard:
                        ShowGuard(item.ConfigID, item.ItemLevel);
                        break;
                    case E_Grid_Type.Necklace:
                        break; 
                    case E_Grid_Type.Pet:
                     //   ShowPet(item.ConfigID,item.ItemLevel,item.ItemUID,true);
                        break;
                    case E_Grid_Type.Flag:
                        WareEquipMent_Flag(item.ConfigID, item.ItemLevel);
                        break;
                    case E_Grid_Type.WristBand://手环
                        WareEquipMent_WristBand(item.ConfigID, item.ItemLevel);
                        break;
                    case E_Grid_Type.Mounts:
                        UseMount((int)item.ConfigID);
                        break;
                    case E_Grid_Type.TianYing:
                        UseTianYing((int)item.ConfigID);
                        break;
                    case E_Grid_Type.LeftBackPos:
                        break;
                    case E_Grid_Type.RightBackPos:
                        break;
                }
            }


        }
        public void UpdateRoleEquipment(Struct_ItemInSlot_Status item)
        {
            switch ((E_Grid_Type)item.SlotID)
            {
                case E_Grid_Type.Weapon:
                    WareEquipMent_Weapon(item.ConfigID, item.ItemLevel);
                    break;
                case E_Grid_Type.Shield:
                    WareEquipMent_Shield(item.ConfigID, item.ItemLevel);
                    break;
                case E_Grid_Type.Helmet:
                case E_Grid_Type.Armor:
                case E_Grid_Type.Leggings:
                case E_Grid_Type.HandGuard:
                case E_Grid_Type.Boots:
                    WareEquipMent(item.ConfigID, item.ItemLevel, PlayerPrefs.GetInt($"hidefashion_{roleEntity.Id}") == 1);
                    break;
                case E_Grid_Type.Wing:
                    WareEquipMent_Wing(item.ConfigID, item.ItemLevel);
                    break;
                case E_Grid_Type.Guard:
                    ShowGuard(item.ConfigID, item.ItemLevel);
                    break;
                case E_Grid_Type.Necklace://项链 不处理
                    break;
                case E_Grid_Type.Pet:
                    ShowPet(item.ConfigID, item.ItemLevel, item.ItemUID);
                    break;
                case E_Grid_Type.LeftRing:
                case E_Grid_Type.RightRing:
                    ShowFashion(item.ConfigID, item.ItemLevel, (E_Grid_Type)item.SlotID);
                    break;
                case E_Grid_Type.Flag:
                    WareEquipMent_Flag(item.ConfigID, item.ItemLevel);
                    break;
                case E_Grid_Type.WristBand://手环
                    WareEquipMent_WristBand(item.ConfigID,item.ItemLevel);
                    break;
                case E_Grid_Type.Mounts:
                    UseMount((int)item.ConfigID);
                    break;
                case E_Grid_Type.TianYing:
                    UseTianYing((int)item.ConfigID);
                    break;


            }

        }


        /// <summary>
        /// 获取角色装备信息
        /// </summary>
        /// <returns></returns>
        public async ETVoid GetWareEquips()
        {
            G2C_GetPlayerInfoByGameUserIdResponse g2C_GetPlayerInfoBy = (G2C_GetPlayerInfoByGameUserIdResponse)await SessionComponent.Instance.Session.Call(
                new C2G_GetPlayerInfoByGameUserIdRequest { GameUserId = this.roleEntity.Id });
            if (g2C_GetPlayerInfoBy.Error != 0)
            {
                Log.DebugRed($"{g2C_GetPlayerInfoBy.Error.GetTipInfo()}");
            }
            else
            {
                UnitEntityComponent.Instance.SetUnitObjState(GlobalDataManager.IsHideRole, roleEntity);
            }
           

        }
        /// <summary>
        /// 天鹰（圣导师专属）
        /// </summary>
        /// <param name="tianyingId"></param>
        /// <param name="lev"></param>
        public void UseTianYing(long tianyingId, int lev = 0)
        {

            tianyingId.GetItemInfo_Out(out Item_infoConfig item_); ;//得到对应的配置表信息
            if (item_ == null)
            {
                Log.DebugRed($"配置不存在：{tianyingId}");
                return;
            }
            Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)tianyingId);
            GameObject tianying = ResourcesComponent.Instance.LoadGameObject(mounts_Info.MountResName.StringToAB(), mounts_Info.MountResName);//加载对应的守护模型

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                tianying.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                tianying.SetLayer(LayerNames.ROLE);
            }

            GuardMoveComponent tianyingMove = tianying.GetComponent<GuardMoveComponent>() ?? tianying.gameObject.AddComponent<GuardMoveComponent>();
            tianyingMove.SetFollowGameObj(this.roleEntity.Game_Object);
            roleCurWare_EquipDic[E_Grid_Type.TianYing] = tianying;

        }

        /// <summary>
        /// 使用坐骑
        /// </summary>
        /// <param name="mountConfigId">坐骑的配置表ID</param>
        public void UseMount(int mountConfigId, int lev = 0)
        {
           
            if (roleEntity.IsSafetyZone&&mountConfigId!=260020) return;

            if (curMountConfigId != 0 && mountConfigId == 0)
            {
                mountConfigId = (int)curMountConfigId;
            }
            animatorComponent.ChangeAnimationLayerWeight(.5f);
            Item_MountsConfig mounts_Info = ConfigComponent.Instance.GetItem<Item_MountsConfig>(mountConfigId);
            if (mounts_Info == null)
            {
               
                curMountConfigId = 0;
                return;
            }
            curMountConfigId = mountConfigId;
            GameObject Mount = ResourcesComponent.Instance.LoadGameObject(mounts_Info.MountResName.StringToAB(), mounts_Info.MountResName);//加载坐骑模型

            if (roleEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                Mount.SetLayer(LayerNames.LOCALROLE);
            }
            else
            {
                Mount.SetLayer(LayerNames.ROLE);
            }

            Animator animator = Mount.GetComponent<Animator>();
            animatorComponent.AddSubAnimator(animator);
            if(animator != null)
            {
                animator.SetBool(MotionType.IsMove, roleEntity.GetComponent<AnimatorComponent>().GetBoolParameterValue(MotionType.IsMove));
                animator.SetBool(MotionType.IsRun, roleEntity.GetComponent<AnimatorComponent>().GetBoolParameterValue(MotionType.IsRun));
            }
            Mount.transform.SetParent(this.roleEntity.roleTrs);//设置坐骑父对象
            Mount.transform.localPosition = Vector3.zero;//设置坐骑位置
            Mount.transform.localRotation = Quaternion.identity;

           // CameraFollowComponent.Instance.followTarget = Mount.transform;
           if(mountConfigId != 260020)
                animatorComponent.SetBoolValue(MotionType.IsMount, true);//拥有坐骑
            else
            {
                animatorComponent.SetBoolValue(MotionType.IsMountIdel, true);
            }
            var rolePos = Mount.GetReferenceCollector().GetGameObject("rolePos").transform;
            rolePos.transform.localPosition = Vector3.zero;
            roleEntity.Game_Object.transform.SetParent(rolePos);
            roleEntity.Game_Object.transform.localPosition= Mount.GetReferenceCollector().GetGameObject("RolePos").transform.localPosition;
            this.roleEntity.Game_Object.transform.localRotation = Quaternion.identity;


            /* Mount.transform.SetParent(this.roleEntity.Game_Object.transform.parent, false);//设置坐骑父对象
             Mount.transform.localPosition = Vector3.zero;//设置坐骑位置
             Mount.transform.localRotation = Quaternion.identity;

             Transform rolePos = Mount.GetReferenceCollector().GetGameObject("rolePos").transform;
             this.roleEntity.Game_Object.transform.SetParent(rolePos, false);
             this.roleEntity.Game_Object.transform.localPosition = Vector3.zero;
             this.roleEntity.Game_Object.transform.localRotation = Quaternion.identity;
             this.roleEntity.Game_Object = Mount;*/


            /* Vector3 rolePos = Mount.transform.Find("RolePos").localPosition;
             rolePos.z = -.25f;
             this.roleEntity.Game_Object.transform.localPosition = rolePos;//设置玩家高度
             animatorComponent.SetBoolValue(MotionType.IsMount, true);//拥有坐骑
             Mount.transform.SetParent(this.roleEntity.Game_Object.transform.parent, false);//设置坐骑父对象
             Mount.transform.localPosition = Vector3.zero;//设置坐骑位置
             Mount.transform.localRotation = Quaternion.identity;*/

            roleCurWare_EquipDic[E_Grid_Type.Mounts] = Mount;

            //播放翅膀动画
        }

        /// <summary>
        /// 更具安全状态改变坐骑状态
        /// </summary>
        /// <param name="ishide">
        /// true->隐藏坐骑
        /// fale ->显示坐骑
        /// </param>
        public void ChangeMountState(bool ishide)
        {

            if (curMountConfigId == 260020)
            {
                animatorComponent.SetBoolValue(MotionType.IsMountIdel,true);
                return;
            }

            if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Mounts, out GameObject mount))
            {
                if (curMountConfigId != 260020)
                    animatorComponent.SetBoolValue(MotionType.IsMount, !ishide);//拥有坐骑
                else
                    animatorComponent.SetBoolValue(MotionType.IsMountIdel, !ishide);
                this.roleEntity.Game_Object.transform.SetParent(this.roleEntity.roleTrs);
                this.roleEntity.Game_Object.transform.localPosition = Vector3.zero;
                this.roleEntity.Game_Object.transform.localRotation = Quaternion.identity;
                ResourcesComponent.Instance.RecycleGameObject(mount);
                roleCurWare_EquipDic[E_Grid_Type.Mounts] = null;
                UseMount((int)curMountConfigId);
            }
                
            //if (roleCurWare_EquipDic.TryGetValue(E_Grid_Type.Mounts, out GameObject mount))
            //{
            //    if (ishide)
            //    {
            //        //隐藏坐骑
            //        //animatorComponent.ChangeAnimationLayerWeight(1f);
            //        mount.SetActive(false);
            //        this.roleEntity.Game_Object.transform.SetParent(this.roleEntity.roleTrs);
            //        this.roleEntity.Game_Object.transform.localPosition = Vector3.zero;
            //        animatorComponent.SetBoolValue(MotionType.IsMount, false);//拥有坐骑
            //    }
            //    else
            //    {
            //        //animatorComponent.ChangeAnimationLayerWeight(.5f);

            //        var rolePos = mount.GetReferenceCollector().GetGameObject("rolePos").transform;
            //        rolePos.transform.localPosition = Vector3.zero;
            //        roleEntity.Game_Object.transform.SetParent(rolePos);
            //        roleEntity.Game_Object.transform.localPosition = mount.GetReferenceCollector().GetGameObject("RolePos").transform.localPosition;
            //        mount.SetActive(true);
            //        animatorComponent.SetBoolValue(MotionType.IsMount, true);//拥有坐骑
            //        if (mount.GetComponent<Animator>() != null)
            //        {
            //            mount.GetComponent<Animator>().SetBool("IsMove", true);
            //        }

            //    }
            //}
        }


        /// <summary>
        /// 清理装备
        /// </summary>
        public void CleanEquipment()
        {
            //回收穿戴的装备
            foreach (var item in roleCurWare_EquipDic)
            {
                if (item.Key == E_Grid_Type.Guard)
                {
                    //删除守护身上的移动组件
                    GameObject.Destroy(item.Value.GetComponent<GuardMoveComponent>());
                }
                if (item.Key == E_Grid_Type.Mounts)
                {
                    this.roleEntity.Game_Object.transform.SetParent(this.roleEntity.roleTrs);
                    this.roleEntity.Game_Object.transform.localPosition = Vector3.zero;
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(item.Value, item.Value.name.StringToAB());
                    continue;
                }

                ResourcesComponent.Instance.DestoryGameObjectImmediate(item.Value, item.Value.name.StringToAB());
            }
            roleCurWare_EquipDic.Clear();
         
            //清除 装备属性实体
            foreach (var item in curWareEquipsData_Dic)
            {
                item.Value.Dispose();
            }
            curWareEquipsData_Dic.Clear();
            //显示默认装备
            foreach (var item in roleDefault_EquipDic)
            {
                if (roleDefault_MeshDic.TryGetValue(item.Key, out Mesh mesh))
                {
                    item.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
                }
                //显示魔法师的护腿
                /*  if ((roleEntity.RoleType == E_RoleType.Magician || roleEntity.RoleType == E_RoleType.Magicswordsman) && item.Key == E_Grid_Type.Leggings && legMesh != null)
                {
                    item.Value.GetComponent<SkinnedMeshRenderer>().sharedMesh = legMesh;
                    legMesh = null;
                }
                
                item.Value.SetActive(true);*/
            }
            roleCurWare_EquipDic.Clear();
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();

            //清理节点装备
            CleanEquipment();
            Skill_Effect_HuoFengHuangQiXuan.Clear();

        }
    }
}