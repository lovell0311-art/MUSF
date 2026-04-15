
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 卓越属性词条,百度翻译首字母，处理方式不同类型重新定义
    /// </summary>
    public enum Excellent
    {
        /// <summary>
        /// 卓越一击概率 加百分比形式
        /// </summary>
        EHP = 1,
        /// <summary>
        /// 攻击力增加 角色/Value形式
        /// </summary>
        API,
        /// <summary>
        /// 攻击力增加  加百分比形式
        /// </summary>
        APIP,
        /// <summary>
        /// 攻速增加 加值形式
        /// </summary>
        ASI,
        /// <summary>
        /// 杀死怪物时所获生命值  生命最大值/Value形式
        /// </summary>
        HGWKM,
        /// <summary>
        /// 杀死怪物时所获魔法值 魔法最大值/Value形式
        /// </summary>
        MVGWKM,
        /// <summary>
        /// 最大生命值增加 加百分比形式
        /// </summary>
        MHI,
        /// <summary>
        /// 最大魔法值增加 加百分比形式
        /// </summary>
        MMVI,
        /// <summary>
        /// 伤害减少 加百分比形式
        /// </summary>
        DR,
        /// <summary>
        /// 伤害反射 加百分比形式
        /// </summary>
        IR,
        /// <summary>
        /// 防御成功率 加百分比形式
        /// </summary>
        DSR,
        /// <summary>
        /// 杀死怪物时所获金增加 加百分比形式
        /// </summary>
        GGWKMI,
    }


}
