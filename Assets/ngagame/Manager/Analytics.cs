#if USE_GA
using GameAnalyticsSDK;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analytics : Singleton<Analytics>
{
	private bool FirebaseInitialized = false;

	public void Init()
	{
		RequestConversion();

#if USE_GA
		GameAnalyticsSDK.GameAnalytics.Initialize();
		GameAnalyticsSDK.GameAnalytics.OnRemoteConfigsUpdatedEvent += OnRemoteConfigsUpdatedEvent;
#endif

#if USE_FIREBASE
		if(FirebaseInitialized)
		{
			return;
		}
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				var app = Firebase.FirebaseApp.DefaultInstance;

				// Set a flag here to indicate whether Firebase is ready to use by your app.
				FirebaseInitialized = true;
			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});
#endif

#if USE_FB && false // Let MondayOFF do it
if (!Facebook.Unity.FB.IsInitialized) {
			// Initialize the Facebook SDK
			Facebook.Unity.FB.Init(InitCallback, OnHideUnity);
    } else {
			// Already initialized, signal an app activation App Event
			Facebook.Unity.FB.ActivateApp();
    }
#endif
	}

#if USE_FB
private void InitCallback ()
{
    if (Facebook.Unity.FB.IsInitialized) {
			// Signal an app activation App Event
			Facebook.Unity.FB.ActivateApp();
        // Continue with Facebook SDK
        // ...
    } else {
        Debug.Log("Failed to Initialize the Facebook SDK");
    }
}

private void OnHideUnity (bool isGameShown)
{
    if (!isGameShown) {
        // Pause the game - we will need to hide
        Time.timeScale = 0;
    } else {
        // Resume the game - we're getting focus again
        Time.timeScale = 1;
    }
}
#endif

	private void Start()
	{
#if USE_GA
		GameAnalyticsILRD.SubscribeIronSourceImpressions();
		Debug.Log("SubscribeIronSourceImpressions");
#endif
	}

	void OnRemoteConfigsUpdatedEvent()
	{
		try
		{
#if USE_GA
			int.TryParse(GameAnalytics.GetRemoteConfigsValueAsString("fs_time", "35"), out Ads.InterstitialIntervalTime);
			bool.TryParse(GameAnalytics.GetRemoteConfigsValueAsString("disable_ads", "false"), out Ads.DisbleAds);
#endif
		}
		catch { }
	}

	public void LogEvent(string name)
	{
		UnityEngine.Analytics.Analytics.CustomEvent(name);

		if (FirebaseInitialized)
		{
#if USE_FIREBASE
			Firebase.Analytics.FirebaseAnalytics.LogEvent(name);
#endif
		}

#if USE_GA
		GameAnalytics.NewDesignEvent(name);
#endif

	}

	public void LogEvent(string name, string param, string value)
	{
		UnityEngine.Analytics.Analytics.CustomEvent(name, new Dictionary<string, object>
			{
				{param, value}
			});

#if USE_GA
		GameAnalytics.NewDesignEvent(name, new Dictionary<string, object>
		{
			{param, value}
		});
#endif
	}

	#region Funnel

	public void LevelStart()
	{
		LogEvent($"level_{Profile.Instance.Level}_start");
#if USE_GA
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level_" + Profile.Instance.Level.ToString("D3"));
#endif
	}

	public void LevelComplete()
	{
		LogEvent($"level_{Profile.Instance.Level}_win");
#if USE_GA
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_" + Profile.Instance.Level.ToString("D3"));
#endif
	}

	public void LevelFail()
	{
		LogEvent($"level_{Profile.Instance.Level}_lose");
#if USE_GA
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level_" + Profile.Instance.Level.ToString("D3"));
#endif
	}

	public void LevelRevive()
	{
		LogEvent($"level_{Profile.Instance.Level}_revive");
	}

	public void LevelTimeout()
	{
		LogEvent($"level_{Profile.Instance.Level}_timeout");
#if USE_GA
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level_" + Profile.Instance.Level.ToString("D3"));
#endif
	}

	public void AdEvent(GAAdAction adAction, GAAdType adType, string adSdkName, string adPlacement)
	{
		LogEvent($"{adAction.ToString()}_{adType.ToString()}");
#if USE_GA
		try
		{
			GameAnalytics.NewAdEvent((GameAnalyticsSDK.GAAdAction)adAction, (GameAnalyticsSDK.GAAdType)adType, adSdkName, adPlacement);
		} catch
		{

		}
#endif
	}

	#endregion

	#region ConversionTracking

	public void RequestConversion()
	{
		StartCoroutine(IERequestConversion());
	}

	IEnumerator IERequestConversion()
	{
#if USE_AF
		while(AppsFlyerObjectScript.ConversionDataDictionary == null)
		{
			yield return new WaitForSeconds(1);
			var ConversionDataDictionary = AppsFlyerObjectScript.ConversionDataDictionary;
			if (ConversionDataDictionary != null)
			{
				if(ConversionDataDictionary.ContainsKey("af_status"))
				{
					LogEvent(ConversionDataDictionary["af_status"].ToString().ToLower());
					Debug.Log("dungnv: af_status: " + ConversionDataDictionary["af_status"]);
				}
			}
		}
#else
		yield return null;
#endif
	}

	#endregion
}

public enum GAAdAction
{
	Undefined = 0,
	Clicked = 1,
	Show = 2,
	FailedShow = 3,
	RewardReceived = 4,
	Request = 5,
	Loaded = 6
}

public enum GAAdType
{
	Undefined = 0,
	Video = 1,
	RewardedVideo = 2,
	Playable = 3,
	Interstitial = 4,
	OfferWall = 5,
	Banner = 6
}