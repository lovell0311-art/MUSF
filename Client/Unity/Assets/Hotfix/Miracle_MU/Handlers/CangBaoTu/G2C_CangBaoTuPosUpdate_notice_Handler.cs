using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Text;

namespace ETHotfix
{
    /// <summary>
    /// 藏宝图 位置通知
    /// </summary>
    [MessageHandler]
    public class G2C_CangBaoTuPosUpdate_notice_Handler : AMHandler<G2C_CangBaoTuPosUpdate_notice>
    {
        protected override void Run(ETModel.Session session, G2C_CangBaoTuPosUpdate_notice message)
        {

            StringBuilder builder=new StringBuilder();
            Npc_InfoConfig npc_Info = ConfigComponent.Instance.GetItem<Npc_InfoConfig>(message.NpcConfigID);
            EnemyConfig_InfoConfig enemy_Info = ConfigComponent.Instance.GetItem<EnemyConfig_InfoConfig>(message.NpcConfigID);

            if (npc_Info != null)
            {
                builder.Append($"<color=red>{npc_Info.Name}</color>在<color=green>{((SceneName)message.MapIndex).GetSceneName()}</color> <color=yellow>[{message.PosX},{message.PosY}]</color> 已生成");

            } else if (enemy_Info!= null) {
                builder.Append($"<color=red>{enemy_Info.Name}</color>在<color=green>{((SceneName)message.MapIndex).GetSceneName()}</color> <color=yellow>[{message.PosX},{message.PosY}]</color> 已生成");
            }
            Log.DebugGreen($"使用 -> {builder} 配置表Id -> {message.NpcConfigID}");
            UIMainComponent.Instance.ShowFuBenInfo(builder.ToString());
            UIComponent.Instance.VisibleUI(UIType.UIHint,builder.ToString());
            GlobalDataManager.astarNode = AstarComponent.Instance.GetNode(message.PosX, message.PosY);
            GlobalDataManager.MapId=message.MapIndex;

            // 记录 用于小地图显示 icon
            TreasureMapComponent.Instance.AllPoint[message.Id] = new TreasurePoint()
            {
                Id = message.Id,
                MapId = message.MapIndex,
                NpcConfigId = message.NpcConfigID,
                PosX = message.PosX,
                PosY = message.PosY,
                TreasureType = message.TreasureType,
            };
        }
    }
}
