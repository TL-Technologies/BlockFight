using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaItem : GameItem<ArenaItem>
{
	enum ArenaSaveKey
	{
		LastArenaKey,
		NewestArenaKey
	}

	public Level GetLevelPrefab(string levelName)
	{
  		if (!Initialized)
		{
			Init();
		}
		if (define == null) return null;
		return Resources.Load<Level>($"{define.PrefabFolder}/{levelName}");
	}

	public string LastArena
	{
		get
		{
			return PlayerPrefs.GetString(ArenaSaveKey.LastArenaKey.ToString(), string.Empty);
		}
	}

	public void OnNewItem(string itemName)
	{
		PlayerPrefs.SetString(ArenaSaveKey.NewestArenaKey.ToString(), itemName);
	}

	public string NewestItem
	{
		get => PlayerPrefs.GetString(ArenaSaveKey.NewestArenaKey.ToString(), string.Empty);
		set => PlayerPrefs.SetString(ArenaSaveKey.NewestArenaKey.ToString(), value);
	}

	public string RandomOwnedItem(string notInclude = "")
	{
		if (!Initialized)
		{
			Init();
		}

		if(NewestItem != string.Empty)
		{
			return NewestItem;
		}

		if (data == null || define == null || define.list.Length <= 0)
		{
			return DefaultItem;
		}

		var list = data.owned;
		List<string> avaibles = new List<string>();
		foreach (var skin in list)
		{
			if (skin.ToString() != notInclude)
			{
				avaibles.Add(skin);
			}
		}
		if (avaibles.Count <= 0)
		{
			return DefaultItem;
		}

		return avaibles[Random.Range(0, avaibles.Count)];
	}

	public void Play(string arenaName)
	{
		Debug.Log("Playing arena: " + arenaName);
		PlayerPrefs.SetString(ArenaSaveKey.LastArenaKey.ToString(), arenaName);
		NewestItem = string.Empty;
	}
}
