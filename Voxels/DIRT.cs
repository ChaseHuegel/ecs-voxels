using System;

namespace Swordfish
{
    public class DIRT : Block
    {
		public override Voxel getType()
		{
			return Voxel.DIRT;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(12, 1);
		}
	}
}