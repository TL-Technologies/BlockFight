using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ngagame;
using UnityEngine.UI;

public class Achievement : MonoBehaviour
{
	public static Achievement Instance;
    public AchievementData achievementList;
	public AchievementSave data;
	const string SAVE_NAME = "ACHIEVEMENT_SAVE_NAME";
	bool Initialized, showing = false;
	Queue<Mission> noticeQueue = new Queue<Mission>();
	MissionItemView[] missions;

	[SerializeField] GameObject menu;

	private void Awake()
	{
		Instance = this;
		Init();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnTriggerAchievement, OnTriggerAchievement);
		EventManager.Instance.PostNotification(GameEvent.OnMissionNotice, this, string.Empty);
	}

	void Refresh()
	{
		if (!Initialized)
		{
			return;
		}
		missions = GetComponentsInChildren<MissionItemView>();
		for (int i = 0; i < missions.Length; i++)
		{
			if (i < data.dailyMissions.Count)
			{
				missions[i].gameObject.SetActive(true);
				missions[i].Init(data.dailyMissions[i]);
			}
			else
			{
				missions[i].gameObject.SetActive(false);
			}
		}
	}

	public void OpenMenu()
	{
		menu.gameObject.SetActive(true);
		Refresh();
	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if (eventManager == null)
		{
			return;
		}
		eventManager.RemoveEvent(GameEvent.OnTriggerAchievement, OnTriggerAchievement);
	}

	private void OnTriggerAchievement(GameEvent Event_Type, Component Sender, object Param)
	{
		var earned = Param as AchievementEarned;
		if(earned == null)
		{
			return;
		}
		foreach(var m in data.dailyMissions)
		{
			if(!m.completed && m.achievement.id == earned.id)
			{
				m.current += earned.amount;
				if (m.current >= m.require)
				{
					m.current = m.require;
					OnMissionComplete(m.achievement.id);
					EventManager.Instance.PostNotification(GameEvent.OnMissionNotice, this, m.achievement.id);
				}
			}
		}

		Save();
	}

	private void OnMissionComplete(string _id)
	{
		foreach (var m in data.dailyMissions)
		{
			if (m.achievement.id == _id && !m.completed)
			{
				m.completed = true;
				RequestNotice(m);
			}
		}
		Save();
	}

	void RequestNotice(Mission mission)
	{
		noticeQueue.Enqueue(mission);
	}

	[SerializeField] Animator animator;
	[SerializeField] Text missionName;
	[SerializeField] Text missionDesc;
	[SerializeField] Image missionIcon;
	void ShowNotice(Mission mission)
	{
		showing = true;
		missionName.text = mission.achievement.name;
		missionDesc.text = string.Format(mission.achievement.description, mission.require);
		missionIcon.sprite = Resources.Load<Sprite>(mission.achievement.id);
		animator.SetTrigger("show");
	}

	// From animation event
	public void OnShowComplete()
	{
		showing = false;
	}

	private void Update()
	{
		if(showing || noticeQueue.Count <= 0)
		{
			return;
		}

		var notice = noticeQueue.Dequeue();
		ShowNotice(notice);

	}

	public void Init()
	{
		try
		{
			if (PlayerPrefs.HasKey(SAVE_NAME))
			{
				string dataAsJson = PlayerPrefs.GetString(SAVE_NAME);
				// Pass the json to JsonUtility, and tell it to create a GameData object from it
				data = JsonUtility.FromJson<AchievementSave>(dataAsJson);
			}
			else
			{
				data = new AchievementSave();
				Debug.LogWarning("Init Achievement data!");
			}

		}
		catch { }

		if (data == null)
		{
			data = new AchievementSave();
		}
		InitDaily();
		Initialized = true;
	}

	public bool HasNew()
	{
		if(!Initialized)
		{
			Init();
		}
		foreach(var m in data.dailyMissions)
		{
			if(m.completed && !m.claimed)
			{
				return true;
			}
		}
		return false;
	}

	public void Claim(string id)
	{
		foreach (var m in data.dailyMissions)
		{
			if (m.achievement.id == id)
			{
				m.claimed = true;
				break;
			}
		}
		Save();
	}

	void InitDaily()
	{
		var now = System.DateTime.Now.Day;
		if(data.lastDay != now && achievementList.achievements.Count > 0)
		{
			var c = Mathf.Min(3, achievementList.achievements.Count - 1);
			var randomList = achievementList.achievements.OrderBy(arg => System.Guid.NewGuid()).Take(c).ToList();
			data.dailyMissions.Clear();
			foreach (var a in randomList)
			{
				data.dailyMissions.Add(new Mission(a));
			}
			data.lastDay = System.DateTime.Now.Day;
		}

		Save();
	}

	void Save()
	{
		string dataAsJson = JsonUtility.ToJson(data);
		try
		{
			PlayerPrefs.SetString(SAVE_NAME, dataAsJson);
		}
		catch
		{

		}
	}
}

[System.Serializable]
public class AchievementSave
{
	public int lastDay;
    public List<Mission> dailyMissions = new List<Mission>();
}

[System.Serializable]
public class Mission
{
    public AchievementEntity achievement;
    public int require = 1;
    public int current = 0;
	public bool completed = false;
	public bool claimed = false;

	public Mission(AchievementEntity achievement)
	{
		this.achievement = achievement;
		this.require = Random.Range(achievement.minRequire, achievement.maxRequire) * achievement.multipleRequire;
		this.current = 0;
	}
}

[System.Serializable]
public class AchievementEntity
{
    public string id;
    public string name;
    public string description;
    public int minRequire = 1;
    public int maxRequire = 1;
    public int multipleRequire = 1;
} 

public class AchievementEarned
{
	public string id;
	public int amount;

	public AchievementEarned(string id, int amount = 1)
	{
		this.id = id;
		this.amount = amount;
	}
}