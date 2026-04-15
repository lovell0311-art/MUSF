using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ETModel
{
    public class ColorPicker : UIBehaviour
    {
        [Serializable]
        public class ColorPickerEvent : UnityEvent<Color> { }

        [SerializeField]
        private ColorPickerEvent m_onPicker = new ColorPickerEvent();
        public ColorPickerEvent onPicker
        {
            get { return m_onPicker; }
            set { m_onPicker = value; }
        }

        [SerializeField]
        private Color m_Color = default;
        public Color Color
        {
            get { return m_Color; }
            set
            {
                m_Color = new Color(value.r, value.g, value.b, value.a);
                if (null != onPicker)
                    onPicker.Invoke(m_Color);
            }
        }

        #region base field
        private Transform m_transform = null;
        #endregion

        #region MainColorTape && ColoredTape

       
        private Slider m_verticalCTSlider = null;
        private ColoredTape m_verticalFirstCT = null;

        #endregion

        #region UIBehaviour functions
        protected override void Start()
        {
            m_transform = this.transform;
            // 水平滑动条
            m_verticalCTSlider = m_transform.GetComponent<Slider>();
            m_verticalFirstCT = m_transform.Find("FirstLayerColoredTape").GetComponent<ColoredTape>();
            m_verticalCTSlider.onValueChanged.AddListener(verticalSliderChanged);
            // 初始化操作
            Color = Color.white;

        }

        protected override void OnDisable()
        {
            base.OnDisable();

        }
        #endregion

        #region vertical ct && main ct

        /// <summary>
        /// set color by slider 
        /// </summary>
        private void verticalSliderChanged(float value)
        {
            float height = m_verticalFirstCT.transform.GetComponent<RectTransform>().sizeDelta.x;
            Debug.Log(height);
            Color = m_verticalFirstCT.GetColor(new Vector2(height * (1 - value) - height / 2.0f,0));
        }

        #endregion


        #region RGBA input



        /// <summary>
        /// change color by alpha slider 
        /// </summary>
        /// <param name="value"></param>
        private void OnAlphaSliderChanged(float value)
        {
            Color = new Color(Color.r, Color.g, Color.b, value);
        }

        #endregion

    }
}