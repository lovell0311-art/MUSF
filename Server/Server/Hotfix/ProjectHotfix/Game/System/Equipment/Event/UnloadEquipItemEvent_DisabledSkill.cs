using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using TencentCloud.Bri.V20190328.Models;
using System.Threading.Tasks;
namespace ETHotfix
{
    /// <summary>
    /// 禁用技能
    /// </summary>
    [EventMethod("UnloadEquipItem")]
    public class UnloadEquipItemEvent_DisabledSkill : ITEventMethodOnRun<ETModel.EventType.UnloadEquipItem>
    {
        public void OnRun(ETModel.EventType.UnloadEquipItem args)
        {
            if (!args.item.HaveSkill()) return;
            // 禁用技能，不保存
            var studySkillId = args.item.GetEquipSkillId((E_GameOccupation)args.unit.Data.PlayerTypeId);

            DataCacheManageComponent mDataCacheManageComponent = args.unit.Player.GetCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
            var mDatalist = mDataCache?.DataQuery(p => p.GameUserId == args.unit.Player.GameUserId);
            if (mDatalist != null && mDatalist.Count > 0)
            {
                DBGameSkillData mData = mDatalist[0];
                if (mData.SkillId.Contains(studySkillId))
                {
                    return;
                }
            }

            int Cnt = 0;
            var equipComponent = args.unit.Player.GetCustomComponent<EquipmentComponent>();
            if (equipComponent != null)
            {
                foreach (var Item in equipComponent.EquipPartItemDict)
                {
                    if (Item.Value.GetEquipSkillId((E_GameOccupation)args.unit.Data.PlayerTypeId) == studySkillId)
                        Cnt++;
                }
                foreach (var Item in equipComponent.TempSlotDict)
                {
                    if (Item.Value.GetEquipSkillId((E_GameOccupation)args.unit.Data.PlayerTypeId) == studySkillId)
                        Cnt++;
                }
            }
            if (Cnt >= 1) return;

            if (args.unit.SkillGroup.TryGetValue(studySkillId, out C_HeroSkillSource skill) == true)
            {
                args.unit.SkillGroup.Remove(studySkillId);

                G2C_DisabledSkillSingle_notice mSkillSingle = new G2C_DisabledSkillSingle_notice();
                mSkillSingle.SkillId = studySkillId;
                args.unit.Player.Send(mSkillSingle);

                skill.Dispose();
            }
        }
    }
}
