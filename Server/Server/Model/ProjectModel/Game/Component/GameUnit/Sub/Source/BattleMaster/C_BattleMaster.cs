using System;
using System.Collections.Generic;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    public partial class C_BattleMaster
    {
        public virtual void AfterAwake()
        {

        }
        public virtual void DataUpdate()
        {

        }
    }
    public partial class C_BattleMaster
    {
        public string Describe { get; set; }
        /// <summary>
        /// 升级消耗
        /// </summary>
        public int Consume { get; set; }
        public List<int> UpLevel { get; set; }

        public virtual void AfterClear()
        {
            Consume = default;
            if (Describe != null)
                Describe = null;
            if (UpLevel != null)
            {
                UpLevel.Clear();
                UpLevel = null;
            }
        }
    }
    public partial class C_BattleMaster : C_BattleMasterSource
    {
        [JsonIgnore]
        private C_ConfigInfo config;
        [JsonIgnore]
        public C_ConfigInfo Config
        {
            get => config;
            private set
            {
                config = value;
                if (config == null)
                {
                    BattleComponent.Log("大师技能配置 赋值null", true);
                }
            }
        }

        public DBMasterData Data { get; private set; }
        public void SetConfig(C_ConfigInfo b_Config, DBMasterData b_Data)
        {
            Config = b_Config;
            Data = b_Data;
        }
        public override void Clear()
        {
            Config = null;
            Data = null;
            AfterClear();
        }
    }
}
