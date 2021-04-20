using System;

namespace Swordfish
{
    public class ASTEROID_ROCK : Block
    {
		public override Voxel getType()
		{
			return Voxel.ASTEROID_ROCK;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(11, 0);
		}
	}
}