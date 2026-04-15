using System;
using System.Collections.Generic;
using System.Text;

namespace CustomFrameWork.Component
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionAttribute : BaseAttribute
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object Default { get; set; }
        /// <summary>
        /// 初始化时是否忽略
        /// </summary>
        public bool OptionIgnore { get; set; } = false;
        public OptionAttribute(string b_Name)
        {
            Name = b_Name;
        }
    }
}
