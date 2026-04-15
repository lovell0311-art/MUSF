using MongoDB.Bson.Serialization.Attributes;
using System;
using ETHotfix;
using CustomFrameWork;
using System.Drawing;

namespace ETModel
{
    public sealed partial class Pets : CombatSource
    {
        public override bool OpenObstacle => true;

        public DBPetsData dBPetsData { get; set; }
        public GamePlayer GamePlayer { get; set; }
        /// <summary>
        /// 宠物配置
        /// </summary>
        [BsonIgnore]
        public Pets_InfoConfig Config { get; set; }

        public C_HeroSkillSource SkillCurrent { get; set; }
        /// <summary>
        /// 吃药间隔
        /// </summary>
        public long TakeMedicineInterval { get; set; } = 0;
        public PetsRankInfo GetPetsRankInfo()
        {
            PetsRankInfo petsInfo = new PetsRankInfo();
            petsInfo.PetsConfigID = dBPetsData.ConfigID;
            petsInfo.PetsID = dBPetsData.PetsId;
            petsInfo.PetsLevel = dBPetsData.PetsLevel;
            petsInfo.IsDeath = IsDeath ? 1 : 0;

            if (DeathSleepTime != 0)
                petsInfo.DeathTime = (dBPetsData.DeathTime - Help_TimeHelper.GetNow()) + Config.Regen;

            if (petsInfo.DeathTime <= 0)
            {
                petsInfo.DeathTime = 0;
                petsInfo.IsDeath = 0;
                dBPetsData.DeathTime = 0;
                IsDeath = false;
                DeathSleepTime = 0;
            }

            if (dBPetsData.PetsTrialTime == 0)
            {
                petsInfo.PetsTrialTime = 0;
            }
            else
            {
                if (Help_TimeHelper.GetNowSecond() - dBPetsData.PetsTrialTime > 0)
                    petsInfo.PetsTrialTime = -1;
                else
                    petsInfo.PetsTrialTime = dBPetsData.PetsTrialTime;
            }
            petsInfo.Point = dBPetsData.AttributePoint;
            petsInfo.ELvis = dBPetsData.EnhanceLv;
            return petsInfo;
        }
        public PetsInfo GetPetsInfo(out bool IsSetDB)
        {
            PetsInfo petsInfo = new PetsInfo();
            IsSetDB = false;

            petsInfo.PetsConfigID = dBPetsData.ConfigID;
            petsInfo.PetsID = dBPetsData.PetsId;
            petsInfo.PetsName = dBPetsData.PetsName;
            petsInfo.PetsType = Config.PetsType;
            petsInfo.PetsLevel = dBPetsData.PetsLevel;
            petsInfo.PetsLVpoint = dBPetsData.AttributePoint;
            petsInfo.PetsHP = dBPetsData.PetsHP;
            petsInfo.PetsHPMax = GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
            petsInfo.PetsMP = dBPetsData.PetsMP;
            petsInfo.PetsMPMax = GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
            petsInfo.PetsSTR = dBPetsData.PetsSTR;
            petsInfo.PetsDEX = dBPetsData.PetsDEX;
            petsInfo.PetsPSTR = dBPetsData.PetsPSTR;
            petsInfo.PetsPINT = dBPetsData.PetsPINT + dBPetsData.PintAdd;
            petsInfo.PetsMinATK = GetNumerialFunc(E_GameProperty.MinAtteck);
            petsInfo.PetsMaxATK = GetNumerialFunc(E_GameProperty.MaxAtteck);
            petsInfo.PetsASM = GetNumerialFunc(E_GameProperty.AtteckSuccessRate);
            petsInfo.PetsPAR = GetNumerialFunc(E_GameProperty.PVPAtteckSuccessRate);
            petsInfo.PetsDEF = GetNumerialFunc(E_GameProperty.Defense);
            petsInfo.PetsDFR = GetNumerialFunc(E_GameProperty.DefenseRate);
            petsInfo.PetsSPD = GetNumerialFunc(E_GameProperty.AttackSpeed);
            petsInfo.PetsPDFR = GetNumerialFunc(E_GameProperty.PVPDefenseRate);
            if ((int)GameHeroSexType == 0)
            {
                petsInfo.PetsSATK = 200 + (dBPetsData.PetsPINT + dBPetsData.PintAdd) / 10 / 100;
            }
            else
            {
                petsInfo.PetsSATK = 200 + dBPetsData.PetsSTR / 10 / 100;//百分之两百+
            }
            petsInfo.PetsMinINT = (dBPetsData.PetsPINT + dBPetsData.PintAdd) / 4;
            petsInfo.PetsMaxINT = (dBPetsData.PetsPINT + dBPetsData.PintAdd) / 2;
            petsInfo.PetsEXP = dBPetsData.PetsExp;
            petsInfo.PetsSkillID = dBPetsData.UseSkillID;
            petsInfo.IsDeath = IsDeath ? 1 : 0;
            petsInfo.DeathTime = DeathSleepTime - Help_TimeHelper.GetNow();
            if (DeathSleepTime != 0 && petsInfo.DeathTime <= 0)
            {
                IsDeath = false;
                DeathSleepTime=0;
                dBPetsData.DeathTime = 0;
                dBPetsData.PetsHP = GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                petsInfo.PetsHP = dBPetsData.PetsHP;
                dBPetsData.PetsMP = GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                petsInfo.PetsMP = dBPetsData.PetsMP;
                petsInfo.IsDeath = 0;
                petsInfo.DeathTime = 0;
                IsSetDB = true;
            }
            if (petsInfo.IsDeath == 0)
                petsInfo.DeathTime = 0;

            if (dBPetsData.PetsTrialTime == 0)
            {
                petsInfo.PetsTrialTime = 0;
            }
            else
            {
                if (Help_TimeHelper.GetNowSecond() - dBPetsData.PetsTrialTime >= 0)
                {
                    dBPetsData.IsDisabled = 1;
                    petsInfo.PetsTrialTime = -1;
                    IsSetDB = true;
                }
                else
                    petsInfo.PetsTrialTime = dBPetsData.PetsTrialTime;
            }
            petsInfo.Elv = dBPetsData.EnhanceLv;
            petsInfo.AdvancedLevel = dBPetsData.AdvancedLevel;
            return petsInfo;
        }
        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }
            dBPetsData = null;
            GamePlayer = null;
            Config = null;
            SkillCurrent = null;
            base.Dispose();
        }
    }
}