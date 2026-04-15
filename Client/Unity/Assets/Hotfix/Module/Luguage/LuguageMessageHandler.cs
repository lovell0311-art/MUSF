using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
   //[ETModel.Event(ETModel.EventIdType.LuguageChange)]
    public class LuguageMessageHandler : ETModel.AEvent<ETModel.LuguageMessage>
    {
        public override void Run(ETModel.LuguageMessage message)
        {
            Debug.Log("∂‡”Ô—‘ ¬º˛");
          //  Game.Scene.GetComponent<LuguageComponent>().SetKeyToLuguage(message.luguageMono.gameObject, message.luguageMono.key);
        }
    }
}