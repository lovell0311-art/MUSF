using ETModel;
using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 红点管理器
    /// </summary>
    public static class RedDotManagerComponent
    {
        #region 红点管理器

        public static Dictionary<string, int> redDotData = new Dictionary<string, int>();
        private static RedDotSystem m_RedDotManager;
        /// <summary>
        /// 红点管理器
        /// </summary>
        public static RedDotSystem RedDotManager
        {
            get
            {
                //通常放在项目的初始化逻辑中，此处只是demo临时写法
                if (m_RedDotManager == null)
                    m_RedDotManager = new RedDotSystem();
                return m_RedDotManager;
            }
        }
        /// <summary>
        /// 节点数改变会通知,通知的时候加入字典,直接判断字典节点的数量来显示红点
        /// </summary>
        /// <param name="redDotDefine"></param>
        /// <param name="count"></param>
        public static void SetRedDotCount(string redDotDefine,int count)
        {
            if (redDotData.ContainsKey(redDotDefine))
            {
                redDotData[redDotDefine] = count;
                return;
            }
            redDotData.Add(redDotDefine, count);
        }
        public static int GetRedDotcount(string redDotDefine)
        {
            if (redDotData.ContainsKey(redDotDefine))
            {
                return redDotData[redDotDefine];
            }
            return 0;
        }
        //保存红点信息
        public static void SavalocalRedDot()
        {
            List<RedDotInfo> redDotInfo = new List<RedDotInfo>();
            foreach (var item in redDotData)
            {
            
                RedDotInfo redDotInfo1 = new RedDotInfo()
                {
                    redDotName = item.Key,
                    redDotCount = item.Value
                };
                redDotInfo.Add(redDotInfo1);
            }
            redDotData.Clear();
            string path = $"{RoleArchiveInfoManager.Instance.LoadRoleUUID}_localRedDot";
            LocalDataJsonComponent.Instance.SavaData(redDotInfo, path);
        }

        public static Dictionary<string, int> LoadlocalRedDot()
        {
            List<RedDotInfo> redDotInfo = LocalDataJsonComponent.Instance.LoadData<List<RedDotInfo>>($"{RoleArchiveInfoManager.Instance.LoadRoleUUID}_localRedDot");
            if (redDotInfo != null)
            {
                foreach (var item in redDotInfo)
                {
                  
                    RedDotManager.Set(item.redDotName, item.redDotCount);
                }
            }
            else
            {
                redDotData.Clear();
               
            }
            
            return redDotData;
        }
        #endregion
    }

    /// <summary>
    /// 红点路径定义
    /// </summary>
    public static class E_RedDotDefine
    {
        /// <summary>
        /// 红点树的根节点
        /// </summary>
        public const string rdRoot = "Root";


        // ---------- 业务红点 ----------

        public const string Root_Friend = "Root_Friend";
        public const string Root_Friend_FriendList = "Root_Friend_FriendList";
        public const string Root_Friend_FriendEnemy = "Root_Friend_FriendEnemy";
        public const string Root_Friend_AddFirend = "Root_Friend_AddFirend";
        public const string Root_Friend_AddFirend_FirendApply = "Root_Friend_AddFirend_FirendApply";

        public const string Root_WarAlliance = "Root_WarAlliance";
        public const string Root_WarAlliance_WarApply = "Root_WarAlliance_WarApply";

        public const string Root_Email = "Root_Email";


        public const string Root_Attribute = "Root_Attribute";
        //public const string Root_Attribute_AddPoint = "Root_Attribute_AddPoint";

        public const string Root_Pet = "Root_Pet";

    }

    /// <summary>
    /// 红点系统
    /// </summary>
    public class RedDotSystem
    {
        public RedDotSystem()
        {
            InitRedDotTreeNode();
        }

        /// <summary>
        /// 红点数变化通知委托
        /// </summary>
        /// <param name="node"></param>
        public delegate void OnRdCountChange(RedDotNode node);

        /// <summary>
        /// 红点树的的 Root节点
        /// </summary>
        private RedDotNode mRootNode;

        /// <summary>
        /// 红点路径的表（每次 E_RedDotDefine 添加完后此处也必须添加）
        /// </summary>
        public List<string> lstRedDotTreeList = new List<string>
        {
            E_RedDotDefine.rdRoot,

            E_RedDotDefine.Root_Friend,
            E_RedDotDefine.Root_Friend_FriendList,
            E_RedDotDefine.Root_Friend_FriendEnemy,
            E_RedDotDefine.Root_Friend_AddFirend,
            E_RedDotDefine.Root_Friend_AddFirend_FirendApply,
            E_RedDotDefine.Root_WarAlliance,
            E_RedDotDefine.Root_WarAlliance_WarApply,
            E_RedDotDefine.Root_Email,
            E_RedDotDefine.Root_Attribute,
            //E_RedDotDefine.Root_Attribute_AddPoint,
            E_RedDotDefine.Root_Pet
        };


        #region 内部接口

        /// <summary>
        /// 初始化红点树
        /// </summary>
        private void InitRedDotTreeNode()
        {
            /*
            * 结构层：根据红点是否显示或显示数，自定义红点的表现方式
            */

            mRootNode = new RedDotNode {rdName = E_RedDotDefine.rdRoot};

            foreach (string path in lstRedDotTreeList)
            {
                string[] treeNodeAy = path.Split('_');
                int nodeCount = treeNodeAy.Length;
                RedDotNode curNode = mRootNode;

                if (treeNodeAy[0] != mRootNode.rdName)
                {
                    Debug.LogError("根节点必须为Root，检查 " + treeNodeAy[0]);
                    continue;
                }

                if (nodeCount > 1)
                {
                    for (int i = 1; i < nodeCount; i++)
                    {
                        if (!curNode.rdChildrenDic.ContainsKey(treeNodeAy[i]))
                        {
                            curNode.rdChildrenDic.Add(treeNodeAy[i], new RedDotNode());
                        }

                        curNode.rdChildrenDic[treeNodeAy[i]].rdName = treeNodeAy[i];
                        curNode.rdChildrenDic[treeNodeAy[i]].parent = curNode;

                        curNode = curNode.rdChildrenDic[treeNodeAy[i]];
                    }
                }
            }
            
        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 设置红点数变化的回调
        /// </summary>
        /// <param name="strNode">红点路径，必须是 RedDotDefine </param>
        /// <param name="callBack">回调函数</param>
        public void SetRedDotNodeCallBack(string strNode, OnRdCountChange callBack = null)
        {
            var nodeList = strNode.Split('_');

            if (nodeList.Length == 1)
            {
                if (nodeList[0] != E_RedDotDefine.rdRoot)
                {
                    Debug.LogError("Get Wrong Root Node! current is " + nodeList[0]);
                    return;
                }
            }

            var node = mRootNode;
            for (int i = 1; i < nodeList.Length; i++)
            {
                if (!node.rdChildrenDic.ContainsKey(nodeList[i]))
                {
                    Debug.LogError("Does Not Contain child Node: " + nodeList[i]);
                    return;
                }

                node = node.rdChildrenDic[nodeList[i]];

                if (i == nodeList.Length - 1)
                {
                   
                    node.countChangeFunc = callBack;
                    return;
                }
            }
        }

        /// <summary>
        /// 设置红点参数
        /// </summary>
        /// <param name="nodePath">红点路径，必须走 RedDotDefine </param>
        /// <param name="rdCount">红点计数</param>
        public void Set(string nodePath, int rdCount = 1)
        {
            string[] nodeList = nodePath.Split('_');

            if (nodeList.Length == 1)
            {
                if (nodeList[0] != E_RedDotDefine.rdRoot)
                {
                    Debug.Log("Get Wrong RootNod！ current is " + nodeList[0]);
                    return;
                }
            }

            //遍历子红点
            RedDotNode node = mRootNode;
            for (int i = 1; i < nodeList.Length; i++)
            {
                //父红点的 子红点字典表 内，必须包含
                if (node.rdChildrenDic.ContainsKey(nodeList[i]))
                {
                    node = node.rdChildrenDic[nodeList[i]];

                    //设置叶子红点的红点数
                    if (i == nodeList.Length - 1)
                    {
                        node.SetRedDotCount(Math.Max(0, rdCount));
                        RedDotManagerComponent.SetRedDotCount(nodePath, rdCount);
                    }
                }
                else
                {
                    Debug.LogError($"{node.rdName}的子红点字典内无 Key={nodeList[i]}, 检查 RedDotSystem.InitRedDotTreeNode()");
                    return;
                }
            }
        }

        /// <summary>
        /// 获取红点的计数
        /// </summary>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public int GetRedDotCount(string nodePath)
        {
            string[] nodeList = nodePath.Split('_');

            int count = 0;
            if (nodeList.Length >= 1)
            {
                //遍历子红点
                RedDotNode node = mRootNode;
                for (int i = 1; i < nodeList.Length; i++)
                {
                    //父红点的 子红点字典表 内，必须包含
                    if (node.rdChildrenDic.ContainsKey(nodeList[i]))
                    {
                        node = node.rdChildrenDic[nodeList[i]];

                        if (i == nodeList.Length - 1)
                        {
                            count = node.rdCount;
                            break;
                        }
                    }
                }
            }

            return count;
        }

        #endregion
    }
}