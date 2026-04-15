using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 左侧中部模块
    /// 队伍中的成员显示、守护、宠物 坐骑显示
    /// </summary>
    public partial class UIMainComponent
    {
        Transform GuardItem, LeftCenterContent;
        Image GuardDurability;//守护耐久

        Dictionary<long, GameObject> TeamItemDic = new Dictionary<long, GameObject>();
        Queue<GameObject> TeamPanelQueue = new Queue<GameObject>();
        GameObject Events;
        Button InvitePlayer;//邀请玩家 按钮（只有队长有权限）
       public Button OpenBtn1;
        ToggleGroup toggleGroup;
        Button UpCaption_Btn, OutLeaveTeam_Btn, LeaveTeam_Btn, RoleInfo_Btn, AddFriends_Btn, PrivateChat_Btn, Move_Btn, Follow_Btn;
        RoleEntity currFollowEntity;//当前跟随的玩家
        int TeamIndex = -1;
        bool isFollow = false;//是否跟随
        RoleMoveControlComponent roleMoveControl;
        RoleEquipmentComponent equipEquipment;
        public GameObject teamPanel;
        public int index = 0;
        public void Init_LeftCenter()
        {
            teamPanel = ReferenceCollector_Main.GetGameObject("Pet_Team").gameObject;
            ReferenceCollector referenceCollector_LeftCenter = ReferenceCollector_Main.GetGameObject("Pet_Team").GetReferenceCollector();
            LeftCenterContent = referenceCollector_LeftCenter.GetGameObject("Content").transform;
            Events = referenceCollector_LeftCenter.GetImage("Events").gameObject;
            HideEvents();
            GuardItem = referenceCollector_LeftCenter.GetImage("GuardItem").transform;
            GuardDurability = GuardItem.transform.Find("hp/value").GetComponent<Image>();//守护耐久
            InitTeamPanelQueue();
            GuardItem.gameObject.SetActive(false);
            toggleGroup = LeftCenterContent.GetComponent<ToggleGroup>();

            OpenBtn1 = referenceCollector_LeftCenter.GetButton("OpenBtn1");
            OpenBtn1.onClick.AddSingleListener(ToggleLeftCenterPanel);
            //邀请队员
            InvitePlayer = referenceCollector_LeftCenter.GetButton("InvitePlayer");
            InvitePlayer.onClick.AddSingleListener(OpenInvitePlayerPanel);
            HideInvitPlayerBtn();

            TeamDatas.action = OnTeamDataChanged;

            ReferenceCollector collector = Events.GetReferenceCollector();
            UpCaption_Btn = collector.GetButton("UpCaption_Btn");//升级为队长
            OutLeaveTeam_Btn = collector.GetButton("OutLeaveTeam_Btn");//踢出队伍
            LeaveTeam_Btn = collector.GetButton("LeaveTeam_Btn");//离开队伍
            RoleInfo_Btn = collector.GetButton("RoleInfo_Btn");//查看队员装备
            AddFriends_Btn = collector.GetButton("AddFriends_Btn");//添加好友
            PrivateChat_Btn = collector.GetButton("PrivateChat_Btn");//私聊
            Move_Btn = collector.GetButton("Move_Btn");//移动
            Follow_Btn = collector.GetButton("Follow_Btn");//跟随 队长

            teamPanel.SetActive(true);

            //初始化守护
            equipEquipment = this.roleEntity.GetComponent<RoleEquipmentComponent>();

            if (equipEquipment.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Guard, out KnapsackDataItem knapsackDataItem))
            {
                ShowGuard(knapsackDataItem.ConfigId);
                ChangeGuardDurability();
            }

        }

        private void InitTeamPanelQueue()
        {
            for (int i = 0, length = LeftCenterContent.childCount - 1; i < length; i++)
            {
                Transform tr = LeftCenterContent.GetChild(i);
                TeamPanelQueue.Enqueue(tr.gameObject);
                tr.gameObject.SetActive(false);
            }
        }

        private void ToggleLeftCenterPanel()
        {
            index += 1;
            bool visible = index % 2 == 0;
            OpenBtn1.transform.GetChild(0).GetComponent<Text>().text = visible ? "隐藏" : "展开";
            Events.gameObject.SetActive(visible);
            LeftCenterContent.gameObject.SetActive(visible);
        }

        private void OpenInvitePlayerPanel()
        {
            UIComponent.Instance.VisibleUI(UIType.UITeam, "InvitePlayer");
        }

        private void OnTeamDataChanged()
        {
            if (TeamDatas.MyTeamState != null && TeamDatas.MyTeamState.IsCaptain)
            {
                if (!InvitePlayer.gameObject.activeSelf)
                {
                    ShowInvitPlayerBtn();
                }
            }
        }

        public void HideEvents()
        {
            Events.SetActive(false);
        }
        public void ShowEvents()
        {
            if (openTog.isOn) openTog.isOn = false;
            Events.SetActive(true);
        }
        /// <summary>
        /// 显示 邀请队友按钮
        /// </summary>
        public void ShowInvitPlayerBtn()
        {
            index = 0;
            OpenBtn1.gameObject.SetActive(true);
            InvitePlayer.gameObject.SetActive(true);
        }
        public void HideInvitPlayerBtn()
        {
            OpenBtn1.gameObject.SetActive(false);
            InvitePlayer.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示守护
        /// </summary>
        /// <param name="guardConfigId">守护的配置表ID</param>
        /// <param name="isshow"></param>
        public void ShowGuard(long guardConfigId, bool isshow = true)
        {
            if (isshow)
            {
                guardConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
                GuardItem.Find("name").GetComponent<Text>().text = item_Info.Name;
            }
            
          //  GuardItem.gameObject.SetActive(isshow);
           
        }
        /// <summary>
        /// 更新守护的耐久
        /// </summary>
        public void ChangeGuardDurability()
        {
            if (equipEquipment.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Guard, out KnapsackDataItem knapsackDataItem))
            {
               // Log.DebugRed($"守护耐久：{knapsackDataItem.GetProperValue(E_ItemValue.Durability)} / {knapsackDataItem.GetProperValue(E_ItemValue.DurabilityMax)}");
                GuardDurability.fillAmount = (float)knapsackDataItem.GetProperValue(E_ItemValue.Durability) / knapsackDataItem.GetProperValue(E_ItemValue.DurabilityMax);

            }

        }

        /// <summary>
        /// 显示 队员信息
        /// </summary>
        /// <param name="playerInTeam"></param>
        public void ShowTeamMember(TeamMateInfo playerInTeam)
        {
            OpenBtn1.gameObject.SetActive(true);
            GameObject teamStatus = TeamItemDic.ContainsKey(playerInTeam.GameUserId) ? TeamItemDic[playerInTeam.GameUserId] : TeamPanelQueue.Dequeue();
            
            teamStatus.SetActive(true);
            teamStatus.transform.Find("name").GetComponent<Text>().text = playerInTeam.UserName;
            teamStatus.transform.Find("Lev").GetComponent<Text>().text = $"Lv.{playerInTeam.Status.Level}";
            teamStatus.transform.Find("IsCaptain").GetComponent<Image>().enabled = playerInTeam.IsCaptain;
            teamStatus.transform.Find("Hp/hpvalue").GetComponent<Image>().fillAmount = (float)playerInTeam.Status.HP / playerInTeam.Status.HPMax;
            teamStatus.transform.Find("Mp/mpvalue").GetComponent<Image>().fillAmount = (float)playerInTeam.Status.MP / playerInTeam.Status.MPMax;
            TeamMateInfo status = playerInTeam;
            if (teamStatus.GetComponent<Toggle>().isOn)
            {
                teamStatus.GetComponent<Toggle>().isOn = false;
            }
            teamStatus.GetComponent<Toggle>().onValueChanged.AddSingleListener((value) =>
            {
                TogEvent(value, status);
            });
            if (TeamItemDic.ContainsKey(playerInTeam.GameUserId) == false)
            {
                TeamItemDic[playerInTeam.GameUserId] = teamStatus;
            }
            ///队长 永远置于队伍顶端位置 队员按入伍的先后顺序排序
            if (playerInTeam.IsCaptain)
            {
                teamStatus.transform.SetAsFirstSibling();
            }
           
            ///是队长 并且是本地玩家 才显示队员邀请按钮
            if (playerInTeam.IsCaptain && playerInTeam.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                ShowInvitPlayerBtn();
            }
            //本地玩家是队长 队员数量达到5 关闭队员邀请
            if (TeamDatas.MyTeamState != null && TeamDatas.MyTeamState.IsCaptain)
            {
                if (TeamItemDic.Count == TeamDatas.TeamMembersMaxCount)
                {
                    HideInvitPlayerBtn();
                }

            }


        }
        /// <summary>
        /// 改变队伍中玩家的属性
        /// </summary>
        /// <param name="teamProperty"></param>
        public void ChangeProperty(TeamMateProperty teamProperty, long playerId)
        {
            if (TeamItemDic.TryGetValue(playerId, out GameObject Item))
            {
                Item.transform.Find("Hp/hpvalue").GetComponent<Image>().fillAmount = (float)teamProperty.HP / teamProperty.HPMax;
                Item.transform.Find("Mp/mpvalue").GetComponent<Image>().fillAmount = (float)teamProperty.MP / teamProperty.MPMax;
                Item.transform.Find("Lev").GetComponent<Text>().text = $"Lv.{teamProperty.Level}";
            }
        }
        /// <summary>
        /// 离开队伍
        /// </summary>
        /// <param name="playerId"></param>
        public void HideTeamMember(long playerId)
        {
            if (TeamItemDic.TryGetValue(playerId, out GameObject itemPanel))
            {
                itemPanel.SetActive(false);
                TeamPanelQueue.Enqueue(itemPanel);
                TeamItemDic.Remove(playerId);
            }

        }
        /// <summary>
        /// 队员 功能选项
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="playerInTeam"></param>
        private void TogEvent(bool isOn, TeamMateInfo playerInTeam)
        {
            if (!toggleGroup.AnyTogglesOn()) //判断是否有Toggle开启
            {
                HideEvents();
            }
            if (!isOn)
            {
                return;
            }



            RoleInfo_Btn.onClick.AddSingleListener(() =>
            {
                //查看人物 （显示玩家的装备）
               
                if (UnitEntityComponent.Instance.Get<RoleEntity>(playerInTeam.GameUserId) is RoleEntity roleEntity)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIEquips);
                    UIComponent.Instance.Get(UIType.UIEquips).GetComponent<UIEquipsComponent>().ToViewRoleEquips(roleEntity);
                }
                else
                {
                    GetTeamMateEquips().Coroutine();
                }
            });
            ///添加好友
            AddFriends_Btn.onClick.AddSingleListener(() => AddFriends().Coroutine());

            PrivateChat_Btn.onClick.AddSingleListener(() =>
            {
                //私聊
                PrivateChat();
            });

            ///离开队伍
            LeaveTeam_Btn.onClick.AddSingleListener(() => LeaveTeamAsync().Coroutine());

            Follow_Btn.gameObject.SetActive(playerInTeam.IsCaptain && playerInTeam.GameUserId != UnitEntityComponent.Instance.LocaRoleUUID);// 跟随 队长
            Follow_Btn.onClick.AddListener(() =>
            {
                TeamIndex = TeamDatas.OtherTeamMemberStatusList.FindIndex(r => r.GameUserId == roleEntity.Id);
                FollowCaptain();
            });

            if (playerInTeam.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID
                || (playerInTeam.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID && playerInTeam.IsCaptain)
                )///当前 队员信息 本地玩家 或者 是队长
            {
                UpCaption_Btn.gameObject.SetActive(false);//禁用 升级为队长
                OutLeaveTeam_Btn.gameObject.SetActive(false);//禁用 踢出队伍
                AddFriends_Btn.gameObject.SetActive(false);// 禁用 添加好友
                PrivateChat_Btn.gameObject.SetActive(false);// 禁用 私聊
                RoleInfo_Btn.gameObject.SetActive(false);//禁用 查看人物
                LeaveTeam_Btn.transform.Find("Text").GetComponent<Text>().text = "离开队伍";//队长离开队伍 第二位的队员 继承队长职务
                LeaveTeam_Btn.gameObject.SetActive(true);
                Move_Btn.gameObject.SetActive(false);
            }
            else
            {
                ///其他队员信息
                RoleInfo_Btn.gameObject.SetActive(true);//查看人物
                AddFriends_Btn.gameObject.SetActive(!FriendListData.BlackList.Exists(r => r.UUID == playerInTeam.GameUserId));//添加好友(不在黑名单的 队员才会显示)
                PrivateChat_Btn.gameObject.SetActive(true);//私聊
                Move_Btn.gameObject.SetActive(true);
                Move_Btn.onClick.AddSingleListener(Move);
                ////本地玩家 是队长
                if (TeamDatas.MyTeamState != null && TeamDatas.MyTeamState.IsCaptain && TeamDatas.MyTeamState.GameUserId != playerInTeam.GameUserId)
                {
                    ///升级为队长
                    UpCaption_Btn.gameObject.SetActive(true);///队长点击 出自己以外的其他队员 才显示升为队长按钮
                    UpCaption_Btn.onClick.AddSingleListener(() => HandTheCaptain().Coroutine());
                    ///踢出该队员
                    OutLeaveTeam_Btn.gameObject.SetActive(true);
                    OutLeaveTeam_Btn.onClick.AddSingleListener(() => KcikThePlayer().Coroutine());
                    LeaveTeam_Btn.gameObject.SetActive(false);
                }
                else
                {
                    ///是队员
                    OutLeaveTeam_Btn.gameObject.SetActive(false);
                    UpCaption_Btn.gameObject.SetActive(false);
                    LeaveTeam_Btn.gameObject.SetActive(playerInTeam.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID);
                }
            }
            ShowEvents();

            ///移动
            void Move()
            {
                TeamDelive().Coroutine();

                /*  RoleEntity entity = UnitEntityComponent.Instance.Get<RoleEntity>(playerInTeam.GameUserId);
                  if (entity != null)
                  {
                      //TODO 判断 该玩家与本地玩家 是否在同一个场景
                      //移动到该玩家 旁边

                  }
                  else
                  {
                      //TODO请求服务器 获取该玩家的 坐标信息

                  }*/
                if (RoleOnHookComponent.Instance.IsOnHooking)
                {
                    HookTog.isOn = false;
                }
                UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                //队伍传送
                async ETVoid TeamDelive()
                {
                    G2C_TeamDeliveryResponse g2C_Team = (G2C_TeamDeliveryResponse)await SessionComponent.Instance.Session.Call(new C2G_TeamDeliveryRequest { GameUserId= playerInTeam.GameUserId });
                    if (g2C_Team.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Team.Error.GetTipInfo());
                        UIComponent.Instance.Remove(UIType.UISceneLoading);
                    }
                    if (RoleOnHookComponent.Instance.IsOnHooking)
                    {
                        HookTog.isOn = false;
                    }
                    HideEvents();
                }
            }
            ///跟随 队长
            void FollowCaptain()
            {
                RoleEntity role = UnitEntityComponent.Instance.Get<RoleEntity>(playerInTeam.GameUserId);
                if (role == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "队长不在附近 无法跟随");
                }
                else
                {
                    currFollowEntity = role;
                    isFollow = !isFollow;
                    Follow_Btn.transform.Find("Text").GetComponent<Text>().text = isFollow ? "取消跟随" : "跟随队长";
                }

            }


            ///私聊
            void PrivateChat()
            {
                UIComponent.Instance.VisibleUI(UIType.UIChatPanel);
                UIChatPanelComponent uIChat = UIComponent.Instance.Get(UIType.UIChatPanel).GetComponent<UIChatPanelComponent>();
                uIChat.InputName.text = playerInTeam.UserName;
                uIChat.CheakNameOnLineOrExist(playerInTeam.UserName).Coroutine();
                uIChat.ShwoPrivateChat();

            }


            ///获取队友的装备
            async ETVoid GetTeamMateEquips()
            {
                G2C_LoginSystemGetGamePlayerInfoResponse g2C_LoginSystemGetGame = (G2C_LoginSystemGetGamePlayerInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemGetGamePlayerInfoRequest
                {
                    GameId = new Google.Protobuf.Collections.RepeatedField<long>() { playerInTeam.GameUserId }
                });
                if (g2C_LoginSystemGetGame.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_LoginSystemGetGame.Error.GetTipInfo());
                }
                else
                {
                    //缓存角色信息 显示角色使用
                    for (int i = 0, length = g2C_LoginSystemGetGame.GameInfos.count; i < length; i++)
                    {
                        //缓存 角色存档
                        G2C_LoginSystemGetGamePlayerInfoMessage roleInfos = g2C_LoginSystemGetGame.GameInfos[i];
                        ///遍历 玩家的装备
                        foreach (var g2C_LoginSystemEquip in roleInfos.AllEquipStatus)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIEquips);
                            UIComponent.Instance.Get(UIType.UIEquips).GetComponent<UIEquipsComponent>().ToViewRoleEquips(g2C_LoginSystemEquip.ConfigID, g2C_LoginSystemEquip.ItemLevel);
                        }

                    }

                }
            }

            ///添加好友
            async ETVoid AddFriends()
            {
                G2C_AddFriendsResponse g2C_AddFriends = (G2C_AddFriendsResponse)await SessionComponent.Instance.Session.Call(new C2G_AddFriendsRequest
                {
                    GameUserId = playerInTeam.GameUserId,
                    CharName = playerInTeam.UserName
                });
                if (g2C_AddFriends.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_AddFriends.Error.GetTipInfo());
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "已向对方发送好友申请");
                }
            }

            ///转让队长
            async ETVoid HandTheCaptain()
            {
                G2C_HandTheCaptain g2C_HandThe = (G2C_HandTheCaptain)await SessionComponent.Instance.Session.Call(new C2G_HandTheCaptain
                {
                    PlayerGameUserId = playerInTeam.GameUserId//让位目标玩家
                });
                if (g2C_HandThe.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_HandThe.Error.GetTipInfo());
                }
            }
            ///踢出玩家 
            async ETVoid KcikThePlayer()
            {
                G2C_KcikThePlayerOutOfTheTeam g2C_KcikThePlayer = (G2C_KcikThePlayerOutOfTheTeam)await SessionComponent.Instance.Session.Call(new C2G_KcikThePlayerOutOfTheTeam { PlayerGameUserId = playerInTeam.GameUserId });
                if (g2C_KcikThePlayer.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_KcikThePlayer.Error.GetTipInfo());
                }
            }
            ///请求离开 当前队伍
            async ETVoid LeaveTeamAsync()
            {
                //离开队伍  没有参数
                G2C_LeaveTheTeam g2C_Leave = (G2C_LeaveTheTeam)await SessionComponent.Instance.Session.Call(new C2G_LeaveTheTeam
                {

                });
                if (g2C_Leave.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Leave.Error.GetTipInfo());
                }

            }
        }

        /// <summary>
        /// 隐藏当前队伍面板
        /// </summary>
        public void LeaveTeam()
        {
            foreach (var item in TeamItemDic)
            {
                TeamPanelQueue.Enqueue(item.Value);
                item.Value.SetActive(false);
            }
            HideEvents();
            HideInvitPlayerBtn();
            TeamItemDic.Clear();
        }
        /// <summary>
        ///线路切换清除 队伍信息
        /// </summary>
        public void ResetTeamWhenChangeLine() {
            if (TeamItemDic.Count > 0) {
                LeaveTeam();
                TeamDatas.MyTeamState = null;//清空 自己的队伍状态
                TeamDatas.OtherTeamMemberStatusList.Clear();//清理 队伍中的成员
                TeamDatas.ApplyPlayersList.Clear();// 清理 队伍申请 列表
                TeamDatas.NearTeamList.Clear();//附件可申请队伍显示 清理
            }
        }
        /// <summary>
        /// 跟随 队长
        /// </summary>
        public void FollowTeamMember()
        {
            if (isFollow)//跟随队长
            {
                if (CheackFollowLead())
                {
                    roleMoveControl ??= roleEntity.GetComponent<RoleMoveControlComponent>();
                    //判断 之间的距离 大于三格子 
                    if (!roleMoveControl.IsNavigation && PositionHelper.Distance2D(currFollowEntity.Position, roleEntity.Position) > (TeamIndex + 1) * 2)
                    {
                        Vector3 target = new Vector3(currFollowEntity.Position.x - ((TeamIndex + 1) * 2), 0, currFollowEntity.Position.z);
                        // roleMoveControl.UnitEntityPathComponent.NavTarget(target);
                        roleMoveControl.UnitEntityPathComponent.NavTarget(currFollowEntity.CurrentNodePos);
                    }
                }
                else
                {
                    CancelFollow();
                }
            }
        }

        /// <summary>
        /// 判断队长 是否在 附近、队长是否离开队伍
        /// </summary>
        /// <returns></returns>
        private bool CheackFollowLead()
        {
            ///队长在附近 且 队长还在队伍中
            if (UnitEntityComponent.Instance.AllUnitEntityDic.ContainsKey(currFollowEntity.Id) && TeamDatas.OtherTeamMemberStatusList.Exists(r => r.GameUserId == currFollowEntity.Id)
                && TeamDatas.OtherTeamMemberStatusList.Find(r => r.GameUserId == currFollowEntity.Id).IsCaptain)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 取消跟随
        /// </summary>
        private void CancelFollow()
        {
            isFollow = !isFollow;
            currFollowEntity = null;
            Follow_Btn.transform.Find("Text").GetComponent<Text>().text = "跟随队长";
        }

        public void Clear_LeftCenter()
        {
            for (int i = 2; i < LeftCenterContent.childCount; i++)
            {
                UnityEngine.GameObject.Destroy(LeftCenterContent.GetChild(i));
            }
        }
    }
}
