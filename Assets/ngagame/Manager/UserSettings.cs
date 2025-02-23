using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSettings : Singleton<UserSettings>
{
	public bool MusicOn
	{
		get
		{
			return PlayerPrefs.GetInt("MusicOn", 1) > 0;
		}
		set
		{
			PlayerPrefs.SetInt("MusicOn", value ? 1 : 0);
		}
	}

	public bool SoundOn
    {
        get
        {
            return PlayerPrefs.GetInt("SoundOn", 1) > 0;
        } 
        set
        {
			PlayerPrefs.SetInt("SoundOn", value ? 1 : 0);
        }
    }

	public bool VibrateOn
	{
		get
		{
			return PlayerPrefs.GetInt("VibrateOn", 1) > 0;
		}
		set
		{
			PlayerPrefs.SetInt("VibrateOn", value ? 1 : 0);
		}
	}

	public float LevelListPosition
	{
		get
		{
			return PlayerPrefs.GetFloat("LevelListPosition", 1.0f);
		}
		set
		{
			PlayerPrefs.SetFloat("LevelListPosition", value);
		}
	}
}
