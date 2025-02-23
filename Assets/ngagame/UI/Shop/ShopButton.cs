using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
	[SerializeField] Animation noticeAnimation;

	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnInteractiveSkin, UpdateNotice);
		EventManager.Instance.AddListener(GameEvent.OnLevelStart, OnLevelStart);
		UpdateNotice(GameEvent.OnCoinChange, null);
	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if (eventManager == null)
		{
			return;
		}
		eventManager.RemoveEvent(GameEvent.OnInteractiveSkin, UpdateNotice);
		eventManager.RemoveEvent(GameEvent.OnLevelStart, OnLevelStart);
	}

	void UpdateNotice(GameEvent Event_Type, Component Sender, object Param = null)
	{
		noticeAnimation.gameObject.SetActive(OutfitItem.Instance.HasNew());
	}

	void OnLevelStart(GameEvent Event_Type, Component Sender, object Param = null)
	{
		gameObject.SetActive(false);
	}

	public void OpenShop()
	{
		SceneMaster.Instance.ShowLoading(1.5f, true);
		SceneMaster.Instance.OpenScene(SceneID.Shop);
	}
}
