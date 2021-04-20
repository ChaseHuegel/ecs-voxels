using System;

namespace Swordfish
{
    public class CAUTION_PANEL : Block
    {
		public override Voxel getType()
		{
			return Voxel.CAUTION_PANEL;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(8, 0);
		}
	}
}