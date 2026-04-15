using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// 日志工具 全局唯一  bool:是不是调用输出控制台操作  bool:是不是写入数据到文本
    /// </summary>
    public partial class LogToolComponent
    {
        public static string LogString(object b_Message)
        {
            StringBuilder mStringBuilder = new StringBuilder();

            var mCurrentType = b_Message.GetType();

            mStringBuilder.AppendLine(mCurrentType.Name);

            var mProperties = mCurrentType.GetProperties();
            for (int i = 0, len = mProperties.Length - 1; i < len; i++)
            {
                var mPropertie = mProperties[i];

                var mPropertieValue = mPropertie.GetValue(b_Message);

                mStringBuilder.AppendLine($"├─{mPropertie.Name}:{mPropertieValue}");
            }

            var mPropertieEnd = mProperties[mProperties.Length - 1];
            var mPropertieValueEnd = mPropertieEnd.GetValue(b_Message);

            mStringBuilder.AppendLine($"└─{mPropertieEnd.Name}:{mPropertieValueEnd}");

            return mStringBuilder.ToString();
        }
    }
}
