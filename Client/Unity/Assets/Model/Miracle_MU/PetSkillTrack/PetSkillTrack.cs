using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public class PetSkillTrack : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 target;
        private GameObject obj;
        private Coroutine trackCoroutine;
        [HideInInspector]
        public Vector3 Target
        {
            get { return target; }
            set
            {
                target = value;
                TrackTarget();
            }
        }
        public void OnEnable()
        {
            obj = this.gameObject;
        }
        public void TrackTarget()
        {
            if (obj == null)
            {
                obj = this.gameObject;
            }

            if (trackCoroutine != null)
            {
                StopCoroutine(trackCoroutine);
            }

            float time = PositionHelper.Distance2D(this.transform.position, target) / 15f;
            trackCoroutine = StartCoroutine(TrackTargetCoroutine(Mathf.Max(0.01f, time)));
        }

        private IEnumerator TrackTargetCoroutine(float duration)
        {
            Vector3 startPosition = obj.transform.position;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                obj.transform.position = Vector3.Lerp(startPosition, target, progress);
                yield return null;
            }

            obj.transform.position = target;
            trackCoroutine = null;
            ResourcesComponent.Instance.RecycleGameObject(obj);
        }
    }
}

