using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
	[Serializable]
	public class Chunk
	{
		public ChunkComponent component;

		[SerializeField]
		private VoxelObject	voxelObject;	//	The voxel object this chunk is in
		[SerializeField]
		private int		x, y, z;			//	Coordinates of this chunk
		[SerializeField]
		private bool	voidState;			//	Whether this chunk is void of any blocks
		[SerializeField]
		private int     voidBlockCount; 	//	How many void blocks are in this chunk

		private Block[]             blocks;			//	All blocks in this chunk
		private List<BehaviorBlock> behaviorBlocks; //	Only blocks with behavior in this chunk

		private RenderData 		renderData;		//	The render data of this chunk
		private CollisionData 	collisionData;	//	The collision data of this chunk

		[SerializeField]
		private ChunkState state 				= ChunkState.Unregistered;		//	The state of this chunk
		[SerializeField]
		private RenderState renderState 		= RenderState.Waiting;		//	The render state of this chunk
		[SerializeField]
		private CollisionState collisionState 	= CollisionState.Waiting;	//	The collision state of this chunk

		public Chunk(int _x, int _y, int _z, VoxelObject _voxelObject)
		{
			x = _x; y = _y; z = _z;
			voxelObject = _voxelObject;

			blocks 			= new Block[(Constants.CHUNK_SIZE * Constants.CHUNK_SIZE * Constants.CHUNK_SIZE)];
			behaviorBlocks 	= new List<BehaviorBlock>();

			voidBlockCount = blocks.Length;
			voidState = true;
		}



	//	Get methods // ///////////////////////////////

		public RenderState getRenderState()
		{
			return renderState;
		}

		public RenderData getRenderData()
		{
			return renderData;
		}

		public CollisionState getCollisionState()
		{
			return collisionState;
		}

		public CollisionData getCollisionData()
		{
			return collisionData;
		}

		public ChunkState getState()
		{
			return state;
		}

		public int getSize()
		{
			return blocks.Length;
		}

		public VoxelObject getVoxelObject()
		{
			return voxelObject;
		}

		public List<BehaviorBlock> getBehaviorBlocks()
		{
			return behaviorBlocks;
		}

		public bool isVoid()
		{
			return voidState;
		}

		public bool isSolid()
		{
			return (voidBlockCount == 0);
		}

		public int getX()
		{
			return x;
		}

		public int getY()
		{
			return y;
		}

		public int getZ()
		{
			return z;
		}

		public int getWorldX()
		{
			return x * Constants.CHUNK_SIZE;
		}

		public int getWorldY()
		{
			return y * Constants.CHUNK_SIZE;
		}

		public int getWorldZ()
		{
			return z * Constants.CHUNK_SIZE;
		}

		public Chunk relativeChunk(int _x, int _y, int _z)
		{
			return voxelObject.getChunk(x + _x, y + _y, z + _z);
		}

		public Chunk relativeChunk(int _dir)
		{
			switch (_dir)
			{
				case 0:
					return relativeChunk(0, 0, 1);

				case 1:
					return relativeChunk(1, 0, 0);

				case 2:
					return relativeChunk(0, 0, -1);

				case 3:
					return relativeChunk(-1, 0, 0);

				case 4:
					return relativeChunk(0, 1, 0);

				case 5:
					return relativeChunk(0, -1, 0);

				default:
					return null;
			}
		}

		public Chunk relativeChunk(Direction _dir)
		{
			return relativeChunk((int)_dir);
		}

		public Chunk[] getNeighbors()
		{
			return new Chunk[]
			{
				relativeChunk(0, 0, 1),
				relativeChunk(1, 0, 0),
				relativeChunk(0, 0, -1),
				relativeChunk(-1, 0, 0),
				relativeChunk(0, 1, 0),
				relativeChunk(0, -1, 0)
			};
		}

		public bool isCulled()
		{
			Chunk[] neighbors = getNeighbors();

			for (int i = 0; i < neighbors.Length; i++)
			{
				if (neighbors[i] != null)
				{
					if (neighbors[i].isVoid() == true || neighbors[i].isSolid() == false)
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public Block raw(int _index)
		{
			return blocks[_index];
		}

		public Block raw(int _x, int _y, int _z)
		{
			return blocks[toIndex(_x, _y, _z)];
		}

		public Block[] getRawData()
		{
			return blocks;
		}

		public Block at(int _index)
		{
			if (state != ChunkState.Loaded) { Load(); }

			Block thisBlock = blocks[(int)Mathf.Clamp(_index, 0, blocks.Length - 1)];

			if (thisBlock != null) { return thisBlock; } else { return Voxel.VOID.toBlock(); }
		}

		public Block at(int _x, int _y, int _z, bool _forceToLocal = false)
		{
			if (state != ChunkState.Loaded) { Load(); }

			if (_forceToLocal == true)
			{
				_x = toLocalSpace(_x);
				_y = toLocalSpace(_y);
				_z = toLocalSpace(_z);
			}
			else if (inBounds(_x, _y, _z) == false)
			{
				int offsetX = 0, offsetY = 0, offsetZ = 0;
				int overflowX = boundsOverflow(_x);
				int overflowY = boundsOverflow(_y);
				int overflowZ = boundsOverflow(_z);
				_x = toLocalSpace(_x);
				_y = toLocalSpace(_y);
				_z = toLocalSpace(_z);

				if (overflowX > 0) { offsetX = 1; } else if (overflowX < 0) { offsetX = -1; }
				if (overflowY > 0) { offsetY = 1; } else if (overflowY < 0) { offsetY = -1; }
				if (overflowZ > 0) { offsetZ = 1; } else if (overflowZ < 0) { offsetZ = -1; }

				Chunk overflowedChunk = relativeChunk(offsetX, offsetY, offsetZ);
				if (overflowedChunk != null)
				{
					return overflowedChunk.at(_x, _y, _z);
				}

				return Voxel.VOID.toBlock();
			}

			Block thisBlock = blocks[toIndex(_x, _y, _z)];

			if (thisBlock != null) { return thisBlock; } else { return Voxel.VOID.toBlock(); }
		}

		public Block highestAt(int _x, int _z, bool _forceToLocal = false)
		{
			if (_forceToLocal == true)
			{
				_x = toLocalSpace(_x);
				_z = toLocalSpace(_z);
			}

			for (int yPos = Constants.CHUNK_SIZE - 1; yPos > 0; yPos--)
			{
				Block thisBlock = raw(_x, yPos, _z);

				if (thisBlock != null)
				{
					return thisBlock;
				}
			}

			return Voxel.VOID.toBlock();
		}



	//	Set methods // ///////////////////////////////

		public void setRenderState(RenderState _renderState)
		{
			renderState = _renderState;
		}

		public void setCollisionState(CollisionState _collisionState)
		{
			collisionState = _collisionState;
		}

		public void setState(ChunkState _state)
		{
			state = _state;
		}

		public Block set(int _x, int _y, int _z, Block _block, bool _update = true, bool _forceToLocal = false)
		{
			if (state != ChunkState.Loaded) { Load(); }

			if (_forceToLocal == true)
			{
				_x = toLocalSpace(_x);
				_y = toLocalSpace(_y);
				_z = toLocalSpace(_z);
			}
			else if (inBounds(_x, _y, _z) == false)
			{
				int offsetX = 0, offsetY = 0, offsetZ = 0;
				int overflowX = boundsOverflow(_x);
				int overflowY = boundsOverflow(_y);
				int overflowZ = boundsOverflow(_z);

				_x = toLocalSpace(_x);
				_y = toLocalSpace(_y);
				_z = toLocalSpace(_z);

				if (overflowX > 0) { offsetX = 1; } else if (overflowX < 0) { offsetX = -1; }
				if (overflowY > 0) { offsetY = 1; } else if (overflowY < 0) { offsetY = -1; }
				if (overflowZ > 0) { offsetZ = 1; } else if (overflowZ < 0) { offsetZ = -1; }

				Chunk overflowedChunk = voxelObject.getChunk(x + offsetX, y + offsetY, z + offsetZ); //relativeChunk(offsetX, offsetY, offsetZ);
				if (overflowedChunk != null)
				{
					return overflowedChunk.set(_x, _y, _z, _block, _update);
				}
			}

			_block.setLocation(_x, _y, _z);
			_block.setChunk(this);

			renderState = RenderState.Waiting;
			collisionState = CollisionState.Waiting;

			if (_block.isOnBorder() == true)
			{
				Chunk[] neighbors = getNeighbors();
				for (int i = 0; i < neighbors.Length; i++)
				{
					Chunk thisNeighbor = neighbors[i];
					if (thisNeighbor != null)
					{
						thisNeighbor.setRenderState(RenderState.Waiting);
						thisNeighbor.setCollisionState(CollisionState.Waiting);
					}
				}
			}

			Block oldBlock = blocks[toIndex(_x, _y, _z)];

			if (_block is BehaviorBlock) { behaviorBlocks.Add((BehaviorBlock)_block); ((BehaviorBlock)_block).Initialize(); }
			if (oldBlock is BehaviorBlock) { behaviorBlocks.Remove((BehaviorBlock)oldBlock); }

			if (oldBlock != null)
			{
				oldBlock.OnRemove();
			}

			if (_block.getType() != Voxel.VOID)
			{
				_block.OnPlace();
			}

			blocks[toIndex(_x, _y, _z)] = _block;

			if (_update == true)
			{
				_block.Update();

				Block[] neighbors = _block.getNeighbors();

				for (int i = 0; i < neighbors.Length; i++)
				{
					Block thisNeighbor = neighbors[i];

					if (thisNeighbor != null)
					{
						thisNeighbor.Update();
					}
				}
			}

			if (_block.getType() == Voxel.VOID) { blocks[toIndex(_x, _y, _z)] = null; voidBlockCount++; } else { voidBlockCount--; }
			if (voidBlockCount >= blocks.Length) { voidBlockCount = blocks.Length; voidState = true; } else { voidState = false; }
			if (voidBlockCount < 0) { voidBlockCount = 0; }

			return _block;
		}

		public Block set(int _x, int _y, int _z, Voxel _voxel, bool _update = true, bool _forceToLocal = true)
		{
			return set(_x, _y, _z, _voxel.toBlock(), _update, _forceToLocal);
		}

		public void setRaw(int _index, Block _block)
		{
			_block.setLocation(fromIndex(_index));
			_block.setChunk(this);

			if (_block.getType() == Voxel.VOID) { _block = null; voidBlockCount++; } else { voidBlockCount--; }

			if (voidBlockCount >= blocks.Length) { voidBlockCount = blocks.Length; voidState = true; } else { voidState = false; }
			if (voidBlockCount < 0) { voidBlockCount = 0; }

			Block oldBlock = blocks[_index];

			if (_block is BehaviorBlock) { behaviorBlocks.Add((BehaviorBlock)_block); ((BehaviorBlock)_block).Initialize(); }
			if (oldBlock is BehaviorBlock) { behaviorBlocks.Remove((BehaviorBlock)oldBlock); }

			blocks[_index] = _block;

			renderState = RenderState.Waiting;
			collisionState = CollisionState.Waiting;
		}

		public void setRaw(int _x, int _y, int _z, Block _block)
		{
			setRaw(toIndex(_x, _y, _z), _block);
		}

		public void setRawData(Block[] _blocks)
		{
			wipe();

			for (int i = 0; i < _blocks.Length; i++)
			{
				Block thisBlock = _blocks[i];
				setRaw(i, thisBlock);
			}
		}

		public void wipe()
		{
			blocks 			= new Block[(Constants.CHUNK_SIZE * Constants.CHUNK_SIZE * Constants.CHUNK_SIZE)];
			behaviorBlocks 	= new List<BehaviorBlock>();

			voidBlockCount = blocks.Length;
			voidState = true;

			renderState = RenderState.Waiting;
			collisionState = CollisionState.Waiting;
		}



	//	Util methods // ///////////////////////////////

		public int toIndex(int _x, int _y, int _z)
		{
			int index = _z * Constants.CHUNK_SIZE * Constants.CHUNK_SIZE + _y * Constants.CHUNK_SIZE + _x;

			index = (int)Mathf.Clamp(index, 0, (float)Math.Pow(Constants.CHUNK_SIZE, 3) - 1);	//	Force into bounds

			return index;
		}

		public Coord3D fromIndex(int _index)	//	TODO: May not work?
		{
			int posZ = _index / (Constants.CHUNK_SIZE * Constants.CHUNK_SIZE);
			_index -= (posZ * Constants.CHUNK_SIZE * Constants.CHUNK_SIZE);
			int posY = _index / Constants.CHUNK_SIZE;
			int posX = _index % Constants.CHUNK_SIZE;

			return new Coord3D(posX, posY, posZ);
		}

		public bool inBounds(int _x, int _y, int _z)
		{
			if (_x < 0 || _y < 0 || _z < 0 || _x >= Constants.CHUNK_SIZE || _y >= Constants.CHUNK_SIZE || _z >= Constants.CHUNK_SIZE)
			{
				return false;
			}

			return true;
		}

		public bool inBounds(int _dimension)
		{
			return !(_dimension < 0 || _dimension >= Constants.CHUNK_SIZE);
		}

		public int boundsOverflow(int _dimension)
		{
			if (_dimension >= Constants.CHUNK_SIZE)
			{
				return _dimension - Constants.CHUNK_SIZE + 1;
			}
			else if (_dimension < 0)
			{
				return _dimension;
			}

			return 0;
		}

		public int toLocalSpace(int _dimension)
		{
			_dimension = _dimension - ( (_dimension >> 4) * (Constants.CHUNK_SIZE) );
			return _dimension;
		}



	//	Work methods // ///////////////////////////////

		public void MarkForRebuild()
		{
			renderState = RenderState.Waiting;
			collisionState = CollisionState.Waiting;
		}

		public void PrepareRenderBuffer(RenderData _renderData)
		{
			renderData = _renderData;
			renderState = RenderState.Ready;
		}

		public void ClearRenderBuffer()
		{
			renderData = null;
		}

		public RenderData Render()
		{
			if (renderData != null)
			{
				renderState = RenderState.Rendered;
			}
			else
			{
				renderState = RenderState.Failed;
			}

			return renderData;
		}

		public void PrepareCollisionBuffer(CollisionData _collisionData)
		{
			collisionData = _collisionData;
			collisionState = CollisionState.Ready;
		}

		public void ClearCollisionBuffer()
		{
			collisionData = null;
		}

		public CollisionData BuildCollision()
		{
			if (collisionData != null)
			{
				collisionState = CollisionState.Built;
			}
			else
			{
				collisionState = CollisionState.Failed;
			}

			return collisionData;
		}

		public void PrepareLoad()
		{
			state = ChunkState.Loading;
			voxelObject.getLoadingChunks().Enqueue(this);
		}

		public void Load()
		{
			if (state != ChunkState.Loaded)
			{
				state = ChunkState.Loaded;
				voxelObject.getLoadedChunks().Add(this);
			}
		}

		public void Unload()
		{
			if (state != ChunkState.Unloaded)
			{
				state = ChunkState.Unloaded;
				voxelObject.getLoadedChunks().Remove(this);
			}
		}

		public void Tick()
		{
			if (state != ChunkState.Loaded || voidState == true) { return; }	//	Don't tick an unloaded or void chunk

			//	Tick all tickable behavior blocks
			for (int i = 0; i < behaviorBlocks.Count; i++)
			{
				if (behaviorBlocks[i].isTickable() == true)
				{
					behaviorBlocks[i].Tick();
				}
			}

			// int xPos = voxelObject.getRandom().Next(Constants.CHUNK_SIZE);
			// int yPos = voxelObject.getRandom().Next(Constants.CHUNK_SIZE);
			// int zPos = voxelObject.getRandom().Next(Constants.CHUNK_SIZE);

			// Block thisBlock = at(xPos, yPos, zPos);
		}
	}
}