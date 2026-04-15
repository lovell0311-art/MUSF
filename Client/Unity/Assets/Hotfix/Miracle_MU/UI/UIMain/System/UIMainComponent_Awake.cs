using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIMainComponent_Awake : AwakeSystem<UIMainComponent>
    {
        private static void SafeInit(UIMainComponent self, string step, Action action)
        {
            float startAt = Time.realtimeSinceStartup;
            LoginStageTrace.Append($"UIMain init start step={step}");
            try
            {
                action?.Invoke();
                LoginStageTrace.Append($"UIMain init finish step={step} dt={(Time.realtimeSinceStartup - startAt):0.000}");
            }
            catch (Exception e)
            {
                LoginStageTrace.Append($"UIMain init failed step={step} type={e.GetType().Name} message={e.Message}");
                Log.Error($"UIMain init failed step={step}: {e}");
            }
        }

        public override void Awake(UIMainComponent self)
        {
            LoginStageTrace.Append("UIMain awake begin");
            UIMainComponent.Instance = self;
            self.ReferenceCollector_Main = self.GetParent<UI>().GameObject.GetReferenceCollector();
            LoginStageTrace.Append($"UIMain awake collector-ready hasCollector={self.ReferenceCollector_Main != null}");

            SafeInit(self, "Init_PK", self.Init_PK);
            SafeInit(self, "Init_Buffer", self.Init_Buffer);
            SafeInit(self, "InitRocker", self.InitRocker);
            SafeInit(self, "InitMiniMap", self.InitMiniMap);
            SafeInit(self, "LoadRedDot", self.LoadRedDot);
            SafeInit(self, "Init_TopLeft", self.Init_TopLeft);
            SafeInit(self, "Init_Team", self.Init_Team);
            SafeInit(self, "Init_LeftCenter", self.Init_LeftCenter);
            SafeInit(self, "InitChangeSeverLine", self.InitChangeSeverLine);
            SafeInit(self, "Init_TopRight", self.Init_TopRight_Safe);
            SafeInit(self, "InitSynthesis", self.InitSynthesis);
            SafeInit(self, "Init_BottomCenter", self.Init_BottomCenter_Safe);
            SafeInit(self, "Init_Exp", self.Init_Exp);
            SafeInit(self, "Init_Medicine", self.Init_Medicine);
            SafeInit(self, "Init_Mount", self.Init_Mount);
            SafeInit(self, "Init_Chat", self.Init_Chat);
            SafeInit(self, "Init_Pet", self.Init_Pet);
            SafeInit(self, "Init_Skill", self.Init_Skill);
            SafeInit(self, "Init_Fuben", self.Init_Fuben);
            SafeInit(self, "Init_InfoMessage", self.Init_InfoMessage);
            SafeInit(self, "Init_Notice", self.Init_Notice);
            SafeInit(self, "Init_SiegeWarfareNotice", self.Init_SiegeWarfareNotice);
            SafeInit(self, "Init_DeadMask", self.Init_DeadMask);
            SafeInit(self, "Init_Task", self.Init_Task);
            SafeInit(self, "InitBeginnerGuide", self.InitBeginnerGuide);
            SafeInit(self, "SitDownInit", self.SitDownInit);
            SafeInit(self, "Init_HitText", self.Init_HitText);
            SafeInit(self, "InitLevelTopUp", self.InitLevelTopUp);
            SafeInit(self, "Init_HuntingLog", self.Init_HuntingLog);
            SafeInit(self, "InitIcon", self.InitIcon);
            SafeInit(self, "ScheduleMainVisualRepairPasses", self.ScheduleMainVisualRepairPasses);
            LoginStageTrace.Append("UIMain awake finish");
        }
    }
}
