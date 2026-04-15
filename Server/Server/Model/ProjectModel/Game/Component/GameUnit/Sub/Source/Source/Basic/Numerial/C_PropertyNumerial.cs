using System;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Collections.Generic;
namespace ETModel
{


    public partial class C_PropertyNumerial : ADataContext
    {
        public virtual int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            return 0;
        }
        public virtual int Run(Summoned b_Component, bool b_HasTemporary = true)
        {
            return 0;
        }
        public virtual int Run(HolyteacherSummoned b_Component, bool b_HasTemporary = true)
        {
            return 0;
        }
        public virtual int Run(Enemy b_Component, bool b_HasTemporary = true)
        {
            return 0;
        }
    }
}