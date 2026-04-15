using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomFrameWork.CustomInterface
{
    /// <summary>
    /// 序列化接口
    /// </summary>
    interface ICustomComponentSerialize
    {
         void BeforeAwakeSerialize<T>(T b_Args);

         void AfterAwakeSerialize<T>(T b_Args);
    }
}
