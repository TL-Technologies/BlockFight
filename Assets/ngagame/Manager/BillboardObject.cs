using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardObject : MonoBehaviour
{
	Camera cam;
	private void Awake()
	{
		cam = Camera.main;
	}
	void Update()
	{
		if (cam == null) return;
		transform.rotation = cam.transform.rotation;
	}
}
