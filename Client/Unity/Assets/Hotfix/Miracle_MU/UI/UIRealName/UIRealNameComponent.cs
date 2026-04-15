using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;
using System;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIRealNameComponentAwake : AwakeSystem<UIRealNameComponent>
    {
        public override void Awake(UIRealNameComponent self)
        {
            self.Init_RealName();
        }
    }

    /// <summary>
    /// 实名认证
    /// </summary>
    public  class UIRealNameComponent : Component,IUGUIStatus
    {
        ReferenceCollector collector_RealName;
        private string input_Name, input_Id;
        private string account;
        public Session session;
        public int errorId = 0;
        public void Init_RealName() 
        {
            
            collector_RealName = this.GetParent<UI>().GameObject.GetReferenceCollector();
            collector_RealName.GetInputField("NameInputField").onEndEdit.AddSingleListener(value =>  input_Name = value);
            collector_RealName.GetInputField("IDInputField").onEndEdit.AddSingleListener(value => input_Id = value);
            collector_RealName.GetButton("SureBtn").onClick.AddSingleListener(async () =>
            {
                if (string.IsNullOrEmpty(input_Name))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入姓名！");
                    return;
                }
                if (string.IsNullOrEmpty(input_Id))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入身份证号！");
                    return;
                }
                if (input_Id.Length  != 18)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "身份证号输入有误！");
                    return;
                }
                if (errorId == 134)
                {
                    G2C_RealNameAuthentication().Coroutine();
                }
                    
                if (errorId == 132)
                {
                    G2C_IdCardInspect().Coroutine();
                }
                
            });
            collector_RealName.GetButton("CancleBtn").onClick.AddSingleListener(() => { UIComponent.Instance.Remove(UIType.UIRealName); });
        }


        public async ETVoid G2C_IdCardInspect()
        {
            Log.DebugBrown("请求实名认证");
            G2C_IdCardInspectResponse g2C_IdCardInspect = (G2C_IdCardInspectResponse)await session.Call(new C2G_IdCardInspectRequest
            {
                Name = input_Name,
                Account = long.Parse(account),
                IdCardCord = input_Id
            });

            if (g2C_IdCardInspect.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_IdCardInspect.Error.GetTipInfo());
                Log.DebugRed($"{g2C_IdCardInspect.Error}");
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "认证成功！");
                UIComponent.Instance.Remove(UIType.UIRealName);
            }
        }
        public async ETVoid G2C_RealNameAuthentication()
        {
            G2C_RealNameAuthenticationResponse g2C_IdCardInspect = (G2C_RealNameAuthenticationResponse)await session.Call(new C2G_RealNameAuthenticationRequest
            {
                Name = input_Name,
                IdNum = input_Id,
                Account = account
            });
            if (g2C_IdCardInspect.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_IdCardInspect.Error.GetTipInfo());
                Log.DebugRed($"{g2C_IdCardInspect.Error}");
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "认证成功！");
                UIComponent.Instance.Remove(UIType.UIRealName);
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
        }

        public void OnVisible(object[] data)
        {
            if (data.Length > 0)
            {
                account = data[0].ToString();
            }
            if (data.Length > 1)
            {
                session = (Session)data[1];
            }
            if (data.Length > 2)
            {
                errorId = (int)data[2];
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
