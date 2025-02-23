using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
	public static bool Holding = false;
	[SerializeField] GameObject model;
	[SerializeField] GameObject fx;
	Collider triggerCollider;
	const string RANDOM_APPEAR_INDEX_KEY = "RANDOM_APPEAR_INDEX_KEY";
	const int RANDOM_APPEAR_RANGE = 3;

	private void Awake()
	{
		Holding = false;
		model.gameObject.SetActive(true);
		fx.gameObject.SetActive(false);
		triggerCollider = GetComponentInChildren<Collider>();

		var randomAppearIndex = PlayerPrefs.GetInt(RANDOM_APPEAR_INDEX_KEY, -1);
		if(randomAppearIndex < 0)
		{
			randomAppearIndex = Random.Range(0, RANDOM_APPEAR_RANGE);
			if(randomAppearIndex == 1)
			{
				randomAppearIndex = 0;
			}
			PlayerPrefs.SetInt(RANDOM_APPEAR_INDEX_KEY, randomAppearIndex);
		}
	}

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != GameConst.Data.characterLayer)
		{
			return;
		}

		Holding = true;
		if(triggerCollider != null)
		{
			triggerCollider.enabled = false;
		}
		model.gameObject.SetActive(false);
		fx.gameObject.SetActive(true);
		CollectKeyUI.Instance?.Collect();
		SoundManager.Instance.PlaySFX(SFXConst.COLLECT_COIN);
	}
}
