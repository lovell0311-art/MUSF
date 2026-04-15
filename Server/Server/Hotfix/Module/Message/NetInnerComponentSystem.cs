using System;
using System.Collections.Generic;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
	[ObjectSystem]
	public class NetInnerComponentAwakeSystem : AwakeSystem<NetInnerComponent>
	{
		public override void Awake(NetInnerComponent self)
		{
			self.Awake();
		}
	}

	[ObjectSystem]
	public class NetInnerComponentAwake1System : AwakeSystem<NetInnerComponent, string>
	{
		public override void Awake(NetInnerComponent self, string a)
		{
			self.Awake(a);
		}
	}
	
	[ObjectSystem]
	public class NetInnerComponentLoadSystem : LoadSystem<NetInnerComponent>
	{
		public override void Load(NetInnerComponent self)
		{
			self.InitMessageDispatcher();
		}
	}

	[ObjectSystem]
	public class NetInnerComponentUpdateSystem : UpdateSystem<NetInnerComponent>
	{
		public override void Update(NetInnerComponent self)
		{
			self.Update();
		}
	}

	public static class NetInnerComponentHelper
	{
		public static void Awake(this NetInnerComponent self)
		{
			self.Awake(NetworkProtocol.TCP, Packet.PacketSizeLength4);
			self.InitMessageDispatcher();
			self.AppType = CustomFrameWork.Component.OptionComponent.Options.AppType;
		}

		public static void Awake(this NetInnerComponent self, string address)
		{
			self.Awake(NetworkProtocol.TCP, address, Packet.PacketSizeLength4);
			self.InitMessageDispatcher();
			self.AppType = CustomFrameWork.Component.OptionComponent.Options.AppType;
		}

		public static void InitMessageDispatcher(this NetInnerComponent self)
        {
			self.MessageDispatcher = null;
            List<Type> types = Game.EventSystem.GetTypes(typeof(SessionMessageDispatcherAttribute));
            foreach (var type in types)
            {
                object[] attributes = type.GetCustomAttributes(typeof(SessionMessageDispatcherAttribute), false);
				for(int i = 0; i < attributes.Length; i++)
                {
					SessionMessageDispatcherAttribute attribute = attributes[i] as SessionMessageDispatcherAttribute;
					if (attribute == null) continue;

					if(attribute.Type == SessionMessageDispatcherType.ServerInner)
                    {
						if(self.MessageDispatcher != null)
                        {
							Log.Error($"{type.Name} 重复的属性类型: SessionMessageDispatcherType.ServerInner");
							continue;
                        }
						self.MessageDispatcher = Activator.CreateInstance(type) as IMessageDispatcher;
						if(self.MessageDispatcher == null)
                        {
                            Log.Error($"{type.Name} 没有继承 IMessageDispatcher");
                        }
                    }
				}
            }
			if(self.MessageDispatcher == null)
            {
				Log.Error($"没有找到属性类型: SessionMessageDispatcherType.ServerInner");
            }
        }

		public static void Update(this NetInnerComponent self)
		{
			self.Update();
		}
	}
}