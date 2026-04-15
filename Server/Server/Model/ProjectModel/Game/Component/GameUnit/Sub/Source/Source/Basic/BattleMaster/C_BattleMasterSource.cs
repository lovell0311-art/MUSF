using System;
using System.Collections.Generic;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    public partial class C_BattleMasterSource : ADataContext<int>
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// 数据是不是有错误
        /// </summary>
        public bool IsDataHasError { get; set; }

        public override void ContextAwake(int b_Id)
        {
            Id = b_Id;
            IsDataHasError = true;
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            IsDataHasError = true;
            Id = default;
            Clear();
            base.Dispose();
        }
    }
    public partial class C_BattleMasterSource
    {
        public virtual void Clear()
        {

        }
    }
    public partial class C_BattleMasterSource
    {
        public virtual bool TryUse(GamePlayer b_Attacker, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            return false;
        }

        public virtual bool UseSkill(GamePlayer b_Attacker, BattleComponent b_BattleComponent, bool b_AddPoint)
        {
            return false;
        }

        protected int GetMasterValue(int b_ConfigValue, int b_ValueType)
        {
            if (b_ValueType == 1) return (int)(b_ConfigValue / 100f);
            if (b_ValueType == 2) return (int)(b_ConfigValue / 10000f);

            return b_ConfigValue;
        }
    }
}
