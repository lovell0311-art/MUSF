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
    unsafe class ETModel_AnimationEventProxy_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.AnimationEventProxy);

            field = type.GetField("Effect_Action_Str", flag);
            app.RegisterCLRFieldGetter(field, get_Effect_Action_Str_0);
            app.RegisterCLRFieldSetter(field, set_Effect_Action_Str_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Effect_Action_Str_0, AssignFromStack_Effect_Action_Str_0);
            field = type.GetField("Effect_action", flag);
            app.RegisterCLRFieldGetter(field, get_Effect_action_1);
            app.RegisterCLRFieldSetter(field, set_Effect_action_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Effect_action_1, AssignFromStack_Effect_action_1);
            field = type.GetField("Monster_DeadEffect_action", flag);
            app.RegisterCLRFieldGetter(field, get_Monster_DeadEffect_action_2);
            app.RegisterCLRFieldSetter(field, set_Monster_DeadEffect_action_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_Monster_DeadEffect_action_2, AssignFromStack_Monster_DeadEffect_action_2);
            field = type.GetField("Sound_action", flag);
            app.RegisterCLRFieldGetter(field, get_Sound_action_3);
            app.RegisterCLRFieldSetter(field, set_Sound_action_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_Sound_action_3, AssignFromStack_Sound_action_3);


        }



        static object get_Effect_Action_Str_0(ref object o)
        {
            return ((ETModel.AnimationEventProxy)o).Effect_Action_Str;
        }

        static StackObject* CopyToStack_Effect_Action_Str_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.AnimationEventProxy)o).Effect_Action_Str;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Effect_Action_Str_0(ref object o, object v)
        {
            ((ETModel.AnimationEventProxy)o).Effect_Action_Str = (System.Action<System.String>)v;
        }

        static StackObject* AssignFromStack_Effect_Action_Str_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.String> @Effect_Action_Str = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.AnimationEventProxy)o).Effect_Action_Str = @Effect_Action_Str;
            return ptr_of_this_method;
        }

        static object get_Effect_action_1(ref object o)
        {
            return ((ETModel.AnimationEventProxy)o).Effect_action;
        }

        static StackObject* CopyToStack_Effect_action_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.AnimationEventProxy)o).Effect_action;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Effect_action_1(ref object o, object v)
        {
            ((ETModel.AnimationEventProxy)o).Effect_action = (System.Action)v;
        }

        static StackObject* AssignFromStack_Effect_action_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @Effect_action = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.AnimationEventProxy)o).Effect_action = @Effect_action;
            return ptr_of_this_method;
        }

        static object get_Monster_DeadEffect_action_2(ref object o)
        {
            return ((ETModel.AnimationEventProxy)o).Monster_DeadEffect_action;
        }

        static StackObject* CopyToStack_Monster_DeadEffect_action_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.AnimationEventProxy)o).Monster_DeadEffect_action;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Monster_DeadEffect_action_2(ref object o, object v)
        {
            ((ETModel.AnimationEventProxy)o).Monster_DeadEffect_action = (System.Action<System.String>)v;
        }

        static StackObject* AssignFromStack_Monster_DeadEffect_action_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.String> @Monster_DeadEffect_action = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.AnimationEventProxy)o).Monster_DeadEffect_action = @Monster_DeadEffect_action;
            return ptr_of_this_method;
        }

        static object get_Sound_action_3(ref object o)
        {
            return ((ETModel.AnimationEventProxy)o).Sound_action;
        }

        static StackObject* CopyToStack_Sound_action_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.AnimationEventProxy)o).Sound_action;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Sound_action_3(ref object o, object v)
        {
            ((ETModel.AnimationEventProxy)o).Sound_action = (System.Action)v;
        }

        static StackObject* AssignFromStack_Sound_action_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @Sound_action = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.AnimationEventProxy)o).Sound_action = @Sound_action;
            return ptr_of_this_method;
        }



    }
}
