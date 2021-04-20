using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class TransformExtensions
{
    internal static Transform FindChildByRecursion(this Transform aParent, string aName)
    {
        if (aParent == null) return null;
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindChildByRecursion(aName);
            if (result != null)
                return result;
        }
        return null;
    }
}

[System.Serializable]
public class FrameData
{
	public int frame = 0;
	public float angle = 0.0f;
}

[System.Serializable]
public class NodeAnimData
{
	public AnimationCurve curve;
	public FrameData[] keyframes;
	public int nodeID = 0;

	public FrameData getDataAtFrame(int _frame)
	{
		int thisFrame = _frame;
		int leastFrame = 0;
		int mostFrame = 9999;

		FrameData prevKeyFrame = null;
		FrameData nextKeyFrame = null;
		FrameData currentFrame = null;

		for (int i = 0; i < keyframes.Length; i++)
		{
			if (keyframes[i].frame == thisFrame) { currentFrame = keyframes[i]; break; }
			else if (keyframes[i].frame > leastFrame && keyframes[i].frame < thisFrame) { prevKeyFrame = keyframes[i]; leastFrame = keyframes[i].frame; }
			else if (keyframes[i].frame < mostFrame && keyframes[i].frame > thisFrame) { nextKeyFrame = keyframes[i]; mostFrame = keyframes[i].frame; }
		}

		if (prevKeyFrame == null) { currentFrame = nextKeyFrame; }
		else if (nextKeyFrame == null) { currentFrame = prevKeyFrame; }

		if (currentFrame == null && prevKeyFrame != null && nextKeyFrame != null)
		{
			currentFrame = new FrameData();

			int totalFrames = mostFrame - leastFrame;

			//	TODO: use a curve for interpolation
			float x = thisFrame - prevKeyFrame.frame;
			x = x / totalFrames;

			if (curve != null)
			{
				currentFrame.angle = curve.Evaluate(x) * (nextKeyFrame.angle - prevKeyFrame.angle) + prevKeyFrame.angle;
			}
			else
			{
				currentFrame.angle = Mathf.Lerp(prevKeyFrame.angle, nextKeyFrame.angle, x);
			}

			UnityEngine.Debug.Log(thisFrame + "/" + totalFrames + " - " + currentFrame.angle + " - " + prevKeyFrame.angle + " - " + nextKeyFrame.angle);
		}
		else
		{
			currentFrame = keyframes[0];
		}

		return currentFrame;
	}
}

[System.Serializable]
public class BoneNode
{
	public int ID = 0;
	public Transform transform = null;
	private float baseAngle = 0.0f;
	private float angle = 0.0f;
	private float angleMin = -180.0f;
	private float angleMax = 180.0f;

	public BoneNode(Transform _transform, int _id = 0, float _angleMin = -180.0f, float _angleMax = 180.0f)
	{
		ID = _id;
		transform = _transform;
		angleMin = _angleMin;
		angleMax = _angleMax;

		baseAngle = transform.localEulerAngles.z;
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

		transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, baseAngle);
		transform.Rotate(0.0f, 0.0f, angle);
	}

	public void rotate(float _modifier)
	{
		angle += _modifier;

		if (angle < angleMin) { angle = angleMin; }
		if (angle > angleMax) { angle = angleMax; }

		transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, baseAngle);
		transform.Rotate(0.0f, 0.0f, angle);
	}
}

[System.Serializable]
public class Animation
{
	public List<NodeAnimData> nodes;
	public int length = 1;
	public float speed = 1.0f;

	private float frameTime = 0.0f;
	private float frame = 1;
	private bool paused = false;

	public Animation(List<NodeAnimData> _nodes = null, int _length = 1, float _speed = 1.0f)
	{
		if (_nodes == null) { _nodes = new List<NodeAnimData>(); }
		nodes = _nodes;
		length = _length;
		speed = _speed;
	}

	public bool isPaused()
	{
		return paused;
	}

	public void pause()
	{
		paused = true;
	}

	public void unpause()
	{
		paused = false;
	}

	public float getCurrentFrame()
	{
		return frame;
	}

	public void Tick()
	{
		// frameTime += Time.deltaTime;
		// int frameIncrement = Mathf.FloorToInt(frameTime);

		// frameTime -= frameIncrement;
		// frame += frameIncrement;
		frame += 1 * speed;

		if (frame > length)
		{
			frame = 1;
		}
	}
}

public class ProceduralAnimController : MonoBehaviour
{
	public List<BoneNode> nodes = new List<BoneNode>();

	public Animation animation;
	public int lastFrame = 0;

	public void LateUpdate()
	{
		Animate();
	}

	public void AddBone(string _name)
	{
		nodes.Add( new BoneNode(this.transform.FindChildByRecursion(_name), _name.GetHashCode()) );
	}

	public void Animate()
	{
		if (animation != null && animation.isPaused() == false)
		{
			if (animation.getCurrentFrame() != lastFrame)
			{
				//	Every node in the animation...
				foreach (NodeAnimData animData in animation.nodes)
				{
					//	Every node in the controller...
					foreach (BoneNode controlNode in nodes)
					{
						//	Check for a matching node
						if (controlNode.ID == animData.nodeID)
						{
							//	Animate the matching node
							controlNode.setAngle(
								animData.getDataAtFrame((int)animation.getCurrentFrame()).angle
							);
						}
					}
				}
			}

			lastFrame = (int)animation.getCurrentFrame();
			//	Tick the animation
			animation.Tick();
		}
	}
}