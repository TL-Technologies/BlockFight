using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource sfxAS;

    [SerializeField]
    private string resourceFolder = "Sounds";
    [SerializeField]
    private List<string> list;
    public float VolumeMultiplier { get; set; } = 1f;

    private Dictionary<string, AudioClip> dict = null;
    private Dictionary<string, AudioClip> Dict
    {
        get
        {
            if (dict == null)
            {
                Serialize();
            }
            return dict;
        }
    }

    float volume = 1f;
    public void PlaySFX(string key, float _volume = 1)
    {
        volume = _volume * VolumeMultiplier;
        if (!UserSettings.Instance.SoundOn)
        {
            return;
        }

        if (Dict.ContainsKey(key))
        {
            sfxAS.PlayOneShot(Dict[key], volume);
        }
        else
        {
            AudioClip audioClip = Resources.Load<AudioClip>(resourceFolder + "/" + key);
            if (audioClip == null)
            {
                Debug.LogError(resourceFolder + "/" + key + " not exist");
                return;
            }
            Dict.Add(key, audioClip);
            sfxAS.PlayOneShot(Dict[key], volume);
        }
    }

    public void Serialize()
    {
        if (dict == null)
        {
            dict = new Dictionary<string, AudioClip>();
        }
    }

    private void Awake()
    {
        if(sfxAS == null)
        {
            sfxAS = GetComponent<AudioSource>();
            if(sfxAS == null)
            {
                gameObject.AddComponent<AudioSource>();
                sfxAS = GetComponent<AudioSource>();
            }
        }
    }
}

public class SFXConst
{
    public static string PUNCH => $"punch-{Random.Range(1, 3)}";
    public static string PUNCH_HEAVY => $"punch-3";
    public static string HURT => $"hurt-{Random.Range(1, 6)}";
    public static string BULLET_HIT => $"impact-6";
    public static string THROW_HIT => $"impact-7";
    public static string SHOOT_PISTOL => $"pistol-shot-1";
    public static string SHOOT_HEAVY => $"pistol-shot-2";
    public static string LIGHT_COLLISION => $"punch-{Random.Range(4, 6)}";
    public static string HEAVY_COLLISION => $"punch-{Random.Range(8, 10)}";
    public static string COLLECT_COIN => $"collect-04";
}
