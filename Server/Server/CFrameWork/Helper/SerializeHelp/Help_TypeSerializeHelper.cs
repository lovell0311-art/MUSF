using System;
using System.Collections.Generic;
using System.Text;

namespace CustomFrameWork
{
    public class Help_TypeSerializeHelper
    {
        public static object DeSerialize(string b_StrData, Type b_TargetType)
        {
            Type type = b_TargetType;
            if (type.IsEnum)
            {
                if (Enum.TryParse(type, b_StrData, out object mResult) == false)
                    throw new ArgumentException($"=>Enum.TryParse<{type.FullName}>()失败!转换参数:{b_StrData}");
                return mResult;
            }
            else
            {
                return Convert.ChangeType(b_StrData, type);
            }
        }
        public static T DeSerialize<T>(string b_StrData)
        {
            Type type = typeof(T);
            if (type.IsEnum)
            {
                if (Enum.TryParse(type, b_StrData, out object mResult) == false)
                    throw new ArgumentException($"=>Enum.TryParse<{type.FullName}>()失败!转换参数:{b_StrData}");
                return (T)mResult;
            }
            else
            {
                return (T)Convert.ChangeType(b_StrData, type);
            }
        }
    }
}
