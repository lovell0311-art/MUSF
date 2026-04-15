using UnityEngine;

namespace ETHotfix
{
	public interface IUIFactory
	{
		//UI Create(Scene scene, string type, GameObject parent);
		/// <summary>
		/// 创建UI面板
		/// </summary>
		/// <returns></returns>
		UI Create();
		//void Remove(string type);
	}
}