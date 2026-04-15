using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    [EventMethod(typeof(BackpackRouterRuleCreateBuilder), EventSystemType.INIT)]
    public class BackpackRouterRuleCreateBuilderAwakeSystem : ITEventMethodOnInit<BackpackRouterRuleCreateBuilder>
    {
        public void OnInit(BackpackRouterRuleCreateBuilder self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(BackpackRouterRuleCreateBuilder), EventSystemType.LOAD)]
    public class BackpackRouterRuleCreateBuilderLoadSystem : ITEventMethodOnLoad<BackpackRouterRuleCreateBuilder>
    {
        public override void OnLoad(BackpackRouterRuleCreateBuilder self)
        {
            self.OnInit();
        }
    }

    public static partial class BackpackRouterRuleCreateBuilderSystem
    {
        public static void OnInit(this BackpackRouterRuleCreateBuilder self)
        {
            self.RouterRuleDict.Clear();


            List<Type> types = Game.EventSystem.GetTypes(typeof(BackpackRouterAttribute));
            foreach(Type type in types)
            {
                object[] attributes = type.GetCustomAttributes(typeof(BackpackRouterAttribute), false);
                IBackpackRouterHandler handler = Activator.CreateInstance(type) as IBackpackRouterHandler;
                if (handler == null)
                {
                    throw new Exception($"背包物品路由规则 '{type.Name}' 需要继承 IBackpackRouterHandler");
                }
                for (int j = 0; j < attributes.Length; j++)
                {
                    BackpackRouterAttribute eventMethod = attributes[j] as BackpackRouterAttribute;
                    if (eventMethod != null)
                    {
                        if(!self.RouterRuleDict.TryAdd(eventMethod.ItemConfigId, handler))
                        {
                            throw new Exception($"背包物品路由规则 '{type.Name}' ItemConfigId={eventMethod.ItemConfigId} 重复");
                        }
                    }
                }
            }
        }


    }
}
