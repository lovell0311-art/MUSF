using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;

namespace ETHotfix
{
    public enum E_ChatType
    {
        World = 1,//系统
        All = 2,//世界
        FullSuit = 3,//全服
        Near,//附近
        Family,//战盟
        Team,//队伍
        PrivateChat,//私聊
        FullSuit1
    }
    [ObjectSystem]
    public class UIChatPanelComponentAwake : AwakeSystem<UIChatPanelComponent>
    {
        public override void Awake(UIChatPanelComponent self)
        {

            self.roleEntity = UnitEntityComponent.Instance.LocalRole;
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("Close").onClick.AddSingleListener(self.Close);//关闭面板
            self.Togs = self.collector.GetGameObject("Togs").transform;
            self.Init_Togs();

            self.IsAll = false;
            self.InputMessage_1 = self.collector.GetInputField("InputMessage_1");
            self.InputMessage_1.characterLimit = self.LimitInputCount;
            self.InputMessage_1.onValueChanged.AddSingleListener(self.InputEndEvent);
            self.sendBtn = self.collector.GetButton("SendBtn");
            self.sendBtn.onClick.AddSingleListener(self.SendMessageEvent);

            self.scrollRect = self.collector.GetImage("Scroll View").GetComponent<ScrollRect>();
            self.Content = self.collector.GetGameObject("Content").transform;
            
            self.input = self.collector.GetGameObject("Input");
            self.InputName = self.input.transform.GetChild(1).GetComponent<InputField>();
            self.InputName.onEndEdit.AddSingleListener(self.InputEndEvent_2);

            self.selectInput = self.collector.GetGameObject("SelectInput");
            self.dropdown = self.selectInput.transform.Find("Dropdown").GetComponent<Dropdown>();
            self.dropdown.onValueChanged.AddListener((value) =>
            {
                self.curChatType = value + 3;
            });


            self.InputMessage_2 = self.input.transform.GetChild(0).GetComponent<InputField>();
            self.InputMessage_2.characterLimit = self.LimitInputCount;
            self.InputMessage_2.onValueChanged.AddSingleListener(self.InputEndEvent);

            self.InputMessage_All = self.selectInput.transform.Find("InputMessage_All").GetComponent<InputField>();
            self.InputMessage_All.characterLimit = self.LimitInputCount;
            self.InputMessage_All.onValueChanged.AddSingleListener(self.InputEndEvent);

            
            self.InputMessage_1.gameObject.SetActive(false);
            self.sendBtn.gameObject.SetActive(false);
            self.input.gameObject.SetActive(false);
            self.selectInput.gameObject.SetActive(false);
            self.SetDropDown();
            self.Init_UICircular();
            self.curChatType = 1;
            self.LastSendMessage = string.Empty;
        }
    }
    [ObjectSystem]

    public class UIChatPanelComponentUpdate : UpdateSystem<UIChatPanelComponent>
    {
        public override void Update(UIChatPanelComponent self)
        {
            if (!self.sendBtn.interactable)//上一次发送的消息部位空
            {
                // Log.DebugGreen($"聊天间隔时间：{self.TempTiem}");
                self.curspaceTime += Time.deltaTime;
                self.curWaitTime += Time.deltaTime;
                if (self.curWaitTime >= 1)
                {
                    self.InputMessage_1.placeholder.GetComponent<Text>().text = $"{--self.TempTiem}秒 再发送消息";
                    self.InputMessage_2.placeholder.GetComponent<Text>().text = $"{self.TempTiem}秒 再发送消息";
                    self.InputMessage_All.placeholder.GetComponent<Text>().text = $"{self.TempTiem}秒 再发送消息";
                    self.curWaitTime = 0;
                }
                self.InputMessage_1.interactable = false;
                self.InputMessage_2.interactable = false;
                self.InputMessage_All.interactable = false;
                if (self.curspaceTime >= self.SpaceTime)
                {
                    self.curspaceTime = 0;
                    self.InputMessage_1.placeholder.GetComponent<Text>().text = $"聊天内容不能超过15字....";
                    self.InputMessage_2.placeholder.GetComponent<Text>().text = $"聊天内容不能超过15字....";
                    self.InputMessage_All.placeholder.GetComponent<Text>().text = $"聊天内容不能超过15字....";
                    self.sendBtn.interactable = true;
                    self.InputMessage_1.interactable = true;
                    self.InputMessage_2.interactable = true;
                    self.InputMessage_All.interactable = true;
                }
            }

            if(self.uIIntroduction != null)
            {
                if (/*Input.anyKeyDown || */Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.LeftMouse))
                {
                    if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name != "UIIntroduction")
                    {
                        UIComponent.Instance.Remove(UIType.UIIntroduction);
                        self.uIIntroduction = null;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 聊天组件
    /// </summary>
    public class UIChatPanelComponent : Component
    {
        public ReferenceCollector collector;
        public RoleEntity roleEntity;

        public Transform Togs;
        public InputField InputMessage_1;
        public InputField InputName;
        public InputField InputMessage_2;
        public InputField InputMessage_All;
        public GameObject input, selectInput;
        public Dropdown dropdown;
        List<Dropdown.OptionData> listOptions = new List<Dropdown.OptionData>();

        public bool IsAll = false;
        public int LimitInputCount = 15;//聊天字数限制

        public string SendMessages;
        public string SendName;

        public string LastSendMessage;//上一次发送的消息

        public int curChatType;//当前选择聊天频道
        public E_ChatType _ChatType = E_ChatType.All;

        public int spaceTime = 10;//默认间隔时间为3秒
        public int spaceMaxTime = 10;//发相同的聊天信息 时间间隔为60秒、

        public int levelLimit = 100;//世界聊天等级限制
        public int TypeIndex = 0;
        public float curspaceTime = 0;
        public float curWaitTime = 0;

        public Button sendBtn;

        public UICircularScrollView<ChatMessage> UICircular;
        public Transform Content;
        public ScrollRect scrollRect;

        public UIIntroductionComponent uIIntroduction;//物品简介组件

        public float TempTiem;
        public float SpaceTime;
        public long CachePlayerID = 0;//缓存私聊中输入玩家的ID
        public void Init_UICircular()
        {

            UICircular = ComponentFactory.Create<UICircularScrollView<ChatMessage>>();
            UICircular.InitInfo(E_Direction.Vertical, 1, 0, 0);
            UICircular.ItemInfoCallBack = InitChatMessage;
            UICircular.ItemClickCallBack = ClickChatMessage;
            UICircular.IninContent(Content.gameObject, scrollRect);
            UICircular.Items = ChatMessageDataManager.WorldMessage;
            scrollRect.verticalNormalizedPosition = 0f; //显示顶部

        }
        public void SetDropDown()
        {
            dropdown.ClearOptions();
            listOptions.Clear();
            listOptions.Add(new Dropdown.OptionData("全服"));
            listOptions.Add(new Dropdown.OptionData("附近"));
            listOptions.Add(new Dropdown.OptionData("战盟"));
            listOptions.Add(new Dropdown.OptionData("队伍"));
            dropdown.AddOptions(listOptions);
        }
        public void ClickChatMessage(GameObject item, ChatMessage chat)
        {
            //Log.DebugBrown($"发送玩家名：{chat.sendUserName}，自己：{roleEntity.RoleName}");
            if(chat.chatType == ChatMessageType.NormalChat)
            {
                if (!string.IsNullOrEmpty(chat.sendUserName) && chat.sendUserName != roleEntity.RoleName)
                {
                    TogEvent(true, E_ChatType.PrivateChat);
                    CheakNameOnLineOrExist(chat.sendUserName).Coroutine();
                    InputName.text = chat.sendUserName;
                    Togs.GetChild(6).GetComponent<Toggle>().isOn = true;
                    SendName = chat.sendUserName;
                }
            }
            else if(chat.chatType == ChatMessageType.ItemChat && (_ChatType == E_ChatType.All|| _ChatType == E_ChatType.FullSuit))
            {
                //Log.DebugGreen("这是件装备");
                if(chat.itemId != 0)
                {
                    BagShareGetInfoRequest(chat.itemId).Coroutine();
                }
            }
        }

        //查看装备
        public async ETTask BagShareGetInfoRequest(long itemId)
        {
            G2C_BagShareGetInfoResponse g2C_BagShare = (G2C_BagShareGetInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_BagShareGetInfoRequest()
            {
                ShareItemId = itemId
            });
            if(g2C_BagShare.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BagShare.Error.GetTipInfo());
            }
            else
            {
                KnapsackDataItem knapsackData = new KnapsackDataItem();
                knapsackData.ConfigId = g2C_BagShare.AllProperty.ConfigId;
                for (int p = 0, pcount = g2C_BagShare.AllProperty.PropList.Count; p < pcount; p++)
                {
                    knapsackData.Set(g2C_BagShare.AllProperty.PropList[p]);
                }

                for (int e = 0, count = g2C_BagShare.AllProperty.ExecllentEntry.Count; e < count; e++)
                {
                    knapsackData.SetExecllentEntry(g2C_BagShare.AllProperty.ExecllentEntry[e]);
                }
                for (int e = 0, count = g2C_BagShare.AllProperty.SpecialEntry.Count; e < count; e++)
                {
                    knapsackData.SetSpecialEntry(g2C_BagShare.AllProperty.SpecialEntry[e]);
                }
                uIIntroduction ??= UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
                uIIntroduction.GetAllAtrs(knapsackData, E_KnapsackIntroduceShowPrice.None);
                uIIntroduction.ShowAtr_Vertical();
            }
        }

        /// <summary>
        /// 初始化 聊天信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="chat"></param>
        public void InitChatMessage(GameObject item, ChatMessage chat)
        {
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            System.DateTime dt = startTime.AddSeconds(chat.sendTime);
            string t = dt.ToString("yyyy/MM/dd HH:mm:ss");//转化为日期时间
            item.transform.Find("time").GetComponent<Text>().text = t;


            string textContent = "";
            if (TypeIndex == 3)
            {
                textContent = $"{ChatMessageDataManager.GetChatType(E_ChatType.FullSuit1)}{chat.curLine}线 {chat.sendUserName}：{chat.chatMessage}";
            }
            else
            {
                textContent = (E_ChatType)chat.curRoonID ==
                E_ChatType.FullSuit ? $"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)}{chat.curLine}线 {chat.sendUserName}：{chat.chatMessage}" :
                $"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)}{chat.sendUserName}：{chat.chatMessage}";
            }

            textContent = textContent.Replace(" ", "\u3000");
            item.transform.Find("info").GetComponent<Text>().text = textContent;

            //string textContent = (E_ChatType)chat.curRoonID ==
            //    E_ChatType.FullSuit ? $"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)}{chat.curLine}线 {chat.sendUserName}：{chat.chatMessage}" :
            //    $"{ChatMessageDataManager.GetChatType((E_ChatType)chat.curRoonID)}{chat.sendUserName}：{chat.chatMessage}";
            //textContent = textContent.Replace(" ", "\u3000");
            //item.transform.Find("info").GetComponent<Text>().text = textContent;
        }
        /// <summary>
        /// 消息缓存达到一定数量后 移除第一个消息
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="index">移除的索引</param>
        /// <param name="MaxCount">缓存的最大数量</param>
        public void RemoveAtMessage(ref List<ChatMessage> messages, int index, int MaxCount)
        {
            if (messages.Count >= MaxCount)
                messages.RemoveAt(index);
        }
        /// <summary>
        /// 初始化聊天类型页签
        /// </summary>
        public void Init_Togs()
        {
            for (int i = 0, length = Togs.transform.childCount; i < length; i++)
            {
                Toggle toggle = Togs.GetChild(i).GetComponent<Toggle>();
                int type = i + 1;
                toggle.onValueChanged.AddSingleListener((value) => { TogEvent(value, (E_ChatType)type); });
            }

        }
        /// <summary>
        /// 显示私聊
        /// </summary>

        public void ShwoPrivateChat()
        {
            Togs.transform.Find("Tog_Private").GetComponent<Toggle>().isOn = true;

        }

        private void TogEvent(bool isOn, E_ChatType type)
        {
            TypeIndex = (int)type;
            if (!isOn) return;
            IsAll = false;
            _ChatType = type;
            switch (type)
            {

                case E_ChatType.All://全部
                    IsAll = true;
                    curChatType = 3;
                   // SetDropDownItemValue(0);
                    UICircular.Items = ChatMessageDataManager.AllMessage;
                    //input.gameObject.SetActive(false);
                    //InputMessage_1.gameObject.SetActive(false);
                    //selectInput.gameObject.SetActive(true);
                    //sendBtn.gameObject.SetActive(true);

                    input.gameObject.SetActive(false);
                    InputMessage_1.gameObject.SetActive(true);
                    sendBtn.gameObject.SetActive(true);
                    selectInput.gameObject.SetActive(false);
                    sendBtn.enabled = true;
                    InputMessage_1.enabled = true;
                    break;
                case E_ChatType.World://世界

                    curChatType = 1;
                    UICircular.Items = ChatMessageDataManager.WorldMessage;
                    InputMessage_1.gameObject.SetActive(false);
                    sendBtn.gameObject.SetActive(false);
                    input.gameObject.SetActive(false);
                    selectInput.gameObject.SetActive(false);
                    if (roleEntity.Level < levelLimit)
                    {
                        //input.gameObject.SetActive(false);
                        //InputMessage_1.gameObject.SetActive(false);
                        //sendBtn.gameObject.SetActive(false);
                        InputMessage_1.enabled = false;
                        sendBtn.enabled = false;
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足100级");
                        //return;
                    }
                    else
                    {
                        sendBtn.enabled = true;
                        InputMessage_1.enabled = true;
                    }
                    break;
                case E_ChatType.FullSuit://全服
                    //UIComponent.Instance.VisibleUI(UIType.UIHint,$"附近聊天 还未开放");
                    curChatType = 3;
                    UICircular.Items = ChatMessageDataManager.FullSuitMessage;
                    input.gameObject.SetActive(false);
                    InputMessage_1.gameObject.SetActive(true);
                    sendBtn.gameObject.SetActive(true);
                    selectInput.gameObject.SetActive(false);
                    sendBtn.enabled = true;
                    InputMessage_1.enabled = true;


                    break;
                case E_ChatType.Near://附近
                    //UIComponent.Instance.VisibleUI(UIType.UIHint,$"附近聊天 还未开放");
                    curChatType = 4;
                    UICircular.Items = ChatMessageDataManager.NearMessage;
                    input.gameObject.SetActive(false);
                    InputMessage_1.gameObject.SetActive(true);
                    sendBtn.gameObject.SetActive(true);
                    selectInput.gameObject.SetActive(false);
                    sendBtn.enabled = true;
                    InputMessage_1.enabled = true;
                    break;
                case E_ChatType.Family://家族
                                       //  Log.DebugGreen($"战盟成员：{WarAllianceDatas.WarLists.Count}");

                    JoinTheWarAllianceRequest().Coroutine();

                    break;
                case E_ChatType.Team://队伍
                    if (TeamDatas.MyTeamState == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"队伍聊天 还未开放");
                        InputMessage_1.gameObject.SetActive(false);
                        sendBtn.gameObject.SetActive(false);
                        input.gameObject.SetActive(false);
                        return;
                    }
                    curChatType = 6;
                    InputMessage_1.gameObject.SetActive(true);
                    sendBtn.gameObject.SetActive(true);
                    selectInput.gameObject.SetActive(false);
                    UICircular.Items = ChatMessageDataManager.TeamMessage;
                    break;
                case E_ChatType.PrivateChat://私聊
                    curChatType = 7;
                    UICircular.Items = ChatMessageDataManager.PrivateMessage;
                    InputMessage_1.gameObject.SetActive(false);
                    sendBtn.gameObject.SetActive(true);
                    selectInput.gameObject.SetActive(false);
                    input.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
            scrollRect.verticalNormalizedPosition = 0f; //显示顶部
            async ETVoid JoinTheWarAllianceRequest()
            {
                G2C_JoinTheWarAllianceResponse g2C_JoinTheWarAlliance = (G2C_JoinTheWarAllianceResponse)await SessionComponent.Instance.Session.Call(new C2G_JoinTheWarAllianceRequest { });
                // Log.DebugGreen($"战盟{g2C_JoinTheWarAlliance.Error}");
                if (g2C_JoinTheWarAlliance.Error != 0)
                {

                    WarAllianceDatas.IsJoinWar = false;
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"战盟聊天 还未开放");
                    InputMessage_1.gameObject.SetActive(false);
                    sendBtn.gameObject.SetActive(false);
                    input.gameObject.SetActive(false);
                    return;
                }
                else
                {

                    WarAllianceDatas.IsJoinWar = true;
                    curChatType = 5;
                    InputMessage_1.gameObject.SetActive(true);
                    sendBtn.gameObject.SetActive(true);
                    selectInput.gameObject.SetActive(false);
                    sendBtn.enabled = true;
                    InputMessage_1.enabled = true;
                    UICircular.Items = ChatMessageDataManager.FamilyMessage;
                }
            }
        }
        /// <summary>
        /// 设置选择的下拉Item
        /// </summary>
        /// <param name="ItemIndex"></param>
        void SetDropDownItemValue(int ItemIndex)
        {
            if (dropdown.options == null)
            {

                return;
            }
            if (ItemIndex >= dropdown.options.Count)
            {
                ItemIndex = dropdown.options.Count - 1;
            }

            if (ItemIndex < 0)
            {
                ItemIndex = 0;
            }

            dropdown.value = ItemIndex;
        }

        /// <summary>
        /// 消息输入框 事件
        /// </summary>
        /// <param name="value">输入的类容</param>
        public void InputEndEvent(string value)
        {
            SendMessages = value;

            if (SendMessages.Contains("\n"))
                SendMessages = SendMessages.Replace("\n", "");
            if (SendMessages.Contains("\t"))
                SendMessages = SendMessages.Replace("\t", "");
        }
        /// <summary>
        /// 消息输入框 事件2
        /// </summary>
        /// <param name="value">输入的类容</param>
        public void InputEndEvent_2(string value)
        {
            CheakNameOnLineOrExist(value).Coroutine();
        }

        public async ETVoid CheakNameOnLineOrExist(string value)
        {
            G2C_GetPlayerInfoByNickName g2C = (G2C_GetPlayerInfoByNickName)await SessionComponent.Instance.Session.Call(new C2G_GetPlayerInfoByNickName
            {
                NickName = value
            });
            if (g2C.Error != 0)
            {
                CachePlayerID = 0;
                InputName.text = null;

                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C.Error.GetTipInfo()}");
            }
            else
            {
                CachePlayerID = g2C.GameUserId;
                SendName = value;
            }
        }

        /// <summary>
        /// 发送消息事件
        /// </summary>
        public void SendMessageEvent()
        {
            //TODO 校验等级是否合格

            if (curChatType == 7)
            {
                if (string.IsNullOrEmpty(SendName))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请先输入名字");
                    return;
                }
            }
            if (string.IsNullOrEmpty(SendMessages))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入信息");
                return;
            }
            TestWords();
            //判断输入的内容是否包含 违规字符
            if (SystemUtil.IsInvaild(SendMessages))
            {
                //替换掉违规字符
                SendMessages = SystemUtil.ReplaceStr(SendMessages);
            }
            //if (CountDigitsInString(SendMessages) > 10)
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "输入信息有误");
            //    return;
            //}
            //if (CountDigitsInString(SendMessages) > 7)
            //{
            //    SendMessages = ReplaceDigits(SendMessages, '*');
            //}
            Debug.Log("发出" + SendMessages);
            SendMessageAsync().Coroutine();
        }
        public string ReplaceDigits(string originalString, char replacement)
        {
            // 使用正则表达式替换所有数字
            return Regex.Replace(originalString, @"\d", replacement.ToString());
        }
        public int CountDigitsInString(string str)
        {
            int count = 0;
            foreach (char c in str)
            {
                if (char.IsDigit(c))
                {
                    count++;
                }
            }
            return count;
        }

        private void TestWords()
        {
            string[] sensitiveWords = new[] { "微信", "QQ", "福利", "新服", "群", "国家", "党", "洗澡", "小姐姐", "鸡", "胸", "屁股", "奶", "乳", "草", "操", "妈", "尼玛", "玩家", "扶持", "内部", "中国", "共产", "逼", "叼", "屌", "吊", "政治", "政府", "性", "舔", "淫", "抖音", "直播", "中央", "联系", "快手", "托", "狗", "贱", "充值", "代金", "券", "色", "挂", "干", "交易", "奶子", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            InitSensitiveWordMap(sensitiveWords);
            string testWords = SendMessages;
            SendMessages = ReplaceSensitiveWords(testWords);

            Debug.Log($"测试字符串:{testWords}");
            Debug.Log($"输出字符串:{SendMessages}");
        }
        private const string END_FLAG = "IsEnd";
        private static Hashtable _hashtable = new Hashtable();

        /// <summary>
        /// 替换 需要剔除的 敏感字
        /// </summary>
        /// <param name="text">需要处理的文本</param>
        /// <returns></returns>
        public static string ReplaceSensitiveWords(string text)
        {
            int i = 0;
            StringBuilder builder = new StringBuilder(text);
            while (i < text.Length)
            {
                int len = SearchSensitiveWord(text, i);
                if (len > 0)
                {
                    for (int j = 0; j < len; j++)
                    {
                        builder[i + j] = '*';
                    }
                    i += len;
                }
                else ++i;
            }
            return builder.ToString();
        }

        /// <summary>
        /// 判断是否是一个符号
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsSymbol(char c)
        {
            int ic = c;
            // 0x2E80-0x9FFF 东亚文字范围
            return !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) && (ic < 0x2E80 || ic > 0x9FFF);
        }

        /// <summary>
        /// 查找所有敏感词，找到则返回敏感词长度
        /// </summary>
        /// <param name="text">需要过滤的字符串</param>
        /// <param name="startIndex">查找的起始位置</param>
        /// <returns></returns>
        public static int SearchSensitiveWord(string text, int startIndex)
        {
            Hashtable newMap = _hashtable;
            bool flag = false;
            int len = 0;
            for (int i = startIndex; i < text.Length; i++)
            {
                char word = text[i];
                if (IsSymbol(word))
                {
                    len++;
                    continue;
                }
                Hashtable temp = (Hashtable)newMap[word];
                if (temp != null)
                {
                    if ((int)temp[END_FLAG] == 1) flag = true;
                    else newMap = temp;
                    len++;
                }
                else break;
            }
            if (!flag) len = 0;
            return len;
        }

        /// <summary>
        /// 初始化 过滤词 词库
        /// </summary>
        public static void InitSensitiveWordMap(string[] worlds)
        {
            _hashtable = new Hashtable(worlds.Length);
            foreach (string word in worlds)
            {
                Hashtable hashtable = _hashtable;
                for (int i = 0; i < word.Length; i++)
                {
                    char c = word[i];
                    if (IsSymbol(c)) continue;
                    if (hashtable.ContainsKey(c))
                    {
                        hashtable = (Hashtable)hashtable[c];
                    }
                    else
                    {
                        var newHashtable = new Hashtable();
                        newHashtable.Add(END_FLAG, 0);
                        hashtable.Add(c, newHashtable);
                        hashtable = newHashtable;
                    }
                    if (i == word.Length - 1)
                    {
                        if (hashtable.ContainsKey(END_FLAG))
                        {
                            hashtable[END_FLAG] = 1;
                        }
                        else
                        {
                            hashtable.Add(END_FLAG, 1);
                        }
                    }
                }
            }
        }
        public async ETVoid SendMessageAsync()
        {
            switch (curChatType)
            {
                case 1:
                    break;
                case 2:
                    if (roleEntity.Level < levelLimit)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足100级");
                        return;
                    }
                    G2C_SendChatMessageToChatRoom g2C_1 = (G2C_SendChatMessageToChatRoom)await SessionComponent.Instance.Session.Call(new C2G_SendChatMessageToChatRoom
                    {
                        ChatRoomID = GlobalDataManager.EnterZoneID,
                        ChatMessage = SendMessages
                    });
                    if (g2C_1.Error != 0)
                    {
                        if (!IsAll)
                        {

                            InputMessage_1.text = string.Empty;
                        }
                        else
                        {
                            InputMessage_All.text = string.Empty;
                        }

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_1.Error.GetTipInfo()}");
                    }
                    else
                    {
                        sendBtn.interactable = false;

                        SpaceTime = !string.IsNullOrEmpty(LastSendMessage) && string.Equals(SendMessages, LastSendMessage) ? spaceMaxTime : spaceTime;
                        LastSendMessage = SendMessages;
                        TempTiem = SpaceTime;
                        if (!IsAll)
                        {
                            InputMessage_1.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_1.text = string.Empty;

                        }
                        else
                        {
                            InputMessage_All.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_All.text = string.Empty;
                        }

                        RefrenshMessage();
                    }
                    break;
                case 3:

                    G2C_FullServiceHornResponse g2C_3 = (G2C_FullServiceHornResponse)await SessionComponent.Instance.Session.Call(new C2G_FullServiceHornRequest
                    {
                        ChatMessage = SendMessages
                    });
                    if (g2C_3.Error != 0)
                    {
                        if (!IsAll)
                        {

                            InputMessage_1.text = string.Empty;
                        }
                        else
                        {
                            InputMessage_All.text = string.Empty;
                        }

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_3.Error.GetTipInfo()}");
                    }
                    else
                    {
                        sendBtn.interactable = false;

                        SpaceTime = !string.IsNullOrEmpty(LastSendMessage) && string.Equals(SendMessages, LastSendMessage) ? spaceMaxTime : spaceTime;
                        LastSendMessage = SendMessages;
                        TempTiem = SpaceTime;
                        if (!IsAll)
                        {
                            InputMessage_1.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_1.text = string.Empty;

                        }
                        else
                        {
                            InputMessage_All.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_All.text = string.Empty;
                        }

                    }
                    break;
                case 4:

                    G2C_SendChatMessageToNearby g2C_4 = (G2C_SendChatMessageToNearby)await SessionComponent.Instance.Session.Call(new C2G_SendChatMessageToNearby
                    {
                        ChatMessage = SendMessages
                    });
                    if (g2C_4.Error != 0)
                    {
                        if (!IsAll)
                        {

                            InputMessage_1.text = string.Empty;
                        }
                        else
                        {
                            InputMessage_All.text = string.Empty;
                        }

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_4.Error.GetTipInfo()}");
                    }
                    else
                    {
                        sendBtn.interactable = false;

                        SpaceTime = !string.IsNullOrEmpty(LastSendMessage) && string.Equals(SendMessages, LastSendMessage) ? spaceMaxTime : spaceTime;
                        LastSendMessage = SendMessages;
                        TempTiem = SpaceTime;
                        if (!IsAll)
                        {
                            InputMessage_1.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_1.text = string.Empty;

                        }
                        else
                        {
                            InputMessage_All.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_All.text = string.Empty;
                        }

                    }
                    break;
                case 5:
                    if (!WarAllianceDatas.IsJoinWar)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"战盟聊天 还未开放");
                        return;
                    }
                    G2C_SendMessageWarAllianceChatResponse g2C_5 = (G2C_SendMessageWarAllianceChatResponse)await SessionComponent.Instance.Session.Call(new C2G_SendMessageWarAllianceChatRequest
                    {
                        ChatMessage = SendMessages
                    });
                    if (g2C_5.Error != 0)
                    {
                        if (!IsAll)
                        {

                            InputMessage_1.text = string.Empty;
                        }
                        else
                        {
                            InputMessage_All.text = string.Empty;
                        }

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_5.Error.GetTipInfo()}");
                    }
                    else
                    {
                        sendBtn.interactable = false;

                        SpaceTime = !string.IsNullOrEmpty(LastSendMessage) && string.Equals(SendMessages, LastSendMessage) ? spaceMaxTime : spaceTime;
                        LastSendMessage = SendMessages;
                        TempTiem = SpaceTime;
                        if (!IsAll)
                        {
                            InputMessage_1.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_1.text = string.Empty;

                        }
                        else
                        {
                            InputMessage_All.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_All.text = string.Empty;
                        }

                        RefrenshMessage();
                    }
                    break;
                case 6:
                    if (TeamDatas.MyTeamState == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"队伍聊天 还未开放");
                        return;
                    }

                    G2C_SendChatMessageToChatRoom g2C_6 = (G2C_SendChatMessageToChatRoom)await SessionComponent.Instance.Session.Call(new C2G_SendChatMessageToChatRoom
                    {
                        ChatRoomID = TeamDatas.ChatRoomId,
                        ChatMessage = SendMessages
                    });
                    if (g2C_6.Error != 0)
                    {
                        if (!IsAll)
                        {

                            InputMessage_1.text = string.Empty;
                        }
                        else
                        {
                            InputMessage_All.text = string.Empty;
                        }

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_6.Error.GetTipInfo()}");
                    }
                    else
                    {
                        sendBtn.interactable = false;

                        SpaceTime = !string.IsNullOrEmpty(LastSendMessage) && string.Equals(SendMessages, LastSendMessage) ? spaceMaxTime : spaceTime;
                        LastSendMessage = SendMessages;
                        TempTiem = SpaceTime;
                        if (!IsAll)
                        {
                            InputMessage_1.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_1.text = string.Empty;

                        }
                        else
                        {
                            InputMessage_All.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                            InputMessage_All.text = string.Empty;
                        }

                        RefrenshMessage();
                    }
                    break;
                case 7:
                    G2C_SendChatMessageToPlayer g2C_7 = (G2C_SendChatMessageToPlayer)await SessionComponent.Instance.Session.Call(new C2G_SendChatMessageToPlayer
                    {
                        PlayerGameUserId = CachePlayerID,
                        ChatMessage = SendMessages
                    });
                    if (g2C_7.Error != 0)
                    {
                        InputMessage_2.text = string.Empty;

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_7.Error.GetTipInfo()}");
                    }
                    else
                    {
                        //只会通知私聊的好友,所以这里要手动加入聊天缓存
                        ChatMessage chat = new ChatMessage()
                        {
                            sendUserName = roleEntity.RoleName,
                            chatMessage = SendMessages,
                            sendTime = (long)SpaceTime,
                            SendGameUserId = roleEntity.Id,
                            curRoonID = 7
                        };
                        ChatMessageDataManager.AddChatMessage(chat);
                        UIMainComponent.Instance.ShowChatInfo(chat);

                        sendBtn.interactable = false;

                        SpaceTime = !string.IsNullOrEmpty(LastSendMessage) && string.Equals(SendMessages, LastSendMessage) ? spaceMaxTime : spaceTime;
                        LastSendMessage = SendMessages;
                        TempTiem = SpaceTime;
                        InputMessage_2.placeholder.GetComponent<Text>().text = $"{TempTiem}秒";
                        InputMessage_2.text = string.Empty;

                        RefrenshMessage();
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 刷新 聊天界面
        /// </summary>
        public void RefrenshMessage()
        {
            if (IsAll)
            {
                UICircular.Items = ChatMessageDataManager.AllMessage;
            }
            else
            {
                UICircular.Items = curChatType.GetCurMessageLis();
            }
            scrollRect.verticalNormalizedPosition = 0f; //显示顶部
        }
        /// <summary>
        /// 两次发送的消息雷人是否相等
        /// </summary>
        /// <returns></returns>
        public bool IsSendMessageEqual()
        {
            if (string.IsNullOrEmpty(LastSendMessage)) return false;
            return string.Equals(SendMessages, LastSendMessage);
        }
        /// <summary>
        /// 关闭面板
        /// </summary>
        public void Close()
        {
            //UIComponent.Instance.Remove(UIType.UIChatPanel);
            if(uIIntroduction != null)
            {
                UIComponent.Instance.Remove(UIType.UIIntroduction);
                uIIntroduction = null;
            }
            UIComponent.Instance.Remove(UIType.UIChatPanel);
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
        }
    }
}