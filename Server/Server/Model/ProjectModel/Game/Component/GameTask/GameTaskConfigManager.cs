using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using CustomFrameWork.Baseic;
using CustomFrameWork;

namespace ETModel
{
    public class GameTaskConfigManager : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 前置任务配置字典
        /// </summary>
        public MultiMap<int,int> BeforeTaskConfigDict = new MultiMap<int, int>();

        public Dictionary<int, GameTaskConfig> AllConfigDict = new Dictionary<int, GameTaskConfig>();

        /// <summary>
        /// 奖励物品
        /// k 任务id，v RewardItem.ConfigId
        /// </summary>
        public MultiMap<int, int> RewardItemsDict = new MultiMap<int, int>();

        /// <summary>
        /// 掉落物品
        /// k 需要击杀的怪物Id v (k 任务id v 掉落的物品id)
        /// </summary>
        public Dictionary<int, MultiMap<int,GameTask_DropItemConfig>> DropItemConfigDict = new Dictionary<int, MultiMap<int, GameTask_DropItemConfig>>();


        public override void Dispose()
        {
            if (IsDisposeable) return;

            BeforeTaskConfigDict.Clear();
            AllConfigDict.Clear();

            base.Dispose();
        }



        

    }
}
