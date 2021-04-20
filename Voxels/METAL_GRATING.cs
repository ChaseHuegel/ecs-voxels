using System;

namespace Swordfish
{
    public class METAL_GRATING : Block
    {
		public override Voxel getType()
		{
			return Voxel.METAL_GRATING;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(7, 0);
		}
	}
}