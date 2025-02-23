using UnityEngine;

public class WeaponPickUpTrigger : MonoBehaviour {

		public RootMotion.Dynamics.PuppetMasterProp prop;
		public LayerMask characterLayers;


		void OnTriggerEnter(Collider collider) {
			if (prop.isPickedUp) return;
			if (!RootMotion.LayerMaskExtensions.Contains(characterLayers, collider.gameObject.layer)) return;

			var characterPuppet = collider.GetComponentInParent<CharacterBase>();
			if (characterPuppet == null) return;

			if (!characterPuppet.Standing) return;

		if (characterPuppet.PropMuscle == null) return;
		if (characterPuppet.PropMuscle.currentProp != null) return;

		characterPuppet.PropMuscle.currentProp = prop;
	}
	}
