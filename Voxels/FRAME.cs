using System;

namespace Swordfish
{
    public class FRAME : Block
    {
		public override Voxel getType()
		{
			return Voxel.FRAME;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(4, 2);
		}
	}
}