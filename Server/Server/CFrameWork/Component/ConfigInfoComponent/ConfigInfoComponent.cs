using CustomFrameWork.Baseic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// 配置工具组件 全局唯一
    /// </summary>
    public class ConfigInfoComponent : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 配置数据缓存
        /// </summary>
        private readonly Dictionary<string, string> mConfigInfoDic = new Dictionary<string, string>();
        /// <summary>
        /// 获取配置数据 无key值为null
        /// </summary>
        /// <param name="b_ConfigKey">key</param>
        /// <returns>Value</returns>
        public string GetConfigInfo(string b_ConfigKey)
        {
            if (mConfigInfoDic.TryGetValue(b_ConfigKey, out string ConfigValue))
                return ConfigValue;
            return null;
        }
        /// <summary>
        /// 初始化 缓存数据
        /// </summary>
        public override void Awake()
        {
            mConfigInfoDic.Clear();

            XmlDocument xmldoc = new XmlDocument();
            string mData = File.ReadAllText($"{AppDomain.CurrentDomain.FriendlyName}.dll.config");
            xmldoc.LoadXml(mData);

            XmlNodeList mRootXmlNodeList = xmldoc.ChildNodes;
            for (int i = 0, len = mRootXmlNodeList.Count; i < len; i++)
            {
                XmlNode mRootXmlNode = mRootXmlNodeList[i];
                if (mRootXmlNode.NodeType == XmlNodeType.Element)
                {
                    if (mRootXmlNode.HasChildNodes)
                    {
                        XmlNodeList mChildXmlNodeList = mRootXmlNode.ChildNodes;
                        for (int j = 0, jlen = mChildXmlNodeList.Count; j < jlen; j++)
                        {
                            XmlNode mChildXmlNode = mChildXmlNodeList[j];
                            if (mChildXmlNode.HasChildNodes)
                            {
                                XmlNodeList mChildChildXmlNodeList = mChildXmlNode.ChildNodes;
                                for (int x = 0, xlen = mChildChildXmlNodeList.Count; x < xlen; x++)
                                {
                                    XmlNode mChildChildChildXmlNode = mChildChildXmlNodeList[x];
                                    if (mChildChildChildXmlNode.NodeType == XmlNodeType.Element)
                                    {
                                        XmlElement mElement = (XmlElement)mChildChildChildXmlNode;
                                        if (mElement.Name == "add" && mElement.HasAttributes)
                                        {
                                            if (mElement.HasAttribute("key"))
                                            {
                                                string key = mElement.GetAttribute("key");
                                                string value = mElement.GetAttribute("value");
                                                mConfigInfoDic[key] = value;
                                            }
                                            else if (mElement.HasAttribute("name"))
                                            {
                                                string name = mElement.GetAttribute("name");
                                                string connectionString = mElement.GetAttribute("connectionString");
                                                mConfigInfoDic[name] = connectionString;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 清理缓存配置数据
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;
            mConfigInfoDic.Clear();
            base.Dispose();
        }
    }
}
