using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 装备套装组件，用来管理套装属性
    /// </summary>
    public class EquipmentSetComponent : TCustomComponent<Player>
    {
        /// <summary>
        /// 装备中包含的全部套装
        /// </summary>
        public Dictionary<int, ItemSet> AllItemSet = new Dictionary<int, ItemSet>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            //清理数据
            AllItemSet.Clear();

            base.Dispose();

        }
    }
}
