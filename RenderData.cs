using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class RenderData
    {
		public Vector3[] vertices;
		public int[] triangles;
		public Vector3[] normals;
		public Vector2[] uvs;
		public Color[] colors;

		public RenderData(Vector3[] _vertices, int[] _triangles, Vector3[] _normals, Vector2[] _uvs, Color[] _colors)
		{
			vertices = _vertices;
			triangles = _triangles;
			normals = _normals;
			uvs = _uvs;
			colors = _colors;
		}

		public RenderData(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors)
		{
			vertices = _vertices.ToArray();
			triangles = _triangles.ToArray();
			normals = _normals.ToArray();
			uvs = _uvs.ToArray();
			colors = _colors.ToArray();
		}
	}
}