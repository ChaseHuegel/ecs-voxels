using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Rendering;
using Swordfish;
using Swordfish.items;

internal sealed class NativeMethods
{
    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();
}

public class GameMaster : MonoBehaviour
{
	public bool debug = false;
	public int asteroidSpawnCount = 10;

	public bool reinitialize = false;

	public VoxelMaster voxelMaster;
	public ulong tickRate = 25;
	public ulong ticks = 0;
	public ulong tickTime = 0;

	public int loadQueue = 0;
	public int renderQueue = 0;
	public int collisionQueue = 0;

	public UnityEngine.Material[] voxelMaterials;
	public UnityEngine.Material[] thumbnailMaterials;
	public UnityEngine.Material[] selectionMaterials;
	public Mesh[] models;
	public Texture2D[] images;

	public CachedModel[] cachedModels;
	public CachedImage[] cachedImages;
	public CachedImage[] itemTextures;
	public CachedImage[] voxelTextures;
	public CachedImage[] voxelThumbnails;

	private int frameCount = 0;
 	private float dt = 0.0f;
	private float fps = 0.0f;
	private int updateRate = 4;

	private float renderRate = 0.0f;
	private float collisionRate = 0.0f;
	private float loadRate = 0.0f;

	public int rotations = 0;

	public PlayerMotor player;

    public EntityManager entityManager;

	public GameObject voxelEntityPrefab;
	public GameObject voxelObjectPrefab;
	public GameObject voxelColliderPrefab;
	public GameObject droppedItemPrefab;

	public AudioSource audioPlayer;
	public AudioClip placeSound;
	public AudioClip pickupSound;

	public Dictionary<Unity.Entities.Entity, Transform> entitiesToObjectMap;

	//  Keep this object alive
    private static GameMaster _instance;
    public static GameMaster Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameMaster").GetComponent<GameMaster>();
            }

            return _instance;
        }
    }

	public void Start()
	{
		GameMaster.Instance.Initialize();
	}

	public void OnGUI()
	{
		if (debug == true)
		{
			GUILayout.Label("FPS: " + (int)fps);
			GUILayout.Label("Render Thread: " + ChunkRenderer.instance.State() + " / " + renderQueue + " / TPS: " + renderRate);
			GUILayout.Label("Collision Thread: " + ChunkCollisionBuilder.instance.State() + " / " + collisionQueue + " / TPS: " + collisionRate);
			GUILayout.Label("Load Thread: " + ChunkLoader.instance.State() + " / " + loadQueue + " / TPS: " + loadRate);
		}
	}

	public void Initialize()
	{

		Console.WriteLine("Initializing GameMaster...");

        entityManager = World.Active.EntityManager;
		entitiesToObjectMap = new Dictionary<Unity.Entities.Entity, Transform>();

		voxelMaster = new VoxelMaster();

		LoadResources();

		ChunkRenderer.instance.Abort();
		ChunkRenderer.instance.ClearQueue();

		ChunkCollisionBuilder.instance.Abort();
		ChunkCollisionBuilder.instance.ClearQueue();

		ChunkLoader.instance.Abort();
		ChunkLoader.instance.ClearQueue();

		//GC.Collect();

		ChunkRenderer.instance.setState(RenderState.Ready);
		ChunkRenderer.instance.Start();

		ChunkCollisionBuilder.instance.setState(CollisionState.Ready);
		ChunkCollisionBuilder.instance.Start();

		ChunkLoader.instance.setState(ChunkState.Ready);
		ChunkLoader.instance.Start();
	}

	public void OnApplicationQuit()
	{
		ChunkRenderer.instance.Abort();
		ChunkCollisionBuilder.instance.Abort();
		ChunkLoader.instance.Abort();
	}

	public void FixedUpdate()
	{
		tickTime++;

		if (tickTime % tickRate == 0 && tickTime != 0)
		{
			voxelMaster.Tick();

			ticks++;
			tickTime = 0;
		}
	}

	// public void LateUpdate()
	// {
	// 	voxelMaster.Render();
	// }

	public void Update()
	{
		// voxelMaster.Update();

		if (Time.frameCount % 60 == 0)
		{
			GC.Collect();
		}

		if (Input.GetKeyDown(KeyCode.Escape) == true)
		{
			Application.Quit();
		}

		if (Input.GetKeyDown(KeyCode.F1) == true)
		{
			SpawnVoxelObject(Swordfish.Position.fromVector3(player.transform.position));
		}

		if (Input.GetKeyDown(KeyCode.F2) == true)
		{
			SpawnAsteroid(Swordfish.Position.fromVector3(player.transform.position));
		}

		if (Input.GetKeyDown(KeyCode.F3) == true)
		{
			debug = !debug;
		}

		if (Input.GetKeyDown(KeyCode.F4) == true)
		{
			LoadResources();
		}

		if (Input.GetKeyDown(KeyCode.F5) == true)
		{
			SpawnAsteroid( new Swordfish.Position(UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000)) );
		}

		if (Input.GetKeyDown(KeyCode.F6) == true)
		{
			for (int i = 0; i < asteroidSpawnCount; i++)
			{
				SpawnAsteroid( new Swordfish.Position(UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000)) );
			}
		}

		if (reinitialize == true)
		{
			reinitialize = false;
			Initialize();
		}

		renderQueue = ChunkRenderer.instance.QueueSize();
		collisionQueue = ChunkCollisionBuilder.instance.QueueSize();
		loadQueue = ChunkLoader.instance.QueueSize();

		frameCount++;
		dt += Time.deltaTime;
		if (dt > 1.0f/updateRate)
		{
			fps = frameCount / dt;
			frameCount = 0;

			if (ChunkRenderer.instance.State() != RenderState.Waiting)
			{
				renderRate = ChunkRenderer.instance.updates / dt;
			}

			if (ChunkCollisionBuilder.instance.State() != CollisionState.Waiting)
			{
				collisionRate = ChunkCollisionBuilder.instance.updates / dt;
			}

			if (ChunkLoader.instance.State() != ChunkState.Waiting)
			{
				loadRate = ChunkLoader.instance.updates / dt;
			}

			ChunkRenderer.instance.updates = 0;
			ChunkCollisionBuilder.instance.updates = 0;
			ChunkLoader.instance.updates = 0;

			dt -= 1.0f/updateRate;
		}
	}

	public int getFPS()
	{
		return (int)fps;
	}

	public CachedModel getCachedModel(string _name)
	{
		for (int i = 0; i < cachedModels.Length; i++)
		{
			if (cachedModels[i].name == _name)
			{
				return cachedModels[i];
			}
		}

		return null;
	}

	public CachedImage getCachedImage(string _name)
	{
		for (int i = 0; i < voxelThumbnails.Length; i++)
		{
			if (voxelThumbnails[i].name == _name)
			{
				return voxelThumbnails[i];
			}
		}

		return null;
	}

	public void LoadResources()
	{
		ResourceManager.Load();

		cachedModels = new CachedModel[models.Length];
		for (int i = 0; i < models.Length; i++)
		{
			cachedModels[i] = new CachedModel(models[i].vertices, models[i].normals, models[i].triangles, models[i].uv, models[i].colors, models[i].name);
		}

		//	Setup material textures
		foreach (UnityEngine.Material mat in voxelMaterials)
		{
			mat.SetTexture("_albedo", ResourceManager.GetImage("atlas.metal_01").image.texture);
			mat.SetTexture("_emission", ResourceManager.GetImage("atlas.metal_01_e").image.texture);
			mat.SetTexture("_metallic", ResourceManager.GetImage("atlas.metal_01_m").image.texture);
		}

		foreach (UnityEngine.Material mat in thumbnailMaterials)
		{
			mat.SetTexture("_UnlitColorMap", ResourceManager.GetImage("atlas.metal_01").image.texture);
		}

		foreach (UnityEngine.Material mat in selectionMaterials)
		{
			mat.SetTexture("_UnlitColorMap", ResourceManager.GetImage("atlas.metal_01").image.texture);
		}

		CreateVoxelThumbnails();
	}

	public void CreateVoxelThumbnails()
	{
		Voxel[] voxels = (Voxel[])Enum.GetValues(typeof(Voxel));
		voxelThumbnails = new CachedImage[voxels.Length];
		for(int i = 0; i < voxels.Length; i++)
		{
			Voxel voxel = voxels[i];
			Block thisBlock = voxel.toBlock();

			int width = 512, height = 512;
			Texture2D result = new Texture2D(width, height);
			Camera renderCamera = new GameObject().AddComponent<Camera>();
			renderCamera.transform.position = new Vector3(-1, 1, -1);
			renderCamera.transform.LookAt(Vector3.zero);
			renderCamera.orthographic = true;
			renderCamera.orthographicSize = 1.0f;
			renderCamera.clearFlags = CameraClearFlags.SolidColor;
			renderCamera.backgroundColor = Color.green;
			// renderCamera.allowMSAA = false;

			RenderTexture temp = RenderTexture.active;
			RenderTexture renderTex = RenderTexture.GetTemporary( width, height, 32 );
			RenderTexture.active = renderTex;
			renderCamera.targetTexture = renderTex;
			// GL.Clear(true, true, Color.green);

			Mesh mesh = ModelBuilder.GetVoxelMesh(voxel);

			Swordfish.Position offset = thisBlock.getViewOffset();
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(new Vector3(offset.x, offset.y, offset.z), Quaternion.identity, Vector3.one), thumbnailMaterials[0], 0, renderCamera);

			renderCamera.Render();
			renderCamera.targetTexture = null;

			result = new Texture2D( width, height, TextureFormat.RGBA32, false );
			result.ReadPixels( new Rect( 0, 0, width, height ), 0, 0, false );
			result.Apply( false, false );

			RenderTexture.active = temp;
			RenderTexture.ReleaseTemporary( renderTex );

			Color clearColor = new Color(0, 0, 0, 0);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < width; y++)
				{
					if (result.GetPixel(x, y).g >= 0.7f && result.GetPixel(x, y).r <= 0.4f && result.GetPixel(x, y).b <= 0.4f)
					{
						clearColor = result.GetPixel(x, y);
						clearColor.a = 0.0f;
						result.SetPixel(x, y, clearColor);
					}
				}
			}
			result.Apply( false, false );
			result.filterMode = FilterMode.Point;

			voxelThumbnails[i] = new CachedImage(result, voxel.ToString());
			Destroy(renderCamera.gameObject);
		}
	}

	public unsafe void SetEntityMass(Unity.Entities.Entity _entity, BlobAssetReference<Unity.Physics.Collider> _collider, float _mass)
	{
		Unity.Physics.Collider* colliderPtr = (Unity.Physics.Collider*)_collider.GetUnsafePtr();
		entityManager.SetComponentData(_entity, PhysicsMass.CreateDynamic(colliderPtr->MassProperties, _mass));
	}

	public unsafe void DestroyEntity(Unity.Entities.Entity _entity)
	{
		entityManager.DestroyEntity(_entity);
	}

	public unsafe Unity.Entities.Entity CreateEntity(float3 _position, quaternion _orientation, float _scale, RenderMesh _renderer)
	{
		ComponentType[] componentTypes = new ComponentType[5];

		componentTypes[0] = typeof(Translation);
		componentTypes[1] = typeof(LocalToWorld);
		componentTypes[2] = typeof(Rotation);
		componentTypes[3] = typeof(Scale);
		componentTypes[4] = typeof(RenderMesh);

		Unity.Entities.Entity entity = entityManager.CreateEntity(componentTypes);

		if (entityManager.HasComponent(entity, typeof(RenderMesh)))
		{
			entityManager.SetSharedComponentData(entity, _renderer);
		}
		else
		{
			entityManager.AddSharedComponentData(entity, _renderer);
		}

		if (entityManager.HasComponent(entity, typeof(Translation)))
		{
			entityManager.SetComponentData(entity, new Translation { Value = _position });
		}
		else
		{
			entityManager.AddComponentData(entity, new Translation { Value = _position });
		}

		if (entityManager.HasComponent(entity, typeof(Rotation)))
		{
			entityManager.SetComponentData(entity, new Rotation { Value = _orientation });
		}
		else
		{
			entityManager.AddComponentData(entity, new Rotation { Value = _orientation });
		}

		if (entityManager.HasComponent(entity, typeof(Scale)))
		{
			entityManager.SetComponentData(entity, new Scale { Value = _scale });
		}
		else
		{
			entityManager.AddComponentData(entity, new Scale { Value = _scale });
		}

		// entityManager.SetSharedComponentData(entity, new RenderMesh { mesh = _mesh, material = _material, castShadows = UnityEngine.Rendering.ShadowCastingMode.Off, receiveShadows = false });

		return entity;
	}

	public unsafe Unity.Entities.Entity CreateBody(float3 position, quaternion orientation,
        BlobAssetReference<Unity.Physics.Collider> collider,
        float3 linearVelocity, float3 angularVelocity, float mass, bool isDynamic)
	{
		ComponentType[] componentTypes = new ComponentType[isDynamic ? 7 : 3];

		componentTypes[0] = typeof(TranslationProxy);
		componentTypes[1] = typeof(RotationProxy);
		componentTypes[2] = typeof(PhysicsCollider);
		if (isDynamic)
		{
			componentTypes[3] = typeof(PhysicsVelocity);
			componentTypes[4] = typeof(PhysicsMass);
			componentTypes[5] = typeof(PhysicsDamping);
			componentTypes[6] = typeof(PhysicsGravityFactor);
		}

		Unity.Entities.Entity entity = entityManager.CreateEntity(componentTypes);

		entityManager.AddComponentData(entity, new Translation { Value = position });
		entityManager.AddComponentData(entity, new Rotation { Value = orientation });
		entityManager.SetComponentData(entity, new PhysicsCollider { Value = collider });
		if (isDynamic)
		{
			Unity.Physics.Collider* colliderPtr = (Unity.Physics.Collider*)collider.GetUnsafePtr();
			entityManager.SetComponentData(entity, PhysicsMass.CreateDynamic(colliderPtr->MassProperties, mass));

			float3 angularVelocityLocal = math.mul(math.inverse(colliderPtr->MassProperties.MassDistribution.Transform.rot), angularVelocity);
			entityManager.SetComponentData(entity, new PhysicsVelocity()
			{
				Linear = linearVelocity,
				Angular = angularVelocityLocal
			});
			entityManager.SetComponentData(entity, new PhysicsDamping()
			{
				Linear = 1.5f,
				Angular = 1.5f
			});

			entityManager.SetComponentData(entity, new PhysicsGravityFactor { Value = 0.0f });
		}

		return entity;
	}

	public void MergeEntitiesTogether(Unity.Entities.Entity _parent, Unity.Entities.Entity _child)
    {
        if (entityManager.HasComponent(_child, typeof(Parent)) == false)
        {
            entityManager.AddComponentData(_child, new Parent { Value = _parent });
        }
		else
		{
            entityManager.SetComponentData(_child, new Parent { Value = _parent });
		}

		if (entityManager.HasComponent(_child, typeof(LocalToParent)) == false)
        {
            entityManager.AddComponentData(_child, new LocalToParent() );
        }
		else
		{
            entityManager.SetComponentData(_child, new LocalToParent());
		}

		DynamicBuffer<LinkedEntityGroup> buf = entityManager.GetBuffer<LinkedEntityGroup>(_parent);
        buf.Add(_child);
    }

	public static bool Raycast(UnityEngine.Ray ray, float distance, out Unity.Physics.RaycastHit outHit, out Unity.Entities.Entity entity)
	{
		return Raycast(ray.origin, ray.origin + (ray.direction * distance), out outHit, out entity);
	}

	public static bool Raycast(float3 RayFrom, float3 RayTo, out Unity.Physics.RaycastHit outHit, out Unity.Entities.Entity entity)
	{
		var physicsWorldSystem = Unity.Entities.World.Active.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
		var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
		RaycastInput input = new RaycastInput()
		{
			Start = RayFrom,
			End = RayTo,
			Filter = new CollisionFilter()
			{
				BelongsTo = ~0u,
				CollidesWith = ~0u, // all 1s, so all layers, collide with everything
				GroupIndex = 0
			}
		};

		Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();
		bool haveHit = collisionWorld.CastRay(input, out hit);

		outHit = hit;
		entity = Unity.Entities.Entity.Null;

		if (haveHit)
		{
			// see hit.Position
			// see hit.SurfaceNormal
			entity = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
			return true;
		}

		return false;
	}

	public void PlaySound(AudioClip _clip, float _volume = 1.0f)
	{
		audioPlayer.PlayOneShot(_clip, _volume);
	}

	public void PlaySound(AudioClip _clip, Vector3 _position, float _volume = 1.0f)
	{
		AudioSource.PlayClipAtPoint(_clip, _position, _volume);
	}

	public Item DropItemNaturally(Swordfish.Position _position, Voxel _voxel, int _count = 1)
	{
		return ((BLOCKITEM)DropItemNaturally(_position, _voxel.toItem(), _count)).setVoxel(_voxel);
	}

	public Item DropItemNaturally(Swordfish.Position _position, ItemType _type, int _count = 1)
	{
		return DropItemNaturally(_position, _type.toItem(), _count);
	}

	public Item DropItemNaturally(Swordfish.Position _position, Item _item, int _count = 1)
	{
		Vector3 offset = new Vector3( UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f ) * 0.5f;
		GameObject temp = (GameObject)Instantiate(droppedItemPrefab, _position.toVector3() + offset, Quaternion.identity);
		DroppedItem dropped = temp.GetComponent<DroppedItem>();
		Item item = _item.copy();
		item.setAmount(_count);
		dropped.setItem(item);
		return item;
	}

	public VoxelObject SpawnVoxelObject(Swordfish.Position _position)
	{
		VoxelComponent component = Instantiate(voxelObjectPrefab, _position.toVector3(), Quaternion.identity).GetComponent<VoxelComponent>();
		component.setName("voxelObject" + voxelMaster.voxelObjects.Count);
		component.setSize(2);
		component.setStatic(false);
		component.Initialize(VoxelObjectType.GENERIC);
		return component.voxelObject;
	}

	public VoxelObject SpawnAsteroid(Swordfish.Position _position)
	{
		VoxelComponent component = Instantiate(voxelObjectPrefab, _position.toVector3(), Quaternion.identity).GetComponent<VoxelComponent>();
		component.setName("asteroid" + voxelMaster.voxelObjects.Count);
		component.setSize(4);
		component.setStatic(true);
		component.Initialize(VoxelObjectType.ASTEROID);
		return component.voxelObject;
	}
}
