//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using ETModel;
using NPOI.Util;
using NPOI.SS.Formula.Functions;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_LoadingComplete_notice_Handler : AMHandler<G2C_LoadingComplete>
    {
        protected override void Run(ETModel.Session session, G2C_LoadingComplete message)
        {
            LoginStageTrace.Append(
                $"LoadingComplete notice received scene={SceneComponent.Instance?.CurrentSceneName} " +
                $"loadRole={RoleArchiveInfoManager.Instance.LoadRoleUUID} " +
                $"localRole={(UnitEntityComponent.Instance?.LocalRole != null)} " +
                $"processed={GlobalDataManager.EnterGameLoadingCompleteProcessed}");
            Log.DebugBrown("G2C_LoadingComplete==>" + JsonHelper.ToJson(message));
            if (GlobalDataManager.EnterGameLoadingCompleteProcessed)
            {
                LoginStageTrace.Append("LoadingComplete duplicate skip-init");
                EnsureGameplayCameraFollow();
                return;
            }

            GlobalDataManager.EnterGameLoadingCompleteProcessed = true;
            EnsureGameplayCameraFollow();
            TraceWorldState().Coroutine();
            //只有魔法师，剑士，弓箭手才有新手引导
            if(UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magician || UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Swordsman || UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer)
                GetRoleProperties().Coroutine();
            WarAlliance().Coroutine();
          //  GetPetAddPoint().Coroutine();
            GetBeginnerGuideStatus().Coroutine();
            GetFriendInfo().Coroutine();
            RemoteOpen().Coroutine();

            //获取宠物是否有可用加点数
            async ETVoid GetPetAddPoint()
            {

                int count = 0;
                G2C_OpenPetsInterfaceResponse g2C_OpenPets = (G2C_OpenPetsInterfaceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenPetsInterfaceRequest() { });
                if (g2C_OpenPets.Error == 0)
                {
                    //有宠物----第一个显示的宠物
                    if (g2C_OpenPets.Current?.PetsConfigID != 0)
                    {
                        count += g2C_OpenPets.Current.PetsLVpoint;
                    }

                    for (int i = 0, length = g2C_OpenPets.List.Count; i < length; i++)
                    {
                        count += g2C_OpenPets.List[i].Point;
                    }
                    if (count > 0)
                    {
                        RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Pet, 1);
                        UIMainComponent.Instance.RedDotFriendCheack();
                    }
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
                }
            }

            //获取玩家是否有可用加点数
            async ETVoid GetRoleProperties()
            {
                G2C_PlayerPropertyResponse c2G_Player = (G2C_PlayerPropertyResponse)await SessionComponent.Instance.Session.Call(new C2G_PlayerPropertyRequest { SelectId = 0 });
                if (c2G_Player.Error != 0)
                {
                   // Log.DebugRed($"获取玩家属性报错：{c2G_Player.Error.GetTipInfo()}");
                }
                else
                {
                    foreach (G2C_BattleKVData item in c2G_Player.Info)
                    {
                        // Log.DebugBrown($"玩家属性：{item.Key} -> {item.Value}");
                        UnitEntityComponent.Instance.LocalRole.Property.Set(item);
                        UnitEntityComponent.Instance.LocalRole.Reincarnation = c2G_Player.ReincarnateCnt;
                        UnitEntityComponent.Instance.LocalRole.GetComponent<UIUnitEntityHpBarComponent>().SetReincarnation();
                        if ((E_GameProperty)item.Key == E_GameProperty.FreePoint)
                        {
                            // Log.DebugGreen($"等级点数：{item.Value}");
                            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.FreePoint, item.Value);

                            if (item.Value > 0)
                            {
                                UIRoleInfoData.RecommendAddPointInit();
                                while (UIMainComponent.Instance == null)
                                {
                                    if (UIMainComponent.Instance != null)
                                    {
                                        UIMainComponent.Instance.SetArributeRedDot(true); 
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                UIMainComponent.Instance.SetArributeRedDot(false);
                            }
                            return;

                        }
                    }
                }


            }

            //判断是否有战盟信息
            async ETVoid WarAlliance()
            {
                G2C_OpenWarAllianceResponse g2C_Open = (G2C_OpenWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenWarAllianceRequest { });
               
                if (g2C_Open.Error != 0)
                {
                    //当前没有战盟
                    WarAllianceDatas.IsJoinWar = false;
                }
                else
                {
                    WarAllianceDatas.IsJoinWar = true;
                    GetWarAlianceInfo().Coroutine();
                }
            }

            //缓存战盟成员信息
            async ETVoid GetWarAlianceInfo()
            {
                G2C_OpenMemberListResponse g2C_OpenMember = (G2C_OpenMemberListResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMemberListRequest
                {
                    Type = 0//0是成员，1是申请列表
                });
                if (g2C_OpenMember.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMember.Error.GetTipInfo());
                }
                else
                {
                    WarAllianceDatas.Clear();
                    foreach (var item in g2C_OpenMember.MemberList)
                    {
                        var info = new WarMemberInfo
                        {
                            Name = item.MemberName,
                            UUID = item.GameUserID,
                            Lev = item.MemberLevel,
                            Post = item.MemberPost,
                            State = item.MeberState
                        };
                        WarAllianceDatas.WarMemberList.Add(info);
                    };
                }
            }

            //获取引导值
            async ETVoid GetBeginnerGuideStatus()
            {
                G2C_GetBeginnerGuideStatus g2C_GetBeginner = (G2C_GetBeginnerGuideStatus)await SessionComponent.Instance.Session.Call(new C2G_GetBeginnerGuideStatus{});
                if(g2C_GetBeginner.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetBeginner.Error.GetTipInfo());
                }
                else
                {
                    //BeginnerGuideData.BeginnerGuideSata = g2C_GetBeginner.Value;
                    //BeginnerGuideData.BeginnerGuideCountTime = true;
                    //Log.Info(JsonHelper.ToJson(g2C_GetBeginner));
                    GuideComponent.Instance.InitData(g2C_GetBeginner.Value);
                    GuideComponent.Instance.CheckIsShowGuide();

                    //UIMainComponent.Instance.cheakBeginner();
                    //if (UIMainComponent.Instance != null)
                    //{
                    //    //Log.DebugGreen($"1服务器返回的引导值{g2C_GetBeginner.Value}");
                    //    UIMainComponent.Instance.SetBeginnerGuide(BeginnerGuideData.IsCompleteTrigger(46, 45) || BeginnerGuideData.IsCompleteTrigger(49, 45) || BeginnerGuideData.IsCompleteTrigger(54, 53) || BeginnerGuideData.IsCompleteTrigger(59, 58));
                    //    return;
                    //}
                    //while (UIMainComponent.Instance == null)
                    //{
                    //    if(UIMainComponent.Instance != null)
                    //    {
                    //        //Log.DebugGreen($"2服务器返回的引导值{g2C_GetBeginner.Value}");
                    //        UIMainComponent.Instance.SetBeginnerGuide(BeginnerGuideData.IsCompleteTrigger(46, 45) || BeginnerGuideData.IsCompleteTrigger(49, 45) || BeginnerGuideData.IsCompleteTrigger(54, 53) || BeginnerGuideData.IsCompleteTrigger(59, 58));
                    //        break;
                    //    }
                    //}
                }
            }
            
            //获取缓存好友信息
            async ETVoid GetFriendInfo()
            {
                G2C_OpenFriendsinterfaceResponse g2C_Open = (G2C_OpenFriendsinterfaceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenFriendsinterfaceRequest()
                {
                    ListType = 4//列表
                });
                if (g2C_Open.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_Open.Error.GetTipInfo()}");
                }
                FriendListData.FriendList.Clear();
                foreach (var item in g2C_Open.FList)
                {
                    //Log.DebugGreen($"好友名字：{item.CharName},战盟名字：{item.WarAllianceName}，战盟职位：{item.WarAlliancePost}，职业：{((E_RoleType)item.ClassType).ToString()}");
                    if (!FriendListData.FriendList.Exists(f => f.NickName == item.CharName))
                    {
                        //Log.Debug("item.CharName:" + item.CharName);
                        FriendInfo friendInfo = new FriendInfo()
                        {
                            NickName = item.CharName,
                            UUID = item.GameUserId,
                            Zhanmeng = item.WarAllianceName,
                            Identity = !string.IsNullOrEmpty(item.WarAllianceName) ? GetPos(item.WarAlliancePost) : string.Empty,
                            Level = item.ILV,
                            Job = ((E_RoleType)item.ClassType).GetRoleName(0),
                            TimeDate = item.TimeDate,
                            State = item.IState == 0 ? "在线" : "离线",
                            isChoose = false
                        };
                        FriendListData.FriendList.Add(friendInfo);
                    }
                }
                string GetPos(int post) => post switch
                {
                    0 => "成员",
                    1 => "小队长",
                    2 => "副盟主",
                    3 => "盟主",
                    _ => "成员"
                };
            }

            async ETVoid RemoteOpen()
            {
                if (UnitEntityComponent.Instance.LocalRole.MinMonthluCardTimeSpan.TotalSeconds <= 0 && UnitEntityComponent.Instance.LocalRole.MaxMonthluCardTimeSpan.TotalSeconds <= 0 && !TitleManager.allTitles.Exists(x => x.TitleId == 60005))
                    return;
                G2C_RemoteOpenResponse g2C_RemoteOpen = (G2C_RemoteOpenResponse)await SessionComponent.Instance.Session.Call(new C2G_RemoteOpenRequest
                {
                    Type = 0
                });
                if (g2C_RemoteOpen.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RemoteOpen.Error.GetTipInfo());
                    //Log.DebugGreen($"g2C_RemoteOpen.Error:{g2C_RemoteOpen.Error}");
                }
                else
                {
                    RecycleEquipTools.CurNpcUUid = g2C_RemoteOpen.NpcId;
                }
            }

            void EnsureGameplayCameraFollow()
            {
                CameraFollowComponent followComponent = CameraFollowComponent.Instance;
                RoleEntity localRole = UnitEntityComponent.Instance?.LocalRole;
                Transform followTarget = localRole?.roleTrs;
                string followSource = "roleTrs";

                if (followTarget == null && localRole?.Game_Object != null)
                {
                    followTarget = localRole.Game_Object.transform.parent;
                    followSource = "Game_Object.parent";
                }

                if (followTarget == null && localRole?.GameObject != null)
                {
                    followTarget = localRole.GameObject.transform;
                    followSource = "Component.GameObject";
                }

                if (followComponent == null || localRole == null || followTarget == null)
                {
                    LoginStageTrace.Append(
                        $"LoadingComplete camera follow skipped " +
                        $"follow={followComponent != null} role={localRole != null} " +
                        $"roleTrs={localRole?.roleTrs != null} gameObject={localRole?.GameObject != null} model={localRole?.Game_Object != null}");
                    return;
                }

                followComponent.followTarget = followTarget;
                followComponent.ChangeScene = true;
                LoginStageTrace.Append(
                    $"LoadingComplete camera follow refreshed " +
                    $"source={followSource} targetPos={followTarget.position.x:F2},{followTarget.position.y:F2},{followTarget.position.z:F2} " +
                    $"h={followComponent.curAngleH} v={followComponent.curAngleV} d={followComponent.distance}");
            }

            async ETVoid TraceWorldState()
            {
                LoginStageTrace.AppendWorldSnapshot("LoadingComplete immediate");
                await TimerComponent.Instance.WaitAsync(200);
                LoginStageTrace.AppendWorldSnapshot("LoadingComplete +200ms");
                await TimerComponent.Instance.WaitAsync(1000);
                LoginStageTrace.AppendWorldSnapshot("LoadingComplete +1200ms");
                await TimerComponent.Instance.WaitAsync(3000);
                LoginStageTrace.AppendWorldSnapshot("LoadingComplete +4200ms");
            }

        }
    }
}
