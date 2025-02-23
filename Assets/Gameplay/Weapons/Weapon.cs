using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public static string TAG = "Weapon";
	public WeaponType type;
	public float attackRange = 1.5f;
	public float impactRadius = 1f;
	public float aimRange = 4f;
	public float attackPulse = 0.8f;
	public float minDamage = 0.05f;
	public float maxDamage = 0.1f;

	[SerializeField] LayerMask characterLayers;
	public bool IsPickedUp => Owner != null;
	public CharacterBase Owner { get; private set; }
	Collider[] weaponColliders;
	public float DropTime { get; set; }

	protected virtual void Awake()
	{
		weaponColliders = GetComponentsInChildren<Collider>();
	}

	public void OnPickUpTrigger(Collider collider)
	{
		if (Time.realtimeSinceStartup - DropTime < 3f) return;
		if (IsPickedUp) return;

		if (!LayerMaskExtensions.Contains(characterLayers, collider.gameObject.layer)) return;

		Owner = collider.GetComponentInParent<CharacterBase>();
		if (Owner == null) return;
		if (!Owner.Standing || Owner.Holding)
		{
			Owner = null;
			return;
		}
		Owner.PickUp(this);
	}

	public virtual void OnPickUp(CharacterBase _character)
	{
		var rb = GetComponent<Rigidbody>();
		if (rb != null) Destroy(rb);

		transform.parent = _character.WeaponHolder.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		// Ignore collision of hand and weapon
		if (_character.RightHandMuscle != null && weaponColliders != null && _character.RightHandMuscle.colliders != null)
			foreach (var weaponCollider in weaponColliders)
			{
				if (!weaponCollider.isTrigger)
				{
					foreach (var handCollider in _character.RightHandMuscle.colliders)
					{
						Physics.IgnoreCollision(weaponCollider, handCollider);
					}
				}
			}
	}

	public virtual void OnDrop(CharacterBase _character)
	{
		gameObject.AddComponent<Rigidbody>();
		DropTime = Time.realtimeSinceStartup;
		Owner = null;
	}

	public virtual bool Attack(CharacterBase _character)
	{
		return true;
	}
}

public enum WeaponType
{
	Boxing,
	Melee,
	Shooting,
	Thrower
}
