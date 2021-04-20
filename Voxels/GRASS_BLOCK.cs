using System;

namespace Swordfish
{
    public class GRASS_BLOCK : Block
    {
		public GRASS_BLOCK()
		{
			blockData.color = new UnityEngine.Color(0.35f, 1.0f, 0.35f);
		}

		public override Voxel getType()
		{
			return Voxel.GRASS_BLOCK;
		}

		public override Coord2D getTexture(Direction _face)
		{
			return new Coord2D(12, 0);
		}
	}
}