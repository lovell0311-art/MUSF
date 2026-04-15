
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 装备强化+11~+12
    /// </summary>
    [SynthesisRule(typeof(InEquipmentNumber4))]
    public class InEquipmentNumber4 : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            await InEquipmentNumber.Run(synthesis, itemList, config, b_Response, 4, mClientFinalR);
        }
    }
}