using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[ObjectSystem]
	public class UIComponentAwakeSystem : AwakeSystem<UIComponent>
	{
		public override void Awake(UIComponent self)
		{
            self.Awake();

			Transform uiCameraTransform = Component.Global?.transform?.Find("UICamera");
			self.Camera = uiCameraTransform != null ? uiCameraTransform.gameObject : null;
			if (self.Camera == null)
			{
				Log.Error("#UIComponent# UICamera missing, fallback to overlay mode");
			}

            List<Type> types = Game.EventSystem.GetTypes();
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UIFactoryAttribute), false);
                if (attrs.Length == 0) continue;
                UIFactoryAttribute attribute = attrs[0] as UIFactoryAttribute;
                if (self.uis.ContainsKey(attribute.Type))
                {
                    Log.Debug("已经存在 同类UI Factory:" + attribute.Type);
                }

                object o = Activator.CreateInstance(type);
                IUIFactory factory = o as IUIFactory;
                if (factory == null)
                {
                    Log.Error($"{0.GetType().FullName} 没有继承 IUIFactory");
                    continue;
                }
                self.uiFactorys[attribute.Type] = factory;
            }
        }
	}
	
	/// <summary>
	/// 管理所有UI实体
	/// </summary>
	public class UIComponent: Component
	{
		public GameObject Camera;
        private static UIComponent instance;
        public static UIComponent Instance => instance;
		public Dictionary<string, UI> uis = new Dictionary<string, UI>();
        Dictionary<string, UI> otheruis = new Dictionary<string, UI>();
        /// <summary>
        /// UIFactory 字典
        /// </summary>
        public readonly Dictionary<string, IUIFactory> uiFactorys = new Dictionary<string, IUIFactory>();

        public void Awake() 
        {
            instance = this;
          //  SceneComponent.Instance.RemoveLoadingAction=()=> Remove(UIType.UISceneLoading);
        }
        public void Add(UI ui)
		{
            //设置指定相机渲染
            Canvas canvas = ui.GameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
				Camera uiCamera = this.Camera != null ? this.Camera.GetComponent<Camera>() : null;
				if (uiCamera != null)
				{
					canvas.renderMode = RenderMode.ScreenSpaceCamera;
					canvas.worldCamera = uiCamera;
				}
				else
				{
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					canvas.worldCamera = null;
					Log.Error($"#UIComponent# add ui fallback overlay type={ui.Name}");
				}
            }
			this.uis.Add(ui.Name, ui);
			//ui.Parent = this;
        }
        /// <summary>
        /// 删除UI面板
        /// </summary>
        /// <param name="name"></param>
		public void Remove(string name)
		{
			if (!this.uis.TryGetValue(name, out UI ui))
			{
				return;
			}
			LoginStageTrace.Append($"UIRemove type={name}");
			this.uis.Remove(name);
			ui.Dispose();
		}
        /// <summary>
        /// 删除除主面板所有UI面板,开启新手引导后使用
        /// </summary>
        /// <param name="name"></param>
		public void RemoveAll()
        {
            if (!Guidance_Define.IsBeginnerGuide) return;
            foreach (var item in uis)
            {
                if(item.Key != UIType.UIMainCanvas  && item.Key != UIType.UIHint)
                {
                    if(item.Value != null)
                    {
                        otheruis[item.Key] = item.Value;
                    }
                }
            }
            TimerComponent.Instance.RegisterTimeCallBack(10, remove);
            void remove()
            {
                foreach (var item in otheruis)
                {
                    if (item.Key == UIType.UIKnapsack)
                    {
                        UIKnapsackComponent.Instance.CloseKnapsack();
                    }
                    //else if (item.Key == UIType.UITask)
                    //{
                    //    uis[item.Key].GetComponent<UITaskComponent>().Close();
                    //}
                    else
                    {
                        uis[item.Key].Dispose();
                        uis.Remove(item.Key);
                    }

                }
                otheruis.Clear();
            }
        }
        /// <summary>
        /// 删除除主面板所有UI面板
        /// </summary>
        /// <param name="name"></param>
		public void RemoveAllNew()
        {
            foreach (var item in uis)
            {
                if (item.Key != UIType.UIMainCanvas && item.Key != UIType.UIHint)
                {
                    if (item.Value != null)
                    {
                        otheruis[item.Key] = item.Value;
                    }
                }
            }
            remove();
            void remove()
            {
                foreach (var item in otheruis)
                {
                    if (item.Key == UIType.UIKnapsack)
                    {
                        UIKnapsackComponent.Instance.CloseKnapsack();
                    }
                    //else if (item.Key == UIType.UITask)
                    //{
                    //    uis[item.Key].GetComponent<UITaskComponent>().Close();
                    //}
                    else
                    {
                        uis[item.Key].Dispose();
                        uis.Remove(item.Key);
                    }

                }
                otheruis.Clear();
            }
        }
        /// <summary>
        /// 获得UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns> 
        public UI Get(string name)
		{
			UI ui = null;
			this.uis.TryGetValue(name, out ui);
			return ui;
		}
        public UI VisibleUI(string type, params object[] @params)
        {
            try
            {
                LoginStageTrace.Append($"UIVisible request type={type} existing={uis.ContainsKey(type)} paramCount={(@params == null ? 0 : @params.Length)}");
                if (uis.ContainsKey(type))
                {
                    if (@params.Length == 0)
                    {
                        uis[type].Visible();
                    }
                    else
                    {
                        uis[type].Visible(@params);
                    }
                    LoginStageTrace.Append($"UIVisible reuse type={type} root={uis[type].GameObject?.name}");
                    return uis[type];
                }

                UI ui = uiFactorys[type].Create();
                Add(ui);
                if (@params.Length == 0)
                {
                    ui.Visible();
                }
                else
                {
                    ui.Visible(@params);
                }
                LoginStageTrace.Append($"UIVisible create type={type} root={ui.GameObject?.name}");
                return ui;
            }
            catch (Exception e)
            {
                Log.Error($"#UIVisible# type={type} error={e}");
                if (type != UIType.UIHint && this.uiFactorys.ContainsKey(UIType.UIHint))
                {
                    try
                    {
                        this.VisibleUI(UIType.UIHint, $"UI[{type}]加载失败");
                    }
                    catch
                    {
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 隐藏ui
        /// </summary>
        public void InVisibilityUI(string type)
        {
            if (uis.ContainsKey(type))
            {
                uis[type].InVisibility();
            }
            else
            {
                Log.Error($"当前设置不可见的UI面板不存在:{type}");
            }
        }
        public void Clean() 
        {
            foreach (var item in uis.Values)
            {
                item.Dispose();
            }
            uis.Clear();
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();

        }
    }
}
