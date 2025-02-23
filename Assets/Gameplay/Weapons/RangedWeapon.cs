using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
	[SerializeField] Transform firePoint;
	[SerializeField] Bullet bullet;
	[SerializeField] GameObject fireFX;

	protected override void Awake()
	{
		base.Awake();
		bullet.gameObject.SetActive(false);
		fireFX.gameObject.SetActive(false);
	}

	public override bool Attack(CharacterBase _character)
	{
		var fx = Instantiate(fireFX, firePoint.transform);
		fx.transform.position = fireFX.transform.position;
		fx.gameObject.SetActive(true);

		var b = Instantiate(bullet);
		if (Owner != null)
		{
			b.transform.parent = Owner.transform;
			if(Owner.DetectTarget != null)
			{
				var targetPoint = Owner.DetectTarget.Character.GetPosition() + Vector3.up * firePoint.transform.position.y;
				b.Fire((targetPoint - firePoint.transform.position).normalized);
			}
			else
			{
				b.Fire(firePoint.transform.forward);
			}
		} else
		{
			b.Fire(firePoint.transform.forward);
		}
		
		b.transform.position = firePoint.transform.position;
		b.gameObject.SetActive(true);
		SoundManager.Instance.PlaySFX(SFXConst.SHOOT_PISTOL);
		return true;
	}
}
