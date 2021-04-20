using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController3D : MonoBehaviour
{
	public Transform target;

	[Header("Settings")]
	public Vector3 movementOffset 	= new Vector3(0, 0, 0);
	public float movementSmoothing 	= 0.0f;

	public Vector3 rotationOffset 	= new Vector3(0, 0, 0);
	public float rotationSmoothing 	= 0.0f;

	[Space(10)]
	public float distance 			= 10.0f;
	public float distanceSmoothing 	= 0.0f;

	private Vector3 cameraVelocity = Vector3.zero;

	private bool following = true;
	private float currentDistance 	= 0.0f;
	private float distanceVelocity 	= 0.0f;

	public void toggleFollow()
	{
		setFollow( !isFollowing() );
	}

	public void setFollow(bool _follow)
	{
		following = _follow;

		//	Reset velocities anytime following is changed
		cameraVelocity = Vector3.zero;
		distanceVelocity = 0.0f;
	}

	public bool isFollowing()
	{
		return following;
	}

	private void Update()
	{
		if (isFollowing() == true)
		{
			if (Input.GetKey(KeyCode.Equals) == true)
			{
				distance += 10 * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.Minus) == true)
			{
				distance -= 10 * Time.deltaTime;
			}
		}
	}

	private void FixedUpdate()
	{
		if (target != null && isFollowing())
		{
			currentDistance = Mathf.SmoothDamp(currentDistance, distance, ref distanceVelocity, distanceSmoothing);

			if (Mathf.Abs(currentDistance - distance) <= 0.01f) //	Approximate to the distance
			{
				currentDistance = distance;
			}

			Vector3 targetPos = target.transform.position +
								(target.transform.right * movementOffset.x) +
								(target.transform.up * movementOffset.y) +
								(target.transform.forward * (movementOffset.z - currentDistance));

			// this.transform.position = targetPos;
			// this.transform.rotation = target.transform.rotation * Quaternion.Euler(rotationOffset);

			this.transform.position = Vector3.LerpUnclamped(this.transform.position, targetPos, movementSmoothing);
			// this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref cameraVelocity, movementSmoothing);

			Quaternion targetRot = target.transform.rotation * Quaternion.Euler(rotationOffset);
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, Time.fixedDeltaTime * rotationSmoothing);
		}
    }
}
