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
    unsafe class SpawnPoint_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::SpawnPoint);

            field = type.GetField("PositionX", flag);
            app.RegisterCLRFieldGetter(field, get_PositionX_0);
            app.RegisterCLRFieldSetter(field, set_PositionX_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_PositionX_0, AssignFromStack_PositionX_0);
            field = type.GetField("PositionY", flag);
            app.RegisterCLRFieldGetter(field, get_PositionY_1);
            app.RegisterCLRFieldSetter(field, set_PositionY_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_PositionY_1, AssignFromStack_PositionY_1);
            field = type.GetField("Index", flag);
            app.RegisterCLRFieldGetter(field, get_Index_2);
            app.RegisterCLRFieldSetter(field, set_Index_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_Index_2, AssignFromStack_Index_2);


        }



        static object get_PositionX_0(ref object o)
        {
            return ((global::SpawnPoint)o).PositionX;
        }

        static StackObject* CopyToStack_PositionX_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SpawnPoint)o).PositionX;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_PositionX_0(ref object o, object v)
        {
            ((global::SpawnPoint)o).PositionX = (System.Int32)v;
        }

        static StackObject* AssignFromStack_PositionX_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @PositionX = ptr_of_this_method->Value;
            ((global::SpawnPoint)o).PositionX = @PositionX;
            return ptr_of_this_method;
        }

        static object get_PositionY_1(ref object o)
        {
            return ((global::SpawnPoint)o).PositionY;
        }

        static StackObject* CopyToStack_PositionY_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SpawnPoint)o).PositionY;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_PositionY_1(ref object o, object v)
        {
            ((global::SpawnPoint)o).PositionY = (System.Int32)v;
        }

        static StackObject* AssignFromStack_PositionY_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @PositionY = ptr_of_this_method->Value;
            ((global::SpawnPoint)o).PositionY = @PositionY;
            return ptr_of_this_method;
        }

        static object get_Index_2(ref object o)
        {
            return ((global::SpawnPoint)o).Index;
        }

        static StackObject* CopyToStack_Index_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SpawnPoint)o).Index;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Index_2(ref object o, object v)
        {
            ((global::SpawnPoint)o).Index = (System.Int64)v;
        }

        static StackObject* AssignFromStack_Index_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @Index = *(long*)&ptr_of_this_method->Value;
            ((global::SpawnPoint)o).Index = @Index;
            return ptr_of_this_method;
        }



    }
}
