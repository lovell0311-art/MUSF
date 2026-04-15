using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//Object并非C#基础中的Object，而是 UnityEngine.Object
using Object = UnityEngine.Object;
using System.Linq;
using ETModel;
using UnityEngine.UI;
using UnityEngine.U2D;
//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof (ReferenceCollector))]
//没有该属性的编辑器在选中多个物体时会提示“Multi-object editing not supported”
[CanEditMultipleObjects]
public class ReferenceCollectorEditor: Editor
{
    //输入在textfield中的字符串
    private string searchKey
	{
		get
		{
			return _searchKey;
		}
		set
		{
			if (_searchKey != value)
			{
				_searchKey = value;
				heroPrefab = referenceCollector.Get<Object>(searchKey);
			}
		}
	}

	private ReferenceCollector referenceCollector;
	//引用类型标签的颜色
	private static Color32 lableColor = new Color32(127, 191, 55, 255);
	private Object heroPrefab;

	private string _searchKey = "";

	private void DelNullReference()
	{
		var dataProperty = serializedObject.FindProperty("data");
		for (int i = dataProperty.arraySize - 1; i >= 0; i--)
		{
			var gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
			if (gameObjectProperty.objectReferenceValue == null)
			{
				dataProperty.DeleteArrayElementAtIndex(i);
			}
		}
	}

	private void OnEnable()
	{
        //将被选中的gameobject所挂载的ReferenceCollector赋值给编辑器类中的ReferenceCollector，方便操作
        referenceCollector = (ReferenceCollector) target;
	}

	public override void OnInspectorGUI()
	{
        //使ReferenceCollector支持撤销操作，还有Redo，不过没有在这里使用
        Undo.RecordObject(referenceCollector, "Changed Settings");
		var dataProperty = serializedObject.FindProperty("data");
        //开始水平布局，如果是比较新版本学习U3D的，可能不知道这东西，这个是老GUI系统的知识，除了用在编辑器里，还可以用在生成的游戏中
		GUILayout.BeginHorizontal();
        //下面几个if都是点击按钮就会返回true调用里面的东西
		if (GUILayout.Button("添加引用"))
		{
            //添加新的元素，具体的函数注释
            // Guid.NewGuid().GetHashCode().ToString() 就是新建后默认的key
            AddReference(dataProperty, Guid.NewGuid().GetHashCode().ToString(), null);
		}
		//if (GUILayout.Button("全部删除"))
		//{
		//	//dataProperty.ClearArray();
		//}
		//if (GUILayout.Button("删除空引用"))
		//{
		//	//DelNullReference();
		//}
		if (GUILayout.Button("排序"))
		{
			referenceCollector.Sort();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
        //可以在编辑器中对searchKey进行赋值，只要输入对应的Key值，就可以点后面的删除按钮删除相对应的元素
        searchKey = EditorGUILayout.TextField(searchKey);
        //添加的可以用于选中Object的框，这里的object也是(UnityEngine.Object
        //第三个参数为是否只能引用scene中的Object
        EditorGUILayout.ObjectField(heroPrefab, typeof (Object), false);
		if (GUILayout.Button("删除"))
		{
			referenceCollector.Remove(searchKey);
			heroPrefab = null;
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		var delList = new List<int>();
        SerializedProperty property;
		string[] enumNames = Enum.GetNames(typeof(MonoReferenceType));
		foreach (var item in enumNames)
		{
			bool isCantains = false;
			int enumIndex = (int)((MonoReferenceType)Enum.Parse(typeof(MonoReferenceType), item));
			//遍历ReferenceCollector中data list的所有元素，显示在编辑器中
			for (int i = referenceCollector.data.Count - 1; i >= 0; i--)
			{
				property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("type");
				if (property.enumValueIndex != enumIndex) continue;

				if (isCantains == false)
				{
					isCantains = true;
					GUILayout.Label(item, new GUIStyle() { fontSize = 16, normal = new GUIStyleState() { textColor = lableColor } });
				}

				GUILayout.BeginHorizontal();
				//这里的知识点在ReferenceCollector中有说
				property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key");
				//
				property.stringValue = EditorGUILayout.TextField(property.stringValue, GUILayout.ExpandWidth(true));
				//EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
				property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
				EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(Object), true);
				if (GUILayout.Button("X"))
				{
					//将元素添加进删除list
					delList.Add(i);
				}
				GUILayout.EndHorizontal();
			}
		}
		var eventType = Event.current.type;
        //在Inspector 窗口上创建区域，向区域拖拽资源对象，获取到拖拽到区域的对象
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
		{
			// Show a copy icon on the drag
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

			if (eventType == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();
				foreach (var o in DragAndDrop.objectReferences)
				{
                    var result = from item in referenceCollector.data
                                 where item.key == o.name
                                 select item;
                    if (result.Count() > 0)
                    {
                        Log.Error($"添加对象引用重复 key:{o.name}");
                        continue;
                    }

                    AddReference(dataProperty, o.name, o);
				}
			}

			Event.current.Use();
		}

        //遍历删除list，将其删除掉
		foreach (var i in delList)
		{
			dataProperty.DeleteArrayElementAtIndex(i);
		}
		serializedObject.ApplyModifiedProperties();
		serializedObject.UpdateIfRequiredOrScript();
	}

    //添加元素，具体知识点在ReferenceCollector中说了
    private void AddReference(SerializedProperty dataProperty, string key, Object obj)
	{
		int index = dataProperty.arraySize;
		dataProperty.InsertArrayElementAtIndex(index);
		var element = dataProperty.GetArrayElementAtIndex(index);
		element.FindPropertyRelative("key").stringValue = key;

		MonoReferceTemp temp = GetMonoReferenceTypeIndex(obj);
		element.FindPropertyRelative("type").enumValueIndex = temp.EnumIndex;
		element.FindPropertyRelative("gameObject").objectReferenceValue = temp.obj;

	}
	private MonoReferceTemp GetMonoReferenceTypeIndex(Object obj)
	{
		if (obj as GameObject)
		{
			GameObject go = (GameObject)obj;
			if (go.GetComponent<InputField>() != null)
			{
				return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.InputField, obj = go.GetComponent<InputField>() };
			}
			if (go.GetComponent<Toggle>() != null)
			{
				return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.Toggle, obj = go.GetComponent<Toggle>() };
			}
			if (go.GetComponent<Button>() != null)
			{
				return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.Button, obj = go.GetComponent<Button>() };
			}
			if (go.GetComponent<Text>() != null)
			{
				return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.Text, obj = go.GetComponent<Text>() };
			}
			if (go.GetComponent<Image>() != null)
			{
				return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.Image, obj = go.GetComponent<Image>() };
			}
		}

		if (obj is Sprite)
		{
			return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.Sprite, obj = obj };
		}

		if (obj is TextAsset)
		{
			return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.TextAsset, obj = obj };
		}

		if (obj is AudioClip)
		{
			return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.AudioClip, obj = obj };
		}
		if (obj is SpriteAtlas)
		{
			return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.SpriteAtlas, obj = obj };
		}
		if (obj is RenderTexture)
		{
			return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.RenderTexture, obj = obj };
		}
		if (obj is AnimationClip)
		{
			return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.AnimationClip, obj = obj };
		}
		

		return new MonoReferceTemp() { EnumIndex = (int)MonoReferenceType.Object, obj = obj };
	}
}
public struct MonoReferceTemp
{
	public int EnumIndex;
	[SerializeField]
	public UnityEngine.Object obj;
}