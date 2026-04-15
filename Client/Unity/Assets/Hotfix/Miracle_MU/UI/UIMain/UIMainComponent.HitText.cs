using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using DG.Tweening;
using UnityEngine.UI;
namespace ETHotfix {
    public partial class UIMainComponent
    {
        //璧勬簮鍚?
        private const string UIUnitTakeDemageText = "UIUnitTakeDamageText";
        //褰撳墠鎵€鏄剧ず鐨勪激瀹虫枃瀛?闆嗗悎
        public Queue<GameObject> goPosDict = new Queue<GameObject>();
        private readonly Dictionary<int, long> hitTextRecycleTimers = new Dictionary<int, long>();

        private Transform parent;

        public void Init_HitText() 
        {
            parent = this.GetParent<UI>().GameObject.transform;
        }
        public void Recycle(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            ClearScheduledRecycle(obj);

            Text text = obj.GetComponent<Text>();
            if (text != null)
            {
                ResetDamageTextState(text);
            }

            obj.SetActive(false);
            goPosDict.Enqueue(obj);
        }

        public GameObject ObjDequeue()
        {
            GameObject obj;
            if (goPosDict.Count > 0)
            {
                obj = goPosDict.Dequeue();
            }
            else
            {
                obj = ResourcesComponent.Instance.LoadGameObject(UIUnitTakeDemageText.StringToAB(), UIUnitTakeDemageText);
                obj.transform.SetParent(parent, false);
            }

            Text text = obj?.GetComponent<Text>();
            if (text != null)
            {
                ClearScheduledRecycle(obj);
                ResetDamageTextState(text);
            }

            return obj;

        }

        public void CleanHitText() 
        {
            foreach (long timerId in hitTextRecycleTimers.Values)
            {
                TimerComponent.Instance?.Remove(timerId);
            }

            hitTextRecycleTimers.Clear();
            goPosDict.Clear();
        }

        public void ScheduleRecycle(GameObject obj, long delay)
        {
            if (obj == null)
            {
                return;
            }

            ClearScheduledRecycle(obj);

            Timer timer = TimerComponent.Instance.RegisterTimeCallBack(delay, () =>
            {
                hitTextRecycleTimers.Remove(obj.GetInstanceID());
                Recycle(obj);
            });

            hitTextRecycleTimers[obj.GetInstanceID()] = timer.Id;
        }

        private static void ResetDamageTextState(Text text)
        {
            if (text == null)
            {
                return;
            }

            text.transform.DOKill();
            text.DOKill();

            Color color = text.color;
            color.a = 1f;
            text.color = color;
            text.text = string.Empty;
            text.raycastTarget = false;
            text.transform.localScale = Vector3.one * 0.75f;
        }

        private void ClearScheduledRecycle(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            int instanceId = obj.GetInstanceID();
            if (!hitTextRecycleTimers.TryGetValue(instanceId, out long timerId))
            {
                return;
            }

            TimerComponent.Instance?.Remove(timerId);
            hitTextRecycleTimers.Remove(instanceId);
        }
    }
}
