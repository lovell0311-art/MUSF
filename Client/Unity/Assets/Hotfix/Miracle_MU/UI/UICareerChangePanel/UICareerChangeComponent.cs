using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Text;
using System;
using ILRuntime.Runtime;

namespace ETHotfix
{
    [ObjectSystem]
    public class UICareerChangeComponentAwake : AwakeSystem<UICareerChangeComponent>
    {
        public override void Awake(UICareerChangeComponent self)
        {
            self.Awake();
        }
    }
    /// <summary>
    /// 转职
    /// </summary>
    public class UICareerChangeComponent : Component
    {
        ReferenceCollector collector;
        public Text title, taskName, taskDesInfo, taskMonstInfo, TaskNeedItems;
        public Button GitTaskBtn;
        RoleEntity roleEntity;
        TaskInfo CurtaskInfo;

        public void Awake()
        {
            roleEntity = UnitEntityComponent.Instance.LocalRole;

            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UICareerChangePanel));
            GitTaskBtn = collector.GetButton("GitTaskBtn");
            title = collector.GetText("title");
            taskName = collector.GetText("taskname");
            taskDesInfo = collector.GetText("taskInfo");
            taskMonstInfo = collector.GetText("taskMonstInfo");
            TaskNeedItems = collector.GetText("needItemInfo");



            if (TaskDatas.ChangeTaskInfo == null)
            {
                Log.DebugGreen("当前转职任务为空");
                CurtaskInfo = TaskDatas.GetCurTaskInfo(E_TaskType.CareerChangeTask);//获取默认第一个转职任务
            }
            else
            {

                if (TaskDatas.ChangeTaskInfo.State == 3)//当前任务 已经完成
                {
                    CurtaskInfo = TaskDatas.ChangeTaskInfo.GetNextTaskInfo();//获取下一个任务
                    TaskDatas.ChangeTaskInfo = CurtaskInfo;

                }
                else
                {
                    CurtaskInfo = TaskDatas.ChangeTaskInfo;
                }
            }

            InitTaskInfo(CurtaskInfo);
            GitTaskBtn.onClick.AddListener(async () =>
            {
                if (CurtaskInfo.State == 0)
                {
                    ///领取任务
                    if (TaskDatas.ChangeTaskInfo.ReqLevelMin > roleEntity.Property.GetProperValue(E_GameProperty.Level))
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足 无法领取转职任务");
                        return;
                    }

                    G2C_ReceiveTask g2C_ReceiveTask = (G2C_ReceiveTask)await SessionComponent.Instance.Session.Call(new C2G_ReceiveTask { TaskId = CurtaskInfo.Id.ToInt32() });
                    Log.DebugBrown("转职任务消息" + g2C_ReceiveTask.Error+":::id"+ CurtaskInfo.Id+"int"+ CurtaskInfo.Id.ToInt32());
                    if (g2C_ReceiveTask.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReceiveTask.Error.GetTipInfo());
                    }
                    else
                    {
                        CurtaskInfo.State = 1;
                        TaskDatas.ChangeTaskInfo = CurtaskInfo;
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "任务领取成功");
                        GitTaskBtn.GetComponentInChildren<Text>().text = "领取奖励";
                        UIMainComponent.Instance.SetTaskInfo(CurtaskInfo);
                    }
                }
                else if (CurtaskInfo.State == 1)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "任务还未 完成");
                }
                else if (CurtaskInfo.State == 2)
                {
                    G2C_ReceiveTaskReward g2C_ReceiveTaskReward = (G2C_ReceiveTaskReward)await SessionComponent.Instance.Session.Call(new C2G_ReceiveTaskReward
                    {
                        TaskId = CurtaskInfo.Id.ToInt32()
                    });
                    if (g2C_ReceiveTaskReward.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReceiveTaskReward.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "奖励领取完成");
                        TaskDatas.ChangeTaskInfo = CurtaskInfo.GetNextTaskInfo();
                        //  CurtaskInfo = CurtaskInfo.GetNextTaskInfo();
                        UIComponent.Instance.Remove(UIType.UICareerChangePanel);
                        // InitTaskInfo(CurtaskInfo);
                        // UIMainComponent.Instance.SetTaskInfo(CurtaskInfo);
                        UIMainComponent.Instance.GiveUpTask(CurtaskInfo);

                    }
                }
                else if (CurtaskInfo.State == 3)
                {

                }

            });
        }

        public void InitTaskInfo(TaskInfo taskInfo)
        {
            taskMonstInfo.text = null;
            // CurtaskInfo = TaskDatas.ChangeTaskInfo;
            if (taskInfo.State != 0) GitTaskBtn.GetComponentInChildren<Text>().text = "领取奖励";
            title.text = $"完成 {roleEntity.ClassLev + 1} 次转职任务";
            taskName.text = taskInfo.TaskName;
            var colorstr = this.roleEntity.Property.GetProperValue(E_GameProperty.Level) < taskInfo.ReqLevelMin ? Color.red : Color.white;
            taskDesInfo.text = /*taskInfo.TaskDes + */"\n" + $"接取等级:<color={colorstr}><b>{taskInfo.ReqLevelMin}</b></color>以上";
            TaskNeedItems.text = GetTaskItem();
            taskMonstInfo.text = "目标怪物：" + GetTaskMonster();
            string GetTaskMonster()
            {
                if (taskInfo.TaskDes == string.Empty) return null;
                StringBuilder stringBuilder = new StringBuilder();
                List<int> list = new List<int>();
                string taskNew = taskInfo.TaskDes.Replace("[","");
                taskNew = taskNew.Replace("]","");
                string[] taskList = taskNew.Split(',');
                for (int i = 0,length = taskList.Length; i < length; i++)
                {
                    list.Add(taskList[i].ToInt32());
                }
                for (int i = 0, length = list.Count; i < length; i++)
                {
                    EnemyConfig_InfoConfig config_InfoConfig = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(list[i]);
                    if (i == length - 1)
                    {
                        stringBuilder.Append(config_InfoConfig.Name);
                    }
                    else
                    {
                        stringBuilder.Append(config_InfoConfig.Name + ", ");
                    }
                }
                return stringBuilder.ToString();
            }

            string GetTaskItem()
            {
                string[] items = taskInfo.MonsterName.Split(new char[] { '\\', '、' }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0, length = taskInfo.TaskTargetCount.Count; i < length; i++)
                {
                    string color = ColorTools.GetColorHtmlString(Color.red);
                    if (taskInfo.TaskProgress != null)
                    {
                        color = ColorTools.GetColorHtmlString(taskInfo.TaskProgress[i] >= taskInfo.TaskTargetCount[i] ? Color.yellow : Color.red);
                        stringBuilder.Append($"{items[i]}  <color={color}>{taskInfo.TaskProgress[i]}</color>/{taskInfo.TaskTargetCount[i]}\n");
                    }
                    else
                    {
                        stringBuilder.Append($"{items[i]}  <color={color}>{0}</color>/{taskInfo.TaskTargetCount[i]}\n");
                    }
                }
                stringBuilder.Append($"金币：{taskInfo.ReqCoin}");
                //int index = stringBuilder.ToString().LastIndexOf("、");
                //  string st = stringBuilder.ToString().Substring(0, index);
                return stringBuilder.ToString();
            }
        }
    }

}