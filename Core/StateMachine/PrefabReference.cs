using UnityEngine;

namespace Utils.Core.Flow
{
	/// <summary>
	/// ScriptableObject for keeping a reference to a prefab, this way only the reference has to be changed instead of all seperate references to a prefab
	/// </summary>
	public class PrefabReference : ScriptableObject
	{
		[SerializeField] private GameObject prefab = null;
		public GameObject Prefab => prefab;
	}
}
