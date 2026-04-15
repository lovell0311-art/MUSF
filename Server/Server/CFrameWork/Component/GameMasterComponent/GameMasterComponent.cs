
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CustomFrameWork.Component
{
    [EventMethod(typeof(GameMasterComponent), EventSystemType.LOAD)]
    public class GameMasterComponentEventOnLoad : ITEventMethodOnLoad<GameMasterComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnLoad(GameMasterComponent b_Component)
        {
            b_Component.Awake();
        }
    }

    public class GameMasterCommandLineAttribute : BaseAttribute
    {
        public string GameMasterCommandLineName { get; }
        public GameMasterCommandLineAttribute(string b_GameMasterCommandLine)
        {
            this.GameMasterCommandLineName = b_GameMasterCommandLine;
        }
    }
    public abstract class C_GameMasterCommandLine<T, T1> : ADataContext
    {
        public virtual async Task Run(T b_Component, List<int> b_Parameter, T1 b_Parameter1)
        {

        }
    }

    public class GameMasterComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<string, Type> GMCommandLineDic;

        public override void Awake()
        {
            if (GMCommandLineDic != null)
            {
                if (GMCommandLineDic.Count > 0)
                {
                    GMCommandLineDic.Clear();
                }
                GMCommandLineDic = null;
            }
            GMCommandLineDic = new Dictionary<string, Type>();

            RegisterCommandLine();
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            if (GMCommandLineDic != null)
            {
                if (GMCommandLineDic.Count > 0)
                {
                    GMCommandLineDic.Clear();
                }
                GMCommandLineDic = null;
            }

            base.Dispose();
        }



        private void RegisterCommandLine()
        {
            if (GMCommandLineDic.Count > 0)
            {
                GMCommandLineDic.Clear();
            }

            Type[] types = Root.HotfixAssembly.GetTypes();
            for (int i = 0, len = types.Length; i < len; i++)
            {
                Type type = types[i];

                if (type.IsDefined(typeof(GameMasterCommandLineAttribute), false))
                {
                    object[] mAttributes = type.GetCustomAttributes(typeof(GameMasterCommandLineAttribute), false);
                    if (mAttributes.Length > 0)
                    {
                        GameMasterCommandLineAttribute mCommandLineAttribute = mAttributes[0] as GameMasterCommandLineAttribute;
                        string commandLineName = mCommandLineAttribute.GameMasterCommandLineName;
                        if (GMCommandLineDic.ContainsKey(commandLineName))
                        {
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:已经存在对象的网络包对象,{mCommandLineAttribute.GameMasterCommandLineName} 注册失败\n{Environment.StackTrace}\n\n");
                        }
                        GMCommandLineDic[commandLineName] = type;
                    }
                }
            }
        }
    }
}
