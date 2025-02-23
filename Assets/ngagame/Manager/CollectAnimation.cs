using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class CollectAnimation : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] int amount = 10;
	[SerializeField] float radius = 10;
	[SerializeField] bool autoStart = false;
	[SerializeField] UnityEvent completeEvent;
	[SerializeField] UnityEvent<float> stepEvent;
	[SerializeField] float moveSpeed = 1000f;
	[SerializeField] float delay = 0f;


	Transform[] icons;

	private void Awake()
	{
		var icon = transform.GetChild(0);
		icon.gameObject.SetActive(false);
		icons = new Transform[amount];
		for(int i = 0; i < amount; i++)
		{
			icons[i] = Instantiate(icon, transform);
			icons[i].gameObject.SetActive(false);
		}
	}

    private void OnEnable()
    {
        if(!autoStart)
        {
			return;
        }
		if(delay > 0)
        {
			DOVirtual.DelayedCall(delay, delegate
			{
				Collect(null, null);
			});
		}
		else
        {
			Collect(null, null);
		}
    }

    const float EXPLODE_TIME = 0.6f;

	public void Collect(System.Action onComplete, System.Action<float> onStep)
	{
		var distance = Vector3.Distance(transform.position, target.position);
		for(int i = 0; i < icons.Length; i++)
		{
			var icon = icons[i];
			icon.gameObject.SetActive(true);
			icon.transform.localPosition = Vector3.zero;
			icon.DOLocalMove(Random.insideUnitSphere * radius, EXPLODE_TIME);
			var p = (float)i / (icons.Length - 1);
			icon.DOMove(target.position, distance / moveSpeed).SetDelay(EXPLODE_TIME + i * 0.08f).OnComplete(delegate
			{
				onStep?.Invoke(p);
				stepEvent?.Invoke(p);
				icon.gameObject.SetActive(false);
			});
		}

		DOVirtual.DelayedCall(distance / moveSpeed + EXPLODE_TIME + icons.Length * 0.05f, delegate
		{
			onComplete?.Invoke();
			completeEvent?.Invoke();
		});
	}
}
