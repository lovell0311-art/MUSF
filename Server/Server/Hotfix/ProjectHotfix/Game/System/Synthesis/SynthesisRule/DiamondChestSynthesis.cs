using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ETHotfix.GoldChestSynthesis;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 装备强化1
    /// </summary>
    [SynthesisRule(typeof(DiamondChestSynthesis))]
    public class DiamondChestSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            await TreasureBoxSynthesis.Run(synthesis, itemList, config, b_Response, 320409,mClientFinalR);
        }
    }
}