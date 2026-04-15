
using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class HeroSkillAttribute : BaseAttribute
    {
        public int BindId { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BattleMasterAttribute : BaseAttribute
    {
        public int BindId { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PropertyNumerialAttribute : BaseAttribute
    {
        public int BindId { get; set; }
    }


}
