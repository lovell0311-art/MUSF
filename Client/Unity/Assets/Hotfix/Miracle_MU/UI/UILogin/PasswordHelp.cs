using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 密码强度判断组件
    /// </summary>
    public static class PasswordHelp 
    {
        /// <summary>
        /// 密码验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RegexLevel(string input)
        {
            //复杂 必须有数字与字母和符号混合组成的8-15位数(就是剔除其余格式可能，留下目标格式)
            string rule1 = @"^(?![a-zA-Z]{8,15}$)(?![\d{8,15}])(?![!@#$%^&*.]{8,15}$)(?![a-zA-Z\d]{8,15}$)(?![a-zA-Z!@#$%^&*.]{8,15})(?![\d!@#$%^&*.]{8,15}$)
        ([a-zA-Z\d!@#$%^&*.]{8,15}&)";
            //中等  由数字和字母或者数字和符号或者字母和符号组成的8-15位数
            string rule2 = @"^(?![a-zA-Z]{8,15}$)(?![\d{8,15}])(?![!@#$%^&*.]{8,15}$)([a-zA-Z\d]|[a-zA-Z!@#$%^&*.]|[\d!@#$%^&*.]){8,15}$";
            //简单  由单一的数字或者字母或者符号组成的8-15位数
            string rule3 = @"^([a-zA-Z]|[\d{8,15}]|[!@#$%^&*.]){8,15}$";
            Regex r1 = new Regex(rule1);
            Regex r2 = new Regex(rule2);
            Regex r3 = new Regex(rule3);
            if (r1.IsMatch(input))
            {
                return "<color=#3eb370>密码强度:复杂<color>";
            }
            else if (r2.IsMatch(input))
            {
                return "<color=#ffd900>密码强度：中等</color>";
            }
            else if (r3.IsMatch(input))
            {
                return "<color=#d7003a>密码强度：容易</color>";
            }
            else
           // return "<color=#d7003a>密码输入格式不正确</color>";
            return "";
        }

    }
}
