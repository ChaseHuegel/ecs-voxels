using System;

namespace Swordfish
{
namespace items
{
    public class DRILL : Tool
    {
		public Voxel blueprint = Voxel.VOID;

		public override ItemType getType()
		{
			return ItemType.DRILL;
		}

		public override string getName()
		{
			return "Drill";
		}

		public override CachedImage getImageData()
		{
			return ResourceManager.GetImage("item.drill").image;
		}

		public Voxel getBlueprint()
		{
			return blueprint;
		}

		public void setBlueprint(Voxel _voxel)
		{
			blueprint = _voxel;
		}
	}
}
}