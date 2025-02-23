using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
#if USE_GOOGLE_ADS
using GoogleMobileAds.Api;
#endif
using UnityEngine.Events;

public class Ads : Singleton<Ads>
{
	public static bool DisbleAds = false;

	AdsConfig adsConfig;
	bool initialized = false;
	const string AD_NETWORK_NAME = "AdMob Mediation";
	readonly List<string> TEST_AD_COUNTRY_LIST = new List<string> { "xxx" }; // List of country will use test ads
	Action<bool> rewardCallback;
	AdsState rewardState = AdsState.None;
	Action<bool> interstitialCallback;
	AdsState interstitialState = AdsState.None;

	public float InterstitialTime { get; set; } = 0;
	public static int InterstitialIntervalTime = 15;

	public void Init()
	{
		if (initialized)
		{
			return;
		}

		GetCountryCode((value) =>
		{
			Debug.Log($"dungnv: countryCode: {UserCountryCode}");
			InternalInit();
		});
	}

    void InternalInit()
	{
		adsConfig = Resources.Load<AdsConfig>("AdsConfig");
		if (adsConfig == null || (!string.IsNullOrEmpty(UserCountryCode) && TEST_AD_COUNTRY_LIST.Contains(UserCountryCode)))
		{
			// Use test app id
			Analytics.Instance.LogEvent($"use_test_ads_{UserCountryCode}");
			adsConfig = new AdsConfig();
		}
#if USE_GOOGLE_ADS
		// Initialize the Mobile Ads SDK.
		MobileAds.Initialize((initStatus) =>
		{
			RequestInterstitial();
			RequestRewardAd();
			if(adsConfig.debug)
			{

			}
			Debug.LogError("dungnv: MobileAds initStatus " + initStatus);
			Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
			foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
			{
				string className = keyValuePair.Key;
				AdapterStatus status = keyValuePair.Value;
				switch (status.InitializationState)
				{
					case AdapterState.NotReady:
						// The adapter initialization did not complete.
						Debug.Log("Adapter: " + className + " not ready.");
						break;
					case AdapterState.Ready:
						// The adapter was successfully initialized.
						Debug.Log("Adapter: " + className + " is initialized.");
						break;
				}
			}
			initialized = true; // Load one time
		});
#endif
		InterstitialTime = Time.realtimeSinceStartup;
	}

	public void ShowInterstitial(Action<bool> callback, string placement = "")
	{
		if (
			Profile.Instance.VIP
			|| Time.realtimeSinceStartup - InterstitialTime < InterstitialIntervalTime
			|| DisbleAds
			)
		{
			callback?.Invoke(false);
			interstitialState = AdsState.None;
			return;
		}

#if USE_GOOGLE_ADS
		if (ngagame.Utils.MobilePlatform)
		{
			if (this.interstitial.IsLoaded())
			{
				this.interstitial.Show();
				interstitialCallback = callback;
				interstitialState = AdsState.Showing;
			}
			else
			{
				if (Application.internetReachability != NetworkReachability.NotReachable)
				{
					RequestInterstitial();
					Analytics.Instance.LogEvent("attemp_request_interstitial");
				}
				callback?.Invoke(false);
				interstitialState = AdsState.None;
			}
		} else
		{
			callback?.Invoke(false);
			interstitialState = AdsState.None;
		}
#else
			callback?.Invoke(true);
			interstitialState = AdsState.None;
#endif
	}

#if USE_GOOGLE_ADS
	InterstitialAd interstitial;
	bool subInterstitial = false;
#endif
	private void RequestInterstitial()
	{
		string adUnitId = adsConfig.GoogleInterstitialId;

#if USE_GOOGLE_ADS
		// Initialize an InterstitialAd.
		this.interstitial = new InterstitialAd(adUnitId);

		this.interstitial.OnAdOpening += HandleOnAdOpening;// Called when an ad is shown.
		this.interstitial.OnAdClosed += HandleOnAdClosed;// Called when the ad is closed.
		this.interstitial.OnAdFailedToShow += HandleOnAdFailedToShow;

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		this.interstitial.LoadAd(request);
#endif
	}

	public void HandleOnAdFailedToShow(object sender, EventArgs args)
	{
		interstitialState = AdsState.Failed;
	}

	public void HandleOnAdOpening(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdOpening event received");
		interstitialState = AdsState.Showing;
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdClosed event received");
		interstitialState = AdsState.Closed;
	}

#if USE_GOOGLE_ADS
	public bool RewardAvailble => rewardedAd != null && rewardedAd.IsLoaded();
#else
public bool RewardAvailble => false;
#endif

	public void ShowRewardedAd(Action<bool> callback, string placementName = "")
	{
		if (ngagame.Utils.MobilePlatform)
		{
#if USE_GOOGLE_ADS
			if (this.rewardedAd.IsLoaded())
			{
				this.rewardedAd.Show();
				InterstitialTime = Time.realtimeSinceStartup;
				rewardCallback = callback;
				rewardState = AdsState.Showing;
			}
			else
			{
				RequestRewardAd();
				callback?.Invoke(false);
				rewardState = AdsState.None;
				Analytics.Instance.LogEvent("attemp_request_reward");
				ngagame.Utils.Toast("No AD available");
			}
#endif
		}
		else
		{
			callback?.Invoke(true);
			rewardState = AdsState.None;
		}

	}

#if USE_GOOGLE_ADS
	RewardedAd rewardedAd;
	bool subReward = false;
#endif
	private void RequestRewardAd()
	{
		string adUnitId = adsConfig.GoogleRewardId;

#if USE_GOOGLE_ADS
		this.rewardedAd = new RewardedAd(adUnitId);

		// Called when an ad request failed to show.
		this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
		// Called when the ad is closed.
		this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the rewarded ad with the request.
		this.rewardedAd.LoadAd(request);
#endif
	}

	public void HandleRewardedAdFailedToShow(object sender, EventArgs args)
	{
		rewardState = AdsState.Failed;
	}

	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{
		rewardState = AdsState.Closed;
	}

	private void Update()
	{
		if (interstitialState == AdsState.Failed || interstitialState == AdsState.Closed)
		{
			interstitialState = AdsState.None;
			interstitialCallback?.Invoke(true);
			interstitialCallback = null;
			SceneMaster.Instance.HideLoading();
		}
		if (rewardState == AdsState.Failed || rewardState == AdsState.Closed)
		{
			rewardState = AdsState.None;
			rewardCallback?.Invoke(true);
			rewardCallback = null;
			SceneMaster.Instance.HideLoading();
		}
	}

#region Get country
	public static string UserCountryCode = string.Empty;
	static string COUNTRY_CODE_KEY = "COUNTRY_CODE_KEY";

	void GetCountryCode(System.Action<string> onComplete)
	{
		var code = PlayerPrefs.GetString(COUNTRY_CODE_KEY, string.Empty);
		if (!string.IsNullOrEmpty(code))
		{
			UserCountryCode = code;
			onComplete?.Invoke(code);
			return;
		}
		StartCoroutine(IEGetCountryCode((value) =>
		{
			Debug.Log("get online");
			if (!string.IsNullOrEmpty(value))
			{
				PlayerPrefs.SetString(COUNTRY_CODE_KEY, value);
				UserCountryCode = value;
			}
			onComplete?.Invoke(value);
		}));
	}

	IEnumerator IEGetCountryCode(System.Action<string> onComplete)
	{
		string uri = "http://ip-api.com/json";
		string countryCode = string.Empty;
		using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
		{
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();
			var receivedText = string.Empty;
			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.DataProcessingError:
					Debug.Log("Error: " + webRequest.error);
					break;
				case UnityWebRequest.Result.ProtocolError:
					Debug.Log("HTTP Error: " + webRequest.error);
					break;
				case UnityWebRequest.Result.Success:
					receivedText = webRequest.downloadHandler.text;
					break;
			}

			if (!string.IsNullOrEmpty(receivedText))
			{
				var dict = SimpleJSON.JSON.Parse(receivedText);

				if (dict != null && dict.HasKey("countryCode"))
				{
					countryCode = dict["countryCode"];
				}
			}
			onComplete?.Invoke(countryCode.ToLower());
		}
	}
#endregion

	enum AdsState
	{
		None,
		Showing,
		Failed,
		Closed
	}
}