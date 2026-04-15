using LitJson;
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
using static UnityEngine.GraphicsBuffer;

#if UNITY_EDITOR

public class TransferPoints
{
    public int Index;//当前场景区域的 ID  Index+100*MapIndex
    public int MapIndex;//当前地图 的 ID
    public int PositionX;//
    public int PositionY;
}

/// <summary>
/// 场景传送点
/// </summary>
public class TransferPointsComponent : MonoBehaviour
{

    [Header("是否是客户端使用")]
    public bool IsClient = false;

    [Header("区域Id")]
    public int AreaIndex;
    [Header("当前所点击的区域Id")]
    public int CurAreaIndex;
    [Header("地图ID")]
    public int MapIndex;//地图ID
    [Header("地图大小")]
    public int GridX = 350;//格子X
    public int GridZ = 350;//格子Z
    public Text GridText;
    [Header("滚轮缩放比例")]
    public float SizeScale = 0.3f;
    public float SizeScaleDis = 80;
    [Header("笔刷大小 *2")]
    public int brushSize = 1;

    private Transform quaPlane;
    private Camera mainCamera;
    private bool isMousePress;
    private Vector3 mouseLostPos;
    private float GridHeight = 5;
    public float width = 2f;



    private Dictionary<Vector2, TransferPoints> MapPointsDic = new Dictionary<Vector2, TransferPoints>();



    public Button savaBtn;
    // Start is called before the first frame update
    void Start()
    {
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
        InitTranferPoints();
    }
    private void InitTranferPoints()
    {
        string currSceneName = SceneManager.GetActiveScene().name + "_TransferPoint";//Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt"
        string path = IsClient==false?"../SpawnPoints/TransferPoints/" + currSceneName + ".json": Application.dataPath + "/Res/TransferPoints/" + currSceneName + ".json";//当前场景 怪物出生点的 Json 配置
        if (File.Exists(path))
        {
            MapPointsDic.Clear();
            string text = File.ReadAllText(path, System.Text.Encoding.UTF8);
            if (!string.IsNullOrEmpty(text))
            {
                TransferPoints[] data = JsonMapper.ToObject<TransferPoints[]>(text);
                foreach (var item in data)
                {
                    MapPointsDic[new Vector2(item.PositionX, item.PositionY)] = item;

                }
            }
        }
    }
    private void OnDestroy()
    {
        SaveFile();
    }



    public void SaveFile()
    {

        string currSceneName = SceneManager.GetActiveScene().name + "_TransferPoint";
        string text = JsonMapper.ToJson(MapPointsDic.Values.ToList());
        string path =IsClient==false?"../SpawnPoints/TransferPoints/": Application.dataPath + "/Res/TransferPoints/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        //File.WriteAllText(path + currSceneName + ".json", text, System.Text.Encoding.UTF8); //保存 当前的怪物出生点信息
        File.WriteAllText(path + currSceneName + ".json", text, new UTF8Encoding(false)); //保存 当前的怪物出生点信息

        Debug.Log("<color=#00FF00>保存完毕</color>");
    }
    // Update is called once per frame
    void Update()
    {
        //判断是否点击到了UI上
        if (EventSystem.current.IsPointerOverGameObject())
        {

            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {

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
                if (MapPointsDic.ContainsKey(vector2))
                {
                    Debug.Log($"MapPointsDic[vector2].Index：{MapPointsDic[vector2].Index}");
                    CurAreaIndex = MapPointsDic[vector2].Index - (100 * MapIndex);
                    return;
                }

                MapPointsDic[vector2] = new TransferPoints { Index = AreaIndex + 100 * MapIndex, MapIndex=MapIndex, PositionX = currX, PositionY = currZ };
               

                if (brushSize != 1)
                {
                    for (int i = -brushSize; i < brushSize; i++)
                    {
                        for (int j = -brushSize; j < brushSize; j++)
                        {
                            MapPointsDic[new Vector2(currX + i, currZ + j)] = new TransferPoints { Index = AreaIndex+100*MapIndex, MapIndex = MapIndex, PositionX = currX + i, PositionY = currZ + j };
                        }
                    }
                }

            }
            else if (Input.GetMouseButtonDown(1))
            {

                if (MapPointsDic.ContainsKey(vector2))
                {
                    MapPointsDic.Remove(vector2);
                }
                if (brushSize != 1)
                {
                    for (int i = -brushSize; i < brushSize; i++)
                    {
                        for (int j = -brushSize; j < brushSize; j++)
                        {
                            if (MapPointsDic.ContainsKey(new Vector2(currX + i, currZ + j)))
                            {
                                MapPointsDic.Remove(new Vector2(currX + i, currZ + j));
                            }

                        }
                    }
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

                if (MapPointsDic.ContainsKey(new Vector2(x, y)))
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

}
#endif
#if UNITY_EDITOR
[CustomEditor(typeof(TransferPointsComponent))]
public class TransferPointsComponentHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("保存导出"))
        {
            var component = target as TransferPointsComponent;
            component.SaveFile();
        }
    }
}
#endif
