using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
	[SerializeField] CharacterBase character;
	[SerializeField] Transform defaultParent;
	[SerializeField] Vector3 globalScale;

	Quaternion quaternionOriginal;

	private void Start()
	{
		gameObject.SetActive(character != null && character.isPlayer);
		quaternionOriginal = transform.rotation;
		SetGlobalScale(globalScale);
	}

	void SetGlobalScale(Vector3 _globalScale)
	{
		transform.parent = null;
		transform.localScale = _globalScale;
		if(defaultParent != null) transform.parent = defaultParent;
	}

	private void Update()
	{
		transform.rotation = quaternionOriginal;
	}
}
