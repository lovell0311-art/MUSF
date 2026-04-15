using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 单个格子信息
    /// </summary>
    public class KnapsackGrid
    {
        #region 镶嵌使用的字段
        /// <summary>
        /// 该物品当前的数量
        /// </summary>
        public int curCount;
        /// <summary>
        /// 该物品所需的最大数量
        /// </summary>
        public int MaxCount;
        #endregion

        public GameObject GridObj { get; set; }
        public Image Image { get; set; }
        public KnapsackGridData Data { get; set; } = new KnapsackGridData();

        public static Color occupyColor = new Color(189, 255, 0);
        public static Color readyColor = new Color(255, 0, 226);
        /// <summary>
        /// 格子类型
        /// </summary>
        public E_Grid_Type Grid_Type = E_Grid_Type.Knapsack;

        /// <summary>
        /// 格子是否占用
        /// </summary>
        public bool isOccupy;
        public bool IsOccupy
        {
            get => isOccupy;
            set
            {
                this.isOccupy = value;
                if (this.isOccupy)
                {
                    Image.color = Color.gray;
                }
                else
                {
                    Image.color = Color.white;
                }
            }
        }
        public void ReadyColor()
        {
            if (this.isOccupy == false)
            {
                this.Image.color = new Color(255, 0, 226);
            }
        }
        public void ResetColor()
        {
            IsOccupy = isOccupy;
        }
    }
}
