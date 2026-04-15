using System;
using System.Collections.Generic;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
	[ObjectSystem]
	public class NetOuterComponentAwakeSystem : AwakeSystem<NetOuterComponent>
	{
		public override void Awake(NetOuterComponent self)
		{
			self.Awake(self.Protocol);
			self.InitMessageDispatcher();
		}
	}

	[ObjectSystem]
	public class NetOuterComponentAwake1System : AwakeSystem<NetOuterComponent, string>
	{
		public override void Awake(NetOuterComponent self, string address)
		{
			self.Awake(self.Protocol, address);
			self.InitMessageDispatcher();
		}
	}
	
	[ObjectSystem]
	public class NetOuterComponentLoadSystem : LoadSystem<NetOuterComponent>
	{
		public override void Load(NetOuterComponent self)
		{
			self.InitMessageDispatcher();
		}
	}
	
	[ObjectSystem]
	public class NetOuterComponentUpdateSystem : UpdateSystem<NetOuterComponent>
	{
		public override void Update(NetOuterComponent self)
		{
			self.Update();
		}
    }

    public static class NetOuterComponentSystem
    {
        public static void InitMessageDispatcher(this NetOuterComponent self)
        {
            self.MessageDispatcher = null;
            List<Type> types = Game.EventSystem.GetTypes(typeof(SessionMessageDispatcherAttribute));
            foreach (var type in types)
            {
                object[] attributes = type.GetCustomAttributes(typeof(SessionMessageDispatcherAttribute), false);
                for (int i = 0; i < attributes.Length; i++)
                {
                    SessionMessageDispatcherAttribute attribute = attributes[i] as SessionMessageDispatcherAttribute;
                    if (attribute == null) continue;

                    if (attribute.Type == SessionMessageDispatcherType.ServerOuter)
                    {
                        if (self.MessageDispatcher != null)
                        {
                            Log.Error($"{type.Name} 重复的属性类型: SessionMessageDispatcherType.ServerOuter");
                            continue;
                        }
                        self.MessageDispatcher = Activator.CreateInstance(type) as IMessageDispatcher;
                        if (self.MessageDispatcher == null)
                        {
                            Log.Error($"{type.Name} 没有继承 IMessageDispatcher");
                        }
                    }
                }
            }
            if (self.MessageDispatcher == null)
            {
                Log.Error($"没有找到属性类型: SessionMessageDispatcherType.ServerInner");
            }
        }
    }

}