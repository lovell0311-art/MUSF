using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiAwakeSystem : AwakeSystem<UI, string, GameObject>
	{
		public override void Awake(UI self, string name, GameObject gameObject)
		{
			self.Awake(name, gameObject);
		}
	}
	
	[HideInHierarchy]
	public sealed class UI: Entity
	{
		public string Name { get; private set; }

		
		public Dictionary<string, UI> children = new Dictionary<string, UI>();

		private Dictionary<string, Component> statuses = new Dictionary<string, Component>();
		public void Awake(string name, GameObject gameObject)
		{
			
			this.children.Clear();
			gameObject.AddComponent<ComponentView>().Component = this;
		//	gameObject.layer = LayerMask.NameToLayer(LayerNames.UI);
			this.Name = name;
			this.GameObject = gameObject;

			
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			base.Dispose();

			foreach (UI ui in this.children.Values)
			{
				ui.Dispose();
			}
			
			UnityEngine.Object.Destroy(GameObject);
			AssetBundleComponent.Instance.UnloadBundle(GameObject.name.StringToAB());
			//释放ab资源
			//Log.DebugGreen($"销毁UI:{this.GameObject.name}");
			children.Clear();
			statuses.Clear();
		}

		public void SetAsFirstSibling()
		{
			this.GameObject.transform.SetAsFirstSibling();
		}

		public void Add(UI ui)
		{
			this.children.Add(ui.Name, ui);
			ui.Parent = this;
		}

		public void Remove(string name)
		{
			UI ui;
			if (!this.children.TryGetValue(name, out ui))
			{
				return;
			}
			this.children.Remove(name);
			ui.Dispose();
		}

		public UI Get(string name)
		{
			UI child;
			if (this.children.TryGetValue(name, out child))
			{
				return child;
			}
			GameObject childGameObject = this.GameObject.transform.Find(name)?.gameObject;
			if (childGameObject == null)
			{
				return null;
			}
			child = ComponentFactory.Create<UI, string, GameObject>(name, childGameObject);
			this.Add(child);
			return child;
		}
		public void Visible()
		{
			this.GameObject.transform.SetAsFirstSibling();
			this.GameObject.SetActive(true);
			foreach (var item in statuses.Values)
			{
				(item as IUGUIStatus).OnVisible();
			}
		}
		public void Visible(params object[] para)
		{
			this.GameObject.transform.SetAsLastSibling();
			this.GameObject.SetActive(true);
			foreach (var item in statuses.Values)
			{
				(item as IUGUIStatus).OnVisible(para);
			}
		}
		public void InVisibility()
		{
			this.GameObject.SetActive(false);
			foreach (var item in statuses.Values)
			{
				(item as IUGUIStatus).OnInVisibility();
			}
		}

        public override Component AddComponent(Component component)
        {
			Component component1=base.AddComponent(component);
			if (component1 is IUGUIStatus)
			{
				statuses[typeof(Component).ToString()] = component1;
			 }
			return component;
        }

        public override Component AddComponent(Type type)
        {
          Component component=base.AddComponent(type);
			if (component is IUGUIStatus) 
			{
				statuses[type.ToString()] = component;
			}
			return component;
        }

        public override K AddComponent<K>()
        {
            K component= base.AddComponent<K>();
			if (component is IUGUIStatus)
			{
				statuses[typeof(K).ToString()] = component;
			 }
			return component;
        }

        public override K AddComponent<K, P1>(P1 p1)
        {
			K component = base.AddComponent<K, P1>(p1);
			if (component is IUGUIStatus)
			{
				statuses[typeof(K).ToString()] = component;
			}
			return component;
        }

        public override K AddComponent<K, P1, P2>(P1 p1, P2 p2)
        {
			K component = base.AddComponent<K, P1, P2>(p1, p2);
			if (component is IUGUIStatus)
			{
				statuses[typeof(K).ToString()] = component;
			}
			return component;
        }

        public override K AddComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3)
        {
			K component = base.AddComponent<K, P1, P2, P3>(p1, p2, p3);
			if (component is IUGUIStatus)
			{
				statuses[typeof(K).ToString()] = component;
			}
			return component;
        }

        public override void RemoveComponent<K>()
        {
            base.RemoveComponent<K>();
			string type = typeof(K).ToString();
			if (statuses.ContainsKey(type))
			{
				statuses.Remove(type);
			}
        }

        public override void RemoveComponent(Type type)
        {
            base.RemoveComponent(type);
			string type_ = type.ToString();
			if (statuses.ContainsKey(type_))
			{
				statuses.Remove(type_);
			}
        }
    }
}