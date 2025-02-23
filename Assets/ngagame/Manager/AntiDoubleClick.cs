using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AntiDoubleClick : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] float antiTime = 1f;

	MaskableGraphic MaskableGraphic;
	float timeSinceAnti = 0f;

	private void Awake()
	{
		MaskableGraphic = GetComponent<MaskableGraphic>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(MaskableGraphic == null)
		{
			return;
		}
		timeSinceAnti = antiTime;
		MaskableGraphic.raycastTarget = false;
	}

	private void Update()
	{
		if(timeSinceAnti <= 0f)
		{
			return;
		}
		timeSinceAnti -= Time.deltaTime;
		if (timeSinceAnti <= 0f)
		{
			MaskableGraphic.raycastTarget = true;
		}
	}
}
