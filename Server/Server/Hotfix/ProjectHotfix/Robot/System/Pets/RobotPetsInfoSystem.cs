using ETModel;
using ETModel.Robot;


namespace ETHotfix.Robot
{
    public static partial class RobotPetsInfoSystem
    {
        public static void FromProtoPetsInfo(this RobotPetsInfo self, PetsInfo info)
        {
            self.IsDeath = info.IsDeath == 1 ? true : false;
            self.DeathTime = info.DeathTime;
            self.PetsTrialTime = info.PetsTrialTime;

            NumericComponent numeric = self.GetComponent<NumericComponent>();
            numeric.SetNoEvent((int)E_GameProperty.Level, info.PetsLevel);
            numeric.SetNoEvent((int)E_GameProperty.FreePoint, info.PetsLVpoint);
            numeric.SetNoEvent((int)E_GameProperty.PROP_HP, info.PetsHP);
            numeric.SetNoEvent((int)E_GameProperty.PROP_HP_MAX, info.PetsHPMax);
            numeric.SetNoEvent((int)E_GameProperty.PROP_MP, info.PetsMP);
            numeric.SetNoEvent((int)E_GameProperty.PROP_MP_MAX, info.PetsMPMax);
            numeric.SetNoEvent((int)E_GameProperty.Property_Strength, info.PetsSTR);
            numeric.SetNoEvent((int)E_GameProperty.Property_Agility, info.PetsDEX);
            numeric.SetNoEvent((int)E_GameProperty.Property_BoneGas, info.PetsPSTR);
            numeric.SetNoEvent((int)E_GameProperty.Property_Willpower, info.PetsPINT);
            numeric.SetNoEvent((int)E_GameProperty.MinAtteck, info.PetsMinATK);
            numeric.SetNoEvent((int)E_GameProperty.MaxAtteck, info.PetsMaxATK);
            numeric.SetNoEvent((int)E_GameProperty.AtteckSuccessRate, info.PetsASM);
            numeric.SetNoEvent((int)E_GameProperty.PVPAtteckSuccessRate, info.PetsPAR);
            numeric.SetNoEvent((int)E_GameProperty.Defense, info.PetsDEF);
            numeric.SetNoEvent((int)E_GameProperty.DefenseRate, info.PetsDFR);
            numeric.SetNoEvent((int)E_GameProperty.AttackSpeed, info.PetsSPD);
            numeric.SetNoEvent((int)E_GameProperty.PVPDefenseRate, info.PetsPDFR);

        }
    }
}
