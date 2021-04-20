using System;
using System.Collections;
using System.Collections.Generic;

namespace Swordfish
{
	[Serializable]
	public class VoxelMaster
	{
		public Dictionary<Guid, VoxelObject> voxelObjects = new Dictionary<Guid, VoxelObject>();

		public List<Coord3D[]> asteroidPresets = new List<Coord3D[]>();
		public Random random;

		public VoxelMaster()
		{
			random = new Random();

			for (int i = 0; i < 64; i++)
			{
				int radius = (int)( (48 * 0.5f) * (getRandom().NextDouble() * (1.0f - 0.65f) + 0.65f) );
				asteroidPresets.Add( Sphere(radius, 16) );
			}
		}

		public void Tick()
		{
			foreach (KeyValuePair<Guid, VoxelObject> entry in voxelObjects)
            {
                VoxelObject thisVoxelObject = entry.Value;

				if (thisVoxelObject != null && thisVoxelObject.isStatic == false)
				{
					thisVoxelObject.Tick();
				}

				if (thisVoxelObject.component != null)
				{
					thisVoxelObject.component.Tick();
				}
			}
		}

		public void Update()
		{
			foreach (KeyValuePair<Guid, VoxelObject> entry in voxelObjects)
            {
                VoxelObject thisVoxelObject = entry.Value;

				if (thisVoxelObject != null)
				{
					thisVoxelObject.Update();
				}

				if (thisVoxelObject.component != null)
				{
					thisVoxelObject.component.TryBuildChunk();
				}
			}
		}

		public void Render()
		{
			foreach (KeyValuePair<Guid, VoxelObject> entry in voxelObjects)
            {
                VoxelObject thisVoxelObject = entry.Value;

				if (thisVoxelObject != null && thisVoxelObject.loaded == true)
				{
					thisVoxelObject.Render();
				}
			}
		}

		public float getDistance(int _x, int _y, int _z)
		{
			return _x * _x + _y * _y + _z * _z;
		}

		public Random getRandom()
		{
			return random;
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
						}
					}
				}
			}

			return coordinates.ToArray();
		}
	}
}