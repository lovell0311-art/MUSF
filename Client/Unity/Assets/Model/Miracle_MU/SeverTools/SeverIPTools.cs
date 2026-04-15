using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETModel
{
    public enum E_SeverIP
    {
        测试=0,
        外网,
        小卞,
        小刘,
        道长,
        
    }
    public enum RunType
    {
        Mono=0,//mono模式
        ILRuntime,//IL模式
    }
    /// <summary>
    /// 服务器 工具 测试使用
    /// </summary>

    public class SeverIPTools : MonoBehaviour
    {
        private const string LocalServerIp = "10.0.2.2:10002";

        public static SeverIPTools Instance;

        public string ServerIp = LocalServerIp;

        public string ServerIp_测试 = LocalServerIp;
        public string ServerIp_外网 = LocalServerIp;
        public string ServerIp_小卞 = LocalServerIp;
        public string ServerIp_小刘 = LocalServerIp;
        public string ServerIp_道长 = LocalServerIp;
     

        public E_SeverIP 服务器 = E_SeverIP.外网;

        
       // public RunType RunType = RunType.Mono;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            ServerIp = ServerIp_测试;

            switch (服务器)
            {
                case E_SeverIP.测试:
                    ServerIp = ServerIp_测试;
                    break;
                case E_SeverIP.外网:
                    ServerIp = ServerIp_外网;
                    break;
                case E_SeverIP.小卞:
                    ServerIp = ServerIp_小卞;
                    break;
                case E_SeverIP.小刘:
                    ServerIp = ServerIp_小刘;
                    break;
                case E_SeverIP.道长:
                    ServerIp = ServerIp_道长;
                    break;
               
                default:
                    break;
            }
        }
    }
}
