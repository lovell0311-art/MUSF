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
    unsafe class ETModel_TextMove_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.TextMove);

            field = type.GetField("RollOveraction", flag);
            app.RegisterCLRFieldGetter(field, get_RollOveraction_0);
            app.RegisterCLRFieldSetter(field, set_RollOveraction_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_RollOveraction_0, AssignFromStack_RollOveraction_0);
            field = type.GetField("speed", flag);
            app.RegisterCLRFieldGetter(field, get_speed_1);
            app.RegisterCLRFieldSetter(field, set_speed_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_speed_1, AssignFromStack_speed_1);


        }



        static object get_RollOveraction_0(ref object o)
        {
            return ((ETModel.TextMove)o).RollOveraction;
        }

        static StackObject* CopyToStack_RollOveraction_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.TextMove)o).RollOveraction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_RollOveraction_0(ref object o, object v)
        {
            ((ETModel.TextMove)o).RollOveraction = (System.Action)v;
        }

        static StackObject* AssignFromStack_RollOveraction_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @RollOveraction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ETModel.TextMove)o).RollOveraction = @RollOveraction;
            return ptr_of_this_method;
        }

        static object get_speed_1(ref object o)
        {
            return ((ETModel.TextMove)o).speed;
        }

        static StackObject* CopyToStack_speed_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.TextMove)o).speed;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_speed_1(ref object o, object v)
        {
            ((ETModel.TextMove)o).speed = (System.Single)v;
        }

        static StackObject* AssignFromStack_speed_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @speed = *(float*)&ptr_of_this_method->Value;
            ((ETModel.TextMove)o).speed = @speed;
            return ptr_of_this_method;
        }



    }
}
