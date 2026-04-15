/*************************
 * UVAnimation function
 * LiuYu. 153250945@qq.com
 ************************/
using UnityEngine;

public class UVAnimation : MonoBehaviour
{

    public bool pauseTimer = false;
    [Header("间隔时间：时间戳秒")]
    public float RepeatTime = 5;
    [Header("流光 y轴 初始的位置")]
    public float UVDefautOffset_Y = 0.39f;
    [Header("流光显示 一次需要的时间")]
    public float ShowTimeUV = .4f;
    public bool isRepeat = false;

    public Vector2 uvDefaultTiling = Vector2.one;
    public Vector2 uvDefaultOffset = Vector2.zero;
    public Vector2 uvOffsetSpeed = Vector2.zero;
    public Color matDefaultColor = Color.white;

    private Renderer _renderer;
    private Material _mat;
    Vector2 m_offset = new Vector2();
    Vector2 m_tiling = new Vector2();

    // Use this for initialization
    void Awake()
    {
        Renderer r = GetComponent<Renderer>();
        if (r)
        {
            _renderer = r;
        }
        else
        {
            enabled = false;
        }
        if (_renderer != null && _renderer.material != null)
        {
            _renderer.material.SetColor("_TintColor", matDefaultColor);
        }
    }

    void Start()
    {
        if (_renderer)
        {
            _mat = _renderer.material;
            m_offset = uvDefaultOffset;
            m_tiling = uvDefaultTiling;
            if (_mat != null)
            {
                _mat.mainTextureOffset = m_offset;
                _mat.mainTextureScale = m_tiling;
            }
        }
        if (RepeatTime != 0)
            InvokeRepeating(nameof(ChangeEffectState), 0, RepeatTime);
        else
        {
            isRepeat = true;
        }

    }
    float curTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (!isRepeat) return;

        if (uvOffsetSpeed.x != 0 || uvOffsetSpeed.y != 0)
        {
            m_offset.x += uvOffsetSpeed.x * (pauseTimer ? Time.deltaTime : Time.unscaledDeltaTime);
            if (m_offset.x > 1)
            {
                m_offset.x -= 1;
            }
            else if (m_offset.x < -1)
            {
                m_offset.x += 1;
            }
            m_offset.y += uvOffsetSpeed.y * (pauseTimer ? Time.deltaTime : Time.unscaledDeltaTime);
            if (m_offset.y > 1)
            {
                m_offset.y -= 1;
            }
            else if (m_offset.y < -1)
            {
                m_offset.y += 1;
            }
            _mat.mainTextureOffset = m_offset;
        }

        if (RepeatTime != 0)
        {
            curTime += Time.deltaTime;
            if (curTime >= ShowTimeUV)
            {
                isRepeat = false;
                curTime = 0;
                m_offset = Vector2.zero;
                _mat.mainTextureOffset = new Vector2(UVDefautOffset_Y, 0);
            }
        }

    }

    private void ChangeEffectState()
    {
        isRepeat = true;
    }

    void OnDestroy()
    {
        if (_mat != null)
        {
            GameObject.DestroyImmediate(_mat);
        }
    }


}
