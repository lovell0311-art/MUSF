using ETModel;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{
    public partial class UIFriendListComponent : Component, IUGUIStatus
    {
        /// <summary>
        /// 聊天内容,好友内容
        /// </summary>
        public GameObject chatContent, firendContent;
        /// <summary>
        /// 列表Togs
        /// </summary>
        public GameObject friendTogs;
        /// <summary>
        /// 聊天视图,好友视图
        /// </summary>
        public ScrollRect chatView, friendView;
        /// <summary>
        /// 好友数量
        /// </summary>
        public Text friendCount;
        /// <summary>
        /// 聊天输入框
        /// </summary>
        public InputField chatInput;
        /// <summary>
        /// 玩家实体
        /// </summary>
        public RoleEntity roleEntity;
        /// <summary>
        /// 发送,添加好友,传送按钮
        /// </summary>
        public Button sendBtn, addFriendBtn, chuanSong;
        /// <summary>
        /// 好友关系滑动框组件
        /// </summary>
        public UICircularScrollView<FriendInfo> uICircular_Friend;
        /// <summary>
        /// 聊天消息滑动框组件
        /// </summary>
        public UICircularScrollView<FriendChatNewInfo> uICircular_Chat;

        /// <summary>
        /// 类型,好友,宿敌,黑名单
        /// </summary>
        public E_FriendsTogNewType Cur_E_Friends;
        /// <summary>
        /// 类型,申请,推荐
        /// </summary>
        public E_AddFriendsTogNewType Cur_E_Apply;
        /// <summary>
        /// 聊天类型,对方,自己
        /// </summary>
        public ChatType chatType;

        /// <summary>
        /// 当前选择的好友,用于聊天框的显示
        /// </summary>
        public FriendInfo nowFriendInfo;
        /// <summary>
        /// /好友界面底部聊天框,按钮
        /// </summary>
        public GameObject ChatManager;
        /// <summary>
        /// 上一次点击的好友
        /// </summary>
        public GameObject LastCLickFriend;
        /// <summary>
        /// 取消屏蔽,移除宿敌列表按钮
        /// </summary>
        public GameObject Cansole;
        /// <summary>
        /// 拉黑或成为仇人时间
        /// </summary>
        public Text BlackOrEnemyTime;
        public Image FriendRedDot;
        public Image FriendEnemyRedDot;
        public Image FriendAddFriendRedDot;

        public void FriendAwake()
        {
            friendReferenceCollector = all_ReferenceCollector.GetGameObject("FriendMainPanel").GetReferenceCollector();
            roleEntity = UnitEntityComponent.Instance.LocalRole;

            FriendAddFriendRedDot = friendReferenceCollector.GetImage("FriendMainAddFriend");
            FriendEnemyRedDot = friendReferenceCollector.GetImage("FriendMainEnemy");
            FriendRedDot = friendReferenceCollector.GetImage("FriendMainFriend");
            ///关闭好友界面
            friendReferenceCollector.GetButton("CloseBtn").onClick.AddListener(delegate
            {
                UIComponent.Instance.Remove(UIType.UIFirendList);
            });

            friendReferenceCollector.GetButton("AddFriendBtn").onClick.AddListener(delegate
            {
                addfriendReferenceCollector.gameObject.SetActive(true);
                InitAddFriend();
            });

            chatContent = friendReferenceCollector.GetGameObject("ChatContent");
            firendContent = friendReferenceCollector.GetGameObject("FriendsContent");
            friendTogs = friendReferenceCollector.GetGameObject("Togs");
            chatView = friendReferenceCollector.GetImage("ChatView").GetComponent<ScrollRect>();
            friendView = friendReferenceCollector.GetImage("FriendsView").GetComponent<ScrollRect>();

            friendCount = friendReferenceCollector.GetText("FriendCount");
            chatInput = friendReferenceCollector.GetInputField("ChatInput");


            ChatManager = friendReferenceCollector.GetGameObject("ChatManager");
            Cansole = friendReferenceCollector.GetGameObject("Cansole");

            sendBtn = friendReferenceCollector.GetButton("SendBtn");

            BlackOrEnemyTime = friendReferenceCollector.GetText("BlackOrEnemyTime");

            //发送聊天消息
            sendBtn.onClick.AddSingleListener(delegate
            {
                SendMessageEvent().Coroutine();
            });
            //添加发送位置信息监听
            chuanSong = friendReferenceCollector.GetButton("Chuansong");
            chuanSong.onClick.AddSingleListener(delegate
            {
                RequestSendPosition().Coroutine();
            });

            ///添加Togs事件点击
            for (int i = 0; i < friendTogs.transform.childCount; i++)
            {
                E_FriendsTogNewType type = FriendListData.GetE_FriendsTogNewType(i);
                friendTogs.transform.GetChild(i).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => TogEvent(value, type));
            }
            ///移除仇人列表
            Cansole.transform.Find("RemoveEnemy").GetComponent<Button>().onClick.AddSingleListener(delegate
            {
                if (nowFriendInfo == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请选择移除宿敌玩家");
                    return;
                }
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"移除宿敌玩家[{nowFriendInfo.NickName}]");
                RequestDelectFriend(0, 2, nowFriendInfo).Coroutine();
            });
            ///移除黑名单列表
            Cansole.transform.Find("RemoveBlack").GetComponent<Button>().onClick.AddSingleListener(delegate
            {
                if (nowFriendInfo == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请选择移除屏蔽玩家");
                    return;
                }
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"移除屏蔽玩家[{nowFriendInfo.NickName}]");
                RequestDelectFriend(0, 1, nowFriendInfo).Coroutine();
            });
            //进入好友面板,默认显示好友列表,所以隐藏移除仇人,黑名单列表按钮
            Cansole.SetActive(false);
            //初始化好友下拉列表视图
            UICircularFriendInit();
            //初始化聊天下拉列表视图
            UICircularChatInit();


        }
        /// <summary>
        /// 发送消息事件
        /// </summary>
        private async ETTask SendMessageEvent()
        {
            Log.Debug("发送信息");

            if (nowFriendInfo == null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请选择聊天好友");
                return;
            }
            if (string.IsNullOrEmpty(chatInput.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "不可发送空消息");
                return;
            }

            if(nowFriendInfo.State != "在线")
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"好友[{nowFriendInfo.NickName}]不在线");
                chatInput.text = null;
                return;
            }
            if (SystemUtil.IsInvaild(chatInput.text))
            {
                chatInput.text = SystemUtil.ReplaceStr(chatInput.text);
            }
            try
            {
                //申请发送消息
                G2C_FriendChatResponse g2C_ = (G2C_FriendChatResponse)await SessionComponent.Instance.Session.Call(new C2G_FriendChatRequest()
                {
                    GameUserId = nowFriendInfo.UUID,
                    IMessage = chatInput.text
                });
                if (g2C_.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_.Error.GetTipInfo());
                }
                else
                {
                    //缓存自己发送消息
                    FriendChatNewInfo chatNewInfo = new FriendChatNewInfo()
                    {
                        NickName = roleEntity.RoleName,
                        Time = TimeHelper.ClientNowSeconds(),
                        Message = chatInput.text,
                        type = ChatType.MyChatMessage
                    };
                    if (!FriendListData.friendChatNewInfos.ContainsKey(nowFriendInfo.UUID))
                    {
                        FriendListData.friendChatNewInfos.Add(nowFriendInfo.UUID, new List<FriendChatNewInfo>());
                    }
                    //本地置顶
                    if (FriendListData.FriendList.Exists((f) => f.UUID == nowFriendInfo.UUID))
                    {
                        int a = FriendListData.FriendList.IndexOf(nowFriendInfo);
                        Swap(FriendListData.FriendList, a, 0);
                    }
                    //刷新好友列表
                    uICircular_Friend.Items = FriendListData.FriendList;
                    //添加到本地缓存
                    FriendListData.friendChatNewInfos[nowFriendInfo.UUID].Add(chatNewInfo);
                    chatInput.text = null;
                    //刷新聊天框
                    uICircular_Chat.Items = FriendListData.friendChatNewInfos[nowFriendInfo.UUID];
                    chatView.verticalNormalizedPosition = 0f;

                }
            }
            catch (Exception e)
            {

                Log.DebugBrown(e.ToString());
            }

        }
        /// <summary>
        /// 请求发送定位
        /// </summary>
        /// <returns></returns>
        public async ETTask RequestSendPosition()
        {
            //当前选择的好友为null,提示
            if (nowFriendInfo == null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请选择好友");
                return;
            }
            try
            {
                Log.Debug($"向好友[{nowFriendInfo.NickName}]发送位置信息");
                G2C_FriendPositionResponse g2C_ = (G2C_FriendPositionResponse)await SessionComponent.Instance.Session.Call(new C2G_FriendPositionRequest()
                {
                    GameUserId = nowFriendInfo.UUID
                });

                if (g2C_.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_.Error.GetTipInfo());
                }
                else
                {
                    Log.Debug($"选中的玩家:{nowFriendInfo?.NickName}");
                    Log.DebugGreen($"坐标信息:{(int)roleEntity.Position.x / 2},{(int)roleEntity.Position.z / 2},当前场景:{(int)(SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>())}");
                    //缓存本地数据
                    FriendChatNewInfo chatNewInfo = new FriendChatNewInfo()
                    {
                        NickName = roleEntity.RoleName,
                        Time = TimeHelper.ClientNowSeconds(),
                        XPos = (int)roleEntity.Position.x / 2,
                        YPos = (int)roleEntity.Position.z / 2,
                        mapID = (int)(SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>()),
                        type = ChatType.MyChatMessage
                    };

                    //发送消息后本地置顶
                    if (FriendListData.FriendList.Exists((f) => f.UUID == nowFriendInfo.UUID))
                    {
                        int a = FriendListData.FriendList.IndexOf(nowFriendInfo);
                        Swap(FriendListData.FriendList, a, 0);
                    }
                    //刷新列表
                    uICircular_Friend.Items = FriendListData.FriendList;

                    //添加到本地消息缓存
                    if (!FriendListData.friendChatNewInfos.ContainsKey(nowFriendInfo.UUID))
                    {
                        FriendListData.friendChatNewInfos.Add(nowFriendInfo.UUID, new List<FriendChatNewInfo>());
                    }
                    FriendListData.friendChatNewInfos[nowFriendInfo.UUID].Add(chatNewInfo);

                    chatInput.text = null;
                    //发送消息的时候,选择的当前好友,直接刷新显示
                    uICircular_Chat.Items = FriendListData.friendChatNewInfos[nowFriendInfo.UUID];
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

        }
        public async ETVoid RequestFriendList(int type)
        {
            //Log.DebugBrown($"开始请求列表{type}");
            try
            {
                G2C_OpenFriendsinterfaceResponse g2C_Open = (G2C_OpenFriendsinterfaceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenFriendsinterfaceRequest()
                {
                    ListType = type//列表
                });
                if (g2C_Open.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_Open.Error.GetTipInfo()}");
                }
                else
                {
                    string state = null;
                    //count,判断第一个目标玩家
                    int count = 0;

                    switch (type)
                    {
                        case 1:
                            //清理之前的好友
                            FriendListData.BlackList.Clear();
                            foreach (var item in g2C_Open.FList)
                            {
                                if (!FriendListData.BlackList.Exists(f => f.NickName == item.CharName))
                                {
                                    Log.Debug("item.CharName:" + item.CharName);
                                    FriendInfo FriendInfo = new FriendInfo()
                                    {
                                        NickName = item.CharName,
                                        UUID = item.GameUserId,
                                        Zhanmeng = item.WarAllianceName,
                                        Identity = string.IsNullOrEmpty(item.WarAllianceName) ? GetPos(item.WarAlliancePost) : string.Empty,
                                        Level = item.ILV,
                                        Job = ((E_RoleType)item.ClassType).ToString(),
                                        TimeDate = item.TimeDate,
                                        State = item.IState == 0 ? "在线" : "离线",
                                        isChoose = count == 0
                                    };
                                    FriendListData.BlackList.Add(FriendInfo);
                                    count++;
                                }
                            }
                            if (FriendListData.BlackList.Count == 0)
                            {
                                nowFriendInfo = null;
                                BlackOrEnemyTime.text = null;
                                Cansole.transform.Find("RemoveBlack").gameObject.SetActive(false);
                            }
                            else
                            {
                                nowFriendInfo = FriendListData.BlackList[0];
                                Cansole.transform.Find("RemoveBlack").gameObject.SetActive(true);//有黑名单玩家才显示移除黑名单按钮
                                BlackOrEnemyTime.text = $"在{TimeConvert(FriendListData.BlackList[0].TimeDate)}拉黑了他";
                            }
                            friendCount.text = $"人数:{count}/50";
                            RefreshFriendsList();
                            break;
                        case 2:
                            FriendListData.EnemyList.Clear();
                            foreach (var item in g2C_Open.FList)
                            {
                                if (!FriendListData.EnemyList.Exists(f => f.NickName == item.CharName))
                                {
                                    Log.Debug("item.CharName:" + item.CharName);
                                    FriendInfo friendInfo = new FriendInfo()
                                    {
                                        NickName = item.CharName,
                                        UUID = item.GameUserId,
                                        Zhanmeng = item.WarAllianceName,
                                        Identity = string.IsNullOrEmpty(item.WarAllianceName) ? GetPos(item.WarAlliancePost) : string.Empty,
                                        Level = item.ILV,
                                        Job = ((E_RoleType)item.ClassType).ToString(),
                                        TimeDate = item.TimeDate,
                                        State = item.IState == 0 ? "在线" : "离线",
                                        isChoose = false
                                    };
                                    FriendListData.EnemyList.Add(friendInfo);
                                    count++;
                                }
                            }
                            if (FriendListData.EnemyList.Count == 0)
                            {
                                nowFriendInfo = null;
                                BlackOrEnemyTime.text = null;
                                Cansole.transform.Find("RemoveEnemy").gameObject.SetActive(false);
                            }
                            else
                            {
                                nowFriendInfo = FriendListData.EnemyList[0];
                                BlackOrEnemyTime.text = $"在{TimeConvert(FriendListData.EnemyList[0].TimeDate)},被玩家\"{FriendListData.EnemyList[0].NickName}\"击杀," +
                                    $"与玩家\"{FriendListData.EnemyList[0].NickName}\"成为仇人";

                                Cansole.transform.Find("RemoveEnemy").gameObject.SetActive(true);//有仇人才显示移除仇人按钮
                            }
                            friendCount.text = $"人数:{count}/100";
                            RefreshFriendsList();
                            break;
                        case 3:
                            Cur_E_Apply = E_AddFriendsTogNewType.Apply;
                            FriendListData.ApplyList.Clear();
                            foreach (var item in g2C_Open.FList)
                            {
                                _ = (item.IState == 0) ? (state = "在线") : (state = "离线");
                                if (!FriendListData.ApplyList.Exists(f => f.NickName == item.CharName))
                                {
                                    FriendListData.ApplyList.Add(new FriendInfo()
                                    {
                                        NickName = item.CharName,
                                        UUID = item.GameUserId,
                                        Zhanmeng = item.WarAllianceName,
                                        Identity = string.IsNullOrEmpty(item.WarAllianceName) ? GetPos(item.WarAlliancePost) : string.Empty,
                                        Level = item.ILV,
                                        Job = ((E_RoleType)item.ClassType).ToString(),
                                        TimeDate = item.TimeDate,
                                        State = state,
                                        isChoose = false
                                    });
                                }
                            }
                            RefreshApplyList();
                            break;
                        case 4:
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
                                        Identity = !string.IsNullOrEmpty(item.WarAllianceName)?GetPos(item.WarAlliancePost) : string.Empty,
                                        Level = item.ILV,
                                        Job = ((E_RoleType)item.ClassType).GetRoleName(0),
                                        TimeDate = item.TimeDate,
                                        State = item.IState == 0 ? "在线" : "离线",
                                        isChoose = false
                                    };
                                    BlackOrEnemyTime.text = null;
                                    FriendListData.FriendList.Add(friendInfo);
                                    count++;
                                }
                            }
                            if (FriendListData.friendChatNewInfos.Count > 0)
                            {
                                foreach (var item in FriendListData.FriendList)
                                {
                                    if (FriendListData.friendChatNewInfos.ContainsKey(item.UUID))
                                    {
                                        if (FriendListData.friendChatNewInfos.TryGetValue(item.UUID, out List<FriendChatNewInfo> value) && value.Count > 0)
                                        {
                                            long time = 0;
                                            foreach (var item1 in value)
                                            {
                                                if (item1.Time > time)
                                                    time = item1.Time;
                                            }
                                            item.SendOrReceiveMessageTimeDate = time;
                                        }

                                    }
                                }
                            }
                            //在线离线排序
                            FriendListData.FriendList.Sort((m1,m2)=> 
                            {
                                return m1.State.CompareTo(m2.State);
                                //int x = m2.SendOrReceiveMessageTimeDate.CompareTo(m1.SendOrReceiveMessageTimeDate);
                                //if (x == 0)
                                //{
                                  //  if (m2.State == "在线")
                                   //     x= 1;
                                  //  else
                                 //       x= -1;
                               // }
                                //return x;
                              
                            });
                            if (FriendListData.FriendList.Count == 0)
                            {
                                nowFriendInfo = null;
                                ChatManager.SetActive(false);//如果没有好友,聊天输入框,发送按钮等就隐藏不显示
                                chatView.gameObject.SetActive(false);//如果没有好友,聊天框就隐藏不显示
                            }
                            else
                            {
                                ChatManager.SetActive(true); //如果没有好友,聊天输入框,发送按钮等就显示
                                chatView.gameObject.SetActive(true);//有好友,聊天框就显示
                                //设置默认选中
                                nowFriendInfo = FriendListData.FriendList[0];
                                FriendListData.FriendList[0].isChoose = true;
                                RefreshChatList(FriendListData.FriendList[0].UUID);//根据id更新聊天框显示

                                //根据消息时间排序

                            }
                            friendCount.text = $"人数:{count}/100";
                            RefreshFriendsList();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.DebugRed(e.ToString());
            }
            
        }
        /// <summary>
        /// 根据 职务id 获取对应的职位名
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public string GetPos(int post) => post switch
        {
            0 => "成员",
            1 => "小队长",
            2 => "副盟主",
            3 => "盟主",
            _ => "成员"
        };
        /// <summary>
        /// 交换位置,用于来新消息后,消息置顶
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public List<T> Swap<T>(List<T> list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
            return list;
        }

        /// <summary>
        /// Tog按钮事件
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="type"></param>
        public void TogEvent(bool isOn, E_FriendsTogNewType type)
        {
            if (!isOn) return;
            switch (type)
            {
                case E_FriendsTogNewType.Friend://好友按钮
                    BlackOrEnemyTime.text = null;//拉黑或成为宿敌时间为null,表示不显示
                    Cansole.SetActive(false); //当前在好友列表,移除黑名单,宿敌列表按钮不显示
                    Cur_E_Friends = E_FriendsTogNewType.Friend;
                    RequestFriendList(4).Coroutine();
                    RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Friend_FriendList, 0);
                    FriendRedDot.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Friend_FriendList) > 0);
                    break;
                case E_FriendsTogNewType.Black:
                    BlackOrEnemyTime.text = null;
                    chatView.gameObject.SetActive(false);
                    ChatManager.SetActive(false);
                    Cansole.SetActive(true);
                    Cansole.transform.Find("RemoveEnemy").gameObject.SetActive(false);//隐藏移除仇人按钮
                    Cur_E_Friends = E_FriendsTogNewType.Black;
                    RequestFriendList(1).Coroutine();
                    break;
                case E_FriendsTogNewType.Enemy:
                    BlackOrEnemyTime.text = null;
                    chatView.gameObject.SetActive(false);
                    ChatManager.SetActive(false);
                    Cansole.SetActive(true);
                    Cansole.transform.Find("RemoveBlack").gameObject.SetActive(false);//隐藏移除黑名单按钮
                    Cur_E_Friends = E_FriendsTogNewType.Enemy;
                    RequestFriendList(2).Coroutine();
                    //取消红点
                    RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Friend_FriendEnemy, 0);
                    FriendEnemyRedDot.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 初始化好友滑动框
        /// </summary>
        public void UICircularFriendInit()
        {
            uICircular_Friend = ComponentFactory.Create<UICircularScrollView<FriendInfo>>();
            uICircular_Friend.InitInfo(E_Direction.Vertical, 1, 0, 0);
            uICircular_Friend.ItemInfoCallBack = InitFriendItem;
            uICircular_Friend.IninContent(firendContent, friendView);
            uICircular_Friend.Items = FriendListData.FriendList;
        }
        /// <summary>
        /// 初始化聊天滑动框
        /// </summary>
        public void UICircularChatInit()
        {
            uICircular_Chat = ComponentFactory.Create<UICircularScrollView<FriendChatNewInfo>>();
            uICircular_Chat.InitInfo(E_Direction.Vertical, 1, 0, 10);
            uICircular_Chat.ItemInfoCallBack = InitChatItem;
            uICircular_Chat.IninContent(chatContent, chatView);
        }
        /// <summary>
        /// 刷新好友列表
        /// </summary>
        public void RefreshFriendsList()
        {
            uICircular_Friend.Items = Cur_E_Friends.GetCurList();
        }
        /// <summary>
        /// 刷新申请列表
        /// </summary>
        public void RefreshApplyList()
        {
            applyUICircularScroll.Items = Cur_E_Apply.GetAddFriendCurList();
        }
        /// <summary>
        /// 根据好友ID刷新聊天列表
        /// </summary>
        /// <param name="uuid"></param>
        public void RefreshChatList(long uuid)
        {
            if (!FriendListData.friendChatNewInfos.ContainsKey(uuid))
            {
                FriendListData.friendChatNewInfos.Add(uuid,new List<FriendChatNewInfo>());
            }
            uICircular_Chat.Items = FriendListData.friendChatNewInfos[uuid];
        }
        /// <summary>
        /// 新消息进来初始化
        /// </summary>
        /// <param name="item"></param>
        /// <param name="friendChatInfo"></param>
        private void InitChatItem(GameObject item, FriendChatNewInfo friendChatInfo)
        {
            Log.Debug($"friendChatInfo.type1 : {friendChatInfo.type}");
            string time = TimeConvert(friendChatInfo.Time);
            switch (friendChatInfo.type)
            {
                //自己发送的消息
                case ChatType.MyChatMessage:
                    item.transform.Find("MyNickName").gameObject.SetActive(true);//自己的名字
                    item.transform.Find("MyNickName").GetComponent<Text>().text = friendChatInfo.NickName + "  " + time;//发送消息的时间
                    item.transform.Find("OtherSideNickName").gameObject.SetActive(false);///对方的名字不显示
                    item.transform.Find("MyBG").gameObject.SetActive(true);//显示我的消息
                    //如果没有坐标信息,就说明发送的是普通消息
                    if (friendChatInfo.XPos == 0 && friendChatInfo.XPos == 0)
                    {
                        item.transform.Find("MyBG").transform.Find("MyMessage").GetComponent<Text>().text = friendChatInfo.Message;
                    }
                    else
                    {
                        Log.Debug($"<color=#00FF00>[{SceneNameExtension.GetSceneName((SceneName)friendChatInfo.mapID)}:{friendChatInfo.XPos},{friendChatInfo.YPos}]</color>");
                        item.transform.Find("MyBG").transform.Find("MyMessage").GetComponent<Text>().text =
                            $"<color=#00FF00>[{SceneNameExtension.GetSceneName((SceneName)friendChatInfo.mapID)}:{friendChatInfo.XPos},{friendChatInfo.YPos}]</color>";
                    }

                    item.transform.Find("OtherSideBG").gameObject.SetActive(false);//对方的消息不显示
                    break;
                //好友发送的消息
                case ChatType.OtherSideMessage:
                    item.transform.Find("OtherSideNickName").gameObject.SetActive(true);
                    item.transform.Find("OtherSideNickName").GetComponent<Text>().text = friendChatInfo.NickName + "  " + time;
                    item.transform.Find("MyNickName").gameObject.SetActive(false);
                    item.transform.Find("OtherSideBG").gameObject.SetActive(true);
                    if (friendChatInfo.XPos == 0 && friendChatInfo.XPos == 0)
                    {
                        item.transform.Find("OtherSideBG").transform.Find("OtherSideMessage").GetComponent<Text>().text = friendChatInfo.Message;
                    }
                    else
                    {
                        item.transform.Find("OtherSideBG").gameObject.GetComponent<Button>().onClick.AddSingleListener(delegate
                        {
                            Map_TransferPointConfig map_Transfer = ConfigComponent.Instance.GetItem<Map_TransferPointConfig>(friendChatInfo.mapID);
                            CheCkLev();
                            void CheCkLev()
                            {
                                if (map_Transfer != null && roleEntity.Property.GetProperValue(E_GameProperty.Level) < map_Transfer.MapMinLevel)
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                                    return;
                                }
                                else
                                {
                                    FriendDeliveryRequest(friendChatInfo.UUID, friendChatInfo.mapID, friendChatInfo.XPos, friendChatInfo.YPos).Coroutine();
                                }
                            }
                        });
                        
                        Log.Debug($"<color=#00FF00>[{SceneNameExtension.GetSceneName((SceneName)friendChatInfo.mapID)}:{friendChatInfo.XPos},{friendChatInfo.YPos}]</color>");
                      //位置信息绿色显示
                        item.transform.Find("OtherSideBG").transform.Find("OtherSideMessage").GetComponent<Text>().text =
                            $"<color=#00FF00>[{SceneNameExtension.GetSceneName((SceneName)friendChatInfo.mapID)}:{friendChatInfo.XPos},{friendChatInfo.YPos}]</color>";
                    }
                    item.transform.Find("MyBG").gameObject.SetActive(false);
                    break;
                case ChatType.Null:
                    break;
                default:
                    break;
            }
            ///检查 等级是否满足条件
           
        }
        /// <summary>
        /// 传送
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ETTask FriendDeliveryRequest(long id,int mapId,int x,int y)
        {
            G2C_FriendDeliveryResponse c2G_FriendDelivery = (G2C_FriendDeliveryResponse)await SessionComponent.Instance.Session.Call(new C2G_FriendDeliveryRequest()
            {
                GameUserId = id
            });
            if(c2G_FriendDelivery.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_FriendDelivery.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.Remove(UIType.UIFirendList);
            }

        }

        /// <summary>
        /// 新好友进来初始化
        /// </summary>
        /// <param name="item"></param>
        /// <param name="friendInfo"></param>
        private void InitFriendItem(GameObject item, FriendInfo friendInfo)
        {
            item.transform.Find("NickName").GetComponent<Text>().text = friendInfo.NickName;//名字
            item.transform.Find("Teams").GetComponent<Text>().text = $"战盟:{friendInfo.Zhanmeng}";//战盟
            item.transform.Find("Level").GetComponent<Text>().text = "Level:" + friendInfo.Level.ToString();//等级
            item.transform.Find("State").GetComponent<Text>().text = SetTextColor(friendInfo.State);//状态
            //点击好友监听
            item.transform.Find("Background").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
                if (LastCLickFriend == item) return;
                //显示最下方得消息
                chatView.verticalNormalizedPosition = 0f;
                //上一个点击得好友得更多按钮变为false
                LastCLickFriend?.transform.Find("More").gameObject.SetActive(false);
                //当前点击的更多按钮显示
                item.transform.Find("More").gameObject.SetActive(true);
                //隐藏上一个点击状态
                LastCLickFriend?.transform.Find("Checkmark").gameObject.SetActive(false);
                //设置上一个点击按钮为当前按钮
                LastCLickFriend = item;
                //设置当前为选中状态
                item.transform.Find("Checkmark").gameObject.SetActive(true);
                FriendTogEvent(friendInfo);
                //宿敌和黑名单列表显示成为敌人或加入黑名单时间提示
                if (Cur_E_Friends == E_FriendsTogNewType.Enemy)
                {
                    BlackOrEnemyTime.text = $"在{TimeConvert(friendInfo.TimeDate)},被玩\"{friendInfo.NickName}\"击杀,与玩家\"{friendInfo.NickName}\"成为仇人";
                }else if (Cur_E_Friends == E_FriendsTogNewType.Black)
                {
                    BlackOrEnemyTime.text = $"在{TimeConvert(friendInfo.TimeDate)}拉黑了他";
                }
                else
                {
                    BlackOrEnemyTime.text = null;
                }
            });
            item.transform.Find("More").GetComponent<Button>().onClick.AddSingleListener(delegate ()
            {
               // Log.DebugGreen($"查看->{friendInfo.NickName},战盟：{friendInfo.Zhanmeng}");
                //好友面板显示
                peopleReferenceCollector.gameObject.SetActive(true);
                //设置好友面板信息
                SetPeopleInfo(friendInfo);
            });
            //Log.DebugGreen($"{friendInfo.NickName}:{friendInfo.isChoose}");
            ///选中提示
            if (friendInfo.isChoose == true)
            {
                LastCLickFriend = item;
                item.transform.Find("Checkmark").gameObject.SetActive(true);
                item.transform.Find("More").gameObject.SetActive(friendInfo.State == "在线");
            }
            else
            {
                item.transform.Find("Checkmark").gameObject.SetActive(false);
                item.transform.Find("More").gameObject.SetActive(false);
            }
            ///敌人列表不显示更多按钮
            if (Cur_E_Friends == E_FriendsTogNewType.Enemy)
            {
                item.transform.Find("More").gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 点击好友事件
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="friendInfo"></param>
        private void FriendTogEvent(FriendInfo friendInfo)
        {
            //设置当前选中好友信息
            nowFriendInfo = friendInfo;
            if(Cur_E_Friends == E_FriendsTogNewType.Friend)
            {
                if (!FriendListData.friendChatNewInfos.ContainsKey(friendInfo.UUID))
                {
                    FriendListData.friendChatNewInfos.Add(friendInfo.UUID, new List<FriendChatNewInfo>());
                }
                //点击后设置当前好友对应的聊天信息
                uICircular_Chat.Items = FriendListData.friendChatNewInfos[friendInfo.UUID];

                for (int i = 0, length = uICircular_Friend.Items.Count; i < length; i++)
                {
                    uICircular_Friend.Items[i].isChoose = uICircular_Friend.Items[i].NickName == friendInfo.NickName;
                }
            }
        }
            
        /// <summary>
        /// 设置在线颜色
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public string SetTextColor(string state)
        { 
            if(state == "在线")
            {
                return $"<color=#00FF00>在线</color>";
            }
            return "离线";
        }
        /// <summary>
        /// 日期转换
        /// </summary>
        /// <param name="timeDate"></param>
        /// <returns></returns>
        public string TimeConvert(long timeDate)
        {
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            System.DateTime dt = startTime.AddSeconds(timeDate);
            string time = dt.ToString("yyyy/MM/dd HH:mm:ss");//转化为日期时间
            return time;
        }

        public void OnVisible()
        {
            friendTogs.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            Cur_E_Friends = E_FriendsTogNewType.Friend;
            TogEvent(true, Cur_E_Friends);
            RedDotFriendCheack();
        }

        /// <summary>
        /// 红点检测
        /// </summary>
        public void RedDotFriendCheack()
        {
            FriendAddFriendRedDot.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Friend_AddFirend_FirendApply) > 0);
            FriendEnemyRedDot.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Friend_FriendEnemy) > 0);
            FriendAddFriendApplyRedDot.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Friend_AddFirend_FirendApply) > 0);
            UIMainComponent.Instance.RedDotFriendCheack();
        }
        public void OnVisible(object[] data)
        {

        }
        public void OnInVisibility()
        {
            
        }


        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            Cur_E_Friends = E_FriendsTogNewType.Null;
            nowFriendInfo = null;
            uICircular_Friend.Dispose();
            uICircular_Chat.Dispose();
            uICircular_AddFriend.Dispose();
            applyUICircularScroll.Dispose();
        }
    }
}