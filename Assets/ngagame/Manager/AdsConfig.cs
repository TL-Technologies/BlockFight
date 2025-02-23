using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsConfig : ScriptableObject
{
	[Header("Android")]
	[SerializeField] string googleAppIdAndroid = "ca-app-pub-3940256099942544~3347511713";
	[SerializeField] string googleInterstitialUnitIdAndroid = "ca-app-pub-3940256099942544/1033173712";
	[SerializeField] string googleRewardUnitIdAndroid = "ca-app-pub-3940256099942544/5224354917";
	[Header("IOS")]
	[SerializeField] string googleAppIdIOS = "ca-app-pub-3940256099942544~1458002511";
	[SerializeField] string googleInterstitialUnitIdIOS = "ca-app-pub-3940256099942544/4411468910";
	[SerializeField] string googleRewardUnitIdIOS = "ca-app-pub-3940256099942544/1712485313";

	public bool debug = false;
	public List<string> testDeviceIds = new List<string>();

	public string GoogleAppId
	{
		get
		{
#if UNITY_ANDROID
			return googleAppIdAndroid;
#else
			return googleAppIdIOS;
#endif
		}
	}

	public string GoogleInterstitialId
	{
		get
		{
#if UNITY_ANDROID
			return googleInterstitialUnitIdAndroid;
#else
			return googleInterstitialUnitIdIOS;
#endif
		}
	}

	public string GoogleRewardId
	{
		get
		{
#if UNITY_ANDROID
			return googleRewardUnitIdAndroid;
#else
			return googleRewardUnitIdIOS;
#endif
		}
	}
}
