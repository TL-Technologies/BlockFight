using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardSkin : Scene
{
	[SerializeField] GameObject skinWraper;

	string skinInProgress;
	private void OnEnable()
	{
		skinInProgress = OutfitItem.Instance.GetItemInProgress();
		var skinPrefab = OutfitItem.Instance.GetPrefab(skinInProgress);
		var skinModel = Instantiate(skinPrefab, skinWraper.transform);
		OutfitItem.Instance.CreateNewProgress();
		if (skinModel == null)
		{
			return;
		}
		var healthBar = skinModel.GetComponentInChildren<HealthBar>();
		if(healthBar != null)
		{
			healthBar.gameObject.SetActive(false);
		}
		var character = skinModel.GetComponentInChildren<CharacterBase>();
		if(character != null)
		{
			character.enabled = false;
		}
		
	}

	public void OnClaimClick()
	{
		Ads.Instance.ShowRewardedAd((value) =>
		{
			if (value)
			{
				OutfitItem.Instance.Unlock(skinInProgress.ToString(), true);
				OutfitItem.Instance.SuggestItem = skinInProgress.ToString();
				Close();
			}
		}, "progress_skin");
	}

	public void OnSkipClick()
	{
		Close();
	}

	void Close()
	{
		SceneMaster.Instance.CloseScene(SceneID.RewardSkin);
	}
}
