using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [EventMethod(typeof(PropertyCreateBuilder), EventSystemType.INIT)]
    public class PropertyCreateBuilderEventOnInit : ITEventMethodOnInit<PropertyCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(PropertyCreateBuilder b_Component)
        {
            b_Component.OnInit();
        }
    }

    [EventMethod(typeof(PropertyCreateBuilder), EventSystemType.LOAD)]
    public class PropertyCreateBuilderEventOnLoad : ITEventMethodOnLoad<PropertyCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnLoad(PropertyCreateBuilder b_Component)
        {
            b_Component.CacheDatas.Clear();
            b_Component.OnInit();
        }
    }

    public static partial class PropertyCreateBuilderSystem
    {
        public static void OnInit(this PropertyCreateBuilder b_Component)
        {
            // 获取所有技能
            Type[] mTypes = Root.HotfixAssembly.GetTypes();
            for (int i = 0, len = mTypes.Length; i < len; i++)
            {
                Type type = mTypes[i];
                if (type.IsDefined(typeof(PropertyNumerialAttribute), false))
                {
                    object[] mAttributes = type.GetCustomAttributes(typeof(PropertyNumerialAttribute), false);
                    for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                    {
                        PropertyNumerialAttribute mEventMethod = mAttributes[j] as PropertyNumerialAttribute;
                        if (mEventMethod != null)
                        {
                            b_Component.CacheDatas[mEventMethod.BindId] = type;
                            break;
                        }
                    }
                }
            }
        }
    }
}