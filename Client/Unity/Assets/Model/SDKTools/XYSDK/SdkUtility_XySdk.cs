using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETModel
{
    /// <summary>
    /// XySdk
    /// </summary>
    public partial class SdkUtility
    {
        //创建角色
        public void CreatRole(string[] objs)
        {
#if UNITY_ANDROID
            if (Init.instance.e_SDK==E_SDK.XY_SDK)
                javaActive.Call("CreateRole", objs);
#endif
        }

        //角色升级
        public void UpdateRoleGrade(string[] objs)
        {
#if UNITY_ANDROID
            if (Init.instance.e_SDK==E_SDK.XY_SDK)
                javaActive.Call("UpdateRoleGrade", objs);
         /*   else if (Init.instance.e_SDK == E_SDK.HaXi)
                javaActive.Call("UpdateRoleGrade", objs);*/
#endif
        }

        //登录游戏服务器：在用户选择游戏服务器登录后调用
        public void LoginGame(string[] objs)
        {
#if UNITY_ANDROID
            if (Init.instance.e_SDK==E_SDK.XY_SDK)
                javaActive.Call("loginGame", objs);
#endif
        }
        //游玩记录
        public void PlayLog(string[] objs)
        {
#if UNITY_ANDROID
            if (Init.instance.e_SDK==E_SDK.XY_SDK)
                javaActive.Call("playLog", objs);
#endif
        }
       

    }
}
