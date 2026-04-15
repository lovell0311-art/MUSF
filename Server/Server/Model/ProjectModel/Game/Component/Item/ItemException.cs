using System;

namespace ETModel
{
    /// <summary>
    /// 创建物品时，没有指定属性时，会抛出这个异常
    /// </summary>
    public class ItemNotSupportAttrException : Exception
    {
        public ItemNotSupportAttrException(string? message) : base(message)
        {

        }
    }

    /// <summary>
    /// 创建物品时，找不到指定配置文件时，会抛出这个异常
    /// </summary>
    public class ItemConfigNotExistException : Exception 
    {
        public ItemConfigNotExistException(string? message):base(message)
        {

        }
    }



}
