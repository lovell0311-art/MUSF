using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    


    public static partial class GamePlayerSystem
    {
    

        private static int Numerial_base(GamePlayer b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_Component.GamePropertyDic.TryGetValue(b_GameProperty, out var mGamePropertyValue))
            {
                mResult += mGamePropertyValue;
            }
            // 装备属性应用
            if (b_Component.EquipPropertyDic.TryGetValue(b_GameProperty, out int mEquipPropertyValue))
            {
                mResult += mEquipPropertyValue;
            }

            switch (b_GameProperty)
            {
                case E_GameProperty.Property_Strength:
                    {
                        mResult += b_Component.Config.Strength + b_Component.Data.Strength + b_Component.Data.Strength2;
                    }
                    break;
                case E_GameProperty.Property_Willpower:
                    {
                        mResult += b_Component.Config.Willpower + b_Component.Data.Willpower + b_Component.Data.Willpower2;
                    }
                    break;
                case E_GameProperty.Property_Agility:
                    {
                        mResult += b_Component.Config.Agility + b_Component.Data.Agility + b_Component.Data.Agility2;
                    }
                    break;
                case E_GameProperty.Property_BoneGas:
                    {
                        mResult += b_Component.Config.BoneGas + b_Component.Data.BoneGas + b_Component.Data.BoneGas2;
                    }
                    break;
                case E_GameProperty.Property_Command:
                    {
                        mResult += b_Component.Config.Command + b_Component.Data.Command + b_Component.Data.Command2;
                    }
                    break;
                default:
                    break;
            }
          
            return mResult;
        }
    }
}