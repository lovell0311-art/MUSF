using Codice.Client.BaseCommands;
using ETModel;
using ILRuntime.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIRoleInfoComponent
    {
        public ReferenceCollector RecommendAddPointcollector;
        public Dictionary<E_RoleType, GameObject> RoleInfoAddPointDic = new Dictionary<E_RoleType, GameObject>();//玩家属性面板字典
        public Dictionary<Transform, int> RecommendAddPointDic = new Dictionary<Transform, int>();//
        public Text CanUsePointTxt;
        private RecommendAddPoint recommendAdd;
        private RecommendAddPoint CurRecommendAdd;
        private RoleAttribute roleAttribute;
        long freePoint = 0;
        public void InitRecommendAddPoint()
        {

            RecommendAddPointcollector = collector.GetImage("RecommendAddPoint").gameObject.GetReferenceCollector();
            CanUsePointTxt = RecommendAddPointcollector.GetText("CanUsePointTxt");

            RecommendAddPointcollector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                RecommendAddPointcollector.gameObject.SetActive(false);
                TitleImage.gameObject.SetActive(true);
            });
            //确认加点
            RecommendAddPointcollector.GetButton("ConfirmAdd").onClick.AddSingleListener(() =>
            {
                foreach (var item in RecommendAddPointDic)
                {
                    if (item.Value != 0)
                        AddPoint(item.Key, item.Value).Coroutine();
                }
                GetRoleProperties().Coroutine();
                GetPlayerSecondaryAttribute().Coroutine();
                RecommendAddPointcollector.gameObject.SetActive(false);
            });

            RecommendAddPointcollector.gameObject.SetActive(false);
        }

        public void ShowRecommendAddPoint()
        {
            RecommendAddPointcollector.gameObject.SetActive(true);
            InitRoleInfosAddPoint();
            CurRecommendAdd = recommendAdd = UIRoleInfoData.GetRoleRecommendAddPoint((int)roleEntity.Property.GetProperValue(E_GameProperty.FreePoint));

            RefreshRolePropertyAddPoint(recommendAdd);
        }

        public void InitRoleInfosAddPoint()
        {
            Dictionary<E_RoleType, string> maps = new Dictionary<E_RoleType, string>();
            maps.Add(E_RoleType.Magician, E_RoleType.Magician.ToString());
            maps.Add(E_RoleType.Swordsman, E_RoleType.Swordsman.ToString());
            maps.Add(E_RoleType.Archer, E_RoleType.Archer.ToString());
            maps.Add(E_RoleType.Magicswordsman, E_RoleType.Magician.ToString());
            maps.Add(E_RoleType.Holymentor, E_RoleType.Swordsman.ToString());
            maps.Add(E_RoleType.Summoner, E_RoleType.Magician.ToString());
            maps.Add(E_RoleType.Gladiator, E_RoleType.Swordsman.ToString());
            maps.Add(E_RoleType.GrowLancer, E_RoleType.Swordsman.ToString());
            for (int i = (int)E_RoleType.Magician; i <= (int)E_RoleType.GrowLancer; i++)
            {
                var typename = maps[(E_RoleType)i];
                GameObject infoobj = RecommendAddPointcollector.GetGameObject(typename.ToString());
                infoobj.SetActive(false);
                RoleInfoAddPointDic[(E_RoleType)i] = infoobj;
            }
        }

        public GameObject GetRoleInfoAddPointPanel()
        {
            GameObject infopanel = null;

            for (int i = (int)E_RoleType.Magician; i <= (int)E_RoleType.GrowLancer; i++)
            {
                RoleInfoAddPointDic.TryGetValue((E_RoleType)i, out GameObject gameObject);
                if (gameObject)
                {
                    gameObject.SetActive(false);
                }

            }
            RoleInfoAddPointDic.TryGetValue(roleEntity.RoleType, out infopanel);
            infopanel.SetActive(true);

            //for (int i = (int)E_RoleType.Magician; i <= (int)E_RoleType.GrowLancer; i++)
            //{
            //    var typename = (E_RoleType)i;
            //    if (RoleInfoAddPointDic.TryGetValue(typename, out GameObject gameObject))
            //    {
            //        if (typename == this.roleEntity.RoleType)
            //        {
            //            infopanel = gameObject;
            //            infopanel.SetActive(true);
            //        }
            //        else
            //        {
            //            gameObject.SetActive(false);
            //        }
            //    }
            //}
            return infopanel;
        }
        public void SetChildAddPoint(Transform trs, string value)
        {
            trs.Find("value").GetComponent<Text>().text = value;
            trs.Find("add").GetComponent<Button>().onClick.AddSingleListener(() => { AddPointBtn(trs); });
            trs.Find("jian").GetComponent<Button>().onClick.AddSingleListener(() => { MinusPointBtn(trs); });
        }

        public void MinusPointBtn(Transform trs)
        {
            E_GameProperty e_GameProperty = (E_GameProperty)int.Parse(trs.name.Split('_')[1]);

            switch (this.roleEntity.RoleType)
            {
                case E_RoleType.Magician:
                    switch (e_GameProperty)
                    {
                        case E_GameProperty.Property_Strength:
                            if (CurRecommendAdd.Four > 0)
                                CurRecommendAdd.Four -= 1;
                            break;
                        case E_GameProperty.Property_Willpower:
                            if (CurRecommendAdd.One > 0)
                                CurRecommendAdd.One -= 1;
                            break;
                        case E_GameProperty.Property_Agility:
                            if (CurRecommendAdd.Two > 0)
                                CurRecommendAdd.Two -= 1;
                            break;
                        case E_GameProperty.Property_BoneGas:
                            if (CurRecommendAdd.Three > 0)
                                CurRecommendAdd.Three -= 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case E_RoleType.Swordsman:
                    switch (e_GameProperty)
                    {
                        case E_GameProperty.Property_Strength:
                            if (CurRecommendAdd.One > 0)
                                CurRecommendAdd.One -= 1;
                            break;
                        case E_GameProperty.Property_Willpower:
                            if (CurRecommendAdd.Four > 0)
                                CurRecommendAdd.Four -= 1;
                            break;
                        case E_GameProperty.Property_Agility:
                            if (CurRecommendAdd.Two > 0)
                                CurRecommendAdd.Two -= 1;
                            break;
                        case E_GameProperty.Property_BoneGas:
                            if (CurRecommendAdd.Three > 0)
                                CurRecommendAdd.Three -= 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case E_RoleType.Archer:
                    switch (e_GameProperty)
                    {
                        case E_GameProperty.Property_Strength:
                            if (CurRecommendAdd.Two > 0)
                                CurRecommendAdd.Two -= 1;
                            break;
                        case E_GameProperty.Property_Willpower:
                            if (CurRecommendAdd.Four > 0)
                                CurRecommendAdd.Four -= 1;
                            break;
                        case E_GameProperty.Property_Agility:
                            if (CurRecommendAdd.One > 0)
                                CurRecommendAdd.One -= 1;
                            break;
                        case E_GameProperty.Property_BoneGas:
                            if (CurRecommendAdd.Three > 0)
                                CurRecommendAdd.Three -= 1;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            RefreshRolePropertyAddPoint(CurRecommendAdd);
        }
        public void AddPointBtn(Transform trs)
        {
            if (freePoint <= 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "剩余属性点数为0");
                return;
            }
            E_GameProperty e_GameProperty = (E_GameProperty)int.Parse(trs.name.Split('_')[1]);

            switch (this.roleEntity.RoleType)
            {
                case E_RoleType.Magician:
                    switch (e_GameProperty)
                    {
                        case E_GameProperty.Property_Strength:
                            CurRecommendAdd.Four += 1;
                            break;
                        case E_GameProperty.Property_Willpower:
                            CurRecommendAdd.One += 1;
                            break;
                        case E_GameProperty.Property_Agility:
                            CurRecommendAdd.Two += 1;
                            break;
                        case E_GameProperty.Property_BoneGas:
                            CurRecommendAdd.Three += 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case E_RoleType.Swordsman:
                    switch (e_GameProperty)
                    {
                        case E_GameProperty.Property_Strength:
                            CurRecommendAdd.One += 1;
                            break;
                        case E_GameProperty.Property_Willpower:
                            CurRecommendAdd.Four += 1;
                            break;
                        case E_GameProperty.Property_Agility:
                            CurRecommendAdd.Two += 1;
                            break;
                        case E_GameProperty.Property_BoneGas:
                            CurRecommendAdd.Three += 1;
                            break;
                        default:
                            break;
                    }
                    break;
                case E_RoleType.Archer:
                    switch (e_GameProperty)
                    {
                        case E_GameProperty.Property_Strength:
                            CurRecommendAdd.Two += 1;
                            break;
                        case E_GameProperty.Property_Willpower:
                            CurRecommendAdd.Four += 1;
                            break;
                        case E_GameProperty.Property_Agility:
                            CurRecommendAdd.One += 1;
                            break;
                        case E_GameProperty.Property_BoneGas:
                            CurRecommendAdd.Three += 1;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            RefreshRolePropertyAddPoint(CurRecommendAdd);
        }
        public void SetChildAddPoint(Transform trs, E_GameProperty property, RoleAttribute roleAttribute)
        {
            switch (property)
            {
                case E_GameProperty.Property_Strength:
                    trs.Find("addvalue").GetComponent<Text>().text = $" +{roleAttribute.Property_Strength}";
                    RecommendAddPointDic.Add(trs, roleAttribute.Property_Strength.ToInt32());
                    break;
                case E_GameProperty.Property_Willpower:
                    trs.Find("addvalue").GetComponent<Text>().text = $" +{roleAttribute.Property_Intelligence}";
                    RecommendAddPointDic.Add(trs, roleAttribute.Property_Intelligence.ToInt32());
                    break;
                case E_GameProperty.Property_Agility:
                    trs.Find("addvalue").GetComponent<Text>().text = $" +{roleAttribute.Property_Agility}";
                    RecommendAddPointDic.Add(trs, roleAttribute.Property_Agility.ToInt32());
                    break;
                case E_GameProperty.Property_BoneGas:
                    trs.Find("addvalue").GetComponent<Text>().text = $" +{roleAttribute.Property_PhysicalStrength}";
                    RecommendAddPointDic.Add(trs, roleAttribute.Property_PhysicalStrength.ToInt32());
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取加点面板玩家的属性
        /// </summary>
        public void RefreshRolePropertyAddPoint(RecommendAddPoint recommendAdd)
        {
            //Log.Info("RefreshRoleProperty-------------------- 1");
            int pointCount = recommendAdd.One + recommendAdd.Two + recommendAdd.Three + recommendAdd.Four;
            //Log.Info("RefreshRoleProperty-------------------- pointCount " + pointCount);
            freePoint = roleEntity.Property.GetProperValue(E_GameProperty.FreePoint) - pointCount;
            //Log.Info("RefreshRoleProperty-------------------- freePoint " + freePoint);
            CanUsePointTxt.text = freePoint.ToString();//等级点数

            var infoObj = GetRoleInfoAddPointPanel();
            UIRoleInfoData.GetAttribute(recommendAdd, out roleAttribute);

            RecommendAddPointDic.Clear();
            for (int i = 0, length = infoObj.transform.childCount; i < length; i++)
            {
                var item = infoObj.transform.GetChild(i);
                if (item.name.Contains("_Add"))
                {
                    E_GameProperty property = (E_GameProperty)int.Parse(item.name.Split('_')[1]);
                    SetChildAddPoint(item, roleEntity.Property.GetProperValue(property).ToString());
                    SetChildAddPoint(item, property, roleAttribute);
                }
                else
                {
                    int propertyId = int.Parse(item.name.Split('_')[2]);
                    Text textvalue = item.Find("value").GetComponent<Text>();
                    Text addvalue = item.Find("addvalue").GetComponent<Text>();
                    switch (propertyId)
                    {
                        case 22://攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.MinAtteck)} ~ {roleEntity.Property.GetProperValue(E_GameProperty.MaxAtteck)}";//攻击力
                            addvalue.text = roleAttribute.Property_Atteck;
                            break;
                        case 27://攻击成功率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.AtteckSuccessRate)}";//攻击成功率
                            addvalue.text = roleAttribute.AtteckSuccessRate;
                            break;
                        case 28://PVP攻击率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.PVPAtteckSuccessRate)}";//PK攻击成功率
                            addvalue.text = roleAttribute.PVPSuccessRate;
                            break;
                        case 35://防御力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.Defense)}";//防御力）
                            addvalue.text = roleAttribute.DefensivePower;
                            break;
                        case 37://攻击速率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.AttackSpeed)}/{GetMaxAttackSpeed()}";//攻击速率
                            if (roleAttribute.AttackRate == " +0")
                            {
                                addvalue.text = $"{roleAttribute.AttackRate}";
                            }
                            else
                            {
                                addvalue.text = $"{roleAttribute.AttackRate}/{GetMaxAttackSpeed()}";
                            }
                            break;
                        case 31://防御率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.DefenseRate)}";//防御率
                            addvalue.text = roleAttribute.DefenseRate;
                            break;
                        case 33://PVP防御率
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.PVPDefenseRate)}";//PVP防御率
                            addvalue.text = roleAttribute.PVPDefenseRate;
                            break;
                        case 42://技能攻击力
                            textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.SkillAddition)}%";//技能攻击力%
                            addvalue.text = roleAttribute.SkillAttack;
                            break;
                        case 24://魔力
                            Log.DebugGreen($"魔力百分比：{roleEntity.Property.GetProperValue(E_GameProperty.MagicRate_Increase)}");
                            addvalue.text = roleAttribute.Property_Magic;
                            if (roleEntity.Property.GetProperValue(E_GameProperty.MagicRate_Increase) is long value && value != 0)
                            {
                                textvalue.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.MinMagicAtteck)}~{roleEntity.Property.GetProperValue(E_GameProperty.MaxMagicAtteck)}\t\t\t(+{roleEntity.Property.GetProperValue(E_GameProperty.MaxMagicAtteck) * value / 100})";//魔力%
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


    }

}
