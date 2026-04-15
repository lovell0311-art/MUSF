using System;
using System.Collections.Generic;
using UnityEngine;
//Object并非C#基础中的Object，而是 UnityEngine.Object
using Object = UnityEngine.Object;
using ETModel;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.U2D;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor;

#endif
[SerializeField]
public enum MonoReferenceType : byte
{
    Object = 0,
    Image,
    Text,
    Button,
    Toggle,
    InputField,
    Sprite,
    TextAsset,
    AudioClip,
    SpriteAtlas,
    RenderTexture,
    AnimationClip
}
//使其能在Inspector面板显示，并且可以被赋予相应值
[Serializable]
public class ReferenceCollectorData
{
    
    public MonoReferenceType type;
    public string key;
    //Object并非C#基础中的Object，而是 UnityEngine.Object
    [SerializeField]
    public UnityEngine.Object gameObject;
}
//继承IComparer对比器，Ordinal会使用序号排序规则比较字符串，因为是byte级别的比较，所以准确性和性能都不错
public class ReferenceCollectorDataComparer : IComparer<ReferenceCollectorData>
{
    public int Compare(ReferenceCollectorData x, ReferenceCollectorData y)
    {
        return string.Compare(x.key, y.key, StringComparison.Ordinal);
    }
}

//继承ISerializationCallbackReceiver后会增加OnAfterDeserialize和OnBeforeSerialize两个回调函数，如果有需要可以在对需要序列化的东西进行操作
//ET在这里主要是在OnAfterDeserialize回调函数中将data中存储的ReferenceCollectorData转换为dict中的Object，方便之后的使用
//注意UNITY_EDITOR宏定义，在编译以后，部分编辑器相关函数并不存在
public class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver
{
    //用于序列化的List
    public List<ReferenceCollectorData> data = new List<ReferenceCollectorData>();
    //Object并非C#基础中的Object，而是 UnityEngine.Object
    private readonly Dictionary<MonoReferenceType, Dictionary<string, UnityEngine.Object>> dict = new Dictionary<MonoReferenceType, Dictionary<string, UnityEngine.Object>>();

#if UNITY_EDITOR
    //添加新的元素
    public void Add(string key, Object obj)
    {
        SerializedObject serializedObject = new SerializedObject(this);
        //根据PropertyPath读取数据
        //如果不知道具体的格式，可以右键用文本编辑器打开一个prefab文件（如Bundles/UI目录中的几个）
        //因为这几个prefab挂载了ReferenceCollector，所以搜索data就能找到存储的数据
        SerializedProperty dataProperty = serializedObject.FindProperty("data");
        int i;
        //遍历data，看添加的数据是否存在相同key
        for (i = 0; i < data.Count; i++)
        {
            if (data[i].key == key)
            {
                break;
            }
        }
        //不等于data.Count意为已经存在于data List中，直接赋值即可
        if (i != data.Count)
        {
            //根据i的值获取dataProperty，也就是data中的对应ReferenceCollectorData，不过在这里，是对Property进行的读取，有点类似json或者xml的节点
            SerializedProperty element = dataProperty.GetArrayElementAtIndex(i);
            //对对应节点进行赋值，值为gameobject相对应的fileID
            //fileID独一无二，单对单关系，其他挂载在这个gameobject上的script或组件会保存相对应的fileID
            element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
        }
        else
        {
            //等于则说明key在data中无对应元素，所以得向其插入新的元素
            dataProperty.InsertArrayElementAtIndex(i);
            SerializedProperty element = dataProperty.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("key").stringValue = key;
            element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
        }
        //应用与更新
        EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }
    //删除元素，知识点与上面的添加相似
    public void Remove(string key)
    {
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty dataProperty = serializedObject.FindProperty("data");
        int i;
        for (i = 0; i < data.Count; i++)
        {
            if (data[i].key == key)
            {
                break;
            }
        }
        if (i != data.Count)
        {
            dataProperty.DeleteArrayElementAtIndex(i);
        }
        EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    public void Clear()
    {
        SerializedObject serializedObject = new SerializedObject(this);
        //根据PropertyPath读取prefab文件中的数据
        //如果不知道具体的格式，可以直接右键用文本编辑器打开，搜索data就能找到
        var dataProperty = serializedObject.FindProperty("data");
        dataProperty.ClearArray();
        EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    public void Sort()
    {
        SerializedObject serializedObject = new SerializedObject(this);
        data.Sort(new ReferenceCollectorDataComparer());
        EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

#endif
    //使用泛型返回对应key的gameobject
    public T Get<T>(string key) where T : class
    {
        if (dict.TryGetValue(MonoReferenceType.Object, out Dictionary<string, Object> value))
        {
            if (value.TryGetValue(key, out Object obj))
            {
                return obj as T;
            }
        }

        //Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");

        return null;
    }
#region 获得组件
    /// <summary>
    /// 获得GameObject
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public GameObject GetGameObject(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.Object, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as GameObject;
            }
        }
        if (this.gameObject.name == "UI"||this.gameObject.name== "World"|| this.gameObject.name == "Back")
            return null;

        //Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;

    }
    public Image GetImage(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.Image, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as Image;
            }
        }
       // Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
    public Text GetText(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.Text, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as Text;
            }
        }
      //  Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
    public Button GetButton(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.Button, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
            
                return obj as Button;
            }
        }

       // Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
    public Toggle GetToggle(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.Toggle, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as Toggle;
            }
        }
      //  Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
    public InputField GetInputField(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.InputField, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as InputField;
            }
        }

       // Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");

        return null;
    }
    public Sprite GetSprite(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.Sprite, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as Sprite;
            }
        }
        Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
    public SpriteAtlas GetSpriteAtlas(string key)
    {
        
        if (dict.TryGetValue(MonoReferenceType.SpriteAtlas, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as SpriteAtlas;
            }
        }
       // Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    } 
    public RenderTexture GetRenderTexture(string key)
    {
        
        if (dict.TryGetValue(MonoReferenceType.RenderTexture, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as RenderTexture;
            }
        }
       // Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
    public TextAsset GetTextAsset(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.TextAsset, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as TextAsset;
            }
        }
       // Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    } 
    public AnimationClip GetAnimationClip(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.AnimationClip, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as AnimationClip;
            }
        }
      //  Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
   
    public UnityEngine.AudioClip GetAudioClip(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.AudioClip, out Dictionary<string, UnityEngine.Object> value))
        {
            if (value.TryGetValue(key, out UnityEngine.Object obj))
            {
                return obj as AudioClip;
            }
        }
      //  Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
    public Object[] GetAssets(MonoReferenceType monoType)
    {
        if (dict.TryGetValue(monoType, out Dictionary<string, UnityEngine.Object> value))
        {
           return value.Values.ToArray();
        }
        Log.Error($"资源未赋值 gameobject:{this.gameObject.transform.parent.name}  type:{monoType}");
        return null;
    }
   
    public Object GetObject(string key)
    {
        if (dict.TryGetValue(MonoReferenceType.Object, out Dictionary<string, Object> value))
        {
            if (value.TryGetValue(key, out Object obj))
            {
                return obj as GameObject;
            }
        }
        Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  key:{key}");
        return null;
    }
    public KeyValuePair<string, UnityEngine.Object>[] GetKVAssets(MonoReferenceType monoType)
    {
        if (dict.TryGetValue(monoType, out Dictionary<string, UnityEngine.Object> value))
        {
            return value.ToArray();
        }
        Log.Error($"资源未赋值 gameobject:{this.gameObject.name}  type:{monoType}");
        return null;
    } 
#endregion

    public void OnBeforeSerialize()
    {
    }
    //在反序列化后运行
    public void OnAfterDeserialize()
    {
        dict.Clear();
        foreach (ReferenceCollectorData referenceCollectorData in data)
        {
            if (dict.ContainsKey(referenceCollectorData.type) == false)
            {
                dict.Add(referenceCollectorData.type, new Dictionary<string, UnityEngine.Object>());
            }

            dict.TryGetValue(referenceCollectorData.type, out Dictionary<string, UnityEngine.Object> value);

            if (!value.ContainsKey(referenceCollectorData.key))
            {
                value.Add(referenceCollectorData.key, referenceCollectorData.gameObject);
            }
            else
            {
                Log.Error($"MonoReference 资源重复 : {referenceCollectorData.key}");
            }
        }
    }
}
