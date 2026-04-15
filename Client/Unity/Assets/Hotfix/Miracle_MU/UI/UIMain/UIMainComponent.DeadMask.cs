using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Threading;
using System.ComponentModel;

namespace ETHotfix
{
    /// <summary>
    /// 复活倒计时
    /// </summary>
    public partial class UIMainComponent
    {
        Text showTxt;
        GameObject deadMask;
        public CancellationTokenSource CancellationTokenSource;
        float waitTime = 10;
        public float waitTimenexttime = 0;

        public GameObject InSitu;

        public void Init_DeadMask() 
        {
            deadMask = ReferenceCollector_Main.GetImage("DeadMask").gameObject;
            ReferenceCollector collector = deadMask.GetReferenceCollector();
            showTxt = collector.GetText("deadTxt");
            InSitu = collector.GetImage("InSitu").gameObject;
            //使用原地复活特权
            collector.GetButton("UseBtn").onClick.AddSingleListener(async () => 
            {
                G2C_ResurrectionInSituResponse g2C_ResurrectionInSitu = (G2C_ResurrectionInSituResponse)await SessionComponent.Instance.Session.Call(new C2G_ResurrectionInSituRequest { });
                if (g2C_ResurrectionInSitu.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ResurrectionInSitu.Error.GetTipInfo());
                    Log.DebugRed($"g2C_ResurrectionInSitu:{g2C_ResurrectionInSitu.Error}");
                }
                else
                {
                    HideInSitu();
                    HideDeadMask();
                }
            });
            //安全区复活
            collector.GetButton("CancelBtn").onClick.AddSingleListener(async() => 
            {
                G2C_SafeAreaResurrectionResponse g2C_SafeArea = (G2C_SafeAreaResurrectionResponse)await SessionComponent.Instance.Session.Call(new C2G_SafeAreaResurrectionRequest { });
                if (g2C_SafeArea.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SafeArea.Error.GetTipInfo());
                    Log.DebugRed($"g2C_ResurrectionInSitu:{g2C_SafeArea.Error}");
                }
                else
                {
                    HideInSitu();
                }
            });
            HideDeadMask();
        }
        /// <summary>
        /// 显示原地复活 特权提示
        /// </summary>
        public void ShowInSitu() 
        {
            ShowDeadMask();
            InSitu.SetActive(true);
         
        }
        public void HideInSitu() 
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
            }
            InSitu.SetActive(false);
        }

        public void ShowDeadMask(float resurrectiontime=26) 
        {
            HideInSitu();//隐藏原地复活特权
            waitTime = 10;
            showTxt.text = $"{waitTime}秒后复活";
            //  WaitResurrection();
            waitTimenexttime = Time.time + 2;
            CountDownAction += WaitResuRrection;
            deadMask.SetActive(true);
        }
        public void HideDeadMask() 
        {
            deadMask.SetActive(false);
            HideInSitu();
            showTxt.text = $"{waitTime}秒后复活";
            
        }

        /// <summary>
        /// 复活倒计时
        /// </summary>
        public void WaitResuRrection()
        {
            
            if (Time.time > waitTimenexttime)
            {
                showTxt.text = $"{--waitTime}秒后复活";
                waitTimenexttime = Time.time + 1;
                if (waitTime <= 0)
                {
                    deadMask.SetActive(false);
                    waitTime = 10;
                    CountDownAction -= WaitResuRrection;
                }
            }
        }
        public async void WaitResurrection()
        {
            CancellationTokenSource = new CancellationTokenSource();
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                await System.Threading.Tasks.Task.Delay(1000);
                showTxt.text = $"{--waitTime}秒后复活";
                if (waitTime == 0)
                {
                    deadMask.SetActive(false);
                    //清空怪物
                   // UnitEntityComponent.Instance.ClearMonsters();
                    waitTime = 10;
                    CancellationTokenSource.Cancel();
                    CancellationTokenSource.Dispose();
                    CancellationTokenSource = null;
                }
            }
        }

    }
}
