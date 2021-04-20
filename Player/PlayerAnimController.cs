using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : ProceduralAnimController
{
    public int length = 60;
    public float speed = 1.0f;
	public List<NodeAnimData> animationData = new List<NodeAnimData>();

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

		animation = new Animation(animationData, length, speed);
    }
}