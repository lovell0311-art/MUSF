using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using ILRuntime.Runtime;



namespace ETHotfix
{
    /// <summary>
    /// 测试默认充值
    /// </summary>
    public partial class UIShopComponent
    {
        public void Init_TopUp()
        {
            Transform TopUps = collector.GetGameObject("TopUps").transform;
            for (int i = 0, length = TopUps.childCount; i < length; i++)
            {
                Button button = TopUps.GetChild(i).GetComponent<Button>();
                var tipvalue = System.Text.RegularExpressions.Regex.Replace(TopUps.GetChild(i).GetComponentInChildren<Text>().text, @"[^0-9]+", "").ToInt32();
                var value = (int)E_PlayerShopQuotaType.StoreRechargeI + i;
               
                button.onClick.AddSingleListener(async () =>
                {
                    TopUpComponent.Instance.TopUp(value).Coroutine();
                   /* G2C_RechargeResponse g2C_RechargeResponse = (G2C_RechargeResponse)await SessionComponent.Instance.Session.Call(new C2G_RechargeRequest
                    {
                        Page = value
                    });
                    if (g2C_RechargeResponse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RechargeResponse.Error.GetTipInfo());
                        // Log.DebugRed($"g2C_RechargeResponse.Error:{g2C_RechargeResponse.Error}");
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"成功充值 {tipvalue} 元");
                    }*/

                });
            }
        }
        //获取充值类型
        public string GetTypeName(int value) => value switch
        {
            0 => "商城充值",
            1 => "首充",
            2 => "限时充值",
            3 => "七天充值",
            _ => "充值"
        };
       
    }
}