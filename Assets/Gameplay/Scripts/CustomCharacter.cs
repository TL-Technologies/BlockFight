using ECM2.Characters;
using ECM2.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCharacter : AgentCharacter
{
	public Transform target;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	public override void MoveToLocation(Vector3 location)
	{
		if (!agent.isOnNavMesh)
		{
			return;
		}

		Vector3 toLocation = (location - GetPosition()).projectedOnPlane(GetUpVector());
		if (toLocation.sqrMagnitude >= MathLib.Square(stoppingDistance))
			agent.SetDestination(location);
	}

    protected override void UpdateRotation()
    {
        // If not locking target use default rotation

        if (target == null)
            base.UpdateRotation();
        else
        {
            // Is Character is disabled, return

            if (IsDisabled())
                return;

            // Should update Character's rotation ?

            RotationMode rotationMode = GetRotationMode();

            if (rotationMode == RotationMode.None)
                return;

            // Look at target

            Vector3 toTarget = target.position - GetPosition();

            RotateTowards(toTarget);
        }
    }

	protected override void HandleInput()
	{
        if(Pushing)
		{
            SetMovementDirection(pushDirection);
            return;
        }

        if (IsPathFollowing())
        {
            return;
        }

        if (actions == null)
            return;

        // Poll movement InputAction

        var movementInput = GetMovementInput();
        //Debug.LogError(movementInput);

        if (cameraTransform == null)
        {
            // If Camera is not assigned, add movement input relative to world axis

            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * movementInput.x;
            movementDirection += Vector3.forward * movementInput.y;

            SetMovementDirection(movementDirection);
        }
        else
        {
            // If Camera is assigned, add input movement relative to camera look direction

            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * movementInput.x;
            movementDirection += Vector3.forward * movementInput.y;

            movementDirection = movementDirection.relativeTo(cameraTransform);

            SetMovementDirection(movementDirection);
        }
    }

	public override void StopMovement()
	{
        if(agent.isOnNavMesh) agent.ResetPath();

        SetMovementDirection(Vector3.zero);
    }

    public void EnableInputAction()
	{
        movementInputAction?.Enable();
	}

    float pushTime, pushDuration;
    Vector3 pushDirection;
    public void Push(Vector3 direction, float duration)
    {
        if (duration <= 0) return;
        Sprint();
        pushDirection = direction;
        SetMovementDirection(direction);
        pushTime = Time.realtimeSinceStartup;
        pushDuration = duration;
    }

    public bool Pushing => Time.realtimeSinceStartup - pushTime < pushDuration;
}
