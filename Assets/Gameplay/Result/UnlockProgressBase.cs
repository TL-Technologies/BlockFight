using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UnlockProgressBase : MonoBehaviour
{
	protected abstract string GetItemInProgress();
	protected abstract Sprite GetPreview(string itemName);
	protected abstract float UnlockProgress { get; set; }
	protected abstract float StepPercent { get; }

	[SerializeField] Slider fillProgress;
	[SerializeField] Text textProgress;
	string itemInProgress = string.Empty;
	[SerializeField] Button getMoreButton;
	[SerializeField] Image preview;
	[SerializeField] UnityEvent onReach;

	private void OnEnable()
	{
		itemInProgress = GetItemInProgress();
		if (string.IsNullOrEmpty(itemInProgress)) return;
		var skinPreview = GetPreview(itemInProgress);
		if (skinPreview != null) preview.sprite = skinPreview;
		UpdateProgress();
	}

	public void GetMorePercent()
	{
		Ads.Instance.ShowRewardedAd((value) =>
		{
			if (value)
			{
				UpdateProgress();
			}
		}, "more_unlock_percent");
	}

	Tween progressTween = null;
	void UpdateProgress()
	{
		var startPercent = UnlockProgress;
		var endPercent = startPercent + StepPercent;
		endPercent = Mathf.Clamp01(endPercent);
		getMoreButton.gameObject.SetActive(false);

		fillProgress.value = startPercent;
		textProgress.text = Mathf.RoundToInt(startPercent * 100) + "%";
		progressTween = DOVirtual.Float(0f, 1f, 0.95f, (value) =>
		{
			fillProgress.value = Mathf.Lerp(startPercent, endPercent, value);
			textProgress.text = Mathf.RoundToInt(fillProgress.value * 100) + "%";
		}).SetDelay(0.65f).OnComplete(delegate
		{
			getMoreButton.gameObject.SetActive(true);
			if (endPercent >= 1f && itemInProgress != string.Empty)
			{
				getMoreButton.gameObject.SetActive(false);
				onReach?.Invoke();
			}
		});
		UnlockProgress = endPercent;

		if (getMoreButton != null)
			getMoreButton.gameObject.SetActive(endPercent < 1f);
	}

	private void OnDisable()
	{
		if (progressTween != null)
		{
			progressTween.Kill();
		}
	}
}
