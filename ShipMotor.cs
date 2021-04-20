using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Physics.Extensions;
using Swordfish;

public class ShipMotor : MonoBehaviour
{
    public float[] thrust;
    public float[] rotation;

    public float pitch = 10.0f;
    public float yaw = 10.0f;
    public float roll = 10.0f;

    public float throttle = 0.0f;

    public VoxelComponent component;

    public void Start()
    {
        component = GetComponent<VoxelComponent>();

        thrust = new float[6];
        rotation = new float[3];

        Initialize();
    }

    public virtual void Initialize()
    {

    }

    public float getThrust(Direction _direction)
    {
        return thrust[(int)_direction];
    }

    public void EngageThrust(Direction _direction)
    {
        float power = 0.0f;

        List<BehaviorBlock> behaviorBlocks = component.voxelObject.getBehaviorBlocks();
        for (int i = 0; i < behaviorBlocks.Count; i++)
        {
            Block thisBlock = behaviorBlocks[i];

            if (thisBlock.getBlockData() is Thruster)
            {
                Quaternion rot = new Quaternion();
                rot.eulerAngles = ((Thruster)thisBlock.getBlockData()).getRotation();
                Vector3 vector = rot * Vector3.forward;
            }

            if (thisBlock.getBlockData() is Thruster && ((Thruster)thisBlock.getBlockData()).getFacing() == _direction)
            {
                power += ((Thruster)thisBlock.getBlockData()).getPower();
            }
        }

        thrust[(int)_direction] = power;
    }

    public void DisengageThrust(Direction _direction)
    {
        thrust[(int)_direction] = 0.0f;
    }

    public void EngagePitch(float _factor)
    {
        rotation[0] = pitch * _factor;
    }

    public void EngageYaw(float _factor)
    {
        rotation[1] = yaw * _factor;
    }

    public void EngageRoll(float _factor)
    {
        rotation[2] = roll * _factor;
    }

    public void DisengagePitch()
    {
        rotation[0] = 0.0f;
    }

    public void DisengageYaw()
    {
        rotation[1] = 0.0f;
    }

    public void DisengageRoll()
    {
        rotation[2] = 0.0f;
    }

    public void FixedUpdate()
    {
        Vector3 force = Vector3.zero;
        Vector3 torque = new Vector3(rotation[0], rotation[1], rotation[2]);

        for (int i = 0; i < thrust.Length; i++)
        {
            if (thrust[i] > 0)
            {
                force = force + ( ((Direction)i).toVector3() * thrust[i] );
            }
        }

        if (force != Vector3.zero)
        {
            if (throttle < 1.0f) { throttle += 0.25f * Time.fixedDeltaTime; }
            else { throttle = 1.0f; }

            component.entity.ApplyLinearImpulse( component.transform.rotation * (throttle * force) );
        }
        else
        {
            if (throttle > 0.0f) { throttle -= 0.25f * Time.fixedDeltaTime; }
            else { throttle = 0.0f; }
        }

        if (torque != Vector3.zero)
        {
            component.entity.ApplyAngularImpulse(torque);
        }

        Vector3 velocity = component.entity.GetLinearVelocity();
        if (velocity.magnitude > 300)
		{
			velocity = Vector3.ClampMagnitude(velocity, 300);
            component.entity.SetLinearVelocity(velocity);
		}

        velocity = component.entity.GetAngularVelocity();
        if (velocity.magnitude > 100)
		{
			velocity = Vector3.ClampMagnitude(velocity, 100);
            component.entity.SetAngularVelocity(velocity);
		}
    }
}
