using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThrowWeapon : Weapon
{
	[SerializeField] float throwForce = 500;
	[SerializeField] float hitForce = 500;
	[SerializeField] float minThrowRange = 2.5f;
	[SerializeField] LayerMask throwToLayers;
	CharacterBase thrower = null;
	float pickUpTime;

	Tween delayTween = null;
	public override bool Attack(CharacterBase _character)
	{
		if (Time.realtimeSinceStartup - pickUpTime < 2f) return false;
		var dir = _character.Character.transform.forward;
		if(_character.FocusTarget  != null)
		{
			var puppet = _character.FocusTarget.GetComponentInParent<CharacterBase>();
			if(puppet != null)
			{
				var target = puppet.PuppetMaster.muscles[puppet.PuppetMaster.GetMuscleIndex(HumanBodyBones.Head)];
				dir = target.transform.position - transform.position;
				if (dir.magnitude < minThrowRange) return false;
			}
		}

		if (Owner != null)
		{
			thrower = Owner;
			Owner.Drop();
		}
		var rb = GetComponent<Rigidbody>();

		rb.AddForce(dir.normalized * throwForce, ForceMode.Force);
		if(delayTween != null)
		{
			delayTween.Kill();
		}
		delayTween = DOVirtual.DelayedCall(5f, delegate
		{
			thrower = null;
		});
		return true;
	}

	public override void OnPickUp(CharacterBase _character)
	{
		base.OnPickUp(_character);
		thrower = null;
		pickUpTime = Time.realtimeSinceStartup;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(thrower != null)
		{
			if(LayerMaskExtensions.Contains(throwToLayers, collider.gameObject.layer))
			{
				var target = collider.GetComponentInParent<CharacterBase>();
				if(target != null && target != thrower)
				{
					SoundManager.Instance.PlaySFX(SFXConst.THROW_HIT);
					target.TakeDamage(Random.Range(minDamage, maxDamage), DamageType.Thrower);
					var force = (target.Character.GetPosition() - transform.position).normalized * hitForce;
					target.KnockOut(force + Vector3.up * 20000);
					thrower = null;
				}
			}
			
		}
	}
}
