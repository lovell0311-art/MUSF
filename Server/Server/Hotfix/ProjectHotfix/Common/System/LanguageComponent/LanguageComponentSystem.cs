
using System.Collections;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using System.Runtime.CompilerServices;

namespace ETHotfix
{

    [EventMethod(typeof(LanguageComponent), EventSystemType.INIT)]
    public class LanguageComponentEventOnInit : ITEventMethodOnInit<LanguageComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(LanguageComponent b_Component)
        {

        }
    }

    public static class LanguageComponentSystem
    {
        public static string GetMessage(this LanguageComponent b_Component, string b_Str)
        {
            return b_Str;
        }
        public static string GetMessage(this LanguageComponent b_Component, int b_StrId)
        {
            string mMessage = null;

            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mLogicErrorConfigDic = mReadConfigComponent.GetJson<Tips_LogicErrorConfigJson>().JsonDic;
            if (mLogicErrorConfigDic.TryGetValue(b_StrId, out var mLogicErrorConfig))
            {
                mMessage = mLogicErrorConfig.TipsDescribe;
            }
            else
            {
                var mCodeErrorConfigDic = mReadConfigComponent.GetJson<Tips_CodeErrorConfigJson>().JsonDic;
                if (mCodeErrorConfigDic.TryGetValue(b_StrId, out var mCodeErrorConfig))
                {
                    mMessage = mCodeErrorConfig.TipsDescribe;
                }
            }

            return mMessage;
        }
        /// <summary>
        /// 禁止使用id 10000 - 20000 | 100000 - 200000 | 995 | 997 | 小于0
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_StrId"></param>
        /// <returns></returns>
        public static int GetMessageId(this LanguageComponent b_Component, int b_StrId,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            Log.Debug(b_Component.GetMessage(b_StrId), callerLineNumber, callerMemberName, callerFilePath);

            return b_StrId;
        }
    }
}