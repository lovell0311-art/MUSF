using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETHotfix
{
    
    /// <summary>
    /// 项目 设置数据
    /// </summary>
    public partial class LocalDataJsonComponent
    {
        public GameSetInfo gameSetInfo;

        public void InitGameSetInfo() 
        {
            gameSetInfo = LoadData<GameSetInfo>(LocalJsonDataKeys.GameSetInfo) ?? new GameSetInfo();
        }

        public void ChangeSetInfo() { }
    }
}
