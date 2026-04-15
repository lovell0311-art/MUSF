using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    public static class HttpHelper
    {
        public static Dictionary<string,string> ParseParameters(string body)
        {
            Dictionary<string, string> kv = new Dictionary<string, string>();
            string[] values = body.Split('&');
            foreach(string value in values)
            {
                int pos = value.IndexOf('=');
                if (pos == -1)
                    throw new Exception($"解析参数失败，未知的参数 '{value}'");
                string k = value.Substring(0, pos);
                string v = value.Substring(pos + 1, value.Length - pos - 1);
                kv[k] = v;
            }
            return kv;
        }
    }
}
