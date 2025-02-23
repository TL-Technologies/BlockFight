using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] float speed = 1;
	[SerializeField] float lifeTime = 5;
	[SerializeField] LayerMask hitLayers;
	[SerializeField] GameObject hitFX;
	[SerializeField] float damage = 0.1f;
	Vector3 direction;
	float travel = 0;

	private void Awake()
	{
		hitFX.gameObject.SetActive(false);
	}

	public void Fire(Vector3 _direction)
	{
		direction = _direction;
	}

	private void FixedUpdate()
	{
		transform.position = transform.position + direction * speed * Time.fixedDeltaTime;
		travel += speed * Time.fixedDeltaTime;
		if(travel > lifeTime)
		{
			Hit();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!LayerMaskExtensions.Contains(hitLayers, other.gameObject.layer)) return;
		var character = other.GetComponentInParent<CharacterBase>();
		if(character != null)
		{
			character.TakeDamage(damage, DamageType.Bullet);
		}
		Hit();
	}

	void Hit()
	{
		SoundManager.Instance.PlaySFX(SFXConst.BULLET_HIT);
		var fx = Instantiate(hitFX, transform.parent);
		fx.transform.position = hitFX.transform.position;
		fx.gameObject.SetActive(true);
		gameObject.SetActive(false);
		enabled = false;
	}
}
