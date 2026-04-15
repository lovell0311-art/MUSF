using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [ObjectSystem]
    public class UIPayComponentAwake : AwakeSystem<UIPayComponent>
    {
        public override void Awake(UIPayComponent self)
        {
          self.collector=self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.gameObject.GetComponent<Canvas>().planeDistance = 50;
            self.collector.GetButton("Close").onClick.AddSingleListener(()=>{ UIComponent.Instance.Remove(UIType.UIPay); });
            self.collector.GetButton("AliPay").onClick.AddSingleListener(()=> { self.Pay("alipayWap").Coroutine(); });
            self.collector.GetButton("WebPay").onClick.AddSingleListener(()=> { GlobalDataManager.HaoYiTopUp(); });
            self.collector.GetButton("WeChatPay").onClick.AddSingleListener(()=> { self.Pay("wechatLiteH5").Coroutine(); });

        }
    }

    /// <summary>
    /// 第三方 微信、支付宝支付
    /// </summary>
    public class UIPayComponent : Component,IUGUIStatus
    {
      public ReferenceCollector collector;

        public int payType = (int)E_PlayerShopQuotaType.StoreRechargeI;

        public async ETVoid Pay(string type)
        {
            if (type == "alipayWap")
            {
                G2C_MyPayTopUp c2G_MyPayTopUp = (G2C_MyPayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyPayTopUp
                {
                    RechargeType = payType,
                });
                if (c2G_MyPayTopUp.Error != 0)
                {
                    Log.DebugRed($"c2G_MyPayTopUp.Error:{c2G_MyPayTopUp.Error}");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_MyPayTopUp.Error.GetTipInfo());
                }
                else
                {
                    SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { c2G_MyPayTopUp.OrderStr });
                }
            }
            else
            {
                //UIComponent.Instance.VisibleUI(UIType.UIHint, "此渠道暂未开通");
                //Log.Info("111111111111 ");
                //WWWForm form = new WWWForm();
                //form.AddField("mchid", "1675637087");
                //form.AddField("serial", "37BC0FBDE560D10903516A8B6F78D791E8549506");
                //form.AddField("appid", "wx7b5d6d6c9f9197b2");
                //form.AddField("out_trade_no", TimeHelper.ClientNow().ToString());
                //form.AddField("description", "魔晶");
                //form.AddField("notify_url", "http://zzws.sfapi.zzws.top:65001/api/Pay/MyWeChatPayinfo");
                //form.AddField("money", "100");
                //form.AddField("key", "apiclient_key.pem");
                //form.AddField("cert", "apiclientnew_cert.pem");
                //form.AddField("api", "h5");
                //string data = await SdkCallBackComponent.Instance.sdkUtility.Post("http://wechat.zzws.top:82/wxpay.php", form);
                //WeChatPayInfo info = JsonHelper.FromJson<WeChatPayInfo>(data);
                //SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { info.data, "wechat" });
                //Application.OpenURL(info.data);

                G2C_MyV4PayTopUp c2G_MyV4PayTopUp = (G2C_MyV4PayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyV4PayTopUp
                {
                    RechargeType = payType,
                    PayType = type
                    //PayType="alipayWap"
                });
                //Log.Info("----------- " + JsonHelper.ToJson(c2G_MyV4PayTopUp));
                if (c2G_MyV4PayTopUp.Error != 0)
                {
                    UIComponent.Instance.Remove(UIType.UIPay);
                    UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_MyV4PayTopUp.Error.GetTipInfo());
                }
                else
                {
                    UIComponent.Instance.Remove(UIType.UIPay);
                    //Application.OpenURL(c2G_MyV4PayTopUp.OrderStrURL);
                    SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { c2G_MyV4PayTopUp.OrderStrURL, "wechat" });
                }
            }
        }






        //public async ETVoid Pay(string type) 旧的
        //{
        //    G2C_MyV4PayTopUp c2G_MyV4PayTopUp = (G2C_MyV4PayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyV4PayTopUp
        //    {
        //        RechargeType = payType,
        //        PayType = type
        //        //PayType="alipayWap"
        //    });
        //    if (c2G_MyV4PayTopUp.Error != 0)
        //    {
        //        UIComponent.Instance.Remove(UIType.UIPay);
        //        UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_MyV4PayTopUp.Error.GetTipInfo());
        //    }
        //    else
        //    {
        //        UIComponent.Instance.Remove(UIType.UIPay);
        //        Application.OpenURL(c2G_MyV4PayTopUp.OrderStrURL);
        //    }
        //}
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
        }

        public void OnVisible(object[] data)
        {
            if (data.Length > 0)
            {
                payType = int.Parse(data[0].ToString());
                Log.DebugWhtie($"请求支付：{payType} -> {(E_PlayerShopQuotaType)payType}");
            }
        }

        public void OnVisible()
        {
           
        }

        public void OnInVisibility()
        {
           
        }
    }
}
