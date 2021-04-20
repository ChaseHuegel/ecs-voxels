using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace Swordfish
{
	public class ChunkLoader : ThreadedJob
	{
		//  Keep this object alive
		private static ChunkLoader _instance;
		public static ChunkLoader instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.ScriptableObject.CreateInstance<ChunkLoader>();
				}

				return _instance;
			}
		}
		private VoxelComponent voxelComponent;

		private ChunkState state = ChunkState.Ready;

		private ConcurrentQueue<ChunkComponent> queue = new ConcurrentQueue<ChunkComponent>();

		private List<Vector3> vertices = new List<Vector3>();
		private List<int> triangles = new List<int>();
		private List<Vector3> normals = new List<Vector3>();
		private List<Vector2> uvs = new List<Vector2>();
		private List<Color> colors = new List<Color>();

		public ChunkState State()
		{
			return state;
		}

		public void setState(ChunkState _state)
		{
			state = _state;
		}

		public void Queue(ChunkComponent _component)
		{
			queue.Enqueue(_component);
		}

		public void ClearQueue()
		{
			ChunkComponent entry;
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
				if (state == ChunkState.Stopped)
				{
					continue;
				}
				else if (state == ChunkState.Stopping)
				{
					state = ChunkState.Stopped;
					continue;
				}

				if (queue.Count == 0)
				{
					state = ChunkState.Waiting;
				}
				else
				{
					state = ChunkState.Loading;

					ChunkComponent thisComponent;
					bool dequeued = queue.TryDequeue(out thisComponent);

					if (dequeued == false || thisComponent == null) { continue; }

					Coord3D thisPosition = thisComponent.position;
					if (thisPosition == null) { continue; }

					VoxelObject voxelObject = thisComponent.position.voxelObject;
					if (voxelObject == null) { continue; }

					if (thisComponent.chunk == null)
					{
						Chunk thisChunk = voxelObject.GenerateChunk(thisPosition.x, thisPosition.y, thisPosition.z);
						thisChunk.component = thisComponent;
						thisComponent.chunk = thisChunk;
						thisChunk.PrepareLoad();
						thisComponent.BuildCollision();
					}
					else
					{
						thisComponent.BuildCollision();
					}

					if (state != ChunkState.Stopped)
					{
						state = ChunkState.Waiting;
					}
				}

				updates++;	//	Increment update count
			}

			IsDone = true;
		}
	}
}