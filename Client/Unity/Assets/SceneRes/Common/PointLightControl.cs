using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class PointLightControl : MonoBehaviour
{
    private GameObject m_lightObj;
    private Light m_light;

    private bool IsStartGame = true;
    [SerializeField]
    [Tooltip("闪光强度")]
    private float flashDensity = 1f;
    [SerializeField]
    [Tooltip("闪光间隔")]
    private int flashInterval = 100;
    // public Texture m_texture;
    // Use this for initialization
    [SerializeField]
    [Tooltip("初始强度")]
    private float InitIntensity = 1f;
    void Start()
    {
        //Debug.Log("1111");
        IsStartGame = true;
        m_lightObj = this.gameObject;
        //获取灯光游戏对象的Light组件
        m_light = m_lightObj.GetComponent<Light>();
        m_light.intensity = InitIntensity;
        ShowLight();
    }

    public async void ShowLight()
    {
        while (IsStartGame)
        {
            await Task.Delay(flashInterval);

            //改变灯光的颜色
            //   m_light.color = new Color(1f, 1f, 1f);
            //灯光的烘焙模式
            //LightmapBakeType.Realtime  实时光
            //LightmapBakeType.Baked   烘焙光
            //LightmapBakeType.Mixed  混合光
            //m_light.lightmapBakeType = LightmapBakeType.Mixed;
            //灯光的强度
            float i = Random.Range(0, flashDensity);


            m_light.intensity = InitIntensity + i;
            //设置阴影效果 LightShadows.None  没有阴影效果
            //LightShadows.Soft 软阴影效果
            //LightShadows.Hard  硬阴影效果
            //   m_light.shadows = LightShadows.None;
            //设置Cookie
            //m_light.cookie = m_texture;


        }

    }

    private void OnDestroy()
    {
        IsStartGame = false;

    }
}
