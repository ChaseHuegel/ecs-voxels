using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class LIGHT_BLOCK : BehaviorBlock
    {
		public bool[,] neighborMatch;

		public Light[] lights = new Light[6];
		public bool hasLights;

		public override Voxel getType()
		{
			return Voxel.LIGHT_BLOCK;
		}

		public override void OnPreRender()
		{
			Block[] neighborBlocks = getNeighbors();

			neighborMatch = new bool[neighborBlocks.Length, neighborBlocks.Length];
			for (int n = 0; n < neighborBlocks.Length; n++)
			{
				for (int i = 0; i < neighborBlocks.Length; i++) { neighborMatch[n, i] = (neighborBlocks[i].getType() == getType() && neighborBlocks[i].isFaceCulled( ((Direction)n) ) == false && neighborBlocks[i].isFaceBlocked( ((Direction)n) ) == false); }
			}
		}

		public override Coord2D getTexture(Direction _face)
		{
			int column = 4;
			int row = 12;

			if (chunk.getVoxelObject() != null && isFaceCulled(_face) == false && isFaceBlocked(_face) == false)
			{
				switch (_face)
				{
				case Direction.BELOW:
				case Direction.ABOVE:
					if (neighborMatch[(int)_face, (int)Direction.NORTH] == true)
					{
						row += 2;

						if (neighborMatch[(int)_face, (int)Direction.SOUTH] == false) { row += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.SOUTH] == true) { row += 1; }
					}

					if (neighborMatch[(int)_face, (int)Direction.EAST] == true)
					{
						column += 1;

						if (neighborMatch[(int)_face, (int)Direction.WEST] == true) { column += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.WEST] == true) { column += 3; }
					}
					break;

				case Direction.NORTH:
				case Direction.SOUTH:
					if (neighborMatch[(int)_face, (int)Direction.ABOVE] == true)
					{
						row += 2;

						if (neighborMatch[(int)_face, (int)Direction.BELOW] == false) { row += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.BELOW] == true) { row += 1; }
					}

					if (neighborMatch[(int)_face, (int)Direction.EAST] == true)
					{
						column += 1;

						if (neighborMatch[(int)_face, (int)Direction.WEST] == true) { column += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.WEST] == true) { column += 3; }
					}
					break;

				case Direction.WEST:
				case Direction.EAST:
					if (neighborMatch[(int)_face, (int)Direction.ABOVE] == true)
					{
						row += 2;

						if (neighborMatch[(int)_face, (int)Direction.BELOW] == false) { row += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.BELOW] == true) { row += 1; }
					}

					if (neighborMatch[(int)_face, (int)Direction.NORTH] == true)
					{
						column += 1;

						if (neighborMatch[(int)_face, (int)Direction.SOUTH] == true) { column += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.SOUTH] == true) { column += 3; }
					}
					break;
				}
			}

			return new Coord2D(column, row);
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
			light.color = new Color(0.6f, 0.6f, 0.6f); light.range = 10.0f; light.intensity = 20.0f;
			light.shadows = LightShadows.Hard;
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