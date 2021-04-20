using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace Swordfish
{
	public class ChunkCollisionBuilder : ThreadedJob
	{
		//  Keep this object alive
		private static ChunkCollisionBuilder _instance;
		public static ChunkCollisionBuilder instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.ScriptableObject.CreateInstance<ChunkCollisionBuilder>();
				}

				return _instance;
			}
		}

		private CollisionState state = CollisionState.Ready;

		private ConcurrentQueue<Coord3D> queue = new ConcurrentQueue<Coord3D>();

		private List<Vector3> centers = new List<Vector3>();
		private List<Vector3> sizes = new List<Vector3>();

		public CollisionState State()
		{
			return state;
		}

		public void setState(CollisionState _state)
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
				if (state == CollisionState.Stopped)
				{
					continue;
				}
				else if (state == CollisionState.Stopping)
				{
					state = CollisionState.Stopped;
					continue;
				}

				if (queue.Count == 0)
				{
					state = CollisionState.Waiting;
				}
				else
				{
					state = CollisionState.Building;

					Coord3D thisPosition;
					bool dequeued = queue.TryDequeue(out thisPosition);

					if (dequeued == false || thisPosition == null || thisPosition.voxelObject == null) { continue; }

					Chunk thisChunk = thisPosition.voxelObject.getChunk(thisPosition.x, thisPosition.y, thisPosition.z);

					if (thisChunk == null)
					{
						continue;
					}

					thisChunk.setCollisionState(CollisionState.Building);

					centers.Clear();
					sizes.Clear();

					//  Loop all blocks in this chunk
					for (int n = 0; n < thisChunk.getSize(); n++)
					{
						if (state == CollisionState.Stopped)
						{
							thisChunk.setCollisionState(CollisionState.Stopped);
							break;
						}

						Block thisBlock = thisChunk.raw(n);
						if (thisBlock == null)
						{
							continue;
						}
						else if (thisBlock.isPassable() == false)
						{
							switch (thisBlock.getModelType())
							{
								case ModelType.CUBE:
									if (thisBlock.isBlocked() == false)
									{
										centers.Add(new Vector3(thisBlock.getX(), thisBlock.getY(), thisBlock.getZ()) + (Vector3.one * 0.5f));
										sizes.Add(Vector3.one);
									}
									break;

								case ModelType.CROSS_SECTION_SMALL:
									if (thisBlock.isBlocked() == false)
									{
										centers.Add(new Vector3(thisBlock.getX(), thisBlock.getY(), thisBlock.getZ()) + (Vector3.one * 0.5f));
										sizes.Add(Vector3.one);
									}
									break;

								case ModelType.SLOPE:
									if (thisBlock.isBlocked() == false)
									{
										centers.Add(new Vector3(thisBlock.getX(), thisBlock.getY(), thisBlock.getZ()) + (Vector3.one * 0.5f));
										sizes.Add(Vector3.one);
									}
									break;

								case ModelType.CUSTOM:
									if (thisBlock.isBlocked() == false)
									{
										centers.Add(new Vector3(thisBlock.getX(), thisBlock.getY(), thisBlock.getZ()) + (Vector3.one * 0.5f));
										sizes.Add(Vector3.one);
									}
									break;

								case ModelType.CUSTOM_CUBE:
									if (thisBlock.isBlocked() == false)
									{
										centers.Add(new Vector3(thisBlock.getX(), thisBlock.getY(), thisBlock.getZ()) + (Vector3.one * 0.5f));
										sizes.Add(Vector3.one);
									}
									break;

								case ModelType.CUBE_HALF:
									if (thisBlock.isBlocked() == false)
									{
										centers.Add(new Vector3(thisBlock.getX(), thisBlock.getY(), thisBlock.getZ()) + (Vector3.one * 0.5f));
										sizes.Add(Vector3.one);
									}
									break;
							}
						}
					}

					if (state != CollisionState.Stopped)
					{
						thisChunk.PrepareCollisionBuffer(new CollisionData(centers, sizes));
						state = CollisionState.Waiting;
					}
				}

				updates++;	//	Increment update count
			}

			IsDone = true;
		}
	}
}