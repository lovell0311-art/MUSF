using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {
        /// <summary>
        /// 初始化角色装备加成
        /// </summary>
        /// <param name="b_GamePlayer"></param>
        /// <param name="b_Config"></param>
        public static void AwakeEquipment(this GamePlayer b_GamePlayer)
        {
            b_GamePlayer.EquipPropertyDic.Clear();
        }

        public static void AddEquipProperty(this GamePlayer gamePlayer, E_GameProperty key, int value)
        {
            if(gamePlayer.EquipPropertyDic.ContainsKey(key))
            {
                gamePlayer.EquipPropertyDic[key] += value;
            }
            else
            {
                gamePlayer.EquipPropertyDic.Add(key, value);
            }
        }


    }
}