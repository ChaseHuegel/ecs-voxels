using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
	public class ModelBuilder
	{
		public static Mesh GetVoxelMesh(Voxel _voxel)
		{
			return GetVoxelMesh(_voxel.toBlock());
		}

		public static Mesh GetVoxelMesh(Block _block)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<Color> colors = new List<Color>();

			Block thisBlock = _block;

			Vector3 scale;
			switch (thisBlock.getModelType())
			{
				case ModelType.CUBE:
					ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock);
					break;

				case ModelType.SLOPE:
					ModelBuilder.Slope.Face(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Slope.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Slope.North(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Slope.East(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Slope.West(vertices, triangles, normals, uvs, colors, thisBlock);
					break;

				case ModelType.CUSTOM:
					ModelBuilder.Custom.Build(vertices, triangles, normals, uvs, colors, thisBlock.getModelData(), thisBlock);
					break;

				case ModelType.CUSTOM_CUBE:
					ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock);
					ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock);
					break;

				case ModelType.CROSS_SECTION_SMALL:
					ModelBuilder.CrossSection.Small.Build(vertices, triangles, normals, uvs, colors, thisBlock);
					break;

				case ModelType.CUBE_HALF:
					scale = new Vector3(1.0f, 1.0f, 0.5f);
					ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					break;

				case ModelType.CUBE_PLATE:
					scale = new Vector3(1.0f, 1.0f, 0.25f);
					ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock, scale);
					break;
			}

			for (int i = 0; i < vertices.Count; i++)
			{
				vertices[i] -= Vector3.one * 0.5f;
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uvs.ToArray();
       		mesh.colors = colors.ToArray();

			return mesh;
		}

		public class Custom
		{
			public static void Build(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, CachedModel _model, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Position offset = _thisBlock.getOffset();
				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				for (int i = 0; i < _model.vertices.Length; i++)
				{
					_vertices.Add( Quaternion.Euler(rotation) * (_model.vertices[i] - origin + origin) + origin );

					_normals.Add(_model.normals[i]);
					_colors.Add(_thisBlock.getBlockData().color);
				}

				for (int i = 0; i < _model.triangles.Length; i++)
				{
					_triangles.Add(vertIndex + _model.triangles[i]);
				}

				for (int i = 0; i < _model.uv.Length; i++)
				{
					_uvs.Add(_model.uv[i]);
				}
			}
		}

		public class CrossSection
		{
			public class Small
			{
				public static void Build(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
				{
					BuildSection1(_vertices, _triangles, _normals, _uvs, _colors, _thisBlock);
					BuildSection2(_vertices, _triangles, _normals, _uvs, _colors, _thisBlock);
				}

				public static void BuildSection1(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
				{
					int vertIndex = _vertices.Count;

					//	Section 1 back

					Coord2D[] textureCoords = new Coord2D[] { new Coord2D(0, 0), new Coord2D(1, 0), new Coord2D(1, 1), new Coord2D(0, 1) };

					Position offset = _thisBlock.getOffset();
					Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 0.792895f, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
														new Vector3(_thisBlock.getX() + offset.x + 0.792895f, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
														new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 0.792895f),
														new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 0.792895f) };

					ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, 1 + _thisBlock.getTextureRotation(Direction.EAST));	//	Perform any rotations by shifting vertice indices

					_vertices.Add(vertexCoords[0]);
					_vertices.Add(vertexCoords[1]);
					_vertices.Add(vertexCoords[2]);
					_vertices.Add(vertexCoords[3]);

					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);

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

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y + (Constants.TEX_UNIT * textureCoords[2].y))));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y + (Constants.TEX_UNIT * textureCoords[3].y))));

					//	Section 1 front

					_vertices.Add(vertexCoords[0]);
					_vertices.Add(vertexCoords[1]);
					_vertices.Add(vertexCoords[2]);
					_vertices.Add(vertexCoords[3]);

					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);

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

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y + (Constants.TEX_UNIT * textureCoords[2].y))));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y + (Constants.TEX_UNIT * textureCoords[3].y))));
				}

				public static void BuildSection2(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
				{
					int vertIndex = _vertices.Count;

					//	Section 2 back

					Coord2D[] textureCoords = new Coord2D[] { new Coord2D(0, 0), new Coord2D(1, 0), new Coord2D(1, 1), new Coord2D(0, 1) };

					Position offset = _thisBlock.getOffset();
					Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 0.792895f, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 0.792895f),
														new Vector3(_thisBlock.getX() + offset.x + 0.792895f, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 0.792895f),
														new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z),
														new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z) };

					ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, 1 + _thisBlock.getTextureRotation(Direction.WEST));	//	Perform any rotations by shifting vertice indices

					_vertices.Add(vertexCoords[0]);
					_vertices.Add(vertexCoords[1]);
					_vertices.Add(vertexCoords[2]);
					_vertices.Add(vertexCoords[3]);

					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);

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

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y + (Constants.TEX_UNIT * textureCoords[2].y))));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y + (Constants.TEX_UNIT * textureCoords[3].y))));

					//	Section 2 front

					_vertices.Add(vertexCoords[0]);
					_vertices.Add(vertexCoords[1]);
					_vertices.Add(vertexCoords[2]);
					_vertices.Add(vertexCoords[3]);

					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);
					_colors.Add(_thisBlock.getBlockData().color);

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

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y + (Constants.TEX_UNIT * textureCoords[2].y))));

					_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y + (Constants.TEX_UNIT * textureCoords[3].y))));
				}
			}
		}

		public class Cube
		{
			//	The top face of a block
			public static void Top(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock, Vector3 _scale = default(Vector3))
			{
				if (_scale == Vector3.zero) { _scale = Vector3.one; }
				int vertIndex = _vertices.Count;

				Vector2[] textureCoords = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
				for (int i = 0; i < textureCoords.Length; i++) { textureCoords[i] = Vector2.Scale( textureCoords[i],  new Vector2(_scale.x, _scale.z) ); }

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z)};

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, _thisBlock.getTextureRotation(Direction.ABOVE));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}

			//	The bottom face of a block
			public static void Bottom(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock, Vector3 _scale = default(Vector3))
			{
				if (_scale == Vector3.zero) { _scale = Vector3.one; }
				int vertIndex = _vertices.Count;

				Vector2[] textureCoords = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
				for (int i = 0; i < textureCoords.Length; i++) { textureCoords[i] = Vector2.Scale( textureCoords[i],  new Vector2(_scale.x, _scale.z) ); }

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z)};

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, _thisBlock.getTextureRotation(Direction.BELOW));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}

			//	The north face of a block
			public static void North(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
			{
				North(_vertices, _triangles, _normals, _uvs, _colors, _thisBlock, Vector3.one);
			}
			public static void North(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock, Vector3 _scale)
			{
				int vertIndex = _vertices.Count;

				Vector2[] textureCoords = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
				for (int i = 0; i < textureCoords.Length; i++) { textureCoords[i] = Vector2.Scale( textureCoords[i],  new Vector2(_scale.x, _scale.y) ); }

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + (1 * _scale.z))};

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, _thisBlock.getTextureRotation(Direction.NORTH));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}

			//	The south face of a block
			public static void South(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock, Vector3 _scale = default(Vector3))
			{
				if (_scale == Vector3.zero) { _scale = Vector3.one; }
				int vertIndex = _vertices.Count;

				Vector2[] textureCoords = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
				for (int i = 0; i < textureCoords.Length; i++) { textureCoords[i] = Vector2.Scale( textureCoords[i],  new Vector2(_scale.x, _scale.y) ); }

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z)};

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, _thisBlock.getTextureRotation(Direction.SOUTH));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.SOUTH).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.SOUTH).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.SOUTH).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.SOUTH).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.SOUTH).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.SOUTH).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.SOUTH).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.SOUTH).y + (Constants.TEX_UNIT * textureCoords[3].y))));

			}

			//	The east face of a block
			public static void East(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock, Vector3 _scale = default(Vector3))
			{
				if (_scale == Vector3.zero) { _scale = Vector3.one; }
				int vertIndex = _vertices.Count;

				Vector2[] textureCoords = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
				for (int i = 0; i < textureCoords.Length; i++) { textureCoords[i] = Vector2.Scale( textureCoords[i],  new Vector2(_scale.z, _scale.y) ); }

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x + (1 * _scale.x), _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z)};

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, _thisBlock.getTextureRotation(Direction.EAST));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}

			//	The west face of a block
			public static void West(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock, Vector3 _scale = default(Vector3))
			{
				if (_scale == Vector3.zero) { _scale = Vector3.one; }
				int vertIndex = _vertices.Count;

				Vector2[] textureCoords = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
				for (int i = 0; i < textureCoords.Length; i++) { textureCoords[i] = Vector2.Scale( textureCoords[i],  new Vector2(_scale.z, _scale.y) ); }

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + (1 * _scale.y), _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + (1 * _scale.z)),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z)};

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, _thisBlock.getTextureRotation(Direction.WEST));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}
		}

		public class Slope
		{
			//	The face of a slope
			public static void Face(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Coord2D[] textureCoords = new Coord2D[] { new Coord2D(0, 0), new Coord2D(1, 0), new Coord2D(1, 1), new Coord2D(0, 1) };

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z) };

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, _thisBlock.getTextureRotation(Direction.ABOVE));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.ABOVE).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}

			//	The bottom face of a slope
			public static void Bottom(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Coord2D[] textureCoords = new Coord2D[] { new Coord2D(0, 0), new Coord2D(1, 0), new Coord2D(1, 1), new Coord2D(0, 1) };

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z) };

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, _thisBlock.getTextureRotation(Direction.BELOW));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.BELOW).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}

			//	The north face of a slope
			public static void North(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Coord2D[] textureCoords = new Coord2D[] { new Coord2D(0, 0), new Coord2D(1, 0), new Coord2D(1, 1), new Coord2D(0, 1) };

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1) };

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, 1 + _thisBlock.getTextureRotation(Direction.NORTH));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);

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

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.NORTH).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}

			//	The east face of a slope
			public static void East(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Coord2D[] textureCoords = new Coord2D[] { new Coord2D(0, 0), new Coord2D(1, 0), new Coord2D(1, 1), new Coord2D(0, 1) };

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													// new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x + 1, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1) };

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, 1 + _thisBlock.getTextureRotation(Direction.EAST));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				// _vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				// _colors.Add(_thisBlock.getBlockData().color);

				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				// _normals.Add(new Vector3(0, 1, 0));

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 1);
				_triangles.Add(vertIndex + 2);

				// _triangles.Add(vertIndex);
				// _triangles.Add(vertIndex + 2);
				// _triangles.Add(vertIndex + 3);

				// _uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.EAST).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}

			//	The west face of a slope
			public static void West(List<Vector3> _vertices, List<int> _triangles, List<Vector3> _normals, List<Vector2> _uvs, List<Color> _colors, Block _thisBlock)
			{
				int vertIndex = _vertices.Count;

				Coord2D[] textureCoords = new Coord2D[] { new Coord2D(0, 0), new Coord2D(1, 0), new Coord2D(1, 1), new Coord2D(0, 1) };

				Position offset = _thisBlock.getOffset();
				Vector3[] vertexCoords = new Vector3[] { new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													// new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y + 1, _thisBlock.getZ() + offset.z + 1),
													new Vector3(_thisBlock.getX() + offset.x, _thisBlock.getY() + offset.y, _thisBlock.getZ() + offset.z + 1) };

				ArrayUtils.ShiftDown<Vector3>(ref vertexCoords, 1 + _thisBlock.getTextureRotation(Direction.WEST));	//	Perform any rotations by shifting vertice indices

				Vector3 origin = new Vector3(_thisBlock.getX() + offset.x + 0.5f, _thisBlock.getY() + offset.y + 0.5f, _thisBlock.getZ() + offset.z + 0.5f);

				Vector3 rotation = (_thisBlock.getBlockData() is Rotatable ? ((Rotatable)_thisBlock.getBlockData()).getRotation() : Vector3.zero);

				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[0] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[1] - origin) + origin);
				_vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[2] - origin) + origin);
				// _vertices.Add(Quaternion.Euler(rotation) * (vertexCoords[3] - origin) + origin);

				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				_colors.Add(_thisBlock.getBlockData().color);
				// _colors.Add(_thisBlock.getBlockData().color);

				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				_normals.Add(new Vector3(0, 1, 0));
				// _normals.Add(new Vector3(0, 1, 0));

				_triangles.Add(vertIndex);
				_triangles.Add(vertIndex + 2);
				_triangles.Add(vertIndex + 1);

				// _triangles.Add(vertIndex);
				// _triangles.Add(vertIndex + 3);
				// _triangles.Add(vertIndex + 2);

				// _uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[0].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y) + (Constants.TEX_UNIT * textureCoords[0].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[1].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y) + (Constants.TEX_UNIT * textureCoords[1].y)));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[2].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y + (Constants.TEX_UNIT * textureCoords[2].y))));

				_uvs.Add(new Vector2(Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).x + (Constants.TEX_UNIT * textureCoords[3].x), 1 - (Constants.TEX_UNIT * _thisBlock.getTexture(Direction.WEST).y + (Constants.TEX_UNIT * textureCoords[3].y))));
			}
		}
	}
}