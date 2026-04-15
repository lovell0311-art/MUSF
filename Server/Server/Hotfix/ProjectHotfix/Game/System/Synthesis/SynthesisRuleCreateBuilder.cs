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
    [EventMethod(typeof(SynthesisRuleCreateBuilder), EventSystemType.INIT)]
    public class SynthesisRuleCreateBuilderEventOnInit : ITEventMethodOnInit<SynthesisRuleCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(SynthesisRuleCreateBuilder b_Component)
        {
            b_Component.OnInit();
        }
    }

    [EventMethod(typeof(SynthesisRuleCreateBuilder), EventSystemType.LOAD)]
    public class SynthesisRuleCreateBuilderEventOnLoad : ITEventMethodOnLoad<SynthesisRuleCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnLoad(SynthesisRuleCreateBuilder b_Component)
        {
            b_Component.OnInit();
        }
    }


    public static partial class SynthesisRuleCreateBuilderSystem
    {
        public static void OnInit(this SynthesisRuleCreateBuilder b_Component)
        {
            b_Component.CacheDatas.Clear();
            Type[] mTypes = Root.HotfixAssembly.GetTypes();
            for (int i = 0, len = mTypes.Length; i < len; i++)
            {
                Type type = mTypes[i];
                if (type.IsDefined(typeof(SynthesisRuleAttribute), false))
                {
                    object[] mAttributes = type.GetCustomAttributes(typeof(SynthesisRuleAttribute), false);
                    for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                    {
                        SynthesisRuleAttribute mEventMethod = mAttributes[j] as SynthesisRuleAttribute;
                        if (mEventMethod != null)
                        {
                            if (b_Component.CacheDatas.ContainsKey(mEventMethod.SynthesisRuleName))
                            {
                                throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {b_Component.GetType().Name}\n   输出日志信息:已经存在对象的事件,{mEventMethod.SynthesisRuleName} 注册失败\n{Environment.StackTrace}\n\n");
                            }
                            b_Component.CacheDatas[mEventMethod.SynthesisRuleName] = type;
                            Log.Debug("=============注册合成方法" + mEventMethod.SynthesisRuleName);
                            break;
                        }
                    }
                }
            }
            //Log.Debug("++++++++++++++++验证方法");
            //var mSynthesisRuleCreateBuilder = Root.MainFactory.GetCustomComponent<SynthesisRuleCreateBuilder>();
            //if (mSynthesisRuleCreateBuilder.CacheDatas.TryGetValue("GuoShiSynthesis", out var mSynthesisRuleType) == false)
            //{
            //    Log.Debug("++++++++++++++++找不到方法");
            //    return;
            //}
            //Log.Debug("进入合成执行方法");
            //var mSynthesisRule = Root.CreateBuilder.GetInstance<C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis>>(mSynthesisRuleType);
            //mSynthesisRule.Run(null,null,null,null);
            //mSynthesisRule.Dispose();
        }
    }
}