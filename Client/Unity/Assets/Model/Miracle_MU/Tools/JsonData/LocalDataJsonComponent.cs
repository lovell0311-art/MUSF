using UnityEngine;
using System.IO;

namespace ETModel
{
    /// <summary>
    /// 序列化 和反序列化Json时 使用的是那种方案
    /// </summary>
    public enum JsonType
    {
        JsonUtlity,
        ListJson,
    }

    /// <summary>
    /// Json 数据管理类
    /// 主要用于进行 Json的序列化存储到本地硬盘 和反序列化从本地硬盘读取到内存
    /// </summary>
    public class LocalDataJsonComponent
    {

        private static LocalDataJsonComponent instance = new LocalDataJsonComponent();

        public static LocalDataJsonComponent Instance => instance;
        private LocalDataJsonComponent() { }

        /// <summary>
        /// 序列化 存储josn数据
        /// </summary>
        /// <param name="data">需要存储的数据</param>
        /// <param name="fileName">文件名</param>
        /// <param name="type"></param>
        public void SavaData(object data, string fileName, JsonType type = JsonType.ListJson)
        {

            //确定存储路径
            string path = Path.Combine(Application.persistentDataPath, $"LocalJsonData_{fileName}.json");
            //序列化 得到json字符串
            string jsonStr = "";
            switch (type)
            {
                case JsonType.JsonUtlity:
                    jsonStr = JsonUtility.ToJson(data);
                    break;
                case JsonType.ListJson:
                    jsonStr = JsonHelper.ToJson(data);
                    break;
            }
            //把序列好的Json字符串 存储到指定的路径文件中
            File.WriteAllText(path, jsonStr);
            
        }
        /// <summary>
        /// 读取指定文件夹中的Json数据 反序列化
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public T LoadData<T>(string fileName, JsonType type = JsonType.ListJson) where T : new()
        {
            //确定从那个路径读取
            string path = Path.Combine(Application.persistentDataPath, $"LocalJsonData_{fileName}.json");
            //先判断 是否存在这个文件
            //如果 不存在默认文件 
            if (!File.Exists(path))
            {
                return new T();
            }
            //进行反序列化
            string jsonStr = File.ReadAllText(path);
            //数据对象
            T data = default(T);
            switch (type)
            {
                case JsonType.JsonUtlity:
                    data = JsonUtility.FromJson<T>(jsonStr);
                    break;
                case JsonType.ListJson:
                    data = JsonHelper.FromJson<T>(jsonStr);
                    break;

            }
            //把对象返回出去
            return data;
        }
    }
}
