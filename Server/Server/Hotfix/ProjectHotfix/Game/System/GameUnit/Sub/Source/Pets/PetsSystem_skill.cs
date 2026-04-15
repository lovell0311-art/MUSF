using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{



    public static partial class PetsSystem
    {

        public static void AwakeSkill(this Pets b_Component)
        {
            if (b_Component.SkillGroup == null) b_Component.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
        }
        //public static async void DataUpdateSkill(this Pets b_Component)
        //{
        //}
    }
}