using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionButton : MonoBehaviour
{
	[SerializeField] Animation noticeAnimation;

	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnMissionNotice, UpdateNotice);
		UpdateNotice(GameEvent.OnMissionNotice, null, null);
	}

	void UpdateNotice(GameEvent Event_Type, Component Sender, object Param = null)
	{
		noticeAnimation.gameObject.SetActive(Achievement.Instance != null && Achievement.Instance.HasNew());
	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if (eventManager == null)
		{
			return;
		}
		eventManager.RemoveEvent(GameEvent.OnMissionNotice, UpdateNotice);
	}

	public void Open()
	{
		Achievement.Instance.OpenMenu();
	}
}
