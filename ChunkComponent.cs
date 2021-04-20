using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Rendering;
using Swordfish;

[Serializable]
public class ChunkComponent
{
	public NativeArray<CompoundCollider.ColliderBlobInstance> colliders;
	public BlobAssetReference<Unity.Physics.Collider> collider;
	public Unity.Entities.Entity entity;
	public VoxelComponent voxelComponent;
	public Chunk chunk;

	public Coord3D position;

	public int renderUpdates = 0;
	public int collisionUpdates = 0;

	public MeshFilter meshFilter;
	public Mesh mesh;

	public int colliderIndex = 0;
	public bool pooledColliders = false;

	public bool waitingForBuild = false;

	public Transform transform;

	public ChunkComponent()
	{
		mesh = new Mesh();
		// mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //  Allow up to 4b verts
	}

	public void BuildChunk()
	{
		if (chunk == null)
		{
			Debug.Log("Null chunk!");
		}
		else
		{
			BuildCollision();
			BuildRender();
		}
	}

	public void BuildRender()
	{
		//	Potential lag point but for some reason chunks dont always have references to their voxel object
		chunk = voxelComponent.voxelObject.getChunk(position.x, position.y, position.z);

		if (chunk.getRenderState() == RenderState.Ready)	//	Render the chunk if it is ready
		{
			chunk.Render();

			if (chunk.getRenderState() == RenderState.Rendered)
			{
				mesh.Clear();
				mesh.vertices = chunk.getRenderData().vertices;
				mesh.triangles = chunk.getRenderData().triangles;
				mesh.normals = chunk.getRenderData().normals;
				mesh.uv = chunk.getRenderData().uvs;
				mesh.colors = chunk.getRenderData().colors;
				mesh.RecalculateNormals();

				renderUpdates++;
			}

			chunk.ClearRenderBuffer();
		}
		else if (chunk.getRenderState() == RenderState.Waiting)
		{
			if (voxelComponent != null)
			{
				ChunkRenderer.instance.Queue(position);
				chunk.setRenderState(RenderState.Queued);
			}
		}
	}

	public void BuildCollision()
	{
		//	Potential lag point but for some reason chunks dont always have references to their voxel object
		chunk = voxelComponent.voxelObject.getChunk(position.x, position.y, position.z);

		if (chunk.getCollisionState() == CollisionState.Ready)	//	Assign collision to this chunk if it is ready
		{
			chunk.BuildCollision();

			if (chunk.getCollisionState() == CollisionState.Built && chunk.getCollisionData().centers.Length > 0)
			{
				voxelComponent.TagCollisionRebuild();
				collisionUpdates++;
			}

			// chunk.ClearCollisionBuffer();
		}
		else if (chunk.getCollisionState() == CollisionState.Waiting)
		{
			if (voxelComponent != null)
			{
				chunk.setCollisionState(CollisionState.Queued);
				ChunkCollisionBuilder.instance.Queue(position);
			}
		}
	}
}
