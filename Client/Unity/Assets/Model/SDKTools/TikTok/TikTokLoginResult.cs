using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    //登录结果类声明
    public class TikTokLoginResult
    {  //错误码
        public int code;
        //错误信息
        public string message;
        //用户信息
        public string accessToken;//access_token
                                  //扩展字段
        public bool isGuest; //是否游客
        public int userType; //用户类型，绑定后为绑定的用户类型
        public long userId; //uid
        public int identityType;//云控返回的用户实名认证的等级 1=low，2=mid，3=high
        public string nickname; //用户抖音or联运昵称
        public string avatarUrl; //用户抖音or联运头像
    }
}
