using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : Singleton<DontDestroy>
{
	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
}
