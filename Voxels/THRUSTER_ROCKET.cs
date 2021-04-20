using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class THRUSTER_ROCKET : BehaviorBlock
    {
		public Light light;
		public ShipMotor motor;

		public THRUSTER_ROCKET()
		{
			blockData = new RocketThruster();
		}

		public override Voxel getType()
		{
			return Voxel.THRUSTER_ROCKET;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override ModelType getModelType()
		{
			return ModelType.CUSTOM;
		}

		public override CachedModel getModelData()
		{
			return GameMaster.Instance.getCachedModel("thruster_rocket");
		}

		public override Position getOffset()
		{
			return new Position(0.5f, 0.5f, 0.5f);
		}

		public override void Update()
		{
			UpdateLight();

			motor = getChunk().getVoxelObject().component.GetComponent<ShipMotor>();
		}

		public override void BlockDataUpdate()
		{
			UpdateLight();
		}

		public override void Initialize()
		{
			UpdateLight();
		}

		public override void OnRemove()
		{
			DestroyLight();
		}

		public override void Tick()
		{
			if (light != null)
			{
				if (motor != null && motor.enabled == true)
				{
					Direction dir = ((Thruster)blockData).getFacing();
					if (light.intensity < 30.0f && motor.getThrust(dir) > 0)
					{
						light.intensity = Mathf.Clamp(light.intensity + 5.0f, 1.5f, 30.0f);
					}
					else if (light.intensity > 6.5f)
					{
						light.intensity = Mathf.Clamp(light.intensity - 5.0f, 1.5f, 30.0f);
					}
				}
				else
				{
					light.intensity = 1.5f;
				}

				light.intensity += Mathf.PingPong(Time.time * 1.5f, 1.0f);
			}
		}

		public override void Interact()
		{

		}

		//	Methods
		public void UpdateLight()
		{
			if (hasLight() && isCulled()) { DestroyLight(); }
			else if (hasLight() == false && isCulled() == false) { CreateLight(); }
			else if (hasLight() == true)
			{
				ReplaceLight( new Vector3(0.5f, 0.5f, 0.2f) );
			}
		}

		public bool hasLight()
		{
			return ( light != null );
		}

		public void CreateLight()
		{
			PlaceLight( new Vector3(0.5f, 0.5f, 0.2f) );
		}

		public void DestroyLight()
		{
			if (light != null) { GameObject.Destroy(light.gameObject); }
		}

		public void ReplaceLight(Vector3 _position)
		{
			Vector3 rotation = ((Thruster)getBlockData()).getRotation();

			Vector3 center = new Vector3(0.5f, 0.5f, 0.5f);
			Vector3 origin = new Vector3(getWorldX(), getWorldY(), getWorldZ()) + center;

			light.transform.parent = getChunk().getVoxelObject().component.transform;
			light.transform.localPosition = (Quaternion.Euler(rotation) * (_position - center) + center) + origin - center + getChunk().getVoxelObject().component.pivotPoint;
		}

		public void PlaceLight(Vector3 _position)
		{
			Vector3 rotation = ((Thruster)getBlockData()).getRotation();

			Vector3 center = new Vector3(0.5f, 0.5f, 0.5f);
			Vector3 origin = new Vector3(getWorldX(), getWorldY(), getWorldZ()) + center;

			light = (new GameObject("thruster_light")).AddComponent<Light>();
			light.transform.parent = getChunk().getVoxelObject().component.transform;
			light.transform.localPosition = (Quaternion.Euler(rotation) * (_position - center) + center) + origin - center + getChunk().getVoxelObject().component.pivotPoint;
			light.color = Color.red + Color.yellow / 1.5f; light.range = 1.0f;
		}
	}
}