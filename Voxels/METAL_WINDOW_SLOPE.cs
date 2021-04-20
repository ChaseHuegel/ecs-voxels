using System;

namespace Swordfish
{
    public class METAL_WINDOW_SLOPE : Block
    {
		public METAL_WINDOW_SLOPE()
		{
			blockData = new Orientated();
			((Orientated)blockData).setOrientation(Direction.BELOW);
		}

		public override Voxel getType()
		{
			return Voxel.METAL_WINDOW_SLOPE;
		}

		public override ModelType getModelType()
		{
			return ModelType.SLOPE;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override Coord2D getTexture(Direction _face)
		{
			int row = 1;

			if (_face == Direction.EAST || _face == Direction.WEST)
			{
				row = 3;
			}

			return new Coord2D(2, row);
		}
	}
}