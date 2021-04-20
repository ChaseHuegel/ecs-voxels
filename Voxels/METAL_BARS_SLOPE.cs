using System;

namespace Swordfish
{
    public class METAL_BARS_SLOPE : Block
    {
		public METAL_BARS_SLOPE()
		{
			blockData = new Orientated();
			((Orientated)blockData).setOrientation(Direction.BELOW);
		}

		public override Voxel getType()
		{
			return Voxel.METAL_BARS_SLOPE;
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
			int row = 0;

			if (_face == Direction.EAST || _face == Direction.WEST)
			{
				row = 1;
			}

			return new Coord2D(9, row);
		}
	}
}