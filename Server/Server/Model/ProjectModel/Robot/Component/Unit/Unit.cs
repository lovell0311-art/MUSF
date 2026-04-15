using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ETModel.Robot
{
    public enum UnitType : byte
    {
        None,
        Player,
        Monster,
        Item,
        Pet,
        Summoned,
        Npc,
    }


    public class Unit : Entity
    {
        public Map CurrentMap = null;

        public Vector2Int Position = new Vector2Int();

        public UnitType UnitType = UnitType.None;

        public bool IgnoreCollision = false;
    }
}
