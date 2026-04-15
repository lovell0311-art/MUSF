using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


#if UNITY_EDITOR_WIN
public class NPCCreatPointJson : MonoBehaviour
{ //public
    [Header("地图大小")]
    public int GridX = 350;//格子X
    public int GridZ = 350;//格子Z
    public Text GridText;
    [Header("滚轮缩放比例")]
    public float SizeScale = 0.3f;
    public float SizeScaleDis = 80;


    private Transform quaPlane;
    private Camera mainCamera;
    private bool isMousePress;
    private Vector3 mouseLostPos;
    private float GridHeight = 5;
    public float width = 2f;   

    public Dropdown monsterDropdown;
    private long currenMonsterIndex = 0;
    private string curNPCResName;
    private string npcjsonPath = "../SpawnPoints/Excle2Json/NPC_InfoConfig.txt";//NPCJson路径
    public SearchBar searchBar;

    private Dictionary<Vector2, SpawnPoint> monsterPointsDic = new Dictionary<Vector2, SpawnPoint>();

    private Dictionary<long, string> NPCResDic = new Dictionary<long, string>();

    public Dictionary<Vector2, GameObject> NPCObjDic = new Dictionary<Vector2, GameObject>();

    public Button savaBtn;


    void Start()
    {
        NPCResDic.Clear();
        NPCObjDic.Clear();

        savaBtn.onClick.AddListener(SaveFile);
        string currSceneName = SceneManager.GetActiveScene().name;
        //加载 该场景的格子信息
        if (File.Exists(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt"))
        {
            string text = File.ReadAllText(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt", System.Text.Encoding.UTF8);
            if (!string.IsNullOrEmpty(text))
            {
                NavGridData data = JsonMapper.ToObject<NavGridData>(text);
                GridX = data.Width;
                GridZ = data.Height;
            }
        }

        if (mainCamera == null)
        {
            mainCamera = new GameObject("mainCamera").AddComponent<Camera>();
            mainCamera.orthographic = true;
            mainCamera.transform.localEulerAngles = Vector3.right * 90;
            mainCamera.transform.position = Vector3.up * 90;
            mainCamera.orthographicSize = 100;
            GridHeight = 45;
            quaPlane = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            quaPlane.localEulerAngles = Vector3.right * 90;
            quaPlane.position = new Vector3(mainCamera.transform.position.x, 90, mainCamera.transform.position.z);
            quaPlane.gameObject.GetComponent<Renderer>().enabled = false;
            quaPlane.localScale = new Vector3(200, 100, 1);
        }



        InitDropDown(npcjsonPath);
        InitMonsterSpanwPoints();


    }



    /// <summary>
    /// 初始化当前 场景的怪物出生点
    /// </summary>
    private void InitMonsterSpanwPoints()
    {
        string currSceneName = SceneManager.GetActiveScene().name + "_NPCSpawnPoints";
        string path = "../SpawnPoints/NpcSpawnPoint/" + currSceneName + ".json";//当前场景 怪物出生点的 Json 配置
        if (File.Exists(path))
        {
            monsterPointsDic.Clear();
            string text = File.ReadAllText(path, System.Text.Encoding.UTF8);
            if (!string.IsNullOrEmpty(text))
            {
                SpawnPoint[] data = JsonMapper.ToObject<SpawnPoint[]>(text);
                foreach (var item in data)
                {
                    monsterPointsDic[new Vector2(item.PositionX, item.PositionY)] = item;

                    if (NPCResDic.ContainsKey(item.Index))
                    {
                        //加载NPC资源
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Bundles/NPC/{NPCResDic[item.Index]}.prefab");
                        GameObject npc = Instantiate<GameObject>(prefab);

                        npc.transform.position = new Vector3(item.PositionX * width, 0, item.PositionY * width);
                        //npc.transform.position = new Vector3(item.PositionX, 0, item.PositionY);
                        npc.transform.rotation = Quaternion.Euler(new Vector3(0, item.Direction, 0));
                        NPCObjDic[new Vector2(item.PositionX, item.PositionY)] = npc;
                    }
                }
            }
        }
    }

    public void InitDropDown(string fillePath)
    {

        List<string> libinfo = new List<string>();
        LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(GetJsonFile(fillePath));
        foreach (JsonData item in jsonData)
        {
            Dropdown.OptionData tempData = new Dropdown.OptionData();
            JsonData id_data = item["Id"];//通过字符串 索引取得键值对的值
            JsonData name_data = item["Name"];//通过字符串 索引取得键值对的值
            tempData.text = $"[{id_data}]{name_data}";
            NPCResDic[long.Parse(System.Text.RegularExpressions.Regex.Replace(tempData.text, @"[^0-9]+", ""))] = item["ResName"].ToString();
            monsterDropdown.options.Add(tempData);
           
            libinfo.Add(tempData.text);
        }
        monsterDropdown.onValueChanged.AddListener(delegate
        {
            Debug.Log(monsterDropdown.options[monsterDropdown.value].text);
            string strs = monsterDropdown.options[monsterDropdown.value].text;
            strs = System.Text.RegularExpressions.Regex.Replace(strs, @"[^0-9]+", "");
            currenMonsterIndex = int.Parse(strs);
        });
        searchBar.SetLibraryList(libinfo);
    }

    private void OnDestroy()
    {
      //  SaveFile();
    }

    public void SavaNPc() 
    {
        try
        {
            foreach (var item in NPCObjDic)
            {
                if (!monsterPointsDic.ContainsKey(item.Key)) continue;
                SpawnPoint spawnPoint = monsterPointsDic[item.Key];
                spawnPoint.Direction = (int)item.Value.transform.eulerAngles.y;
                if (spawnPoint.PositionX != item.Value.transform.position.x/2)
                    spawnPoint.PositionX = (int)item.Value.transform.position.x/2;
                if (spawnPoint.PositionY != item.Value.transform.position.z/2)
                    spawnPoint.PositionY = (int)item.Value.transform.position.z/2;
            }
        }
        catch (System.Exception)
        {

          
        }
     
    }

    public void SaveFile()
    {
        SavaNPc();
        string currSceneName = SceneManager.GetActiveScene().name + "_NPCSpawnPoints";
        string text = JsonMapper.ToJson(monsterPointsDic.Values.ToList());
        string path = "../SpawnPoints/NpcSpawnPoint/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        //File.WriteAllText(path + currSceneName + ".json", text, System.Text.Encoding.UTF8); //保存 当前的怪物出生点信息
        File.WriteAllText(path + currSceneName + ".json", text, new UTF8Encoding(false)); //保存 当前的怪物出生点信息

        Debug.Log("<color=#00FF00>保存完毕</color>");
    }

    void Update()
    {
        //判断是否点击到了UI上
        if (EventSystem.current.IsPointerOverGameObject())
        {

            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (currenMonsterIndex == 0)
            {
                Debug.LogError("请先选择一种NPC");
                return;
            }
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);
            if (hit.collider == null)
            {
                Debug.Log("点击目标为空");
                return;

            }
            Vector3 pos = hit.point;
            int currX = (int)(pos.x / width);
            int currZ = (int)(pos.z / width);
            Vector2 vector2 = new Vector2(currX, currZ);

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("currenMonsterIndex:" + currenMonsterIndex);
                monsterPointsDic[vector2] = new SpawnPoint { Index = currenMonsterIndex, PositionX = currX, PositionY = currZ };
                if (NPCResDic.ContainsKey(currenMonsterIndex))
                {
                    //加载NPC资源
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Bundles/NPC/{NPCResDic[currenMonsterIndex]}.prefab");
                    GameObject npc = Instantiate<GameObject>(prefab);

                    npc.transform.position = new Vector3(currX * width, 0, currZ * width);
                    npc.transform.rotation = Quaternion.identity;
                    NPCObjDic[vector2] = npc;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (NPCObjDic.ContainsKey(vector2)) 
                {
                    DestroyImmediate(NPCObjDic[vector2]);
                }
                if (monsterPointsDic.ContainsKey(vector2))
                {
                    monsterPointsDic.Remove(vector2);
                }
            }
            GridText.text = "选择坐标：(" + currX + "," + currZ + ")";
        }
        float xw = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * SizeScaleDis;
        if (xw != 0)
        {
            mainCamera.orthographicSize -= xw;
            Vector3 pos = mainCamera.gameObject.transform.localPosition;
            pos.y -= xw * SizeScale;
            GridHeight = pos.y * 0.5f;
            mainCamera.gameObject.transform.localPosition = pos;
        }
        if (Input.GetMouseButtonDown(2))
        {
            mouseLostPos = Input.mousePosition;
            isMousePress = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isMousePress = false;
        }
        if (isMousePress)
        {
            Vector3 dir = Input.mousePosition - mouseLostPos;
            mouseLostPos = Input.mousePosition;
            dir.z = dir.y;
            dir.y = 0;
            mainCamera.transform.position -= dir * Time.deltaTime * 2;
            quaPlane.position -= dir * Time.deltaTime * 2;
        }
    }
    private void OnDrawGizmos()
    {
        for (int y = 0; y < GridZ; y++)
        {
            for (int x = 0; x < GridX; x++)
            {

                if (monsterPointsDic.ContainsKey(new Vector2(x, y)))
                {
                    Gizmos.color = new Color(67, 0, 0, 100 / 255f);
                    Gizmos.DrawCube(new Vector3(width * 0.5f + x * width, GridHeight, width * 0.5f + y * width), new Vector3(width, 0, width));
                }
                else
                {
                    Gizmos.color = new Color(1, 1, 1, 10 / 255f);
                    Gizmos.DrawWireCube(new Vector3(width * 0.5f + x * width, GridHeight, width * 0.5f + y * width), new Vector3(width, 0, width));
                }

            }
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(NPCCreatPointJson))]
    public class MonsterCreatPointJsonHelper : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("保存导出"))
            {
                var component = target as NPCCreatPointJson;
                component.SaveFile();
            }
        }
    }
#endif
    #region Json
    /// <summary>
    /// 获取到本地的Json文件并且解析返回对应的json字符串
    /// </summary>
    /// <param name="filepath">文件路径</param>
    /// <returns></returns>
    private string GetJsonFile(string filepath)
    {
        string json = string.Empty;
        using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                json = sr.ReadToEnd().ToString();
            }
        }
        return json;
    }
    #endregion
}
#endif


