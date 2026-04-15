using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ETModel
{   
    /// <summary>
    /// UGUIÊÂ¼þ
    /// </summary>
    [RequireComponent(typeof(EventTrigger))]
    public class UGUITriggerProxy : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
    {
        
        public Action OnBeginDragEvent { get; set; }
        public Action OnEndDragEvent { get; set; }
        public Action OnPointerEnterEvent { get; set; }
        public Action OnPointerExitEvent { get; set; }
        public Action OnPointerDownEvent { get; set; }
        public Action OnPointerUpEvent { get; set; }
        public Action OnPointerClickEvent { get; set; }
        public Action OnDragEvent { get; set; }


        public Action<PointerEventData> OnBeginDragEvents { get; set; }
        public Action<PointerEventData> OnEndDragEvents { get; set; }
        public Action<PointerEventData> OnPointerEnterEvents { get; set; }
        public Action<PointerEventData> OnPointerExitEvents { get; set; }
        public Action<PointerEventData> OnPointerDownEvents { get; set; }
        public Action<PointerEventData> OnPointerUpEvents { get; set; }
        public Action<PointerEventData> OnPointerClickEvents { get; set; }
        public Action<PointerEventData> OnDragEvents { get; set; }

        public void OnBeginDrag(PointerEventData eventData)
        {

            OnBeginDragEvent?.Invoke();

            OnBeginDragEvents?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke();
            OnDragEvents?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {

            OnEndDragEvent?.Invoke();
            OnEndDragEvents?.Invoke(eventData);

        }

        public void OnPointerClick(PointerEventData eventData)
        {

            OnPointerClickEvent?.Invoke();
            OnPointerClickEvents?.Invoke(eventData);

        }

        public void OnPointerDown(PointerEventData eventData)
        {

            OnPointerDownEvent?.Invoke();
            OnPointerDownEvents?.Invoke(eventData);

        }
        public void OnPointerEnter(PointerEventData eventData)
        {

            OnPointerEnterEvent?.Invoke();
            OnPointerEnterEvents?.Invoke(eventData);

        }

        public void OnPointerExit(PointerEventData eventData)
        {

            OnPointerExitEvent?.Invoke();
            OnPointerExitEvents?.Invoke(eventData);

        }

        public void OnPointerUp(PointerEventData eventData)
        {

            OnPointerUpEvent?.Invoke();
            OnPointerUpEvents?.Invoke(eventData);

        }
       

    }
}