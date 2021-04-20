using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swordfish;

[RequireComponent(typeof(CharacterController))]
public class EntityMotor : Entity
{
	//	Variables
	[Header("Movement Variables")]

	[SerializeField]
	protected 	Vector3 	velocity 		= Vector3.zero;
	[SerializeField]
	protected 	Vector3 	torque 			= Vector3.zero;
	[SerializeField]
	protected 	Vector3 	movementVector 	= Vector3.zero;
	[SerializeField]
	protected 	Vector3 	rotationVector 	= Vector3.zero;

	//	Settings
	[Header("Movement Settings")]
	public 	float 		friction 				= 1.0f;
	public 	float 		acceleration			= 1.0f;
	public 	float 		minVelocity				= 0.5f;
	public 	float 		drag 					= 1.0f;
	public 	float 		angularAcceleration		= 1.0f;
	public 	float 		minTorque				= 0.5f;

	//	Getters
	public Vector3 	getVelocity() 			{ return velocity; }
	public Vector3 	getMovementVector() 	{ return movementVector; }
	public Vector3 	getRotationVector() 	{ return rotationVector; }

	//	Setters
	public void 	setVelocity(Vector3 c) 			{ velocity = c; }
	public void 	setMovementVector(Vector3 c) 	{ movementVector = c; }
	public void 	setRotationVector(Vector3 c) 	{ rotationVector = c; }

	//	Checks
	public bool 	isMoving() 		{ return (velocity.magnitude > minVelocity); }
	public bool 	isRotating() 	{ return (torque.magnitude > minTorque); }

	//	Overridable methods
	protected override void OnLateUpdate()
	{
		torque -= drag * Time.deltaTime * torque;
		torque += rotationVector * angularAcceleration * Time.deltaTime;

		if (isRotating() )
		{
			transform.Rotate(torque * Time.deltaTime);
		}

		velocity -= friction * Time.deltaTime * velocity;
		velocity += movementVector * acceleration * Time.deltaTime;

		if ( isMoving() )
		{
			controller.Move(velocity * Time.deltaTime);
		}
	}
}