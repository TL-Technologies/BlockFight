using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gameplay : MonoBehaviour
{
	public static Gameplay Instance;
	public static bool Win { get; private set; } = false;
    public State CurrentState { get; private set; } = State.None;
	public bool IsPlaying => CurrentState == State.Play;
	CharacterBase[] characters;
	public CharacterBase Player { get; private set; }
	Level level;
	string levelName;
	[SerializeField] Cinemachine.CinemachineTargetGroup targetGroup;
	[SerializeField] Cinemachine.CinemachineConfiner confiner;
	[SerializeField] GameObject winFX;

	private void Awake()
	{
		Instance = this;
		ChangeState(State.WarmUp);
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void Play()
	{
		ChangeState(State.Play);
	}

	public void EndGame()
	{
		ChangeState(State.End);
	}

	private void Init()
	{
		Win = false;
		SoundManager.Instance.VolumeMultiplier = 0.5f;
		level = GetComponentInChildren<Level>();
		// If in tutorial, we don't have any arena
		if (string.IsNullOrEmpty(ArenaItem.Instance.LastArena)) ArenaItem.Instance.ClearOwned();
		if (level == null)
		{
			levelName = !string.IsNullOrEmpty(ArenaItem.Instance.LastArena) ? ArenaItem.Instance.RandomOwnedItem(ArenaItem.Instance.LastArena) : "level-tutorial";
			//levelName = $"level{Profile.Instance.LevelNumber.ToString("000")}";
			var levelPrefab = ArenaItem.Instance.GetLevelPrefab(levelName);
			level = Instantiate(levelPrefab, transform);
		}
		var spawners = GetComponentsInChildren<PlayerSpawner>();
		foreach(var spawner in spawners)
		{
			GameObject prefab = null;
			prefab = OutfitItem.Instance.GetPrefab(spawner.Skin);

			var ins = prefab != null ? Instantiate(prefab, spawner.transform.position, spawner.transform.rotation, level.transform) : null;
			if(ins != null)
			{
				var characterBase = ins.GetComponent<CharacterBase>();
				if (characterBase != null)
				{
					characterBase.isPlayer = spawner.Type == PlayerSpawner.PlayerType.Player;
				}
			}
		}

		characters = GetComponentsInChildren<CharacterBase>();
		foreach(var character in characters)
		{
			if (character.isPlayer)
			{
				Player = character;
			}
			character.Init();
			targetGroup.AddMember(character.Character.transform, character.isPlayer ? 1 : 0.8f, 1);
			confiner.m_BoundingShape2D.transform.localScale = level.Size;
		}
	}

	void OnStartGame()
	{
		foreach(var character in characters)
		{
			character.OnStartGame();
		}
		ArenaItem.Instance.Play(levelName);
		EventManager.Instance.PostNotification(GameEvent.OnLevelStart, this);
		if(OutfitItem.Instance.SuggestItem == OutfitItem.Instance.Current)
		{
			OutfitItem.Instance.SuggestItem = string.Empty;
		}
	}

	void OnEndGame()
	{
		Win = Player != null && !Player.Death;
		winFX.gameObject.SetActive(Win);
		foreach (var character in characters)
		{
			character.OnEndGame();
		}

		DOVirtual.DelayedCall(3f, delegate
		{
			Time.timeScale = 1;
			Ads.Instance.ShowInterstitial((value) =>
			{
				SceneMaster.Instance.OpenScene(SceneID.Result);
			}, "level_end");
		});
	}

	void ChangeState(State _state)
	{
		if (CurrentState == _state) return;

		switch(_state)
		{
			case State.WarmUp:
				Init();
				break;
			case State.Play:
				if (CurrentState == State.End) return;
				OnStartGame();
				break;
			case State.End:
				OnEndGame();
				break;
		}
		CurrentState = _state;
	}

	public void OpenHome()
	{
		SceneMaster.Instance.OpenScene(SceneID.Home);
		SceneMaster.Instance.CloseScene(SceneID.Gameplay);
	}

	// Update is called once per frame
	void Update()
    {
		switch (CurrentState)
		{
			case State.WarmUp:
				break;
			case State.Play:
				break;
			case State.End:
				break;

		}
	}

    public enum State
	{
        None,
        WarmUp,
        Play,
        End
	}
}
