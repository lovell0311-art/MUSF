using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public partial class GameTask_MainConfig
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
            conf.AutoReceiveReward = (AutoReceiveReward > 0) ? true : false;

            conf.TaskBeforeId = TaskBeforeId;

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
    public partial class PassportTask_PassportConfig
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
            conf.AutoReceiveReward = (AutoReceiveReward > 0) ? true : false;
            conf.RewardUCoin = RewardUCoin;
            conf.TaskBeforeId = TaskBeforeId;

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
