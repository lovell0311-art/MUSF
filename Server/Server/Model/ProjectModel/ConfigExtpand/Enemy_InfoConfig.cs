using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// G怪物.xlsx-怪物
    /// </summary>
    public partial class Enemy_InfoConfig
    {

        /// <summary>
        /// 掉落
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> DropDicDic { get; set; }
        [JsonIgnore] public Dictionary<int, int> AttackTypeDic { get; set; }
        [JsonIgnore] public int AttackTypeDicSum { get; set; }

        public int Ran2 { get; set; }
        public override void InitExpand()
        {
            //DropDicDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(DropDic);
            AttackTypeDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(AttackType);
            AttackTypeDicSum = AttackTypeDic.Values.ToList().Sum();

            Ran2 = Ran * 2;

            switch (Monster_Type)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    switch (Id)
                    {
                        case 197:   // 血色城门
                        case 562:   // 变异幼龙
                        case 563:   // 变异冰后
                        case 564:   // 变异地狱蜘蛛
                        case 565:   // 变异恶魔
                        case 566:   // 变异黑炎魔
                        case 567:   // 变异灰熊
                        case 568:   // 变异石巨人
                        case 569:   // 变异巫师王
                            break;
                        default:
                            if (SpecialDrop == 0)
                            {
                                Log.Warning($"怪物配置表异常 {Name}:{Id} 特殊掉落组ID 不能为 0");
                            }
                            break;
                    }
                    break;
            }

        }
    }
}