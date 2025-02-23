using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
	[SerializeField] float minImpulse;
	[SerializeField] float relativeVelocity;
	[SerializeField] GameObject collisionFX;
	float attackTime;

	public void OnImpulse(CharacterBase character, MuscleCollision collision)
	{
		if (!IsPickedUp) return;
		if (Time.realtimeSinceStartup - attackTime < attackPulse) return;
		if (collision.collision.impulse.magnitude > minImpulse && collision.collision.relativeVelocity.magnitude > relativeVelocity)
		{
			if(collision.collision.contactCount > 0 &&  Owner != null)
			{
				Instantiate(collisionFX, collision.collision.contacts[0].point, Quaternion.identity, Owner.transform);
				character.TakeDamage(Random.Range(minDamage, maxDamage), DamageType.MeleeWeapon);
				character.Character.Push((character.Character.GetPosition() - transform.position).normalized, Random.Range(0.5f, 0.7f));

				SoundManager.Instance.PlaySFX(SFXConst.LIGHT_COLLISION);
			}
			attackTime = Time.realtimeSinceStartup;
		}
	}
}
