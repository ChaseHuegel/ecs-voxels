using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class LIGHT_BULB : BehaviorBlock
    {
		public Light light;

		public bool flicker = false;

		public LIGHT_BULB()
		{
			blockData = new Orientated();
		}

		public override Voxel getType()
		{
			return Voxel.LIGHT_BULB;
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
			return GameMaster.Instance.getCachedModel("light_panel");
		}

		public override Position getOffset()
		{
			return new Position(0.5f, 0.5f, 0.5f);
		}

		public override Position getViewOffset()
		{
			return new Position(0.5f, -0.5f, 0.0f);
		}

		public override void Update()
		{
			UpdateLight();
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

		// public override void Tick()
		// {
		// 	if (light != null && getRandom(getChunk().getVoxelObject().ticksAlive).Next(100) <= 1)
		// 	{
		// 		flicker = true;
		// 	}

		// 	if (flicker == true)
		// 	{
		// 		light.intensity = 100.0f;
		// 		flicker = false;
		// 	}
		// 	else
		// 	{
		// 		light.intensity = 20.0f;
		// 	}
		// }

		public override void Interact()
		{

		}

		//	Methods
		public void UpdateLight()
		{
			if (hasLight() == true && isBlocked() == true) { DestroyLight(); }
			else if (hasLight() == false && isBlocked() == false) { CreateLight(); }
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

		public void PlaceLight(Vector3 _position)
		{
			Vector3 rotation = Vector3.zero;

			switch ( ((Rotatable)getBlockData()).getFacing() )
			{
			case Direction.NORTH:
				break;
			case Direction.EAST:
				rotation = new Vector3(0, 90, 0);
				break;
			case Direction.SOUTH:
				rotation = new Vector3(0, 180, 0);
				break;
			case Direction.WEST:
				rotation = new Vector3(0, 270, 0);
				break;
			case Direction.ABOVE:
				rotation = new Vector3(-90, 0, 0);
				break;
			case Direction.BELOW:
				rotation = new Vector3(90, 0, 0);
				break;
			}

			switch ( ((Orientated)getBlockData()).getOrientation() )
			{
			case Direction.NORTH:
				rotation = new Vector3(rotation.x, rotation.y, rotation.z + 90);
				break;
			case Direction.EAST:
				rotation = new Vector3(rotation.x, rotation.y + 90, rotation.z + 90);
				break;
			case Direction.SOUTH:
				rotation = new Vector3(rotation.x, rotation.y + 180, rotation.z - 90);
				break;
			case Direction.WEST:
				rotation = new Vector3(rotation.x, rotation.y - 90, rotation.z - 90);
				break;
			case Direction.ABOVE:
				rotation = new Vector3(rotation.x - 90, rotation.y, rotation.z + 90);
				break;
			case Direction.BELOW:
				rotation = new Vector3(rotation.x + 90, rotation.y + 180, rotation.z - 90);
				break;
			}

			Vector3 center = new Vector3(0.5f, 0.5f, 0.5f);
			Vector3 origin = new Vector3(getWorldX(), getWorldY(), getWorldZ()) + center;

			light = (new GameObject("panel_light")).AddComponent<Light>();
			light.transform.parent = getChunk().getVoxelObject().component.transform;
			light.transform.localPosition = (Quaternion.Euler(rotation) * (_position - center) + center) + origin - center + getChunk().getVoxelObject().component.pivotPoint;
			light.color = new Color(0.6f, 0.6f, 0.6f); light.range = 10.0f; light.intensity = 15.0f;
			light.shadows = LightShadows.Hard;
		}
	}
}