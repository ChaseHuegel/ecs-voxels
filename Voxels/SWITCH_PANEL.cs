using System;

namespace Swordfish
{
    public class SWITCH_PANEL : Block
    {
		public SWITCH_PANEL()
		{
			blockData = new Rotatable();
		}

		public override Voxel getType()
		{
			return Voxel.SWITCH_PANEL;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override ModelType getModelType()
		{
			return ModelType.CUSTOM_CUBE;
		}

		public override Coord2D getTexture(Direction _face)
		{
			switch (_face)
			{
				case Direction.ABOVE:
					return new Coord2D(5, 0);

				case Direction.BELOW:
					return new Coord2D(5, 0);

				case Direction.SOUTH:
					int index = getRandom().Next(4);
					return new Coord2D(7 + index, 2);

				default:
					return new Coord2D(6, 0);
			}
		}
	}
}