using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIFriendListComponent : Component
    {
        /// <summary>
        /// 添加好友,好友申请按钮组
        /// </summary>
        public GameObject addFriendTogs;
        /// <summary>
        /// 滑动框
        /// </summary>
        public ScrollRect systemRecommendView;
        /// <summary>
        /// 滑动框容器
        /// </summary>
        public GameObject systemRecommendContent;
        /// <summary>
        /// 系统推荐刷新时间
        /// </summary>
        public Text refreshTimeTxt;
        /// <summary>
        /// 是否可以刷新
        /// </summary>
        public bool isRefresh = true;
        public Button refreshButton;
        public Image refreshImage;
        /// <summary>
        /// 搜索输入框
        /// </summary>
        public InputField searchInput;
        /// <summary>
        /// 查找按钮
        /// </summary>
        public Button findBtn;
        /// <summary>
        /// 系统推荐
        /// </summary>
        public GameObject systemRecommends;
        /// <summary>
        /// 申请好友
        /// </summary>
        public GameObject friendApplyObj;
        /// <summary>
        /// 搜索玩家信息
        /// </summary>
        public string searchMessage;
        /// <summary>
        /// 搜索失败提示
        /// </summary>
        public Text sreachConfirm;
        /// <summary>
        /// 搜索提示
        /// </summary>
        public GameObject sreachConfirms;
        /// <summary>
        /// 是否搜索了
        /// </summary>
        public bool isSreach = false;
        /// <summary>
        /// 搜索是否成功了
        /// </summary>
        public bool isSreachSuccess = false;
        /// <summary>
        /// 下次搜索时间
        /// </summary>
        public float sreachTime = 20f;
        /// <summary>
        /// 滑动组件
        /// </summary>
        public UICircularScrollView<FriendInfo> uICircular_AddFriend;
        public Image FriendAddFriendApplyRedDot;
        public void ADDFriendAwake()
        {
            addfriendReferenceCollector = all_ReferenceCollector.GetGameObject("AddFriendPanel").GetReferenceCollector();
            refreshTimeTxt = addfriendReferenceCollector.GetGameObject("Refresh").transform.Find("refreshTimeTxt").GetComponent<Text>();
            refreshButton = addfriendReferenceCollector.GetGameObject("Refresh").transform.Find("refreshBtn").GetComponent<Button>();
            refreshImage = addfriendReferenceCollector.GetGameObject("Refresh").transform.Find("refreshImage").GetComponent<Image>();
            //刷新按钮
            refreshButton.gameObject.SetActive(true);
            //刷新图片
            refreshImage.gameObject.SetActive(false);
            addfriendReferenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(delegate
            {
                addfriendReferenceCollector.gameObject.SetActive(false);
            });
            refreshButton.onClick.AddSingleListener(delegate
            {
                if(isRefresh)
                {
                    isRefresh = false;
                    refreshButton.gameObject.SetActive(false);
                    refreshImage.gameObject.SetActive(true);
                    RequestRecommendList().Coroutine();
                }
            });
            sreachConfirm = addfriendReferenceCollector.GetText("SreachConfirm");
            sreachConfirms = addfriendReferenceCollector.GetImage("SreachConfirms").gameObject;
            
            addFriendTogs = addfriendReferenceCollector.GetGameObject("AddFriendTogs");
            systemRecommendView = addfriendReferenceCollector.GetImage("systemRecommendView").gameObject.GetComponent<ScrollRect>();
            systemRecommendContent = addfriendReferenceCollector.GetGameObject("systemRecommendContent");
            searchInput = addfriendReferenceCollector.GetInputField("searchInput");
            searchInput.onEndEdit.AddSingleListener(CheakSearchInput);
            findBtn = addfriendReferenceCollector.GetButton("FindBtn");
            findBtn.onClick.AddSingleListener(FindBtnEvent);
            systemRecommends = addfriendReferenceCollector.GetGameObject("SystemRecommends");
            friendApplyObj = addfriendReferenceCollector.GetImage("FriendApply").gameObject;

            FriendAddFriendApplyRedDot = addfriendReferenceCollector.GetImage("FriendMainAddFriendApply");
            UICircularAddFriendInit();

        }
        /// <summary>
        /// 更新添加好友
        /// </summary>
        public void AddFriendUpdate()
        {
            if (!isRefresh)
            {
                sreachTime -= Time.deltaTime;
                refreshTimeTxt.text = $"{(int)sreachTime}s";
                if(sreachTime <= 0.1f)
                {
                    isRefresh = true;
                    refreshTimeTxt.text = "换一批";
                    sreachTime = 20f;
                    refreshButton.gameObject.SetActive(true);
                    refreshImage.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// 点击添加好友按钮初始化添加好友界面变量
        /// </summary>
        public void InitAddFriend()
        {
            isSreachSuccess = false;
            isSreach = false;
            searchInput.text = null;
            sreachConfirm.gameObject.SetActive(false);
            sreachConfirms.gameObject.SetActive(false);
            systemRecommends.SetActive(true);
            systemRecommends.transform.GetChild(0).gameObject.SetActive(true);
            friendApplyObj.SetActive(false);

            RequestRecommendList().Coroutine();
            for (int i = 0; i < addFriendTogs.transform.childCount; i++)
            {
                E_AddFriendsTogNewType type = FriendListData.GetE_AddFriendsTogNewType(i);
                addFriendTogs.transform.GetChild(i).GetComponent<Toggle>().onValueChanged.AddListener((value) => AddFriendTogEvent(value, type));
            }
            addFriendTogs.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            AddFriendRedDotCheack();
        }
        /// <summary>
        /// 添加好友红点检测
        /// </summary>
        public void AddFriendRedDotCheack()
        {
            if (RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_Friend_AddFirend_FirendApply) > 0)
            {
                FriendAddFriendApplyRedDot.gameObject.SetActive(true);
            }
            else
            {
                FriendAddFriendApplyRedDot.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 检查输入框是否为空
        /// </summary>
        /// <param name="value"></param>
        public void CheakSearchInput(string value)
        {
            searchMessage = value;
        }
        /// <summary>
        /// 搜索好友按钮事件
        /// </summary>
        public void FindBtnEvent()
        {
            if (string.IsNullOrEmpty(searchMessage))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "搜索好友不能为空");
                return;
            }
            if (searchMessage == roleEntity.RoleName)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "不能搜索自己");
                return;
            }
            //请求搜索玩家
            SeachFriend(searchMessage).Coroutine();
        }
        /// <summary>
        /// 请求搜索好友
        /// </summary>
        /// <param name="seachName"></param>
        /// <returns></returns>
        public async ETTask SeachFriend(string seachName)
        {
          
            G2C_SearchForFriendsResponse g2C = (G2C_SearchForFriendsResponse)await SessionComponent.Instance.Session.Call(new C2G_SearchForFriendsRequest
            {
                CharName = seachName,
            });
            
            if(g2C.Error != 0)
            {
                if (g2C.Error == 214005)
                {
                    //清空推荐列表
                    FriendListData.RecommendList.Clear();
                 
                    //玩家不存在
                    isSreach = true;//搜索过了
                    isSreachSuccess = false;//搜索失败
                    systemRecommends.transform.GetChild(0).gameObject.SetActive(false);//隐藏系统推荐文字提示
                    sreachConfirms.gameObject.SetActive(true);//显示搜索结果文字提示
                    sreachConfirm.gameObject.SetActive(true);//玩家不存在提示
                    searchInput.text = null;//输入框至null
                    uICircular_AddFriend.Items = FriendListData.RecommendList;//刷新列表
                    return;
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C.Error.GetTipInfo()}");
                }
            }
            else
            {
                FriendListData.RecommendList.Clear();
              
                isSreach = true;//搜索过了
                isSreachSuccess = true;//搜索成功
                sreachConfirms.gameObject.SetActive(true);//显示搜索结果文字提示
                systemRecommends.transform.GetChild(0).gameObject.SetActive(false);//隐藏系统推荐文字提示
                sreachConfirm.gameObject.SetActive(false);//隐藏玩家不存在提示
                //添加到列表
                if (!FriendListData.RecommendList.Exists((f) => f.NickName == g2C.CharName))
                {
                    FriendListData.RecommendList.Add(new FriendInfo()
                    {
                        NickName = g2C.CharName,
                        UUID = g2C.GameUserId,
                        Level = g2C.ILV
                    });
                }
                searchInput.text = null;
                uICircular_AddFriend.Items = FriendListData.RecommendList;//刷新列表
            }
        }
        /// <summary>
        /// 好友按钮事件
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="type"></param>
        public void AddFriendTogEvent(bool isOn, E_AddFriendsTogNewType type)
        {
            if (!isOn) return;
            switch (type)
            {
                case E_AddFriendsTogNewType.Recommend:
                    if (isSreach)
                    {
                        //如果是搜索过的,显示搜索的内容
                        systemRecommends.transform.GetChild(0).gameObject.SetActive(false);
                        sreachConfirms.gameObject.SetActive(true);
                        if (!isSreachSuccess)
                        {
                            //如果搜索没成功,显示玩家不存在
                            sreachConfirm.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        //如果没搜索,显示推荐内容
                        systemRecommends.transform.GetChild(0).gameObject.SetActive(true);
                        //InitAddFriendList().Coroutine();
                    }
                    //显示推荐列表
                    systemRecommends.SetActive(true);
                    //隐藏申请列表
                    friendApplyObj.SetActive(false);
                    //刷新推荐列表
                    uICircular_AddFriend.Items = FriendListData.RecommendList;
                    break;
                case E_AddFriendsTogNewType.Apply:
                    //申请申请添加列表
                    ApplyInit();
                    //隐藏搜索提示
                    sreachConfirms.gameObject.SetActive(false);
                    //隐藏系统推荐
                    systemRecommends.SetActive(false);
                    //隐藏搜索不存在提示
                    sreachConfirm.gameObject.SetActive(false);
                    //显示好友申请
                    friendApplyObj.SetActive(true);
                    break;
                default:
                    break;
            }
            //InitFirendList().Coroutine();
        }
       
        /// <summary>
        /// 初始化添加好友
        /// </summary>
        public void UICircularAddFriendInit()
        {
            uICircular_AddFriend = ComponentFactory.Create<UICircularScrollView<FriendInfo>>();
            uICircular_AddFriend.InitInfo(E_Direction.Vertical, 1, 0, 10);
            uICircular_AddFriend.ItemInfoCallBack = InitAddFriendItem;
            uICircular_AddFriend.IninContent(systemRecommendContent, systemRecommendView);
            uICircular_AddFriend.Items = FriendListData.RecommendList;
        }
        /// <summary>
        /// 搜索,推荐好友进入初始化
        /// </summary>
        /// <param name="item"></param>
        /// <param name="friendInfo"></param>
        private void InitAddFriendItem(GameObject item, FriendInfo friendInfo)
        {
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(-836, 100);
            item.transform.Find("NickName").GetComponent<Text>().text = friendInfo.NickName;
            item.transform.Find("Level").GetComponent<Text>().text = "Level:" + friendInfo.Level.ToString();
            //拉黑屏蔽按钮
            item.transform.Find("ShieldBtn").GetComponent<Button>().onClick.AddSingleListener(delegate
            {
                RequestDelectFriend(0, 0, friendInfo).Coroutine();
            });
            item.transform.Find("AddFriendBtn").GetComponent<Button>().onClick.AddSingleListener(delegate
            {
                RequestAddFriend(friendInfo).Coroutine();
            });
        }

        /// <summary>
        /// 请求添加好友
        /// </summary>
        /// <param name="friendInfo"></param>
        /// <returns></returns>
        public async ETTask RequestAddFriend(FriendInfo friendInfo)
        {
            G2C_AddFriendsResponse g2C_ = (G2C_AddFriendsResponse)await SessionComponent.Instance.Session.Call(new C2G_AddFriendsRequest()
            {
                GameUserId = friendInfo.UUID,
                CharName = friendInfo.NickName
            });
            if(g2C_.Error != 0)
            {
              
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_.Error.GetTipInfo()}");
            }
            else
            {
         
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"好友申请发送成功");
            }
        }
        /// <summary>
        /// 请求好友推荐列表
        /// </summary>
        /// <returns></returns>
        public async ETTask RequestRecommendList()
        {
            try
            {
                FriendListData.RecommendList.Clear();
                G2C_FriendRecommendResponse g2C_FriendRecommend = (G2C_FriendRecommendResponse)await SessionComponent.Instance.Session.Call(new C2G_FriendRecommendRequest());
                if (g2C_FriendRecommend.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,g2C_FriendRecommend.Error.GetTipInfo());
                }
                else
                {
                    foreach (var item in g2C_FriendRecommend.FList)
                    {
                        FriendListData.RecommendList.Add(new FriendInfo()
                        {
                            UUID = item.GameUserId,
                            NickName = item.CharName,
                            Level = item.ILV
                        });
                    }
                    //刷新
                    uICircular_AddFriend.Items = FriendListData.RecommendList;
                }
            }
            catch (System.Exception e)
            {
                Log.DebugRed(e.ToString());
            }
           
        }
    }

}
