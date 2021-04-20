using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bone
{
	public int ID = 0;
	public Transform transform = null;
	private float baseAngle = 0.0f;
	private float angle = 0.0f;
	private float angleMin = 0.0f;
	private float angleMax = 0.0f;

	private Vector3 baseRotation;

	public Bone(Transform _transform, int _id = 0, float _angleMin = -360.0f, float _angleMax = 360.0f)
	{
		ID = _id;
		transform = _transform;
		angleMin = _angleMin;
		angleMax = _angleMax;

		baseAngle = transform.localEulerAngles.z;
		baseRotation = transform.localEulerAngles;
	}

	public float getAngle()
	{
		return angle;
	}

	public void setAngle(float _angle)
	{
		angle = _angle;

		if (angle < angleMin) { angle = angleMin; }
		if (angle > angleMax) { angle = angleMax; }

		transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, baseRotation.z + angle);
		//transform.Rotate(0.0f, 0.0f, angle);
	}

	public void rotate(float _modifier)
	{
		angle += _modifier;

		if (angle < angleMin) { angle = angleMin; }
		if (angle > angleMax) { angle = angleMax; }

		transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, baseRotation.z + angle);
		//transform.Rotate(0.0f, 0.0f, angle);
	}

	public void rotateToward(float _modifier, float _targetAngle)
	{
		if (_modifier > 0 && angle + _modifier > _targetAngle)
		{
			angle = _targetAngle;
		}
		else if (_modifier < 0 && angle - _modifier < _targetAngle)
		{
			angle = _targetAngle;
		}
		else
		{
			rotate(_modifier);
		}
	}
}

public class MotionAnimator : MonoBehaviour
{
	public float frameRate = 60.0f;
    public float frame = 0.0f;
	public float returnRate = 1.0f;

    public List<string> nodesToLoad = new List<string>();

    public List<Bone> bones = new List<Bone>();
	public List<Motion> motions = new List<Motion>();

    public void Start()
    {
		// AddBone("head");
		// AddBone("body");

		// AddBone("arm_l");
		// AddBone("forearm_l");

		// AddBone("arm_r");
		// AddBone("forearm_r");

		// AddBone("leg_l");
		// AddBone("shin_l");

		// AddBone("leg_r");
		// AddBone("shin_r");

		// AddBone("bicep_l");
		// AddBone("bicep_r");
		// AddBone("thigh_l");
		// AddBone("thigh_r");

		foreach (string str in nodesToLoad)
		{
			AddBone(str);
		}
    }

	public void LateUpdate()
	{
		Animate();
	}

	public void AddBone(string _name)
	{
		Transform obj = this.transform.FindChildByRecursion(_name);

		if (obj != null) { bones.Add( new Bone(obj, _name.GetHashCode()) ); }
	}

	public Bone GetBone(string _name)
	{
		foreach (Bone bone in bones)
		{
			if (bone.ID == _name.GetHashCode())
			{
				return bone;
			}
		}

		return null;
	}

	public Motion GetMotion(string _name)
	{
		foreach (Motion motion in motions)
		{
			if (motion.name == _name)
			{
				return motion;
			}
		}

		return null;
	}

	public void PlayMotion(string _name)
	{
		foreach (Motion motion in motions)
		{
			if (motion.name == _name)
			{
				motion.Play();
			}
		}
	}

	public void Animate()
	{
		//	Apply motion to each bone
		foreach (Bone bone in bones)
		{
			int modifiers = 0;
			float motionAmount = 0.0f;
			float motionBase = 0.0f;

			foreach (Motion motion in motions)
			{
				if (motion.getBoneID() == bone.ID && motion.isPlaying() == true)
				{
					modifiers += 1;

					float frameTime = (frame % motion.frameCount) / motion.frameCount;

					if (motion.isBaseMotion() == true)
					{
						motionBase = motion.getValueAtTime(frameTime);
					}
					else
					{
						motionAmount += motion.getValueAtTime(frameTime);
					}
				}
			}

			motionAmount += motionBase;

			if (modifiers > 0)
			{
				motionAmount += (motionAmount / modifiers);	//	Average the modifiers together
			}

			if (motionAmount == 0.0f)
			{
				if (bone.getAngle() > 0) { bone.rotateToward(-frameRate * Time.deltaTime * returnRate, 0); }
				else if (bone.getAngle() < 0) { bone.rotateToward(frameRate * Time.deltaTime * returnRate, 0); }
			}
			else
			{
				bone.setAngle(motionAmount);
			}
		}

		frame += frameRate * Time.deltaTime;
	}
}