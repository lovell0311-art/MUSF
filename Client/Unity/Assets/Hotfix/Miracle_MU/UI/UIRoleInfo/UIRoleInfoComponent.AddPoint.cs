using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// 自定义 添加属性点
    /// </summary>
    public partial class UIRoleInfoComponent
    {
        public InputField input;
        public ReferenceCollector referenceCollector_AddPoint;
        public GameObject AddPointPanel;
        public Text title;
        public Transform curpropertyTrs = null;

        public void Init_AddPoint()
        {
            AddPointPanel = collector.GetImage("AddPoint").gameObject;
            referenceCollector_AddPoint = AddPointPanel.GetReferenceCollector();
            input = referenceCollector_AddPoint.GetInputField("InputField");
            title = referenceCollector_AddPoint.GetText("title");
            input.onValueChanged.AddSingleListener((value) =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

            });
            referenceCollector_AddPoint.GetButton("SureBtn").onClick.AddSingleListener(RequestAddPoint);
            referenceCollector_AddPoint.GetButton("CancelBtn").onClick.AddSingleListener(HideAddPointPanel);

            HideAddPointPanel();
        }
        /// <summary>
        /// 确认添加点数
        /// </summary>
        public void RequestAddPoint()
        {

            // if (int.TryParse(input.text, out int resule) && int.TryParse(roleLevPoints.text, out int curLevPoints))
            if (int.TryParse(input.text, out int resule) && roleEntity.Property.GetProperValue(E_GameProperty.FreePoint) is long curLevPoints)
            {
                if (resule > curLevPoints)//判断所输入的点数 是否 大于当前所拥有的点数
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "当前输入的点数 已超过所拥有的点数");
                    input.text = curLevPoints.ToString();
                }
                else if (resule < 0)//当前输入的点数 是否为正数
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "输入的点数 不能输入负数");
                    input.text = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(input.text))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请输入点数");
                return;
            }

            //请求添加点数
            AddPoint(curpropertyTrs, int.Parse(input.text)).Coroutine();
        }

        public void ShowAddPointPanel(Transform Propertrs)
        {
            curpropertyTrs = Propertrs;
            title.text = $"请输入点数 - <color=red>{GetPropertyName(Propertrs)}</color>";
            input.text = string.Empty;
            AddPointPanel.SetActive(true);
        }
        public void HideAddPointPanel()
        {
            curpropertyTrs = null;
            AddPointPanel.SetActive(false);
        }

        /// <summary>
        /// 添加属性点
        /// </summary>
        /// <param name="propertyTrs">属性点 对应的名称</param>
        /// <param name="point">需要加的点 默认为1</param>
        /// <returns></returns>
        public async ETVoid AddPoint(Transform propertyTrs, int point = 1)
        {
            if (this.roleEntity.Property.GetProperValue(E_GameProperty.FreePoint) <= 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "剩余属性点数为0");
                return;
            }

            G2C_BattlePropertyAddPointResponse g2C_BattleProperty = (G2C_BattlePropertyAddPointResponse)await SessionComponent.Instance.Session.Call(new C2G_BattlePropertyAddPointRequest
            {
                BattlePropertyId = GetPropertyId(propertyTrs),
                AddPointNumber = point
            });
            if (g2C_BattleProperty.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BattleProperty.Error.GetTipInfo());
                Log.DebugRed($"{g2C_BattleProperty.Error.GetTipInfo()}");
            }
            else
            {
                HideAddPointPanel();
                UIComponent.Instance.VisibleUI(UIType.UIHint, "添加成功");
                roleLevPoints.text = g2C_BattleProperty.PropertyPoint.ToString();//等级点数
                AddPointValue = g2C_BattleProperty.PropertyPoint;//等级点数
                if (g2C_BattleProperty.PropertyPoint == 0)
                {
                    SetArriteRedDot();
                }
                roleEntity.Property.ChangeProperValue(E_GameProperty.FreePoint, g2C_BattleProperty.PropertyPoint);

                foreach (G2C_BattleKVData item in g2C_BattleProperty.Info)
                {

                    roleEntity.Property.Set(item);

                }

                if (UIRoleInfoData.RecommendkeyValues.TryGetValue(propertyTrs.name.Split('_')[0], out int number))
                {
                    UIRoleInfoData.RecommendkeyValues[propertyTrs.name.Split('_')[0]] = number - point;
                }
                //RecommendAddPointInit();
                //刷新玩家的属性
                RefreshRoleProperty();
                //if (BeginnerGuideData.IsCompleteTrigger(48, 45))
                //{
                //    BeginnerGuideData.SetBeginnerGuide(48);
                //    UIMainComponent.Instance.SetBeginnerGuide(true);
                //    UIComponent.Instance.RemoveAll();
                //}
            }
        }
    }
}