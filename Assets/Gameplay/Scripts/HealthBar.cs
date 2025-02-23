using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image green;
    [SerializeField] Image red;

    float value = 1f;
	public float Value
	{
		get => value;
		set
		{
			if(value != this.value)
			{
				this.value = Mathf.Clamp01(value);
				Refresh();
			}
		}
	}

	Tween greenTween, redTween;
	public void Refresh()
	{
		if (greenTween != null) greenTween.Kill();
		if (redTween != null) redTween.Kill();
		greenTween = green.DOFillAmount(value, 0.5f);
		redTween = red.DOFillAmount(value, 0.3f).SetDelay(0.8f);
	}

	private void Start()
	{
		Refresh();
	}
}
