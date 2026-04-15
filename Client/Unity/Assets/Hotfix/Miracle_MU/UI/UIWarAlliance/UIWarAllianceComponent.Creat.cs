 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

using System.Linq;

namespace ETHotfix
{

    /// <summary>
    /// 创建战盟 模块
    /// </summary>
    public partial class UIWarAllianceComponent
    {

        public InputField inputField;
        public string warName;

        ReferenceCollector collector_Creat;

        /// <summary>
        /// 0->叉叉
        /// 1->黑色
        /// 2->灰色
        /// 3->白色
        /// 4->红色
        /// 5->橘色
        /// 6->黄色
        /// 7->绿色
        /// 8->草绿
        /// 9->嫩绿
        /// 10->天蓝
        /// 11->蓝色
        /// 12->深蓝
        /// 13->紫色
        /// 14->粉色
        /// 15->紫色
        /// </summary>
        public int curChooseColorIndex = 0;//当前选择的 颜色序号

        public Dictionary<int,int> DrawIconDic=new Dictionary<int,int>();//战盟 图标字典  key:格子序号 value:格子对应的精灵编号

        public List<int> badgeIcon=new List<int>();
        public void InitCreat()
        {
            collector_Creat = CreatPanel.GetReferenceCollector();
            inputField = collector_Creat.GetInputField("WarName_InputField");
            inputField.onValueChanged.AddSingleListener((value) =>
            {
                if (string.IsNullOrEmpty(value)) return;
                if (SystemUtil.IsInvaild(value))//是否包含非法字符
                {
                    warName = SystemUtil.ReplaceStr(value);
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"包含非法字符 请重新输入战盟昵称");
                    inputField.text = string.Empty;
                    return;
                }
                else
                {
                    warName = value;
                }
            });

            collector_Creat.GetButton("LastStepBtn").onClick.AddSingleListener(() => 
            {
                Show(E_WarType.Init);
            });
            collector_Creat.GetButton("NextStepBtn").onClick.AddSingleListener(() => 
            {
                if (string.IsNullOrEmpty(inputField.text))
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"请您先输入战盟的名字");
                    return;
                }
                Dic2List();
                Show(E_WarType.Preview);
            });
            InitColorIcons();
            InitChooseColor();
        }
        /// <summary>
        /// 初始化 战盟徽章 格子
        /// </summary>
        private void InitColorIcons()
        {
            Transform icons = collector_Creat.GetGameObject("Icons").transform;
            Transform temp = icons.GetChild(0);
            int childCount = icons.childCount;
            for (int i = 0; i < 64; i++)
            {
                Transform icon;
                if (i < childCount)
                {
                    icon = icons.GetChild(i);
                }
                else
                {
                    icon = GameObject.Instantiate<Transform>(temp, icons);
                }
                int number = i;
                icon.name = number.ToString();
                Toggle toggle = icon.GetComponent<Toggle>();
                toggle.onValueChanged.AddSingleListener((value) =>
               {
                   DrawEvent(value,toggle, number);
               });
                DrawIconDic[number] = curChooseColorIndex;
            }
        }
        /// <summary>
        /// 绘制 图标 事件
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="toggle"></param>
        /// <param name="gridNumber"></param>

        private void DrawEvent(bool isOn, Toggle toggle,int gridNumber)
        {
            if (isOn)
            {
                toggle.graphic.GetComponent<Image>().sprite = GetSprite();
                DrawIconDic[gridNumber] = curChooseColorIndex;//缓存 每个格子 所使用的精灵的序号
            }
            else
            {
                if (DrawIconDic[gridNumber] != 0)
                DrawIconDic[gridNumber] = 0;
            }

        }

        public void Dic2List() 
        {
            badgeIcon = DrawIconDic.Values.ToList();
        }


        /// <summary>
        /// 重置战盟图标格子
        /// </summary>
        public void ResetIcon() 
        {
            //curChooseColorIndex = 0;
            Transform icons = collector_Creat.GetGameObject("Icons").transform;
            for (int i = 0,length= icons.childCount; i < length; i++)
            {
                icons.GetChild(i).GetComponent<Toggle>().graphic.GetComponent<Image>().sprite = GetSprite();
                icons.GetChild(i).GetComponent<Toggle>().isOn = false;
                DrawIconDic[i] = 0;
            }
        }

        /// <summary>
        /// 初始化 颜色 选择事件
        /// </summary>
        public void InitChooseColor()
        {
            Transform Colors = collector_Creat.GetGameObject("Colors").transform;
            for (int i = 0, length = Colors.childCount; i < length; i++)
            {
                Toggle toggle = Colors.GetChild(i).GetComponent<Toggle>();
                int colorindex = i;
                toggle.onValueChanged.AddSingleListener((value) => ChooseColorEcent(value, colorindex));
            }
        }

        private void ChooseColorEcent(bool isOn, int colorIndex)
        {
            if (isOn)
                curChooseColorIndex = colorIndex;
        }
       
    }
}