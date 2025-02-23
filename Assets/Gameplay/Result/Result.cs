using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
	[SerializeField] GameObject winGroup;
	[SerializeField] GameObject loseGroup;

	private void Awake()
	{
		winGroup.gameObject.SetActive(Gameplay.Win);
		loseGroup.gameObject.SetActive(!Gameplay.Win);
		if (Gameplay.Win)
		{
			Profile.Instance.Level++;
			Profile.Instance.Coins += 100;
		} else
		{

		}
	}

	public void OnNewOutfit()
	{
		SceneMaster.Instance.OpenScene(SceneID.RewardSkin);
	}

	public void OnNewArena()
	{
		SceneMaster.Instance.OpenScene(SceneID.RewardArena);
	}

	public void Close()
	{
		if(string.IsNullOrEmpty(OutfitItem.Instance.SuggestItem))
		{
			SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
		} else
		{
			SceneMaster.Instance.OpenScene(SceneID.Home);
		}
		
		SceneMaster.Instance.CloseScene(SceneID.Result);
	}
}
