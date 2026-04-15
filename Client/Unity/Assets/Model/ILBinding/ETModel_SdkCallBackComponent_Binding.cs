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
    unsafe class ETModel_SdkCallBackComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.SdkCallBackComponent);

            field = type.GetField("Instance", flag);
            app.RegisterCLRFieldGetter(field, get_Instance_0);
            app.RegisterCLRFieldSetter(field, set_Instance_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Instance_0, AssignFromStack_Instance_0);
            field = type.GetField("sdkUtility", flag);
            app.RegisterCLRFieldGetter(field, get_sdkUtility_1);
            app.RegisterCLRFieldSetter(field, set_sdkUtility_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_sdkUtility_1, AssignFromStack_sdkUtility_1);
            field = type.GetField("LogoutCallBack", flag);
            app.RegisterCLRFieldGetter(field, get_LogoutCallBack_2);
            app.RegisterCLRFieldSetter(field, set_LogoutCallBack_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_LogoutCallBack_2, AssignFromStack_LogoutCallBack_2);
            field = type.GetField("LoginCallBack", flag);
            app.RegisterCLRFieldGetter(field, get_LoginCallBack_3);
            app.RegisterCLRFieldSetter(field, set_LoginCallBack_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_LoginCallBack_3, AssignFromStack_LoginCallBack_3);


        }



        static object get_Instance_0(ref object o)
        {
            return ETModel.SdkCallBackComponent.Instance;
        }

        static StackObject* CopyToStack_Instance_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ETModel.SdkCallBackComponent.Instance;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Instance_0(ref object o, object v)
        {
            ETModel.SdkCallBackComponent.Instance = (ETModel.SdkCallBackComponent)v;
        }

        static StackObject* AssignFromStack_Instance_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ETModel.SdkCallBackComponent @Instance = (ETModel.SdkCallBackComponent)typeof(ETModel.SdkCallBackComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ETModel.SdkCallBackComponent.Instance = @Instance;
            return ptr_of_this_method;
        }

        static object get_sdkUtility_1(ref object o)
        {
            return ((ETModel.SdkCallBackComponent)o).sdkUtility;
        }

        static StackObject* CopyToStack_sdkUtility_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.SdkCallBackComponent)o).sdkUtility;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_sdkUtility_1(ref object o, object v)
        {
            ((ETModel.SdkCallBackComponent)o).sdkUtility = (ETModel.SdkUtility)v;
        }

        static StackObject* AssignFromStack_sdkUtility_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ETModel.SdkUtility @sdkUtility = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.SdkCallBackComponent)o).sdkUtility = @sdkUtility;
            return ptr_of_this_method;
        }

        static object get_LogoutCallBack_2(ref object o)
        {
            return ((ETModel.SdkCallBackComponent)o).LogoutCallBack;
        }

        static StackObject* CopyToStack_LogoutCallBack_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.SdkCallBackComponent)o).LogoutCallBack;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LogoutCallBack_2(ref object o, object v)
        {
            ((ETModel.SdkCallBackComponent)o).LogoutCallBack = (System.Action<System.String>)v;
        }

        static StackObject* AssignFromStack_LogoutCallBack_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.String> @LogoutCallBack = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.SdkCallBackComponent)o).LogoutCallBack = @LogoutCallBack;
            return ptr_of_this_method;
        }

        static object get_LoginCallBack_3(ref object o)
        {
            return ((ETModel.SdkCallBackComponent)o).LoginCallBack;
        }

        static StackObject* CopyToStack_LoginCallBack_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.SdkCallBackComponent)o).LoginCallBack;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LoginCallBack_3(ref object o, object v)
        {
            ((ETModel.SdkCallBackComponent)o).LoginCallBack = (System.Action<System.String>)v;
        }

        static StackObject* AssignFromStack_LoginCallBack_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.String> @LoginCallBack = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.SdkCallBackComponent)o).LoginCallBack = @LoginCallBack;
            return ptr_of_this_method;
        }



    }
}
