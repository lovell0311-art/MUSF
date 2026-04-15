using System;

namespace ETModel
{
    /// <summary>
    /// 私有对象标签
    /// <para>禁止其他人调用未提供接口的变量</para>
    /// <para>如需调用，让对象维护者提供接口</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PrivateObjectAttribute : Attribute
    {
    }
}
