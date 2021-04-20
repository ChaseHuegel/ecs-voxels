using System;

namespace Swordfish
{
    public class SHIP_MONITOR : Block
    {
		public override Voxel getType()
		{
			return Voxel.SHIP_MONITOR;
		}

		public override Coord2D getTexture(Direction _face)
		{
			switch (_face)
			{
				case Direction.ABOVE:
					return new Coord2D(6, 0);

				case Direction.BELOW:
					return new Coord2D(5, 0);

				default:
					return new Coord2D(3, 0);
			}
		}
	}
}