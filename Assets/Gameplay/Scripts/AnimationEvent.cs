using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationEvent : MonoBehaviour
{
	Animator animator;
	public int layer;
	public Action<string> OnAnimationEvent;

	private void Awake()
	{
		animator = GetComponent<Animator>();	
	}

	public void FireEvent(string data)
	{
		OnAnimationEvent?.Invoke(data);
	}
}
