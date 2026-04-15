using ETModel;
using UnityEngine;

namespace ETHotfix
{
	public static class LayerNames
	{
		/// <summary>
		/// UI层
		/// </summary>
		public const string UI = "UI";

		/// <summary>
		/// 游戏单位层
		/// </summary>
		public const string UNIT = "Unit";

		/// <summary>
		/// 地形层
		/// </summary>
		public const string MAP = "Map";

		/// <summary>
		/// 默认层
		/// </summary>
		public const string DEFAULT = "Default";
		
		public const string HIDDEN = "Hidden";
		/// <summary>
		/// Render相机渲染曾
		/// </summary>
		public const string RENDER = "RenderRole";
		/// <summary>
		/// 玩家层
		/// </summary>
		public const string ROLE = "Role";
		/// <summary>
		/// 本地玩家
		/// </summary>
		public const string LOCALROLE = "LocalRole";
		/// <summary>
		/// 怪物层
		/// </summary>
		public const string MONSTER = "Monster";
		/// <summary>
		/// NPC层
		/// </summary>
		public const string NPC = "NPC";


		/// <summary>
		/// 通过Layers名字得到对应层
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static int GetLayerInt(string name)
		{
			return LayerMask.NameToLayer(name);
		}

		public static string GetLayerStr(int name)
		{
			return LayerMask.LayerToName(name);
		}
		/// <summary>
		/// 设置物体的层级（包括子对象）
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="layer"></param>
		public static void SetLayer(this GameObject obj,string layer)
		{
            if (obj.transform.parent.name.Contains("WristBand"))
            {
                if (obj.GetReferenceCollector() != null)
                {
                    ReferenceCollector reference = obj.GetReferenceCollector();
                    var objs = reference.GetAssets(MonoReferenceType.Object);
                    for (int i = 0; i < objs.Length; i++)
                    {
                        ((GameObject)objs[i]).SetLayer(layer);
                    }
                    return;
                }

            }

            obj.layer = GetLayerInt(layer);
            int length = obj.transform.childCount;
            for (int i = 0; i < length; i++)
            {
                Transform tempobj = obj.transform.GetChild(i);
                tempobj.gameObject.layer = GetLayerInt(layer);
                if (tempobj.transform.childCount != 0)
                    tempobj.gameObject.SetLayer(layer);
            }
        }
		public static void SetRoleInfoLayer(this GameObject obj,string layer)
		{
			obj.layer = GetLayerInt(layer);
			int length = obj.transform.childCount;
			for (int i = 0; i < length; i++)
            {
				Transform tempobj = obj.transform.GetChild(i);
			
				tempobj.gameObject.layer = GetLayerInt(layer);
				if (tempobj.transform.childCount != 0)
					tempobj.gameObject.SetLayer(layer);
			}
		}
		
	}
}