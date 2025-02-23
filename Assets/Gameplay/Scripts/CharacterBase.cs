using ECM2.Characters;
using ECM2.Helpers;
using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Micosmo.SensorToolkit;

public class CharacterBase : MonoBehaviour
{
	const string ATTACK_TYPE_ANIM_PARAM = "AttackType";
	const string ATTACK_TRIGGER_ANIM_PARAM = "Attack";

	public bool isPlayer;
	[SerializeField] bool isDummy = false;
	[SerializeField] CharacterBaseConfig config;
	[SerializeField] PuppetMasterHumanoidConfig puppetConfig;
	List<CustomCharacter> otherList = new List<CustomCharacter>(1);

	private PuppetMaster puppetMaster;
	public PuppetMaster PuppetMaster => puppetMaster;

	private CustomCharacter character;
	public CustomCharacter Character => character;

	private Animator animator;
	public Animator Animator => animator;

	public PropMuscle PropMuscle { get; private set; }

	public BehaviourPuppet Behaviour { get; private set; }
	private AnimationEvent animationEvent;
	private	InputAction attackInputAction;
	public bool Standing => Behaviour != null ? Behaviour.state == BehaviourPuppet.State.Puppet : false;
	public bool Holding => weapon != null;
	private HealthBar healthBar;
	public bool Active { get; set; } = false;
	public WeaponHolder WeaponHolder { get; private set; }

	private void Update()
	{
		if (!Active || Death) return;
		UpdateAnimator();
		UpdateAutoAim();
		if(!isPlayer && !isDummy && Standing) UpdateAIMove();
	}

	readonly Vector3 attackSensorOrigin = new Vector3(0, 0.5f, 0);
	float attackTime;
	private void FixedUpdate()
	{
		if (!Active || Death) return;
		if (Time.time - attackTime > AttackPulse)
		{
			CheckAttack();
			attackTime = Time.time;
		}
	}

	Collider[] overlapOther;
	public CharacterBase DetectTarget { get; private set; } = null;
	
	void CheckAttack()
	{
		if (!Standing)
		{
			return;
		}

		overlapOther = Physics.OverlapSphere(character.transform.position + attackSensorOrigin, AttackRange, 1 << character.gameObject.layer);
		
		if (overlapOther != null)
		{
			DetectTarget = null;
			foreach (var t in overlapOther)
			{
				if(t.gameObject != character.gameObject)
				{
					DetectTarget = t.GetComponentInParent<CharacterBase>();
				}
			}

			if (DetectTarget != null) {
				Attack();
			}
		}
	}

	TriggerHelper attackSensor;
	SphereCollider attackCollider;
	public bool Initialized { get; private set; } = false;
	public void Init()
	{
		
		if (Initialized) return;
		puppetMaster = GetComponentInChildren<PuppetMaster>();
		character = GetComponentInChildren<CustomCharacter>();
		animator = character.GetComponentInChildren<Animator>();
		animationEvent = animator.GetComponent<AnimationEvent>();
		WeaponHolder = GetComponentInChildren<WeaponHolder>();
		Behaviour = GetComponentInChildren<BehaviourPuppet>();
		Behaviour.OnCollision += OnMuscleCollision;
		Behaviour.onLoseBalance.unityEvent.AddListener(OnLoseBalance);
		Behaviour.onRegainBalance.unityEvent.AddListener(OnRegainBalance);
		attackSensor = character.GetComponentInChildren<TriggerHelper>();
		if (attackSensor != null) { 
			attackSensor.target = gameObject;
			attackCollider = attackSensor.GetComponent<SphereCollider>();
		}
		
		UpdateAttackType();
		animationEvent.OnAnimationEvent = OnAnimationEvent;

		attackInputAction = (character != null && character.actions != null) ? character.actions.FindAction("Attack") : null;
		if (attackInputAction != null)
		{
			attackInputAction.started += Attack;
			attackInputAction.canceled += Attack;
		}
		attackInputAction?.Enable();

		if(transform.parent != null) transform.parent.GetComponentsInChildren(otherList);
		foreach(var c in otherList)
		{
			if(c == character)
			{
				otherList.Remove(c);
				break;
			}
		}
		healthBar = GetComponentInChildren<HealthBar>();
		var gloves = GetComponentsInChildren<Glove>();
		foreach(var glove in gloves)
		{
			if (glove != null) glove.SetColor(isPlayer ? Glove.Color.Blue : Glove.Color.Red);
		}
		if (!isPlayer) InitAIMove();
		Initialized = true;
	}

	private void OnDestroy()
	{
		if (gameObject == null || Behaviour == null || Behaviour.OnCollision == null) return;
		Behaviour.OnCollision -= OnMuscleCollision;
		Behaviour.onLoseBalance.unityEvent.RemoveListener(OnLoseBalance);
	}

	void OnMuscleCollision(MuscleCollision collision)
	{
		if (collision.collision.collider.gameObject.tag != Weapon.TAG) return;
		var weapon = collision.collision.collider.GetComponent<MeleeWeapon>();
		if(weapon != null)
		{
			weapon.OnImpulse(this, collision);
		}
	}

	void OnLoseBalance()
	{
		SoundManager.Instance.PlaySFX(SFXConst.HURT);
		animator.SetLayerWeight(1, 0);
	}

	void OnRegainBalance ()
	{
		animator.SetLayerWeight(1, 1);
	}

	void Attack(InputAction.CallbackContext context)
	{
		if (context.canceled) {
			return;
		}
		Attack();
	}

	public void OnAnimationEvent(string data)
	{
		switch (data)
		{
			case "left_punch":
				if (weapon == null && LeftHandMuscle != null) Punch(LeftHandMuscle.transform.position);
				break;
			case "right_punch":
				if(weapon == null && LeftHandMuscle != null) Punch(RightHandMuscle.transform.position);
				break;
		}
	}

	Collider[] hitColliders;
	void Punch(Vector3 center)
	{
		hitColliders = Physics.OverlapSphere(center, ImpactRadius, 1 << puppetMaster.gameObject.layer);
		PuppetMaster hitPuppet = null;
		MuscleCollisionBroadcaster muscleBoardcaster = null;
		foreach (var hitCollider in hitColliders)
		{
			muscleBoardcaster = hitCollider.GetComponent<MuscleCollisionBroadcaster>();
			if(muscleBoardcaster == null || muscleBoardcaster.puppetMaster == puppetMaster)
			{
				continue;
			}
			if(muscleBoardcaster.puppetMaster.muscles[muscleBoardcaster.muscleIndex].props.group == Muscle.Group.Head)
			{
				var b = muscleBoardcaster.puppetMaster.behaviours[0] as BehaviourPuppet;
				muscleBoardcaster.Hit(config.attackPower, (muscleBoardcaster.transform.position - center).normalized * config.attackForce, muscleBoardcaster.transform.position);
				hitPuppet = muscleBoardcaster.puppetMaster;
			}
		}

		if(hitPuppet != null)
		{
			var otherCharacter = hitPuppet.transform.GetComponentInParent<CharacterBase>();
			if(otherCharacter != null)
			{
				Instantiate(config.hitFX, center, Quaternion.identity, transform);
				
				otherCharacter.TakeDamage(Damage, DamageType.Punch);
				var dir = otherCharacter.Character.GetPosition() - center;
				otherCharacter.Character.Push(dir.normalized, PushDuration(dir));
			}
		}
	}

	float PushDuration(Vector3 dir) {
		if(isPlayer)
		{
			return Random.Range(0.3f, 0.6f);
		} else
		{
			return Random.Range(0f, 0.5f);
		}
	}

	public float Damage
	{
		get
		{
			return weapon != null ? Random.Range(weapon.minDamage, weapon.maxDamage) : Random.Range(0.02f, 0.08f);
		}
	}

	readonly string[] punchTriggerList = new string[] { "Punch1", "Punch2",};
	void Attack()
	{
		//animator.SetFloat("AttackSpeed", Random.Range(1.0f, 1.3f));

		if(weapon != null)
		{
			var atk = weapon.Attack(this);
			if(atk) animator.SetTrigger(ATTACK_TRIGGER_ANIM_PARAM);
		} else
		{
			var a = punchTriggerList[Random.Range(0, punchTriggerList.Length)];
			animator.SetTrigger(a);
			
		}
	}

	int hitCountSincePickUp = 0;
	int punchCountSinceKnockout = 0;
	public void TakeDamage(float damage, DamageType damageType)
	{
		if(damageType == DamageType.Punch)
		{
			punchCountSinceKnockout++;
			if(punchCountSinceKnockout > (isPlayer ? 4 : 3) && Standing)
			{
				if(Standing)
				{
					Behaviour.Unpin();
					KnockOut(Vector3.up * 5000);
				} else
				{
					//character.LaunchCharacter(Vector3.right * 10000);
				}

				if (weapon != null) Drop();
				punchCountSinceKnockout = 0;
				SoundManager.Instance.PlaySFX(SFXConst.PUNCH_HEAVY);
			} else
			{
				SoundManager.Instance.PlaySFX(SFXConst.PUNCH);
			}
		}
		
		
		if (isPlayer && healthBar.Value < 0.5f) damage = damage * 0.5f;
		hitCountSincePickUp++; 
		
		if (hitCountSincePickUp > 3)
		{
			hitCountSincePickUp = 0;
			
		}
		healthBar.Value -= damage;
		if (healthBar.Value <= 0) KnockOut(Vector3.up * 18000);
		if(healthBar.Value <= 0f && !Death)
		{
			Die();
		}
	}

	public bool Death => !Behaviour.canGetUp;

	void Die()
	{
		Time.timeScale = 0.5f;
		Behaviour.Unpin();
		Behaviour.canGetUp = false;
		healthBar.Value = 0;
		Gameplay.Instance.EndGame();
	}

	public void KnockOut(Vector3 force)
	{
		Behaviour.Unpin();
		var hipsIndex = puppetMaster.GetMuscleIndex(HumanBodyBones.Hips);
		var hipsMuscle = puppetMaster.muscles[hipsIndex];
		Behaviour.OnMuscleHit(new MuscleHit(
			hipsIndex,
			100,
			force,
			hipsMuscle.transform.position
			));
	}

	[SerializeField] bool dirtyFlag;
	[ContextMenu("Validate")]
	void Validate()
	{
		puppetMaster = GetComponentInChildren<PuppetMaster>();
		character = GetComponentInChildren<CustomCharacter>();
		animator = character.GetComponentInChildren<Animator>();
		if(animator != null)
			animationEvent = animator.GetComponent<AnimationEvent>();

		if(puppetMaster == null)
		{
			Debug.LogError("Not contains PuppetMaster");
			return;
		}

		if (character == null)
		{
			Debug.LogError("Not contains Character");
			return;
		}

		puppetMaster.targetRoot = character.transform;
		puppetMaster.humanoidConfig = puppetConfig;
		if (animator.GetComponent<RootMotionController>() == null)
		{
			animator.gameObject.AddComponent<RootMotionController>();
		}

		if(animationEvent == null)
		{
			animationEvent = animator.gameObject.AddComponent<AnimationEvent>();
		}
	}

	float minRange = 0;
	float cacheRange;
	public Character FocusTarget { get; private set; } = null;
	void UpdateAutoAim()
	{
		FocusTarget = null;
		if (otherList.Count > 0)
		{
			minRange = Vector3.Distance(character.transform.position, otherList[0].transform.position);
		}
		for(int i = 0; i < otherList.Count; i++)
		{
			cacheRange = Vector3.Distance(character.transform.position, otherList[i].transform.position);
			if (cacheRange <= AimRange)
			{
				if(cacheRange < minRange)
				{
					minRange = cacheRange;
				}
				
				FocusTarget = otherList[i];
			}
		}

		character.target = FocusTarget != null ? FocusTarget.transform : null;
	}

	#region Enemy
	[Header("Enemy AI")]
	[SerializeField] float destinationRange = 2f;
	[SerializeField] float randomAngleRange = 30f;
	[SerializeField] float randomDestinationPulse = 1f;
	[SerializeField] float maxWalkSpeed = 2.5f;
	SteeringSensor steerSensor;
	Vector3 moveDirection, randomDirection;
	float randomDestinationTime, randomAngle;
	const float turnFrequency = 1f, speedFrequency = 1f;

	void InitAIMove()
	{
		steerSensor = GetComponentInChildren<SteeringSensor>();
		randomDirection = Character.transform.forward;
		steerSensor.Destination = Character.transform.position + randomDirection * destinationRange;
		if(character != null)
		{
			character.maxWalkSpeed = maxWalkSpeed;
			character.actions = null;
		}
	}

	void UpdateAIMove()
	{
		if(Character.Pushing)
		{
			return;
		}
		if (character.IsSprinting()) character.StopSprinting();
		if (Character.GetMovementDirection().magnitude < 0.05f)
		{
			Character.SetMovementDirection(Character.transform.forward);
		}

		// Auto avoid and noise speed
		if (steerSensor != null)
		{
			
			if (randomDestinationTime > randomDestinationPulse)
			{
				randomAngle = Mathf.Lerp(-randomAngleRange, randomAngleRange, Mathf.PerlinNoise(Time.time * turnFrequency, 0f));
				randomDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * steerSensor.GetSteeringVector().normalized;

				if (FocusTarget != null)
				{
					//steerSensor.Destination = Character.transform.forward * destinationRange;
					var des = FocusTarget.GetPosition() + Random.onUnitSphere * AttackRange * 0.8f;
					des.y = Character.transform.position.y;
					steerSensor.Destination = des;
				} else
				{
					steerSensor.Destination = randomDirection.normalized * destinationRange;
				}
				
				randomDestinationTime = 0;
			}
			moveDirection = steerSensor.GetSteeringVector();
			moveDirection = moveDirection.normalized * Mathf.Lerp(0.6f, 1f, Mathf.PerlinNoise(Time.time * speedFrequency, 0f));
			Character.SetMovementDirection(moveDirection);
			randomDestinationTime += Time.deltaTime;
		}
	}
	#endregion

	private static readonly int Ground = Animator.StringToHash("OnGround");
	private static readonly int Crouch = Animator.StringToHash("Crouch");
	private static readonly int Jump = Animator.StringToHash("Jump");
	void UpdateAnimator()
	{
		var moveDirection = character.GetMovementDirection();
		// Compute move vector in local space
		var move = transform.InverseTransformDirection(character.GetMovementDirection());

		animator.SetFloat("Velocity", moveDirection.magnitude * 1.5f, Time.deltaTime, Time.deltaTime);
		float angle = Vector3.SignedAngle(character.transform.forward, moveDirection, Vector3.up);
		animator.SetFloat("Direction", angle / 180, Time.deltaTime, Time.deltaTime);
		animator.SetBool(Crouch, character.IsCrouching());
		animator.SetBool(Ground, character.IsOnGround() || character.WasOnGround());

		if (character.IsFalling() && !character.IsOnGround())
		{
			float verticalSpeed =
				Vector3.Dot(character.GetVelocity(), character.GetUpVector());

			animator.SetFloat(Jump, verticalSpeed, 0.3333f, Time.deltaTime);
		}
	}

	Weapon weapon;

	public void PickUp(Weapon _weapon)
	{
		if(weapon != null)
		{
			return;
		}
		hitCountSincePickUp = 0;
		weapon = _weapon;
		weapon.OnPickUp(this);

		UpdateAttackType();
	}

	public void Drop()
	{
		weapon.transform.parent = transform;
		weapon.OnDrop(this);
		weapon = null;
		UpdateAttackType();
	}

	void UpdateAttackType()
	{
		var weaponPoseThreshold = weapon != null ? (float)weapon.type / (System.Enum.GetNames(typeof(WeaponType)).Length - 1) : 0;
		animator.SetFloat(ATTACK_TYPE_ANIM_PARAM,  weaponPoseThreshold);
		if(attackSensor != null && attackCollider != null)
		{
			attackCollider.radius = AttackRange;
		}
	}

	private void Start()
	{
		UpdateConfig();
	}

	public Muscle RightHandMuscle { get; private set; } = null;
	public Muscle LeftHandMuscle { get; private set; } = null;
	void UpdateConfig()
	{
		var puppet = PuppetMaster;
		if (puppet == null)
		{
			return;
		}

		RightHandMuscle = PuppetMaster.muscles[PuppetMaster.GetMuscleIndex(HumanBodyBones.RightHand)];
		LeftHandMuscle = PuppetMaster.muscles[PuppetMaster.GetMuscleIndex(HumanBodyBones.LeftHand)];
		//if(RightHandMuscle != null)
		//{
		//	PuppetMaster.AddPropMuscle(RightHandMuscle.joint, Vector3.zero, Quaternion.identity, Vector3.zero);
		//	PropMuscle = GetComponentInChildren<PropMuscle>(true);
		//}

		foreach (var joint in config.joinConfigs)
		{
			var index = puppet.GetMuscleIndex(joint.bone);
			if (index >= 0 && index < puppet.muscles.Length)
			{
				var lowAngularXLimit = puppet.muscles[index].joint.lowAngularXLimit;
				lowAngularXLimit.limit = joint.lowAngularLimitX;
				puppet.muscles[index].joint.lowAngularXLimit = lowAngularXLimit;
				var highAngularXLimit = puppet.muscles[index].joint.highAngularXLimit;
				highAngularXLimit.limit = joint.highAngularLimitX;
				puppet.muscles[index].joint.highAngularXLimit = highAngularXLimit;
				puppet.muscles[index].joint.massScale = joint.massScale;
				puppet.muscles[index].joint.GetComponent<Rigidbody>().angularDrag = joint.angularDrag;
			}
		}
		puppet.angularLimits = config.angularLimits;
	}

	public void OnStartGame()
	{
		Active = true;
		if(isPlayer) Character.EnableInputAction();
	}

	public void OnEndGame()
	{
		Character.SetMovementMode(MovementMode.None);
		animator.SetBool("Victory", !Death);
		animator.SetFloat("Velocity", 0f);
		Active = false;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if(!isActiveAndEnabled)
		{
			return;
		}
		Validate();	
	}

	private void OnDrawGizmos()
	{
		if (character == null)
		{
			return;
		}
		Gizmos.color = DetectTarget ? Color.blue : Color.red;
		Gizmos.DrawWireSphere(character.transform.position + attackSensorOrigin, AttackRange);

		if(steerSensor != null)
		{
			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.ArrowHandleCap(
				GetInstanceID(), steerSensor.Destination,
				Quaternion.LookRotation(steerSensor.Destination - character.transform.position),
				1f,
				EventType.Repaint);
		}
	}
#endif

	float AttackRange => weapon != null ? weapon.attackRange : config.baseAttackRange;
	float ImpactRadius => weapon != null ? weapon.impactRadius : config.baseImpactRadius;
	float AimRange => weapon != null ? weapon.aimRange : config.baseAimRange;
	float AttackPulse => weapon != null ? weapon.attackPulse : config.baseAttackPulse;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer != character.gameObject.layer)
		{
			return;
		}
		if(other.gameObject != character.gameObject)
		{
			Attack();
			DetectTarget = GetComponentInParent<CharacterBase>();
			attackTime = Time.time;
		}
	}
}

[System.Serializable]
public class CharacterBaseConfig
{
	public float baseAttackRange = 1f; 
	public float baseImpactRadius = 1f;
	public float baseAimRange = 4f;
	public float baseAttackPulse = 0.5f;
	public float attackForce = 2000f;
	public float attackPower = 1f;
	public float cameraAimSpeed = 0.2f;
	public float cameraReturnSpeed = 0.1f;
	public bool angularLimits = true;
	public JoinConfig[] joinConfigs;
	public GameObject hitFX;

	[System.Serializable]
	public class JoinConfig
	{
		public HumanBodyBones bone;
		public float massScale = 1;
		public float lowAngularLimitX;
		public float highAngularLimitX;
		public float angularDrag;
	}
}

public enum DamageType
{
	Punch,
	Bullet,
	Fire,
	MeleeWeapon,
	Thrower
}
