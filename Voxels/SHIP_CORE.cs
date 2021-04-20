using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class SHIP_CORE : BehaviorBlock
    {
		public Light[] lights = new Light[6];
		public bool hasLights;

		public override Voxel getType()
		{
			return Voxel.SHIP_CORE;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(1, 1);
		}

		public override void Update()
		{
			UpdateLights();
		}

		public override void Initialize()
		{
			UpdateLights();
		}

		public override void OnRemove()
		{
			DestroyLights();
		}

		public override void Tick()
		{
			for (int i = 0; i < 6; i++)
			{
				if (lights[i] != null)
				{
					lights[i].intensity = 1.2f + Mathf.PingPong(Time.time * 0.5f, 0.5f);
				}
			}
		}

		public override void Interact()
		{

		}

		//	Methods
		public void UpdateLights()
		{
			for (int i = 0; i < 6; i++)
			{
				if (hasLight(i) && isFaceBlocked(i)) { DestroyLight(i); }
				else if (hasLight(i) == false && isFaceBlocked(i) == false) { CreateLight(i); }
			}
		}

		public bool hasLight(Direction _face)
		{
			return hasLight((int)_face);
		}

		public bool hasLight(int _face)
		{
			return ( lights[_face] != null );
		}

		public void PlaceLight(Vector3 _position, int _index)
		{
			Light light = (new GameObject("core_light_" + _index)).AddComponent<Light>();
			light.transform.parent = getChunk().getVoxelObject().component.transform;
			light.transform.localPosition = _position + getChunk().getVoxelObject().component.pivotPoint;
			light.color = Color.yellow * Color.white / 2; light.range = 1.0f;
			lights[_index] = light;
		}

		public void CreateLight(Direction _face)
		{
			CreateLight((int)_face);
		}

		public void CreateLight(int _face)
		{
			switch (_face)
			{
			case 0:
				PlaceLight( new Vector3(getWorldX() + 0.5f, getWorldY() + 0.5f, getWorldZ() + 1.3f), _face );
				break;
			case 1:
				PlaceLight( new Vector3(getWorldX() + 1.3f, getWorldY() + 0.5f, getWorldZ() + 0.5f), _face );
				break;
			case 2:
				PlaceLight( new Vector3(getWorldX() + 0.5f, getWorldY() + 0.5f, getWorldZ() - 0.3f), _face );
				break;
			case 3:
				PlaceLight( new Vector3(getWorldX() - 0.3f, getWorldY() + 0.5f, getWorldZ() + 0.5f), _face );
				break;
			case 4:
				PlaceLight( new Vector3(getWorldX() + 0.5f, getWorldY() + 1.3f, getWorldZ() + 0.5f), _face );
				break;
			case 5:
				PlaceLight( new Vector3(getWorldX() + 0.5f, getWorldY() - 0.3f, getWorldZ() + 0.5f), _face );
				break;
			}
		}

		public void DestroyLight(Direction _face)
		{
			DestroyLight((int)_face);
		}

		public void DestroyLight(int _face)
		{
			if (lights[_face] != null)
			{
				GameObject.Destroy(lights[_face].gameObject);
				lights[_face] = null;
			}
		}

		public void DestroyLights()
		{
			for (int i = 0; i < 6; i++)
			{
				if (lights[i] != null)
				{
					GameObject.Destroy(lights[i].gameObject);
					lights[i] = null;
				}
			}
		}
	}
}