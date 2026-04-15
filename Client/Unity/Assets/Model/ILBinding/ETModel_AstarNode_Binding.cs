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
    unsafe class ETModel_AstarNode_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.AstarNode);
            args = new Type[]{typeof(ETModel.AstarNode)};
            method = type.GetMethod("Compare", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Compare_0);
            args = new Type[]{};
            method = type.GetMethod("get_AreaPosX", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_AreaPosX_1);
            args = new Type[]{};
            method = type.GetMethod("get_AreaPosY", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_AreaPosY_2);

            field = type.GetField("isWalkable", flag);
            app.RegisterCLRFieldGetter(field, get_isWalkable_0);
            app.RegisterCLRFieldSetter(field, set_isWalkable_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_isWalkable_0, AssignFromStack_isWalkable_0);
            field = type.GetField("x", flag);
            app.RegisterCLRFieldGetter(field, get_x_1);
            app.RegisterCLRFieldSetter(field, set_x_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_x_1, AssignFromStack_x_1);
            field = type.GetField("z", flag);
            app.RegisterCLRFieldGetter(field, get_z_2);
            app.RegisterCLRFieldSetter(field, set_z_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_z_2, AssignFromStack_z_2);


        }


        static StackObject* Compare_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.AstarNode @node = (ETModel.AstarNode)typeof(ETModel.AstarNode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.AstarNode instance_of_this_method = (ETModel.AstarNode)typeof(ETModel.AstarNode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Compare(@node);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_AreaPosX_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.AstarNode instance_of_this_method = (ETModel.AstarNode)typeof(ETModel.AstarNode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AreaPosX;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_AreaPosY_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.AstarNode instance_of_this_method = (ETModel.AstarNode)typeof(ETModel.AstarNode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AreaPosY;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static object get_isWalkable_0(ref object o)
        {
            return ((ETModel.AstarNode)o).isWalkable;
        }

        static StackObject* CopyToStack_isWalkable_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.AstarNode)o).isWalkable;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isWalkable_0(ref object o, object v)
        {
            ((ETModel.AstarNode)o).isWalkable = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isWalkable_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isWalkable = ptr_of_this_method->Value == 1;
            ((ETModel.AstarNode)o).isWalkable = @isWalkable;
            return ptr_of_this_method;
        }

        static object get_x_1(ref object o)
        {
            return ((ETModel.AstarNode)o).x;
        }

        static StackObject* CopyToStack_x_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.AstarNode)o).x;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_x_1(ref object o, object v)
        {
            ((ETModel.AstarNode)o).x = (System.Int32)v;
        }

        static StackObject* AssignFromStack_x_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @x = ptr_of_this_method->Value;
            ((ETModel.AstarNode)o).x = @x;
            return ptr_of_this_method;
        }

        static object get_z_2(ref object o)
        {
            return ((ETModel.AstarNode)o).z;
        }

        static StackObject* CopyToStack_z_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.AstarNode)o).z;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_z_2(ref object o, object v)
        {
            ((ETModel.AstarNode)o).z = (System.Int32)v;
        }

        static StackObject* AssignFromStack_z_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @z = ptr_of_this_method->Value;
            ((ETModel.AstarNode)o).z = @z;
            return ptr_of_this_method;
        }



    }
}
