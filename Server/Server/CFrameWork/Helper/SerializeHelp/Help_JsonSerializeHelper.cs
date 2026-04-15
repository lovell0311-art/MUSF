
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomFrameWork
{
    public class Help_JsonSerializeHelper
    {
        public static JsonSerializerSettings jsonSerializerSettings = null;
        public static string Serialize(object b_ObjData,bool ignoreDefaultValue = false)
        {
            if (b_ObjData is null)
                throw new ArgumentNullException($"=>{nameof(b_ObjData)}");
            if(ignoreDefaultValue)
            {
                if (jsonSerializerSettings == null)
                {
                    jsonSerializerSettings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
                }
                return JsonConvert.SerializeObject(b_ObjData, jsonSerializerSettings);
            }
            else
            {
                return JsonConvert.SerializeObject(b_ObjData);
            }
        }

        public static T DeSerialize<T>(string b_StrData)
        {
            if (b_StrData is null)
                throw new ArgumentNullException($"=>{nameof(b_StrData)}");

            return JsonConvert.DeserializeObject<T>(b_StrData);
        }
        public static object DeSerialize(string b_StrData, Type b_DataType)
        {
            if (b_StrData is null)
                throw new ArgumentNullException($"=>{nameof(b_StrData)}");

            return JsonConvert.DeserializeObject(b_StrData, b_DataType);
        }
    }
}
