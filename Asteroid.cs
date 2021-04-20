using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace Swordfish
{
	public class Asteroid : VoxelObject
	{
		public Asteroid(int _sizeX = 1, int _sizeY = 1, int _sizeZ = 1, VoxelComponent _component = null) : base(_sizeX, _sizeY, _sizeZ, _component) {}
		public Asteroid(int _sizeX, int _sizeY, int _sizeZ, VoxelComponent _component, Guid _guid) : base(_sizeX, _sizeY, _sizeZ, _component, _guid) {}

		public float getDistance(int _x, int _y, int _z)
		{
			return _x * _x + _y * _y + _z * _z;
		}

		public Coord3D[] Sphere(int _radius, int _cutouts = 0)
		{
			List<Coord3D> coordinates = new List<Coord3D>();

			int radiusSquared = _radius * _radius;

			Coord3D[] cutoutOrigins = new Coord3D[_cutouts];
			int[] cutoutRadiuses = new int[_cutouts];
			for (int i = 0; i < _cutouts; i++)
			{
				int xPos = getRandom().Next((int)(_radius * 2)) - _radius;
				int yPos = getRandom().Next((int)(_radius * 2)) - _radius;
				int zPos = getRandom().Next((int)(_radius * 2)) - _radius;
				Coord3D cutoutOrigin = new Coord3D( xPos, yPos, zPos );
				int cutoutRadius = getRandom().Next(4, (int)(_radius * 0.6f));
				cutoutRadius *= cutoutRadius;

				cutoutOrigins[i] = cutoutOrigin;
				cutoutRadiuses[i] = cutoutRadius;
			}

			Coord3D[] addOrigins = new Coord3D[_cutouts];
			int[] addRadiuses = new int[_cutouts];
			for (int i = 0; i < _cutouts; i++)
			{
				int xPos = getRandom().Next((int)(_radius * 2)) - _radius;
				int yPos = getRandom().Next((int)(_radius * 2)) - _radius;
				int zPos = getRandom().Next((int)(_radius * 2)) - _radius;
				Coord3D cutoutOrigin = new Coord3D( xPos, yPos, zPos );
				int cutoutRadius = getRandom().Next(4, (int)(_radius * 0.6f));
				cutoutRadius *= cutoutRadius;

				addOrigins[i] = cutoutOrigin;
				addRadiuses[i] = cutoutRadius;
			}

			float xScale = ((float)getRandom().Next(30, 100)) / 100;
			float yScale = ((float)getRandom().Next(30, 100)) / 100;
			float zScale = ((float)getRandom().Next(30, 100)) / 100;

			//	Distance filled sphere
			for (int x = -_radius; x < _radius; x++)
			{
				for (int y = -_radius; y < _radius; y++)
				{
					for (int z = -_radius; z < _radius; z++)
					{
						Coord3D coord = new Coord3D(x * xScale, y * yScale, z * zScale);
						float distance = getDistance(x, y, z);
						if (distance < radiusSquared)
						{
							bool isCut = false;

							for (int i = 0; i < _cutouts; i++)
							{
								float cutoutDistance = getDistance(x - cutoutOrigins[i].x, y - cutoutOrigins[i].y, z - cutoutOrigins[i].z);
								if (cutoutDistance < cutoutRadiuses[i])
								{
									isCut = true;
									break;
								}
							}

							if (isCut == false)
							{
								for (int i = 0; i < _cutouts; i++)
								{
									float cutoutDistance = getDistance(x - addOrigins[i].x, y - addOrigins[i].y, z - addOrigins[i].z);
									if (cutoutDistance < addRadiuses[i])
									{
										coordinates.Add(coord);
										break;
									}
								}
							}

							// if (isCut == false) { coordinates.Add(coord); }
						}
					}
				}
			}

			//	Hallow sphere
			// double step1 = (2*Math.PI) / (_radius * 10);
			// double step2 = 360 / (_radius * 10);

			// for (double i = 0; i <= 2*Math.PI; i += step1)
			// {
			// 	for (double n = 0; n <= 360; n += step2)
			// 	{
			// 		for (int radius = 0; radius < _radius; radius++)
			// 		{
			// 			int x = (int)( radius * Math.Cos(n) * Math.Sin(i) );
			// 			int y = (int)( radius * Math.Cos(i));
			// 			int z = (int)( radius * Math.Sin(n) * Math.Sin(i) );

			// 			coordinates.Add(new Coord3D(x, y, z));
			// 		}
			// 	}
			// }

			//	Filled circle
			// for (int i = 0; i < _radius; i++)
			// {
			// 	for (double n = 0.0d; n < 360.0d; n += 0.1d)
			// 	{
			// 		double angle = n * Math.PI / 180.0d;
			// 		int x = (int)( i * Math.Cos(angle) );
			// 		int y = (int)( i * Math.Sin(angle) );

			// 		coordinates.Add(new Coord3D(x, y, z));
			// 	}
			// }

			return coordinates.ToArray();
		}

		public override void FinishLoad()
		{
			base.FinishLoad();

			this.setBlockAt(getBlockSizeX() / 2, getBlockSizeY() / 2, getBlockSizeZ() / 2, Voxel.VOID);

			int radius = (int)( (getBlockSizeX() * 0.5f) * (getRandom().NextDouble() * (1.0f - 0.65f) + 0.65f) );
			Coord3D[] points = GameMaster.Instance.voxelMaster.asteroidPresets[getRandom().Next(0, GameMaster.Instance.voxelMaster.asteroidPresets.Count)]; //Sphere( radius, 16 );

			Voxel voxel = Voxel.ASTEROID_ROCK;
			if (getRandom().NextDouble() <= 0.5d)
			{
				voxel = Voxel.ASTEROID_ICE;
			}

			Coord3D offset = new Coord3D(getBlockSizeX() / 2, getBlockSizeY() / 2, getBlockSizeZ() / 2);
			for (int i = 0; i < points.Length; i++)
			{
				Coord3D point = points[i] + offset;

				this.setBlockAt(point, voxel, false);
			}

			component.transform.Rotate(getRandom().Next(360), getRandom().Next(360), getRandom().Next(360));
		}
	}
}