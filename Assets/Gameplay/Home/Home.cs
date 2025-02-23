using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
	[SerializeField] Transform previewWrapper;
	CharacterCard[] characterCards;
	Dictionary<string, GameObject> previews = new Dictionary<string, GameObject>(1);

	private void Awake()
	{
		if (!GameManager.Initialized)
		{
			SceneMaster.Instance.OpenScene(SceneID.Main);
		}
		var characterList = OutfitItem.Instance.Define.list;
		characterCards = GetComponentsInChildren<CharacterCard>();
		var count = Mathf.Min(characterCards.Length, characterList.Length);
		for (int i = 0; i < count; i++)
		{
			characterCards[i].Init(characterList[i].ToString());
			characterCards[i].OnClick = OnItemClicked;
			var model = Instantiate(OutfitItem.Instance.GetPrefab(characterList[i]), previewWrapper);
			if(model != null)
			{
				var healthBar = model.GetComponentInChildren<HealthBar>();
				if (healthBar != null) healthBar.gameObject.SetActive(false);
				var pointer = model.GetComponentInChildren<Pointer>();
				if (pointer != null) pointer.gameObject.SetActive(false);
				if (!previews.ContainsKey(characterList[i])) previews.Add(characterList[i], model);
				model.gameObject.SetActive(OutfitItem.Instance.Current == characterList[i].ToString());
			}
		}
	}

	void OnItemClicked(string id)
	{
		Refresh();
	}

	public void Refresh()
	{
		var characterList = OutfitItem.Instance.Define.list;
		characterCards = GetComponentsInChildren<CharacterCard>();
		for (int i = 0; i < Mathf.Min(characterCards.Length, characterList.Length); i++)
		{
			characterCards[i].Refresh();
			if (previews.ContainsKey(characterList[i]))
			{
				previews[characterList[i]].gameObject.SetActive(OutfitItem.Instance.Current == characterList[i].ToString());
			}
		}
	}

	public void OnStartClicked()
	{
		SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
		SceneMaster.Instance.CloseScene(SceneID.Home);
	}
}
