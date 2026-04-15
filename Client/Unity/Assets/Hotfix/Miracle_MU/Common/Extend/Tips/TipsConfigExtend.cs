using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 提示类
    /// </summary>
    public static class TipsConfigExtend
    {

        private static void GetTipsInfo(this int self,out TipsInfo info) 
        {
            info = null;
            switch (self/10000)
            {
                case 4:
                    Tips_InfoConfig tips_Info = ConfigComponent.Instance.GetItem<Tips_InfoConfig>(self);
                    if (tips_Info == null) return;

                    info = new TipsInfo
                    {
                        Id = tips_Info.Id,
                        TipsDescribe = tips_Info.TipsDescribe,
                        str = tips_Info.str,
                    };
                    break;
                case 2:
                    Tips_CodeErrorConfig tips_CodeErrorConfig = ConfigComponent.Instance.GetItem<Tips_CodeErrorConfig>(self);
                    if (tips_CodeErrorConfig == null) return;

                    info = new TipsInfo
                    {
                        Id = tips_CodeErrorConfig.Id,
                        TipsDescribe = tips_CodeErrorConfig.TipsDescribe,
                        str = tips_CodeErrorConfig.str,
                    };
                    break;
                default:
                    Tips_LogicErrorConfig tips_LogicError = ConfigComponent.Instance.GetItem<Tips_LogicErrorConfig>(self);
                    if (tips_LogicError == null) return;

                    info = new TipsInfo
                    {
                        Id = tips_LogicError.Id,
                        TipsDescribe = tips_LogicError.TipsDescribe,
                        str = tips_LogicError.str,
                    };
                    break;
            }
        }
        private static void GetTipsInfo_Ref(this int self, ref TipsInfo info)
        {
            switch (self / 10000)
            {
                case 4:
                    Tips_InfoConfig tips_Info = ConfigComponent.Instance.GetItem<Tips_InfoConfig>(self);
                    if (tips_Info == null) return;

                    info = new TipsInfo
                    {
                        Id = tips_Info.Id,
                        TipsDescribe = tips_Info.TipsDescribe,
                        str = tips_Info.str,
                    };
                    break;
                case 2:
                    Tips_CodeErrorConfig tips_CodeErrorConfig = ConfigComponent.Instance.GetItem<Tips_CodeErrorConfig>(self);
                    if (tips_CodeErrorConfig == null) return;

                    info = new TipsInfo
                    {
                        Id = tips_CodeErrorConfig.Id,
                        TipsDescribe = tips_CodeErrorConfig.TipsDescribe,
                        str = tips_CodeErrorConfig.str,
                    };
                    break;
                default:
                    Tips_LogicErrorConfig tips_LogicError = ConfigComponent.Instance.GetItem<Tips_LogicErrorConfig>(self);
                    if (tips_LogicError == null) return;

                    info = new TipsInfo
                    {
                        Id = tips_LogicError.Id,
                        TipsDescribe = tips_LogicError.TipsDescribe,
                        str = tips_LogicError.str,
                    };
                    break;
            }
        }
        
        public static string GetTipInfo(this int self)
        {
            self.GetTipsInfo(out TipsInfo tips);
            if (tips == null)
            {
                if (Enum.IsDefined(typeof(SocketError), (int)self))
                {
                    return $"网络异常";
                }
                else
                {             
                    return $"未知错误：{self}";
                }

            }
            return tips.TipsDescribe;
        }
    }
}
