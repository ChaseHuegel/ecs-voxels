using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class CollisionData
    {
		public Vector3[] centers;
		public Vector3[] sizes;
		public int Count = 0;

		public CollisionData(Vector3[] _centers, Vector3[] _sizes)
		{
			centers = _centers;
			sizes = _sizes;
			Count = centers.Length;
		}

		public CollisionData(List<Vector3> _centers, List<Vector3> _sizes)
		{
			centers = _centers.ToArray();
			sizes = _sizes.ToArray();
			Count = centers.Length;
		}
	}
}