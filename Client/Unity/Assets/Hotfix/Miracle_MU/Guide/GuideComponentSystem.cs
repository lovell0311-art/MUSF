using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ETHotfix
{
    public static class GuideComponentSystem
    {

        public static void Awake(this GuideComponent self)
        {
            Log.Debug("--------------------初始话引导");
        }

        public static void LoginOut(this GuideComponent self)
        {

        }


        public static void InitConfig(this GuideComponent self)
        {
            Log.Info("初始化配置--------------------------");
            self.Guide_AllConfigs = new List<Guide_AllConfig>();
            self.Guide_StepConfigs = new List<Guide_StepConfig>();
            IConfig[] guides;
            IConfig[] steps;
            try
            {
                guides = ConfigComponent.Instance.GetAll<Guide_AllConfig>();
                steps = ConfigComponent.Instance.GetAll<Guide_StepConfig>();
            }
            catch (Exception e)
            {
                Log.Error($"guide config disabled: {e}");
                return;
            }

            for (int i = 0; i < guides.Length; i++)
            {
                self.Guide_AllConfigs.Add(guides[i] as Guide_AllConfig);
            }

            for (int i = 0; i < steps.Length; i++)
            {
                self.Guide_StepConfigs.Add(steps[i] as Guide_StepConfig);
            }
        }

        //初始化数据-----
        public static void InitData(this GuideComponent self, long configid)
        {
            //RoleEntity roleEntity = UnitEntityComponent.Instance.LocalRole;
            //if (configid == 0)
            //{
            //    //获取当前任务
            //    self.CurGuideConfig = self.Guide_AllConfigs.Find(p => p.Id == 1001);
            //    //Log.Info("当前引导---------------------- " + self.CurGuideConfig.Id);
            //}
            //else
            //{
            //    self.ChangeTaskResetGuide(TaskDatas.MainTaskInfo);
            //}
            self.ChangeTaskResetGuide(TaskDatas.MainTaskInfo);

            Log.Debug(UnitEntityComponent.Instance.LocalRole.RoleName);

            OpenMailRequest().Coroutine();
        }
        public static async ETTask OpenMailRequest()
        {
            G2C_OpenMailResponse g2C_OpenMail = (G2C_OpenMailResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMailRequest() { });
            //Log.DebugBrown("邮件错误码" + g2C_OpenMail.Error + "::::数量" + g2C_OpenMail.MailList.Count);
            bool state = false;
            for (int i = 0, length = g2C_OpenMail.MailList.Count; i < length; i++)
            {
              //  Log.DebugBrown("邮件数据" + g2C_OpenMail.MailList[i].MailState + ":::" + g2C_OpenMail.MailList[i].ReceiveOrNot + ":::名字" + g2C_OpenMail.MailList[i].MailName + ":::附件" + g2C_OpenMail.MailList[i].MailEnclosure.count);
                if (g2C_OpenMail.MailList[i].MailState == 0|| g2C_OpenMail.MailList[i].ReceiveOrNot==0)
                {
                    if (g2C_OpenMail.MailList[i].MailEnclosure.count>0)
                    {
                        state = true;
                        break;
                    }
                    
                }
               
            }
            UIMainComponent.EmailRed.gameObject.SetActive(state);
        }
        public static void ChangeTaskResetGuide(this GuideComponent self, TaskInfo taskInfo)
        {
            //CameraInfo cameraInfo = new CameraInfo();
            //ETModel.Game.Scene.GetComponent<CameraFollowComponent>().curAngleH = cameraInfo.curAngleH;
            //ETModel.Game.Scene.GetComponent<CameraFollowComponent>().curAngleV = cameraInfo.curAngleV;
            //ETModel.Game.Scene.GetComponent<CameraFollowComponent>().distance = cameraInfo.distance;
           // Log.DebugBrown("cameraInfo.curAngleH" + cameraInfo.curAngleH + ":::cameraInfo.curAngleV" + cameraInfo.curAngleV + ":::" + cameraInfo.distance);
            int index = -1;
            Guide_AllConfig config = null;
            if(taskInfo == null)
            {
                Log.Info("未查找到配置数据 ChangeTaskResetGuide");
                return;
            }
            Log.Info($" -----------------------ID= {taskInfo.Id} TaskName={taskInfo.TaskName}  State={taskInfo.State}  TaskType={taskInfo.TaskType} ");
            if (self.Guide_AllConfigs == null)
            {
                self.InitConfig();
            }
            //查询列表id符合的引导
            for (int i = 0; i < self.Guide_AllConfigs.Count; i++)
            {
                if (self.Guide_AllConfigs[i].Value != "")
                {
                    string[] ids = self.Guide_AllConfigs[i].Value.Split(',');
                    for (int j = 0; j < ids.Length; j++)
                    {
                        if (int.Parse(ids[j]) == taskInfo.Id)
                        {
                            if (self.Guide_AllConfigs[i].OpType == 1 && taskInfo.State == 1)
                            {
                                config = self.Guide_AllConfigs[i];
                            }
                            else if (self.Guide_AllConfigs[i].OpType == 2 && taskInfo.State == 2)
                            {
                                config = self.Guide_AllConfigs[i];
                            }
                            if (config != null)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                }
            }

            if (index != -1)
            {
                if (taskInfo.TaskType == E_TaskType.MainTask) //当主线任务有变化的时候---可以直接重置引导id
                {
                    if (config.OpenGuideType == (int)E_TaskType.MainTask)
                    {
                        if (self.CurGuideConfig != null && config.Id == self.CurGuideConfig.Id)
                        {
                            Log.Info("---------------此id已经触发过了");
                            return;
                        }
                        self.CurGuideConfig = config;
                        self.LHSetBeginnerGuide((int)self.CurGuideConfig.Id);
                    }
                }
            }
            else
            {
                self.CurGuideConfig = null;
            }

          //  self.CheckIsShowGuide();

        }

        //检测当前是否需要引导
        public static void CheckIsShowGuide(this GuideComponent self, bool isC = false)
        {
            //Log.Info("CheckIsShowGuide  - 1 ");
            ////return;
            //if (self.CurGuideConfig == null)
            //{
            //    Log.Debug("当前职业不存在引导");
            //    return;
            //}
            //if (self.IsGuideIng && !isC)
            //{
            //    Log.Debug("当前正在引导中");
            //    return;
            //}
            ////return;
            ////获取配置表
            //Log.Info("CheckIsShowGuide  - 2 ");
            //if (UnitEntityComponent.Instance.LocalRole.RoleType > E_RoleType.GrowLancer)
            //    return;
            ////查看当前是否需要引导
            //if (UnitEntityComponent.Instance.LocalRole.Level != self.CurGuideConfig.Level || UnitEntityComponent.Instance.LocalRole.ClassLev != 0)
            //{
            //    return;
            //}
            //Log.Info("CheckIsShowGuide  - 3 ");

            ////return;
            ////查看当前引导页面是否存在
            //UI ui = UIComponent.Instance.Get(UIType.UIGuide);
            //self.IsGuideIng = true;
            //if (ui == null)
            //{
            //    ui = UIComponent.Instance.VisibleUI(UIType.UIGuide, self.CurGuideConfig);
            //    ui.GetComponent<UIGuideComponent>().Init(self.CurGuideConfig);
            //}
            //else
            //{
            //    ui.GetComponent<UIGuideComponent>().ContinueGuide();
            //}
        }


        public static void LHSetBeginnerGuide(this GuideComponent self, int configId)
        {
            Log.Info("LHSetBeginnerGuide  --- 存入引导进度 " + configId);
            //self.GuideSata = configId;
            self.SetBeginnerGuideStatus(configId).Coroutine();
        }

        private static async ETVoid SetBeginnerGuideStatus(this GuideComponent self, int id)
        {
            G2C_SetBeginnerGuideStatus c2G_SetBeginnerGuide = (G2C_SetBeginnerGuideStatus)await SessionComponent.Instance.Session.Call(new C2G_SetBeginnerGuideStatus()
            {
                Value = id
            });
            if (c2G_SetBeginnerGuide.Error == 0)
            {

            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{c2G_SetBeginnerGuide.Error.GetTipInfo()}");
            }
        }

        public static void ResetGuide(this GuideComponent self)
        {
            UIComponent.Instance.Remove(UIType.UIGuide);
            self.IsGuideIng = false;
            self.CheckIsShowGuide();
        }
    }
}
