using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    //本地玩家出战的宠物
    public class PetArchiveInfoManager : SimpleSingleton<PetArchiveInfoManager>
    {
        /// <summary>
        /// 本地玩家出战的宠物ID
        /// </summary>
        public long petId;
        /// <summary>
        /// 本地玩家出战的宠物名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 本地玩家出战的宠物的最大血量
        /// </summary>
        public long HpMaxValue;
        /// <summary>
        /// 本地玩家出战的宠物的最大蓝量
        /// </summary>
        public long MpMaxValue;
        public override void Disponse()
        {
            Release();
            base.Disponse();
        }
    }
}