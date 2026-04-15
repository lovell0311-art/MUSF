using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class ETModel_AgentTool_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.AgentTool);

            field = type.GetField("agentstr", flag);
            app.RegisterCLRFieldGetter(field, get_agentstr_0);
            app.RegisterCLRFieldSetter(field, set_agentstr_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_agentstr_0, AssignFromStack_agentstr_0);


        }



        static object get_agentstr_0(ref object o)
        {
            return ETModel.AgentTool.agentstr;
        }

        static StackObject* CopyToStack_agentstr_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ETModel.AgentTool.agentstr;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_agentstr_0(ref object o, object v)
        {
            ETModel.AgentTool.agentstr = (System.String)v;
        }

        static StackObject* AssignFromStack_agentstr_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @agentstr = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ETModel.AgentTool.agentstr = @agentstr;
            return ptr_of_this_method;
        }



    }
}
