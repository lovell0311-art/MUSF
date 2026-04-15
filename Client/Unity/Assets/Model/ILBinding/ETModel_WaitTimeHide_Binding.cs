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
    unsafe class ETModel_WaitTimeHide_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.WaitTimeHide);

            field = type.GetField("time", flag);
            app.RegisterCLRFieldGetter(field, get_time_0);
            app.RegisterCLRFieldSetter(field, set_time_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_time_0, AssignFromStack_time_0);


        }



        static object get_time_0(ref object o)
        {
            return ((ETModel.WaitTimeHide)o).time;
        }

        static StackObject* CopyToStack_time_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.WaitTimeHide)o).time;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_time_0(ref object o, object v)
        {
            ((ETModel.WaitTimeHide)o).time = (System.Single)v;
        }

        static StackObject* AssignFromStack_time_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @time = *(float*)&ptr_of_this_method->Value;
            ((ETModel.WaitTimeHide)o).time = @time;
            return ptr_of_this_method;
        }



    }
}
