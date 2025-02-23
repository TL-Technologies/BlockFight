using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
	[SerializeField] Image icon;
	[SerializeField] GameObject selected;
	[SerializeField] GameObject locked;
	bool IsLocked => locked.activeSelf;

	string id;
	public string ID
	{
		get => id;
		set
		{
			id = value;
			Refresh();
		}
	}

	public Action<string> OnClick;

	public void Init(string _id)
	{
		id = _id;
		icon.sprite = OutfitItem.Instance.GetPreview(id);
		Refresh();
		if(IsLocked)
		{
			GetComponent<Button>().interactable = false;
		}
		else
		{
			GetComponent<Button>().interactable = true;
			GetComponent<Button>().onClick.AddListener(OnClicked);
		}
	}

	void OnClicked()
	{
		if(IsLocked)
		{
			return;
		}
		OutfitItem.Instance.Current = id;
		OnClick?.Invoke(id);
	}

	public void Refresh()
	{
		selected.gameObject.SetActive(OutfitItem.Instance.Current == id);
		locked.gameObject.SetActive(!OutfitItem.Instance.Owned(id));
	}
}
