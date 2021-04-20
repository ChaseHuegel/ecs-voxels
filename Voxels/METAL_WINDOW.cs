using System;

namespace Swordfish
{
    public class METAL_WINDOW : Block
    {
		public bool[,] neighborMatch;

		public override Voxel getType()
		{
			return Voxel.METAL_WINDOW;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override bool isCombinable()
		{
			return true;
		}

		public override void OnPreRender()
		{
			Block[] neighborBlocks = getNeighbors();

			neighborMatch = new bool[neighborBlocks.Length, neighborBlocks.Length];
			for (int n = 0; n < neighborBlocks.Length; n++)
			{
				for (int i = 0; i < neighborBlocks.Length; i++) { neighborMatch[n, i] = (neighborBlocks[i].getType() == getType() && neighborBlocks[i].isFaceCulled( ((Direction)n) ) == false && neighborBlocks[i].isFaceBlocked( ((Direction)n) ) == false); }
			}
		}

		public override Coord2D getTexture(Direction _face)
		{
			int column = 0;
			int row = 8;

			if (chunk.getVoxelObject() != null && isFaceCulled(_face) == false && isFaceBlocked(_face) == false)
			{
				switch (_face)
				{
				case Direction.BELOW:
				case Direction.ABOVE:
					if (neighborMatch[(int)_face, (int)Direction.NORTH] == true)
					{
						row += 2;

						if (neighborMatch[(int)_face, (int)Direction.SOUTH] == false) { row += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.SOUTH] == true) { row += 1; }
					}

					if (neighborMatch[(int)_face, (int)Direction.EAST] == true)
					{
						column += 1;

						if (neighborMatch[(int)_face, (int)Direction.WEST] == true) { column += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.WEST] == true) { column += 3; }
					}
					break;

				case Direction.NORTH:
				case Direction.SOUTH:
					if (neighborMatch[(int)_face, (int)Direction.ABOVE] == true)
					{
						row += 2;

						if (neighborMatch[(int)_face, (int)Direction.BELOW] == false) { row += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.BELOW] == true) { row += 1; }
					}

					if (neighborMatch[(int)_face, (int)Direction.EAST] == true)
					{
						column += 1;

						if (neighborMatch[(int)_face, (int)Direction.WEST] == true) { column += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.WEST] == true) { column += 3; }
					}
					break;

				case Direction.WEST:
				case Direction.EAST:
					if (neighborMatch[(int)_face, (int)Direction.ABOVE] == true)
					{
						row += 2;

						if (neighborMatch[(int)_face, (int)Direction.BELOW] == false) { row += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.BELOW] == true) { row += 1; }
					}

					if (neighborMatch[(int)_face, (int)Direction.NORTH] == true)
					{
						column += 1;

						if (neighborMatch[(int)_face, (int)Direction.SOUTH] == true) { column += 1; }
					}
					else
					{
						if (neighborMatch[(int)_face, (int)Direction.SOUTH] == true) { column += 3; }
					}
					break;
				}
			}

			return new Coord2D(column, row);
		}
	}
}