using ETModel;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UserGuidance : MonoBehaviour, ICanvasRaycastFilter
{
    public Canvas canvas;
    public Camera camera;
    //UserGuidance userGuidance;
    public RectTransform rectTransform;
    bool one = true;
    // Start is called before the first frame update
    //void OnEnable()
    //{
    //if (!one)
    //{
    //    OnMaskRectShow(CameraComponent.Instance.UICamera, canvas, rectTransform);
    //}
    //}
    //void Start()
    //{
    //    one = false;
    //    OnMaskRectShow(CameraComponent.Instance.UICamera, canvas, rectTransform);
    //}

    private Rect mastRect;
    private GameObject highlight;

    public Action clickAction;
    Material GetMaterial
    {
        get
        {
            return GetComponent<Image>().material;
        }

    }

    public void SetCanvas(Canvas canvas)
    {
        this.canvas = canvas;
    }

    public void SetCamera(Camera camera)
    {
        this.camera = camera;
    }

    //public void SetRectTransform(RectTransform rect)
    //{
    //    rectTransform = rect;
    //    OnMaskRectShow(CameraComponent.Instance.UICamera, canvas, rectTransform);
    //}

    public void ResetMaskShow(RectTransform rect)
    {
        if (highlight != null)
        {
            highlight.SetActive(true);
        }
        gameObject.SetActive(true);
        OnMaskRectShow(rect);
    }

    public void OnMaskRectShow(RectTransform rect)
    {
        //if (rectTransform == null) return;
        if (camera == null) return;
        if (canvas == null) return;
        rectTransform = rect;
        ///要现实的位置 RrctTransform, 转化到屏幕坐标。
        Vector3 vect = RectTransformUtility.WorldToScreenPoint(camera, rect.gameObject.transform.position);
        //Canvas canvas = Canvas;
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
        //Log.Info(mastRect.ToString());

        GetMaterial.SetFloat("_bNeedSet", 1);
        GetMaterial.SetVector("_MaskCull", vector4);

        SetRectHighligt();
    }

    public void SetRectHighligt()
    {
        //highlight = rectTransform?.transform.Find("highlight").gameObject;
        //if (highlight != null)
        //{
        //    highlight.SetActive(true);
        //}
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

    public void SetForceTip()
    {
        gameObject.SetActive(true);
        vector = Vector2.zero;


        Vector4 vector4 = new Vector4(0, 0, Screen.width, Screen.height);
        GetMaterial.SetFloat("_bNeedSet", 1);
        GetMaterial.SetVector("_MaskCull", vector4);
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
        //gameObject.SetActive(false);
    }


    bool isClick = false;
    private Vector2 vector;

    private void Update()
    {
        if (rectTransform)
        {
            OnMaskRectShow(rectTransform);
            if (mastRect.Contains(vector))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isClick = true;
                }
                if (Input.GetMouseButtonUp(0) && isClick)
                {
                    //isClick = false;
                    //OnMaskRectHide();

                    if (rectTransform)
                    {
                        Button btn = rectTransform.GetComponent<Button>();
                        if (btn)
                        {
                            Log.Info("执行按钮上的回调事件  " + rectTransform.name);
                            OnMaskRectHide();
                            rectTransform = null;
                            btn.onClick.Invoke();

                            StartCoroutine(Delety());
                        }
                        else
                        {
                            var t = rectTransform.GetComponent<UGUITriggerProxy>();
                            if (t)
                            {

                                OnMaskRectHide();
                                rectTransform = null;

                                t.OnPointerEnterEvent?.Invoke();
                                t.OnPointerClickEvent?.Invoke();


                                StartCoroutine(Delety());
                            }
                        }
                    }
                }
            }
            else
            {
                isClick = false;
            }
        }
    }

    IEnumerator Delety()
    {
        yield return new WaitForSeconds(0.1f);
        clickAction?.Invoke();
    }

    /// <summary>
    /// 点击事件 往下传导
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        //Log.Info("IsRaycastLocationValid --------------- ");

        vector = sp;
        //if (mastRect.Contains(sp))
        //{
        //    if (rectTransform)
        //    {
        //        Button btn = rectTransform.GetComponent<Button>();
        //        if (btn)
        //        {
        //            Log.Info("执行按钮上的回调事件  " + rectTransform.name);
        //            rectTransform = null;
        //            btn.onClick.Invoke();
        //            clickAction?.Invoke();
        //            return true;
        //        }
        //    }
        //    return false;

        //}
        //else
        //{
        //    return true;
        //}
        return true;
    }

}