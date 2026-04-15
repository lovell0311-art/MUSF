using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using MongoDB.Bson;

namespace ETHotfix
{
    /// <summary>
    /// 商城 奇迹币、元宝数量变动
    /// </summary>
    [MessageHandler]
    public class G2C_PlayerShopInfoUpdataMessage_Handler : AMHandler<G2C_PlayerShopInfoUpdataMessage>
    {
        protected override void Run(ETModel.Session session, G2C_PlayerShopInfoUpdataMessage message)
        {

           // Log.DebugBrown("G2C_PlayerShopInfoUpdataMessage_Handler==>" + JsonHelper.ToJson(message));
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.MiracleCoin,message.MiracleCoin);
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.MoJing, message.CurrentYuanbao);
            UnitEntityComponent.Instance.LocalRole.RechargeStates = message.RechargeStatus;
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.AccumulatedRecharge, message.AccumulatedRecharge);//累计充值额度
          
            UnitEntityComponent.Instance.LocalRole.RechargeRecord = message.RechargeRecord;//首充Dictionary<int, int> <档次，金额>

            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.InSituCd, message.InSituCd);//原地复活到期时间

            ///更新·商城面板上的奇迹币、元宝数量
            if (UIComponent.Instance.Get(UIType.UIShop)?.GetComponent<UIShopComponent>() is UIShopComponent shopComponent)
            {
                shopComponent.UpdateCoin();
            }

            //购买了小月卡
            if (message.MinMCEndTime != 0)
            {


                if (UnitEntityComponent.Instance.LocalRole.MinMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    //注册倒计时
                    UIMainComponent.Instance.MinMothCard();
                }
                //更新 月卡时间
                UnitEntityComponent.Instance.LocalRole.MinMonthluCardTimeSpan = TimeHelper.GetSpacingTime_Milliseconds(message.MinMCEndTime);
            }
            //购买了大月卡
            if (message.MaxMCEndTime != 0)
            {
                if (UnitEntityComponent.Instance.LocalRole.MaxMonthluCardTimeSpan.TotalSeconds <= 0)
                {
                    //注册倒计时
                    UIMainComponent.Instance.MaxMothCard();
                }
                //更新 月卡时间
                UnitEntityComponent.Instance.LocalRole.MaxMonthluCardTimeSpan = TimeHelper.GetSpacingTime_Milliseconds(message.MaxMCEndTime);
            }
            //原地复活CD时间
            if (message.InSituCd != 0)
            {
                if (UnitEntityComponent.Instance.LocalRole.InsiteTimeSpan.TotalSeconds <= 0)
                {
                    //注册倒计时
                    UIMainComponent.Instance.InSituCd();
                }
                //Log.DebugGreen($"时间->{message.InSituCd}");
                UnitEntityComponent.Instance.LocalRole.InsiteTimeSpan = TimeHelper.GetSpacingTime_Milliseconds(message.InSituCd);
            }
            //刷新首充界面
            if (UIComponent.Instance.Get(UIType.UIFirstCharge)?.GetComponent<UIFirstChargeComponent>() is UIFirstChargeComponent firstChargeComponent)
            {
                firstChargeComponent.ChangeBtnState();
            }
        }
    }
}
