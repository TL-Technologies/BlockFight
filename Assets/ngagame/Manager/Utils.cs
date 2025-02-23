using System.Collections.Generic;
using UnityEngine;

namespace ngagame
{
	public class Utils
	{
		public static bool RequestShowRate = false;

#if !UNITY_EDITOR
	public static bool MobilePlatform => true;
#else
		public static bool MobilePlatform => false;
#endif

		public static void SetLayerRecursively(GameObject obj, int newLayer)
		{
			if (null == obj)
			{
				return;
			}

			obj.layer = newLayer;

			foreach (Transform child in obj.transform)
			{
				if (null == child)
				{
					continue;
				}
				SetLayerRecursively(child.gameObject, newLayer);
			}
		}

		public static int RandomSign()
		{
			return Random.Range(0, 2) * 2 - 1;
		}

		public static Vector2 RandomWithinTriangle(Vector2[] triangle)
		{
			var r1 = Mathf.Sqrt(Random.Range(0f, 1f));
			var r2 = Random.Range(0f, 1f);
			var m1 = 1 - r1;
			var m2 = r1 * (1 - r2);
			var m3 = r2 * r1;

			var p1 = triangle[0];
			var p2 = triangle[1];
			var p3 = triangle[2];
			return (m1 * p1) + (m2 * p2) + (m3 * p3);
		}

		public static Vector2 Vector3XZ(Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		#region Plugin

		private static bool PluginReady = false;
		private static AndroidJavaObject plugin;
		private static AndroidJavaClass unityPlayer;
		private static AndroidJavaObject activity;

		private static void InitPlugin()
		{
			if (MobilePlatform && !PluginReady)
			{
				unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				plugin = new AndroidJavaObject("com.dunghn94.utilslibrary.Utils");
				plugin.Call("SetActivity", activity);

				PluginReady = true;
			}
		}

		public static void Toast(string message)
		{
#if UNITY_EDITOR
			Debug.LogError("Toast: " + message);
			return;
#endif
#if UNITY_IOS
			Debug.LogError("Toast from ios: " + message);
			return;
#endif
			InitPlugin();
			if (activity != null)
			{
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
				{
					AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", activity, message, 0);
					toastObject.Call("show");
				}));
			}
		}

		#endregion
	}

	public class RandomPercent
	{
		List<int> data;
		public RandomPercent(params int[] weights)
		{
			data = new List<int>(weights.Length);
			for (int i = 0; i < weights.Length; i++)
			{
				for (int j = 0; j < weights[i]; j++)
				{
					data.Add(i);
				}
			}
		}

		public int RandomValue()
		{
			return data[Random.Range(0, data.Count - 1)];
		}
	}
}