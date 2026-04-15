using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{

    [ObjectSystem]
    public class SdkCallBackComponentAwake : AwakeSystem<SdkCallBackComponent>
    {
        public override void Awake(SdkCallBackComponent self)
        {
            SdkCallBackComponent.Instance = self;
            self.sdkUtility = Component.Global.GetComponent<SdkUtility>();
        }
    }

    public class SdkCallBackComponent : Component
    {
        public static SdkCallBackComponent Instance;
        public SdkUtility sdkUtility;

        #region 抖音SDK
        public Action<string> LoginCallBack;//登录
        public Action<string> SwitchLoginCallBack;//切换账号
        public Action<string> LogoutCallBack;//账号退出
        public Action<string> ExitCallBack;//退出游戏
        public Action<string> PaySucessCallBack;// 支付完成回调
        public Action<string> PayFailureCallBack;// 支付失败回调
        #endregion
    }
}
