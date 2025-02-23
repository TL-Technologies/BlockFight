using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeaponSpawn : MonoBehaviour
{
	[SerializeField] SpawnPoint spawnPoint;
	[SerializeField] float minDelay = 0;
	[SerializeField] float maxDelay = 5;
	[SerializeField] float randomRange = 0f;
	[SerializeField] float startHeight = 5f;
	[SerializeField] int minArenaOwnedRequire = 1;
	[SerializeField] GameObject[] weapons;
	Transform spawnTarget;

	private void OnEnable()
	{
		EventManager.Instance.AddListener(GameEvent.OnLevelStart, OnLevelStart);
	}

	private void OnDisable()
	{
		if(EventManager.Instance != null)
		{
			EventManager.Instance.RemoveEvent(GameEvent.OnLevelStart, OnLevelStart);
		}
	}

	void OnLevelStart(GameEvent Event_Type, Component Sender, object Param = null)
	{
		if (ArenaItem.Instance.UnlockIndex < minArenaOwnedRequire) return;
		var delayTime = maxDelay > 0 ? Random.Range(minDelay, maxDelay) : 0f;
		if (delayTime > 0)
			DOVirtual.DelayedCall(delayTime, delegate
			{
				Spawn();
			});
		else
			Spawn();
	}

	void Spawn()
	{
		var spawnTargets = (transform.parent != null) ? transform.parent.GetComponentsInChildren<CharacterBase>() : null;
		switch (spawnPoint)
		{
			case SpawnPoint.Default:
				spawnTarget = transform;
				break;
			case SpawnPoint.Player:
				if (spawnTargets != null)
				{
					foreach (var s in spawnTargets)
					{
						if (s.isPlayer)
						{
							spawnTarget = s.Character.transform;
						}
					}
				}
				break;
			case SpawnPoint.Enemy:

				if (spawnTargets != null)
				{
					foreach (var s in spawnTargets)
					{
						if (!s.isPlayer)
						{
							spawnTarget = s.Character.transform;
						}
					}
				}
				break;
		}

		var pos = spawnTarget != null ? spawnTarget.transform.position + Vector3.up * startHeight : transform.position;
		if (randomRange > 0)
		{
			pos += Random.Range(-randomRange, randomRange) * Vector3.right + Random.Range(-randomRange, randomRange) * Vector3.forward;
		}
		var weaponPrefab = weapons.Length > 0 ? weapons[Random.Range(0, weapons.Length)] : null;
		if (weaponPrefab == null) return;
		var weapon = Instantiate(weaponPrefab, pos, Random.rotation, transform);
	}

	enum SpawnPoint
	{
		Default,
		Player,
		Enemy
	}
}
