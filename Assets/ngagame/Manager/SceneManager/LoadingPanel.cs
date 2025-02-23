using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
	public CanvasGroup CanvasGroup;
	public GameObject loadingObject;
	public GameObject shieldObject;
	public RectTransform bannerShield;

	public static LoadingPanel Instance;

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void ShowBannerShield()
	{
		var bannerHeight = Screen.width / (Screen.dpi / 160f) <= 720 ? 50 : 90;
		bannerShield.sizeDelta = new Vector2(bannerShield.sizeDelta.x, (Screen.dpi / 160f) * bannerHeight);
		bannerShield.gameObject.SetActive(true);
	}
}
