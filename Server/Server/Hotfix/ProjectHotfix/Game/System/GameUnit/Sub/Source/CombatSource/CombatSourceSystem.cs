using ETModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ETHotfix
{
    public static partial class CombatSourceSystem
    {
        public static void EnterMap(this CombatSource self,MapComponent targetMap,int x,int y,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            targetMap.Enter(self,x,y,false,
                callerLineNumber,
                callerMemberName,
                callerFilePath);
        }

        public static void EnterMap(this CombatSource self, MapComponent targetMap, Vector2Int targetPos,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            targetMap.Enter(self, targetPos,false,
                callerLineNumber,
                callerMemberName,
                callerFilePath);
        }

        public static void Move(this CombatSource self,int x,int y,bool needMove = true)
        {
            self.CurrentMap.Move(self, x, y, needMove);
        }

        public static void Move(this CombatSource self, Vector2Int targetPos, bool needMove = true)
        {
            self.CurrentMap.Move(self, targetPos, needMove);
        }

        public static void LeaveMap(this CombatSource self,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            self.CurrentMap?.Leave(self,
                callerLineNumber,
                callerMemberName,
                callerFilePath);
        }

        public static void SwitchMap(this CombatSource self, MapComponent targetMap, int x, int y,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            if(self.CurrentCell == null)
            {
                // 没进入地图，直接进入
                targetMap.Enter(self, x, y,false,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }
            else
            {
                targetMap.Switch(self, x, y,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }
        }

        public static void SwitchMap(this CombatSource self, MapComponent targetMap, Vector2Int targetPos,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            if (self.CurrentCell == null)
            {
                // 没进入地图，直接进入
                targetMap.Enter(self, targetPos,false,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }
            else
            {
                targetMap.Switch(self, targetPos,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }
        }

    }
}
