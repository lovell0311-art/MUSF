using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod(typeof(ItemCustomAttrMethodCreateBuilder), EventSystemType.INIT)]
    public class ItemCustomAttrMethodCreateBuilderInitSystem : ITEventMethodOnInit<ItemCustomAttrMethodCreateBuilder>
    {
        public void OnInit(ItemCustomAttrMethodCreateBuilder self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(ItemCustomAttrMethodCreateBuilder), EventSystemType.LOAD)]
    public class ItemCustomAttrMethodCreateBuilderLoadSystem : ITEventMethodOnLoad<ItemCustomAttrMethodCreateBuilder>
    {
        public override void OnLoad(ItemCustomAttrMethodCreateBuilder self)
        {
            self.OnInit();
        }
    }



    public static class ItemCustomAttrMethodCreateBuilderSystem
    {
        public static void OnInit(this ItemCustomAttrMethodCreateBuilder self)
        {
            self.MethodDict.Clear();
            List<Type> types = Game.EventSystem.GetTypes(typeof(ItemCustomAttrMethodAttribute));
            foreach (var type in types)
            {
                IItemCustomAttrMethodHandler handler = Activator.CreateInstance(type) as IItemCustomAttrMethodHandler;
                if (handler == null)
                {
                    Log.Error($"物品属性更新方法 '{type.Name}' 需要继承 IItemCustomAttrMethodHandler");
                }
                if (self.MethodDict.ContainsKey(type.Name))
                {
                    self.MethodDict[type.Name] = handler;
                }
                else
                {
                    self.MethodDict.Add(type.Name, handler);
                }
            }
        }
    }
}
