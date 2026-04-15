using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;

namespace ETHotfix
{
    /// <summary>
    /// 登录类型
    /// </summary>
    public enum LoginType
    {
        LOGIN = 0,//登录
        REGISTER = 1,//注册
    }
    /// <summary>
    /// 验证码 使用类型
    /// </summary>
    public enum UseType
    {
        REGISTER = 0,//注册
        CHANGEPASSWORD//修改密码
    }
    [ObjectSystem]
    public class UiLoginComponentAwake : AwakeSystem<UILoginComponent>
    {
        public override void Awake(UILoginComponent self)
        {
            LoginStageTrace.Append("UILoginComponent.AwakeSystem start");

            self.Awake();
            try
            {
                self.RegisterAwake();
            }
            catch (Exception e)
            {
                Log.Error(e);
                LoginStageTrace.Append($"UILoginComponent.RegisterAwake failed type={e.GetType().Name} message={e.Message}");
            }

            try
            {
                self.Reset_Awake();
            }
            catch (Exception e)
            {
                Log.Error(e);
                LoginStageTrace.Append($"UILoginComponent.ResetAwake failed type={e.GetType().Name} message={e.Message}");
            }

            self.OpenRegisterPanel(LoginPanelType.Login);//默认显示登录
            LoginStageTrace.Append("UILoginComponent.AwakeSystem finish");

        }
    }
    [ObjectSystem]
    public class UILoginComponentUpdate : UpdateSystem<UILoginComponent>
    {
        public override void Update(UILoginComponent self)
        {
            self.RegisterUpdate();
            self.ResetUpdate();
        }
    }
    public enum LoginPanelType
    {
        Login,
        Register,
        Rester,
        RealName
    }
    public partial class UILoginComponent : Component
    {
        private const float FallbackRegisterButtonX = -198f;
        private const float FallbackActionButtonY = -227f;
        private const long RealmRpcTimeoutMs = 5000;
        private const long GateRpcTimeoutMs = 5000;
        private static bool AutoLoginConsumed;
        private static bool AgreementPopupRequestedByUser;
        private InputField loginaccount;
        private InputField loginpassWord;
        private Button loginBtn;
        private Button opeenregisterBtn;
        private Button ResterBtn;
        private int loginCount = 0;//登录次数
        public long NewKey=0;
        private GameObject LoginPanel, RegisterPanel, ResetPassWorld;
        public UIHyperlinkText hyperlinkText;
        private bool IsRead => isReadTog == null || isReadTog.isOn;//是否已经读取相关许可协议
        Toggle isReadTog;
        public void Awake()
        {
            LoginStageTrace.Append("UILoginComponent.Awake enter");
            AgreementPopupRequestedByUser = false;
            UI parentUi = this.GetParent<UI>();
            GameObject uiRoot = parentUi.GameObject;
            ReferenceCollector rc = uiRoot.GetComponent<ReferenceCollector>();
            RegisterPanel = rc?.GetGameObject("RegisterPanel") ?? uiRoot.transform.Find("LoginBg/RegisterPanel")?.gameObject;
            LoginPanel = rc?.GetGameObject("LoginPanel") ?? uiRoot.transform.Find("LoginBg/LoginPanel")?.gameObject;
            ResetPassWorld = rc?.GetGameObject("ResetPassWorld") ?? uiRoot.transform.Find("LoginBg/ResetPassWorld")?.gameObject;
            SetLoginPanelsDefaultState();
            CloseUnexpectedAgreementPopup("awake");

            Text agreementLabel = rc?.GetText("Label")
                ?? uiRoot.transform.Find("Label")?.GetComponent<Text>()
                ?? ResolveComponent<Text>(uiRoot.transform, "Label");
            hyperlinkText = agreementLabel != null ? agreementLabel.GetComponent<UIHyperlinkText>() : null;
            if (hyperlinkText != null)
            {
                hyperlinkText.RegisterClickCallback((s, s1) => 
                {
                    MarkAgreementPopupRequestedByUser();
                    UIComponent.Instance.VisibleUI(UIType.UI_UserAgreement);
                });
            }
            else
            {
                LoginStageTrace.Append("UILogin agreement label missing");
            }

            isReadTog = rc?.GetToggle("Toggle")
                ?? uiRoot.transform.Find("Toggle")?.GetComponent<Toggle>()
                ?? ResolveComponent<Toggle>(uiRoot.transform, "Toggle");
            if (isReadTog != null)
            {
                isReadTog.onValueChanged.AddSingleListener(IsReadTogEvent);
                isReadTog.isOn = true;
            }
            else
            {
                LoginStageTrace.Append("UILogin agreement toggle missing");
            }

            ReferenceCollector loginrc = LoginPanel != null ? LoginPanel.GetComponent<ReferenceCollector>() : null;

            loginaccount = loginrc?.GetInputField("Account") ?? ResolveComponent<InputField>(LoginPanel?.transform, "Account");
            loginpassWord = loginrc?.GetInputField("Password") ?? ResolveComponent<InputField>(LoginPanel?.transform, "Password");

            loginBtn = loginrc?.GetButton("LoginBtn") ?? ResolveComponent<Button>(LoginPanel?.transform, "LoginBtn");
            opeenregisterBtn = loginrc?.GetButton("RegisterBtn") ?? ResolveComponent<Button>(LoginPanel?.transform, "RegisterBtn");
            ResterBtn = loginrc?.GetButton("ResterBtn") ?? ResolveComponent<Button>(LoginPanel?.transform, "ResterBtn");
            opeenregisterBtn = EnsureRegisterEntryButton(opeenregisterBtn, ResterBtn, loginBtn);

            if (loginBtn != null)
            {
                loginBtn.onClick.AddSingleListener(OnLogin);
            }
            else
            {
                LoginStageTrace.Append("UILogin login button missing");
            }

            if (opeenregisterBtn != null)
            {
                opeenregisterBtn.onClick.AddSingleListener(() => { OpenRegisterPanel(LoginPanelType.Register); });
            }
            else
            {
                LoginStageTrace.Append("UILogin register button missing");
            }

            if (ResterBtn != null)
            {
                ResterBtn.onClick.AddSingleListener(() => { OpenRegisterPanel(LoginPanelType.Rester); });
            }
            else
            {
                LoginStageTrace.Append("UILogin reset button missing");
            }

            SetLoginPanelsDefaultState();
            EnsureVisible(loginaccount);
            EnsureVisible(loginpassWord);
            EnsureVisible(loginBtn);
            EnsureVisible(opeenregisterBtn);
            EnsureVisible(ResterBtn);
            RepairLoginVisuals(uiRoot);
            CloseUnexpectedAgreementPopup("post-repair");
            ScheduleVisualRepairPasses(uiRoot);
            ScheduleAgreementPopupGuard();

            DumpHierarchy(uiRoot.transform, "UILoginRoot", 0, 2);
            DumpHierarchy(LoginPanel?.transform, "LoginPanel", 0, 2);

            InitAccount_PassWord();
            TryScheduleDiagnosticAutoLogin();
            LoginStageTrace.Append($"UILoginComponent.Awake finish loginPanelNull={LoginPanel == null} registerPanelNull={RegisterPanel == null}");

        }

        private void TryScheduleDiagnosticAutoLogin()
        {
            if (!LoginDiagnosticOptions.AutoFlowEnabled)
            {
                return;
            }

            if (AutoLoginConsumed)
            {
                LoginStageTrace.Append("DiagnosticAutoLogin skip consumed");
                return;
            }

            if (loginaccount == null || loginpassWord == null || loginBtn == null)
            {
                LoginStageTrace.Append("DiagnosticAutoLogin skip missing-ui");
                return;
            }

            AccountConfigInfo accountConfig = LocalDataJsonComponent.Instance.LoadData<AccountConfigInfo>(LocalJsonDataKeys.UserAccount) ?? new AccountConfigInfo();
            string account = accountConfig.account?.Trim();
            string password = accountConfig.password;
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
            {
                LoginStageTrace.Append("DiagnosticAutoLogin skip missing-account");
                return;
            }

            AutoLoginConsumed = true;
            LoginStageTrace.Append($"DiagnosticAutoLogin scheduled account={account}");

            TimerComponent.Instance.RegisterTimeCallBack(1200, () =>
            {
                try
                {
                    if (loginaccount == null || loginpassWord == null || loginBtn == null)
                    {
                        LoginStageTrace.Append("DiagnosticAutoLogin abort missing-ui-on-fire");
                        return;
                    }

                    loginaccount.text = account;
                    loginpassWord.text = password;
                    if (isReadTog != null)
                    {
                        isReadTog.isOn = true;
                    }

                    LoginStageTrace.Append($"DiagnosticAutoLogin fire account={account} buttonActive={loginBtn.gameObject.activeInHierarchy} interactable={loginBtn.interactable}");
                    OnLogin();
                }
                catch (Exception e)
                {
                    LoginStageTrace.Append($"DiagnosticAutoLogin exception type={e.GetType().Name} message={e.Message}");
                }
            });
        }

        private static void EnsureVisible(UnityEngine.Component component)
        {
            if (component == null)
            {
                return;
            }

            Transform current = component.transform;
            while (current != null)
            {
                current.gameObject.SetActive(true);
            current = current.parent;
            }
        }

        private ETTask<IResponse> CallWithTimeout(Session session, IRequest request, long timeoutMs, string stageTag)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ETTaskCompletionSource<IResponse> completionSource = new ETTaskCompletionSource<IResponse>();
            bool finished = false;
            ETModel.Timer timeoutTimer = TimerComponent.Instance.RegisterTimeCallBack(timeoutMs, () =>
            {
                if (finished)
                {
                    return;
                }

                finished = true;
                LoginStageTrace.Append($"RpcTimeout stage={stageTag} timeoutMs={timeoutMs} remote={session.session?.RemoteAddress}");
                try
                {
                    session.Dispose();
                }
                catch
                {
                }

                completionSource.TrySetException(new TimeoutException($"{stageTag} timeout {timeoutMs}ms"));
            });

            WaitResponse().Coroutine();
            return completionSource.Task;

            async ETVoid WaitResponse()
            {
                try
                {
                    IResponse response = await session.Call(request);
                    if (finished)
                    {
                        return;
                    }

                    finished = true;
                    TimerComponent.Instance.RemoveTimer(timeoutTimer.Id);
                    completionSource.TrySetResult(response);
                }
                catch (Exception e)
                {
                    if (finished)
                    {
                        return;
                    }

                    finished = true;
                    TimerComponent.Instance.RemoveTimer(timeoutTimer.Id);
                    completionSource.TrySetException(e);
                }
            }
        }

        private Button EnsureRegisterEntryButton(Button currentButton, Button resetTemplate, Button loginTemplate)
        {
            if (currentButton != null)
            {
                return currentButton;
            }

            Button template = resetTemplate != null ? resetTemplate : loginTemplate;
            if (template == null || LoginPanel == null)
            {
                LoginStageTrace.Append("UILogin register entry fallback skipped templateMissing");
                return null;
            }

            try
            {
                Vector2 fallbackPosition = new Vector2(FallbackRegisterButtonX, GetFallbackActionButtonY(template));
                Button fallbackButton = CloneFallbackActionButton(template, "RegisterBtn", fallbackPosition);
                LoginStageTrace.Append($"UILogin register entry fallback created template={template.name} pos={fallbackPosition}");
                return fallbackButton;
            }
            catch (Exception e)
            {
                LoginStageTrace.Append($"UILogin register entry fallback failed type={e.GetType().Name} message={e.Message}");
                return null;
            }
        }

        private static float GetFallbackActionButtonY(Button template)
        {
            RectTransform rectTransform = template?.transform as RectTransform;
            if (rectTransform == null)
            {
                return FallbackActionButtonY;
            }

            return Mathf.Abs(rectTransform.anchoredPosition.y) > 1f ? rectTransform.anchoredPosition.y : FallbackActionButtonY;
        }

        private Button CloneFallbackActionButton(Button template, string buttonName, Vector2 anchoredPosition)
        {
            if (template == null)
            {
                return null;
            }

            GameObject cloneObject = UnityEngine.Object.Instantiate(template.gameObject, template.transform.parent, false);
            cloneObject.name = buttonName;
            cloneObject.SetActive(true);

            RectTransform templateRect = template.transform as RectTransform;
            RectTransform cloneRect = cloneObject.transform as RectTransform;
            if (templateRect != null && cloneRect != null)
            {
                cloneRect.anchorMin = templateRect.anchorMin;
                cloneRect.anchorMax = templateRect.anchorMax;
                cloneRect.pivot = templateRect.pivot;
                cloneRect.sizeDelta = templateRect.sizeDelta;
                cloneRect.localScale = templateRect.localScale == Vector3.zero ? Vector3.one : templateRect.localScale;
                cloneRect.anchoredPosition = anchoredPosition;
                cloneRect.SetAsLastSibling();
            }

            foreach (Graphic graphic in cloneObject.GetComponentsInChildren<Graphic>(true))
            {
                if (graphic == null)
                {
                    continue;
                }

                graphic.enabled = true;
                graphic.gameObject.SetActive(true);
            }

            Button cloneButton = cloneObject.GetComponent<Button>();
            if (cloneButton != null)
            {
                cloneButton.onClick.RemoveAllListeners();
                cloneButton.interactable = true;
            }

            return cloneButton;
        }

        private void ScheduleVisualRepairPasses(GameObject uiRoot)
        {
            if (uiRoot == null)
            {
                return;
            }

            if (!VisualRepairCompatibility.ShouldRepairLogin(uiRoot, LoginPanel, loginaccount, loginpassWord, loginBtn, opeenregisterBtn, ResterBtn))
            {
                LoginStageTrace.Append("RepairLoginVisuals skipped baseline-healthy");
                return;
            }

            ScheduleVisualRepairPass(uiRoot, 200, "200ms");
            ScheduleVisualRepairPass(uiRoot, 900, "900ms");
        }

        private void ScheduleAgreementPopupGuard()
        {
            ScheduleAgreementPopupGuardPass(2500, "2500ms");
        }

        private void ScheduleAgreementPopupGuardPass(long delay, string tag)
        {
            TimerComponent.Instance?.RegisterTimeCallBack(delay, () =>
            {
                if (this.IsDisposed)
                {
                    return;
                }

                try
                {
                    CloseUnexpectedAgreementPopup($"guard-{tag}");
                }
                catch (Exception e)
                {
                    LoginStageTrace.Append($"AgreementPopupGuard failed={tag} type={e.GetType().Name} message={e.Message}");
                }
            });
        }

        private void ScheduleVisualRepairPass(GameObject uiRoot, long delay, string tag)
        {
            TimerComponent.Instance?.RegisterTimeCallBack(delay, () =>
            {
                if (this.IsDisposed || uiRoot == null)
                {
                    return;
                }

                try
                {
                    SetLoginPanelsDefaultState();
                    RepairLoginVisuals(uiRoot);
                    CloseUnexpectedAgreementPopup($"repair-{tag}");
                    LoginStageTrace.Append($"RepairLoginVisuals delayed={tag}");
                }
                catch (Exception e)
                {
                    LoginStageTrace.Append($"RepairLoginVisuals delayedFailed={tag} type={e.GetType().Name} message={e.Message}");
                }
            });
        }

        private void MarkAgreementPopupRequestedByUser()
        {
            AgreementPopupRequestedByUser = true;
            LoginStageTrace.Append("UILogin agreement popup requestedByUser");
            TimerComponent.Instance?.RegisterTimeCallBack(8000, () =>
            {
                if (this.IsDisposed)
                {
                    return;
                }

                AgreementPopupRequestedByUser = false;
                LoginStageTrace.Append("UILogin agreement popup request window closed");
            });
        }

        private void CloseUnexpectedAgreementPopup(string tag)
        {
            UIComponent uiComponent = UIComponent.Instance;
            if (uiComponent == null)
            {
                return;
            }

            UI agreementUi = uiComponent.Get(UIType.UI_UserAgreement);
            if (agreementUi == null)
            {
                return;
            }

            if (AgreementPopupRequestedByUser)
            {
                LoginStageTrace.Append($"UILogin agreement popup kept tag={tag}");
                return;
            }

            LoginStageTrace.Append($"UILogin agreement popup removed tag={tag}");
            uiComponent.Remove(UIType.UI_UserAgreement);
        }

        private static void DumpHierarchy(Transform root, string tag, int depth, int maxDepth)
        {
            if (root == null || depth > maxDepth)
            {
                return;
            }

            string indent = new string('>', depth);
            RectTransform rect = root as RectTransform;
            string rectInfo = rect == null ? string.Empty : $" pos={rect.anchoredPosition} size={rect.rect.size}";
            LoginStageTrace.Append($"{tag} {indent}{root.name} active={root.gameObject.activeSelf} childCount={root.childCount}{rectInfo}");

            if (depth >= maxDepth)
            {
                return;
            }

            foreach (Transform child in root)
            {
                DumpHierarchy(child, tag, depth + 1, maxDepth);
            }
        }
        /// <summary>
        /// 初始化账号/密码
        /// </summary>
        public void InitAccount_PassWord()
        {
            
            AccountConfigInfo accountConfig = LocalDataJsonComponent.Instance.LoadData<AccountConfigInfo>(LocalJsonDataKeys.UserAccount) ?? new AccountConfigInfo();
            if (loginaccount != null)
            {
                ApplyInputFieldValue(loginaccount, accountConfig.account);
            }

            if (loginpassWord != null)
            {
                ApplyInputFieldValue(loginpassWord, accountConfig.password);
            }

            if (isReadTog != null)
            {
                isReadTog.isOn = accountConfig.IsRead;
                isReadTog.isOn = true;
            }
        }


        private static void ApplyInputFieldValue(InputField field, string value)
        {
            if (field == null)
            {
                return;
            }

            field.text = value ?? string.Empty;
            field.ForceLabelUpdate();
        }

        private static string ResolveAccountInput(InputField field, string fallbackAccount)
        {
            string value = field?.text?.Trim();
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            string placeholderText = (field?.placeholder as Text)?.text;
            if (field?.textComponent != null)
            {
                value = NormalizeVisibleInput(field.textComponent.text, placeholderText);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            if (field != null)
            {
                foreach (Text child in field.GetComponentsInChildren<Text>(true))
                {
                    if (child == null)
                    {
                        continue;
                    }

                    value = NormalizeVisibleInput(child.text, placeholderText);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }

            return fallbackAccount?.Trim() ?? string.Empty;
        }

        private static string ResolvePasswordInput(InputField field, string fallbackPassword)
        {
            string value = field?.text;
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            return fallbackPassword ?? string.Empty;
        }

        private static string NormalizeVisibleInput(string value, string placeholderText)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.Trim();
            if (!string.IsNullOrEmpty(placeholderText) && string.Equals(normalized, placeholderText.Trim(), StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (normalized.StartsWith("请输入", StringComparison.Ordinal) || normalized.StartsWith("Please", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return normalized;
        }

        private void IsReadTogEvent(bool isOn)
        {
            AccountConfigInfo accountConfig = LocalDataJsonComponent.Instance.LoadData<AccountConfigInfo>(LocalJsonDataKeys.UserAccount) ?? new AccountConfigInfo();
            accountConfig.IsRead = isOn;
        }
        public void OnLogin()
        {
            if (loginaccount == null || loginpassWord == null)
            {
                LoginStageTrace.Append("UILogin OnLogin aborted missing input");
                UIComponent.Instance.VisibleUI(UIType.UIHint, "登录界面初始化失败，请重新打开客户端");
                return;
            }

            AccountConfigInfo accountConfig = LocalDataJsonComponent.Instance.LoadData<AccountConfigInfo>(LocalJsonDataKeys.UserAccount) ?? new AccountConfigInfo();
            string _account = ResolveAccountInput(loginaccount, accountConfig.account);
            string _password = ResolvePasswordInput(loginpassWord, accountConfig.password);
            LoginStageTrace.Append($"OnLogin resolved account='{_account}' rawAccount='{loginaccount.text}' rawPasswordLen={(loginpassWord.text == null ? -1 : loginpassWord.text.Length)} resolvedPasswordLen={(string.IsNullOrEmpty(_password) ? 0 : _password.Length)}");
            ApplyInputFieldValue(loginaccount, _account);
            ApplyInputFieldValue(loginpassWord, _password);
            if (!IsRead)
            {
                if (isReadTog != null)
                {
                    isReadTog.isOn = true;
                }
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请你阅读并勾选相关许可协议");
                return;
            }
            //判断账号、密码是否为空
            if (string.IsNullOrEmpty(_account) || string.IsNullOrEmpty(_password))
            {
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("账号或密码为空 请输入账号……", true);
                return;
            }
            // 普通登录同时支持手机号和自定义账号名
            if (_account.Length < 3)
            {
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText("账号格式不对 请重新输入账号……", true);
                uIConfirmComponent.AddActionEvent(() => { loginaccount.text = string.Empty; });
                return;
            }
            OnLoginAsync(_account, _password).Coroutine();

            /// <summary>
            /// 登录事件 
            /// </summary>
            /// <param name="account">账号</param>
            /// <param name="password">密码</param>
            /// <returns></returns>
            async ETVoid OnLoginAsync(string account, string password)
            {
                string loginStage = "初始化";
                try
                {
                    LoginStageTrace.Clear();
                    LoginStageTrace.Append($"LoginBegin account={account}");
                    string realmAddress = GlobalDataManager.LoginConnetIP;
                    UnityEngine.Debug.Log($"[DIAG] LoginConnetIP={realmAddress} LoginConnetIP_raw={GlobalDataManager.LoginConnetIP}");
                    string gateAddress = string.Empty;
                    GlobalDataManager.Address = string.Empty;
                    GlobalDataManager.LastRealmAddress = string.Empty;
                    Session realmSession = null;
                    R2C_RegisterOrLoginResponse r2c_RegisterOrLogin = null;

                    loginBtn.interactable = false;
                    foreach (string realmCandidate in GlobalDataManager.GetLoginConnectAddresses())
                    {
                        realmAddress = realmCandidate;
                        LoginStageTrace.Append($"RealmCandidateStart address={realmAddress}");

                        try
                        {
                            //创建一个会话实体session
                            loginStage = "连接Realm";
                            LoginStageTrace.Append($"Stage={loginStage} address={realmAddress}");
                            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(realmAddress);
                            //热更层也创建一个Seesion,将Model层的session传递过去
                            //热更层的Seesion创建后,会调用Awake方法,在内部关联了Model层的session
                            //以后调用热更层的Seesion 就是调用间接的调用了主工程的 Seesion
                            realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
                            LoginStageTrace.Append($"RealmSessionCreated modelSession={session.Id} remote={session.RemoteAddress}");
                            //await等待服务器响应 r2CLogin这个是响应后 解包->反序列化得到的对象 里面已经包含服务器发送过来的数据
                            loginStage = "Realm登录";
                            LoginStageTrace.Append($"Stage={loginStage} requestAccount={account}");
                            r2c_RegisterOrLogin = (R2C_RegisterOrLoginResponse)await CallWithTimeout(realmSession, new C2R_RegisterOrLoginRequest
                            {
                                Account = account,
                                Password = MD5.GetMD5Hash(password),
                               // Password = "C6D4E7ED0B63888B23EE30539C7234CF",
                                VerificationCode = "",
                                LoginType = (int)LoginType.LOGIN,
                                TerminalIP = DeviceUtil.GetIP() ?? "模拟器",//登录ip
                                ChannelId = ETModel.Init.instance.agentStr,//营商id
                                DeviceNum = DeviceUtil.DeviceIdentifier,
                                DeviceType = DeviceUtil.DeviceModel,
                                OSType = DeviceUtil.DeviceVersionType,
                                BaseVersion = "1123",//版本号
                                CPUType = "12312"//cpu型号
                            }, RealmRpcTimeoutMs, $"{loginStage} {realmAddress}");
                            LoginStageTrace.Append($"Stage={loginStage} responseError={r2c_RegisterOrLogin.Error} gate={r2c_RegisterOrLogin.Address} key={r2c_RegisterOrLogin.Key}");
                            Log.DebugBrown("请求登录游戏" + r2c_RegisterOrLogin.Error);

                            if (GlobalDataManager.IsNetworkAddressError(r2c_RegisterOrLogin.Error))
                            {
                                LoginStageTrace.Append($"RealmCandidateRetry address={realmAddress} error={r2c_RegisterOrLogin.Error}");
                                realmSession.Dispose();
                                realmSession = null;
                                r2c_RegisterOrLogin = null;
                                continue;
                            }

                            GlobalDataManager.LastRealmAddress = realmAddress;
                            break;
                        }
                        catch (Exception candidateException)
                        {
                            Log.Error(candidateException);
                            LoginStageTrace.Append($"RealmCandidateException address={realmAddress} type={candidateException.GetType().Name} message={candidateException.Message}");

                            try
                            {
                                realmSession?.Dispose();
                            }
                            catch
                            {
                            }

                            realmSession = null;
                            if (GlobalDataManager.ShouldRetryWithAlternativeAddress(candidateException))
                            {
                                r2c_RegisterOrLogin = null;
                                continue;
                            }

                            throw;
                        }
                    }

                    loginBtn.interactable = true;
                    if (r2c_RegisterOrLogin == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"登录中断[连接Realm] {realmAddress} ({ErrorCode.ERR_PeerDisconnect})");
                        return;
                    }
                    //提示错误信息
                    if (r2c_RegisterOrLogin.Error != 0)
                    {
                        if (r2c_RegisterOrLogin.Error == ErrorCode.ERR_PeerDisconnect)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"登录中断[{loginStage}] {realmAddress} ({r2c_RegisterOrLogin.Error})");
                            return;
                        }
                        if (r2c_RegisterOrLogin.Error == 132)
                        {
                            //实名认证
                            UIComponent.Instance.VisibleUI(UIType.UIRealName, account, realmSession, r2c_RegisterOrLogin.Error);
                            return;
                        }

                        
                        if (r2c_RegisterOrLogin.Error == 117)
                        {
                            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                            uIConfirm.AddCancelEventAction(() => Application.Quit());
                            uIConfirm.AddActionEvent(() => Application.Quit());
                            TimeSpan timeSpan = GetSpacingTime_Seconds(r2c_RegisterOrLogin.BanTillTime);
                            DateTime dateTime = TimeHelper.GetDateTime_Milliseconds(r2c_RegisterOrLogin.BanTillTime);
                            var str = $"您的账号存在存在异常行为({r2c_RegisterOrLogin.BanReason})\n被封禁{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes}分钟\n解封时间：{dateTime.Year}年{dateTime.Month}月{dateTime.Hour}:{dateTime.Minute}";
                            uIConfirm.SetTipText(str, true, "账号异常");

                            static TimeSpan GetSpacingTime_Seconds(long Seconds)
                            {
                                //获取时间戳 秒
                                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Seconds);
                                DateTime curdateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                                TimeSpan timeSpan = new TimeSpan(startTime.Ticks);
                                TimeSpan CurtimeSpan = new TimeSpan(curdateTime.Ticks);
                                return timeSpan.Subtract(CurtimeSpan).Duration();
                            }
                        }
                        else if (r2c_RegisterOrLogin.Error == 134)//实名认证
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIRealName, account, realmSession, r2c_RegisterOrLogin.Error);
                            return;
                        }
                        else
                        {
                            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                            uIConfirm.SetTipText($"{r2c_RegisterOrLogin.Error.GetTipInfo()}({r2c_RegisterOrLogin.Error})", true);
                        }
                        return;
                    }
                    Log.DebugBrown("R2C_GetLastLoginToTheRegion===>登录的uid" + account);
                    //获取上一次的登录信息
                    loginStage = "获取上次区服";
                    LoginStageTrace.Append($"Stage={loginStage} uid={account}");
                    R2C_GetLastLoginToTheRegion r2C_GetLastLoginToTheRegion = (R2C_GetLastLoginToTheRegion)await CallWithTimeout(realmSession, new C2R_GetLastLoginToTheRegion { 
                        Uid=account,
                        Type=0
                    }, RealmRpcTimeoutMs, $"{loginStage} {realmAddress}");
                    LoginStageTrace.Append($"Stage={loginStage} responseError={r2C_GetLastLoginToTheRegion.Error} area={r2C_GetLastLoginToTheRegion.GameAreaId} line={r2C_GetLastLoginToTheRegion.Gameline}");
                    Log.DebugBrown("获取上一次信息" + r2C_GetLastLoginToTheRegion.Error);
                    if (r2C_GetLastLoginToTheRegion.Error != 0)
                    {
                        if (r2C_GetLastLoginToTheRegion.Error == ErrorCode.ERR_PeerDisconnect)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"登录中断[{loginStage}] {realmAddress} ({r2C_GetLastLoginToTheRegion.Error})");
                            return;
                        }

                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{loginStage}:{r2C_GetLastLoginToTheRegion.Error.GetTipInfo()}({r2C_GetLastLoginToTheRegion.Error})");
                    }
                    else
                    {
                        if (r2C_GetLastLoginToTheRegion.GameAreaId == 0)
                        {
                            Log.DebugGreen($"上一次的区服信息：{r2C_GetLastLoginToTheRegion.GameAreaId}区{r2C_GetLastLoginToTheRegion.Gameline}线");
                            GlobalDataManager.EnterZoneID = 1;
                            GlobalDataManager.EnterLineID = 1;
                        }
                        else
                        {
                           
                            GlobalDataManager.EnterZoneID = r2C_GetLastLoginToTheRegion.GameAreaId;
                            GlobalDataManager.EnterLineID = r2C_GetLastLoginToTheRegion.Gameline;
                          
                        }
                    }


                    realmSession.Dispose();
                    LoginStageTrace.Append("RealmSessionDisposedAfterLastRegion");
                    LogCollectionComponent.Instance.UserId = r2c_RegisterOrLogin.UserId;
                    LogCollectionComponent.Instance.IP = r2c_RegisterOrLogin.SelfIP;
                    LogCollectionComponent.Instance.Info("#登录# 使用手机号登录成功");

                    //服务器返回了网关地址
                    //那么就根据网关地址创建一个新的Session 连接到网关去
                    gateAddress = r2c_RegisterOrLogin.Address;
                    G2C_LoginGateResponse g2CLoginGate = null;
                    foreach (string gateCandidate in GlobalDataManager.GetGateConnectAddresses(gateAddress))
                    {
                        gateAddress = gateCandidate;
                        LoginStageTrace.Append($"GateCandidateStart address={gateAddress}");

                        try
                        {
                            loginStage = "连接Gate";
                            LoginStageTrace.Append($"Stage={loginStage} address={gateAddress}");
                            ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(gateAddress);
                            GlobalDataManager.Address = gateAddress;

                            //在Scene实体中添加SessionComponent组件 并且缓存Session对象 以后就可以直接获取来发送消息
                            ETModel.SessionComponent modelSessionComponent = ETModel.Game.Scene.GetComponent<ETModel.SessionComponent>();
                            if (modelSessionComponent == null)
                            {
                                modelSessionComponent = ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>();
                            }
                            modelSessionComponent.Session = gateSession;

                            //这里跟上面逻辑一样,创建热更层的Session,关联到主工程
                            SessionComponent hotfixSessionComponent = Game.Scene.GetComponent<SessionComponent>();
                            if (hotfixSessionComponent == null)
                            {
                                hotfixSessionComponent = Game.Scene.AddComponent<SessionComponent>();
                            }
                            hotfixSessionComponent.Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);

                            if (Game.Scene.GetComponent<SessionHelper>() == null)
                            {
                                Game.Scene.AddComponent<SessionHelper>();
                            }

                            loginStage = "Gate登录";
                            LoginStageTrace.Append($"Stage={loginStage} key={(this.NewKey==0?r2c_RegisterOrLogin.Key:this.NewKey)}");
                            g2CLoginGate = (G2C_LoginGateResponse)await CallWithTimeout(SessionComponent.Instance.Session, new C2G_LoginGateRequest()
                            {
                                Key = this.NewKey==0?r2c_RegisterOrLogin.Key:this.NewKey,
                                ChannelId = AgentTool.agentstr,//运营商id
                            }, GateRpcTimeoutMs, $"{loginStage} {gateAddress}");
                            LoginStageTrace.Append($"Stage={loginStage} responseError={g2CLoginGate.Error} newKey={g2CLoginGate.NewKey}");

                            if (GlobalDataManager.IsNetworkAddressError(g2CLoginGate.Error))
                            {
                                LoginStageTrace.Append($"GateCandidateRetry address={gateAddress} error={g2CLoginGate.Error}");
                                gateSession.Dispose();
                                GlobalDataManager.Address = string.Empty;
                                g2CLoginGate = null;
                                continue;
                            }

                            GlobalDataManager.Address = gateAddress;
                            break;
                        }
                        catch (Exception gateException)
                        {
                            Log.Error(gateException);
                            LoginStageTrace.Append($"GateCandidateException address={gateAddress} type={gateException.GetType().Name} message={gateException.Message}");
                            GlobalDataManager.Address = string.Empty;
                            g2CLoginGate = null;

                            if (GlobalDataManager.ShouldRetryWithAlternativeAddress(gateException))
                            {
                                continue;
                            }

                            throw;
                        }
                    }

                    if (g2CLoginGate == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"登录中断[连接Gate] {gateAddress} ({ErrorCode.ERR_PeerDisconnect})");
                        this.NewKey = 0;
                        return;
                    }

                    if (g2CLoginGate.Error != 0)
                    {
                        LogCollectionComponent.Instance.Info($"#Session# LoginGate Error:{g2CLoginGate.Error}");
                        if (g2CLoginGate.Error == ErrorCode.ERR_PeerDisconnect)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"登录中断[{loginStage}] {gateAddress} ({g2CLoginGate.Error})");
                            this.NewKey = 0;
                            return;
                        }
                        if (g2CLoginGate.Error == 111)
                        {
                            this.NewKey = g2CLoginGate.NewKey;
                            if (++loginCount >= 3)
                            {
                                this.NewKey = 0;
                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2CLoginGate.Error.GetTipInfo()}({g2CLoginGate.Error})");

                            }
                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2CLoginGate.Error.GetTipInfo()}({g2CLoginGate.Error})");
                            this.NewKey =0;
                        }
                    }
                    else
                    {
                        LogCollectionComponent.Instance.Info($"#Session# LoginGate 成功 session.Id:{SessionComponent.Instance.Session.session.Id}");
                        SessionComponent.Instance.Session.AddComponent<PingComponent>();

                        //登录成功 将账号保存到本地 方便下一次的登录 使用
                        AccountConfigInfo config = LocalDataJsonComponent.Instance.LoadData<AccountConfigInfo>(LocalJsonDataKeys.UserAccount) ?? new AccountConfigInfo();
                        config.account = account;
                        config.password = password;
                        config.IsRead = true;
                        LocalDataJsonComponent.Instance.SavaData(config, LocalJsonDataKeys.UserAccount);
                        // 设置这个 Session 可以重连
                        #region 断线重连

                        // 记录下次重连 GateServer 需要用的密钥
                        GlobalDataManager.GateLoginKey = g2CLoginGate.NewKey.ToString();

                      
                        #endregion

                        //登录成功事件
                        LoginStageTrace.Append("LoginFinishEventRun");
                        Game.EventSystem.Run(EventIdType.LoginFinish);
                    }


                }
                catch (Exception e)
                {
                    Log.Error(e);
                    LoginStageTrace.Append($"Exception stage={loginStage} type={e.GetType().Name} message={e.Message}");
                    string phase = string.IsNullOrEmpty(GlobalDataManager.Address) ? "Realm" : "Gate";
                    string target = string.IsNullOrEmpty(GlobalDataManager.Address)
                        ? (string.IsNullOrEmpty(GlobalDataManager.LastRealmAddress) ? GlobalDataManager.LoginConnetIP : GlobalDataManager.LastRealmAddress)
                        : GlobalDataManager.Address;
                    string detail = e is RpcException rpcException
                        ? $"登录失败[{loginStage}/{phase}] {target} RpcError:{rpcException.Error}"
                        : $"登录失败[{loginStage}/{phase}] {target} {e.GetType().Name}";
                    UIComponent.Instance.VisibleUI(UIType.UIHint, detail);
                }
            }
        }



        public void OpenRegisterPanel(LoginPanelType panelType)
        {
            if (LoginPanel != null)
            {
                LoginPanel.SetActive(panelType == LoginPanelType.Login);
                if (panelType == LoginPanelType.Login)
                {
                    LoginPanel.transform.SetAsLastSibling();
                }
            }

            if (RegisterPanel != null)
            {
                RegisterPanel.SetActive(panelType == LoginPanelType.Register);
                if (panelType == LoginPanelType.Register)
                {
                    RegisterPanel.transform.SetAsLastSibling();
                }
            }

            if (ResetPassWorld != null)
            {
                ResetPassWorld.SetActive(panelType == LoginPanelType.Rester);
                if (panelType == LoginPanelType.Rester)
                {
                    ResetPassWorld.transform.SetAsLastSibling();
                }
            }
        }
    }
}
