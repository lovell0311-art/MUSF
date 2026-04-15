using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public partial class GameTask_CareerChangeConfig
    {

        public GameTaskConfig ToGameTaskConfig()
        {
            GameTaskConfig conf = new GameTaskConfig();
            conf.ConfigId = Id;
            conf.TaskActionType = (EGameTaskActionType)TaskActionType;
            conf.TaskTargetId = TaskTargetId;
            conf.TaskTargetCount = TaskTargetCount;
            conf.RewardExp = RewardExp;
            conf.RewardCoin = RewardCoin;
            conf.CustomReward = CustomReward;

            conf.TaskBeforeId = TaskBeforeId;

            conf.ReqCoin = ReqCoin;
            conf.ReqLevelMin = ReqLevelMin;
            conf.ReqLevelMax = ReqLevelMax;

            conf.Spell = Spell;
            conf.Swordsman = Swordsman;
            conf.Archer = Archer;
            conf.Spellsword = Spellsword;
            conf.Holyteacher = Holyteacher;
            conf.SummonWarlock = SummonWarlock;
            conf.Combat = Combat;
            conf.GrowLancer = GrowLancer;

            return conf;
        }

    }
}
