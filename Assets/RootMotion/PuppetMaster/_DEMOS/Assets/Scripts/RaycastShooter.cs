﻿using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;
using UnityEngine.InputSystem;

namespace RootMotion.Demos {

	public class RaycastShooter : MonoBehaviour {

		public LayerMask layers;
		public float unpin = 10f;
		public float force = 10f;
		public ParticleSystem blood;

        // Update is called once per frame
        void Update () {
			if (Mouse.current.leftButton.wasPressedThisFrame) {
				Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

				// Raycast to find a ragdoll collider
				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(ray, out hit, 100f, layers)) {
					var broadcaster = hit.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

					if (broadcaster != null) {
						broadcaster.Hit(unpin, ray.direction * force, hit.point);
                        

						blood.transform.position = hit.point;
						blood.transform.rotation = Quaternion.LookRotation(-ray.direction);
						blood.Emit(5);
					}
				}
			}
		}
	}
}
