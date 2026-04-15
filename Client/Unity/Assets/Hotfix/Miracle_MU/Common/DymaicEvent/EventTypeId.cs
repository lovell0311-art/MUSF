using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{   
    /// <summary>
    /// 事件类型
    /// </summary>
    public class EventTypeId
    {
        /// <summary>分发摇杆值</summary>
        public const string UI_JOYSTICK_VALUE = "UI_JOYSTICK_VALUE";
        /// <summary>摇杆抬起</summary>
        public const string UI_JOYSTICK_UP = "UI_JOYSTICK_UP"; 
        /// <summary>摇杆按下</summary>
        public const string UI_JOYSTICK_DOWN = "UI_JOYSTICK_DOWN";
        /// <summary>本地玩家挂机配置改变</summary>
        public const string LOCALROLE_ONHOOKINFO_CAHNGE = "LOCALROLE_ONHOOKINFO_CAHNGE";
        /// <summary>本地玩家死亡</summary>
        public const string LOCALROLE_DEAD = "LOCALROLE_DEAD"; 
        public const string LOCALROLE_GRIDCHANGE = "LOCALROLE_GRIDCHANGE"; 
        
        /// <summary>仓库金币改变</summary>
        public const string WARE_GOLDCOIN_CHANGE = "WARE_GOLDCOIN_CHANGE";
        /// <summary>仓库添加物品</summary>
        public const string WARE_ADD_ITEM= "WARE_ADD_ITEM";
        
        //金币变动
        public const string GLOD_CHANGE= "GLOD_CHANGE";
        //移除背包中的物品
        public const string RemoveKnapsack= "RemoveKnapsack";

        /// <summary>
        /// 场景加载进度
        /// </summary>
        public const string LOAD_SCENE_PROGRESS = "LOAD_SCENE_PROGRESS";

        public const string Hit = "Hit";


        public const string LugaugeRefrash = "LugaugeRefrash";

        /// <summary>
        /// 进入收费地图
        /// </summary>
        public const string CHARGE_MAP= "CHARGE_MAP";


    }
}