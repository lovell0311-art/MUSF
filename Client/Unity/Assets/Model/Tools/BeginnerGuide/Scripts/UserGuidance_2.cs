using ETModel;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserGuidance_2 : MonoBehaviour, ICanvasRaycastFilter
{
    public Canvas canvas;
    //UserGuidance_2 userGuidance;
    public RectTransform rectTransform;
    bool one = true;
    public Camera camera;
    public UGUITriggerProxy eventTrigger;
    //拖拽时需要移动到的目标框
    public RectTransform targetTransfrom;
    // Start is called before the first frame update
    void OnEnable()
    {
        //if (!one)
        //{
        //    OnMaskRectShow(rectTransform);
        //}
    }
    void Start()
    {
        //one = false;
        //OnMaskRectShow(rectTransform);
    }
    private Rect mastRect;
    private GameObject highlight;
    Material GetMaterial
    {
        get
        {
            return GetComponent<Image>().material;
        }

    }

    public void SetTargetRect(RectTransform rect)
    {
        targetTransfrom = rect;
    }
    public void SetRect(RectTransform rect)
    {
        rectTransform = rect;
        eventTrigger = rectTransform.GetComponent<UGUITriggerProxy>();
    }

    public void SetCanvas(Canvas canvas)
    {
        this.canvas = canvas;
    }

    public void SetCamera(Camera camera)
    {
        this.camera = camera;
    }

    public void Update()
    {
        if (mastRect.Contains(vector))
        {
            if (Input.GetMouseButtonDown(0))
            {
                //proxy.OnPointerEnterEvent += () => { OnPointerEnter(x, y, type); };
                //proxy.OnPointerClickEvent += () => { OnPointerClickEvent(x, y, type); };
                //proxy.OnPointerExitEvent += () => { OnPointerExit(x, y, type); };
                //proxy.OnPointerDownEvent += () => { OnPointerDownEvent(x, y, type); };
                //proxy.OnPointerUpEvent += () => { OnPointerUpEvent(x, y, type); };

                if (!isClick)
                {
                    eventTrigger.OnBeginDragEvent.Invoke();
                    if (targetTransfrom)
                    {
                        if (targetTransfrom.GetComponent<UGUITriggerProxy>() || targetTransfrom.GetComponent<Button>())
                        {
                            OnMaskRectShow(targetTransfrom);
                        }
                    }
                }
                isClick = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isClick)
            {
                if (targetTransfrom)
                {
                    var t = targetTransfrom.GetComponent<UGUITriggerProxy>();
                    if (t)
                    {
                        if (mastRect.Contains(vector))
                        {
                            t.OnPointerEnterEvent.Invoke();
                        }
                        else
                        {
                            t.OnPointerExitEvent.Invoke();
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isClick)
            {
                if (targetTransfrom)
                {
                    var t = targetTransfrom.GetComponent<UGUITriggerProxy>();
                    if (t)
                    {
                        t.OnPointerEnterEvent?.Invoke();
                    }
                }

                eventTrigger.OnEndDragEvent.Invoke();
            }
            isClick = false;
            OnMaskRectShow(rectTransform);
            //eventTrigger.OnPointerExitEvent.Invoke();
        }
    }

    public void OnMaskRectShow(RectTransform rect)
    {
        //rectTransform  = rect ;
        ///要现实的位置 RrctTransform, 转化到屏幕坐标。
        Vector3 vect = RectTransformUtility.WorldToScreenPoint(camera, rect.gameObject.transform.position);
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();

        float radiox = Screen.width / canvasScaler.referenceResolution.x;//适配
        float radioy = Screen.height / canvasScaler.referenceResolution.y;//适配

        float x = vect.x - rect.sizeDelta.x * rect.pivot.x * radiox;
        float y = vect.y - rect.sizeDelta.y * rect.pivot.y * radioy;
#if UNITY_EDITOR || UNITY_IOS
        float vy = Screen.height - vect.y - rect.sizeDelta.y * rect.pivot.y * radioy;
#else
        float vy = /*Screen.height - */vect.y - rect.sizeDelta.y * rect.pivot.y * radioy;
#endif
        Vector4 vector4 = new Vector4(x, vy, x + rect.sizeDelta.x * radiox, vy + rect.sizeDelta.y * radioy);
        mastRect = new Rect(x, y, rect.sizeDelta.x * radiox, rect.sizeDelta.y * radioy);

        GetMaterial.SetFloat("_bNeedSet", 1);
        GetMaterial.SetVector("_MaskCull", vector4);

        SetRectHighligt();
    }

    public void SetRectHighligt()
    {
        if (rectTransform)
        {
            var gameobj = rectTransform.transform.Find("highlight");
            if (gameobj)
            {
                highlight = gameobj.gameObject;
                if (highlight != null)
                {
                    highlight.SetActive(true);
                }
            }
        }
    }

    public void OnMaskRectHide()
    {
        //GetMaterial.SetVector("_MaskCull", Vector4.zero);
        //GetMaterial.SetFloat("_bNeedSet", 0);
        //mastRect = Rect.zero;
        //GetComponent<Image>().raycastTarget = false;
        if (highlight != null)
        {
            highlight.SetActive(false);
        }
        gameObject.SetActive(false);
    }
    bool isClick = false;
    //IEnumerator InputEvent()
    //{
    //    while (true)
    //    {
    //        if (mastRect.Contains(vector))
    //        {
    //            if (Input.GetMouseButtonDown(0))
    //            {
    //                isClick = true;
    //            }
    //            if (Input.GetMouseButtonUp(0) && isClick)
    //            {
    //                isClick = false;
    //                OnMaskRectHide();
    //                yield break;
    //            }
    //        }
    //        else
    //        {
    //            isClick = false;
    //        }
    //        Debug.Log($"{isClick}");
    //        yield return null;
    //    }
    //}
    private Vector2 vector;
    /// <summary>
    /// 点击事件 往下传导
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        vector = sp;
        if (targetTransfrom) //检测拖拽到的目标点是否存在
        {
            if (targetTransfrom.GetComponent<Button>())
            {
                return true;
            }
            if (targetTransfrom.GetComponent<UGUITriggerProxy>())
            {
                return true;
            }
        }

        if (mastRect.Contains(sp))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}