using System;

namespace Swordfish
{
    public class METAL_SLAB : Block
    {
		public METAL_SLAB()
		{
			blockData = new Orientated();
		}

		public override Voxel getType()
		{
			return Voxel.METAL_SLAB;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override ModelType getModelType()
		{
			return ModelType.CUBE_HALF;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(0, 4);
		}
	}
}