using System;

namespace Swordfish
{
    public class CAUTION_PANEL_SLOPE : Block
    {
		public CAUTION_PANEL_SLOPE()
		{
			blockData = new Orientated();
			((Orientated)blockData).setOrientation(Direction.BELOW);
		}

		public override Voxel getType()
		{
			return Voxel.CAUTION_PANEL_SLOPE;
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
			return new Coord2D(8, 0);
		}
	}
}