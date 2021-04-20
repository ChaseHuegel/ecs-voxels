using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class CONSOLE : BehaviorBlock
    {
		public Light light;

		public CONSOLE()
		{
			blockData = new Rotatable();
		}

		public override Voxel getType()
		{
			return Voxel.CONSOLE;
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
			return GameMaster.Instance.getCachedModel("console");
		}

		public override Position getOffset()
		{
			return new Position(0.5f, 0.5f, 0.5f);
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

		public override void Tick()
		{
			if (light != null)
			{
				light.intensity = 1.0f + Mathf.PingPong(Time.time * 0.25f, 0.5f);
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
		}

		public bool hasLight()
		{
			return ( light != null );
		}

		public void CreateLight()
		{
			float yMod = 1.0f;
			if ( ((Flippable)blockData).isFlipped() ) { yMod = 0.0f; }

			switch ( ((Rotatable)blockData).getFacing() )
			{
			case Direction.NORTH:
				PlaceLight( new Vector3(getWorldX() + 0.5f, getWorldY() + yMod, getWorldZ() + 0.2f) );
				break;
			case Direction.EAST:
				PlaceLight( new Vector3(getWorldX() + 0.2f, getWorldY() + yMod, getWorldZ() + 0.5f) );
				break;
			case Direction.SOUTH:
				PlaceLight( new Vector3(getWorldX() + 0.5f, getWorldY() + yMod, getWorldZ() + 0.8f) );
				break;
			case Direction.WEST:
				PlaceLight( new Vector3(getWorldX() + 0.8f, getWorldY() + yMod, getWorldZ() + 0.5f) );
				break;
			}
		}

		public void DestroyLight()
		{
			if (light != null) { GameObject.Destroy(light.gameObject); }
		}

		public void PlaceLight(Vector3 _position)
		{
			light = (new GameObject("console_light")).AddComponent<Light>();
			light.transform.parent = getChunk().getVoxelObject().component.transform;
			light.transform.localPosition = _position + getChunk().getVoxelObject().component.pivotPoint;
			light.color = new Color(0.44f, 0.65f, 1.0f); light.range = 1.0f;
		}
	}
}