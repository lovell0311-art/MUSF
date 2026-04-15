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
    unsafe class ETModel_Init_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.Init);

            field = type.GetField("instance", flag);
            app.RegisterCLRFieldGetter(field, get_instance_0);
            app.RegisterCLRFieldSetter(field, set_instance_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_instance_0, AssignFromStack_instance_0);
            field = type.GetField("e_SDK", flag);
            app.RegisterCLRFieldGetter(field, get_e_SDK_1);
            app.RegisterCLRFieldSetter(field, set_e_SDK_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_e_SDK_1, AssignFromStack_e_SDK_1);
            field = type.GetField("agentStr", flag);
            app.RegisterCLRFieldGetter(field, get_agentStr_2);
            app.RegisterCLRFieldSetter(field, set_agentStr_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_agentStr_2, AssignFromStack_agentStr_2);


        }



        static object get_instance_0(ref object o)
        {
            return ETModel.Init.instance;
        }

        static StackObject* CopyToStack_instance_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ETModel.Init.instance;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_instance_0(ref object o, object v)
        {
            ETModel.Init.instance = (ETModel.Init)v;
        }

        static StackObject* AssignFromStack_instance_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ETModel.Init @instance = (ETModel.Init)typeof(ETModel.Init).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ETModel.Init.instance = @instance;
            return ptr_of_this_method;
        }

        static object get_e_SDK_1(ref object o)
        {
            return ((ETModel.Init)o).e_SDK;
        }

        static StackObject* CopyToStack_e_SDK_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.Init)o).e_SDK;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_e_SDK_1(ref object o, object v)
        {
            ((ETModel.Init)o).e_SDK = (ETModel.E_SDK)v;
        }

        static StackObject* AssignFromStack_e_SDK_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ETModel.E_SDK @e_SDK = (ETModel.E_SDK)typeof(ETModel.E_SDK).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((ETModel.Init)o).e_SDK = @e_SDK;
            return ptr_of_this_method;
        }

        static object get_agentStr_2(ref object o)
        {
            return ((ETModel.Init)o).agentStr;
        }

        static StackObject* CopyToStack_agentStr_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.Init)o).agentStr;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_agentStr_2(ref object o, object v)
        {
            ((ETModel.Init)o).agentStr = (System.String)v;
        }

        static StackObject* AssignFromStack_agentStr_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @agentStr = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.Init)o).agentStr = @agentStr;
            return ptr_of_this_method;
        }



    }
}
