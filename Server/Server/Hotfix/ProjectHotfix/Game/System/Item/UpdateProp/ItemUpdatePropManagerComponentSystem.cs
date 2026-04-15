using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [EventMethod(typeof(ItemUpdatePropManagerComponent), EventSystemType.INIT)]
    public class ItemUpdatePropManagerComponentAwakeSystem : ITEventMethodOnInit<ItemUpdatePropManagerComponent>
    {
        public void OnInit(ItemUpdatePropManagerComponent self)
        {
            self.Load();
        }
    }

    [EventMethod(typeof(ItemUpdatePropManagerComponent), EventSystemType.LOAD)]
    public class ItemUpdatePropManagerComponentLoadSystem : ITEventMethodOnLoad<ItemUpdatePropManagerComponent>
    {
        public override void OnLoad(ItemUpdatePropManagerComponent self)
        {
            self.Load();
        }
    }



    public static class ItemUpdatePropManagerComponentSystem
    {
        public static void Load(this ItemUpdatePropManagerComponent self)
        {
            self.UpdateMethodDict.Clear();
            Log.Debug("=========物品属性更新方法=========");
            List<Type> types = Game.EventSystem.GetTypes(typeof(ItemUpdatePropAttribute));
            foreach(var type in types)
            {
                Log.Debug($"Name:{type.Name}");
                IItemUpdatePropHandler handler = Activator.CreateInstance(type) as IItemUpdatePropHandler;
                if(handler == null)
                {
                    Log.Error($"物品属性更新方法 '{type.Name}' 需要继承 ItemUpdatePropMethod");
                }
                self.AddHandler(type.Name, handler);
            }
            self.DefaultHandler = new ItemUpdateProp.Default();
            Log.Debug("=========End=========");
        }

        public static void AddHandler(this ItemUpdatePropManagerComponent self,string key, IItemUpdatePropHandler handler)
        {
            if (self.UpdateMethodDict.ContainsKey(key))
            {
                self.UpdateMethodDict[key] = handler;
            }
            else
            {
                self.UpdateMethodDict.Add(key, handler);
            }
        }

        public static IItemUpdatePropHandler GetHandler(this ItemUpdatePropManagerComponent self,string key)
        {
            if(self.UpdateMethodDict.TryGetValue(key,out var handler))
            {
                return handler;
            }
            return null;
        }
    }
}
