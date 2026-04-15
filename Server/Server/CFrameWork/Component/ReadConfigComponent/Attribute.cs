using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using CustomFrameWork.Baseic;

namespace CustomFrameWork.Component
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ReadConfigAttribute : BaseAttribute
    {
        public List<AppType> BindId { get; set; }
        public string ConfigName { get; private set; }
        public Type ConfigType { get; private set; }
        public ReadConfigAttribute(Type b_ConfigType, string b_BindId)
        {
            this.ConfigType = b_ConfigType;
            this.BindId = Help_JsonSerializeHelper.DeSerialize<List<AppType>>(b_BindId);
        }
        public ReadConfigAttribute(string b_ConfigName, string b_BindId)
        {
            this.ConfigName = b_ConfigName;
            this.BindId = Help_JsonSerializeHelper.DeSerialize<List<AppType>>(b_BindId);
        }
        public ReadConfigAttribute(Type b_ConfigType, AppType[] b_BindId)
        {
            this.ConfigType = b_ConfigType;
            this.BindId = b_BindId.ToList();
            if (this.BindId.Count == 0)
            {
                this.BindId.Add(AppType.AllServer);
            }
        }
    }
}
