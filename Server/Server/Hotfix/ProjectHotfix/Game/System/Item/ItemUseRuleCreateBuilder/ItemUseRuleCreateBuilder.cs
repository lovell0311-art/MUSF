using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [EventMethod(typeof(ItemUseRuleCreateBuilder), EventSystemType.INIT)]
    public class ItemUseRuleCreateBuilderEventOnInit : ITEventMethodOnInit<ItemUseRuleCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(ItemUseRuleCreateBuilder b_Component)
        {
            b_Component.OnInit();
        }
    }

    [EventMethod(typeof(ItemUseRuleCreateBuilder), EventSystemType.LOAD)]
    public class ItemUseRuleCreateBuilderEventOnLoad : ITEventMethodOnLoad<ItemUseRuleCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnLoad(ItemUseRuleCreateBuilder b_Component)
        {
            b_Component.OnInit();
        }
    }

    public static partial class ItemUseRuleCreateBuilderSystem
    {
        public static void OnInit(this ItemUseRuleCreateBuilder b_Component)
        {
            b_Component.CacheDatas.Clear();
            Type[] mTypes = Root.HotfixAssembly.GetTypes();
            for (int i = 0, len = mTypes.Length; i < len; i++)
            {
                Type type = mTypes[i];
                if (type.IsDefined(typeof(ItemUseRuleAttribute), false))
                {
                    object[] mAttributes = type.GetCustomAttributes(typeof(ItemUseRuleAttribute), false);
                    for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                    {
                        ItemUseRuleAttribute mEventMethod = mAttributes[j] as ItemUseRuleAttribute;
                        if (mEventMethod != null)
                        {
                            if (b_Component.CacheDatas.ContainsKey(mEventMethod.ItemUseRuleName))
                            {
                                throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {b_Component.GetType().Name}\n   输出日志信息:已经存在对象的事件,{mEventMethod.ItemUseRuleName} 注册失败\n{Environment.StackTrace}\n\n");
                            }
                            b_Component.CacheDatas[mEventMethod.ItemUseRuleName] = type;
                            break;
                        }
                    }
                }
            }
        }
    }
}