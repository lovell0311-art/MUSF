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
    unsafe class ETModel_Guidance_Define_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.Guidance_Define);

            field = type.GetField("IsBeginnerGuide", flag);
            app.RegisterCLRFieldGetter(field, get_IsBeginnerGuide_0);
            app.RegisterCLRFieldSetter(field, set_IsBeginnerGuide_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsBeginnerGuide_0, AssignFromStack_IsBeginnerGuide_0);


        }



        static object get_IsBeginnerGuide_0(ref object o)
        {
            return ETModel.Guidance_Define.IsBeginnerGuide;
        }

        static StackObject* CopyToStack_IsBeginnerGuide_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ETModel.Guidance_Define.IsBeginnerGuide;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IsBeginnerGuide_0(ref object o, object v)
        {
            ETModel.Guidance_Define.IsBeginnerGuide = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IsBeginnerGuide_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IsBeginnerGuide = ptr_of_this_method->Value == 1;
            ETModel.Guidance_Define.IsBeginnerGuide = @IsBeginnerGuide;
            return ptr_of_this_method;
        }



    }
}
