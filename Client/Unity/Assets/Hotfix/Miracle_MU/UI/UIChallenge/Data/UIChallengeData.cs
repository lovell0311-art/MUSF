using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public class EnemyConfig_Challenge
    {
        public EnemyConfig_ChallengeConfig config_ChallengeConfig;
        public bool IsSelect = false;
    }
    public enum BossType
    {
        Gold = 1,
        Boss,
        CBT,
        Elite
    }
    public class EnemyConfig
    {
        public EnemyConfig_Challenge enemyConfig;
        public GameObject item;
    }
}
