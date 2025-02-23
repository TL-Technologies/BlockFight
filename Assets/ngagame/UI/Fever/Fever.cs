using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fever : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Image fill;
    [SerializeField] Image glow;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
	[SerializeField] float reduceSpeed = 0.05f;
	[SerializeField] float feverTime = 10;

	public static Fever Instance;

	private void Awake()
	{
		Instance = this;
		feverTimeStatic = feverTime;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void TurnOn()
	{
		Value = 0;
		panel.gameObject.SetActive(true);
	}

	public void TurnOff()
	{
		panel.gameObject.SetActive(false);
		OnStartFever = null;
		OnEndFever = null;
	}

	static float val;
	public static float Value
	{
		get => val;
		private set
		{
			val = Mathf.Clamp01(value);
		}
	}
	public static UnityAction OnStartFever;
	public static UnityAction OnEndFever;

	static Tween changeTween = null;
	static bool charging => changeTween != null && changeTween.IsPlaying();
	static Tween feverReduceTween = null;
	static bool fevering => feverReduceTween != null && feverReduceTween.IsPlaying();
	public void ChangeAmount(float amount)
	{
		if(fevering)
		{
			return;
		}
		if(changeTween != null)
		{
			changeTween.Kill();
		}
		var target = Value + amount;
		changeTween = DOVirtual.Float(Value, target, 0.5f, (value) =>
		{
			Value = value;
			if(Value >= 0.95f)
			{
				val = 1;
				changeTween.Kill();
				StartFever();
			}
		});
	}

	static float feverTimeStatic = 10f;
	static float glowAlpha = 0;
	static Tween glowTween = null;
	void StartFever()
	{
		OnStartFever?.Invoke();
		feverReduceTween = DOVirtual.Float(Value, 0f, feverTimeStatic, (value) =>
		{
			Value = value;
		}).OnComplete(EndFever).SetEase(Ease.Linear);

		if(glowTween != null)
		{
			glowTween.Kill();
		}
		glowTween = DOVirtual.Float(0f, 1f, 0.35f, (value) =>
		{
			glowAlpha = value;
		}).SetLoops(-1, LoopType.Yoyo);
	}

	void EndFever()
	{
		OnEndFever?.Invoke();
		if (glowTween != null)
		{
			glowTween.Kill();
		}
		glowAlpha = 0;
	}

	private void Update()
	{
		fill.rectTransform.sizeDelta = new Vector2(
			fill.rectTransform.sizeDelta.x,
			Mathf.Lerp(minHeight, maxHeight, Value
			));
		glow.color = new Color(glow.color.r, glow.color.g, glow.color.b, glowAlpha);
		if(!charging && !fevering)
		{
			Value -= reduceSpeed * Time.deltaTime;
		}
	}
}
