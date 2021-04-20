using System;

namespace Swordfish
{
    public class ASTEROID_ICE : Block
    {
		public override Voxel getType()
		{
			return Voxel.ASTEROID_ICE;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(11, 1);
		}
	}
}