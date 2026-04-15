using ETModel;

using UnityEngine;
using static UnityEditor.Progress;

namespace ETHotfix
{

    /// <summary>
    /// 实体属性 变动
    /// </summary>
    [MessageHandler]
    public class G2C_ChangeValue_notice_Handler : AMHandler<G2C_ChangeValue_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ChangeValue_notice message)
        {
            // message.GameUserId == 0 为本地玩家 
            UnitEntity unitEntity = message.GameUserId == 0 ? UnitEntityComponent.Instance.LocalRole : UnitEntityComponent.Instance.Get<UnitEntity>(message.GameUserId);
            if (unitEntity == null) return;

            for (int i = 0, length = message.Info.Count; i < length; i++)
            {
                G2C_BattleKVData itemdata = message.Info[i];
                // Log.DebugWhtie($"itemdata:{itemdata.Key} -> {itemdata.Value}");
                unitEntity.Property.Set(itemdata);
                if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magician || UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Swordsman || UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer)
                    if ((E_GameProperty)itemdata.Key == E_GameProperty.Level && message.GameUserId == 0)
                    {
                        if (UnitEntityComponent.Instance.LocalRole.Level != 0)
                        {
                            UIRoleInfoData.RecommendAddPointInit();
                        }
                    }
            }
            for (int i = 0, length = message.Info.Count; i < length; i++)
            {
                G2C_BattleKVData itemdata = message.Info[i];
                // Log.DebugWhtie($"itemdata:{itemdata.Key} -> {itemdata.Value}");
                ///当前 打怪所获得的经验值
                if ((E_GameProperty)itemdata.Key == E_GameProperty.Exprience)
                {



                    // if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                    {
                        UIMainComponent.Instance.UpdateExperience();//更新 玩家的 经验条
                    }
                }

                if ((E_GameProperty)itemdata.Key == E_GameProperty.ExprienceDrop)
                {

                    //if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                    if (itemdata.Value >= 0)
                    {
                        //更新每秒经验
                        if (unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                        {

                            UIMainComponent.Instance?.ChangeGetExp(itemdata.Value);
                        }

                        UIMainComponent.Instance.ShowInfo($"<color=#00A1FF>你得到 {itemdata.Value} 经验</color>");
                    }
                    else
                    {
                        if (itemdata.Value != 0)
                            UIMainComponent.Instance.ShowInfo($"<color=red>你损失 {-itemdata.Value} 经验</color>");
                    }
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.FreePoint)
                {
                    UIMainComponent.Instance.SetArributeRedDot(itemdata.Value > 0);
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.MoJing)
                {
                    // Log.DebugBrown($"魔晶变化：{itemdata.Value}");
                    //更新抽奖界面的魔晶数
                    if (UIComponent.Instance.Get(UIType.UIChouJiang)?.GetComponent<UIChouJiangComponent>() is UIChouJiangComponent uIChouJiangComponent)
                    {
                        uIChouJiangComponent.UpdateMoJing();
                    }
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.Level && message.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    UIMainComponent.Instance.SetLV(itemdata.Value);
                    if (ETModel.Init.instance.e_SDK == E_SDK.XY_SDK)
                    {
                        //角色升级：角色升级时调用
                        SdkCallBackComponent.Instance.sdkUtility.UpdateRoleGrade(new string[] { $"{GlobalDataManager.XYUUID}", $"{GlobalDataManager.EnterZoneID}", $"{GlobalDataManager.EnterZoneName}", $"{UnitEntityComponent.Instance.LocaRoleUUID}", $"{UnitEntityComponent.Instance.LocalRole.RoleName}", $"{itemdata.Value}", "" });
                    }
                    else if (ETModel.Init.instance.e_SDK == E_SDK.SHOU_Q)
                    {
                        SdkCallBackComponent.Instance.sdkUtility.UploadRoleInfo(new string[] { $"{UnitEntityComponent.Instance.LocalRole.RoleName}", $"{GlobalDataManager.EnterZoneID}_{GlobalDataManager.EnterLineID}", $"{itemdata.Value}", $"{GlobalDataManager.EnterZoneName}", $"{UnitEntityComponent.Instance.LocaRoleUUID}", $"{GlobalDataManager.ShouQUUID}", "" });
                    }
                    else if (ETModel.Init.instance.e_SDK == E_SDK.HaXi)
                    {
                        Log.Info("-------------------------------ChangeValue-------------- ");

                        SdkCallBackComponent.Instance.sdkUtility.UploadRoleInfo(new string[] {
                        UnitEntityComponent.Instance.LocalRole.RoleName, // 角色名
                        UnitEntityComponent.Instance.LocalRole.Level.ToString(),// 角色等级
                        GlobalDataManager.EnterZoneID.ToString(),// 服务器ID
                        GlobalDataManager.EnterZoneName,// 服务器名
                        UnitEntityComponent.Instance.LocaRoleUUID.ToString(),// 角色ID
                        UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing).ToString(),// 角色账户余额
                        "0",// 角色创建时间(秒)
                        "0", // 帮派/工会ID
                        "0",  // 帮派/工会
                        "0",    // 角色VIP等级
                        "0",     // 战斗力
                        UnitEntityComponent.Instance.LocalRole.ClassLev.ToString(),    // 转生等级
                        UnitEntityComponent.Instance.LocalRole.RoleType.ToString(),    // 角色职业
                        });
                    }
                    UIMainComponent.Instance.SetLV(itemdata.Value);

                    //string value = null;
                    //int level = 0;
                    //Log.DebugGreen($"等级 {value}");
                    //if (itemdata.Value == 220 || itemdata.Value == 250 || itemdata.Value == 280 || itemdata.Value == 300 ||
                    //    itemdata.Value == 320 || itemdata.Value == 350 || itemdata.Value == 400)
                    //{
                    //    if (PlayerPrefs.GetInt(value) == 0)
                    //    {
                    //        UIComponent.Instance.VisibleUI(UIType.UILevelTopUp, itemdata.Value, value);
                    //    }
                    //}


                    /* if (itemdata.Value >= 220 && itemdata.Value < 250)
                     {
                         level = 220;
                         value = 220.ToString() + ',' + UnitEntityComponent.Instance.LocaRoleUUID.ToString();

                     }else if (itemdata.Value >= 250 && itemdata.Value < 280)
                     {
                         level = 250;
                         value = 250.ToString() + ',' + UnitEntityComponent.Instance.LocaRoleUUID.ToString();
                     }else if (itemdata.Value >= 280 && itemdata.Value < 300)
                     {
                         level = 280;
                         value = 280.ToString() + ',' + UnitEntityComponent.Instance.LocaRoleUUID.ToString();
                     }else if (itemdata.Value >= 300 && itemdata.Value < 320)
                     {
                         level = 300;
                         value = 300.ToString() + ',' + UnitEntityComponent.Instance.LocaRoleUUID.ToString();
                     }else if (itemdata.Value >= 320 && itemdata.Value < 350)
                     {
                         level = 320;
                         value = 320.ToString() + ',' + UnitEntityComponent.Instance.LocaRoleUUID.ToString();
                     }else if (itemdata.Value >= 350 && itemdata.Value < 400)
                     {
                         level = 350;
                         value = 350.ToString() + ',' + UnitEntityComponent.Instance.LocaRoleUUID.ToString();
                     }else if (itemdata.Value >= 400)
                     {
                         level = 400;
                         value = 400.ToString() + ',' + UnitEntityComponent.Instance.LocaRoleUUID.ToString();
                     }
                     value += "change1";
                     if (level != 0)
                     {
                         if (PlayerPrefs.GetInt(value + "弹窗1") == 0)
                         {
                             PlayerPrefs.SetInt(value + "弹窗1",1);
                             UIComponent.Instance.VisibleUI(UIType.UILevelTopUp, level, value);
                         }
                     }*/

                    //if (itemdata.Value == 2 || itemdata.Value == 3 || itemdata.Value == 4 || itemdata.Value == 5 || 
                    //    itemdata.Value == 6 || itemdata.Value == 7 || itemdata.Value == 8)
                    //{
                    //    if (PlayerPrefs.GetInt(value) == 0)
                    //    {
                    //        UIComponent.Instance.VisibleUI(UIType.UILevelTopUp, itemdata.Value);
                    //    }
                    //}

                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.PROP_MP || (E_GameProperty)itemdata.Key == E_GameProperty.PROP_MP_MAX)
                {
                    //  Log.DebugWhtie($"当前魔力值：{itemdata.Value}");
                    // if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                    UIMainComponent.Instance.ChangeRoleMp();
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.PROP_HP || (E_GameProperty)itemdata.Key == E_GameProperty.PROP_HP_MAX)
                {
                    // Log.DebugWhtie($"当前生命值：{itemdata.Value}  {unitEntity.Game_Object.name}");
                    //  if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                    UIMainComponent.Instance.ChangeRoleHp();
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.PROP_AG || (E_GameProperty)itemdata.Key == E_GameProperty.PROP_AG_MAX)
                {
                    // Log.DebugWhtie($"当前AG：{itemdata.Value}  {UIMainComponent.Instance == null}");
                    //if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                    UIMainComponent.Instance.ChangeAG();
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.PROP_SD || (E_GameProperty)itemdata.Key == E_GameProperty.PROP_SD_MAX)
                {
                    // Log.DebugWhtie($"当前SD：{itemdata.Value}  {UIMainComponent.Instance == null}");
                    //if (UIComponent.Instance.Get(UIType.UIMainCanvas)?.GetComponent<UIMainComponent>() is UIMainComponent mainComponent)
                    UIMainComponent.Instance.ChangeSD();
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.PlayerKillingMedel)//pk模式
                {
                    //  Log.DebugWhtie($"当前pk模式：{itemdata.Value}");
                    GlobalDataManager.BattleModel = (E_BattleType)itemdata.Value;

                    UIMainComponent.Instance?.ChangePkState((int)itemdata.Value);
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.PkNumber)//PK点数
                {
                    //  Log.DebugRed($"PK点数:{itemdata.Value}  {UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.PkNumber)}  {unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID}");
                    //红名时间->每分钟回复120点惩罚点数
                    if (itemdata.Value != 0)
                    {
                        if (unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                        {
                            //本地玩家
                            if (UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.PkNumber) <= 0)
                            {
                                UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.PkNumber, itemdata.Value);
                                UIMainComponent.Instance.PkNumber();
                            }

                        }

                        //改变角色名字颜色
                        if (unitEntity is RoleEntity role)
                            role.GetComponent<UIUnitEntityHpBarComponent>().ChangeNameColor(role.GetRedNameColor());

                    }
                }
                else if ((E_GameProperty)itemdata.Key == E_GameProperty.Pet_FangYuHuZhao)//pk模式
                {
                    //无防护罩
                    if (unitEntity is PetEntity petEntity)
                    {
                        GameObject obj = petEntity.Game_Object.transform.Find("Pet_ShuangTouLong_Skill_01").gameObject;
                        if (obj.activeSelf)
                        {
                            if (itemdata.Value == 0)
                            {
                                obj.SetActive(false);
                            }
                        }
                        else
                        {
                            if (itemdata.Value != 0)
                            {
                                obj.SetActive(true);
                            }
                        }
                    }
                    else if (unitEntity is RoleEntity roleEntity)
                    {
                        //Log.DebugGreen($"人物是否有护盾{itemdata.Value == 0} 护盾值{itemdata.Value}");
                        if (itemdata.Value == 0)
                        {
                            if (roleEntity.Game_Object != null)
                            {
                                GameObject skillObj = roleEntity.Game_Object?.transform.Find("Pet_ShuangTouLong_Skill_01")?.gameObject;
                                if (skillObj != null)
                                {
                                    ResourcesComponent.Instance.DestoryGameObjectImmediate(skillObj, skillObj.name.StringToAB());
                                }
                            }
                        }
                        else
                        {
                            if (roleEntity.Game_Object != null)
                            {
                                if (roleEntity.Game_Object.transform.Find("Pet_ShuangTouLong_Skill_01") == null)
                                {
                                    GameObject skillObj = ResourcesComponent.Instance.LoadGameObject("Pet_ShuangTouLong_Skill_01".StringToAB(), "Pet_ShuangTouLong_Skill_01");
                                    skillObj.transform.parent = roleEntity.Game_Object.transform;
                                    skillObj.transform.localPosition = Vector3.up;
                                }
                            }
                        }
                    }
                }
                ///时间 差值
                if ((E_GameProperty)itemdata.Key == E_GameProperty.ServerTime)
                {
                    GlobalDataManager.ServerTime = itemdata.Value - TimeHelper.Now();
                }
            }
        }
    }
}