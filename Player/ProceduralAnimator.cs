using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    public List<BoneNode> nodes = new List<BoneNode>();

    public float frameRate = 60.0f;
    public float frame = 0.0f;
	public int lastFrame = 0;

    private List<AnimationDefinition> animations = new List<AnimationDefinition>();

    public AnimationDefinition animTest;

    public void Start()
    {
		AddBone("head");
		AddBone("body");

		AddBone("arm_l");
		AddBone("forearm_l");

		AddBone("arm_r");
		AddBone("forearm_r");

		AddBone("leg_l");
		AddBone("shin_l");

		AddBone("leg_r");
		AddBone("shin_r");

        PlayAnimation(animTest);
    }

	public void LateUpdate()
	{
		Animate();
	}

	public void AddBone(string _name)
	{
		nodes.Add( new BoneNode(this.transform.FindChildByRecursion(_name), _name.GetHashCode()) );
	}

    public void PlayAnimation(AnimationDefinition _anim)
    {
        if (animations.Contains(_anim) == false)
        {
            animations.Add(_anim);
        }
    }

    public void OverridePlayAnimation(AnimationDefinition _anim)
    {
        animations.Clear();
        animations.Add(_anim);
    }

    public void StopAnimation(AnimationDefinition _anim)
    {
        animations.Remove(_anim);
    }

	public void Animate()
	{
        //	Every animation...
        foreach (AnimationDefinition animation in animations)
        {
            //	Every bone in the animation...
            foreach (AnimationEntry animationNode in animation.bones)
            {
                //	Every bone in the controller...
                foreach (BoneNode bone in nodes)
                {
                    if (bone.ID == animationNode.boneName.GetHashCode())
                    {
                        float frameTime = (frame % animation.frameCount) / animation.frameCount;
                        bone.setAngle( animationNode.getAngleAtTime(frameTime) );
                    }
                }
            }
        }

        lastFrame = (int)frame;

        frame += 1 * frameRate * Time.deltaTime;
	}
}

[System.Serializable]
public class AnimationDefinition
{
    public int frameCount = 60;

    public AnimationEntry[] bones;
}

[System.Serializable]
public class AnimationEntry
{
    public int boneID = 0;
    public string boneName = "";

    public AnimationCurve curve;

    public float angleMin = 0.0f;
    public float angleMax = 45.0f;

    public float getAngleAtTime(float _frameTime)
    {
        if (curve != null)
        {
            return curve.Evaluate(_frameTime) * (angleMax - angleMin) + angleMin;
        }
        else
        {
            return Mathf.Lerp(angleMin, angleMax, _frameTime);
        }
    }
}