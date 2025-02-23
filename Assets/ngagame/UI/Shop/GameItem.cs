using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameItem<T> : MonoBehaviour where T : MonoBehaviour
{
	protected GameItemSave data;
	protected ItemDefine define;
	public ItemDefine Define { get
		{
			if (!Initialized) Init();
			return define;
		} 
	}
	protected bool Initialized = false;

	public string DefaultItem
	{
		get
		{
			if (!Initialized) Init();
			return define != null && define.list.Length > 0 ? define.list[0] : string.Empty;
		}
	}
	protected string SaveKey => DefineAssetName + "_SAVE_DATA";
	protected string DefineAssetName => typeof(T).ToString();

	protected void Init()
	{
		define = Resources.Load<ItemDefine>(DefineAssetName);
		if(define == null)
		{
			Debug.LogError($"Please create ItemDefine asset and name to {DefineAssetName}");
			return;
		} else
		{
			if (string.IsNullOrEmpty(SaveKey))
			{
				Debug.LogError($"Please open {DefineAssetName} and set field of SaveKey");
			}
		}
		try
		{
			if (PlayerPrefs.HasKey(SaveKey))
			{
				string dataAsJson = PlayerPrefs.GetString(SaveKey);
				// Pass the json to JsonUtility, and tell it to create a GameData object from it
				data = JsonUtility.FromJson<GameItemSave>(dataAsJson);
			}
			else
			{
				data = new GameItemSave();

				if (data.owned != null && data.owned.Count <= 0 && define.list != null && define.list.Length > 0)
				{
					data.owned.Add(define.list[0]);
				}
			}

		}
		catch { }

		if (data == null)
		{
			data = new GameItemSave();
		}

		Initialized = true;
	}

	public string Current
	{
		get
		{
			if (!Initialized)
			{
				Init();
			}
			return data != null && !string.IsNullOrEmpty(data.current) ? data.current : DefaultItem;
		}

		set
		{
			if (!Initialized)
			{
				Init();
			}
			if (data != null)
			{
				data.current = value;
				if (data.newUnlocks.Contains(value))
				{
					data.newUnlocks.Remove(value);
				}
				EventManager.Instance.PostNotification(GameEvent.OnInteractiveSkin, null); // On equip skin
				SaveData();
			}
		}
	}

	public float UnlockIndex
	{
		get
		{
			if (!Initialized)
			{
				Init();
			}
			return data.unlockIndex;
		}
	}

	public float UnlockProgress
	{
		get
		{
			if (!Initialized)
			{
				Init();
			}
			return data.unlockProgress;
		}

		set
		{
			if (!Initialized)
			{
				Init();
			}
			data.unlockProgress = value;
			SaveData();
		}
	}

	public void Unlock(string id, bool equip)
	{
		if (!Initialized)
		{
			Init();
		}
		if (data != null && data.owned != null)
		{
			if (data.owned.Contains(id))
			{
				if (equip) data.current = id;
				return;
			}
			data.owned.Add(id);

			if (equip)
			{
				data.current = id;
				if (data.newUnlocks.Contains(id))
				{
					data.newUnlocks.Remove(id);
				}
			}
			else
			{
				data.newUnlocks.Add(id);
			}

			EventManager.Instance.PostNotification(GameEvent.OnInteractiveSkin, null); // On unlock new skin 

			SaveData();
		}
	}

	public bool Owned(string id)
	{
		if (!Initialized)
		{
			Init();
		}
		return data != null && data.owned != null ? data.owned.Contains(id) : false;
	}

	public bool HasNew()
	{
		if (!Initialized)
		{
			Init();
		}
		return data.newUnlocks.Count > 0;
	}

	public bool IsNew(string skinName)
	{
		if (!Initialized)
		{
			Init();
		}
		return data.newUnlocks.Contains(skinName);
	}

	public bool IsFull
	{
		get
		{
			if (!Initialized)
			{
				Init();
			}
			if (define == null || define.list.Length <= 0) return true;
			return data.owned.Count >= define.list.Length;
		}
	}

	public string GetItemInProgress()
	{
		if (!Initialized)
		{
			Init();
		}
		if (data == null || define == null || define.list.Length <= 0)
		{
			return string.Empty;
		}
		var index = data.unlockIndex;
		var list = define.list;
		var guess = list[index % list.Length];
		if (Owned(guess))
		{
			foreach (var item in list)
			{
				if (!Owned(item))
				{
					return item;
				}
			}
			return string.Empty;
		}
		else
		{
			return guess;
		}
	}

	public void CreateNewProgress()
	{
		if (!Initialized)
		{
			Init();
		}
		if (data == null)
		{
			return;
		}

		data.unlockIndex++;
		data.unlockProgress = 0f;
		SaveData();
	}

	public string RandomItem(string notInclude = "")
	{
		if (!Initialized)
		{
			Init();
		}
		if (data == null || define == null || define.list.Length <= 0)
		{
			return string.Empty;
		}

		var list = define.list;
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
			return string.Empty;
		}

		return avaibles[Random.Range(0, avaibles.Count)];
	}

	public GameObject GetPrefab(string itemName)
	{
		if (!Initialized)
		{
			Init();
		}
		if (define == null) return null;
		return Resources.Load<GameObject>($"{define.PrefabFolder}/{itemName}");
	}

	public Sprite GetPreview(string itemName)
	{
		if (!Initialized)
		{
			Init();
		}
		if (define == null) return null;
		return Resources.Load<Sprite>($"{define.PrefabFolder}/Preview/{itemName}");
	}

	public void ClearOwned()
	{
		if (!Initialized)
		{
			Init();
		}
		data.owned = new List<string>(0);
	}

	protected void SaveData()
	{
		string dataAsJson = JsonUtility.ToJson(data);
		try
		{
			PlayerPrefs.SetString(SaveKey, dataAsJson);
		}
		catch
		{

		}
	}

	// Check to see if we're about to be destroyed.
	private static bool m_ShuttingDown = false;
	private static object m_Lock = new object();
	private static T m_Instance;
	public static T Instance
	{
		get
		{
			if (m_ShuttingDown)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
					"' already destroyed. Returning null.");
				return null;
			}

			lock (m_Lock)
			{
				if (m_Instance == null)
				{
					// Search for existing instance.
					m_Instance = (T)FindObjectOfType(typeof(T));

					// Create new instance if one doesn't already exist.
					if (m_Instance == null)
					{
						// Need to create a new GameObject to attach the singleton to.
						var singletonObject = new GameObject();
						m_Instance = singletonObject.AddComponent<T>();
						singletonObject.name = typeof(T).ToString() + " (Singleton)";

						// Make instance persistent.
						DontDestroyOnLoad(singletonObject);
					}
				}

				return m_Instance;
			}
		}
	}


	private void OnApplicationQuit()
	{
		m_ShuttingDown = true;
	}


	private void OnDestroy()
	{
		m_ShuttingDown = true;
	}
}

[System.Serializable]
public class GameItemSave
{
	public string current;
	public List<string> owned = new List<string>();
	public List<string> newUnlocks = new List<string>();
	public int unlockIndex = 0;
	public float unlockProgress = 0;
}
