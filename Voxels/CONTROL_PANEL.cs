using System;

namespace Swordfish
{
    public class CONTROL_PANEL : Block
    {
		public override Voxel getType()
		{
			return Voxel.CONTROL_PANEL;
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
					return new Coord2D(4, 0);
			}
		}
	}
}