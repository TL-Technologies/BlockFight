using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelBar : MonoBehaviour
{
	[SerializeField] CanvasGroup canvasGroup;
	[SerializeField] GameObject dotGroup;
	[SerializeField] Image fill;
	[SerializeField] int startAppearLevel = 1;

	int dotAmount = 1;
	int chapterIndex = 0;
	private void Init(int level)
	{
		gameObject.SetActive(level >= startAppearLevel);
		if(dotGroup == null)
		{
			Debug.LogError("Error: dotGroup is null!");
			return;
		}
		dotAmount = dotGroup.transform.childCount;
		chapterIndex = (level - 1) / dotAmount;
		for(int i = 0; i < dotAmount; i++)
		{
			var dot = dotGroup.transform.GetChild(i);
			var dotText = dot.GetChild(3).GetComponent<Text>();
			
			if(dotText != null)
			{
				int levelNumber = chapterIndex * dotAmount + i + 1;
				dotText.text = levelNumber.ToString();
				
				int status = 0;
				if(level == levelNumber)
				{
					status = 2;
				} else
				{
					status = levelNumber > level ? 0 : 1;
				}
				SetStatus(dot, status);
				SetSpecial(dot, levelNumber % dotAmount == 0);
			}
		}

		fill.fillAmount = (level - 1) % dotAmount * (1f / (dotAmount - 1));
	}

	private void Start()
	{
		Init(Profile.Instance.Level);
	}

	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnLevelWarmup, OnLevelWarmup);
		EventManager.Instance.AddListener(GameEvent.OnLevelStart, OnLevelStart);
		EventManager.Instance.AddListener(GameEvent.OnLevelEnd, OnLevelEnd);
	}

	private void OnDisable()
	{
		var eventManager = EventManager.Instance;
		if(eventManager == null)
		{
			return;
		}
		eventManager.RemoveEvent(GameEvent.OnLevelWarmup, OnLevelWarmup);
		eventManager.RemoveEvent(GameEvent.OnLevelStart, OnLevelStart);
		eventManager.RemoveEvent(GameEvent.OnLevelEnd, OnLevelEnd);
	}

	void OnLevelWarmup(GameEvent Event_Type, Component Sender, object Param = null)
	{

	}

	void OnLevelStart(GameEvent Event_Type, Component Sender, object Param = null)
	{
		return;
		if(canvasGroup != null)
		{
			canvasGroup.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(delegate
			{
				canvasGroup.gameObject.SetActive(false);
			});
		} else
		{
			canvasGroup.gameObject.SetActive(false);
		}
	}

	void OnLevelEnd(GameEvent Event_Type, Component Sender, object Param = null)
	{
		var win = true;
		
		canvasGroup.gameObject.SetActive(true);
		if (canvasGroup != null)
		{
			//canvasGroup.alpha = 0;
			//canvasGroup.DOFade(1, 0.5f);

			if (!win)
			{
				return;
			}
			for (int i = 0; i < dotAmount; i++)
			{
				var dot = dotGroup.transform.GetChild(i);
				if (dot.GetChild(2).gameObject.activeSelf)
				{
					
					dot.transform.localScale = Vector3.one * 0.9f;
					dot.transform.DOScale(1.1f, 0.35f).SetDelay(1.0f).SetEase(Ease.InOutBack).OnComplete(delegate
					{
						dot.GetChild(1).gameObject.SetActive(true);
						dot.GetChild(2).gameObject.SetActive(false);
					});
					
				}
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="dot">dot transform</param>
	/// <param name="status"> 0 = normal, 1 = passed, 2 = current</param>
	void SetStatus(Transform dot, int status)
	{
		dot.GetChild(0).gameObject.SetActive(status == 0);
		dot.GetChild(1).gameObject.SetActive(status == 1);
		dot.GetChild(2).gameObject.SetActive(status == 2);
	}

	void SetSpecial(Transform dot, bool isSpecial)
	{
		dot.GetChild(3).gameObject.SetActive(!isSpecial);
		dot.GetChild(4).gameObject.SetActive(isSpecial);
	}
}
