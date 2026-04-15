using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 掉落物品信息类
    /// </summary>
    public class ItemDropDataInfo
    {
        public int Key;// 掉落类型
        public long Value;// 数量
                          // 1 << 0 .有技能
                          // 1 << 2 .有幸运
                          // 1 << 3 .有卓越
                          // 1 << 4 .有套装
                          // 1 << 5 .有镶嵌
                          // (Quality & 1 << ? == 1 << ?) ? true : false
        public int Quality;// 品质
        public int PosX;
        public int PosY;
        public long InstanceId;//物品实例对象Id
                               // 拾取保护时间 时间戳 utc 秒
        public long ProtectTick;
        // 击杀玩家列表，谁可以拾取
        public List<long> KillerId;
        // 物品强化等级
        public int Level;
        //套装ID
        public int SetId;
        //生成类型 0：其他 1：玩家丢弃 2：怪物掉落 3：宝箱掉落
        public int CreatType;
    }
}