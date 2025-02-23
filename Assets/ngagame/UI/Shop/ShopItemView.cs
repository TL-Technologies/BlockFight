using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
	[SerializeField] GameObject ownedObject;
	[SerializeField] GameObject lockedObject;
	[SerializeField] GameObject selectedObject;
	[SerializeField] GameObject vipObject;
	[SerializeField] GameObject rvLockObject;
	[SerializeField] GameObject newObject;
	[SerializeField] Image icon;

	public string Id
	{
		get;
		private set;
	}

	public Action<string> OnSelect;

	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnInteractiveSkin, RefreshNewNotice);
	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if (eventManager == null)
		{
			return;
		}

		EventManager.Instance.RemoveEvent(GameEvent.OnInteractiveSkin);
	}

	void RefreshNewNotice(GameEvent Event_Type, Component Sender, object Param = null)
	{
		newObject.gameObject.SetActive(OutfitItem.Instance.IsNew(Id));
	}

	public void Init(string _id)
	{
		Id = _id;
		icon.sprite = OutfitItem.Instance.GetPreview(Id);
		Refresh();
		RefreshNewNotice(GameEvent.OnInteractiveSkin, null, null);
	}

	bool IsOwned()
	{
		return OutfitItem.Instance.Owned(Id);
	}

	public bool IsSelected()
	{
		return OutfitItem.Instance.Current == Id;
	}

	public void Unlock()
	{
		OutfitItem.Instance.Unlock(Id, true);
		var skin = string.Empty;
		OutfitItem.Instance.Current = Id;
		OnSelect?.Invoke(Id);
	}

	public void OnClick()
	{
		if(IsOwned())
		{
			OutfitItem.Instance.Current = Id;
			OutfitItem.Instance.Current = Id;
			OnSelect?.Invoke(Id);
		} else
		{
			
		}
	}

	public void Refresh()
	{
		var showPreview = IsOwned();
		ownedObject.gameObject.SetActive(showPreview);
		lockedObject.gameObject.SetActive(!showPreview);
		selectedObject.gameObject.SetActive(OutfitItem.Instance.Current == Id);
		newObject.gameObject.SetActive(OutfitItem.Instance.IsNew(Id));
	}
}
