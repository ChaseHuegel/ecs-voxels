using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

	[Header("Settings")]
	public Vector3  cameraOffset 	    = new Vector3(0.0f, 0.0f, -10.0f);
	public float    cameraSmoothing 	= 1.0f;

    [Space(10)]
	public Vector3  rotationOffset 	    = new Vector3(0.0f, 0.0f, 0.0f);
	public float    rotationSmoothing 	= 1.0f;

	[Space(10)]
	public float fov            = 60.0f;
	public float fovSmoothing 	= 1.0f;

	private Vector3 cameraVelocity  = Vector3.zero;
	private float   fovVelocity     = 0.0f;

    private void Start()
    {
        Camera.main.fieldOfView = 1.0f;
    }

    private void LateUpdate()
    {
		if (target != null)
		{
            Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, fov, ref fovVelocity, fovSmoothing);
            if (Mathf.Abs(Camera.main.fieldOfView - fov) <= 0.01f) { Camera.main.orthographicSize = fov; }	//	Snap to FOV

            Vector3 targetPos = target.transform.position + (target.transform.rotation * cameraOffset);
            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref cameraVelocity, cameraSmoothing);

            Quaternion targetRot = target.transform.rotation;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRot, Time.deltaTime * rotationSmoothing);
        }
    }
}
