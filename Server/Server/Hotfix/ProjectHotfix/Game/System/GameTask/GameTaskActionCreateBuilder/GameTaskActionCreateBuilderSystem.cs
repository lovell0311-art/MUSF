using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod(typeof(GameTaskActionCreateBuilder), EventSystemType.INIT)]
    public class GameTaskActionCreateBuilderInitSystem : ITEventMethodOnInit<GameTaskActionCreateBuilder>
    {
        public void OnInit(GameTaskActionCreateBuilder self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(GameTaskActionCreateBuilder), EventSystemType.LOAD)]
    public class GameTaskActionCreateBuilderLoadSystem : ITEventMethodOnLoad<GameTaskActionCreateBuilder>
    {
        public override void OnLoad(GameTaskActionCreateBuilder self)
        {
            self.OnInit();
        }
    }

    public static class GameTaskActionCreateBuilderSystem
    {
        public static void OnInit(this GameTaskActionCreateBuilder self)
        {
            self.GameTaskActionDict.Clear();
            List<Type> types = Game.EventSystem.GetTypes(typeof(GameTaskActionMethodAttribute));
            foreach (var type in types)
            {
                IGameTaskActionHandler handler = Activator.CreateInstance(type) as IGameTaskActionHandler;
                if (handler == null)
                {
                    throw new Exception($"任务行为动作 {type.Name} 需要继承 IGameTaskActionHandler");
                }
                object[] attrs = type.GetCustomAttributes(typeof(GameTaskActionMethodAttribute), false);
                foreach (object attr in attrs)
                {
                    GameTaskActionMethodAttribute actionMethod = attr as GameTaskActionMethodAttribute;
                    if(actionMethod != null)
                    {
                        if (self.GameTaskActionDict.ContainsKey(actionMethod.ActionType))
                        {
                            throw new Exception($"任务行为类型 {actionMethod.ActionType} 以注册，不能重复注册 {type.Name}");
                        }
                        else
                        {
                            self.GameTaskActionDict.Add(actionMethod.ActionType, handler);
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// 通过任务获取处理自己的方法
        /// </summary>
        /// <param name="gameTask"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static IGameTaskActionHandler GetHandlerByGameTask(this GameTaskActionCreateBuilder self,GameTask gameTask)
        {
            if(self.GameTaskActionDict.TryGetValue(gameTask.Config.TaskActionType, out IGameTaskActionHandler handler))
            {
                return handler;
            }
            throw new KeyNotFoundException($"没有找到 {gameTask.Config.TaskActionType} 处理方法，taskId = {gameTask.Config.ConfigId}");
        }

    }
}
