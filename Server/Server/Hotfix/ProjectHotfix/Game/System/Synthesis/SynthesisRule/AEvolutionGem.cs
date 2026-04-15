
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    /// <summary>
    /// 龙王旗帜合成
    /// </summary>
    [SynthesisRule(typeof(AEvolutionGem))]
    public class AEvolutionGem : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response,int mClientFinalR)
        {
            await JingHuaBaoShi.Run(synthesis, itemList, config, b_Response, mClientFinalR);
        }
    }
}