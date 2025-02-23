using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnlockProgressOutfit : UnlockProgressBase
{
	protected override float UnlockProgress {
		get => OutfitItem.Instance.UnlockProgress;
		set => OutfitItem.Instance.UnlockProgress = value;
	}

	protected override float StepPercent
	{
		get
		{
			return ArenaItem.Instance.UnlockIndex < 1 ? 0.35f : 0.25f;
		}
	}

	protected override string GetItemInProgress()
	{
		return OutfitItem.Instance.GetItemInProgress();
	}

	protected override Sprite GetPreview(string itemName)
	{
		return OutfitItem.Instance.GetPreview(itemName);
	}
}
