using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
	public class ColliderBuilder
	{
		public class CrossSection
		{
			public class Small
			{
				public static void Build(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
				{
					BuildSection1(_vertices, _triangles, _normals, _thisBlock);
					BuildSection2(_vertices, _triangles, _normals, _thisBlock);
				}

				public static void BuildSection1(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
				{
					int vertIndex = _vertices.Count;

					//	Section 1 back

					Position offset = _thisBlock.getOffset();
					Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 0.792895f, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
														new Vector3(_thisBlock.getX() + offset.x + 0.792895f, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
														new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 0.792895f),
														new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 0.792895f) };

					_vertices.Add(vertexCoords[0]);
					_vertices.Add(vertexCoords[1]);
					_vertices.Add(vertexCoords[2]);
					_vertices.Add(vertexCoords[3]);

					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));

					_triangles.Add(vertIndex);
					_triangles.Add(vertIndex + 1);
					_triangles.Add(vertIndex + 2);

					_triangles.Add(vertIndex);
					_triangles.Add(vertIndex + 2);
					_triangles.Add(vertIndex + 3);
					
					//	Section 1 front

					_vertices.Add(vertexCoords[0]);
					_vertices.Add(vertexCoords[1]);
					_vertices.Add(vertexCoords[2]);
					_vertices.Add(vertexCoords[3]);

					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));

					_triangles.Add(vertIndex);
					_triangles.Add(vertIndex + 2);
					_triangles.Add(vertIndex + 1);

					_triangles.Add(vertIndex);
					_triangles.Add(vertIndex + 3);
					_triangles.Add(vertIndex + 2);
				}

				public static void BuildSection2(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
				{
					int vertIndex = _vertices.Count;

					//	Section 2 back
					Position offset = _thisBlock.getOffset();
					Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 0.792895f, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 0.792895f),
														new Vector3(_thisBlock.getX() + offset.x + 0.792895f, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 0.792895f),
														new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
														new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z) };

					_vertices.Add(vertexCoords[0]);
					_vertices.Add(vertexCoords[1]);
					_vertices.Add(vertexCoords[2]);
					_vertices.Add(vertexCoords[3]);

					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));

					_triangles.Add(vertIndex);
					_triangles.Add(vertIndex + 1);
					_triangles.Add(vertIndex + 2);

					_triangles.Add(vertIndex);
					_triangles.Add(vertIndex + 2);
					_triangles.Add(vertIndex + 3);
					
					//	Section 2 front

					_vertices.Add(vertexCoords[0]);
					_vertices.Add(vertexCoords[1]);
					_vertices.Add(vertexCoords[2]);
					_vertices.Add(vertexCoords[3]);

					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));
					_normals.Add(new Vector3(0, 1, 0));

					_triangles.Add(vertIndex);
					_triangles.Add(vertIndex + 2);
					_triangles.Add(vertIndex + 1);

					_triangles.Add(vertIndex);
					_triangles.Add(vertIndex + 3);
					_triangles.Add(vertIndex + 2);
				}
			}
		}

		public class Cube
		{
			//	The top face of a block
			public static void Top(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z) };

				_vertices.Add(vertexCoords[0]);
				_vertices.Add(vertexCoords[1]);
				_vertices.Add(vertexCoords[2]);
				_vertices.Add(vertexCoords[3]);

				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 1);
				_triangles.Add(vertIndex + 2);

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 2);
				_triangles.Add(vertIndex + 3);
			}

			//	The bottom face of a block
			public static void Bottom(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z) };

				_vertices.Add(vertexCoords[0]);
				_vertices.Add(vertexCoords[1]);
				_vertices.Add(vertexCoords[2]);
				_vertices.Add(vertexCoords[3]);

				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 2);
				_triangles.Add(vertIndex + 1);

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 3);
				_triangles.Add(vertIndex + 2);
			}

			//	The north face of a block
			public static void North(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1) };

				_vertices.Add(vertexCoords[0]);
				_vertices.Add(vertexCoords[1]);
				_vertices.Add(vertexCoords[2]);
				_vertices.Add(vertexCoords[3]);

				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 1);
				_triangles.Add(vertIndex + 2);

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 2);
				_triangles.Add(vertIndex + 3);
			}

			//	The south face of a block
			public static void South(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z) };

				_vertices.Add(vertexCoords[0]);
				_vertices.Add(vertexCoords[1]);
				_vertices.Add(vertexCoords[2]);
				_vertices.Add(vertexCoords[3]);

				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 2);
				_triangles.Add(vertIndex + 1);

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 3);
				_triangles.Add(vertIndex + 2);
				
			}

			//	The east face of a block
			public static void East(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1) };

				_vertices.Add(vertexCoords[0]);
				_vertices.Add(vertexCoords[1]);
				_vertices.Add(vertexCoords[2]);
				_vertices.Add(vertexCoords[3]);

				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 1);
				_triangles.Add(vertIndex + 2);

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 2);
				_triangles.Add(vertIndex + 3);
			}

			//	The west face of a block
			public static void West(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1) };

				_vertices.Add(vertexCoords[0]);
				_vertices.Add(vertexCoords[1]);
				_vertices.Add(vertexCoords[2]);
				_vertices.Add(vertexCoords[3]);

				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 2);
				_triangles.Add(vertIndex + 1);

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 3);
				_triangles.Add(vertIndex + 2);
			}
		}
	}
}