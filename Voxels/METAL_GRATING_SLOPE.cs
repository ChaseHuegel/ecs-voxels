using System;

namespace Swordfish
{
    public class METAL_GRATING_SLOPE : Block
    {
		public METAL_GRATING_SLOPE()
		{
			blockData = new Orientated();
			((Orientated)blockData).setOrientation(Direction.BELOW);
		}

		public override Voxel getType()
		{
			return Voxel.METAL_GRATING_SLOPE;
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
			return new Coord2D(7, 0);
		}
	}
}