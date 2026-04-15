using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ETModel;

namespace ETEditor
{
    [CustomEditor(typeof(SeverIPTools))]//关联 服务器工具脚本
    public class SeverIpEditor : Editor
    {
        private SerializedObject severIp;//序列化

        private SerializedProperty severStr, 服务器, ServerIp_测试, ServerIp_外网, ServerIp_小卞, ServerIp_小刘, ServerIp_道长;

        private SerializedProperty runType;

        private void OnEnable()
        {
            severIp = new SerializedObject(target);
            severStr = severIp.FindProperty("ServerIp");//获取变量 ServerIp
            服务器 = severIp.FindProperty("服务器");//获取变量 服务器
            ServerIp_测试 = severIp.FindProperty("ServerIp_测试");//获取变量 服务器
            ServerIp_外网 = severIp.FindProperty("ServerIp_外网");//获取变量 服务器
            ServerIp_小卞 = severIp.FindProperty("ServerIp_小卞");//获取变量 服务器
            ServerIp_小刘 = severIp.FindProperty("ServerIp_小刘");//获取变量 服务器
            ServerIp_道长 = severIp.FindProperty("ServerIp_道长");//获取变量 服务器
        

            runType = severIp.FindProperty("RunType");
        }
        public override void OnInspectorGUI()
        {
            severIp.Update();//更新severIp
            EditorGUILayout.PropertyField(服务器);
            switch (服务器.enumValueIndex)
            {
                case 0:
                    severStr = ServerIp_测试;
                    break;
                case 1:
                    severStr = ServerIp_外网;
                    break;
                case 2:
                    severStr = ServerIp_小卞;
                    break;
                case 3:
                    severStr = ServerIp_小刘;
                    break;
                case 4:
                    severStr = ServerIp_道长;
                    break;
               


            }
            EditorGUILayout.PropertyField(severStr);

            severIp.ApplyModifiedProperties();


        }

       
    }
}
