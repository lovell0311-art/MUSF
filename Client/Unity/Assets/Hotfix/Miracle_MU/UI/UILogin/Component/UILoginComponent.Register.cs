using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// 账号注册
    /// </summary>
    public partial class UILoginComponent
    {
        private ReferenceCollector referenceCollector;

        private Text PasswordLevelTxt;

        private Button registerBtn;//注册
        private Button returnBtn;//取消
        public Button getAuthcodeBtn;//获取验证码

        private InputField account;//账号
        private InputField password;//密码
        private InputField authCode;//验证码
        private InputField InviteCode;//邀请码

        public Text waitTimetxt;//倒计时提示文本

        public bool isScend = false;//是否已经发送请求
        public int countDownTime = 60;//倒计时 60s
        public float curTime = 0;//当前的时间

        public void RegisterAwake()
        {
            if (RegisterPanel == null)
            {
                LoginStageTrace.Append("UILogin RegisterAwake missing RegisterPanel");
                return;
            }

            referenceCollector = RegisterPanel.GetComponent<ReferenceCollector>();

            PasswordLevelTxt = referenceCollector?.GetText("PasswordLevelTxt") ?? ResolveComponent<Text>(RegisterPanel.transform, "PasswordLevelTxt");
            if (PasswordLevelTxt != null)
            {
                PasswordLevelTxt.text = string.Empty;
            }

            account = referenceCollector?.GetInputField("Account") ?? ResolveComponent<InputField>(RegisterPanel.transform, "Account");
            password = referenceCollector?.GetInputField("Password") ?? ResolveComponent<InputField>(RegisterPanel.transform, "Password");
            authCode = referenceCollector?.GetInputField("AuthCode") ?? ResolveComponent<InputField>(RegisterPanel.transform, "AuthCode");
            InviteCode = referenceCollector?.GetInputField("InviteCode") ?? ResolveComponent<InputField>(RegisterPanel.transform, "InviteCode");

            waitTimetxt = referenceCollector?.GetText("Text") ?? ResolveComponent<Text>(RegisterPanel.transform, "Text");
            registerBtn = referenceCollector?.GetButton("RegisterBtn") ?? ResolveComponent<Button>(RegisterPanel.transform, "RegisterBtn");
            getAuthcodeBtn = referenceCollector?.GetButton("GetAuthCodeBtn") ?? ResolveComponent<Button>(RegisterPanel.transform, "GetAuthCodeBtn");
            returnBtn = referenceCollector?.GetButton("CancelBtn") ?? ResolveComponent<Button>(RegisterPanel.transform, "CancelBtn");

            if (account != null)
            {
                account.onEndEdit.AddSingleListener(OnAccountInputEnd);
            }
            else
            {
                LoginStageTrace.Append("UILogin RegisterAwake missing Account input");
            }

            if (authCode != null)
            {
                authCode.onEndEdit.AddSingleListener(OnCodeInputEnd);
            }
            else
            {
                LoginStageTrace.Append("UILogin RegisterAwake missing AuthCode input");
            }

            if (password != null)
            {
                password.onEndEdit.AddSingleListener(OnPasswordInputEnd);
            }
            else
            {
                LoginStageTrace.Append("UILogin RegisterAwake missing Password input");
            }

            if (getAuthcodeBtn != null)
            {
                getAuthcodeBtn.onClick.AddSingleListener(OnClickGetAuthCode);
            }

            if (registerBtn != null)
            {
                registerBtn.onClick.AddSingleListener(OnClickRegister);
            }

            if (returnBtn != null)
            {
                returnBtn.onClick.AddSingleListener(CancleRegister);
            }
        }
        public void RegisterUpdate()
        {
            if (isScend)
            {
                //倒计时
                if (waitTimetxt != null)
                {
                    waitTimetxt.text = countDownTime.ToString() + "s";
                }
                curTime += Time.deltaTime;
                if (curTime >= 1)
                {
                    countDownTime--;
                    curTime = 0;
                }
                if (countDownTime <= 0)
                {
                    isScend = false;
                    if (waitTimetxt != null)
                    {
                        waitTimetxt.text = "获取验证码";
                    }
                    if (getAuthcodeBtn != null)
                    {
                        getAuthcodeBtn.interactable = true;
                    }
                }
            }
        }
        private void OnAccountInputEnd(string value)
        {
            if (value.Length != 11)
            {
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("手机号格式不正确,请你重新输入……", true);
                uIConfirmComponent.AddActionEvent(() => { account.text = string.Empty; });
            }
        }
        private void OnCodeInputEnd(string value)
        {
            if (value.Length != 6)
            {
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("验证码格式错误……", true);
                uIConfirmComponent.AddActionEvent(() =>
                {
                    if (authCode != null)
                    {
                        authCode.text = string.Empty;
                    }
                });
            }
        }
        private void OnPasswordInputEnd(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            string tiptxt = PasswordHelp.RegexLevel(value);
            //密码不符合规定要求 则提示需要重新输入
            if (string.IsNullOrEmpty(tiptxt))
            {
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("密码需由数字或者字母或者符号组成的8-15位数组成\n请你重新设置密码", true);
                uIConfirmComponent.AddActionEvent(() => { password.text = string.Empty; });
                return;
            }
            PasswordLevelTxt.text = tiptxt;
        }
        /// <summary>
        /// 取消注册
        /// </summary>
        private void CancleRegister()
        {
            isScend = false;
            if (account != null) account.text = string.Empty;
            if (password != null) password.text = string.Empty;
            if (authCode != null) authCode.text = string.Empty;
            if (getAuthcodeBtn != null) getAuthcodeBtn.interactable = true;
            if (InviteCode != null) InviteCode.text = string.Empty;
            if (PasswordLevelTxt != null) PasswordLevelTxt.text = string.Empty;
            if (waitTimetxt != null) waitTimetxt.text = "获取验证码";
            //关闭注册界面
            OpenRegisterPanel(LoginPanelType.Login);
        }

        private void OnClickRegister()
        {
            if (account == null || password == null || authCode == null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "注册界面初始化失败，请重新打开客户端");
                return;
            }
           
            if (string.IsNullOrEmpty(account.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请你输入手机号");
                return;
            }
            if (string.IsNullOrEmpty(password.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入密码");
                return;
            }
            if (string.IsNullOrEmpty(authCode.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入验证码");
                return;
            }
            //if (string.IsNullOrEmpty(InviteCode.text))
            //{
            //    UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入邀请码");
            //    return;
            //}
            OnRegisterAsync().Coroutine();
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <returns></returns>
        public async ETVoid OnRegisterAsync()
        {
            try
            {
                string accountText = account?.text ?? string.Empty;
                string passwordText = password?.text ?? string.Empty;
                string authCodeText = authCode?.text ?? string.Empty;
                string inviteCodeText = InviteCode?.text ?? string.Empty;

                //创建一个会话实体
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                R2C_RegisterOrLoginResponse r2c_RegisterOrLogin = (R2C_RegisterOrLoginResponse)await realmSession.Call(new C2R_RegisterOrLoginRequest()
                {
                    Account = accountText,
                    Password = MD5.GetMD5Hash(passwordText),
                    VerificationCode = authCodeText,
                    LoginType = (int)LoginType.REGISTER,
                    TerminalIP = DeviceUtil.GetIP() ?? "模拟器",//登录ip
                    ChannelId = AgentTool.agentstr,//运营商id
                    DeviceNum = DeviceUtil.DeviceIdentifier,
                    DeviceType = DeviceUtil.DeviceModel,
                    OSType = DeviceUtil.DeviceVersionType,
                    BaseVersion = "1123",//版本号
                    CPUType = "12312",//cpu型号
                    Code = inviteCodeText
                });
              
                if (r2c_RegisterOrLogin.Error == 0)
                {
                    //注册成功 直接保存账号 登录
                    AccountConfigInfo config = new AccountConfigInfo
                    {
                        account = accountText,
                        password = passwordText
                    };
                    LocalDataJsonComponent.Instance.SavaData(config, LocalJsonDataKeys.UserAccount);
                    //登录界面显示 当前注册的账号 密码
                    if (loginaccount != null) loginaccount.text = accountText;
                    if (loginpassWord != null) loginpassWord.text = passwordText;

                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText("账号 注册成功!!!", true);
                    uIConfirmComponent.AddActionEvent(CancleRegister);

                }
                else
                {
                    if (r2c_RegisterOrLogin.Error == 132 || r2c_RegisterOrLogin.Error == 134)
                    {
                        //实名认证
                        UIComponent.Instance.VisibleUI(UIType.UIRealName, accountText, realmSession, r2c_RegisterOrLogin.Error);
                        //UIRealNameComponent uIRealNameComponent = UIComponent.Instance.Get(UIType.UIRealName).GetComponent<UIRealNameComponent>();
                        //uIRealNameComponent.Session = realmSession;
                        return;
                    }
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"{r2c_RegisterOrLogin.Error.GetTipInfo()}", true);
                }
                realmSession.Dispose();
            }
            catch (System.Exception e)
            {
                Log.Error(e);
                UIComponent.Instance.VisibleUI(UIType.UIHint, "注册失败，请稍后重试");
            }
        }



        #region 获取验证码
        private void OnClickGetAuthCode()
        {
            if (account == null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "注册界面初始化失败，请重新打开客户端");
                return;
            }

            if (string.IsNullOrEmpty(account.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请你输入手机号");
                return;
            }
            if (account.text.Length != 11)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "手机号格式不正确 \n请你重新输入");
                return;
            }
            isScend = true;
            countDownTime = 60;
            if (getAuthcodeBtn != null)
            {
                getAuthcodeBtn.interactable = false;
            }
            GetSMSApplyCode().Coroutine();
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        private async ETVoid GetSMSApplyCode()
        {
            try
            {

                //创建一个会话实体
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                R2C_LoginSystemSMSApplyCodeResponse r2C_SMSApplyCode = (R2C_LoginSystemSMSApplyCodeResponse)await realmSession.Call(new C2R_LoginSystemSMSApplyCodeRequest
                {
                    PhoneNumber = account.text,//手机号
                    CountryCode = "86",
                    UseType = (int)UseType.REGISTER
                });
                realmSession.Dispose();
                Log.Debug(r2C_SMSApplyCode.Error.GetTipInfo());
                //获取失败
                if (r2C_SMSApplyCode.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"{r2C_SMSApplyCode.Error.GetTipInfo()}");
                }


            }
            catch (System.Exception e)
            {
                Log.Debug(e.ToString());
                Debug.LogError(e.ToString());
            }
        }
        #endregion


    }
}
