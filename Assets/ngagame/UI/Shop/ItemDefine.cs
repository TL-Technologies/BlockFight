using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "Create/ItemList")]
public class ItemDefine : ScriptableObject
{
	[SerializeField] string prefabFolder;
	public string PrefabFolder => prefabFolder;
	public string[] list = new string[0];
}
