using System.Collections.Generic;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// 战盟 所以使用的协议接口
    /// </summary>

    public partial class UIWarAllianceComponent
    {
        bool Isremoved = false;
        /// <summary>
        /// 请求 创建 战盟
        /// </summary>
        /// <returns></returns>

        public async ETVoid CreatWar()
        {
            C2G_WarAllianceEstablishRequest c2G_War = new C2G_WarAllianceEstablishRequest();
            c2G_War.WarAllianceName = warName;
            c2G_War.WarAllianceBadge.AddRange(badgeIcon);
            G2C_WarAllianceEstablishResponse g2C_War = (G2C_WarAllianceEstablishResponse)await SessionComponent.Instance.Session.Call(c2G_War);
            
            if (g2C_War.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_War.Error.GetTipInfo());
            }
            else
            {
                WarAllianceDatas.IsJoinWar = true;
                Init_Info(g2C_War.Info,1);
            }
        }

        /// <summary>
        /// 打开战盟
        /// </summary>
        /// <returns></returns>
        public async ETVoid OpenWarAsync()
        {
            G2C_OpenWarAllianceResponse g2C_Open = (G2C_OpenWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenWarAllianceRequest 
            {
                
            });
            if (g2C_Open.Error != 0)
            {
                //当前没有战盟
                Show(E_WarType.Init);
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Open.Error.GetTipInfo());
            }
            else
            {
                Init_Info(g2C_Open.Info, g2C_Open.MemberListCont);
            }
        }
        /// <summary>
        /// 请求 已有的战盟列表
        /// </summary>
        /// <returns></returns>

        public async ETVoid JoinWarAsync(int type) 
        {
            G2C_AddWarAllianceListResponse g2C_AddWar = (G2C_AddWarAllianceListResponse)await SessionComponent.Instance.Session.Call(new C2G_AddWarAllianceListRequest 
            {
                Type = type,
            });
            if (g2C_AddWar.Error!= 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddWar.Error.GetTipInfo());
            }
            else
            {
                //显示战盟列表
                List<WarInfo> WarLists = new List<WarInfo>();
                WarLists.Clear();
                foreach (Struct_WarAllinceInfo item in g2C_AddWar.WAInfo)
                {
                    WarLists.Add(new WarInfo
                    {
                        struct_WarAllince = item,
                        State = g2C_AddWar.List.Equals(item.WarAllianceID) ? 1 : 0
                        //WarAllianceDatas.WarLists.Exists(r => r.struct_WarAllince.WarAllianceID == item.WarAllianceID) ? WarAllianceDatas.WarLists.Find(r => r.struct_WarAllince.WarAllianceID == item.WarAllianceID).State : 0

                    });
                }
                WarAllianceDatas.WarLists.Clear();
                WarAllianceDatas.WarLists = WarLists;
                WarLists = null;


                ////根据战盟 人数 降序
                //WarAllianceDatas.WarLists.Sort((m1, m2) =>
                //{
                //    return m2.struct_WarAllince.WarAllianceLevel.CompareTo(m1.struct_WarAllince.WarAllianceLevel);
                //});

                //显示 战盟列表
                WarJoinInfoScrollView.Items = WarAllianceDatas.WarLists;
            }
        }
        /// <summary>
        /// 搜索战盟 
        /// </summary>
        /// <param name="warName">被搜索的战盟名字</param>
        /// <returns></returns>
        public async ETVoid SearchWar(string warName) 
        {
            G2C_SearchWarAllianceResponse g2C_SearchWar = (G2C_SearchWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_SearchWarAllianceRequest { WarAllianceName=warName});
            if (g2C_SearchWar.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SearchWar.Error.GetTipInfo());
            }
            else
            {
                SearchWarInfoList.Clear();
                SearchWarInfoList.Add(new WarInfo 
                {
                 struct_WarAllince=new Struct_WarAllinceInfo 
                 {
                  WarAllianceID=g2C_SearchWar.WarAllianceID,//战盟 id
                  WarAllianceName=g2C_SearchWar.WarAllianceName,//战盟 名字
                  WarAllianceLevel=g2C_SearchWar.WarAllianceLevel,//战盟等级
                  LeaderName = g2C_SearchWar.AllianceLeaderName//盟主名字
                 },
                 State=0
                });
                WarJoinInfoScrollView.Items = SearchWarInfoList;
            }
        }

        /// <summary>
        /// 申请 加入战盟
        /// </summary>
        /// <param name="warUUID">战盟的UUID</param>
        /// <returns></returns>
        public async ETVoid RequestAddWarAsync(long warUUID)
        {
            G2C_AddWarAllianceResponse g2C_AddWar = (G2C_AddWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_AddWarAllianceRequest 
            {
             WarAllianceID=warUUID
            });
            if (g2C_AddWar.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddWar.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "已申请");
            }
        }


        /// <summary>
        /// 解散战盟
        /// </summary>
        /// <returns></returns>
        public async ETVoid DisbandTheWarAsync()
        {
            G2C_DisbandTheWarAllianceResponse g2C_Disband = (G2C_DisbandTheWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_DisbandTheWarAllianceRequest { });
            if (g2C_Disband.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Disband.Error.GetTipInfo());
            }
            else
            {
                WarAllianceDatas.IsJoinWar = false;
                UIComponent.Instance.VisibleUI(UIType.UIHint,"战盟已解散");
                UIComponent.Instance.Remove(UIType.UIWarAlliance);
                roleEntity.unionName = string.Empty;
            }
        }
        /// <summary>
        /// 打开成员列表
        /// 0 -》成员列表
        /// 1-》申请列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ETVoid OpenMemberListAsync(int type)
        {
            G2C_OpenMemberListResponse g2C_OpenMember = (G2C_OpenMemberListResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMemberListRequest
            {
                Type = type
            });
            if (g2C_OpenMember.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMember.Error.GetTipInfo());
            }
            else
            {
                if (type == 0)//成员列表
                {
                    WarAllianceDatas.Clear();
                    warMemberInfos.Clear();
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
                        warMemberInfos.Add(info);
                        WarAllianceDatas.WarMemberList.Add(info);
                    };
                    // 先按 状态是否在线（在线、离线）排序  如果状态相同再按职位 （盟主、副盟主、小队长、成员排序  最后按等级高低 排序
                    warMemberInfos.Sort((m1, m2) =>
                    {
                        // return m2.State.CompareTo(m1.State);//降序 （先显示 在线的成员 再显示不在先的成员）
                        int x = m1.State.CompareTo(m2.State);
                        if (x == 0)
                        {
                            if (m2.Post > m1.Post)
                                x = 1;
                            else if (m2.Post == m1.Post)
                            {
                                x = m2.Lev.CompareTo(m1.Lev);
                                // x = 0;
                            }
                            else
                                x = -1;
                        }
                        return x;
                    });
                    WarMemberInfoScrollView.Items = warMemberInfos;
                }
                else if (type == 1)
                {
                    WarApplyInfos.Clear();

                    foreach (var item in g2C_OpenMember.MemberList)
                    {
                        WarApplyInfos.Add(new WarMemberInfo
                        {
                            Name = item.MemberName,
                            UUID = item.GameUserID,
                            Lev = item.MemberLevel,
                            Post = item.MemberPost,
                            State = item.MeberState
                        });
                    };
                    WarApplyInfoScrollView.Items = WarApplyInfos;
                }
            }
        }
        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>

        public async ETVoid ChangeAnnoAsync(string str)
        {
            G2C_ModifyAnnouncementResponse g2C_Modify = (G2C_ModifyAnnouncementResponse)await SessionComponent.Instance.Session.Call(new C2G_ModifyAnnouncementRequest
            {
                Notice = str
            });
            if (g2C_Modify.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Modify.Error.GetTipInfo());
            }
            else
            {
          
                UIComponent.Instance.VisibleUI(UIType.UIHint, "公告修改完成");
                isEditorAnno = false;
            }
        }

        /// <summary>
        /// 任命职务
        /// </summary>
        /// <returns></returns>
        public async ETVoid WarAllianceAppointmentAsync(long uid, int type)
        {
            G2C_WarAllianceAppointmentResponse g2C_WarAlliance = (G2C_WarAllianceAppointmentResponse)await SessionComponent.Instance.Session.Call(new C2G_WarAllianceAppointmentRequest
            {
                GameUserID = uid,
                ClassType = type
            });
            if(g2C_WarAlliance.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_WarAlliance.Error.GetTipInfo());

            }
            else
            {
                if (type == 3)
                {
                    UIComponent.Instance.InVisibilityUI(UIType.UIWarAlliance);
                }
                OpenMemberListAsync(0).Coroutine();
            }
        }
        public void WarBeAppointed(int type, string Name)
        {
            if (Members.activeSelf)
            {
                if (Isremoved)
                {
                    //撤职
                    var member = WarMemberInfoScrollView.Items.Find(r => r.Name == Name);
                    member.Post = 0;
                    CurChooseWarMemberTrsAll.Find(e => SplitString(e.Find("Name")?.GetComponent<Text>().text) == Name).
                        transform.Find("Job").GetComponent<Text>().text = GetPos(0);
                }
                else
                {
                    var member = WarMemberInfoScrollView.Items.Find(r => r.Name == Name);
                    member.Post = type;
                    CurChooseWarMemberTrsAll.Find(e => SplitString(e.Find("Name")?.GetComponent<Text>().text) == Name).
                        transform.Find("Job").GetComponent<Text>().text = GetPos(type);
                    PostSetPanel.SetActive(false);
                }
            }  
        }
        
        public string SplitString(string str)
        {
            string[] str1 = str.Split('>');
            string[] str2 = str1[1].Split('<');
            return str2[0];
        }
        /// <summary>
        /// 踢出 战盟
        /// </summary>
        /// <returns></returns>
        public async ETVoid WarAlliancePropose()
        {
            G2C_WarAllianceProposeResponse g2C_WarAlliance = (G2C_WarAllianceProposeResponse)await SessionComponent.Instance.Session.Call(new C2G_WarAllianceProposeRequest
            {
                GameUserID = CurChooseWarMemberInfo.UUID
            });
            if (g2C_WarAlliance.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_WarAlliance.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "成功移除 战盟");
                WarMemberInfoScrollView.Items.Remove(CurChooseWarMemberInfo);
                WarMemberInfoScrollView.Items = warMemberInfos;
            }
        }
        /// <summary>
        /// 同意 拒绝战盟审核
        /// </summary>
        /// <param name="gameIDs"></param>
        /// <param name="type">0 拒绝 1 同意</param>
        /// <returns></returns>
        public async ETVoid WarAllianceAgreeOrReject(List<long> gameIDs, int type)
        {
            C2G_WarAllianceAgreeOrRejectRequest c2G_WarAllianceAgree = new C2G_WarAllianceAgreeOrRejectRequest();
            c2G_WarAllianceAgree.GameUserID.AddRange(gameIDs);
            c2G_WarAllianceAgree.Type = type;//0 拒绝 1 同意
           
            G2C_WarAllianceAgreeOrRejectResponse g2C_WarAlliance = (G2C_WarAllianceAgreeOrRejectResponse)await SessionComponent.Instance.Session.Call(c2G_WarAllianceAgree);
            if (g2C_WarAlliance.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_WarAlliance.Error.GetTipInfo());
            }
            else
            {
                /*if(type == 1)
                    Log.DebugBrown($"同意战盟申请");
                else
                    Log.DebugBrown($"拒绝战盟申请");*/
                
                foreach (var id in gameIDs)
                {
                    //从战盟申请列表中 移除
                    if (WarApplyInfos.Exists(m => m.UUID == id))
                    {
                        WarApplyInfos.Remove(WarApplyInfos.Find(m => m.UUID == id));
                    }
                }
                WarApplyInfoScrollView.Items = WarApplyInfos;//刷新 战盟申请列表
            }

        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public async ETVoid WarAllianceSignOut()
        {
            G2C_WarAllianceSignOutResponse g2C_WarAlliance = (G2C_WarAllianceSignOutResponse)await SessionComponent.Instance.Session.Call(new C2G_WarAllianceSignOutRequest { });
            if (g2C_WarAlliance.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_WarAlliance.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"成功退出 战盟");
                WarMainPanel?.gameObject.SetActive(false);
                InitPanel?.gameObject.SetActive(true);
            }
        }
    }
}
