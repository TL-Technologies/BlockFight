using ECM2.Examples.Animation.UnityCharacterAnimatorExample;
using ECM2.Helpers;
using ngagame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChestRoom : MonoBehaviour
{
	[SerializeField] Transform chestWraper;
	[SerializeField] GameObject previewWraper;
	[SerializeField] GameObject gemsModel;
	[SerializeField] Image key;
	[SerializeField] Button getMoreKeyButton;
	[SerializeField] Button closeButton;
	[SerializeField] Text gemText;
	[SerializeField] GameObject guideText;
	[SerializeField] GameObject surprise;

	GameObject itemPreview;
	string rewardSkin;
	ChestItemView.ChestItemData bestPrize;
	ChestItemView[] chests;
	int openCount = 0;
	int bestPrizeIndex = 2;

	private void Awake()
	{
		chests = chestWraper.GetComponentsInChildren<ChestItemView>();
		for (int i = 0; i < chests.Length; i++)
		{
			var c = chests[i];
			//chests[i].GetComponent<Button>().onClick.AddListener(delegate
			//{
			//	OpenChest(c);
			//});
		}
	}

	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnCoinChange, OnCoinChange);
		OnCoinChange(GameEvent.OnCoinChange, this, null);
		Key.Holding = false;
		Profile.Instance.KeyAmount = 1;
		openCount = 0;
		bestPrizeIndex = Random.Range(1, 3);
		rewardSkin = OutfitItem.Instance.RandomItem();
		var skin = OutfitItem.Instance.GetPrefab(rewardSkin);
		var animator = skin != null ? skin.GetComponentInChildren<Animator>() : null;
		
		if (rewardSkin != string.Empty && animator != null)
		{
			itemPreview = Instantiate(animator.gameObject, previewWraper.transform);
			var unityCharacterAnimator = itemPreview != null ? itemPreview.GetComponentInChildren<UnityCharacterAnimator>() : null;
			if (unityCharacterAnimator != null)
			{
				Destroy(unityCharacterAnimator);
			}
			var RootMotionController = itemPreview != null ? itemPreview.GetComponentInChildren<RootMotionController>() : null;
			if (RootMotionController != null)
			{
				Destroy(RootMotionController);
			}

			Utils.SetLayerRecursively(previewWraper.gameObject, GameConst.Data.UILayer);
			itemPreview.transform.localPosition = Vector3.zero;
			itemPreview.transform.localRotation = Quaternion.identity;
			bestPrize = new ChestItemView.ChestItemData(10, rewardSkin, itemPreview.gameObject);
			gemsModel.gameObject.SetActive(false);
		} else
		{
			bestPrize = new ChestItemView.ChestItemData(Random.Range(5, 11) * 100, null, null);
			gemsModel.gameObject.SetActive(true);
		}

		RefreshButton();
		Analytics.Instance.LogEvent("enter_chest_room");
	}

	public void OnDisable()
	{
		if (EventManager.Instance != null)
		{
			EventManager.Instance.RemoveEvent(GameEvent.OnCoinChange, OnCoinChange);
		}
	}

	void OnCoinChange(GameEvent Event_Type, Component Sender, object Param = null)
	{
		gemText.text = Profile.Instance.Coins.ToString();
	}

	public void OpenChest(ChestItemView chest)
	{
		if(Profile.Instance.KeyAmount <= 0)
		{
			Utils.Toast("Need more key");
			return;
		}
		Profile.Instance.KeyAmount--;

		key.color = 0 < Profile.Instance.KeyAmount ? Color.white : new Color(0, 0, 0, 0.2f);

		ChestItemView.ChestItemData data;
		bool isSpecial = openCount == bestPrizeIndex;
		if (isSpecial)
		{
			data = bestPrize;
			if(itemPreview != null)
			{
				surprise.gameObject.SetActive(true);
				surprise.transform.localScale = Vector3.zero;
				surprise.transform.DOScale(Vector3.one, 0.3f);
				surprise.transform.DOScale(Vector3.zero, 0.3f).SetDelay(3f);
			}
		} else
		{
			data = new ChestItemView.ChestItemData(Random.Range(1, 6) * 50, null, null);
		}
		chest.SetData(data, isSpecial);
		chest.Open();
		ClaimData(data);
		openCount++;

		RefreshButton();
	}

	public void OpenChestByRV(ChestItemView chest)
    {
		Ads.Instance.ShowRewardedAd((value) =>
		{
			if(value)
            {
				OpenChestFromUI(chest);
            }
		}, "open_chest");
    }

	public void OpenChestFromUI(ChestItemView chest)
	{
		ChestItemView.ChestItemData data;
		bool isSpecial = openCount == bestPrizeIndex;
		if (isSpecial)
		{
			data = bestPrize;
			if (itemPreview != null)
			{
				surprise.gameObject.SetActive(true);
				surprise.transform.localScale = Vector3.zero;
				surprise.transform.DOScale(Vector3.one, 0.3f);
				surprise.transform.DOScale(Vector3.zero, 0.3f).SetDelay(3f);
			}
		}
		else
		{
			data = new ChestItemView.ChestItemData(Random.Range(1, 6) * 50, null, null);
		}
		chest.SetData(data, isSpecial);
		chest.Open();
		ClaimData(data);
		openCount++;
		closeButton.gameObject.SetActive(true);
		var rvButton = chest.transform.GetChild(4);
		if(rvButton != null)
        {
			rvButton.gameObject.SetActive(false);
        }
		if (chests != null)
		{
			foreach(var c in chests)
            {
				var freeButton = c.transform.GetChild(5);
				if(freeButton != null)
                {
					freeButton.gameObject.SetActive(false);
				}
			}
		}
	}

	void RefreshButton()
	{
		getMoreKeyButton.gameObject.SetActive(Profile.Instance.KeyAmount <= 0 && openCount < 3);
		closeButton.gameObject.SetActive(openCount >= 1);
		guideText.gameObject.SetActive(!closeButton.gameObject.activeSelf && !getMoreKeyButton.gameObject.activeSelf);
	}

	public void GetMoreKey()
	{
		Ads.Instance.ShowRewardedAd((value) =>
		{
			if(value)
			{
				Profile.Instance.KeyAmount += 1;
				key.color = 0 < Profile.Instance.KeyAmount ? Color.white : new Color(0, 0, 0, 0.2f);

				RefreshButton();
			}
		}, "more_key");
	}

	public void Close()
	{
		SceneMaster.Instance.ShowLoading();
		SceneMaster.Instance.CloseScene(SceneID.RewardSkin);
		SceneMaster.Instance.ReloadScene(SceneID.Gameplay);
		SceneMaster.Instance.CloseScene(SceneID.ChestRoom);
	}

	void ClaimData(ChestItemView.ChestItemData data)
	{
		if(data.itemId != null && data.itemPreview != null)
		{
			OutfitItem.Instance.Unlock(data.itemId.ToString(), false);
			return;
		}

		Profile.Instance.Coins += data.gem;
	}
}
