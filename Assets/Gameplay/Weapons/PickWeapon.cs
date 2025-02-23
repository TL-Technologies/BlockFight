using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickWeapon : MonoBehaviour
{
	[SerializeField] GameObject owner;
	[SerializeField] GameObject weaponFX;
	Weapon weapon;
	private void Awake()
	{
		weapon = owner.GetComponent<Weapon>();
	}
	private void OnTriggerEnter(Collider other)
	{
		weapon.OnPickUpTrigger(other);
		weaponFX.gameObject.SetActive( weapon.Owner == null);
	}
}
