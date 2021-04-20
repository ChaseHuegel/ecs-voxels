using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swordfish;
using Swordfish.items;

public class PlayerMotor : EntityMotor
{
	//	Overridable methods
	protected override void OnInitialize()
	{
		GameMaster.Instance.player = this;

		Spawn();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		//	Handle movement if the player is alive & spawned
		if (isValid() == true)
		{
			movementVector = Vector3.zero;
			if (InputManager.Get("Forward") == true) { movementVector.z = 1.0f; }
			else if (InputManager.Get("Backward") == true) { movementVector.z = -1.0f; }
			else { movementVector.z = 0.0f; }

			if (InputManager.Get("Right") == true) { movementVector.x = 1.0f; }
			else if (InputManager.Get("Left") == true) { movementVector.x = -1.0f; }
			else { movementVector.x = 0.0f; }

			if (InputManager.Get("Up") == true) { movementVector.y = 1.0f; }
			else if (InputManager.Get("Down") == true) { movementVector.y = -1.0f; }
			else { movementVector.y = 0.0f; }

			if (InputManager.Get("Roll Right") == true) { rotationVector.z = 1.0f; }
			else if (InputManager.Get("Roll Left") == true) { rotationVector.z = -1.0f; }
			else { rotationVector.z = 0.0f; }
		}
	}
}