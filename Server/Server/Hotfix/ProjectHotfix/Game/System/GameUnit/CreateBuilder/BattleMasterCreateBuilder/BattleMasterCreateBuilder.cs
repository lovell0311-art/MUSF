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
    [EventMethod(typeof(BattleMasterCreateBuilder), EventSystemType.INIT)]
    public class BattleMasterCreateBuilderEventOnInit : ITEventMethodOnInit<BattleMasterCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(BattleMasterCreateBuilder b_Component)
        {
            b_Component.OnInit();
        }
    }

    [EventMethod(typeof(BattleMasterCreateBuilder), EventSystemType.LOAD)]
    public class BattleMasterCreateBuilderEventOnLoad : ITEventMethodOnLoad<BattleMasterCreateBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnLoad(BattleMasterCreateBuilder b_Component)
        {
            b_Component.CacheDatas.Clear();
            b_Component.OnInit();
        }
    }

    public static partial class BattleMasterCreateBuilderSystem
    {
        public static void OnInit(this BattleMasterCreateBuilder b_Component)
        {
            // 获取所有技能
            Dictionary<long, Type> mCustomHeroSkillDic = new Dictionary<long, Type>();
            Type[] mTypes = Root.HotfixAssembly.GetTypes();
            for (int i = 0, len = mTypes.Length; i < len; i++)
            {
                Type type = mTypes[i];
                if (type.IsDefined(typeof(BattleMasterAttribute), false))
                {
                    object[] mAttributes = type.GetCustomAttributes(typeof(BattleMasterAttribute), false);
                    for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                    {
                        BattleMasterAttribute mEventMethod = mAttributes[j] as BattleMasterAttribute;
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
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_SpellConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }
            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_SwordsmanConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }
            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_ArcherConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }
            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_SpellswordConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }
            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_HolyteacherConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }
            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_SummonWarlockConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }

            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_ALLConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }
            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_CombatConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }

            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_DreamKnightConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
                    }
                }
            }
            {
                var mConfigs = mReadConfigComponent.GetJson<BattleMaster_CareerConfigJson>().JsonDic.Values.ToArray();
                for (int i = 0, len = mConfigs.Length; i < len; i++)
                {
                    var mConfig = mConfigs[i];

                    if (mCustomHeroSkillDic.TryGetValue(mConfig.Id, out Type mSkilType))
                    {
                        b_Component.CacheDatas[mConfig.Id] = (mSkilType, mConfig);
                    }
                    else
                    {
                        Log.Warning($"没有实现大师技能{mConfig.Id} {mConfig.Name}");
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
        public static C_BattleMaster CreateHeroMaster(this BattleMasterCreateBuilder b_Component, int b_SKillId, DBMasterData b_DBMasterData)
        {
            C_BattleMaster mResult = null;
            if (b_Component.CacheDatas.TryGetValue(b_SKillId, out (Type mType, C_ConfigInfo Config) mResultType))
            {
                mResult = Root.CreateBuilder.GetInstance<C_BattleMaster, int>(mResultType.mType, b_SKillId);

                mResult.SetConfig(mResultType.Config, b_DBMasterData);

                mResult.AfterAwake();
            }
            return mResult;
        }
    }
}