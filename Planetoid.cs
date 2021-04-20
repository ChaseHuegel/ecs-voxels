using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace Swordfish
{
	public class Planetoid : VoxelObject
	{
		public Planetoid(int _sizeX = 1, int _sizeY = 1, int _sizeZ = 1, VoxelComponent _component = null) : base(_sizeX, _sizeY, _sizeZ, _component) {}
		public Planetoid(int _sizeX, int _sizeY, int _sizeZ, VoxelComponent _component, Guid _guid) : base(_sizeX, _sizeY, _sizeZ, _component, _guid) {}

		public float getDistance(int _x, int _y, int _z)
		{
			return _x * _x + _y * _y + _z * _z;
		}

		public void Sphere(int _radius)
		{
			int radiusSquared = _radius * _radius;
			Coord3D offset = new Coord3D(getBlockSizeX() / 2, getBlockSizeY() / 2, getBlockSizeZ() / 2);

			//	Distance filled sphere
			for (int x = -_radius; x < _radius; x++)
			{
				for (int y = -_radius; y < _radius; y++)
				{
					for (int z = -_radius; z < _radius; z++)
					{
						Coord3D coord = new Coord3D(x, y, z);
						//float distance = Util.DistanceUnsquared(coord.toVector3(), UnityEngine.Vector3.zero);
						float distance = getDistance(x, y, z);

						if (distance < radiusSquared)
						{
							Voxel voxel = Voxel.ASTEROID_ROCK;
							if (distance > radiusSquared - (_radius * 3)) {voxel = Voxel.GRASS_BLOCK; }
							else if (distance > radiusSquared - (_radius * 9)) {voxel = Voxel.DIRT; }

							this.setBlockAt(coord + offset, voxel, false);
						}
					}
				}
			}
		}

		public override void FinishLoad()
		{
			base.FinishLoad();

			int radius = (int)( (getBlockSizeX() * 0.5f) * (getRandom().NextDouble() * (1.0f - 0.65f) + 0.65f) );
			Sphere(radius);

			//component.transform.Rotate(getRandom().Next(360), getRandom().Next(360), getRandom().Next(360));
		}
	}
}