using System;

namespace Swordfish
{
    public class VOID : Block
    {
		public override Voxel getType()
		{
			return Voxel.VOID;
		}

		public override bool isPassable()
		{
			return true;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(0, 0);
		}

		public override int getTextureRotation(Direction _face)
		{
			return 0;
		}
	}
}