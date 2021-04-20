using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Rendering;
using Swordfish;

public class VoxelComponent : MonoBehaviour
{
	public Unity.Entities.Entity entity = Unity.Entities.Entity.Null;
	public VoxelObject voxelObject = null;
	public Vector3 pivotPoint = Vector3.zero;

	public bool seedFromName = true;

	public bool initializeOnStart = false;
    public bool reinitialize = false;

    public bool reload = false;

	public bool physicsRebuildWaiting = false;
	public bool buildPhysics = false;

	public ulong tickRate = 25;
	public ulong ticks = 0;
	public ulong tickTime = 0;

	public int chunksWaitingForBuild = 0;

	public int sizeX = 4;
	public int sizeZ = 4;
    public int sizeY = 4;

	public bool isStatic = false;
	public bool canTick = false;

	public GameObject[,,] chunkObjects;
	public ChunkComponent[,,] chunkComponents;

	public Queue<ChunkComponent> buildingChunks = new Queue<ChunkComponent>();

	public VoxelObjectType type = VoxelObjectType.GENERIC;

	public bool loaded = false;
	public bool initialized = false;

	public void Start()
	{
		if (initializeOnStart == true)
		{
			reinitialize = false;
			Initialize(type);
		}
	}

	public void Update()
	{
		if (voxelObject != null)
		{
			voxelObject.Update();
		}
	}

	public void LateUpdate()
	{
		if (voxelObject != null)
		{
			CheckBuildChunk();
			TryBuildChunk();
			voxelObject.Render();
		}
	}

	public void setName(string _name)
	{
		gameObject.name = _name;
		voxelObject.setName(_name);
	}

	public void setSize(int _size)
	{
		sizeX = _size;
		sizeY = _size;
		sizeZ = _size;
	}

	public void setStatic(bool _static)
	{
		isStatic = _static;
		if (voxelObject != null) { voxelObject.isStatic = _static; }
		canTick = !isStatic;
	}

	public VoxelComponent ChangeType(VoxelObjectType _type)
	{
		return Initialize(_type);
	}

	public Unity.Entities.Entity CreateEntity()
	{
		BoxGeometry box = new BoxGeometry
		{
			Center = float3.zero,
			Orientation = quaternion.identity,
			Size = new float3(1),
			BevelRadius = 0.0f
		};

		entity = GameMaster.Instance.CreateBody(this.transform.position, this.transform.rotation, Unity.Physics.BoxCollider.Create(box), Vector3.zero, Vector3.zero, 1.0f, !isStatic);

		return entity;
	}

	public void TagCollisionRebuild()
	{
		buildPhysics = true;
	}

	public void RebuildCollision()
	{
		List<ChunkComponent> chunkList = new List<ChunkComponent>();
		int colliderCount = 0;
		for (int x = 0; x < sizeX; x++)
		{
			for (int z = 0; z < sizeZ; z++)
			{
				for (int y = 0; y< sizeY; y++)
				{
					if (chunkComponents[x, y, z] != null || chunkComponents[x, y, z].chunk.getCollisionData() != null)
					{
						chunkList.Add(chunkComponents[x, y, z]);
						colliderCount += chunkComponents[x, y, z].chunk.getCollisionData().Count;
					}
				}
			}
		}

		BoxGeometry box = new BoxGeometry
		{
			Center = float3.zero,
			Orientation = quaternion.identity,
			Size = new float3(0.9f),
			BevelRadius = 0.0f
		};

		Vector3 position = Vector3.zero;
		quaternion rotation = quaternion.identity;
		Chunk chunk = null;
		int colliderIndex = 0;

		NativeArray<CompoundCollider.ColliderBlobInstance> colliders = new NativeArray<CompoundCollider.ColliderBlobInstance>(colliderCount, Allocator.Temp);

		for (int i = 0; i < chunkList.Count; i++)
		{
			chunk = chunkList[i].chunk;

			for (int n = 0; n < chunk.getCollisionData().Count; n++)
			{
				float3 center = chunk.getCollisionData().centers[n] + (new Vector3( chunk.getX(), chunk.getY(), chunk.getZ() ) * Constants.CHUNK_SIZE) + pivotPoint;
				box.Size = chunk.getCollisionData().sizes[n];

				colliders[colliderIndex] = new CompoundCollider.ColliderBlobInstance
				{
					CompoundFromChild = new RigidTransform(quaternion.identity, center),
					Collider = Unity.Physics.BoxCollider.Create(box)
				};

				colliderIndex++;
			}
		}

		BlobAssetReference<Unity.Physics.Collider> collider = CompoundCollider.Create(colliders);
		GameMaster.Instance.entityManager.SetComponentData(entity, new PhysicsCollider { Value = collider });

		if (isStatic == false) { GameMaster.Instance.SetEntityMass(entity, collider, colliders.Length); }

		colliders.Dispose();
	}

	public void TagForReload()
	{
		loaded = false;
		voxelObject.loaded = false;
		for (int x = 0; x < sizeX; x++)
		{
			for (int z = 0; z < sizeZ; z++)
			{
				for (int y = sizeY - 1; y >= 0; y--)
				{
					ChunkLoader.instance.Queue(chunkComponents[x, y, z]);
				}
			}
		}
	}

	public VoxelComponent Initialize(VoxelObjectType _type = VoxelObjectType.GENERIC)
	{
		if (entity == Unity.Entities.Entity.Null)
		{
			CreateEntity();
			this.GetComponent<EntityTracker>().SetReceivedEntity(entity);
		}

		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		type = _type;
		switch (type)
		{
			case VoxelObjectType.GENERIC:
				voxelObject = new VoxelObject(sizeX, sizeY, sizeZ, this);
				break;

			case VoxelObjectType.ASTEROID:
				voxelObject = new Asteroid(sizeX, sizeY, sizeZ, this);
				break;

			case VoxelObjectType.PLANETOID:
				voxelObject = new Planetoid(sizeX, sizeY, sizeZ, this);
				break;
		}

		voxelObject.isStatic = isStatic;
		voxelObject.setName(gameObject.name);

		if (seedFromName == false)
		{
			voxelObject.setRandomSeed(voxelObject.getGUID().GetHashCode());
		}

		chunkObjects = new GameObject[sizeX, sizeY, sizeZ];
		chunkComponents = new ChunkComponent[sizeX, sizeY, sizeZ];

		pivotPoint = new Vector3(sizeX * Constants.CHUNK_SIZE, sizeY * Constants.CHUNK_SIZE, sizeZ * Constants.CHUNK_SIZE) * 0.5f;
        pivotPoint += (Vector3.one * 0.5f);
		pivotPoint *= -1;

		for (int x = 0; x < sizeX; x++)
		{
			for (int z = 0; z < sizeZ; z++)
			{
				for (int y = sizeY - 1; y >= 0; y--)
				{
					ChunkComponent chunkComponent = new ChunkComponent();
					chunkComponent.transform = this.transform;
					chunkComponent.position = new Coord3D(x, y, z, voxelObject);
					chunkComponent.voxelComponent = this;
					chunkComponents[x, y, z] = chunkComponent;

					ChunkLoader.instance.Queue(chunkComponent);
				}
			}
		}

		reinitialize = false;
		initialized = true;
		return this;
	}

	public bool TryBuildChunk()
	{
		if (initialized == true)
		{
			if (buildingChunks.Count > 0)
			{
				ChunkComponent thisComponent = buildingChunks.Dequeue();
				if (thisComponent != null) { thisComponent.waitingForBuild = false; thisComponent.BuildRender(); Debug.Log("built render"); return true; }
			}
		}

		return false;
	}

	public void CheckBuildChunk()
	{
		if (initialized == true)
		{
			// if (buildingChunks.Count > 0)
			// {
			// 	ChunkComponent thisComponent = buildingChunks.Dequeue();
			// 	if (thisComponent != null) { thisComponent.BuildChunk(); }

			// 	// for (int i = 0; i < 4; i++)
			// 	// {
			// 	// 	if (buildingChunks.Count > 0)
			// 	// 	{
			// 	// 		ChunkComponent thisComponent = buildingChunks.Dequeue();
			// 	// 		if (thisComponent != null) { thisComponent.BuildChunk(); }
			// 	// 	}
			// 	// }
			// }

			for (int x = 0; x < sizeX; x++)
			{
				for (int z = 0; z < sizeZ; z++)
				{
					for (int y = 0; y < sizeY; y++)
					{
						ChunkComponent thisComponent = chunkComponents[x, y, z];

						if (thisComponent == null) { Debug.Log("Null component!"); return; }
						if (thisComponent.chunk == null) { Debug.Log("Null chunk!"); return; }

						if (thisComponent.chunk != null && thisComponent.waitingForBuild == false)
						{
							if (thisComponent.chunk.getCollisionState() != CollisionState.Built || thisComponent.chunk.getRenderState() != RenderState.Rendered)
							{
								thisComponent.waitingForBuild = true;
								ChunkLoader.instance.Queue(thisComponent);
								buildingChunks.Enqueue(thisComponent);//thisComponent.BuildChunk();
							}
						}
					}
				}
			}

			chunksWaitingForBuild = buildingChunks.Count;
		}
	}

	public void Tick()
	{
		if (loaded == false && voxelObject.loaded == true)
		{
			loaded = true;
			Util.LoadVoxelObject(voxelObject, voxelObject.getName());

			TagCollisionRebuild();
		}

		if (physicsRebuildWaiting == false && buildingChunks.Count > 0)
		{
			physicsRebuildWaiting = true;
		}

		if (physicsRebuildWaiting == true && buildingChunks.Count == 0)
		{
			physicsRebuildWaiting = false;
			TagCollisionRebuild();
		}

		if (buildPhysics == true)
		{
			buildPhysics = false;
			RebuildCollision();
		}

		if (reload == true)
		{
			reload = false;
			TagForReload();
		}

		if (reinitialize == true)
		{
			reinitialize = false;
			Initialize(type);
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (Application.platform != RuntimePlatform.WindowsEditor) { return; }

		Matrix4x4 rotationMatrix;

		if (Application.isPlaying == true && initialized == true)
		{
			for (int x = 0; x < sizeX; x++)
			{
				for (int z = 0; z < sizeZ; z++)
				{
					for (int y = 0; y < sizeY; y++)
					{
						ChunkComponent thisComponent = chunkComponents[x, y, z];
						Chunk thisChunk = thisComponent.chunk;

						if (thisChunk != null)
						{
							if (thisChunk.isVoid() == false && thisChunk.isSolid() == false)
							{
								rotationMatrix = Matrix4x4.TRS(transform.TransformPoint(thisComponent.transform.localPosition), transform.rotation, transform.lossyScale);
								Gizmos.matrix = rotationMatrix;
								Gizmos.color = Color.green;
								Gizmos.DrawWireCube((Vector3.one * (Constants.CHUNK_SIZE * 0.5f)), new Vector3(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.CHUNK_SIZE));
							}
							else if (thisChunk.isSolid() == true)
							{
								rotationMatrix = Matrix4x4.TRS(transform.TransformPoint(thisComponent.transform.localPosition), transform.rotation, transform.lossyScale);
								Gizmos.matrix = rotationMatrix;
								Gizmos.color = Color.blue;
								Gizmos.DrawWireCube((Vector3.one * (Constants.CHUNK_SIZE * 0.5f)), new Vector3(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.CHUNK_SIZE));
							}
						}
					}
				}
			}
		}
		else
		{
			pivotPoint = new Vector3(sizeX * Constants.CHUNK_SIZE, sizeY * Constants.CHUNK_SIZE, sizeZ * Constants.CHUNK_SIZE) * 0.5f;
			pivotPoint += (Vector3.one * 0.5f);
			pivotPoint *= -1;

			for (int x = 0; x < sizeX; x++)
			{
				for (int z = 0; z < sizeZ; z++)
				{
					for (int y = 0; y < sizeY; y++)
					{
						Vector3 chunkPos = new Vector3(x * Constants.CHUNK_SIZE, y * Constants.CHUNK_SIZE, z * Constants.CHUNK_SIZE);
                    	chunkPos += pivotPoint;
						rotationMatrix = Matrix4x4.TRS(transform.TransformPoint(chunkPos), transform.rotation, transform.lossyScale);
						Gizmos.matrix = rotationMatrix;
						Gizmos.color = Color.red;
						Gizmos.DrawWireCube((Vector3.one * (Constants.CHUNK_SIZE * 0.5f)), new Vector3(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.CHUNK_SIZE));
					}
				}
			}
		}

        Gizmos.color = Color.yellow;
		rotationMatrix = Matrix4x4.TRS(transform.TransformPoint(Vector3.zero), transform.rotation, transform.lossyScale);
		Gizmos.matrix = rotationMatrix;
		Gizmos.DrawWireCube(-(Vector3.one * 0.5f),
            new Vector3(Constants.CHUNK_SIZE * sizeX, Constants.CHUNK_SIZE * sizeY, Constants.CHUNK_SIZE * sizeZ)
            );
	}
}