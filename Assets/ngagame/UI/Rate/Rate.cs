using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rate : MonoBehaviour
{
	[SerializeField] Transform starGroup;
	[SerializeField] int levelRequire = 8;
	[SerializeField] GameObject content;

	int result = 0;
	const string RATE_SHOWED_KEY = "RATE_SHOWED_KEY";

    // Start is called before the first frame update
    void Start()
    {
		bool rateShowed = PlayerPrefs.GetInt(RATE_SHOWED_KEY, -1) > 0;
		content.gameObject.SetActive(Profile.Instance.Level == 8 && !rateShowed);
    }

	public void OnStarClick(int index)
	{
		result = index;
		if(starGroup == null)
		{
			return;
		}
		for(int i = 0; i < starGroup.childCount; i++)
		{
			var child = starGroup.GetChild(i);
			var active = child != null ? child.GetChild(0) : null;
			if(active != null)
			{
				active.gameObject.SetActive(i <= index);
			}
		}
	}

	public void OnRateClick()
	{
		gameObject.SetActive(false);
		PlayerPrefs.SetInt(RATE_SHOWED_KEY, 1);
		if (result > 2)
		{
			Application.OpenURL($"https://play.google.com/store/apps/details?id=" + Application.identifier);
		} else
		{
			ngagame.Utils.Toast("Thank for rating!");
		}
	}

	private void OnValidate()
	{
		content.gameObject.SetActive(false);
	}
}
