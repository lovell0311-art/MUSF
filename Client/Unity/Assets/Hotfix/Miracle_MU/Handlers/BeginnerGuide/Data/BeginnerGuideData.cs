using ETModel;

namespace ETHotfix
{
    public static class BeginnerGuideData
    {
        public static long BeginnerGuideSata;
        public static bool BeginnerGuideCountTime = false;
        public static bool BeginnerIsOver = false;
        /// <summary>
        /// 当前任务是否可以做---强制
        /// </summary>
        /// <param name="configId">引导ID</param>
        /// <returns></returns>
        public static bool IsComplete(int configId)
        {
            return false;
            if (configId > 6) return false;
          //  Log.DebugGreen($"1引导状态->{BeginnerGuideSata} 比较->{(BeginnerGuideSata & (1 << configId)) == (1 << configId)}");
            //是否开启了新手引导
            if (!Guidance_Define.IsBeginnerGuide) return false;
            if (!((BeginnerGuideSata & ((long)1 << configId)) == ((long)1 << configId)))
            {
                if(configId - 1 > 0)
                {
                    int id = configId - 1;
                   // Log.DebugGreen($"2引导状态->{BeginnerGuideSata} 比较->{(BeginnerGuideSata & ((long)1 << id)) == ((long)1 << id)}");
                    if ((BeginnerGuideSata & ((long)1 << id)) == ((long)1 << id))
                    {
                        return true;
                    }
                }
                else
                {
                    int id = configId + 1;
                  //  Log.DebugGreen($"3引导状态->{BeginnerGuideSata} 比较->{(BeginnerGuideSata & ((long)1 << id)) == ((long)1 << id)}");
                    if (!((BeginnerGuideSata & ((long)1 << id)) == ((long)1 << id)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 当前任务是否可以做---触发
        /// 100-150触发
        /// </summary>
        /// <param name="configId">引导ID</param>
        /// <returns></returns>
        public static bool IsCompleteTrigger(int configId,int beginnerId)
        {
            return false;
            //是否开启了新手引导
            if (!Guidance_Define.IsBeginnerGuide) return false;
            if (!((BeginnerGuideSata & ((long)1 << configId)) == ((long)1 << configId)))
            {
                if (configId == beginnerId)
                {
                    int id = configId + 1;
                    if (!((BeginnerGuideSata & ((long)1 << id)) == ((long)1 << id)))
                    {
                        return true;
                    }
                    return true;
                }
                else if(configId > beginnerId)
                {
                    int id = configId - 1;
                    if (((BeginnerGuideSata & ((long)1 << id)) == ((long)1 << id)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="configId"></param>
        public static void SetBeginnerGuide(int configId)
        {
            BeginnerGuideSata |= ((long)1 << configId);
           
            if (configId == 3 || configId == 6 || configId == 7 || configId == 10 || configId == 13 || configId == 16 || configId == 19 || 
                configId == 23 || /*configId == 28 || */configId == 31 || configId == 34 || configId == 38 || configId == 42 || configId == 45||
                configId == 48 || configId == 51 || configId == 53 || configId == 56 || configId == 58 || configId == 61)
            {
                SetBeginnerGuideStatus(configId).Coroutine();
            }

        }
        private static async ETVoid SetBeginnerGuideStatus(int id)
        {
            G2C_SetBeginnerGuideStatus c2G_SetBeginnerGuide = (G2C_SetBeginnerGuideStatus)await SessionComponent.Instance.Session.Call(new C2G_SetBeginnerGuideStatus()
            {
                Value = BeginnerGuideSata
            });
            if(c2G_SetBeginnerGuide.Error == 0)
            {
                
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,$"{c2G_SetBeginnerGuide.Error.GetTipInfo()}");
            }
        }
    }
}

