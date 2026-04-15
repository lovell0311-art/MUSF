using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace ETHotfix
{


    public class MiniMap
    {
        /// <summary>鐐瑰嚮浜嬩欢</summary>
        public Action<Vector2> OnClickEvent;

        public GameObject MiniMap_Icon;

        /// <summary>鍙互鎷栨嫿</summary>
        public bool CanDrag = true;
        /// <summary>鍙互缂╂斁</summary>
        public bool CanZoom = true;
        public float MinZoomScale = 1f;
        public float MaxZoomScale = 10f;

        /// <summary>鍦板浘缂╂斁</summary>
        public Vector3 MapScale
        {
            get => Vector3.one * mapZoomScale;
            set
            {
                SetMapScale(value);
            }
        }

        /// <summary>鍦板浘Image缁勪欢</summary>
        public Image MapImage;

        /// <summary>璺熻釜鐩爣</summary>
        public Icon TrackIcon = null;

        public Transform OffsetTranform => viewportRoot != null ? viewportRoot : offset;


        private Transform parent;
        private RectTransform parentRectTransform;
        private Vector2 parentCenterPoint;
        private Camera uiCamera;
        private Canvas parentCanvas;

        private Transform viewportRoot;
        private GameObject goViewport;

        private Transform offset;
        private GameObject goOffset;

        private RectTransform mapRectTransform;
        private GameObject goMiniMap;

        private Matrix4x4 matrix4X4 = Matrix4x4.identity;
        private bool matrixChanged = false;
        private bool refreshLayoutFallbackLogged = false;

        /// <summary>Icon 瀵硅薄姹?/summary>
        private Dictionary<Type, Queue<Icon>> iconPool = new Dictionary<Type, Queue<Icon>>();

        private GameObject icon_loop;


        private CanvasScaler canvasScaler;
        private float mapZoomScale = 1f;
        private float mapCalibrationScale = 1f;
        private Vector2 mapCalibrationOffset = Vector2.zero;
        private Vector2 viewportCenterOffset = Vector2.zero;
        private Vector2 trackCenterOffset = Vector2.zero;
        private float ScaleFactor => parentCanvas != null
            ? parentCanvas.scaleFactor
            : (canvasScaler != null ? canvasScaler.scaleFactor : 1f);

        private static Vector2 ToVector2(Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }

        private static Vector3 ToVector3(Vector2 value)
        {
            return new Vector3(value.x, value.y, 0f);
        }


        public void Init(Transform parent)
        {
            this.parent = parent;
            this.parentRectTransform = parent as RectTransform;
            RefreshLayoutContext();

            if (goViewport == null)
            {
                goViewport = new GameObject("viewport");
                goViewport.transform.SetParent(this.parent, false);
                goViewport.transform.localScale = Vector3.one;
                RectTransform viewportRect = goViewport.AddComponent<RectTransform>();
                viewportRect.anchorMin = Vector2.one * 0.5f;
                viewportRect.anchorMax = Vector2.one * 0.5f;
                viewportRect.pivot = Vector2.one * 0.5f;
                viewportRect.anchoredPosition = Vector2.zero;

                viewportRoot = goViewport.transform;

                goOffset = new GameObject("offset");
                goOffset.transform.SetParent(viewportRoot, false);
                goOffset.transform.localScale = Vector3.one;
                RectTransform rect = goOffset.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.one * 0.5f;
                rect.anchorMax = Vector2.one * 0.5f;
                rect.pivot = Vector2.one * 0.5f;
                rect.anchoredPosition = Vector2.zero;


                offset = goOffset.transform;
                Transform trans = viewportRoot;
                for (int i = 0; i < 10; ++i)
                {
                    trans = trans.parent;
                    if (trans == null) break;
                    canvasScaler = trans.GetComponent<CanvasScaler>();
                    if (canvasScaler != null) break;
                }

                goMiniMap = new GameObject("MiniMap");
                goMiniMap.transform.SetParent(offset, false);
                goMiniMap.transform.localScale = Vector3.one;
                mapRectTransform = goMiniMap.AddComponent<RectTransform>();
                mapRectTransform.anchorMin = Vector2.one * 0.5f;
                mapRectTransform.anchorMax = Vector2.one * 0.5f;
                mapRectTransform.pivot = new Vector2(0.0f, 1f);
                mapRectTransform.anchoredPosition = new Vector2(0f, 0f);
                mapRectTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 315f));
                //mapRectTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                // 娣诲姞 Image
                MapImage = goMiniMap.AddComponent<Image>();
                MapImage.gameObject.SetActive(false);
                UGUITriggerProxy proxy = goMiniMap.AddComponent<UGUITriggerProxy>();

                InitLevel();

                icon_loop = new GameObject($"icon_loop");
                icon_loop.transform.SetParent(offset.transform, false);
                icon_loop.transform.localScale = Vector3.one;
                rect = icon_loop.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.one * 0.5f;
                rect.anchorMax = Vector2.one * 0.5f;
                rect.pivot = Vector2.one * 0.5f;
                rect.anchoredPosition = Vector2.zero;
            }
            else if (goViewport.transform.parent != this.parent)
            {
                goViewport.transform.SetParent(this.parent, false);
            }


            {
                RectTransform viewportRect = goViewport.GetComponent<RectTransform>();
                viewportRect.anchoredPosition = viewportCenterOffset;
                offset.localPosition = Vector3.zero;
                mapCalibrationOffset = Vector2.zero;
                mapCalibrationScale = 1f;
                mapZoomScale = 1f;
                ApplyMapTransform();

                isZoom = false;

                TrackIcon = null;


                {
                    // 鎷栨嫿鍒濆鍖?
                    oriPos = Vector3.zero;
                    firstClickPos = Vector2.zero;
                    isDrag = false;
                    dragDis = 0f;
                    dragPosition = Vector2.zero;
                }
                {
                    // 缂╂斁鍒濆鍖?
                    isZoom = false;
                    oriScale = 1f;
                    zoomMapPos = Vector2.zero;
                    zoomCanvasPos = Vector2.zero;
                    touchInMapDis = 0f;
                }
                {
                    // 鐐瑰嚮鍒濆鍖?
                    lastDragStatus = false;
                    isClickInvalid = false;
                    lastTouchCount = 0;
                }
                {
                    // 璺熻釜Icon
                    lastTrackIconPosition = Vector2.zero;

                }
            }

            RefreshLayoutContext();
            ApplyMapTransform();

            // 淇璇偣鍑婚棶棰?
            Update();
            isClickInvalid = true;
        }

        public void SetViewportCenterOffset(Vector2 centerOffset)
        {
            viewportCenterOffset = centerOffset;

            if (goViewport == null)
            {
                return;
            }

            RectTransform rect = goViewport.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = viewportCenterOffset;
            }
        }

        public void SetTrackCenterOffset(Vector2 centerOffset)
        {
            trackCenterOffset = centerOffset;
        }

        public void UpdateMiniMap(Sprite mapSprite, Vector2Int mapSize)
        {
            if (MapImage == null)
            {
                return;
            }

            bool hasSprite = mapSprite != null;
            MapImage.material = null;
            MapImage.overrideSprite = null;
            MapImage.sprite = mapSprite;
            MapImage.color = Color.white;
            MapImage.enabled = hasSprite;
            MapImage.gameObject.SetActive(hasSprite);

            if (!hasSprite)
            {
                mapRectTransform.sizeDelta = Vector2.zero;
                return;
            }

            if (mapSize.x > 0 && mapSize.y > 0)
            {
                mapRectTransform.sizeDelta = mapSize;
            }
            else
            {
                MapImage.SetNativeSize();
            }

            ApplyMapTransform();
        }

        public void ConfigureMapCalibration(Vector2 offset, float calibrationScale)
        {
            mapCalibrationOffset = offset;
            mapCalibrationScale = calibrationScale > 0f ? calibrationScale : 1f;
            ApplyMapTransform();
        }

        public void SetMapScale(Vector3 scale)
        {
            mapZoomScale = ClampZoomScale(scale.x);
            ApplyMapTransform();
        }

        private float ClampZoomScale(float scale)
        {
            float minZoom = Mathf.Max(0.01f, MinZoomScale);
            float maxZoom = Mathf.Max(minZoom, MaxZoomScale);
            return Mathf.Clamp(scale, minZoom, maxZoom);
        }

        /// <summary>
        /// 璁剧疆涓績浣嶇疆
        /// </summary>
        /// <param name="pos"></param>
        public void SetCenterPosition(Vector2 pos)
        {
            RefreshLayoutContext();
            Vector2 localOffset = parentCenterPoint - Map2UIPosition(pos);
            offset.localPosition = ToVector3(localOffset);
        }
        /// <summary>
        /// 鍒濆鍖栧眰绾?0 - 10
        /// </summary>
        private void InitLevel()
        {
            for (int i = 0; i <= 10; ++i)
            {
                GameObject go = new GameObject($"level_{i}");
                go.transform.parent = offset.transform;
                go.transform.localScale = Vector3.one;
                RectTransform rect = go.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.one * 0.5f;
                rect.anchorMax = Vector2.one * 0.5f;
                rect.pivot = Vector2.one * 0.5f;
                rect.anchoredPosition = Vector2.zero;
            }
        }

        public void Update()
        {
            RefreshLayoutContext();
            UpdatePosition();
            UpdateZoom();
            UpdateClickEvent();
            UpdateIcons();

            UpdateTrack();



            LateUpdate();
        }

        private void LateUpdate()
        {
            if (lastTouchCount != Input.touchCount)
            {
                lastTouchCount = Input.touchCount;
            }
        }

        #region 鎷栧姩
        private Vector3 oriPos = Vector3.zero;
        private Vector2 firstClickPos = Vector2.zero;

        /// <summary>姝ｅ湪鎷栨嫿</summary>
        private bool isDrag = false;
        /// <summary>鎷栨嫿璺濈</summary>
        private float dragDis = 0f;

        private Vector2 dragPosition = Vector2.zero;
        private void UpdatePosition()
        {
            if (CanDrag == false) return;
            if (Input.touchCount == 1)
            {
                dragPosition = Input.GetTouch(0).position;
            }
            else if (Input.GetMouseButton(0))
            {
                dragPosition = ToVector2(Input.mousePosition);
            }
            else
            {
                isDrag = false;
                return;
            }
            //Debug.Log($"clickPos:{clickPos}");
            if (isDrag == false)
            {
                // 绗竴娆＄偣鍑诲睆骞?
                isDrag = true;

                firstClickPos = dragPosition;
                oriPos = offset.localPosition;
                dragDis = 0f;
                // Treat single-finger taps as valid even if the previous frame still
                // reported one touch; only suppress click synthesis for multi-touch.
                isClickInvalid = Input.touchCount > 1 || lastTouchCount > 1;
                LoginStageTrace.Append($"MiniMap drag-begin screen={dragPosition.x:0.0},{dragPosition.y:0.0} touchCount={Input.touchCount} lastTouchCount={lastTouchCount} invalid={isClickInvalid}");
                return;
            }
            Vector2 dis = (dragPosition - firstClickPos);
            dragDis += dis.magnitude;
            offset.localPosition = oriPos + ToVector3(dis / ScaleFactor);
            if (dragDis > 0.1f)
            {
                TrackIcon = null;   // 鍙栨秷璺熻釜
            }
        }
        #endregion

        #region 缂╂斁
        private bool isZoom = false;
        private float oriScale = 1f;
        /// <summary>缂╂斁鐐?/summary>
        private Vector2 zoomMapPos = Vector2.zero;
        private Vector2 zoomCanvasPos = Vector2.zero;

        /// <summary>鍦ㄥ湴鍥句腑鐨勮窛绂?/summary>
        private float touchInMapDis = 0f;
        private float lastTouchZoomRealtime = -10f;
        private const float TouchZoomResumeWindowSeconds = 0.25f;

        private void UpdateZoom()
        {
            if (CanZoom == false) return;
            float scale = 0.0f;
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1Data = Input.GetTouch(1);
                Vector2 touch0Screen = touch0.position;
                Vector2 touch1Screen = touch1Data.position;
                Vector2 previousTouch0Screen = touch0Screen - touch0.deltaPosition;
                Vector2 previousTouch1Screen = touch1Screen - touch1Data.deltaPosition;
                float currentTouchDis = (touch0Screen - touch1Screen).magnitude;
                float previousTouchDis = (previousTouch0Screen - previousTouch1Screen).magnitude;
                bool resumeDiscretePinch =
                    previousTouchDis < 0.001f &&
                    touchInMapDis > 0.001f &&
                    Time.unscaledTime - lastTouchZoomRealtime <= TouchZoomResumeWindowSeconds;
                if (resumeDiscretePinch)
                {
                    previousTouchDis = touchInMapDis;
                }

                Vector2 touch0Map = ScreenToCalibratedMapPosition(touch0Screen);
                Vector2 touch1Map = ScreenToCalibratedMapPosition(touch1Screen);
                Vector2 centerPos = (touch0Map + touch1Map) / 2f;

                isZoom = true;
                isClickInvalid = true;
                TrackIcon = null;   // 鍙栨秷璺熻釜
                zoomMapPos = centerPos;
                zoomCanvasPos = Map2UIPosition(centerPos);
                oriPos = offset.localPosition;
                oriScale = mapZoomScale;

                touchInMapDis = currentTouchDis;
                lastTouchZoomRealtime = Time.unscaledTime;

                if (previousTouchDis < 0.001f)
                {
                    LoginStageTrace.Append(
                        $"MiniMap zoom-touch baseline current={currentTouchDis:0.0} " +
                        $"delta0={touch0.deltaPosition.magnitude:0.0} delta1={touch1Data.deltaPosition.magnitude:0.0}");
                    return;
                }

                scale = ClampZoomScale(oriScale * currentTouchDis / previousTouchDis);
                LoginStageTrace.Append(
                    $"MiniMap zoom-touch prev={previousTouchDis:0.0} current={currentTouchDis:0.0} " +
                    $"from={oriScale:0.000} to={scale:0.000} resume={resumeDiscretePinch}");
            }
            else
            {
                float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
                if (Mathf.Abs(scroll) < 0.001f)
                {
                    float mouseScrollDelta = Input.mouseScrollDelta.y;
                    if (Mathf.Abs(mouseScrollDelta) >= 0.001f)
                    {
                        scroll = Mathf.Clamp(mouseScrollDelta * 0.1f, -1f, 1f);
                    }
                }
                if (scroll == 0.0f)
                {
                    isZoom = false;
                    return;
                }
                if (isZoom == false)
                {
                    isZoom = true;
                    TrackIcon = null;   // 鍙栨秷璺熻釜
                    zoomMapPos = ScreenToCalibratedMapPosition(ToVector2(Input.mousePosition));
                    zoomCanvasPos = Map2UIPosition(zoomMapPos);
                    oriPos = offset.localPosition;
                }
                oriScale = mapZoomScale;
                scale = ClampZoomScale(oriScale + scroll);
                LoginStageTrace.Append($"MiniMap zoom-mouse scroll={scroll:0.00} from={oriScale:0.00} to={scale:0.00} screen={Input.mousePosition.x:0.0},{Input.mousePosition.y:0.0}");
            }
            mapZoomScale = scale;
            ApplyMapTransform();


            Vector2 diff = Map2UIPosition(zoomMapPos) - zoomCanvasPos;

            offset.localPosition = oriPos - ToVector3(diff);

            if ((int)oriScale != (int)scale)
            {
                OnEventLodChange((int)scale);
            }
        }

        private void RefreshLayoutContext()
        {
            parentCenterPoint = trackCenterOffset;
            parentCanvas = FindParentCanvas(parent);
            uiCamera = parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? parentCanvas.worldCamera
                : null;
        }

        private static Canvas FindParentCanvas(Transform start)
        {
            Transform current = start;
            while (current != null)
            {
                Canvas canvas = current.GetComponent<Canvas>();
                if (canvas != null)
                {
                    return canvas;
                }

                current = current.parent;
            }

            return null;
        }

        private void ApplyMapTransform()
        {
            if (mapRectTransform == null)
            {
                return;
            }

            Quaternion mapRotation = Quaternion.Euler(new Vector3(0f, 0f, 315f));
            float finalScale = mapZoomScale * mapCalibrationScale;
            mapRectTransform.anchoredPosition = mapCalibrationOffset;
            mapRectTransform.localRotation = mapRotation;
            mapRectTransform.localScale = Vector3.one * finalScale;

            matrix4X4.SetTRS(
                new Vector3(mapCalibrationOffset.x, mapCalibrationOffset.y, 0f),
                mapRotation,
                new Vector3(finalScale, finalScale, finalScale));
            matrixChanged = true;
        }

        private Vector2 ScreenToCalibratedMapPosition(Vector2 screenPos)
        {
            RectTransform offsetRect = goOffset?.GetComponent<RectTransform>();
            if (offsetRect == null)
            {
                return Vector2.zero;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                offsetRect,
                screenPos,
                uiCamera,
                out Vector2 localPos);
            Vector3 localPoint = new Vector3(localPos.x, localPos.y, 0f);
            Vector3 rawPos3 = matrix4X4.inverse.MultiplyPoint3x4(localPoint);
            Vector2 rawPos = new Vector2(rawPos3.x, rawPos3.y);
            return new Vector2(-rawPos.y, rawPos.x);
        }
        #endregion

        #region 鐐瑰嚮
        bool lastDragStatus = false;
        /// <summary>鐐瑰嚮鏃犳晥</summary>
        bool isClickInvalid = false;

        int lastTouchCount = 0;
        private void UpdateClickEvent()
        {
            if (CanDrag == false) return;
            if (isDrag == lastDragStatus) return;
            lastDragStatus = isDrag;
            if (isDrag == true) return;

            if (Input.touchCount != 0)
            {
                return;
            }
            if (isClickInvalid)
            {
                if (lastTouchCount <= 1)
                {
                    LoginStageTrace.Append($"MiniMap click-stale-invalid screen={dragPosition.x:0.0},{dragPosition.y:0.0} dragDis={dragDis:0.0} lastTouchCount={lastTouchCount}");
                    isClickInvalid = false;
                }
                else
                {
                LoginStageTrace.Append($"MiniMap click-invalid screen={dragPosition.x:0.0},{dragPosition.y:0.0} dragDis={dragDis:0.0} lastTouchCount={lastTouchCount}");
                Debug.Log("鐐瑰嚮鏃犳晥");
                return;
                }
            }
            if (dragDis < 50f)
            {
                Vector2 mapPosition = ScreenToCalibratedMapPosition(dragPosition);
                LoginStageTrace.Append($"MiniMap click screen={dragPosition.x:0.0},{dragPosition.y:0.0} map={mapPosition.x:0.0},{mapPosition.y:0.0} dragDis={dragDis:0.0}");
                OnClickEvent?.Invoke(mapPosition);
            }
            else
            {
                LoginStageTrace.Append($"MiniMap click-suppressed screen={dragPosition.x:0.0},{dragPosition.y:0.0} dragDis={dragDis:0.0}");
            }
        }

        #endregion


        #region 璺熻釜Icon

        private Vector2 lastTrackIconPosition = Vector2.zero;

        private void UpdateTrack()
        {
            if (TrackIcon == null) return;
            if (lastTrackIconPosition == TrackIcon.Position) return;
            lastTrackIconPosition = TrackIcon.Position;
            SetCenterPosition(TrackIcon.Position);

        }

        #endregion



        public Vector2 Map2UIPosition(Vector2 pos)
        {
            Vector3 mapPoint = new Vector3(pos.y, -pos.x, 0f);
            return matrix4X4.MultiplyPoint3x4(mapPoint);
        }
        public Vector2 UI2MapPosition(Vector2 pos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                      mapRectTransform,//鎯宠鏀瑰彉浣嶇疆瀵硅薄 鐨勭埗瀵硅薄
                      pos,//寰楀埌灞忓箷鐨勯紶鏍囦綅缃?
                      uiCamera,//Ui鐢ㄧ殑鐩告満
                      out Vector2 localPos);//寰楀埌涓€涓浆鎹㈠畬鎴愮殑鐩稿鍧愭爣
            return ScreenToCalibratedMapPosition(pos);
        }


        private Dictionary<long, Icon> iconDict = new Dictionary<long, Icon>();

        private void UpdateIcons()
        {
            if (matrixChanged == false) return;
            matrixChanged = false;

            foreach (Icon icon in iconDict.Values)
            {
                icon.transform.localPosition = ToVector3(Map2UIPosition(icon.Position));
            }
        }

        private void OnEventLodChange(int lod)
        {
            foreach (Icon icon in iconDict.Values)
            {
                icon.LOD = lod;
                icon.LODChange();
            }
        }


        #region Create Icon

        public T Create<T>(string name, int level) where T : Icon
        {
            T icon = CreateInstance<T>();
            icon.Parent = this;
            icon.Init(name);
            icon.Level = level;
            icon.LOD = Mathf.Max(1, (int)mapZoomScale);
            icon.Awake();
            icon.LODChange();
            iconDict.Add(icon.InstanceId, icon);
            icon.gameObject.SetActive(true);
            return icon;
        }


        public T CreateInstance<T>() where T : Icon
        {
            Queue<Icon> queue;
            if (iconPool.TryGetValue(typeof(T), out queue) == false)
            {
                queue = new Queue<Icon>();
                iconPool[typeof(T)] = queue;
            }

            T obj;
            if (queue.Count != 0)
            {
                obj = (T)queue.Dequeue();
            }
            else
            {
                obj = (T)Activator.CreateInstance(typeof(T));
            }

            return obj;
        }

        public void Recycle(object obj)
        {
            Queue<Icon> queue;
            Type type = obj.GetType();
            Icon icon = obj as Icon;
            if (iconPool.TryGetValue(type, out queue) == false)
            {
                queue = new Queue<Icon>();
                iconPool[type] = queue;
            }

            queue.Enqueue(icon);

            icon.gameObject.SetActive(false);
            icon.transform.parent = icon_loop.transform;
        }


        public void Clear()
        {
            TrackIcon = null;

            Icon[] icons = iconDict.Values.ToArray();
            foreach (Icon icon in icons)
            {
                icon.Dispose();
            }

        }
        #endregion


        #region Icon
        public class Icon
        {
            public long InstanceId => instanceId;

            public MiniMap Parent
            {
                get => parent;
                set
                {
                    if (value != null)
                    {
                        instanceId = IdGenerater.GenerateInstanceId();
                    }
                    parent = value;
                }
            }

            public RectTransform transform;
            /// <summary>鍦ㄥ湴鍥句腑鐨勪綅缃?/summary>
            public Vector2 Position
            {
                get { return position; }
                set
                {
                    position = value;
                    UpdatePosition();
                }
            }

            /// <summary>灞傜骇 0 - 10</summary>
            public int Level
            {
                get { return level; }
                set
                {
                    level = value;
                    transform.parent = Parent.offset.Find($"level_{level}");
                    go.transform.localScale = Vector3.one;
                }
            }

            public int LOD = 0;


            public GameObject gameObject { get => go; }



            protected GameObject go;


            private long instanceId;
            public MiniMap parent;
            private Vector2 position;
            private int level;


            public void Init(string name)
            {
                if (go == null)
                {
                    go = new GameObject(name);
                    transform = go.AddComponent<RectTransform>();
                }
                go.name = name;
                transform.anchorMin = Vector2.one * 0.5f;
                transform.anchorMax = Vector2.one * 0.5f;
                transform.pivot = Vector2.one * 0.5f;
                transform.anchoredPosition = Vector2.zero;

            }

            public virtual void Awake()
            {
            }

            public virtual void LODChange()
            {
            }


            public void UpdatePosition()
            {
                transform.localPosition = ToVector3(Parent.Map2UIPosition(position));
            }

            public virtual void Dispose()
            {
                parent.iconDict.Remove(InstanceId);
                parent.Recycle(this);
                instanceId = 0;
            }
        }

        public class IconNpc : Icon
        {
            private GameObject npcIcon;
            public Image image;
            public Text text;
            public override void Awake()
            {
                if (npcIcon == null)
                {
                    Transform uiminimap_icon = Parent.MiniMap_Icon.transform;
                    npcIcon = GameObject.Instantiate(uiminimap_icon.Find(n: "npc_icon"), go.transform).gameObject;
                    image = npcIcon.transform.Find("Icon").GetComponent<Image>();
                    text = npcIcon.transform.Find("Image/Text").GetComponent<Text>();

                }
                npcIcon.SetActive(true);
            }

            public override void LODChange()
            {
                if (LOD >= 4)
                {
                    npcIcon.transform.Find("Image").gameObject.SetActive(true);
                }
                else
                {
                    npcIcon.transform.Find("Image").gameObject.SetActive(false);
                }
            }
        }

        public class IconMonster : Icon
        {
            private GameObject monsterIcon;
            public Text text;
            public Image iconImage;
            public override void Awake()
            {
                if (monsterIcon == null)
                {
                    Transform uiminimap_icon = Parent.MiniMap_Icon.transform;
                    monsterIcon = GameObject.Instantiate(uiminimap_icon.Find(n: "monster_icon"), go.transform).gameObject;
                    text = monsterIcon.transform.Find("Image/Text").GetComponent<Text>();
                    iconImage = monsterIcon.transform.Find("Icon").GetComponent<Image>();

                }
                monsterIcon.SetActive(true);
            }

            public override void LODChange()
            {
                if (LOD >= 4)
                {
                    monsterIcon.transform.Find("Image").gameObject.SetActive(true);
                    iconImage.gameObject.SetActive(false);
                }
                else
                {
                    monsterIcon.transform.Find("Image").gameObject.SetActive(false);
                    iconImage.gameObject.SetActive(true);
                }
            }
        }

        public class IconRole : Icon
        {
            private GameObject roleIcon;
            public Image image;

            public Quaternion Rotation
            {
                get => roleIcon.transform.rotation;
                set => SetRotation(value);
            }

            public override void Awake()
            {
                if (roleIcon == null)
                {
                    Transform uiminimap_icon = Parent.MiniMap_Icon.transform;
                    roleIcon = GameObject.Instantiate(uiminimap_icon.Find(n: "role_icon"), go.transform).gameObject;
                    image = roleIcon.transform.Find("Image").GetComponent<Image>();
                }
                roleIcon.SetActive(true);
            }

            public void SetRotation(Quaternion rotation)
            {
                roleIcon.transform.rotation = rotation;
            }

            public override void LODChange()
            {

            }
        }

        public class IconTargetPoint : Icon
        {
            private GameObject pointIcon;


            public override void Awake()
            {
                if (pointIcon == null)
                {
                    Transform uiminimap_icon = Parent.MiniMap_Icon.transform;
                    pointIcon = GameObject.Instantiate(uiminimap_icon.Find("targetPoint_icon"), go.transform).gameObject;

                }
                pointIcon.SetActive(true);
            }
            public override void LODChange()
            {

            }
        }

        public class IconNavPoint : Icon
        {
            private GameObject pointIcon;

            public override void Awake()
            {
                if (pointIcon == null)
                {
                    Transform uiminimap_icon = Parent.MiniMap_Icon.transform;
                    pointIcon = GameObject.Instantiate(uiminimap_icon.Find("navPoint_icon"), go.transform).gameObject;
                }
                pointIcon.SetActive(true);
            }

            public override void LODChange()
            {

            }
        }


        public class IconTransterPoint : Icon
        {
            private GameObject transterPointIcon;

            public Text text;

            public Image image;

            public override void Awake()
            {
                if (transterPointIcon == null)
                {
                    Transform uiminimap_icon = Parent.MiniMap_Icon.transform;
                    transterPointIcon = GameObject.Instantiate(uiminimap_icon.Find("transferPoint_icon"), go.transform).gameObject;
                    text = transterPointIcon.transform.Find("Image/Text").GetComponent<Text>();
                    image = transterPointIcon.transform.Find("Icon").GetComponent<Image>();
                }
                transterPointIcon.SetActive(true);
            }
            public override void LODChange()
            {
                if(LOD >= 4)
                {
                    transterPointIcon.transform.Find("Image").gameObject.SetActive(true);
                    image.gameObject.SetActive(false);
                }
                else
                {
                    transterPointIcon.transform.Find("Image").gameObject.SetActive(false);
                    image.gameObject.SetActive(true);
                }
            }
        }


        public class IconTreasurePoint : Icon
        {
            private GameObject treasurePointIcon;
            public Image image;
            public Text text;
            public override void Awake()
            {
                if (treasurePointIcon == null)
                {
                    Transform uiminimap_icon = Parent.MiniMap_Icon.transform;
                    treasurePointIcon = GameObject.Instantiate(uiminimap_icon.Find(n: "treasurePoint_icon"), go.transform).gameObject;
                    image = treasurePointIcon.transform.Find("Icon").GetComponent<Image>();
                    text = treasurePointIcon.transform.Find("Image/Text").GetComponent<Text>();

                }
                treasurePointIcon.SetActive(true);
            }

            public override void LODChange()
            {
                if (LOD >= 4)
                {
                    treasurePointIcon.transform.Find("Image").gameObject.SetActive(true);
                }
                else
                {
                    treasurePointIcon.transform.Find("Image").gameObject.SetActive(false);
                }
            }
        }

        #endregion
    }


}

