using System;

namespace Swordfish
{
    public class METAL_BARS : Block
    {
		public override Voxel getType()
		{
			return Voxel.METAL_BARS;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(9, 0);
		}
	}
}