using System;

namespace Swordfish
{
    public class INTERFACE_PANEL : Block
    {
		public INTERFACE_PANEL()
		{
			blockData = new Rotatable();
		}

		public override Voxel getType()
		{
			return Voxel.INTERFACE_PANEL;
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
			int index = getRandom().Next(3) + 1;

			switch (_face)
			{
				case Direction.ABOVE:
					return new Coord2D(5, 0);

				case Direction.BELOW:
					return new Coord2D(5, 0);

				case Direction.SOUTH:
					return new Coord2D(6, index);

				default:
					return new Coord2D(6, 0);
			}
		}
	}
}