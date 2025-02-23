using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnlockProgressArena : UnlockProgressBase
{
	protected override float UnlockProgress {
		get => ArenaItem.Instance.UnlockProgress;
		set => ArenaItem.Instance.UnlockProgress = value;
	}

	protected override float StepPercent { 
		get {
			return ArenaItem.Instance.UnlockIndex < 4 ? 1f : Random.Range(0.35f, 0.5f);
		}
	}

	protected override string GetItemInProgress()
	{
		return ArenaItem.Instance.GetItemInProgress();
	}

	protected override Sprite GetPreview(string itemName)
	{
		return ArenaItem.Instance.GetPreview(itemName);
	}
}
