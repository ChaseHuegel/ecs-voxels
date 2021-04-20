using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using UnityEngine;

namespace Swordfish
{
	[Serializable]
	public class VoxelObject
	{
		public VoxelComponent component;
		public int ticksAlive = 0;

		protected string name = "default_voxelObject";

		public int chunkSizeX;
		public int chunkSizeZ;
		public int chunkSizeY;

		public int getBlockSizeX() { return chunkSizeX * Constants.CHUNK_SIZE; }
		public int getBlockSizeY() { return chunkSizeY * Constants.CHUNK_SIZE; }
		public int getBlockSizeZ() { return chunkSizeZ * Constants.CHUNK_SIZE; }

		public bool isStatic;
		public bool loaded;
		public int unloadedChunks;

		//private List<Chunk>		chunks;
		private Dictionary<string, Chunk> chunks;
		private Queue<Chunk> 	chunkLoadQueue;
		private Queue<Chunk> 	chunkUnloadQueue;

		private System.Random 	random;
		private Guid guid;

		public VoxelObject(int _sizeX = 1, int _sizeY = 1, int _sizeZ = 1, VoxelComponent _component = null)
		{
			guid = Guid.NewGuid();
			GameMaster.Instance.voxelMaster.voxelObjects[guid] = this;

			chunkSizeX = _sizeX;
			chunkSizeY = _sizeY;
			chunkSizeZ = _sizeZ;

			component = _component;

			Initialize();
		}

		public VoxelObject(int _sizeX, int _sizeY, int _sizeZ, VoxelComponent _component, Guid _guid)
		{
			guid = _guid;
			GameMaster.Instance.voxelMaster.voxelObjects[guid] = this;

			chunkSizeX = _sizeX;
			chunkSizeY = _sizeY;
			chunkSizeZ = _sizeZ;

			component = _component;

			Initialize();
		}

		private void Initialize()
		{
			random = new System.Random( name.GetHashCode() );

			loaded = false;
			unloadedChunks = chunkSizeX * chunkSizeY * chunkSizeZ;

			//chunks              = new List<Chunk>();
			chunks              = new Dictionary<string, Chunk>();
			chunkLoadQueue      = new Queue<Chunk>();
            chunkUnloadQueue    = new Queue<Chunk>();
		}

		public void Reset()
		{
			foreach (KeyValuePair<string, Chunk> entry in chunks)
			{
				entry.Value.wipe();
			}

			random = new System.Random( name.GetHashCode() );

			unloadedChunks = chunkSizeX * chunkSizeY * chunkSizeZ;

			chunkLoadQueue.Clear();
			chunkUnloadQueue.Clear();
		}

		public Guid getGUID()
		{
			return guid;
		}

		public string getName()
		{
			return name;
		}

		public void setName(string _name)
		{
			name = _name;
			random = new System.Random( name.GetHashCode() );
		}

		public System.Random getRandom()
		{
			return random;
		}

		public void setRandomSeed(int _seed)
		{
			random = new System.Random( _seed );
		}

		public List<Chunk> getLoadedChunks()
        {
            return chunks.Values.ToList();
        }

        public Queue<Chunk> getLoadingChunks()
        {
            return chunkLoadQueue;
        }

        public Queue<Chunk> getUnloadingChunks()
        {
            return chunkUnloadQueue;
        }

		public List<BehaviorBlock> getBehaviorBlocks()
		{
			List<BehaviorBlock> behaviorBlocks = new List<BehaviorBlock>();

			foreach (KeyValuePair<string, Chunk> entry in chunks)
			{
				if (entry.Value.getBehaviorBlocks().Count > 0)
				{
					behaviorBlocks.AddRange(entry.Value.getBehaviorBlocks());
				}
			}

			return behaviorBlocks;
		}

		public Block getBlockAt(int _x, int _y, int _z)
		{
			Chunk thisChunk = getChunk(_x >> 4, _y >> 4, _z >> 4);

			if (thisChunk == null)
			{
				return Voxel.VOID.toBlock();
			}

			if (thisChunk.getState() == ChunkState.Loaded)
			{
				return thisChunk.at(_x, _y, _z, true);
			}

			return Voxel.VOID.toBlock();
		}

		public Block getBlockAt(Coord3D _coord)
		{
			return getBlockAt(_coord.x, _coord.y, _coord.z);
		}

		public Block getBlockAt(Location _location)
		{
			return getBlockAt((int)_location.x, (int)_location.y, (int)_location.z);
		}

		public Block getHighestBlockAt(int _x, int _y, int _z)
		{
			Chunk thisChunk = getChunk(_x >> 4, _y >> 4, _z >> 4);

			if (thisChunk == null)
			{
				return Voxel.VOID.toBlock();
			}

			if (thisChunk.getState() == ChunkState.Loaded)
			{
				return thisChunk.highestAt(_x, _z, true);
			}

			return Voxel.VOID.toBlock();
		}

		public Block getHighestBlockAt(Coord3D _coord)
		{
			return getHighestBlockAt(_coord.x, _coord.y, _coord.z);
		}

		public Block getHighestBlockAt(Location _location)
		{
			return getHighestBlockAt((int)_location.x, (int)_location.y, (int)_location.z);
		}

		public Chunk getChunk(int _x, int _y, int _z)
        {
			try
			{
				return chunks["/" + _x + "-" + _y + "-" + _z];
			}
			catch (KeyNotFoundException) { return null; }
        }

		public Chunk getChunkAt(int _x, int _y, int _z)
        {
			return getChunk(_x >> 4, _y >> 4, _z >> 4);
		}

		public Block setBlockAt(int _x, int _y, int _z, Block _block, bool _update = true)
		{
			Chunk thisChunk = getChunk(_x >> 4, _y >> 4, _z >> 4);

			if (thisChunk == null)
			{
				return Voxel.VOID.toBlock();
			}

			if (thisChunk.getState() == ChunkState.Loaded)
			{
				return thisChunk.set(_x, _y, _z, _block, _update, true);
			}

			return Voxel.VOID.toBlock();
		}

		public Block setBlockAt(Coord3D _coord, Block _block, bool _update = true)
		{
			return setBlockAt(_coord.x, _coord.y, _coord.z, _block, _update);
		}

		public Block setBlockAt(Location _location, Block _block, bool _update = true)
		{
			return setBlockAt((int)_location.x, (int)_location.y, (int)_location.z, _block, _update);
		}

		public Block setBlockAt(int _x, int _y, int _z, Voxel _voxel, bool _update = true)
		{
			return setBlockAt(_x, _y, _z, _voxel.toBlock(), _update);
		}

		public Block setBlockAt(Coord3D _coord, Voxel _voxel, bool _update = true)
		{
			return setBlockAt(_coord.x, _coord.y, _coord.z, _voxel.toBlock(), _update);
		}

		public Block setBlockAt(Location _location, Voxel _voxel, bool _update = true)
		{
			return setBlockAt((int)_location.x, (int)_location.y, (int)_location.z, _voxel.toBlock(), _update);
		}

		public Chunk GenerateChunk(int _x, int _y, int _z, bool _checkGenerated = true)
        {
			if (_checkGenerated == true)
			{
				Chunk checkedChunk = getChunk(_x, _y, _z);
				if (checkedChunk != null)
				{
					return checkedChunk;
				}
			}

            Chunk freshChunk = new Chunk(_x, _y, _z, this);

			//	Set blocks in the chunk

			chunks.Add("/" + _x + "-" + _y + "-" + _z, freshChunk);
			return freshChunk;
        }

		public virtual void FinishLoad()
		{
			this.setBlockAt(getBlockSizeX() / 2, getBlockSizeY() / 2, getBlockSizeZ() / 2, Voxel.SHIP_CORE);
			loaded = true;
		}

		public void Render()
		{
			for (int x = 0; x < chunkSizeX; x++)
			{
				for (int z = 0; z < chunkSizeZ; z++)
				{
					for (int y = 0; y < chunkSizeY; y++)
					{
						Chunk thisChunk = getChunk(x, y, z);
						if (thisChunk == null)
						{
							continue;
						}

						ChunkComponent thisComponent = thisChunk.component;

						if (thisComponent.mesh != null)
						{
							Vector3 position = (thisComponent.position.toVector3() * Constants.CHUNK_SIZE) + component.pivotPoint;

							position = component.transform.rotation * position;
							position += component.transform.position;

							foreach (UnityEngine.Material mat in GameMaster.Instance.voxelMaterials)
							{
								Graphics.DrawMesh(thisComponent.mesh, Matrix4x4.TRS(position, component.transform.rotation, component.transform.lossyScale), mat, 0);
							}
						}
					}
				}
			}
		}

		public void Update()
		{
			// for (int i = 0; i < chunkLoadQueue.Count; i++)
			if (chunkLoadQueue.Count > 0)
			{
				Chunk thisChunk = chunkLoadQueue.Dequeue();
				if (thisChunk != null)
				{
					unloadedChunks -= 1;
					thisChunk.Load();

					// UnityEngine.Transform obj =
					// 	component.transform.Find("Chunk:" + thisChunk.getX() + "-" + thisChunk.getY() + "-" + thisChunk.getZ());
					// ChunkComponent chunkComponent = ( obj == null ? null : obj.GetComponent<ChunkComponent>() );

					ChunkComponent chunkComponent = component.chunkComponents[thisChunk.getX(), thisChunk.getY(), thisChunk.getZ()];

					if (chunkComponent != null) { chunkComponent.chunk = thisChunk; }

					if (unloadedChunks == 0) { FinishLoad(); }
				}
			}
		}

		public void Tick()
		{
			ticksAlive++;

			if (loaded == false && unloadedChunks == 0)
			{
				FinishLoad();
			}

            foreach (KeyValuePair<string, Chunk> entry in chunks)
            {
                Chunk thisChunk = entry.Value;

				if (thisChunk != null)
				{
					thisChunk.Tick();
				}
				else
				{
					UnityEngine.Debug.Log("Null chunk!");
				}
			}
		}
	}
}