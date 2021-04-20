using System;

namespace Swordfish
{
    public class VENT_PANEL : Block
    {
		public override Voxel getType()
		{
			return Voxel.VENT_PANEL;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(5, 1);
		}
	}
}