using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class Profile : Singleton<Profile>
{
    string SAVE_NAME = "user.dat";
    private UserData data = null;

    void Awake()
    {
        Init();
    }

    private bool Initialized = false;

    private void Init()
    {
        LoadData();
        Initialized = true;
    }

	public bool FirstOpen => data.firstOpen;
	public bool RateShowed
	{
		get
		{
			return data.rateShowed;
		}

		set
		{
			data.rateShowed = value;
			RequestSave();
		}
	}

	public string PlayerName
	{
		get
		{
			return data.playerName;
		}

		set
		{
			data.playerName = value;
			RequestSave();
		}
	}

	public bool VIP
    {
        get
        {
            return data.vip;
        }

        set
        {
            data.vip = value;
			RequestSave();
        }
    }
	
    public int Coins
    {
        get
        {
            return data.coins;
        }

        set
        {
            data.coins = value;
			RequestSave();
			EventManager.Instance.PostNotification(GameEvent.OnCoinChange, this);
        }
    }

	public int Gem
	{
		get
		{
			return data.gem;
		}

		set
		{
			data.gem = value;
			RequestSave();
			EventManager.Instance.PostNotification(GameEvent.OnGemChange, this);
		}
	}

	public int Level
	{
		get
		{
			return data.level;
		}

		set
		{
			data.level = value;
			RequestSave();
		}
	}

	public int KeyAmount
	{
		get
		{
			return data.keyAmount;
		}

		set
		{
			data.keyAmount = value;
			RequestSave();
		}
	}

	void LoadData()
    {
        try
        {
            if (PlayerPrefs.HasKey(SAVE_NAME))
            {
				string dataAsJson = PlayerPrefs.GetString(SAVE_NAME);
                // Pass the json to JsonUtility, and tell it to create a GameData object from it
                data = JsonUtility.FromJson<UserData>(dataAsJson);
            }
            else
            {
				data = new UserData();
				Debug.LogWarning("Init game data!");
            }

        }
        catch { }

        if (data == null)
        {
            data = new UserData();
        }
    }

    public void SaveData()
    {
        string dataAsJson = JsonUtility.ToJson(data);
		try
		{
			PlayerPrefs.SetString(SAVE_NAME, dataAsJson);
		} catch
		{

		}
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }

    private void OnApplicationQuit()
    {
		data.firstOpen = false;
        SaveData();
    }

	private bool needSave = false;

	private void RequestSave()
	{
		needSave = true;
	}

	private void Update()
	{
		if(needSave)
		{
			SaveData();
			needSave = false;
		}
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/Clear UserData")]
#endif
    private static void ClearUserData()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, "user.dat"));
		PlayerPrefs.DeleteAll();
    }
}
