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
    unsafe class ETModel_UserInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.UserInfo);

            field = type.GetField("id", flag);
            app.RegisterCLRFieldGetter(field, get_id_0);
            app.RegisterCLRFieldSetter(field, set_id_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_id_0, AssignFromStack_id_0);
            field = type.GetField("loginTime", flag);
            app.RegisterCLRFieldGetter(field, get_loginTime_1);
            app.RegisterCLRFieldSetter(field, set_loginTime_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_loginTime_1, AssignFromStack_loginTime_1);
            field = type.GetField("token", flag);
            app.RegisterCLRFieldGetter(field, get_token_2);
            app.RegisterCLRFieldSetter(field, set_token_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_token_2, AssignFromStack_token_2);


        }



        static object get_id_0(ref object o)
        {
            return ((ETModel.UserInfo)o).id;
        }

        static StackObject* CopyToStack_id_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.UserInfo)o).id;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_id_0(ref object o, object v)
        {
            ((ETModel.UserInfo)o).id = (System.String)v;
        }

        static StackObject* AssignFromStack_id_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @id = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.UserInfo)o).id = @id;
            return ptr_of_this_method;
        }

        static object get_loginTime_1(ref object o)
        {
            return ((ETModel.UserInfo)o).loginTime;
        }

        static StackObject* CopyToStack_loginTime_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.UserInfo)o).loginTime;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_loginTime_1(ref object o, object v)
        {
            ((ETModel.UserInfo)o).loginTime = (System.String)v;
        }

        static StackObject* AssignFromStack_loginTime_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @loginTime = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.UserInfo)o).loginTime = @loginTime;
            return ptr_of_this_method;
        }

        static object get_token_2(ref object o)
        {
            return ((ETModel.UserInfo)o).token;
        }

        static StackObject* CopyToStack_token_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.UserInfo)o).token;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_token_2(ref object o, object v)
        {
            ((ETModel.UserInfo)o).token = (System.String)v;
        }

        static StackObject* AssignFromStack_token_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @token = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.UserInfo)o).token = @token;
            return ptr_of_this_method;
        }



    }
}
