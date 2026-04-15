using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace ETModel
{
    /// <summary>
    /// SDK
    /// </summary>
    public partial class SdkUtility : MonoBehaviour
    {

        AndroidJavaClass javaClass;
        public AndroidJavaObject javaActive;
        string javaClassStr = "com.unity3d.player.UnityPlayer";
        string javaActiveStr = "currentActivity";

        public string CallAllObjName = "Global";
        [HideInInspector]
        public string LoginInfo;

        #region IOSAliPay
#if UNITY_IPHONE
        //外部函数 这是我们写的ios支付接口
        //获取参数
        [HideInInspector]
        public string appid;
        [HideInInspector]
        public string mch_id;
        [HideInInspector]
        public string prepayid;
        [HideInInspector]
        public string noncestr;
        [HideInInspector]
        public string timestamp;
        [HideInInspector]
        public string packageValue;
        [HideInInspector]
        public string sign;

        [DllImport("__Internal")]
        static extern void RBSDK_UploadRoleInfo(IntPtr strings);

        [DllImport("__Internal")]
        static extern bool IsWechatInstalled_iOS();
        [DllImport("__Internal")]
        static extern void RegisterApp_iOS(string appId);
        [DllImport("__Internal")]
        static extern void WechatPay_iOS(string appId, string partnerId, string prepayId, string nonceStr, int timeStamp, string packageValue, string sign);
        [DllImport("__Internal")]
        static extern float AliPay_iOS(string info);

        [DllImport("__Internal")]
        static extern void RegisterApp(string appId, string appkey);

        [DllImport("__Internal")]
        static extern void Pay(string orderInfo);

        [DllImport("__Internal")]
        static extern void SendLogin();
#endif
        /// <summary>
        /// 调用阿里支付的接口
        /// </summary>
        /// <param name="info">Info.</param>
        public void SendAliPay(string info)
        {
#if UNITY_IPHONE
            AliPay_iOS(info);
#endif
        }
        public void SendPay(string info)
        {
#if UNITY_IPHONE
            Pay(info);
#endif
        }

        #endregion
        private void Start()
        {
#if UNITY_ANDROID
            switch (Init.instance.e_SDK)
            {
                case E_SDK.NONE:
                    break;
                case E_SDK.HAO_YI_SDK:
                    break;
                case E_SDK.TIKTOK_SDK:
                case E_SDK.XY_SDK:
                case E_SDK.SHOU_Q:
                case E_SDK.ZHIFUBAO_WECHAT:
                case E_SDK.HaXi:
                    //初始化 获得项目对应的MainActivity
                    javaClass = new AndroidJavaClass(javaClassStr);
                    javaActive = javaClass.GetStatic<AndroidJavaObject>(javaActiveStr);
                    javaActive.Set<string>("CallAllObjName", CallAllObjName);//设置安卓调用Unity方法的OBj名字
                    break;
                default:
                    break;
            }
#endif
        }

        public void init(string appId, string appkey)
        {
#if UNITY_IPHONE
            Log.Info("RegisterApp   ");
            RegisterApp(appId, appkey);
#endif
        }

        //隐私协议许可
        public void ShowUserAgreementPrivac()
        {
#if UNITY_ANDROID
            if (Init.instance.e_SDK == E_SDK.XY_SDK)
                javaActive.Call("ShowUserAgreementPrivac");
#endif
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            Log.Info("Login-------------------4 ");
#if UNITY_ANDROID
            if (Init.instance.e_SDK == E_SDK.TIKTOK_SDK || Init.instance.e_SDK == E_SDK.XY_SDK || Init.instance.e_SDK == E_SDK.HaXi)
            {
                javaActive.Call("Login", "LoginCallCack");
            }
#elif UNITY_IPHONE
            Log.Info("登录   ");
            SendLogin();
#endif
        }
        /// <summary>
        /// 切换账号
        /// </summary>
        public void SwitchLogin()
        {
#if UNITY_ANDROID
            javaActive.Call("SwitchLogin", "SwitchLoginCallBack");
#endif
        }
        //退出游戏
        public void LoginOut()
        {

#if UNITY_ANDROID
            if (Init.instance.e_SDK == E_SDK.TIKTOK_SDK)
            {
                javaActive.Call("LoginOut", "LoginOutCallBack");
            }
            else if (Init.instance.e_SDK == E_SDK.XY_SDK)
            {
                javaActive.Call("Logout");
            }
#endif
        }
        //是否登录
        public bool IsLogin()
        {
#if UNITY_ANDROID
            return javaActive.Call<bool>("IsLogin");
#else
            return false;
#endif
        }

        //退出
        public void Exit()
        {
#if UNITY_ANDROID
            if (Init.instance.e_SDK == E_SDK.XY_SDK)
            {
                javaActive.Call("Exit");
            }

#endif
        }

        //获取获取风控参数
        public string GetRiskControlInfo()
        {
#if UNITY_ANDROID
            return javaActive.Call<string>("GetRiskControlInfo");
#else
            return null;
#endif
        }
        //获取年龄
        public int GetAge()
        {
#if UNITY_ANDROID
            return javaActive.Call<int>("GetAge");
#else
            return 0;
#endif
        }

        //支付
        public void Pay(string[] objs)
        {
#if UNITY_ANDROID
            switch (Init.instance.e_SDK)
            {
                case E_SDK.NONE:
                    break;
                case E_SDK.HAO_YI_SDK:
                    break;
                case E_SDK.TIKTOK_SDK:
                case E_SDK.SHOU_Q:
                case E_SDK.HaXi:
                    javaActive.Call("Pay", objs);
                    break;
                case E_SDK.XY_SDK:
                    javaActive.Call("Pay", objs);
                    break;
                case E_SDK.ZHIFUBAO_WECHAT:

                    javaActive.Call("AliPay", objs[0]);
                    break;

                default:
                    break;
            }
#elif UNITY_IPHONE
            switch (Init.instance.e_SDK)
            {
                case E_SDK.NONE:
                    break;
                case E_SDK.HAO_YI_SDK:
                    break;
                case E_SDK.TIKTOK_SDK:
                case E_SDK.SHOU_Q:

                    break;
                case E_SDK.XY_SDK:

                    break;
                case E_SDK.HaXi:
                    SendPay(objs[0]);
                    break;
                case E_SDK.ZHIFUBAO_WECHAT:
                    SendAliPay(objs[0]);
                    break;

                default:
                    break;
            }

#endif
        }
        //更新角色信息

        public void UploadRoleInfo(string[] objs)
        {
#if UNITY_IOS
            // 将字符串数组转换为指针数组
            IntPtr[] ptrArray = new IntPtr[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                ptrArray[i] = Marshal.StringToHGlobalAnsi(objs[i]);
            }

            // 分配内存并复制指针数组
            IntPtr ptr = Marshal.AllocHGlobal(ptrArray.Length * Marshal.SizeOf(typeof(IntPtr)));
            Marshal.Copy(ptrArray, 0, ptr, ptrArray.Length);

            // 调用外部方法
            RBSDK_UploadRoleInfo(ptr);

            // 释放内存
            for (int i = 0; i < objs.Length; i++)
            {
                Marshal.FreeHGlobal(ptrArray[i]);
            }
            Marshal.FreeHGlobal(ptr);
#endif
#if UNITY_ANDROID
            javaActive.Call("UploadRoleInfo", objs);
#endif
        }
        //事件上报
        public void ReportGameEvent(string eventname, string str)
        {
#if UNITY_ANDROID
            if (Init.instance.e_SDK == E_SDK.SHOU_Q)
            {
                javaActive.Call("ReportGameEvent", eventname, str);
            }
#endif
        }

        /***************************************************
         SDK回调函数
         ****************************************************/
        //登录
        public void LoginCallCack(string info)
        {
            Log.Info("LoginCallCack---- " + info);
            if (SdkCallBackComponent.Instance == null)
            {
                LoginInfo = info;
            }
            else
            {
                SdkCallBackComponent.Instance.LoginCallBack?.Invoke(info);
            }
        }
        //切换账号
        public void SwitchLoginCallBack(string info)
        {
            SdkCallBackComponent.Instance.SwitchLoginCallBack?.Invoke(info);
        }
        //退出
        public void LoginOutCallBack(string info)
        {
            SdkCallBackComponent.Instance.SwitchLoginCallBack?.Invoke(info);
        }
        //支付成功
        public void PaySucessCallBack(string info)
        {
            SdkCallBackComponent.Instance.PaySucessCallBack?.Invoke(info);
        }
        //支付失败
        public void PayFailureCallBack(string info)
        {
            SdkCallBackComponent.Instance.PayFailureCallBack?.Invoke(info);
        }
    }

}
