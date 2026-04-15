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
    unsafe class ETModel_TikTokLoginResult_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.TikTokLoginResult);

            field = type.GetField("accessToken", flag);
            app.RegisterCLRFieldGetter(field, get_accessToken_0);
            app.RegisterCLRFieldSetter(field, set_accessToken_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_accessToken_0, AssignFromStack_accessToken_0);
            field = type.GetField("userId", flag);
            app.RegisterCLRFieldGetter(field, get_userId_1);
            app.RegisterCLRFieldSetter(field, set_userId_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_userId_1, AssignFromStack_userId_1);


        }



        static object get_accessToken_0(ref object o)
        {
            return ((ETModel.TikTokLoginResult)o).accessToken;
        }

        static StackObject* CopyToStack_accessToken_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.TikTokLoginResult)o).accessToken;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_accessToken_0(ref object o, object v)
        {
            ((ETModel.TikTokLoginResult)o).accessToken = (System.String)v;
        }

        static StackObject* AssignFromStack_accessToken_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @accessToken = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.TikTokLoginResult)o).accessToken = @accessToken;
            return ptr_of_this_method;
        }

        static object get_userId_1(ref object o)
        {
            return ((ETModel.TikTokLoginResult)o).userId;
        }

        static StackObject* CopyToStack_userId_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.TikTokLoginResult)o).userId;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_userId_1(ref object o, object v)
        {
            ((ETModel.TikTokLoginResult)o).userId = (System.Int64)v;
        }

        static StackObject* AssignFromStack_userId_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @userId = *(long*)&ptr_of_this_method->Value;
            ((ETModel.TikTokLoginResult)o).userId = @userId;
            return ptr_of_this_method;
        }



    }
}
