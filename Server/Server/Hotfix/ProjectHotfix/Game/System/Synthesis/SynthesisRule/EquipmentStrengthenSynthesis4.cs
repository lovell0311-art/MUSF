
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using static ETHotfix.EquipmentStrengthenSynthesis1;

namespace ETHotfix
{
    /// <summary>
    /// 装备强化1
    /// </summary>
    [SynthesisRule(typeof(EquipmentStrengthenSynthesis4))]
    public class EquipmentStrengthenSynthesis4 : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            await EquipmentStrengthenSynthesis1_9.Run(synthesis, itemList, config, b_Response, 3, mClientFinalR);
        }
    }
}