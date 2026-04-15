using System.Collections;
using UnityEngine;

namespace ETHotfix
{
    public partial class UIPetComponent
    {
        public UIPetInfo GetPetInfoToId(long petId)
        {
            foreach (var item in PetList)
            {
                if(item.petId == petId)
                {
                    return item;
                }
            }
            return null;
        }
    }
}