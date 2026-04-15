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
    unsafe class ETModel_UIUtils_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(ETModel.UIUtils);
            args = new Type[]{typeof(UnityEngine.EventSystems.UIBehaviour), typeof(UnityEngine.EventSystems.EventTriggerType), typeof(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>)};
            method = type.GetMethod("AddCustomEventListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddCustomEventListener_0);


        }


        static StackObject* AddCustomEventListener_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData> @callBack = (UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>)typeof(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.EventSystems.EventTriggerType @type = (UnityEngine.EventSystems.EventTriggerType)typeof(UnityEngine.EventSystems.EventTriggerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.EventSystems.UIBehaviour @control = (UnityEngine.EventSystems.UIBehaviour)typeof(UnityEngine.EventSystems.UIBehaviour).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            ETModel.UIUtils.AddCustomEventListener(@control, @type, @callBack);

            return __ret;
        }



    }
}
