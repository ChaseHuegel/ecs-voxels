using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace Swordfish
{
	public class ChunkRenderer : ThreadedJob
	{
		//  Keep this object alive
		private static ChunkRenderer _instance;
		public static ChunkRenderer instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.ScriptableObject.CreateInstance<ChunkRenderer>();
				}

				return _instance;
			}
		}

		private RenderState state = RenderState.Ready;

		private ConcurrentQueue<Coord3D> queue = new ConcurrentQueue<Coord3D>();

		private List<Vector3> vertices = new List<Vector3>();
		private List<int> triangles = new List<int>();
		private List<Vector3> normals = new List<Vector3>();
		private List<Vector2> uvs = new List<Vector2>();
		private List<Color> colors = new List<Color>();

		public RenderState State()
		{
			return state;
		}

		public void setState(RenderState _state)
		{
			state = _state;
		}

		public void Queue(Coord3D _chunk)
		{
			//	Make certain this chunk isn't already queued
			/*for (int i = 0; i < queue.Count; i++)
			{
				if (ReferenceEquals(queue[i], _chunk) == true)
				{
					return;
				}
			}*/

			queue.Enqueue(_chunk);

			// if (_chunk != null)// && _chunk.isVoid() == false && _chunk.isCulled() == false)
			// {
			// 	queue.Enqueue(_chunk);
			// }
		}

		public void ClearQueue()
		{
			Coord3D entry;
			while (!queue.IsEmpty)
			{
				queue.TryDequeue(out entry);
			}
		}

		public int QueueSize()
		{
			return queue.Count;
		}

		protected override void ThreadFunction()
		{
			while (stop != true)
			{
				if (state == RenderState.Stopped)
				{
					continue;
				}
				else if (state == RenderState.Stopping)
				{
					state = RenderState.Stopped;
					continue;
				}

				if (queue.Count == 0)
				{
					state = RenderState.Waiting;
				}
				else
				{
					state = RenderState.Rendering;

					Coord3D thisPosition;
					bool dequeued = queue.TryDequeue(out thisPosition);

					if (dequeued == false || thisPosition == null || thisPosition.voxelObject == null) { continue; }

					Chunk thisChunk = thisPosition.voxelObject.getChunk(thisPosition.x, thisPosition.y, thisPosition.z);

					if (thisChunk == null)
					{
						continue;
					}

					thisChunk.setRenderState(RenderState.Rendering);

					vertices.Clear();
					triangles.Clear();
					normals.Clear();
					uvs.Clear();
					colors.Clear();

					//  Loop all blocks in this chunk
					for (int n = 0; n < thisChunk.getSize(); n++)
					{
						if (state == RenderState.Stopped)
						{
							thisChunk.setRenderState(RenderState.Stopped);
							break;
						}

						Block thisBlock = thisChunk.raw(n);
						if (thisBlock == null)
						{
							continue;
						}
						else
						{
							thisBlock.OnPreRender();

							Vector3 scale;
							switch (thisBlock.getModelType())
							{
								case ModelType.CUBE:
									//  Top face
									if (thisBlock.isFaceCulled(Direction.ABOVE) == false)
									{
										ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock);
									}

									//  Bottom face
									if (thisBlock.isFaceCulled(Direction.BELOW) == false)
									{
										ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
									}

									//  North face
									if (thisBlock.isFaceCulled(Direction.NORTH) == false)
									{
										ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock);
									}

									//  South face
									if (thisBlock.isFaceCulled(Direction.SOUTH) == false)
									{
										ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock);
									}

									//  East face
									if (thisBlock.isFaceCulled(Direction.EAST) == false)
									{
										ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock);
									}

									//  West face
									if (thisBlock.isFaceCulled(Direction.WEST) == false)
									{
										ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock);
									}
									break;

								case ModelType.SLOPE:
									if (thisBlock.isCulled() == false)
									{
										ModelBuilder.Slope.Face(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Slope.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Slope.North(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Slope.East(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Slope.West(vertices, triangles, normals, uvs, colors, thisBlock);
									}
									break;

								case ModelType.CROSS_SECTION_SMALL:
									if (thisBlock.isCulled() == false)
									{
										ModelBuilder.CrossSection.Small.Build(vertices, triangles, normals, uvs, colors, thisBlock);
									}
									break;

								case ModelType.CUSTOM:
									if (thisBlock.isCulled() == false)
									{
										ModelBuilder.Custom.Build(vertices, triangles, normals, uvs, colors, thisBlock.getModelData(), thisBlock);
									}
									break;

								case ModelType.CUSTOM_CUBE:
									if (thisBlock.isCulled() == false)
									{
										ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock);
										ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock);
									}
									break;

								case ModelType.CUBE_HALF:
									if (thisBlock.isCulled() == false)
									{
										scale = new Vector3(1.0f, 1.0f, 0.5f);
										ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock, scale);
									}
									break;

								case ModelType.CUBE_PLATE:
									if (thisBlock.isCulled() == false)
									{
										scale = new Vector3(1.0f, 1.0f, 0.25f);
										ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock, scale);
										ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock, scale);
									}
									break;
							}
						}
					}

					if (state != RenderState.Stopped)
					{
						thisChunk.PrepareRenderBuffer(new RenderData(vertices, triangles, normals, uvs, colors));
						state = RenderState.Waiting;
					}
				}

				updates++;	//	Increment update count
			}

			IsDone = true;
		}
	}
}