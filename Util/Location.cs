using System;

namespace Swordfish
{
	//	Location in a voxel object
	public class Location
	{
		public float x = 0;
		public float y = 0;
		public float z = 0;

		public VoxelObject voxelObject;

		public Location(float _x, float _y, float _z, VoxelObject _voxelObject)
		{
			x = _x;
			y = _y;
			z = _z;

			voxelObject = _voxelObject;
		}

		//	Convert int to float
		public Location(int _x, int _y, int _z, VoxelObject _voxelObject)
		{
			x = (float)_x;
			y = (float)_y;
			z = (float)_z;

			voxelObject = _voxelObject;
		}
	}
}