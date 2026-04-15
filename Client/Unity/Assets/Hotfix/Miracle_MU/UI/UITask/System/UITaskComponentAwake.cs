using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    [ObjectSystem]
    public class UITaskComponentAwake : AwakeSystem<UITaskComponent>
    {
        public override void Awake(UITaskComponent self)
        {
            Log.Info("打开任务面板");
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UITask));//关闭面板事件
            self.TaskDes = self.collector.GetText("TaskDes");//任务描述
            self.RequestText = self.collector.GetText("RequestText");//注意事项
            self.RewardText = self.collector.GetText("RewardText");//奖励
            self.GitTaskBtn = self.collector.GetButton("GitTaskBtn");///接受任务
            self.GiveUpBtn = self.collector.GetButton("GiveUpBtn");//放弃任务
            self.MoveBtn = self.collector.GetButton("MoveBtn");//移动到任务点
            self.GitRewardBtn = self.collector.GetButton("GitReWardBtn");//领取奖励
            self.TaskBg = self.collector.GetImage("TaskBg").gameObject;//领取奖励
            self.ActiveMap = self.collector.GetImage("ActiveMap").gameObject;//领取奖励
                                                                             //self.UIBeginnerGuideMove = self.collector.GetImage("UIBeginnerGuideMove").gameObject;
                                                                             //self.UIBeginnerGuideGet = self.collector.GetImage("UIBeginnerGuideGet").gameObject;
                                                                             // self.UIBeginnerGuideShouLie = self.collector.GetImage("UIBeginnerGuideShouLie").gameObject;
                                                                             //self.UIBeginnerGuideGetTask = self.collector.GetImage("UIBeginnerGuideGetTask").gameObject;
                                                                             //self.UIBeginnerGuideGetBuff = self.collector.GetImage("UIBeginnerGuideGetBuff").gameObject;
            self.InitMainTaskScrollView();
            self.InitTaskTypeTog();
            self.IsNpc = true;

            self.IsHaveXiaofeixie();

            //新手buffer
            self.collector.GetButton("BufferBtn").onClick.AddSingleListener(async () =>
            {
                G2C_XinShouBuffResponse g2C_XinShouBuff = (G2C_XinShouBuffResponse)await SessionComponent.Instance.Session.Call(new C2G_XinShouBuffRequest());
                if (g2C_XinShouBuff.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_XinShouBuff.Error.GetTipInfo());
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "您已经得到精灵的祝福，攻击力和防御力提高了");
                    UIMainComponent.Instance.ShowInfo("<color=#00A1FF>您已经得到精灵的祝福，攻击力和防御力提高了</color>");
                    //if (BeginnerGuideData.IsComplete(6))
                    //{
                    //    BeginnerGuideData.SetBeginnerGuide(6);
                    //    self.UIBeginnerGuideGet.SetActive(true);
                    //}
                }
            });

            ////新手引导
            //if (BeginnerGuideData.IsCompleteTrigger(50, 45))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(50);
            //    self.UIBeginnerGuideGet.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(2))
            //{
            //    UIMainComponent.Instance.SetMask(true);
            //    BeginnerGuideData.SetBeginnerGuide(2);
            //    self.UIBeginnerGuideMove.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(5))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(5);
            //    self.UIBeginnerGuideGetBuff.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(7))
            //{
            //    self.UIBeginnerGuideGet.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(12))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(12);
            //    self.UIBeginnerGuideGet.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(18))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(18);
            //    self.UIBeginnerGuideGet.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(27))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(27);
            //    self.UIBeginnerGuideMove.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(33))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(33);
            //    self.UIBeginnerGuideGet.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(36))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(36);
            //    self.UIBeginnerGuideMove.SetActive(true);
            //}
            //else if (BeginnerGuideData.IsComplete(40))
            //{
            //    BeginnerGuideData.SetBeginnerGuide(40);
            //    self.UIBeginnerGuideShouLie.SetActive(true);
            //}

        }
    }
}
