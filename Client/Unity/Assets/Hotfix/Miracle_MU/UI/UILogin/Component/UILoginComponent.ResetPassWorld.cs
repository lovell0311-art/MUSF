using ETModel;

using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 重置密码
    /// </summary>

    public partial class UILoginComponent
    {
        private ReferenceCollector referenceCollector_Reset;

        private Text PasswordLevelTxt_Reset;

        private Button ResetBtn;//重置密码
        private Button CancleBtn;//取消
        public Button getResetAuthcodeBtn;//获取验证码

        private InputField Reset_account;//账号
        private InputField Reset_password;//密码
        private InputField Confirm_password;//确认密码
        private InputField Reset_authCode;//验证码
        private InputField IdentityCard;//验证码
      
        public Text Reset_waitTimetxt;//倒计时提示文本

        public bool Reset_isScend = false;//是否已经发送请求
        public int Reset_countDownTime = 60;//倒计时 60s
        public float Reset_curTime = 0;//当前的时间

        public void Reset_Awake()
        {
            referenceCollector_Reset = ResetPassWorld.GetReferenceCollector();

            PasswordLevelTxt_Reset = referenceCollector_Reset.GetText("PasswordLevelTxt");
            PasswordLevelTxt_Reset.text = string.Empty;

            Reset_account = referenceCollector_Reset.GetInputField("Account");
            Reset_password = referenceCollector_Reset.GetInputField("Password");
            Reset_authCode = referenceCollector_Reset.GetInputField("AuthCode");
            Confirm_password = referenceCollector_Reset.GetInputField("InviteCode");
            IdentityCard = referenceCollector_Reset.GetInputField("IdentityCard");

            Reset_waitTimetxt = referenceCollector_Reset.GetText("Text");
            ResetBtn = referenceCollector_Reset.GetButton("RegisterBtn");
            getResetAuthcodeBtn = referenceCollector_Reset.GetButton("GetAuthCodeBtn");
            CancleBtn = referenceCollector_Reset.GetButton("CancelBtn");

            Reset_account.onEndEdit.AddSingleListener(OnResetAccountInputEnd);
            Reset_password.onEndEdit.AddSingleListener(OnResetPasswordInputEnd);
            Confirm_password.onEndEdit.AddSingleListener((value) => 
            {
                if (value.Length != Reset_password.text.Length || value.Equals(Reset_password.text) == false)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"两次密码不一样 请重新输入");
                    Confirm_password.text = string.Empty;
                    Log.DebugRed($"Confirm_password.textComponent.text:{Confirm_password.text}");
                }
            });
            IdentityCard.onEndEdit.AddSingleListener((value) => 
            {
                if (value.Length != 6)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"两次密码不一样 请重新输入");
                }
            });

            getResetAuthcodeBtn.onClick.AddSingleListener(OnClickGetResetAuthCode);
            ResetBtn.onClick.AddSingleListener(OnClickReset);
            CancleBtn.onClick.AddSingleListener(CancleReset);
        }
        public void ResetUpdate()
        {
            if (Reset_isScend)
            {
                //倒计时
                Reset_waitTimetxt.text = Reset_countDownTime.ToString() + "s";
                Reset_curTime += Time.deltaTime;
                if (Reset_curTime >= 1)
                {
                    Reset_countDownTime--;
                    Reset_curTime = 0;
                }
                if (Reset_countDownTime <= 0)
                {
                    Reset_isScend = false;
                    Reset_waitTimetxt.text = "获取验证码";
                    getResetAuthcodeBtn.interactable = true;
                }
            }
        }
        private void OnResetAccountInputEnd(string value)
        {
            if (value.Length != 11)
            {
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("手机号格式不正确,请你重新输入……", true);
                uIConfirmComponent.AddActionEvent(() => { Reset_account.text = string.Empty; });
            }
        }
        private void OnResetPasswordInputEnd(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            string tiptxt = PasswordHelp.RegexLevel(value);
            //密码不符合规定要求 则提示需要重新输入
            if (string.IsNullOrEmpty(tiptxt))
            {
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("密码需由数字或者字母或者符号组成的8-15位数组成\n请你重新设置密码", true);
                uIConfirmComponent.AddActionEvent(() => { Reset_password.text = string.Empty; });
                return;
            }
            PasswordLevelTxt_Reset.text = tiptxt;
        }
        /// <summary>
        /// 取消注册
        /// </summary>
        private void CancleReset()
        {
            Reset_isScend = false;
            Reset_account.text = string.Empty;
            Reset_password.text = string.Empty;
            Confirm_password.text = string.Empty;
            Reset_authCode.text = string.Empty;
            IdentityCard.text = string.Empty;
            getResetAuthcodeBtn.interactable = true;
            //关闭重置密码界面
            OpenRegisterPanel(LoginPanelType.Login);
        }

        private void OnClickReset()
        {
            Log.DebugGreen($"重置密码：{string.IsNullOrEmpty(Reset_account.text)}");
            if (string.IsNullOrEmpty(Reset_account.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请你输入手机号");
                return;
            }
            if (string.IsNullOrEmpty(Reset_password.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入密码");
                return;
            }
            if (string.IsNullOrEmpty(Reset_authCode.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入验证码");
                return;
            }
            if (string.IsNullOrEmpty(IdentityCard.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入身份证后6位");
                return;
            }
            if (IdentityCard.text.Length != 6)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入身份证后6位");
                return;
            }
            OnResetAsync().Coroutine();
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <returns></returns>
        public async ETVoid OnResetAsync()
        {
            try
            {

                //创建一个会话实体
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                R2C_ResetPasswordByPhoneNumberResponse r2c_RegisterOrLogin = (R2C_ResetPasswordByPhoneNumberResponse)await realmSession.Call(new C2R_ResetPasswordByPhoneNumberRequest()
                {
                    PhoneNumber = Reset_account.text,
                    NewPassword = MD5.GetMD5Hash(Reset_password.text),
                    VerificationCode = Reset_authCode.text,
                    ChannelId = "123112414",//运营商id
                    DeviceNum = DeviceUtil.DeviceIdentifier,
                    DeviceType = DeviceUtil.DeviceModel,
                    OSType = DeviceUtil.DeviceVersionType,
                    BaseVersion = "1123",//版本号
                    CPUType = "12312",//cpu型号
                    Idcard = IdentityCard.text
                });
                realmSession.Dispose();
                if (r2c_RegisterOrLogin.Error == 0)
                {
                    //注册成功 直接保存账号 登录
                    AccountConfigInfo config = new AccountConfigInfo
                    {
                        account = Reset_account.text,
                        password = Reset_password.text
                    };
                    LocalDataJsonComponent.Instance.SavaData(config, LocalJsonDataKeys.UserAccount);
                    //登录界面显示 当前注册的账号 密码
                    loginaccount.text = Reset_account.text;
                    loginpassWord.text = Reset_password.text;

                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText("密码 修改成功!!!", true);
                    uIConfirmComponent.AddActionEvent(CancleRegister);

                }
                else
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"{r2c_RegisterOrLogin.Error.GetTipInfo()}", true);
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }



        #region 获取验证码
        private void OnClickGetResetAuthCode()
        {
            if (Reset_account.text == null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请你输入手机号");
                return;
            }
            if (Reset_account.text.Length != 11)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "手机号格式不正确 \n请你重新输入");
                return;
            }
            Reset_isScend = true;
            Reset_countDownTime = 60;
            getResetAuthcodeBtn.interactable = false;
            GetResetSMSApplyCode().Coroutine();
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        private async ETVoid GetResetSMSApplyCode()
        {
            try
            {
                //创建一个会话实体
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalDataManager.LoginConnetIP);
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                R2C_LoginSystemSMSApplyCodeResponse r2C_SMSApplyCode = (R2C_LoginSystemSMSApplyCodeResponse)await realmSession.Call(new C2R_LoginSystemSMSApplyCodeRequest
                {
                    PhoneNumber = Reset_account.text,//手机号
                    CountryCode = "86",
                    UseType = (int)UseType.CHANGEPASSWORD
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
