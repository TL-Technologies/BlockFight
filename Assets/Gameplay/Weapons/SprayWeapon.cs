using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayWeapon : Weapon
{
	[SerializeField] GameObject fx;
	[SerializeField] float maxVolume = 0.5f;
	[SerializeField] float maxDistanceOfSound = 5f;
	AudioSource audioSource;

	protected override void Awake()
	{
		base.Awake();
		fx.gameObject.SetActive(false);
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (audioSource == null) return;
		if(Owner != null)
		{
			var distance = Vector3.Distance(Gameplay.Instance.Player.Character.GetPosition(), transform.position);
			audioSource.volume = Mathf.Lerp(maxVolume, 0, distance / maxDistanceOfSound);
		} else
		{
			audioSource.volume = 0;
		}
	}

	public override bool Attack(CharacterBase _character)
	{
		if (Owner != null && Owner.DetectTarget != null)
		{
			Owner.DetectTarget.TakeDamage(Random.Range(minDamage, maxDamage), DamageType.Fire);
		}
			return true;
	}

	public override void OnPickUp(CharacterBase _character)
	{
		base.OnPickUp(_character);
		fx.gameObject.SetActive(true);
		audioSource.Play();
	}

	public override void OnDrop(CharacterBase _character)
	{
		base.OnDrop(_character);
		fx.gameObject.SetActive(false);
		audioSource.Stop();
	}
}
