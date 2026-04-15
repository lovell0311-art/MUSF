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
    unsafe class ETModel_UGUITriggerProxy_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(ETModel.UGUITriggerProxy);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnPointerClickEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnPointerClickEvent_0);
            args = new Type[]{};
            method = type.GetMethod("get_OnBeginDragEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OnBeginDragEvent_1);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnBeginDragEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnBeginDragEvent_2);
            args = new Type[]{};
            method = type.GetMethod("get_OnEndDragEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OnEndDragEvent_3);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnEndDragEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnEndDragEvent_4);
            args = new Type[]{};
            method = type.GetMethod("get_OnPointerEnterEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OnPointerEnterEvent_5);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnPointerEnterEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnPointerEnterEvent_6);
            args = new Type[]{};
            method = type.GetMethod("get_OnPointerExitEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OnPointerExitEvent_7);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnPointerExitEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnPointerExitEvent_8);
            args = new Type[]{};
            method = type.GetMethod("get_OnPointerClickEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OnPointerClickEvent_9);
            args = new Type[]{};
            method = type.GetMethod("get_OnPointerDownEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OnPointerDownEvent_10);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnPointerDownEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnPointerDownEvent_11);
            args = new Type[]{};
            method = type.GetMethod("get_OnPointerUpEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OnPointerUpEvent_12);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnPointerUpEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnPointerUpEvent_13);
            args = new Type[]{typeof(System.Action<UnityEngine.EventSystems.PointerEventData>)};
            method = type.GetMethod("set_OnDragEvents", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnDragEvents_14);
            args = new Type[]{typeof(System.Action<UnityEngine.EventSystems.PointerEventData>)};
            method = type.GetMethod("set_OnPointerDownEvents", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnPointerDownEvents_15);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnDragEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnDragEvent_16);


        }


        static StackObject* set_OnPointerClickEvent_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPointerClickEvent = value;

            return __ret;
        }

        static StackObject* get_OnBeginDragEvent_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OnBeginDragEvent;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_OnBeginDragEvent_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnBeginDragEvent = value;

            return __ret;
        }

        static StackObject* get_OnEndDragEvent_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OnEndDragEvent;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_OnEndDragEvent_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnEndDragEvent = value;

            return __ret;
        }

        static StackObject* get_OnPointerEnterEvent_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OnPointerEnterEvent;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_OnPointerEnterEvent_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPointerEnterEvent = value;

            return __ret;
        }

        static StackObject* get_OnPointerExitEvent_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OnPointerExitEvent;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_OnPointerExitEvent_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPointerExitEvent = value;

            return __ret;
        }

        static StackObject* get_OnPointerClickEvent_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OnPointerClickEvent;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_OnPointerDownEvent_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OnPointerDownEvent;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_OnPointerDownEvent_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPointerDownEvent = value;

            return __ret;
        }

        static StackObject* get_OnPointerUpEvent_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OnPointerUpEvent;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_OnPointerUpEvent_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPointerUpEvent = value;

            return __ret;
        }

        static StackObject* set_OnDragEvents_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<UnityEngine.EventSystems.PointerEventData> @value = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnDragEvents = value;

            return __ret;
        }

        static StackObject* set_OnPointerDownEvents_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<UnityEngine.EventSystems.PointerEventData> @value = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPointerDownEvents = value;

            return __ret;
        }

        static StackObject* set_OnDragEvent_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.UGUITriggerProxy instance_of_this_method = (ETModel.UGUITriggerProxy)typeof(ETModel.UGUITriggerProxy).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnDragEvent = value;

            return __ret;
        }



    }
}
