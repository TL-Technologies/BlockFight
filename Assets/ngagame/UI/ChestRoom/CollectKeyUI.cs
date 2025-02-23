using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectKeyUI : MonoBehaviour
{
    [SerializeField] GameObject anim;
	public static CollectKeyUI Instance;
	Animator animator;

	private void Awake()
	{
		Instance = this;

		animator = GetComponent<Animator>();
		anim.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void Collect()
	{
		if(animator == null)
		{
			return;
		}

		anim.gameObject.SetActive(true);
		animator.SetTrigger("play");
	}
}
