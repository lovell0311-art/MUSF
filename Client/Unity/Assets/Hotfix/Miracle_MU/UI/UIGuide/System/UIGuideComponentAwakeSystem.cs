using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Runtime.CompilerServices;
namespace ETHotfix
{


    [ObjectSystem]
    public partial class UIGuideComponentAwakeSystem : AwakeSystem<UIGuideComponent>
    {
        public override void Awake(UIGuideComponent self)
        {
            var collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            var mask = collector.GetImage("Mask");

            self.knapsackdelete = collector.GetImage("knapsackdelete").gameObject;
            self.knapsackwar = collector.GetImage("knapsackwar").gameObject;


            self.guidance = mask.GetComponent<UserGuidance>();
            self.guideIng = collector.GetImage("guideIng");
            self.guidance.SetCamera(CameraComponent.Instance.UICamera);
            self.guidance.SetCanvas(self.GetParent<UI>().GameObject.GetComponent<Canvas>());
            self.guidance.clickAction += self.Next;

            self.downdir = collector.GetGameObject("downdir");
            self.updir = collector.GetGameObject("updir");
        }
    }

    public static class UIGuideComponentSystem
    {
        public static void Init(this UIGuideComponent self, Guide_AllConfig config)
        {
            self.stepIndex = 0;
            if (config == null)
            {
                Log.Info("Init未查找到配置");
                UIComponent.Instance.Remove(UIType.UIGuide);
                return;
            }
            self.allConfig = config;
            string[] steps = config.Steps.Split(',');

            self.stepIds = new int[steps.Length];
            for (int i = 0; i < steps.Length; i++)
            {
                self.stepIds[i] = int.Parse(steps[i]);
            }
            self.RestartStepConfig();
            self.guideIng.gameObject.SetActive(false);

            Transform transform = ResourcesComponent.Instance.resourceRoot.Find(self.stepConfig.ClickTarget);
            if (transform)
            {
                Log.Info(transform.position.ToString());
                self.guidance.OnMaskRectShow(transform as RectTransform);
                self.updir.SetActive(self.stepConfig.Dir == 1);
                self.downdir.SetActive(self.stepConfig.Dir == 2);
                self.updir.transform.position = transform.position;
                self.downdir.transform.position = transform.position;
            }
            else
            {
                self.DalyUIOpen().Coroutine();
                Log.Info("未查找到节点---- " + self.stepConfig.ClickTarget);
            }
        }

        public static async ETTask DalyUIOpen(this UIGuideComponent self)
        {
            while (true)
            {
                if(self.stepConfig.OpType == 100 || self.stepConfig.OpType == 102)
                {
                    //UI ui = UIComponent.Instance.Get(UIType.UIKnapsackNew);
                    //if (ui!= null)
                    //{

                    //}
                    Transform transform = ResourcesComponent.Instance.resourceRoot.Find(self.stepConfig.ClickTarget);
                    if (transform)
                    {
                        Log.Info(transform.position.ToString());
                        self.guidance.OnMaskRectShow(transform as RectTransform);
                        self.updir.SetActive(self.stepConfig.Dir == 1);
                        self.downdir.SetActive(self.stepConfig.Dir == 2);
                        self.updir.transform.position = transform.position;
                        self.downdir.transform.position = transform.position;
                        break;
                    }
                    else
                    {
                        Log.Info("未查找到节点---- " + self.stepConfig.ClickTarget);
                    }
                }
                await TimerComponent.Instance.WaitAsync(1);
            }


            
        }

        public static void Trigger(this UIGuideComponent self)
        {
            Log.Info($"Trigger Name={self.stepConfig.Desc} GuideType={self.stepConfig.GuideType} OpType={self.stepConfig.OpType}");

            if (self.stepConfig.GuideType == GuideType.MainTask)
            {
                if (self.stepConfig.OpType == (int)E_TaskActionType.ReceiveXinShouBuff)
                {
                    //打开任务面板
                    UIComponent.Instance.VisibleUI(UIType.UITask, new object[] { TaskDatas.MainTaskInfo.TaskType, TaskDatas.MainTaskInfo.State });
                    UIComponent.Instance.Get(UIType.UITask).GetComponent<UITaskComponent>().IsNpc = false;
                    ////刷新当前任务面板
                    UIComponent.Instance.Get(UIType.UITask).GetComponent<UITaskComponent>().ShowCurTaskInfo(TaskDatas.MainTaskInfo);
                }
            }
        }

        public static void ContinueGuide(this UIGuideComponent self)
        {
            Log.Info("ContinueGuide   " + self.stepConfig.GuideType + " OpType=" + self.stepConfig.OpType);
            //查询当前任务步骤类型
            if(self.stepConfig.OpType == (int)E_TaskActionType.DiscardItem || self.stepConfig.OpType == (int)E_TaskActionType.WearEquip)
            {
                return;
            }
            if(self.allConfig.Level == 3)
            {
                return;
            }

            self.Trigger();
            self.Next();
        }

        public static void RestartStepConfig(this UIGuideComponent self)
        {
            self.stepConfig = GuideComponent.Instance.Guide_StepConfigs.Find(p => p.Id == self.stepIds[self.stepIndex]);
        }

        public static void Next(this UIGuideComponent self)
        {
            Log.Info("当前步骤 " + self.stepConfig.Id);
            //检测是否存在下一步
            self.stepIndex++;
            self.guideIng.gameObject.SetActive(false);

            //查看当前是否需要保存引导进度
            //if (self.stepConfig.SaveGuideId != 0)
            //{
            //    GuideComponent.Instance.LHSetBeginnerGuide(self.stepConfig.SaveGuideId);
            //}

            //查看当前打开类型
            self.Trigger();

            Log.Info("11111111111111  "+"当前记录的index"+self.stepIndex+"最大"+ self.stepIds.Length);
            if (self.stepIndex >= self.stepIds.Length)
            {
                //当前步骤结束
                Log.Info("未查找到下一步配置");
                //查看是否还存在引导--------------------------------
                //int next = self.allConfig.NextGuidId;
                //if (next != 0)
                //{
                //    Log.Info("存在下一步引导----继续执行引导" + next);
                //    //查看当前是否需要保存引导进度
                //    if (self.allConfig.NextGuidId != 0)
                //    {
                //        GuideComponent.Instance.LHSetBeginnerGuide(self.allConfig.NextGuidId);
                //    }
                //}
                //else
                //{
                //    UIComponent.Instance.Remove(UIType.UIGuide);
                //}
                Log.Info("结束当前引导");

                self.updir.SetActive(false);
                self.downdir.SetActive(false);
                if (self.allConfig.NextGuidId == -1)
                {
                    UIComponent.Instance.Remove(UIType.UIGuide);
                    GuideComponent.Instance.IsGuideIng = false;
                }
                else
                {
                    GuideComponent.Instance.ResetGuide();
                }
                
            }
            else
            {
                //Log.Info("2222222222222  ");
                self.RestartStepConfig();
               
                if (self.stepConfig == null)
                {
                    self.updir.SetActive(false);
                    self.downdir.SetActive(false);
                    return;
                }
                if (self.stepConfig.ClickTarget == "")
                {
                    self.guidance.SetForceTip();
                    self.guideIng.gameObject.SetActive(true);
                    self.updir.SetActive(false);
                    self.downdir.SetActive(false);
                }
                else
                {
                    try
                    {
                        Transform transform = ResourcesComponent.Instance.resourceRoot.Find(self.stepConfig.ClickTarget);
                        self.updir.SetActive(self.stepConfig.Dir == 1);
                        self.downdir.SetActive(self.stepConfig.Dir == 2);
                        if (transform)
                        {

                            self.updir.transform.position = transform.position;
                            self.downdir.transform.position = transform.position;
                            if (self.stepConfig.ClickType == 1)
                            {
                                Log.Info("点击事件 " + transform.name);
                                self.guidance.ResetMaskShow(transform as RectTransform);
                            }
                            else
                            {
                                if (self.stepConfig.GuideType == GuideType.Knaspack)
                                {
                                    if (self.stepConfig.OpType == (int)E_TaskActionType.DiscardItem)
                                    {
                                        var s = self.knapsackdelete.GetComponent<UserGuidance_2>();
                                        s.SetRect(transform as RectTransform);
                                        s.SetCamera(CameraComponent.Instance.UICamera);
                                        s.SetCanvas(self.GetParent<UI>().GameObject.GetComponent<Canvas>());
                                        self.guidance.gameObject.SetActive(false);
                                        s.OnMaskRectShow(transform as RectTransform);
                                        self.knapsackdelete.SetActive(true);
                                        //return;
                                    }
                                    else if (self.stepConfig.OpType == (int)E_TaskActionType.WearEquip)
                                    {
                                        var s = self.knapsackwar.GetComponent<UserGuidance_2>();
                                        s.SetRect(transform as RectTransform);
                                        s.SetCamera(CameraComponent.Instance.UICamera);
                                        s.SetCanvas(self.GetParent<UI>().GameObject.GetComponent<Canvas>());
                                        s.SetTargetRect(ResourcesComponent.Instance.resourceRoot.Find(self.stepConfig.DragEndTarget) as RectTransform);
                                        self.guidance.gameObject.SetActive(false);
                                        s.OnMaskRectShow(transform as RectTransform);

                                    }
                                }
                                else
                                {
                                    Log.Info("为查找到拖拽");
                                }
                            }
                            self.knapsackdelete.SetActive(self.stepConfig.ClickType == 2 && self.stepConfig.GuideType == GuideType.Knaspack && self.stepConfig.OpType == (int)E_TaskActionType.DiscardItem);
                            self.knapsackwar.SetActive(self.stepConfig.ClickType == 2 && self.stepConfig.GuideType == GuideType.Knaspack && self.stepConfig.OpType == (int)E_TaskActionType.WearEquip);
                            self.guidance.gameObject.SetActive(self.stepConfig.ClickType == 1);
                        }
                        else
                        {
                            Log.Info("未查询到目标节点");
                            self.DalyUIOpen().Coroutine();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Info("-- " + e.ToString());
                        UIComponent.Instance.VisibleUI(UIType.UIHint,"引导错误 "+ e.Message);
                        UIComponent.Instance.Remove(UIType.UIGuide);
                        GuideComponent.Instance.IsGuideIng = false;
                    }
                }
            }
        }
    }
}
