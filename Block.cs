using System;

namespace Swordfish
{
    public class Block
    {
		protected int x = 0, y = 0, z = 0;

		protected Biome biome 	= Biome.PLAINS;
		protected Chunk chunk 	= new Chunk(0, 0, 0, null);

		protected BlockData blockData;

		public Block()
		{
			blockData = new BlockData();
		}

		public virtual Voxel getType()	//	This block's type
		{
			return Voxel.VOID;
		}

		public virtual ModelType getModelType()	//	This block's model type
		{
			return ModelType.CUBE;
		}

		public virtual CachedModel getModelData()	//	This block's custom model data
		{
			return null;
		}

		public virtual bool isPassable() //	Whether this block can be collided with
		{
			return false;
		}

		public virtual bool isTransparent()	//	Whether this block culls other blocks
		{
			return false;
		}

		public virtual bool isCombinable()	//	Whether this block merges with others of its voxel
		{
			return false;
		}

		public virtual bool isLiquid()	//	Whether this block is a liquid
		{
			return false;
		}

		public virtual bool isFlammable()	//	Whether this block can catch on fire
		{
			return false;
		}

		public virtual bool canBurn()	//	Whether this block can burn away
		{
			return false;
		}

		public virtual Position getOffset()	//	This block's mesh offset
		{
			return new Position(0.0f, 0.0f, 0.0f);
		}

		public virtual Position getViewOffset()	//	This block's view model offset
		{
			return new Position(0.0f, 0.0f, 0.0f);
		}

		public virtual Coord2D getTexture(Direction _face)	//	This block's texture coordinates
		{
			switch (_face)
			{
				case Direction.NORTH:
					return new Coord2D(0, 0);

				case Direction.EAST:
					return new Coord2D(0, 0);

				case Direction.SOUTH:
					return new Coord2D(0, 0);

				case Direction.WEST:
					return new Coord2D(0, 0);

				case Direction.ABOVE:
					return new Coord2D(0, 0);

				case Direction.BELOW:
					return new Coord2D(0, 0);

				default:
					return new Coord2D(0, 0);
			}
		}

		public virtual int getTextureRotation(Direction _face)	//	This block's texture rotation by face
		{
			switch (_face)
			{
				case Direction.NORTH:
					return 0;

				case Direction.EAST:
					return 0;

				case Direction.SOUTH:
					return 0;

				case Direction.WEST:
					return 0;

				case Direction.ABOVE:
					return 0;

				case Direction.BELOW:
					return 0;

				default:
					return 0;
			}
		}

		//	Called when this block is updated by a neighboring block changing
		public virtual void Update()
		{

		}

		//	Called when this block's data has been set
		public virtual void BlockDataUpdate()
		{

		}

		//	Called when the block is about to be rendered
		public virtual void OnPreRender()
		{

		}

		//	Called when this block is removed
		public virtual void OnRemove()
		{

		}

		//	called when this block is placed
		public virtual void OnPlace()
		{

		}

	//	Immutable methods
		public bool isValid()
		{
			return (chunk != null && chunk.getVoxelObject() != null);
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
			return x + chunk.getWorldX();
		}

		public int getWorldY()
		{
			return y + chunk.getWorldY();
		}

		public int getWorldZ()
		{
			return z + chunk.getWorldZ();
		}

		public void setLocation(int _x, int _y, int _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}

		public void setLocation(Coord3D _coord)
		{
			this.setLocation(_coord.x, _coord.y, _coord.z);
		}

		public void setLocation(Location _location)
		{
			this.setLocation((int)_location.x, (int)_location.y, (int)_location.z);
		}

		public void setChunk(Chunk _chunk)	//	Use carefully!
		{
			chunk = _chunk;
		}

		public void setBiome(Biome _biome)
		{
			biome = _biome;
		}

		public void setBlockData(BlockData _blockData)
		{
			blockData = _blockData;
			if (isValid()) { BlockDataUpdate(); }
		}

		public BlockData getBlockData()
		{
			return blockData;
		}

		public Chunk getChunk()
		{
			return chunk;
		}

		public int getSeed()
		{
			return (int)(getWorldX() ^ 2 + getWorldY() ^ 5 * getWorldZ()) ^ 7.GetHashCode();
		}

		public System.Random getRandom(int _seedModifier = 0, bool _multiply = true)
		{
			if (_multiply)
			{
				return new System.Random(this.getSeed() * (1 + _seedModifier));
			}
			else
			{
				return new System.Random(this.getSeed() + _seedModifier);
			}
		}

		public Block[] getNeighbors()
		{
			return new Block[]
			{
				chunk.at(x, y, z + 1),
				chunk.at(x + 1, y, z),
				chunk.at(x, y, z - 1),
				chunk.at(x - 1, y, z),
				chunk.at(x, y + 1, z),
				chunk.at(x, y - 1, z)
			};
		}

		public bool isOnBorder()
		{
			if (x == Constants.CHUNK_SIZE - 1 || y == Constants.CHUNK_SIZE - 1 || z == Constants.CHUNK_SIZE - 1 || x == 0 || y == 0 || z == 0)
			{
				return true;
			}

			return false;
		}

		public bool isTouchingBorder(Direction _dir)
		{
			return this.isTouchingBorder((int)_dir);
		}

		public bool isTouchingBorder(int _dir)
		{
			switch (_dir)
			{
				case 0:
					return (z == Constants.CHUNK_SIZE - 1);

				case 1:
					return (x == Constants.CHUNK_SIZE - 1);

				case 2:
					return (z == 0);

				case 3:
					return (x == 0);

				case 4:
					return (y == Constants.CHUNK_SIZE - 1);

				case 5:
					return (y == 0);
			}

			return false;
		}

		public Block getRelative(Direction _face)
		{
			return this.getRelative((int)_face);
		}

		public Block getRelative(int _face)
		{
			switch (_face)
			{
				case 0:
					return chunk.at(x, y, z + 1);

				case 1:
					return chunk.at(x + 1, y, z);

				case 2:
					return chunk.at(x, y, z - 1);

				case 3:
					return chunk.at(x - 1, y, z);

				case 4:
					return chunk.at(x, y + 1, z);

				case 5:
					return chunk.at(x, y - 1, z);

				default:
					return Voxel.VOID.toBlock();
			}
		}

		public bool isCulled()
		{
			for (int i = 0; i < 6; i++)
			{
				if (this.isFaceCulled(i) == false)
				{
					return false;
				}
			}

			return true;	//	Default to culled
		}

		public bool isFaceCulled(Direction _face)
		{
			return this.isFaceCulled((int)_face);
		}

		public bool isFaceCulled(int _face)
		{
			Block block;
			bool culled = true;	//	Default to being culled

			// if (isTouchingBorder(_face) == true && chunk.relativeChunk(_face) == null)
			// {
			// 	return true;
			// }

			switch (_face)
			{
				case 0:
					block = chunk.at(x, y, z + 1);
					if (block.isTransparent() == true)
					{
						culled = false;

						if (this.isCombinable() == true)
						{
							if (block.getType() == this.getType())
							{
								culled = true;
							}
						}
					}
					break;

				case 1:
					block = chunk.at(x + 1, y, z);
					if (block.isTransparent() == true)
					{
						culled = false;

						if (this.isCombinable() == true)
						{
							if (block.getType() == this.getType())
							{
								culled = true;
							}
						}
					}
					break;

				case 2:
					block = chunk.at(x, y, z - 1);
					if (block.isTransparent() == true)
					{
						culled = false;

						if (this.isCombinable() == true)
						{
							if (block.getType() == this.getType())
							{
								culled = true;
							}
						}
					}
					break;

				case 3:
					block = chunk.at(x - 1, y, z);
					if (block.isTransparent() == true)
					{
						culled = false;

						if (this.isCombinable() == true)
						{
							if (block.getType() == this.getType())
							{
								culled = true;
							}
						}
					}
					break;

				case 4:
					block = chunk.at(x, y + 1, z);
					if (block.isTransparent() == true)
					{
						culled = false;

						if (this.isCombinable() == true)
						{
							if (block.getType() == this.getType())
							{
								culled = true;
							}
						}
					}
					break;

				case 5:
					block = chunk.at(x, y - 1, z);
					if (block.isTransparent() == true)
					{
						culled = false;

						if (this.isCombinable() == true)
						{
							if (block.getType() == this.getType())
							{
								culled = true;
							}
						}
					}
					break;
			}

			return culled;
		}

		public bool isBlocked()
		{
			for (int i = 0; i < 6; i++)
			{
				if (this.isFaceBlocked(i) == false)
				{
					return false;
				}
			}

			return true;	//	Default to blocked
		}

		public bool isFaceBlocked(Direction _face)
		{
			return this.isFaceBlocked((int)_face);
		}

		public bool isFaceBlocked(int _face)
		{
			Block block;
			bool blocked = true;	//	Default to being blocked

			// if (isTouchingBorder(_face) == true && chunk.relativeChunk(_face) == null)
			// {
			// 	return true;
			// }

			switch (_face)
			{
				case 0:
					block = chunk.at(x, y, z + 1);
					if (block.isPassable() == true)
					{
						blocked = false;
					}
					return blocked;

				case 1:
					block = chunk.at(x + 1, y, z);
					if (block.isPassable() == true)
					{
						blocked = false;
					}
					return blocked;

				case 2:
					block = chunk.at(x, y, z - 1);
					if (block.isPassable() == true)
					{
						blocked = false;
					}
					return blocked;

				case 3:
					block = chunk.at(x - 1, y, z);
					if (block.isPassable() == true)
					{
						blocked = false;
					}
					return blocked;

				case 4:
					block = chunk.at(x, y + 1, z);
					if (block.isPassable() == true)
					{
						blocked = false;
					}
					return blocked;

				case 5:
					block = chunk.at(x, y - 1, z);
					if (block.isPassable() == true)
					{
						blocked = false;
					}
					return blocked;
			}

			return blocked;
		}
	}
}