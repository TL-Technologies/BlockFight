using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutfitItem : GameItem<OutfitItem>
{
	const string SUGGEST_ITEM_KEY = "SUGGEST_ITEM_KEY";

	public string SuggestItem
	{
		get
		{
			return PlayerPrefs.GetString(SUGGEST_ITEM_KEY, string.Empty);
		}

		set
		{
			PlayerPrefs.SetString(SUGGEST_ITEM_KEY, value);
		}
	}
}
