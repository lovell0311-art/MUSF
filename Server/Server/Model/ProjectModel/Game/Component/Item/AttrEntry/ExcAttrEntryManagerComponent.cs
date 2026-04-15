using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 卓越属性词条管理组件
    /// </summary>
    public class ExcAttrEntryManagerComponent : TCustomComponent<CustomComponent>
    {
        // 武器、项链 词条
        public RandomSelector<int> __WeaponAttrEntry = new RandomSelector<int>();
        // 防具、戒指 词条
        public RandomSelector<int> __ArmorAttrEntry = new RandomSelector<int>();
        //宠物词条
        public RandomSelector<int> __PetsAttrEntry = new RandomSelector<int>();
        /// <summary>旗帜 /// </summary>
        public RandomSelector<int> __FlagAttrEntry = new RandomSelector<int>();
        /// <summary>
        /// 卓越属性词条数
        /// </summary>
        public RandomSelector<int> ExcAttrEntryCount = new RandomSelector<int>();
        /// <summary>
        /// 宠物卓越属性词条数
        /// </summary>
        public RandomSelector<int> PetsExcAttrEntryCount = new RandomSelector<int>();
        /// <summary>
        /// 旗帜 卓越属性词条数
        /// </summary>
        public RandomSelector<int> FlagExcAttrEntryCount = new RandomSelector<int>();
        /// <summary>
        /// 荧光宝石属性
        /// </summary>
        public Dictionary<int,Dictionary<int,List<int>>> YingGuangBaoShi = new Dictionary<int, Dictionary<int, List<int>>>();
        public override void Dispose()
        {
            if (IsDisposeable) return;

            //清理数据
            __WeaponAttrEntry.Clear();
            __ArmorAttrEntry.Clear();
            __PetsAttrEntry.Clear();
            ExcAttrEntryCount.Clear();
            __FlagAttrEntry.Clear();
            YingGuangBaoShi.Clear();
            base.Dispose();
        }
    }
}
