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
    unsafe class ETModel_SdkUtility_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.SdkUtility);
            args = new Type[]{typeof(System.String[])};
            method = type.GetMethod("UploadRoleInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UploadRoleInfo_0);
            args = new Type[]{typeof(System.String[])};
            method = type.GetMethod("UpdateRoleGrade", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UpdateRoleGrade_1);
            args = new Type[]{typeof(System.String[])};
            method = type.GetMethod("Pay", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Pay_2);
            args = new Type[]{};
            method = type.GetMethod("GetRiskControlInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetRiskControlInfo_3);
            args = new Type[]{};
            method = type.GetMethod("Exit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Exit_4);
            args = new Type[]{typeof(System.String[])};
            method = type.GetMethod("PlayLog", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayLog_5);
            args = new Type[]{typeof(System.String[])};
            method = type.GetMethod("CreatRole", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CreatRole_6);
            args = new Type[]{};
            method = type.GetMethod("Login", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Login_7);
            args = new Type[]{};
            method = type.GetMethod("ShowUserAgreementPrivac", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ShowUserAgreementPrivac_8);
            args = new Type[]{};
            method = type.GetMethod("IsLogin", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsLogin_9);
            args = new Type[]{};
            method = type.GetMethod("SwitchLogin", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SwitchLogin_10);
            args = new Type[]{};
            method = type.GetMethod("LoginOut", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoginOut_11);

            field = type.GetField("CallAllObjName", flag);
            app.RegisterCLRFieldGetter(field, get_CallAllObjName_0);
            app.RegisterCLRFieldSetter(field, set_CallAllObjName_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_CallAllObjName_0, AssignFromStack_CallAllObjName_0);
            field = type.GetField("LoginInfo", flag);
            app.RegisterCLRFieldGetter(field, get_LoginInfo_1);
            app.RegisterCLRFieldSetter(field, set_LoginInfo_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_LoginInfo_1, AssignFromStack_LoginInfo_1);


        }


        static StackObject* UploadRoleInfo_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String[] @objs = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UploadRoleInfo(@objs);

            return __ret;
        }

        static StackObject* UpdateRoleGrade_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String[] @objs = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UpdateRoleGrade(@objs);

            return __ret;
        }

        static StackObject* Pay_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String[] @objs = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Pay(@objs);

            return __ret;
        }

        static StackObject* GetRiskControlInfo_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetRiskControlInfo();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Exit_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Exit();

            return __ret;
        }

        static StackObject* PlayLog_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String[] @objs = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PlayLog(@objs);

            return __ret;
        }

        static StackObject* CreatRole_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String[] @objs = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CreatRole(@objs);

            return __ret;
        }

        static StackObject* Login_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Login();

            return __ret;
        }

        static StackObject* ShowUserAgreementPrivac_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ShowUserAgreementPrivac();

            return __ret;
        }

        static StackObject* IsLogin_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsLogin();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* SwitchLogin_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SwitchLogin();

            return __ret;
        }

        static StackObject* LoginOut_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ETModel.SdkUtility instance_of_this_method = (ETModel.SdkUtility)typeof(ETModel.SdkUtility).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.LoginOut();

            return __ret;
        }


        static object get_CallAllObjName_0(ref object o)
        {
            return ((ETModel.SdkUtility)o).CallAllObjName;
        }

        static StackObject* CopyToStack_CallAllObjName_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.SdkUtility)o).CallAllObjName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_CallAllObjName_0(ref object o, object v)
        {
            ((ETModel.SdkUtility)o).CallAllObjName = (System.String)v;
        }

        static StackObject* AssignFromStack_CallAllObjName_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @CallAllObjName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.SdkUtility)o).CallAllObjName = @CallAllObjName;
            return ptr_of_this_method;
        }

        static object get_LoginInfo_1(ref object o)
        {
            return ((ETModel.SdkUtility)o).LoginInfo;
        }

        static StackObject* CopyToStack_LoginInfo_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ETModel.SdkUtility)o).LoginInfo;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LoginInfo_1(ref object o, object v)
        {
            ((ETModel.SdkUtility)o).LoginInfo = (System.String)v;
        }

        static StackObject* AssignFromStack_LoginInfo_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @LoginInfo = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ETModel.SdkUtility)o).LoginInfo = @LoginInfo;
            return ptr_of_this_method;
        }



    }
}
