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
    unsafe class ETModel_CameraFollowComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.CameraFollowComponent);

            field = type.GetField("Instance", flag);
            app.RegisterCLRFieldGetter(field, get_Instance_0);
            app.RegisterCLRFieldSetter(field, set_Instance_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Instance_0, AssignFromStack_Instance_0);
            field = type.GetField("followTarget", flag);
            app.RegisterCLRFieldGetter(field, get_followTarget_1);
            app.RegisterCLRFieldSetter(field, set_followTarget_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_followTarget_1, AssignFromStack_followTarget_1);
            field = type.GetField("ChangeScene", flag);
            app.RegisterCLRFieldGetter(field, get_ChangeScene_2);
            app.RegisterCLRFieldSetter(field, set_ChangeScene_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_ChangeScene_2, AssignFromStack_ChangeScene_2);
            field = type.GetField("h", flag);
            app.RegisterCLRFieldGetter(field, get_h_3);
            app.RegisterCLRFieldSetter(field, set_h_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_h_3, AssignFromStack_h_3);
            field = type.GetField("distance", flag);
            app.RegisterCLRFieldGetter(field, get_distance_4);
            app.RegisterCLRFieldSetter(field, set_distance_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_distance_4, AssignFromStack_distance_4);
            field = type.GetField("curAngleV", flag);
            app.RegisterCLRFieldGetter(field, get_curAngleV_5);
            app.RegisterCLRFieldSetter(field, set_curAngleV_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_curAngleV_5, AssignFromStack_curAngleV_5);
            field = type.GetField("curAngleH", flag);
            app.RegisterCLRFieldGetter(field, get_curAngleH_6);
            app.RegisterCLRFieldSetter(field, set_curAngleH_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_curAngleH_6, AssignFromStack_curAngleH_6);


        }



        static object get_Instance_0(ref object o)
        {
            return ETModel.CameraFollowComponent.Instance;
        }

        static StackObject* CopyToStack_Instance_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ETModel.CameraFollowComponent.Instance;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Instance_0(ref object o, object v)
        {
            ETModel.CameraFollowComponent.Instance = (ETModel.CameraFollowComponent)v;
        }

        static StackObject* AssignFromStack_Instance_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ETModel.CameraFollowComponent @Instance = (ETModel.CameraFollowComponent)typeof(ETModel.CameraFollowComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ETModel.CameraFollowComponent.Instance = @Instance;
            return ptr_of_this_method;
        }

        static object get_followTarget_1(ref object o)
        {
            return ((ETModel.CameraFollowComponent)o).followTarget;
        }

        static StackObject* CopyToStack_followTarget_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.CameraFollowComponent)o).followTarget;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_followTarget_1(ref object o, object v)
        {
            ((ETModel.CameraFollowComponent)o).followTarget = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_followTarget_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @followTarget = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.CameraFollowComponent)o).followTarget = @followTarget;
            return ptr_of_this_method;
        }

        static object get_ChangeScene_2(ref object o)
        {
            return ((ETModel.CameraFollowComponent)o).ChangeScene;
        }

        static StackObject* CopyToStack_ChangeScene_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.CameraFollowComponent)o).ChangeScene;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_ChangeScene_2(ref object o, object v)
        {
            ((ETModel.CameraFollowComponent)o).ChangeScene = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_ChangeScene_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @ChangeScene = ptr_of_this_method->Value == 1;
            ((ETModel.CameraFollowComponent)o).ChangeScene = @ChangeScene;
            return ptr_of_this_method;
        }

        static object get_h_3(ref object o)
        {
            return ((ETModel.CameraFollowComponent)o).h;
        }

        static StackObject* CopyToStack_h_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.CameraFollowComponent)o).h;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_h_3(ref object o, object v)
        {
            ((ETModel.CameraFollowComponent)o).h = (System.Single)v;
        }

        static StackObject* AssignFromStack_h_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @h = *(float*)&ptr_of_this_method->Value;
            ((ETModel.CameraFollowComponent)o).h = @h;
            return ptr_of_this_method;
        }

        static object get_distance_4(ref object o)
        {
            return ((ETModel.CameraFollowComponent)o).distance;
        }

        static StackObject* CopyToStack_distance_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.CameraFollowComponent)o).distance;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_distance_4(ref object o, object v)
        {
            ((ETModel.CameraFollowComponent)o).distance = (System.Single)v;
        }

        static StackObject* AssignFromStack_distance_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @distance = *(float*)&ptr_of_this_method->Value;
            ((ETModel.CameraFollowComponent)o).distance = @distance;
            return ptr_of_this_method;
        }

        static object get_curAngleV_5(ref object o)
        {
            return ((ETModel.CameraFollowComponent)o).curAngleV;
        }

        static StackObject* CopyToStack_curAngleV_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.CameraFollowComponent)o).curAngleV;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_curAngleV_5(ref object o, object v)
        {
            ((ETModel.CameraFollowComponent)o).curAngleV = (System.Single)v;
        }

        static StackObject* AssignFromStack_curAngleV_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @curAngleV = *(float*)&ptr_of_this_method->Value;
            ((ETModel.CameraFollowComponent)o).curAngleV = @curAngleV;
            return ptr_of_this_method;
        }

        static object get_curAngleH_6(ref object o)
        {
            return ((ETModel.CameraFollowComponent)o).curAngleH;
        }

        static StackObject* CopyToStack_curAngleH_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.CameraFollowComponent)o).curAngleH;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_curAngleH_6(ref object o, object v)
        {
            ((ETModel.CameraFollowComponent)o).curAngleH = (System.Single)v;
        }

        static StackObject* AssignFromStack_curAngleH_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @curAngleH = *(float*)&ptr_of_this_method->Value;
            ((ETModel.CameraFollowComponent)o).curAngleH = @curAngleH;
            return ptr_of_this_method;
        }



    }
}
