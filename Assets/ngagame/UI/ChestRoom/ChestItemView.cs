using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestItemView : MonoBehaviour
{
	public struct ChestItemData
	{
		public int gem;
		public object itemId;
		public GameObject itemPreview;

		public ChestItemData(int gem, object itemId, GameObject itemPreview)
		{
			this.gem = gem;
			this.itemId = itemId;
			this.itemPreview = itemPreview;
		}
	}

	ChestItemData data;
	
	Transform special => transform.GetChild(0);
	Transform chest => transform.GetChild(1);
	Transform gemGroup => transform.GetChild(2);
	Transform itemGroup => transform.GetChild(3);
	Transform model => itemGroup.GetChild(0);
	Transform gem => gemGroup.GetChild(0);

	public void SetData(ChestItemData _data, bool isSpecial)
	{
		data = _data;

		special.gameObject.SetActive(isSpecial);
		chest.gameObject.SetActive(true);
		gemGroup.gameObject.SetActive(false);
		itemGroup.gameObject.SetActive(false);

		if (data.itemPreview == null)
		{
			gem.GetComponent<Text>().text = data.gem.ToString();
		}
	}

	public void Open()
	{
		chest.gameObject.SetActive(false);
		bool hasItem = data.itemPreview != null;
		gemGroup.gameObject.SetActive(!hasItem);
		itemGroup.gameObject.SetActive(hasItem);
	}
}
