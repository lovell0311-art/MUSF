using UnityEngine;
using UnityEngine.UI;
using ETModel;
using System.Collections.Generic;
using System.Linq;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Text;

namespace ETHotfix
{

    [ObjectSystem]
    public class UICreatComponentAwale : AwakeSystem<UICreatComponent>
    {
        public override void Awake(UICreatComponent self)
        {
            self.Awale();
        }
    }
   
    /// <summary>
    /// 创建角色
    /// </summary>
    public class UICreatComponent : Component
    {
        private GameObject TogGroup;
        private Button StartBtn, CancleBtn;
        private InputField NameInputField;
        private Text RoleInfoTxt,StrText, VolText, AgileText, BackboneText, HpText, CommandText, MagicText;

        private bool ConformTo = false;
        private string SendMessages;//输入的名字内容
        private GameObject currentShowPlayer;//当前选择的角色
        private int roleType = -1;//当前角色的类型
        private string creatroleName;//角色名字
        private int max_Level = 0;//存档角色的最大等级

        public int canCreatRoleIndex = 6;//可创建的角色索引
        private readonly List<Toggle> roleToggles = new List<Toggle>();

        public void Awale()
        {
            ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            TogGroup = collector.GetGameObject("TogGroup");
            RoleInfoTxt = collector.GetText("RoleInfoTxt");//简介
            StrText = collector.GetText("StrText");//体力
            VolText = collector.GetText("VolText");//敏捷   
            AgileText = collector.GetText("AgileText");//体力
            BackboneText = collector.GetText("BackboneText");//智力
            HpText = collector.GetText("HpText");//生命值
            CommandText = collector.GetText("CommandText");//统率
            MagicText = collector.GetText("MagicText");//魔法值
            StartBtn = collector.GetButton("StartBtn");
            CancleBtn = collector.GetButton("CancleBtn");
            NameInputField = collector.GetInputField("NameInputField");
           
            NameInputField.onEndEdit.AddSingleListener(CheackInputNick);
            CancleBtn.onClick.AddSingleListener(OnCancelBtnClick);
            StartBtn.onClick.AddSingleListener(OnYecBtnClick);
            GetCacheRoleMaxLevel();
            InitTogGroups();
            ETModel.Game.Scene.GetComponent<CameraComponent>().RenderCamera.gameObject.SetActive(true);
        }
      
      
        /// <summary>
        /// 获取存档角色的最大等级
        /// </summary>
        /// <returns></returns>
        public void GetCacheRoleMaxLevel()
        {
            foreach (RoleArchiveInfo role in RoleArchiveInfoManager.Instance.roleArchiveInfosDic.Values)
            {
                if (max_Level <= role.Level)
                    max_Level = role.Level;
            }
        }
        /// <summary>
        /// 初始化人物选择按钮
        /// </summary>
        private void InitTogGroups()
        {
            roleToggles.Clear();
            for (int i = 0, length = TogGroup.transform.childCount; i < length; i++)
            {
                Toggle toggle = TogGroup.transform.GetChild(i).GetComponent<Toggle>();
                if (toggle == null)
                {
                    continue;
                }

                toggle.onValueChanged.AddSingleListener(OnAnyRoleToggleChanged);
                int index = i + 1;
                toggle.interactable = IsCanCreat(index);
                roleToggles.Add(toggle);
            }

        }

        private void OnAnyRoleToggleChanged(bool _)
        {
            for (int i = 0; i < roleToggles.Count; ++i)
            {
                Toggle toggle = roleToggles[i];
                if (toggle == null || !toggle.isOn)
                {
                    continue;
                }

                int index = i + 1;
                roleType = index;
                OnTogClick(true, index);
                return;
            }
        }

        /// <summary>
        /// Tog点击事件
        /// </summary>
        /// <param name="ison"></param>
        /// <param name="index"></param>
        private void OnTogClick(bool ison, int index)
        {
           
            if (!ison) return;
            //TODO 有些角色的创建 有等级限制
            //根据配置表索引ID 获取角色配置信息
            CreateRoleConfig_InfoConfig infoConfig = Game.Scene.GetComponent<ConfigComponent>().GetItem<CreateRoleConfig_InfoConfig>(index);
            //显示角色简介和熟悉信息
            RoleInfoTxt.text = infoConfig.Desc;
            StrText.text = "力量：" + infoConfig.Strength;
            VolText.text = "敏捷：" + infoConfig.Agility;
            AgileText.text = "体力：" + infoConfig.BoneGas;
            BackboneText.text = "智力：" + infoConfig.Willpower;
            HpText.text = "生命值：" + infoConfig.Hp;
            CommandText.text = "统率：" + infoConfig.Command;
            MagicText.text = "魔法值：" + infoConfig.Mp;
            //显示模型
            ShowRoleMode(index);

        }
        /// <summary>
        /// 当前角色是否可以创建
        /// </summary>
        /// <param name="roleInde"></param>
        /// <returns></returns>
        private bool IsCanCreat(int roleInde)
        {
            E_RoleType roleType = (E_RoleType)roleInde;
            switch (roleType)
            {
                case E_RoleType.Magician://直接创建
                case E_RoleType.Swordsman://直接创建
                case E_RoleType.Archer://直接创建
                    return true;
                case E_RoleType.Summoner://可直接创建
                case E_RoleType.Magicswordsman://拥有一个等级超过220级的角色 才可以创建
                case E_RoleType.Holymentor://拥有一个等级超过250级的角色 才可以创建
                case E_RoleType.Gladiator://使用【格斗家角色卡片】进行创建
                case E_RoleType.GrowLancer://使用【梦幻骑士角色卡片】进行创建 或者 拥有一个等级超过200级的角色
                    return RoleArchiveInfoManager.Instance.CanCreatRoleList.Contains(roleInde);
            }
            return false;
        }
        /// <summary>
        /// 显示当前角色的模型
        /// </summary>
        /// <param name="roleInde"></param>
        private void ShowRoleMode(int roleInde)
        {
            string roleRes = ((E_RoleType)roleInde).GetRoleResName();//获取对应的资源模型
            if (string.IsNullOrEmpty(roleRes)) return;
            if (currentShowPlayer != null) ResourcesComponent.Instance.RecycleGameObject(currentShowPlayer);//回收上一个 角色模型

            currentShowPlayer = ResourcesComponent.Instance.LoadGameObject(roleRes.StringToAB(), roleRes);//显示当前角色的模型
            Animator animator = currentShowPlayer.transform.Find("Role").GetComponent<Animator>();
            AssetBundleComponent.Instance.LoadBundle($"Animator_{roleRes}".StringToAB());
            animator.runtimeAnimatorController = (RuntimeAnimatorController)AssetBundleComponent.Instance.GetAsset($"Animator_{roleRes}".StringToAB(), $"Animator_{roleRes}");
            currentShowPlayer.transform.position = new Vector3(-5,1.2f,4);//设置模型的位置
            currentShowPlayer.transform.localRotation = Quaternion.identity;



        }
        #region Button点击事件
        private void OnCancelBtnClick()
        {
            roleType = -1;
            Game.Scene.GetComponent<UIComponent>().VisibleUI(UIType.UIChooseRole);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICreatRole);
        }
        /// <summary>
        /// 检查玩家输入的昵称是否违法
        /// </summary>
        /// <param name="value"></param>
        private void CheackInputNick(string value)
        {
            SendMessages = value;
            TestWords();
            //昵称是否 包含非法字符 
            if (SystemUtil.IsInvaild(value)||ConformTo==true)
            {
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText($"昵称{ SystemUtil.ReplaceStr(value)}包含 非法字符 请你重新输入昵称", true);
                NameInputField.text = string.Empty;
            }
            else
            {
                //昵称最多六个字
                creatroleName = NameInputField.text;
            }
        }
        private void OnYecBtnClick()
        {

            /*if (roleType > 3)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "当前不能创建该角色");
                return;
            }
            */
            if (roleType == -1)
            {
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText($"请选择一个你喜爱的角色", true);
                return;
            }
            if (string.IsNullOrEmpty(creatroleName))
            {
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText($"请输入角色昵称", true);
                return;
            }
           
            //创建角色
            CreatNewRole().Coroutine();
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
        private const string END_FLAG = "IsEnd";
        private static Hashtable _hashtable = new Hashtable();

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

        /// <summary>
        /// 替换 需要剔除的 敏感字
        /// </summary>
        /// <param name="text">需要处理的文本</param>
        /// <returns></returns>
        public  string ReplaceSensitiveWords(string text)
        {
            ConformTo = false;
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
                        ConformTo = true;
                    }
                    i += len;
                }
                else ++i;
            }
            return builder.ToString();
        }
        private void TestWords()
        {
            string[] sensitiveWords = new[] { "微信", "QQ", "福利", "新服", "群", "国家", "党", "洗澡", "小姐姐", "鸡", "胸", "屁股", "奶", "乳", "草", "操", "妈", "尼玛", "玩家", "扶持", "内部", "中国", "共产", "逼", "叼", "屌", "吊", "政治", "政府", "性", "舔", "淫", "抖音", "直播", "中央", "联系", "快手", "托", "狗", "贱", "充值", "代金", "券", "色", "挂", "干", "交易", "奶子", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "星辰战纪" };
            InitSensitiveWordMap(sensitiveWords);
            string testWords = SendMessages;
            SendMessages = ReplaceSensitiveWords(testWords);

            Debug.Log($"测试字符串:{testWords}");
            Debug.Log($"输出字符串:{SendMessages}");
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <returns></returns>
        private async ETVoid CreatNewRole()
        {
            StartBtn.interactable = false;//防止多次点击
            G2C_LoginSystemCreateGamePlayerResponse g2C_GamePlayerCreateResponse = (G2C_LoginSystemCreateGamePlayerResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemCreateGamePlayerRequest
            {
                PlayerType = roleType,
                NickName = creatroleName
            });
            Log.DebugBrown("G2C_LoginSystemCreateGamePlayerResponse创建角色返回" + g2C_GamePlayerCreateResponse.Error);
            //角色创建失败
            if (g2C_GamePlayerCreateResponse.Error != 0)
            {
                NameInputField.text = string.Empty;
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText(g2C_GamePlayerCreateResponse.Error.GetTipInfo(), true);
                StartBtn.interactable = true;
                return;

            }
            else
            {
                if (ETModel.Init.instance.e_SDK == E_SDK.XY_SDK)
                {
                    //上报 sdk里面的 createRole 和 updateRoleGrade
                    SdkCallBackComponent.Instance.sdkUtility.CreatRole(new string[] { $"{GlobalDataManager.XYUUID}", $"{GlobalDataManager.EnterZoneID}", $"{GlobalDataManager.EnterZoneName}", $"{g2C_GamePlayerCreateResponse.GameIds[0]}", $"{creatroleName}", $"{TimeHelper.GetTimestamp()}", "" });
                    SdkCallBackComponent.Instance.sdkUtility.UpdateRoleGrade(new string[] { $"{GlobalDataManager.XYUUID}", $"{GlobalDataManager.EnterZoneID}", $"{GlobalDataManager.EnterZoneName}", $"{g2C_GamePlayerCreateResponse.GameIds[0]}", $"{creatroleName}", $"{1}", "" });
                }
            }
            //缓存当前创建的角色
            LastRoelInfo lastRoelInfo = new LastRoelInfo { LastRoleUUID = g2C_GamePlayerCreateResponse.GameIds.Last() };
            LocalDataJsonComponent.Instance.SavaData(lastRoelInfo, LocalJsonDataKeys.LastRoleInfo);
            //角色创建成功
            //获取角色信息
            G2C_LoginSystemGetGamePlayerInfoResponse g2C_GamePlayerGetInfoResponse = (G2C_LoginSystemGetGamePlayerInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemGetGamePlayerInfoRequest
            {
                GameId = g2C_GamePlayerCreateResponse.GameIds
            });
           // Log.DebugGreen($"角色创建成功：{JsonHelper.ToJson(g2C_GamePlayerCreateResponse.GameIds)}");
            //提示错误信息
            if (g2C_GamePlayerGetInfoResponse.Error != 0)
            {
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText(g2C_GamePlayerGetInfoResponse.Error.GetTipInfo(), true);
                StartBtn.interactable = true;
                return;
            }
            else
            {
                //缓存角色信息 显示角色使用
                for (int i = 0, length = g2C_GamePlayerGetInfoResponse.GameInfos.count; i < length; i++)
                {
                    //缓存 角色存档
                    G2C_LoginSystemGetGamePlayerInfoMessage roleInfos = g2C_GamePlayerGetInfoResponse.GameInfos[i];
                    RoleArchiveInfo roleArchive = new RoleArchiveInfo { UUID = roleInfos.GameId, Name = roleInfos.NickName, Level = roleInfos.Level, RoleType = roleInfos.PlayerType, struct_ItemIns = roleInfos.AllEquipStatus.ToList(),ClassLev=roleInfos.OccupationLevel};
                    RoleArchiveInfoManager.Instance.Add(roleInfos.GameId, roleArchive);
                }
                NameInputField.text = string.Empty;
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText("角色创建成功！", true);
                uIConfirm.AddCancelEventAction(() => { StartBtn.interactable = true; });
                uIConfirm.AddActionEvent(() =>
                {
                    roleType = -1;
                    StartBtn.interactable = true;
                    Game.Scene.GetComponent<UIComponent>().VisibleUI(UIType.UIChooseRole);
                    Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICreatRole);
                    UIComponent.Instance.Get(UIType.UIChooseRole).GetComponent<UIChooseRoleComponent>().IsRefresh = true;//新建角色成功 刷新选择角色界面的角色

                });
            }
        }

        #endregion
        public override void Dispose()
        {
            base.Dispose();
            ETModel.Game.Scene.GetComponent<CameraComponent>().RenderCamera.gameObject.SetActive(false);
            if (currentShowPlayer != null)
                ResourcesComponent.Instance.DestoryGameObjectImmediate(currentShowPlayer, currentShowPlayer.name.StringToAB());
            currentShowPlayer = null;
        }

    }
}
