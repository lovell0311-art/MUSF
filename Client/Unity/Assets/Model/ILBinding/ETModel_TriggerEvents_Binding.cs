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
    unsafe class ETModel_TriggerEvents_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.TriggerEvents);

            field = type.GetField("RoleActionLevea", flag);
            app.RegisterCLRFieldGetter(field, get_RoleActionLevea_0);
            app.RegisterCLRFieldSetter(field, set_RoleActionLevea_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_RoleActionLevea_0, AssignFromStack_RoleActionLevea_0);
            field = type.GetField("RoleActionEnter", flag);
            app.RegisterCLRFieldGetter(field, get_RoleActionEnter_1);
            app.RegisterCLRFieldSetter(field, set_RoleActionEnter_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_RoleActionEnter_1, AssignFromStack_RoleActionEnter_1);
            field = type.GetField("RoleActionStay", flag);
            app.RegisterCLRFieldGetter(field, get_RoleActionStay_2);
            app.RegisterCLRFieldSetter(field, set_RoleActionStay_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_RoleActionStay_2, AssignFromStack_RoleActionStay_2);


        }



        static object get_RoleActionLevea_0(ref object o)
        {
            return ((ETModel.TriggerEvents)o).RoleActionLevea;
        }

        static StackObject* CopyToStack_RoleActionLevea_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.TriggerEvents)o).RoleActionLevea;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_RoleActionLevea_0(ref object o, object v)
        {
            ((ETModel.TriggerEvents)o).RoleActionLevea = (System.Action<UnityEngine.Transform>)v;
        }

        static StackObject* AssignFromStack_RoleActionLevea_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Transform> @RoleActionLevea = (System.Action<UnityEngine.Transform>)typeof(System.Action<UnityEngine.Transform>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.TriggerEvents)o).RoleActionLevea = @RoleActionLevea;
            return ptr_of_this_method;
        }

        static object get_RoleActionEnter_1(ref object o)
        {
            return ((ETModel.TriggerEvents)o).RoleActionEnter;
        }

        static StackObject* CopyToStack_RoleActionEnter_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.TriggerEvents)o).RoleActionEnter;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_RoleActionEnter_1(ref object o, object v)
        {
            ((ETModel.TriggerEvents)o).RoleActionEnter = (System.Action<UnityEngine.Transform>)v;
        }

        static StackObject* AssignFromStack_RoleActionEnter_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Transform> @RoleActionEnter = (System.Action<UnityEngine.Transform>)typeof(System.Action<UnityEngine.Transform>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.TriggerEvents)o).RoleActionEnter = @RoleActionEnter;
            return ptr_of_this_method;
        }

        static object get_RoleActionStay_2(ref object o)
        {
            return ((ETModel.TriggerEvents)o).RoleActionStay;
        }

        static StackObject* CopyToStack_RoleActionStay_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.TriggerEvents)o).RoleActionStay;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_RoleActionStay_2(ref object o, object v)
        {
            ((ETModel.TriggerEvents)o).RoleActionStay = (System.Action<UnityEngine.Transform>)v;
        }

        static StackObject* AssignFromStack_RoleActionStay_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Transform> @RoleActionStay = (System.Action<UnityEngine.Transform>)typeof(System.Action<UnityEngine.Transform>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.TriggerEvents)o).RoleActionStay = @RoleActionStay;
            return ptr_of_this_method;
        }



    }
}
