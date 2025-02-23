using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHelper : MonoBehaviour
{
	public GameObject target;

	void OnTriggerEnter(Collider collider)
	{
		if (target != null) target.SendMessage("OnTriggerEnter", collider, SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerStay(Collider collider)
	{
		if (target != null) target.SendMessage("OnTriggerStay", collider, SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerExit(Collider collider)
	{
		if (target != null) target.SendMessage("OnTriggerExit", collider, SendMessageOptions.DontRequireReceiver);
	}
}
