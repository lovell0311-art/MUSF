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
    [EventMethod(typeof(PetsSkillCreateBuilder), EventSystemType.INIT)]
    public class PetsSkillCreateBuilderEventOnInit : ITEventMethodOnInit<PetsSkillCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(PetsSkillCreateBuilder b_Component)
        {
            b_Component.OnInit();
        }
    }

    [EventMethod(typeof(PetsSkillCreateBuilder), EventSystemType.LOAD)]
    public class PetsSkillCreateBuilderEventOnLoad : ITEventMethodOnLoad<PetsSkillCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnLoad(PetsSkillCreateBuilder b_Component)
        {
            b_Component.CacheDatas.Clear();
            b_Component.OnInit();
        }
    }


    public static partial class PetsSkillCreateBuilderSystem
    {
        public static void OnInit(this PetsSkillCreateBuilder b_Component)
        {
            // 获取所有技能
            Dictionary<long, Type> mCustomHeroSkillDic = new Dictionary<long, Type>();
            Type[] mTypes = Root.HotfixAssembly.GetTypes();
            for (int i = 0, len = mTypes.Length; i < len; i++)
            {
                Type type = mTypes[i];
                if (type.IsDefined(typeof(HeroSkillAttribute), false))
                {
                    object[] mAttributes = type.GetCustomAttributes(typeof(HeroSkillAttribute), false);
                    for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                    {
                        HeroSkillAttribute mEventMethod = mAttributes[j] as HeroSkillAttribute;
                        if (mEventMethod != null)
                        {
                            mCustomHeroSkillDic[mEventMethod.BindId] = type;
                            break;
                        }
                    }
                }
            }


            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            {
                var mConfigs = mReadConfigComponent.GetJson<Pets_SkillConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }
        }

        /// <summary>
        /// 创建一个新的技能单位 整合配置
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_GameHero">初始化谁的技能</param>
        /// <param name="b_SKillId">技能id</param>
        /// <returns></returns>
        public static C_HeroSkillSource CreateHeroSKill(this PetsSkillCreateBuilder b_Component, int b_SKillId)
        {
            C_HeroSkillSource mResult = null;
            if (b_Component.CacheDatas.TryGetValue(b_SKillId, out (Type mType, C_ConfigInfo Config) mResultType))
            {
                mResult = Root.CreateBuilder.GetInstance<C_HeroSkillSource, int>(mResultType.mType, b_SKillId);

                mResult.SetConfig(mResultType.Config);

                mResult.AfterAwake();
            }
            return mResult;
        }
    }
}