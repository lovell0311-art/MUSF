using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod(typeof(GameTaskRewardCreateBuilder), EventSystemType.INIT)]
    public class GameTaskRewardCreateBuilderInitSystem : ITEventMethodOnInit<GameTaskRewardCreateBuilder>
    {
        public void OnInit(GameTaskRewardCreateBuilder self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(GameTaskRewardCreateBuilder), EventSystemType.LOAD)]
    public class GameTaskRewardCreateBuilderLoadSystem : ITEventMethodOnLoad<GameTaskRewardCreateBuilder>
    {
        public override void OnLoad(GameTaskRewardCreateBuilder self)
        {
            self.OnInit();
        }
    }

    public static class GameTaskRewardCreateBuilderSystem
    {
        public static void OnInit(this GameTaskRewardCreateBuilder self)
        {
            self.GameTaskActionDict.Clear();
            List<Type> types = Game.EventSystem.GetTypes(typeof(GameTaskRewardMethodAttribute));
            foreach (var type in types)
            {
                IGameTaskRewardHandler handler = Activator.CreateInstance(type) as IGameTaskRewardHandler;
                if (handler == null)
                {
                    throw new Exception($"任务自定义奖励 {type.Name} 需要继承 IGameTaskRewardHandler");
                }
                if(self.GameTaskActionDict.ContainsKey(type.Name))
                {
                    throw new Exception($"任务自定义奖励规则，命名重复 type.Name={type.Name}");
                }
                self.GameTaskActionDict.Add(type.Name, handler);
            }
        }

        /// <summary>
        /// 通过方法名获取处理自己的方法
        /// </summary>
        /// <param name="gameTask"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static IGameTaskRewardHandler GetHandlerByMethodName(this GameTaskRewardCreateBuilder self, string methodName)
        {
            if (self.GameTaskActionDict.TryGetValue(methodName, out IGameTaskRewardHandler handler))
            {
                return handler;
            }
            throw new KeyNotFoundException($"没有找到指定要奖励发放方法,methodName={methodName}");
        }

    }
}
