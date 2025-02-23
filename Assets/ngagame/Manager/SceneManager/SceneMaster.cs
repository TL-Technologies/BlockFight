using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum SceneID
{
	Main,
	Gameplay,
	Home,
	Result,
	RewardSkin,
	RewardArena,
	ChestRoom,
	Settings,
	Shop
}

public class SceneMaster : Singleton<SceneMaster>
{
	public void OpenScene(SceneID scene)
	{
		sceneActions.Enqueue(new SceneAction(SceneActionType.Open, scene));
	}

	public void CloseScene(SceneID scene)
	{
		sceneActions.Enqueue(new SceneAction(SceneActionType.Close, scene));
	}

	public void ReloadScene(SceneID scene)
	{
		sceneActions.Enqueue(new SceneAction(SceneActionType.Reload, scene));
	}

	void OpenSceneInternal(SceneID scene)
	{
		SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
	}

	void CloseSceneInternal(SceneID scene)
	{
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == scene.ToString())
			{
				SceneManager.UnloadSceneAsync(scene.ToString());
			}
		}
	}

	void ReloadSceneInternal(SceneID scene)
	{
		bool exist = false;
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == scene.ToString())
			{
				exist = true;
			}
		}
		if (exist)
        {
			
		}
		SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
	}

	LoadingPanel loadingPanel;
	private void Awake()
	{
		var loadingPanelPrefab = Resources.Load<LoadingPanel>("LoadingPanel");
		loadingPanel = Instantiate(loadingPanelPrefab);
	}

	float showLoadingTime;
	public void ShowLoading(float time = 3f, bool useShield = false)
	{
		if(loadingPanel == null)
		{
			return;
		}
		loadingPanel.gameObject.SetActive(true);
		loadingPanel.loadingObject.SetActive(!useShield);
		loadingPanel.shieldObject.SetActive(useShield);
		loadingPanel.CanvasGroup.DOKill();
		loadingPanel.CanvasGroup.alpha = 1;
		showLoadingTime = time;
	}

	public void HideLoading()
	{
		if (loadingPanel == null)
		{
			return;
		}
		loadingPanel.gameObject.SetActive(false);
	}

	public void ShowBannerShield()
	{
		if (loadingPanel == null)
		{
			return;
		}
		loadingPanel.ShowBannerShield();
	}

	private void FixedUpdate()
	{
		if(loadingPanel == null || !loadingPanel.gameObject.activeSelf)
		{
			return;
		}

		showLoadingTime -= Time.fixedDeltaTime;
		if(showLoadingTime < 0)
		{
			loadingPanel.loadingObject.gameObject.SetActive(false);
			loadingPanel.shieldObject.gameObject.SetActive(false);
		}
	}

	Queue<SceneAction> sceneActions = new Queue<SceneAction>();
	SceneAction cachedAction;
	private void Update()
	{
		if (sceneActions.Count == 0) return;
		cachedAction = sceneActions.Dequeue();
		switch(cachedAction.type)
		{
			case SceneActionType.Open:
				OpenSceneInternal(cachedAction.sceneID);
				break;
			case SceneActionType.Close:
				CloseSceneInternal(cachedAction.sceneID);
				break;
			case SceneActionType.Reload:
				ReloadSceneInternal(cachedAction.sceneID);
				break;
		}
	}

	struct SceneAction
	{
		public SceneActionType type;
		public SceneID sceneID;

		public SceneAction(SceneActionType type, SceneID sceneID)
		{
			this.type = type;
			this.sceneID = sceneID;
		}
	}

	enum SceneActionType
	{
		Open,
		Close,
		Reload
	}

#if UNITY_EDITOR
	private void OnGUI()
	{
		return;
		if (GUI.Button(new Rect(Size(1), Size(1), Size(3), Size(1)), "Open"))
		{
			OpenSceneInternal(SceneID.Gameplay);
		}

		if (GUI.Button(new Rect(Size(1), Size(2), Size(3), Size(1)), "Reload"))
		{
			ReloadSceneInternal(SceneID.Gameplay);
		}

		if (GUI.Button(new Rect(Size(1), Size(3), Size(3), Size(1)), "Close"))
		{
			CloseSceneInternal(SceneID.Gameplay);
		}

	}

	float Size(int size)
	{
		return Screen.width * 0.1f * size;
	}
#endif
}
